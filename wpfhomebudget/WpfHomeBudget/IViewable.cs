using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfHomeBudget
{
    public interface IViewable
    {
        // Show items in the main window
        /// <summary>
        /// Displays a list of the budget's items.
        /// </summary>
        /// <typeparam name="T">The generic Type of the budget items. Could be BudgetItem, BudgetItemByCategory, BudgetItemByMonth, or something else.</typeparam>
        /// <param name="budgetItems">The list of budget items to be displayed.</param>
        void ShowBudget<T>(List<T> budgetItems);

        // Display an error message if something goes wrong
        /// <summary>
        /// Displays an error message.
        /// </summary>
        /// <param name="error">The message to be displayed.</param>
        void ShowError(string error);

        /// <summary>
        /// Displays a message confirming a successful database operation.
        /// </summary>
        /// <param name="message">The message to be displayed.</param>
        void ShowSuccess(string message);

        /// <summary>
        /// Display the window with a darker color scheme
        /// </summary>
        void turnDark();

        /// <summary>
        /// Display the window with a lighter color scheme
        /// </summary>
        void turnLight();

        /// <summary>
        /// Select an item at the given index
        /// </summary>
        /// <param name="index"></param>
        void Select(int index);
    }
}
