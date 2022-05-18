using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace EnterpriseBudget.DeptBudgets
{
    /// <summary>
    /// Presenter logic for the DeptBudgets.View
    /// </summary>
    public class Presenter
    {
        Model.DepartmentBudgets budget;
        InterfaceView view;
        int deptId;
        public Model.DepartmentBudgets Budget { get { return budget; } }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="_view">View associated with this presenter</param>
        /// <param name="deptId">This presenter is for a specific department</param>
        public Presenter(InterfaceView _view, int deptId)
        {
            view = _view;
            this.deptId = deptId;
        }

        /// <summary>
        /// Get the data from the database, etc
        /// </summary>
        /// <returns>true if successful, false otherwise</returns>
        public bool LoadData()
        {
            budget = new Model.DepartmentBudgets();
            return budget.DownLoadAndOpenDepartmentBudgetFile(deptId);
        }

        /// <summary>
        /// The view is closing, and needs to tidy-up by calling
        /// this routine.
        /// </summary>
        public void onClose()
        {
            if (budget != null)
            {
                budget.Close();
            }
        }

        public double getCategoryLimit(int categoryId)
        {
            SqlDataReader rdr;
            double limit = 0;
            try
            {
                SqlCommand getLimit = Model.Connection.cnn.CreateCommand();

                getLimit.CommandText = "SELECT limit FROM budgetCategoryLimits " +
                    "WHERE catId = @catId " +
                    "AND deptId = @deptId";

                getLimit.Parameters.AddWithValue("@catId", categoryId);
                getLimit.Parameters.AddWithValue("@deptId", deptId);

                rdr = getLimit.ExecuteReader();

                if (rdr.HasRows)
                {
                    rdr.Read();

                    //float floatLimit = rdr.GetDouble(0);

                    //limit = (double)floatLimit;

                    limit = rdr.GetDouble(0);
                }
            }
            catch (Exception e)
            {

                throw;
            }
            if(rdr != null)
                try { rdr.Close(); } catch { }
            return limit;
        }
    }


}
