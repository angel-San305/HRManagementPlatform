using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;

namespace HRManagement.Pages.Employee
{
    public partial class EmployeeAccount : System.Web.UI.Page
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["HRConnectionString"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            ValidationSettings.UnobtrusiveValidationMode = UnobtrusiveValidationMode.None;

            // Check if user is logged in
            if (Session["UserID"] == null || Session["Username"] == null)
            {
                Response.Redirect("~/Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadCurrentAccountInfo();
            }
        }

        private void LoadCurrentAccountInfo()
        {
            try
            {
                int userId = Convert.ToInt32(Session["UserID"]);

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "SELECT Username, Email FROM Users WHERE UserID = @UserID";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@UserID", userId);

                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        lblCurrentUsername.Text = reader["Username"].ToString();
                        lblCurrentEmail.Text = reader["Email"].ToString();
                    }

                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Error loading account information: " + ex.Message, "danger");
            }
        }

        protected void btnUpdateAccount_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            try
            {
                int userId = Convert.ToInt32(Session["UserID"]);
                bool updateSuccess = false;
                string successMessage = "";

                // Check if password is being changed
                bool changePassword = !string.IsNullOrWhiteSpace(txtCurrentPassword.Text);
                bool changeUsername = !string.IsNullOrWhiteSpace(txtNewUsername.Text);

                if (!changePassword && !changeUsername)
                {
                    ShowMessage("Please enter either a new username or password to update.", "warning");
                    return;
                }

                // Verify current password if changing password
                if (changePassword)
                {
                    if (!VerifyCurrentPassword(userId, txtCurrentPassword.Text))
                    {
                        ShowMessage("Current password is incorrect.", "danger");
                        return;
                    }

                    // Validate new password
                    if (txtNewPassword.Text.Length < 6)
                    {
                        ShowMessage("New password must be at least 6 characters long.", "warning");
                        return;
                    }
                }

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Update username if provided
                    if (changeUsername)
                    {
                        // Check if username already exists
                        if (UsernameExists(txtNewUsername.Text, userId))
                        {
                            ShowMessage("Username already exists. Please choose a different username.", "warning");
                            return;
                        }

                        string updateUsernameQuery = "UPDATE Users SET Username = @Username WHERE UserID = @UserID";
                        SqlCommand cmdUsername = new SqlCommand(updateUsernameQuery, conn);
                        cmdUsername.Parameters.AddWithValue("@Username", txtNewUsername.Text.Trim());
                        cmdUsername.Parameters.AddWithValue("@UserID", userId);

                        int rowsAffected = cmdUsername.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            updateSuccess = true;
                            successMessage = "Username updated successfully. ";
                            Session["Username"] = txtNewUsername.Text.Trim();
                        }
                    }

                    // Update password if provided
                    if (changePassword)
                    {
                        string updatePasswordQuery = "UPDATE Users SET Password = @Password WHERE UserID = @UserID";
                        SqlCommand cmdPassword = new SqlCommand(updatePasswordQuery, conn);
                        cmdPassword.Parameters.AddWithValue("@Password", txtNewPassword.Text);
                        cmdPassword.Parameters.AddWithValue("@UserID", userId);

                        int rowsAffected = cmdPassword.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            updateSuccess = true;
                            successMessage += "Password updated successfully.";
                        }
                    }
                }

                if (updateSuccess)
                {
                    ShowMessage(successMessage, "success");
                    ClearPasswordFields();
                    ClearUsernameFields();
                    LoadCurrentAccountInfo();
                }
                else
                {
                    ShowMessage("Failed to update account. Please try again.", "danger");
                }
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2627 || ex.Number == 2601) // Duplicate key error
                {
                    ShowMessage("Username already exists. Please choose a different username.", "warning");
                }
                else
                {
                    ShowMessage("Database error: " + ex.Message, "danger");
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Error updating account: " + ex.Message, "danger");
            }
        }

        private bool VerifyCurrentPassword(int userId, string currentPassword)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "SELECT Password FROM Users WHERE UserID = @UserID";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@UserID", userId);

                    conn.Open();
                    object result = cmd.ExecuteScalar();

                    if (result != null)
                    {
                        string storedPassword = result.ToString();
                        return storedPassword == currentPassword;
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Error verifying password: " + ex.Message, "danger");
            }

            return false;
        }

        private bool UsernameExists(string username, int currentUserId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "SELECT COUNT(*) FROM Users WHERE Username = @Username AND UserID != @UserID";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Username", username.Trim());
                    cmd.Parameters.AddWithValue("@UserID", currentUserId);

                    conn.Open();
                    int count = (int)cmd.ExecuteScalar();

                    return count > 0;
                }
            }
            catch
            {
                return true; // Return true to prevent update in case of error
            }
        }

        private void ShowMessage(string message, string type)
        {
            pnlMessage.Visible = true;
            lblMessage.Text = message;
            pnlMessage.CssClass = "alert alert-" + type;
        }

        private void ClearPasswordFields()
        {
            txtCurrentPassword.Text = string.Empty;
            txtNewPassword.Text = string.Empty;
            txtConfirmPassword.Text = string.Empty;
        }

        private void ClearUsernameFields()
        {
            txtNewUsername.Text = string.Empty;
            txtConfirmUsername.Text = string.Empty;
        }
    }
}