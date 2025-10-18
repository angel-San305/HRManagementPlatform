using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;

namespace HRManagement.Pages
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Clear any existing session
                Session.Clear();
                Session.Abandon();

                // Set focus to username field
                txtUsername.Focus();
            }
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                string username = txtUsername.Text.Trim();
                string password = txtPassword.Text.Trim();

                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                {
                    ShowMessage("Please enter both username and password.");
                    return;
                }

                DataRow user = ValidateUser(username, password);

                if (user != null)
                {
                    // Store user information in session
                    Session["UserID"] = user["UserID"];
                    Session["Username"] = user["Username"];
                    Session["UserRole"] = user["Role"];
                    Session["Email"] = user["Email"];

                    // Redirect based on role
                    string role = user["Role"].ToString();
                    if (role == "Admin" || role == "HR")
                    {
                        Response.Redirect("~/pages/Dashboard.aspx");
                    }
                    else if (role == "Employee")
                    {
                        // Get employee ID for this user
                        int employeeId = GetEmployeeIdByUserId(Convert.ToInt32(user["UserID"]));
                        if (employeeId > 0)
                        {
                            Session["EmployeeID"] = employeeId;
                            Response.Redirect("~/pages/employee/EmployeeDashboard.aspx");
                        }
                        else
                        {
                            ShowMessage("Employee record not found. Please contact HR.");
                        }
                    }
                    else
                    {
                        ShowMessage("Invalid user role.");
                    }
                }
                else
                {
                    ShowMessage("Invalid username or password. Please try again.");
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Login failed: " + ex.Message);
            }
        }

        private int GetEmployeeIdByUserId(int userId)
        {
            string query = "SELECT EmployeeID FROM Employees WHERE UserID = @UserID AND IsActive = 1";

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["HRConnectionString"].ConnectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@UserID", userId);
                conn.Open();

                object result = cmd.ExecuteScalar();
                return result != null ? Convert.ToInt32(result) : 0;
            }
        }

        private DataRow ValidateUser(string username, string password)
        {
            try
            {
                // Debug: Check if connection string exists
                var connectionStringSection = ConfigurationManager.ConnectionStrings["HRConnectionString"];

                if (connectionStringSection == null)
                {
                    // Try alternative names in case there's a mismatch
                    var allConnectionStrings = "";
                    foreach (ConnectionStringSettings connStr in ConfigurationManager.ConnectionStrings)
                    {
                        allConnectionStrings += connStr.Name + ", ";
                    }
                    throw new Exception($"Connection string 'HRConnectionString' not found. Available connection strings: {allConnectionStrings}");
                }

                string connectionString = connectionStringSection.ConnectionString;

                if (string.IsNullOrEmpty(connectionString))
                {
                    throw new Exception("Connection string is empty. Please check Web.config file.");
                }

                // For demo purposes, using plain text passwords
                // In production, use hashed passwords
                string query = @"SELECT UserID, Username, Email, Role, IsActive 
                               FROM Users 
                               WHERE Username = @Username AND Password = @Password AND IsActive = 1";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Username", username);
                        cmd.Parameters.AddWithValue("@Password", password);

                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);

                            if (dt.Rows.Count > 0)
                            {
                                return dt.Rows[0];
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the actual error for debugging
                System.Diagnostics.Debug.WriteLine("Login error: " + ex.Message);
                throw new Exception("Database connection failed. Please contact administrator. Details: " + ex.Message);
            }

            return null;
        }

        /// <summary>
        /// Show message to user
        /// </summary>
        /// <param name="message">Message to display</param>
        private void ShowMessage(string message)
        {
            lblMessage.Text = message;
            pnlMessage.Visible = true;
        }
    }
}