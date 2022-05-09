using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnterpriseBudget.MainControl

{
    /// <summary>
    /// View for the Main Control (entry point for the App)
    /// </summary>
    public interface InterfaceView
    {
        /// <summary>
        /// Bring the view to the foreground
        /// </summary>
        void ComeBackToForeground();
        /// <summary>
        /// Make the window disappear, but not unloaded
        /// </summary>
        void GoAway();
    }
}
