using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfHomeBudget
{
    interface IViewable
    {
        // Show items in the main window
        void ShowBudgetItems();

        // Display an error message if something goes wrong
        void ShowError();

        // Clear error messages 
        void ClearError();

        // Refresh the list of budget items that are displayed
        void Refresh();

        // In case we have a textbox that must me cleared afterwards - TBD
        void ClearForm();

        // In case we need to unselect an option - TBD
        void ClearSelection();
    }
}
