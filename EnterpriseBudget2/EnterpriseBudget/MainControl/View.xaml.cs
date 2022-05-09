using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Windows;


namespace EnterpriseBudget.MainControl
{
    /// <summary>
    /// What type of views are available
    /// </summary>
    public enum ViewType
    {
        /// <summary>
        /// Can view the department budget, but cannot modify it
        /// </summary>
        ReadOnly,
        /// <summary>
        /// Can view and modify the department budget
        /// </summary>
        ReadWrite,
        /// <summary>
        /// Can view and modify ANY department budget, and
        /// modify the budget limits, get reports, etc
        /// </summary>
        Manage
    }

    public partial class View : Window, InterfaceView
    {
        Connection.Presenter _connectPresenter;
        Presenter _mainViewPresenter;

        /// <summary>
        /// MainControl.View - entry point to the application
        /// </summary>
        public View()
        {
            InitializeComponent();

            // first we need to connect to the database
            // ... if we can't, we close the app
            _connectPresenter = new Connection.Presenter();
            var connectView = new Connection.View();
            connectView.presenter = _connectPresenter;
            if (!_connectPresenter.ConnectToServer(connectView)) this.Close();

            // define presenter for this view
            _mainViewPresenter = new Presenter(this);
            txtUserName.Focus();
        }

        /// <summary>
        /// Re-display the window
        /// </summary>
        public void ComeBackToForeground()
        {
            this.Visibility = Visibility.Visible;
        }

        /// <summary>
        ///  hide the window
        /// </summary>
        public void GoAway()
        {
            this.Visibility = Visibility.Hidden;
        }

        #region event handlers
        private void FacultyButton_Click(object sender, RoutedEventArgs e)
        {
            txtStatus.Text = "";

            // get employee
            var employee = _mainViewPresenter.GetEmployeeForSpecifiedView(txtUserName.Text, txtPassword.Password, ViewType.ReadOnly);
            
            // user has permission
            if (employee != null)
            {
                // define view and presenters
                var readOnlyDataView = new DeptBudgets.ReadOnlyView();
                readOnlyDataView.presenter = new DeptBudgets.Presenter((DeptBudgets.InterfaceView)readOnlyDataView, employee.deptartmentID);
                readOnlyDataView.mainControl = this;
                
                // Show the view
                this.GoAway();
                readOnlyDataView.ShowDialog();
            }
            else
            {
                txtStatus.Text = "Invalid username/password OR not enough privileges for " + ViewType.ReadWrite.ToString() + "View";
            }
        }

        private void ChairPersonButton_Clicked(object sender, RoutedEventArgs e)
        {
            txtStatus.Text = "";

            // get employee
            var employee = _mainViewPresenter.GetEmployeeForSpecifiedView(txtUserName.Text, txtPassword.Password, ViewType.ReadWrite);

            // user has permission
            if (employee != null)
            {
                // define view and presenters
                var readWriteView = new DeptBudgets.ReadWriteView();
                readWriteView.presenter = new DeptBudgets.Presenter((DeptBudgets.InterfaceView)readWriteView, employee.deptartmentID);
                readWriteView.mainControl = this;

                // show the view
                this.GoAway();
                readWriteView.ShowDialog();
            }

            // user does not have permission
            else
            {
                txtStatus.Text = "Invalid username/password OR not enough privileges for " + ViewType.ReadWrite.ToString() + "View";
            }
        }

        // 
        private void AdministratorButton_Clicked(object sender, RoutedEventArgs e)
        {
            this.txtStatus.Text = "";
            // TODO: get employee etc.

            // define view and presenter
            var manageAllBudgetsView = new ManageAllBudgets.View();
            manageAllBudgetsView.presenter = new ManageAllBudgets.Presenter((ManageAllBudgets.InterfaceView) manageAllBudgetsView);
            manageAllBudgetsView.mainControl = this;
            
            // show view
            this.GoAway();
            manageAllBudgetsView.ShowDialog();
        }
        #endregion

    }
}


