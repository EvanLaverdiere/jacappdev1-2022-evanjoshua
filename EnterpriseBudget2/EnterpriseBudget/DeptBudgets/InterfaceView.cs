using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnterpriseBudget.DeptBudgets
{
    /// <summary>
    /// Deptartment Budgets View Interface
    /// </summary>
    public interface InterfaceView
    {
        /// <summary>
        /// Presenter for Department Budgets
        /// </summary>
        Presenter presenter { get; set; }

        /// <summary>
        ///  Prepare for closing, and close
        /// </summary>
        void TidyUpAndClose();

        /// <summary>
        /// The main control view (entry point for the application)
        /// </summary>
        MainControl.InterfaceView mainControl { get; set; }

    }
}
