using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnterpriseBudget.ChairpersonControl
{
    public interface IDisplayable
    {
        /// <summary>
        /// Displays an error message if something goes wrong
        /// </summary>
        /// <param name="error">The message to be displayed.</param>
        void ShowError(string error);

        /// <summary>
        /// Displays a message confirming a successful database operation.
        /// </summary>
        /// <param name="message">The message to be displayed.</param>
        void ShowSuccess(string message);

        /// <summary>
        /// Select an item at the given index
        /// </summary>
        /// <param name="index"></param>
        void Select(int index);

        string GetDisplayType();
        bool isOrderedByMonthAndCategory();
        void DisplayToGrid<T>(List<T> budgetItems);
        void DisplayToChart(List<object> budgetItems);
        void InitializeByCategoryAndMonthDisplay(List<string> categoryNames);
    }
}
