using System;
using System.Windows;

namespace EnterpriseBudget.Connection
{
    /// <summary>
    /// Connection.View
    ///    Form for the entering of user data and connecting to DB server
    /// </summary>
    public partial class View : Window, InterfaceView
    {
        /// <summary>
        /// Standard WPF constructor
        /// </summary>
        public View()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Presenter for this view
        /// </summary>
        public Presenter presenter { get; set; }

        /// <summary>
        /// username that user has entered, needed to connect to server
        /// </summary>
        public string username { get { return txtUserName.Text; } }

        /// <summary>
        /// password that the user has entered, needed to connect to server
        /// </summary>
        public string password { get { return txtPassword.Password; } }

        /// <summary>
        ///  name of the sqlserver database that user has entered, needed to connect to server
        /// </summary>
        public string database { get { return txtDbName.Text; } }

        /// <summary>
        /// Shows window in Modal form, called by the Presenter 
        /// </summary>
        public bool TryToConnect()
        {
            return this.ShowDialog() == true;
        }

        /// <summary>
        ///  Indicates that the connections was unsuccessful, called by the Presenter.
        ///  Closes window, returning 'false' from the 'ShowDialog' method
        ///  <ref>TryToConnect</ref>
        /// </summary>
        public void FailedToConnect()
        {
            MessageBox.Show("Unable to log on.\n Closing Application", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            DialogResult = false;
        }

        /// <summary>
        /// Indicates that the connection was successful, called by the Presenter
        ///  Closes window, returning 'true' from the 'ShowDialog' method
        ///  <ref>TryToConnect</ref>
        /// </summary>
        public void Connected()
        {
            DialogResult = true;
        }

        /// <summary>
        /// Could not connect, allow user to try again, called by the Presenter
        /// </summary>
        public void TryAgain()
        {
            txtStatus.Text = "";
            MessageBox.Show("Unable to log on.\n Try again, or cancel", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        // User hit the 'connect' buton, call presenter with this info
        private void Connect_Click(object sender, RoutedEventArgs e)
        {
            // Note: use dispatcher to try to connect to database
            //       _after_ the form has updated the text status
            //       ... otherwise it won't redraw the form while
            //           trying to connect.
            txtStatus.Text = "... Connecting";
            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.ApplicationIdle,
                new Action(() => { presenter.Connect(); }));
        }

        // User hit the 'cancel' button
        //  Closes window, returning 'false' from the 'ShowDialog' method
        //  <ref>TryToConnect</ref>
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

    }
}
