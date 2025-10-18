using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace HRManagement.Pages
{
    public partial class Departments : System.Web.UI.Page
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["HRConnectionString"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            // Check authentication
            if (Session["Username"] == null)
            {
                Response.Redirect("~/pages/users/Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadDepartments();
            }
        }

        /// <summary>
        /// Load all departments with employee count
        /// </summary>
        private void LoadDepartments()
        {
            try
            {
                string query = @"SELECT d.DepartmentID, d.DepartmentName, d.Description,
                               COUNT(e.EmployeeID) AS EmployeeCount
                               FROM Departments d
                               LEFT JOIN Employees e ON d.DepartmentID = e.DepartmentID AND e.IsActive = 1
                               WHERE d.IsActive = 1
                               GROUP BY d.DepartmentID, d.DepartmentName, d.Description
                               ORDER BY d.DepartmentName";

                using (SqlConnection conn = new SqlConnection(connectionString))
                using (SqlDataAdapter adapter = new SqlDataAdapter(query, conn))
                {
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    gvDepartments.DataSource = dt;
                    gvDepartments.DataBind();
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Error loading departments: " + ex.Message, "danger");
            }
        }

        /// <summary>
        /// Load employees for selected department
        /// </summary>
        private void LoadDepartmentEmployees(int departmentId)
        {
            try
            {
                string query = @"SELECT e.EmployeeID, 
                               e.FirstName + ' ' + e.LastName AS FullName,
                               r.RoleName, e.DateJoined,
                               d.DepartmentName
                               FROM Employees e
                               LEFT JOIN Roles r ON e.RoleID = r.RoleID
                               LEFT JOIN Departments d ON e.DepartmentID = d.DepartmentID
                               WHERE e.DepartmentID = @DepartmentID AND e.IsActive = 1
                               ORDER BY e.FirstName, e.LastName";

                using (SqlConnection conn = new SqlConnection(connectionString))
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@DepartmentID", departmentId);
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);

                        if (dt.Rows.Count > 0)
                        {
                            lblDepartmentTitle.Text = dt.Rows[0]["DepartmentName"].ToString() + " Employees";
                            rptrDepartmentEmployees.DataSource = dt;
                            rptrDepartmentEmployees.DataBind();

                            pnlNoSelection.Visible = false;
                            pnlDepartmentEmployees.Visible = true;
                        }
                        else
                        {
                            lblDepartmentTitle.Text = "No Employees Found";
                            pnlNoSelection.Visible = true;
                            pnlDepartmentEmployees.Visible = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Error loading department employees: " + ex.Message, "danger");
            }
        }

        protected void gvDepartments_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int departmentId = Convert.ToInt32(e.CommandArgument);

            switch (e.CommandName)
            {
                case "ViewEmployees":
                    LoadDepartmentEmployees(departmentId);
                    break;
                case "EditDepartment":
                    EditDepartment(departmentId);
                    break;
                case "DeleteDepartment":
                    DeleteDepartment(departmentId);
                    break;
            }
        }

        protected void rptrDepartmentEmployees_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "ViewProfile")
            {
                int employeeId = Convert.ToInt32(e.CommandArgument);
                Response.Redirect($"~/pages/EmployeeProfile.aspx?id={employeeId}");
            }
        }

        protected void btnSaveDepartment_Click(object sender, EventArgs e)
        {
            try
            {
                // Optional debug
                System.Diagnostics.Debug.WriteLine($"Hidden DepartmentID: '{hdnDepartmentID.Value}'");

                if (!ValidateDepartmentForm())
                    return;

                if (string.IsNullOrEmpty(hdnDepartmentID.Value))
                {
                    // Add new department
                    AddDepartment();
                }
                else
                {
                    // Edit/update existing department
                    UpdateDepartment();
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Error saving department: " + ex.Message, "danger");
                System.Diagnostics.Debug.WriteLine("Save error: " + ex.ToString());
            }
        }

        private void AddDepartment()
        {
            string query = @"INSERT INTO Departments (DepartmentName, Description) 
                           VALUES (@DepartmentName, @Description)";

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@DepartmentName", txtDepartmentName.Text.Trim());
                cmd.Parameters.AddWithValue("@Description",
                    string.IsNullOrEmpty(txtDescription.Text) ? DBNull.Value : (object)txtDescription.Text.Trim());

                conn.Open();
                int result = cmd.ExecuteNonQuery();

                if (result > 0)
                {
                    ClearForm();
                    LoadDepartments();
                    ShowMessage("Department added successfully!", "success");

                    ScriptManager.RegisterStartupScript(this, GetType(), "hideModal",
                        "var modal = bootstrap.Modal.getInstance(document.getElementById('addDepartmentModal')); modal.hide();", true);
                }
            }
        }

        private void UpdateDepartment()
        {
            string query = @"UPDATE Departments 
                           SET DepartmentName = @DepartmentName, Description = @Description
                           WHERE DepartmentID = @DepartmentID";

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@DepartmentID", hdnDepartmentID.Value);
                cmd.Parameters.AddWithValue("@DepartmentName", txtDepartmentName.Text.Trim());
                cmd.Parameters.AddWithValue("@Description",
                    string.IsNullOrEmpty(txtDescription.Text) ? DBNull.Value : (object)txtDescription.Text.Trim());

                conn.Open();
                int result = cmd.ExecuteNonQuery();

                if (result > 0)
                {
                    ClearForm();
                    LoadDepartments();
                    ShowMessage("Department updated successfully!", "success");

                    ScriptManager.RegisterStartupScript(this, GetType(), "hideModal",
                        "var modal = bootstrap.Modal.getInstance(document.getElementById('addDepartmentModal')); modal.hide();", true);
                }
            }
        }

        private void EditDepartment(int departmentId)
        {
            try
            {
                string query = "SELECT * FROM Departments WHERE DepartmentID = @DepartmentID AND IsActive = 1";

                using (SqlConnection conn = new SqlConnection(connectionString))
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@DepartmentID", departmentId);
                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Assign hidden field
                            hdnDepartmentID.Value = reader["DepartmentID"].ToString();

                            // Populate form fields
                            txtDepartmentName.Text = reader["DepartmentName"].ToString();
                            txtDescription.Text = reader["Description"]?.ToString() ?? "";

                            // Update modal labels/buttons
                            lblModalTitle.Text = "Edit Department";
                            btnSaveDepartment.Text = "Update Department";

                            // Debug (optional)
                            System.Diagnostics.Debug.WriteLine("Editing DepartmentID: " + hdnDepartmentID.Value);

                            // FIXED: Use ClientScript instead of ScriptManager for better reliability
                            string script = @"
                        document.addEventListener('DOMContentLoaded', function() {
                            var modalEl = document.getElementById('addDepartmentModal');
                            var modal = new bootstrap.Modal(modalEl);
                            modal.show();
                        });";

                            ClientScript.RegisterStartupScript(this.GetType(), "showEditModal", script, true);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Error loading department details: " + ex.Message, "danger");
            }
        }

        private void DeleteDepartment(int departmentId)
        {
            try
            {
                // Check if department has employees
                string checkQuery = "SELECT COUNT(*) FROM Employees WHERE DepartmentID = @DepartmentID AND IsActive = 1";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@DepartmentID", departmentId);
                        conn.Open();
                        int employeeCount = Convert.ToInt32(checkCmd.ExecuteScalar());

                        if (employeeCount > 0)
                        {
                            ShowMessage("Cannot delete department. It has active employees assigned to it.", "warning");
                            return;
                        }
                    }

                    // Soft delete - set IsActive to 0
                    string deleteQuery = "UPDATE Departments SET IsActive = 0 WHERE DepartmentID = @DepartmentID";

                    using (SqlCommand deleteCmd = new SqlCommand(deleteQuery, conn))
                    {
                        deleteCmd.Parameters.AddWithValue("@DepartmentID", departmentId);
                        int result = deleteCmd.ExecuteNonQuery();

                        if (result > 0)
                        {
                            LoadDepartments();
                            ShowMessage("Department deleted successfully!", "success");

                            pnlNoSelection.Visible = true;
                            pnlDepartmentEmployees.Visible = false;
                            lblDepartmentTitle.Text = "Department Employees";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Error deleting department: " + ex.Message, "danger");
            }
        }

        private bool ValidateDepartmentForm()
        {
            if (string.IsNullOrEmpty(txtDepartmentName.Text.Trim()))
            {
                ShowMessage("Department name is required.", "danger");
                return false;
            }

            if (IsDepartmentNameExists(txtDepartmentName.Text.Trim(), hdnDepartmentID.Value))
            {
                ShowMessage("Department name already exists. Please use a different name.", "danger");
                return false;
            }

            return true;
        }

        private bool IsDepartmentNameExists(string departmentName, string departmentId)
        {
            string query = "SELECT COUNT(*) FROM Departments WHERE DepartmentName = @DepartmentName AND IsActive = 1";

            if (!string.IsNullOrEmpty(departmentId))
            {
                query += " AND DepartmentID != @DepartmentID";
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@DepartmentName", departmentName);
                if (!string.IsNullOrEmpty(departmentId))
                {
                    cmd.Parameters.AddWithValue("@DepartmentID", departmentId);
                }

                conn.Open();
                int count = Convert.ToInt32(cmd.ExecuteScalar());
                return count > 0;
            }
        }

        private void ClearForm()
        {
            hdnDepartmentID.Value = "";
            txtDepartmentName.Text = "";
            txtDescription.Text = "";
            lblModalTitle.Text = "Add New Department";
            btnSaveDepartment.Text = "Save Department";

            pnlMessage.Visible = false;
        }

        private void ShowMessage(string message, string type = "info")
        {
            lblMessage.Text = message;
            pnlMessage.CssClass = $"alert alert-{type}";
            pnlMessage.Visible = true;
        }
    }
}