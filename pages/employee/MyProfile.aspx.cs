using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Globalization;
using System.Threading;
using System.Web.UI;

namespace HRManagement.Pages.Employee
{
    public partial class MyProfile : System.Web.UI.Page
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["HRConnectionString"].ConnectionString;
        private int employeeId;

        protected override void InitializeCulture()
        {
            CultureInfo philippineCulture = new CultureInfo("en-PH");
            Thread.CurrentThread.CurrentCulture = philippineCulture;
            Thread.CurrentThread.CurrentUICulture = philippineCulture;
            base.InitializeCulture();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserRole"] == null || Session["UserRole"].ToString() != "Employee")
            {
                Response.Redirect("~/Pages/Login.aspx");
                return;
            }

            if (Session["EmployeeID"] == null)
            {
                Response.Redirect("~/Pages/Login.aspx");
                return;
            }

            employeeId = Convert.ToInt32(Session["EmployeeID"]);

            if (!IsPostBack)
            {
                LoadProfile();
            }
        }

        private void LoadProfile()
        {
            string query = @"SELECT e.EmployeeID, e.FirstName + ' ' + e.LastName AS FullName,
                           e.Email, e.PhoneNumber, e.Salary, e.DateJoined, e.Address, e.Photo,
                           d.DepartmentName, r.RoleName,
                           DATEDIFF(YEAR, e.DateJoined, GETDATE()) AS YearsOfService
                           FROM Employees e
                           LEFT JOIN Departments d ON e.DepartmentID = d.DepartmentID
                           LEFT JOIN Roles r ON e.RoleID = r.RoleID
                           WHERE e.EmployeeID = @EmployeeID AND e.IsActive = 1";

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
                        lblEmployeeRole.Text = reader["RoleName"]?.ToString() ?? "";
                        lblEmployeeDepartment.Text = reader["DepartmentName"]?.ToString() ?? "";
                        lblEmployeeID.Text = reader["EmployeeID"]?.ToString() ?? "";
                        lblEmail.Text = reader["Email"]?.ToString() ?? "";
                        lblPhone.Text = reader["PhoneNumber"]?.ToString() ?? "N/A";
                        lblAddress.Text = reader["Address"]?.ToString() ?? "N/A";
                        lblYearsOfService.Text = reader["YearsOfService"]?.ToString() ?? "0";

                        if (reader["Salary"] != DBNull.Value)
                        {
                            decimal salary = Convert.ToDecimal(reader["Salary"]);
                            lblSalary.Text = salary.ToString("C", new CultureInfo("en-PH"));
                        }
                        else
                        {
                            lblSalary.Text = "N/A";
                        }

                        if (reader["DateJoined"] != DBNull.Value)
                        {
                            lblDateJoined.Text = Convert.ToDateTime(reader["DateJoined"]).ToString("MMMM dd, yyyy");
                        }

                        if (reader["Photo"] != DBNull.Value && !string.IsNullOrEmpty(reader["Photo"].ToString()))
                        {
                            imgProfile.ImageUrl = reader["Photo"].ToString();
                        }
                    }
                }
            }
        }
    }
}