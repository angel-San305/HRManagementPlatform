using System;
using System.Web.UI;

namespace HRManagement
{
    public partial class Site : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Username"] == null)
            {
                Response.Redirect("~/Pages/Login.aspx");
                return;
            }

            // Block employees from accessing admin pages
            if (Session["UserRole"] != null && Session["UserRole"].ToString() == "Employee")
            {
                Response.Redirect("~/Pages/employee/EmployeeDashboard.aspx");
                return;
            }

            if (!IsPostBack)
            {
                lblUsername.Text = Session["Username"].ToString();
                SetPageTitle();
            }
        }
        protected void btnLogout_Click(object sender, EventArgs e)
        {
            // Clear session and redirect to login
            Session.Clear();
            Session.Abandon();
            Response.Redirect("~/pages/users/Login.aspx");
        }

        private void SetPageTitle()
        {
            string pageName = System.IO.Path.GetFileNameWithoutExtension(Request.Url.AbsolutePath);

            switch (pageName.ToLower())
            {
                case "dashboard":
                    lblPageTitle.Text = "Dashboard";
                    break;
                case "employees":
                    lblPageTitle.Text = "Employee Management";
                    break;
                case "departments":
                    lblPageTitle.Text = "Department Management";
                    break;
                case "roles":
                    lblPageTitle.Text = "Role Management";
                    break;
                case "attendance":
                    lblPageTitle.Text = "Attendance Management";
                    break;
                case "payroll":
                    lblPageTitle.Text = "Payroll Management";
                    break;
                case "employeeprofile":
                    lblPageTitle.Text = "Employee Profile";
                    break;
                default:
                    lblPageTitle.Text = "HR Management";
                    break;
            }
        }
    }
}


