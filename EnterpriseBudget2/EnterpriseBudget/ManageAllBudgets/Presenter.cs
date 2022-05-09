using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnterpriseBudget.ManageAllBudgets
{
    /// <summary>
    /// Presenter for view that Manages all of the budgets
    /// </summary>
    public class Presenter
    {
        InterfaceView view;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="_view">ManageAllBudgets.View </param>
        public Presenter(InterfaceView _view)
        {
            view = _view;
        }
    }
}
