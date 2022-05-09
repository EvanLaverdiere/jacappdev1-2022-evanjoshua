using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnterpriseBudget.ManageAllBudgets
{
    /// <summary>
    /// ManageAllBudgets interface view
    /// </summary>
    public interface InterfaceView
    {

        /// <summary>
        /// Presenter for ManageAllBudgets.InterfaceView
        /// </summary>
        Presenter presenter { get; set; }

        /// <summary>
        /// Do whatever needs to be done before closing
        /// </summary>
        void TidyUpAndClose();

        /// <summary>
        /// Main Control View (entry point of this app)
        /// </summary>
        MainControl.InterfaceView mainControl { get; set; }

    }
}
