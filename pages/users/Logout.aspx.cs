using System;

namespace HRManagementSystem.Users
{
    public partial class Logout : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();

            // Redirect to login page in same folder
            Response.Redirect("login.aspx");
        }
    }
}
