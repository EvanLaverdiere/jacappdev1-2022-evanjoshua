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
        void ShowBudgetItems();

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

        // Change the color scheme to dark mode
        void turnDark();

        // Change the color scheme to the colorfull one

        void turnLight();
    }
}
