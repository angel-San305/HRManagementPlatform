using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace HRManagement.Pages
{
    public partial class Employees : System.Web.UI.Page
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["HRConnectionString"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Username"] == null)
            {
                Response.Redirect("~/pages/users/Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadDropDowns();
                SetDefaultDate();
            }

            // Always reload employees on every request
            LoadEmployees();
        }


        protected void txtSearch_TextChanged(object sender, EventArgs e)
        {
            LoadEmployees(); // reload employees based on what was typed in txtSearch
        }


        /// <summary>
        /// Load dropdown lists with data
        /// </summary>
        private void LoadDropDowns()
        {
            try
            {
                // Load Departments
                LoadDepartments();
                LoadRoles();
            }
            catch (Exception ex)
            {
                ShowMessage("Error loading dropdown data: " + ex.Message, "danger");
            }
        }

        private void LoadDepartments()
        {
            string query = "SELECT DepartmentID, DepartmentName FROM Departments WHERE IsActive = 1 ORDER BY DepartmentName";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlDataAdapter adapter = new SqlDataAdapter(query, conn))
                {
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    // For Add Employee form only
                    ddlDepartment.DataSource = dt;
                    ddlDepartment.DataTextField = "DepartmentName";
                    ddlDepartment.DataValueField = "DepartmentID";
                    ddlDepartment.DataBind();
                    ddlDepartment.Items.Insert(0, new ListItem("Select Department", ""));
                }
            }
        }

        private void LoadRoles()
        {
            string query = "SELECT RoleID, RoleName FROM Roles WHERE IsActive = 1 ORDER BY RoleName";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlDataAdapter adapter = new SqlDataAdapter(query, conn))
                {
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    // For Add Employee form only
                    ddlRole.DataSource = dt;
                    ddlRole.DataTextField = "RoleName";
                    ddlRole.DataValueField = "RoleID";
                    ddlRole.DataBind();
                    ddlRole.Items.Insert(0, new ListItem("Select Role", ""));
                }
            }
        }

        /// <summary>
        /// Load employees with filters
        /// </summary>
        private void LoadEmployees()
        {
            try
            {
                StringBuilder query = new StringBuilder();
                query.Append(@"SELECT e.EmployeeID,
                      e.FirstName + ' ' + e.LastName AS FullName,
                      e.Email, e.PhoneNumber,
                      d.DepartmentName, r.RoleName,
                      e.Salary, e.DateJoined, e.Address
                      FROM Employees e
                      LEFT JOIN Departments d ON e.DepartmentID = d.DepartmentID
                      LEFT JOIN Roles r ON e.RoleID = r.RoleID
                      WHERE e.IsActive = 1");

                var parameters = new List<SqlParameter>();

                // Universal search: Name, Email, Department, Role, DateJoined
                if (!string.IsNullOrEmpty(txtSearch.Text.Trim()))
                {
                    query.Append(@" AND (e.FirstName LIKE @Search 
                                 OR e.LastName LIKE @Search 
                                 OR e.Email LIKE @Search
                                 OR d.DepartmentName LIKE @Search
                                 OR r.RoleName LIKE @Search
                                 OR CONVERT(varchar, e.DateJoined, 23) LIKE @Search)");
                    parameters.Add(new SqlParameter("@Search", "%" + txtSearch.Text.Trim() + "%"));
                }

                query.Append(" ORDER BY e.FirstName, e.LastName");

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(query.ToString(), conn))
                    {
                        cmd.Parameters.AddRange(parameters.ToArray());
                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);
                            gvEmployees.DataSource = dt;
                            gvEmployees.DataBind();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Error loading employees: " + ex.Message, "danger");
            }
        }


        protected void btnSearch_Click(object sender, EventArgs e)
        {
            LoadEmployees();
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            txtSearch.Text = "";
            LoadEmployees();
        }


        protected void btnSaveEmployee_Click(object sender, EventArgs e)
        {
            try
            {
                if (ValidateEmployeeForm())
                {
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();
                        SqlTransaction transaction = conn.BeginTransaction();

                        try
                        {
                            // 1. Insert Employee
                            string employeeQuery = @"INSERT INTO Employees 
                                           (FirstName, LastName, Email, PhoneNumber, DepartmentID, RoleID, Salary, DateJoined, Address)
                                           OUTPUT INSERTED.EmployeeID
                                           VALUES (@FirstName, @LastName, @Email, @Phone, @DepartmentID, @RoleID, @Salary, @DateJoined, @Address)";

                            int newEmployeeId;
                            using (SqlCommand cmd = new SqlCommand(employeeQuery, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@FirstName", txtFirstName.Text.Trim());
                                cmd.Parameters.AddWithValue("@LastName", txtLastName.Text.Trim());
                                cmd.Parameters.AddWithValue("@Email", txtEmail.Text.Trim());
                                cmd.Parameters.AddWithValue("@Phone", string.IsNullOrEmpty(txtPhone.Text) ? DBNull.Value : (object)txtPhone.Text.Trim());
                                cmd.Parameters.AddWithValue("@DepartmentID", ddlDepartment.SelectedValue);
                                cmd.Parameters.AddWithValue("@RoleID", ddlRole.SelectedValue);
                                cmd.Parameters.AddWithValue("@Salary", string.IsNullOrEmpty(txtSalary.Text) ? DBNull.Value : (object)decimal.Parse(txtSalary.Text));
                                cmd.Parameters.AddWithValue("@DateJoined", DateTime.Parse(txtDateJoined.Text));
                                cmd.Parameters.AddWithValue("@Address", string.IsNullOrEmpty(txtAddress.Text) ? DBNull.Value : (object)txtAddress.Text.Trim());

                                newEmployeeId = (int)cmd.ExecuteScalar();
                            }

                            // 2. Create User Account for Employee
                            string userQuery = @"INSERT INTO Users (Username, Email, Password, Role, IsActive)
                                       OUTPUT INSERTED.UserID
                                       VALUES (@Username, @Email, @Password, 'Employee', 1)";

                            int newUserId;
                            string defaultPassword = "Welcome123"; // Default password
                            string username = txtEmail.Text.Trim().Split('@')[0]; // Use email prefix as username

                            using (SqlCommand cmd = new SqlCommand(userQuery, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@Username", username);
                                cmd.Parameters.AddWithValue("@Email", txtEmail.Text.Trim());
                                cmd.Parameters.AddWithValue("@Password", defaultPassword);

                                newUserId = (int)cmd.ExecuteScalar();
                            }

                            // 3. Link User to Employee
                            string linkQuery = "UPDATE Employees SET UserID = @UserID WHERE EmployeeID = @EmployeeID";
                            using (SqlCommand cmd = new SqlCommand(linkQuery, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@UserID", newUserId);
                                cmd.Parameters.AddWithValue("@EmployeeID", newEmployeeId);
                                cmd.ExecuteNonQuery();
                            }

                            transaction.Commit();

                            ClearForm();
                            LoadEmployees();
                            ShowMessage($"Employee added successfully! Login credentials: Username: {username}, Password: {defaultPassword}", "success");

                            ScriptManager.RegisterStartupScript(this, GetType(), "hideModal",
                                "$('#addEmployeeModal').modal('hide');", true);
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Error adding employee: " + ex.Message, "danger");
            }
        }

        protected void gvEmployees_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvEmployees.PageIndex = e.NewPageIndex;
            LoadEmployees();
        }

        protected void gvEmployees_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int employeeId = Convert.ToInt32(e.CommandArgument);

            switch (e.CommandName)
            {
                case "ViewProfile":
                    Response.Redirect($"~/pages/EmployeeProfile.aspx?id={employeeId}");
                    break;
                case "EditEmployee":
                    // For now, redirect to profile page for editing
                    Response.Redirect($"~/pages/EmployeeProfile.aspx?id={employeeId}&edit=true");
                    break;
                case "DeleteEmployee":
                    DeleteEmployee(employeeId);
                    break;
            }
        }

        protected void btnExportCSV_Click(object sender, EventArgs e)
        {
            try
            {
                DataTable dt = GetEmployeeData(); // ✅ gets your query results
                if (dt != null && dt.Rows.Count > 0)
                {
                    ExportToCSV(dt, "Employees_" + DateTime.Now.ToString("yyyyMMdd") + ".csv");
                }
                else
                {
                    ShowMessage("No data available to export.", "warning");
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Error exporting data: " + ex.Message, "danger");
            }
        }


        private void DeleteEmployee(int employeeId)
        {
            try
            {
                // Soft delete - set IsActive to 0
                string query = "UPDATE Employees SET IsActive = 0 WHERE EmployeeID = @EmployeeID";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@EmployeeID", employeeId);
                        conn.Open();
                        int result = cmd.ExecuteNonQuery();

                        if (result > 0)
                        {
                            LoadEmployees();
                            ShowMessage("Employee deleted successfully!", "success");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Error deleting employee: " + ex.Message, "danger");
            }
        }

        private DataTable GetEmployeeData()
        {
            StringBuilder query = new StringBuilder();
            query.Append(@"SELECT e.FirstName + ' ' + e.LastName AS FullName,
                   e.Email, e.PhoneNumber, d.DepartmentName, r.RoleName, 
                   e.Salary, e.DateJoined, e.Address
                   FROM Employees e
                   LEFT JOIN Departments d ON e.DepartmentID = d.DepartmentID
                   LEFT JOIN Roles r ON e.RoleID = r.RoleID
                   WHERE e.IsActive = 1");

            var parameters = new List<SqlParameter>();

            // Apply same search filter as GridView
            if (!string.IsNullOrEmpty(txtSearch.Text.Trim()))
            {
                query.Append(@" AND (e.FirstName LIKE @Search 
                         OR e.LastName LIKE @Search 
                         OR e.Email LIKE @Search
                         OR d.DepartmentName LIKE @Search
                         OR r.RoleName LIKE @Search
                         OR CONVERT(varchar, e.DateJoined, 23) LIKE @Search)");
                parameters.Add(new SqlParameter("@Search", "%" + txtSearch.Text.Trim() + "%"));
            }

            query.Append(" ORDER BY e.FirstName, e.LastName");

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query.ToString(), conn))
                {
                    cmd.Parameters.AddRange(parameters.ToArray());
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        return dt;
                    }
                }
            }
        }



        private void ExportToCSV(DataTable dt, string filename)
        {
            try
            {
                Response.Clear();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", $"attachment;filename={filename}");
                Response.Charset = "";
                Response.ContentType = "text/csv";

                StringBuilder sb = new StringBuilder();

                // Add column headers
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    sb.Append("\"" + dt.Columns[i].ColumnName.Replace("\"", "\"\"") + "\"");
                    if (i < dt.Columns.Count - 1)
                        sb.Append(",");
                }
                sb.AppendLine();

                // Add data rows
                foreach (DataRow row in dt.Rows)
                {
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        string value = row[i].ToString();

                        // Escape quotes by doubling them
                        value = value.Replace("\"", "\"\"");

                        sb.Append("\"" + value + "\"");

                        if (i < dt.Columns.Count - 1)
                            sb.Append(",");
                    }
                    sb.AppendLine();
                }

                Response.Output.Write(sb.ToString());
                Response.Flush();

                // ✅ Stop page from rendering extra HTML
                Response.SuppressContent = true;
                Response.End();
            }
            catch (Exception ex)
            {
                ShowMessage("Error exporting to CSV: " + ex.Message, "danger");
            }
        }

        private bool ValidateEmployeeForm()
        {
            if (string.IsNullOrEmpty(txtFirstName.Text.Trim()))
            {
                ShowMessage("First name is required.", "danger");
                return false;
            }

            if (string.IsNullOrEmpty(txtLastName.Text.Trim()))
            {
                ShowMessage("Last name is required.", "danger");
                return false;
            }

            if (string.IsNullOrEmpty(txtEmail.Text.Trim()))
            {
                ShowMessage("Email is required.", "danger");
                return false;
            }

            if (string.IsNullOrEmpty(ddlDepartment.SelectedValue))
            {
                ShowMessage("Department is required.", "danger");
                return false;
            }

            if (string.IsNullOrEmpty(ddlRole.SelectedValue))
            {
                ShowMessage("Role is required.", "danger");
                return false;
            }

            if (string.IsNullOrEmpty(txtDateJoined.Text))
            {
                ShowMessage("Date joined is required.", "danger");
                return false;
            }

            // Check if email already exists
            if (IsEmailExists(txtEmail.Text.Trim()))
            {
                ShowMessage("Email already exists. Please use a different email.", "danger");
                return false;
            }

            return true;
        }

        private bool IsEmailExists(string email)
        {
            string query = "SELECT COUNT(*) FROM Employees WHERE Email = @Email AND IsActive = 1";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Email", email);
                    conn.Open();
                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    return count > 0;
                }
            }
        }

        private void ClearForm()
        {
            txtFirstName.Text = "";
            txtLastName.Text = "";
            txtEmail.Text = "";
            txtPhone.Text = "";
            txtSalary.Text = "";
            txtAddress.Text = "";
            ddlDepartment.SelectedIndex = 0;
            ddlRole.SelectedIndex = 0;
            SetDefaultDate();
        }

        private void SetDefaultDate()
        {
            txtDateJoined.Text = DateTime.Today.ToString("yyyy-MM-dd");
        }

        private void ShowMessage(string message, string type = "info")
        {
            lblMessage.Text = message;
            pnlMessage.CssClass = $"alert alert-{type}";
            pnlMessage.Visible = true;
        }
    }
}