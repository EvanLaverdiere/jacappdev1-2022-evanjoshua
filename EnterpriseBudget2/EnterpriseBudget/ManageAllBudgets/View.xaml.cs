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

namespace EnterpriseBudget.ManageAllBudgets
{
    /// <summary>
    /// Interaction logic for ManageAllBudgetsView.xaml
    /// </summary>
    public partial class View : Window, InterfaceView
    {
        /// <summary>
        /// Presenter for ManageAllBudgets.View
        /// </summary>
        public Presenter presenter { get; set; }
 
        /// <summary>
        /// mainControl view (entry point for this app)
        /// </summary>
        public MainControl.InterfaceView mainControl { get; set; }
        
        /// <summary>
        /// Standard windows constructor
        /// </summary>
        public View()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Do anything that needs to be done before closing
        /// </summary>
        public void TidyUpAndClose()
        {
            this.Close();
        }

        private void ReturnButton_Clicked(object sender, RoutedEventArgs e)
        {
            TidyUpAndClose();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (mainControl != null)
            {
                mainControl.ComeBackToForeground();
            }
            else
            {
                MessageBox.Show("You did not set mainControl - You need to fix this bug", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

   }
}
