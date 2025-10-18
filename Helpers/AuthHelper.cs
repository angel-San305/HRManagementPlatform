using System.Data.SqlClient;

namespace HRManagementSystem.Helpers
{
    public class AuthHelper
    {
        public static bool ValidateLogin(string email, string password)
        {
            using (SqlConnection conn = DbHelper.GetConnection())
            {
                conn.Open();
                string query = "SELECT COUNT(*) FROM Users WHERE Email=@Email AND Password=@Password";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@Password", password);

                return (int)cmd.ExecuteScalar() > 0;
            }
        }
    }
}
