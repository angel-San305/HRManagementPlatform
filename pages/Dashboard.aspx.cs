using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;

namespace HRManagement.Pages
{
    public partial class Dashboard : System.Web.UI.Page
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
                LoadDashboardData();
                LoadRecentEmployees();
                SetWelcomeMessage();
            }
        }

        /// <summary>
        /// Load dashboard statistics
        /// </summary>
        private void LoadDashboardData()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Get total employees
                    string employeeQuery = "SELECT COUNT(*) FROM Employees WHERE IsActive = 1";
                    using (SqlCommand cmd = new SqlCommand(employeeQuery, conn))
                    {
                        lblTotalEmployees.Text = cmd.ExecuteScalar().ToString();
                    }

                    // Get total departments
                    string deptQuery = "SELECT COUNT(*) FROM Departments WHERE IsActive = 1";
                    using (SqlCommand cmd = new SqlCommand(deptQuery, conn))
                    {
                        lblTotalDepartments.Text = cmd.ExecuteScalar().ToString();
                    }

                    // Get total roles
                    string roleQuery = "SELECT COUNT(*) FROM Roles WHERE IsActive = 1";
                    using (SqlCommand cmd = new SqlCommand(roleQuery, conn))
                    {
                        lblTotalRoles.Text = cmd.ExecuteScalar().ToString();
                    }

                    // Get today's attendance count
                    string attendanceQuery = @"SELECT COUNT(*) FROM Attendance 
                                             WHERE AttendanceDate = CAST(GETDATE() AS DATE)";
                    using (SqlCommand cmd = new SqlCommand(attendanceQuery, conn))
                    {
                        lblTodayAttendance.Text = cmd.ExecuteScalar().ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                // Log error (in production, use proper logging)
                Response.Write("<script>alert('Error loading dashboard data: " + ex.Message + "');</script>");
            }
        }

        /// <summary>
        /// Load recent employees (last 5)
        /// </summary>
        private void LoadRecentEmployees()
        {
            try
            {
                string query = @"SELECT TOP 5 
                                e.FirstName + ' ' + e.LastName AS FullName,
                                d.DepartmentName,
                                e.DateJoined
                               FROM Employees e
                               LEFT JOIN Departments d ON e.DepartmentID = d.DepartmentID
                               WHERE e.IsActive = 1
                               ORDER BY e.DateJoined DESC";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlDataAdapter adapter = new SqlDataAdapter(query, conn))
                    {
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);

                        gvRecentEmployees.DataSource = dt;
                        gvRecentEmployees.DataBind();
                    }
                }
            }
            catch (Exception ex)
            {
                Response.Write("<script>alert('Error loading recent employees: " + ex.Message + "');</script>");
            }
        }

        /// <summary>
        /// Set welcome message with user info
        /// </summary>
        private void SetWelcomeMessage()
        {
            if (Session["Username"] != null)
            {
                lblWelcomeUser.Text = Session["Username"].ToString();
                lblUserRole.Text = Session["UserRole"].ToString();
            }
        }

        /// <summary>
        /// Format amount as Philippine Peso
        /// </summary>
        private string FormatPHP(object amount)
        {
            if (amount == null || amount == DBNull.Value)
                return "₱0.00";

            if (decimal.TryParse(amount.ToString(), out decimal value))
                return "₱" + value.ToString("N2");

            return "₱0.00";
        }
    }
}