using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;

namespace HRManagement.Pages.Employee
{
    public partial class EmployeeDashboard : System.Web.UI.Page
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["HRConnectionString"].ConnectionString;
        private int employeeId;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserRole"] == null || Session["UserRole"].ToString() != "Employee")
            {
                Response.Redirect("~/Pages/Users/Login.aspx");
                return;
            }

            if (Session["EmployeeID"] == null)
            {
                Response.Redirect("~/Pages/Users/Login.aspx");
                return;
            }

            employeeId = Convert.ToInt32(Session["EmployeeID"]);

            if (!IsPostBack)
            {
                LoadDashboard();
            }
        }

        private void LoadDashboard()
        {
            LoadEmployeeInfo();
            LoadAttendanceStats();
            LoadPayrollStats();
            LoadRecentAttendance();
            LoadRecentPayroll();
        }

        private void LoadEmployeeInfo()
        {
            string query = @"SELECT e.FirstName + ' ' + e.LastName AS FullName,
                           d.DepartmentName,
                           DATEDIFF(YEAR, e.DateJoined, GETDATE()) AS YearsOfService
                           FROM Employees e
                           LEFT JOIN Departments d ON e.DepartmentID = d.DepartmentID
                           WHERE e.EmployeeID = @EmployeeID";

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@EmployeeID", employeeId);
                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        lblEmployeeName.Text = reader["FullName"]?.ToString() ?? "";
                        lblDepartment.Text = reader["DepartmentName"]?.ToString() ?? "-";
                        lblYearsOfService.Text = reader["YearsOfService"]?.ToString() ?? "0";
                    }
                }
            }
        }

        private void LoadAttendanceStats()
        {
            string query = @"SELECT COUNT(*) AS PresentDays
                           FROM Attendance
                           WHERE EmployeeID = @EmployeeID
                           AND MONTH(AttendanceDate) = MONTH(GETDATE())
                           AND YEAR(AttendanceDate) = YEAR(GETDATE())
                           AND Status IN ('Present', 'Late')";

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@EmployeeID", employeeId);
                conn.Open();

                object result = cmd.ExecuteScalar();
                lblAttendanceDays.Text = result?.ToString() ?? "0";
            }
        }

        private void LoadPayrollStats()
        {
            string query = @"SELECT TOP 1 NetSalary
                           FROM Payroll
                           WHERE EmployeeID = @EmployeeID
                           ORDER BY Year DESC, Month DESC";

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@EmployeeID", employeeId);
                conn.Open();

                object result = cmd.ExecuteScalar();
                if (result != null)
                {
                    lblLastPayroll.Text = "₱" + Convert.ToDecimal(result).ToString("N2");
                }
            }
        }

        private void LoadRecentAttendance()
        {
            string query = @"SELECT TOP 5 AttendanceDate, Status, Hours
                           FROM Attendance
                           WHERE EmployeeID = @EmployeeID
                           ORDER BY AttendanceDate DESC";

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlDataAdapter adapter = new SqlDataAdapter(query, conn))
            {
                adapter.SelectCommand.Parameters.AddWithValue("@EmployeeID", employeeId);

                DataTable dt = new DataTable();
                adapter.Fill(dt);

                gvRecentAttendance.DataSource = dt;
                gvRecentAttendance.DataBind();
            }
        }

        private void LoadRecentPayroll()
        {
            string query = @"SELECT TOP 5 Month, Year, NetSalary
                           FROM Payroll
                           WHERE EmployeeID = @EmployeeID
                           ORDER BY Year DESC, Month DESC";

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlDataAdapter adapter = new SqlDataAdapter(query, conn))
            {
                adapter.SelectCommand.Parameters.AddWithValue("@EmployeeID", employeeId);

                DataTable dt = new DataTable();
                adapter.Fill(dt);

                gvRecentPayroll.DataSource = dt;
                gvRecentPayroll.DataBind();
            }
        }
    }
}