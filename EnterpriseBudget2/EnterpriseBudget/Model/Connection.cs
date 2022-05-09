using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;


namespace EnterpriseBudget.Model
{
    static class Connection
    {
        /// <summary>
        /// Connection object
        /// </summary>
        public static SqlConnection cnn;

        /// <summary>
        /// Connect to an sql server database and open the connection
        /// </summary>
        /// <param name="database">name of the database on SQL server</param>
        /// <param name="username">user name</param>
        /// <param name="password">password</param>
        /// <returns>true if successfully connected, false otherwise</returns>
        static public bool Connect(string database, string username, string password)
        {
            string connectionString;
            try
            {
                connectionString = $"Data Source=10.101.0.12\\SQLEXPRESS;Database={database};User Id={username};Password={password};";
                cnn = new SqlConnection(connectionString);
                cnn.Open();
                return true;
            }
            catch
            {
                return false;
            }
        }  
    }
}
