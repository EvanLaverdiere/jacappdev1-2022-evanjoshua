using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace EnterpriseBudget.Model
{
    /// <summary>
    /// The different type of jobs that are available
    /// </summary>
    public enum JobTypes
    {
        /// <summary>
        /// No job specified
        /// </summary>
        Nothing = 0,

        /// <summary>
        /// Teacher (faculty)
        /// </summary>
        Faculty = 1,

        /// <summary>
        /// chair person
        /// </summary>
        Chair = 2,

        /// <summary>
        /// job with absolute power
        /// </summary>
        Admin = 3
    } 
    
    /// <summary>
    /// Employee class
    /// </summary>
    public class Employee
    {
        JobTypes _jobType;
        String _jobName;
        int _deptId;
        String _departmentName;
        String _userName;
        
        /// <summary>
        /// what job type does the employee have?
        /// </summary>
        public JobTypes jobType { get { return _jobType; } }

        /// <summary>
        /// what is the department name of the employee
        /// </summary>
        public String departmentName { get { return _departmentName; } }

        /// <summary>
        /// what is the name of the employee's job?
        /// </summary>
        public String jobName { get { return _jobName; } }

        /// <summary>
        /// employee name
        /// </summary>
        public String userName { get { return _userName; } }

        /// <summary>
        /// employee department id
        /// </summary>
        public int deptartmentID { get { return _deptId; } }

        /// <summary>
        /// Verify that there is an employee with username/password
        /// </summary>
        /// <param name="username">username</param>
        /// <param name="password">password</param>
        /// <returns>an Employee object if such a user exists, null otherwise</returns>
        public static Employee validateUser(String username, String password)
        {
            Employee person = null;
            SqlDataReader rdr = null;
            try
            {
                SqlCommand verifyUser = Connection.cnn.CreateCommand();

                //string sql = 

                // TODO: FIX THIS 
                verifyUser.CommandText = "SELECT e.name, e.departmentId, d.name, e.jobId, j.name " +
                    "FROM employees e, departments d, jobTitles j " +
                    "WHERE e.departmentId = d.id " +
                    "AND e.jobId = j.id " +
                    "AND e.name=@name " +
                    "AND e.password=@password";

                verifyUser.Parameters.AddWithValue("@name", username);
                verifyUser.Parameters.AddWithValue("@password", password);

                rdr = verifyUser.ExecuteReader();

                int id, deptId, jobId;
                string name, pass;

                if (rdr.HasRows)
                {
                    rdr.Read();

                    //id = rdr.GetInt32(0);
                    //name = rdr.GetString(1);
                    //pass = rdr.GetString(2);
                    //deptId = rdr.GetInt32(3);
                    //jobId = rdr.GetInt32(4);

                    name = rdr.GetString(0);
                    deptId = rdr.GetInt32(1);
                    string deptName = rdr.GetString(2);
                    jobId = rdr.GetInt32(3);
                    string jobTitle = rdr.GetString(4);

                    person = new Employee();
                }



            }
            catch (Exception e) {
                var txt = e.Message;
            }
            if (rdr != null)
            {
                try { rdr.Close(); } catch { }
            }
            return person;
        }

        private Employee()
        {

        }

        private Employee(string userName, string password, int departmentId, JobTypes jobId)
        {
            _userName = userName;
            _deptId = departmentId;
            _jobType = jobId;
            // To be filled
            _jobName = "";
            _departmentName = "";
        }

    }
}
