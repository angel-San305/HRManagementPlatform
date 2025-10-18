using System;
using System.Web.UI;

namespace HRManagement
{
    public partial class EmployeeSite : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Check if user is logged in
            if (Session["Username"] == null || Session["UserRole"] == null)
            {
                Response.Redirect("~/Pages/Users/Login.aspx");
                return;
            }

            // Ensure only employees can access
            if (Session["UserRole"].ToString() != "Employee")
            {
                Response.Redirect("~/Pages/Dashboard.aspx");
                return;
            }

            // Check if EmployeeID exists
            if (Session["EmployeeID"] == null)
            {
                Response.Redirect("~/Pages/Users/Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                lblUsername.Text = Session["Username"].ToString();

                // Set page title based on current page
                SetPageTitle();

                // Set active menu item
                SetActiveMenuItem();
            }
        }

        private void SetPageTitle()
        {
            string currentPage = System.IO.Path.GetFileName(Request.Url.AbsolutePath);

            switch (currentPage.ToLower())
            {
                case "employeedashboard.aspx":
                    lblPageTitle.Text = "My Dashboard";
                    break;
                case "employeeprofile.aspx":
                    lblPageTitle.Text = "My Profile";
                    break;
                case "attendance.aspx":
                    lblPageTitle.Text = "My Attendance";
                    break;
                case "employeeaccount.aspx":
                    lblPageTitle.Text = "Account Settings";
                    break;
                default:
                    lblPageTitle.Text = "Employee Portal";
                    break;
            }
        }

        private void SetActiveMenuItem()
        {
            string currentPage = System.IO.Path.GetFileName(Request.Url.AbsolutePath).ToLower();

            // Remove active class from all links
            lnkDashboard.CssClass = "nav-link";
            lnkProfile.CssClass = "nav-link";
            lnkAttendance.CssClass = "nav-link";
            lnkAccount.CssClass = "nav-link";

            // Set active class based on current page
            switch (currentPage)
            {
                case "employeedashboard.aspx":
                    lnkDashboard.CssClass = "nav-link active";
                    break;
                case "EmployeeProfile.aspx":
                    lnkProfile.CssClass = "nav-link active";
                    break;
                case "Attendance.aspx":
                    lnkAttendance.CssClass = "nav-link active";
                    break;
                case "employeeaccount.aspx":
                    lnkAccount.CssClass = "nav-link active";
                    break;
            }
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("~/Pages/Users/Login.aspx");
        }
    }
}