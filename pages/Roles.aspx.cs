using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace HRManagement.Pages
{
    public partial class Roles : System.Web.UI.Page
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
                LoadRoles();
            }
        }

        /// <summary>
        /// Load all roles with employee count
        /// </summary>
        private void LoadRoles()
        {
            try
            {
                string query = @"SELECT r.RoleID, r.RoleName, r.Description,
                               COUNT(e.EmployeeID) AS EmployeeCount
                               FROM Roles r
                               LEFT JOIN Employees e ON r.RoleID = e.RoleID AND e.IsActive = 1
                               WHERE r.IsActive = 1
                               GROUP BY r.RoleID, r.RoleName, r.Description
                               ORDER BY r.RoleName";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlDataAdapter adapter = new SqlDataAdapter(query, conn))
                    {
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);

                        gvRoles.DataSource = dt;
                        gvRoles.DataBind();
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Error loading roles: " + ex.Message, "danger");
            }
        }

        protected void gvRoles_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int roleId = Convert.ToInt32(e.CommandArgument);

            switch (e.CommandName)
            {
                case "EditRole":
                    EditRole(roleId);
                    break;
                case "DeleteRole":
                    DeleteRole(roleId);
                    break;
            }
        }

        protected void btnSaveRole_Click(object sender, EventArgs e)
        {
            try
            {
                if (ValidateRoleForm())
                {
                    if (string.IsNullOrEmpty(hdnRoleID.Value))
                    {
                        // Add new role
                        AddRole();
                    }
                    else
                    {
                        // Update existing role
                        UpdateRole();
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Error saving role: " + ex.Message, "danger");
            }
        }

        private void AddRole()
        {
            string query = @"INSERT INTO Roles (RoleName, Description) 
                   VALUES (@RoleName, @Description)";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@RoleName", txtRoleName.Text.Trim());
                    cmd.Parameters.AddWithValue("@Description",
                        string.IsNullOrEmpty(txtDescription.Text) ? DBNull.Value : (object)txtDescription.Text.Trim());

                    conn.Open();
                    int result = cmd.ExecuteNonQuery();

                    if (result > 0)
                    {
                        ClearForm();
                        LoadRoles();
                        ShowMessage("Role added successfully!", "success");

                        // FIXED: Use Bootstrap 5 syntax
                        Page.ClientScript.RegisterStartupScript(this.GetType(), "hideModal",
                            "var modal = bootstrap.Modal.getInstance(document.getElementById('addRoleModal')); modal.hide();", true);
                    }
                }
            }
        }

        private void UpdateRole()
        {
            string query = @"UPDATE Roles 
                   SET RoleName = @RoleName, Description = @Description
                   WHERE RoleID = @RoleID";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@RoleID", hdnRoleID.Value);
                    cmd.Parameters.AddWithValue("@RoleName", txtRoleName.Text.Trim());
                    cmd.Parameters.AddWithValue("@Description",
                        string.IsNullOrEmpty(txtDescription.Text) ? DBNull.Value : (object)txtDescription.Text.Trim());

                    conn.Open();
                    int result = cmd.ExecuteNonQuery();

                    if (result > 0)
                    {
                        ClearForm();
                        LoadRoles();
                        ShowMessage("Role updated successfully!", "success");

                        // FIXED: Use Bootstrap 5 syntax
                        Page.ClientScript.RegisterStartupScript(this.GetType(), "hideModal",
                            "var modal = bootstrap.Modal.getInstance(document.getElementById('addRoleModal')); modal.hide();", true);
                    }
                }
            }
        }

        private void EditRole(int roleId)
        {
            try
            {
                string query = "SELECT * FROM Roles WHERE RoleID = @RoleID";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@RoleID", roleId);
                        conn.Open();

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                hdnRoleID.Value = reader["RoleID"].ToString();
                                txtRoleName.Text = reader["RoleName"].ToString();
                                txtDescription.Text = reader["Description"].ToString();
                                lblModalTitle.Text = "Edit Role";
                                btnSaveRole.Text = "Update Role";

                                // FIXED: Use Bootstrap 5 syntax with timeout for reliability
                                string script = @"
                            setTimeout(function() {
                                var modalEl = document.getElementById('addRoleModal');
                                if (modalEl) {
                                    var modal = new bootstrap.Modal(modalEl, {
                                        backdrop: 'static',
                                        keyboard: false
                                    });
                                    modal.show();
                                }
                            }, 100);";

                                Page.ClientScript.RegisterStartupScript(this.GetType(), "showEditModal", script, true);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Error loading role details: " + ex.Message, "danger");
            }
        }
        private void DeleteRole(int roleId)
        {
            try
            {
                // Check if role has employees
                string checkQuery = "SELECT COUNT(*) FROM Employees WHERE RoleID = @RoleID AND IsActive = 1";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@RoleID", roleId);
                        conn.Open();
                        int employeeCount = Convert.ToInt32(checkCmd.ExecuteScalar());

                        if (employeeCount > 0)
                        {
                            ShowMessage("Cannot delete role. It has active employees assigned to it.", "warning");
                            return;
                        }
                    }

                    // Soft delete - set IsActive to 0
                    string deleteQuery = "UPDATE Roles SET IsActive = 0 WHERE RoleID = @RoleID";

                    using (SqlCommand deleteCmd = new SqlCommand(deleteQuery, conn))
                    {
                        deleteCmd.Parameters.AddWithValue("@RoleID", roleId);
                        int result = deleteCmd.ExecuteNonQuery();

                        if (result > 0)
                        {
                            LoadRoles();
                            ShowMessage("Role deleted successfully!", "success");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Error deleting role: " + ex.Message, "danger");
            }
        }

        private bool ValidateRoleForm()
        {
            if (string.IsNullOrEmpty(txtRoleName.Text.Trim()))
            {
                ShowMessage("Role name is required.", "danger");
                return false;
            }

            // Check if role name already exists (for new roles or different role)
            if (IsRoleNameExists(txtRoleName.Text.Trim(), hdnRoleID.Value))
            {
                ShowMessage("Role name already exists. Please use a different name.", "danger");
                return false;
            }

            return true;
        }

        private bool IsRoleNameExists(string roleName, string roleId)
        {
            string query = "SELECT COUNT(*) FROM Roles WHERE RoleName = @RoleName AND IsActive = 1";

            if (!string.IsNullOrEmpty(roleId))
            {
                query += " AND RoleID != @RoleID";
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@RoleName", roleName);
                    if (!string.IsNullOrEmpty(roleId))
                    {
                        cmd.Parameters.AddWithValue("@RoleID", roleId);
                    }

                    conn.Open();
                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    return count > 0;
                }
            }
        }

        private void ClearForm()
        {
            hdnRoleID.Value = "";
            txtRoleName.Text = "";
            txtDescription.Text = "";
            lblModalTitle.Text = "Add New Role";
            btnSaveRole.Text = "Save Role";
        }

        private void ShowMessage(string message, string type = "info")
        {
            lblMessage.Text = message;
            pnlMessage.CssClass = $"alert alert-{type}";
            pnlMessage.Visible = true;
        }
    }
}