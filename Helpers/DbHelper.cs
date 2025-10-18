using System.Configuration;
using System.Data.SqlClient;

namespace HRManagementSystem.Helpers
{
    public class DbHelper
    {
        private static string connString = ConfigurationManager.ConnectionStrings["HRDb"].ConnectionString;

        public static SqlConnection GetConnection()
        {
            return new SqlConnection(connString);
        }
    }
}
