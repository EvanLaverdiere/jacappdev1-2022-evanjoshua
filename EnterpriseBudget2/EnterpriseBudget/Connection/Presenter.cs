using System;
using System.Data.SqlClient;

namespace EnterpriseBudget.Connection
{
    /// <summary>
    /// Connection.Presenter
    ///    Uses the username/password to connect to the server where
    ///    the data is being stored
    /// </summary>
    public class Presenter
    {
        /// <summary>
        /// The connection object
        /// </summary>
        public SqlConnection cnn;
        private InterfaceView view;
        private int MaxTriesAllowed = 3;
        private int tries = 0;

        /// <summary>
        /// Entry Point to connecting to the server
        /// </summary>
        /// <param name="_view">A view that implements Connection.InterfaceView</param>
        /// <returns>true if connection succeeded</returns>
        public bool ConnectToServer(InterfaceView _view)
        {
            view = _view;
            return view.TryToConnect();
        }

        /// <summary>
        /// Connect to the database
        /// </summary>
        /// <remarks>Allowed maximum number of tries is 'MaxTriesAllowed</remarks>
        public void Connect()
        {
            bool loggedIn = false;
            if (!loggedIn && tries < MaxTriesAllowed)
            {
                loggedIn = Model.Connection.Connect(view.database, view.username, view.password);
                if (loggedIn)
                {
                    view.Connected();
                    return;
                }
                tries++;
                view.TryAgain();
            }
            else
            {
                view.FailedToConnect();
            }

        }
    }

}
