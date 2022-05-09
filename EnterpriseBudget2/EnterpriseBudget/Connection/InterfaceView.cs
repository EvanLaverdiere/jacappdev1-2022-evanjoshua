using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnterpriseBudget.Connection
{
    /// <summary>
    /// Interface for the Connection View
    /// </summary>
    public interface InterfaceView
    {
        /// <summary>
        /// Presenter for this view
        /// </summary>
        Presenter presenter { get; set; }
        /// <summary>
        /// username that user has entered, needed to connect to server
        /// </summary>
        String username { get;  }
        /// <summary>
        /// password that the user has entered, needed to connect to server
        /// </summary>
        String password { get;  }
        /// <summary>
        ///  name of the sqlserver database that user has entered, needed to connect to server
        /// </summary>
        String database { get; }
        /// <summary>
        /// Shows window in Modal form 
        /// </summary>
        bool TryToConnect();
        /// <summary>
        ///  Indicates that the connections was unsuccessful
        ///  Closes window, returning 'false' from the 'ShowDialog' method
        /// </summary>
        void FailedToConnect();
        /// <summary>
        /// Indicates that the connection was successful
        ///  Closes window, returning 'true' from the 'ShowDialog' method
        /// </summary>
        void Connected();
        /// <summary>
        /// Could not connect, allow user to try again
        /// Does not close the window
        /// </summary>
        void TryAgain();
    }
}
