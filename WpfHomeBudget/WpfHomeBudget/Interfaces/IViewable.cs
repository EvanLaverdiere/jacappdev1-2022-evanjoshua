using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfHomeBudget
{
    // Show items in the main window
    public interface IViewable
    {
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
