using System;
using System.Collections.Generic;
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
        
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="_view">View associated with this presenter</param>
        /// <param name="deptId">This presenter is for a specific department</param>
        public Presenter(InterfaceView _view,int deptId)
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
    }


}
