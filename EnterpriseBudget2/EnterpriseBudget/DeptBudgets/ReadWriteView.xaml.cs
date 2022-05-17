using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace EnterpriseBudget.DeptBudgets
{
    /// <summary>
    /// Interaction logic for ReadWriteDataView.xaml
    /// </summary>
    public partial class ReadWriteView : Window, InterfaceView
    {
        /// <summary>
        /// presenter for the DeptBudgets.ReadWriteView
        /// </summary>
        public Presenter presenter { get; set; }

        /// <summary>
        /// view for the mainControl (starting point for the app)
        /// </summary>
        public MainControl.InterfaceView mainControl { get; set; }

        /// <summary>
        /// Standard Windows constructor
        /// </summary>
        public ReadWriteView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// The window is about to close, cleans up anything
        /// that needs to be done before closing
        /// </summary>
        public void TidyUpAndClose()
        {
            this.Close();
        }

        // Call this when the window is closing,
        // put main control back to the forefront
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            presenter.onClose();
            if (mainControl != null)
            {
                mainControl.ComeBackToForeground();
            }
            else
            {
                MessageBox.Show("You did not set mainControl - You need to fix this bug", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // window is being displayed via the "ShowDialog" method
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (presenter != null)
            {
                if (presenter.LoadData())
                {
                    // Hide the loading message
                    txtWait.Visibility = Visibility.Collapsed;

                    // Display the budget page for the chairperson
                    theChairpersonView.Visibility = Visibility.Visible;

                }
                else
                {
                    txtWait.Text = "something went wrong, unable to load home budget";
                }
            }
        }


    }
}
