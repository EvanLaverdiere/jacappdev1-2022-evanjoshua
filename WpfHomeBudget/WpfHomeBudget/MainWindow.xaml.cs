using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Budget;
using System.IO;
using Windows.UI.Xaml;
using Application = Windows.UI.Xaml.Application;

namespace WpfHomeBudget
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window, IViewable
    {
        private Presenter presenter;
        private bool isDarkMode;
        public MainWindow()
        {
            isDarkMode = ShouldSystemUseDarkMode();
            if (isDarkMode)
            {
                turnDark();
            }

            // Create the entry window
            EntryWindow entryWindow = new EntryWindow();

            // Open the new entry window
            _ = entryWindow.ShowDialog();


            if (entryWindow.dbLocation == null)
            {
                this.Close();
            }

            InitializeComponent();

            presenter = new Presenter(this);

            presenter.CreateBudget(entryWindow.dbLocation, entryWindow.IsNewDatabase);

            Closing += confirmClose;
        }

        [System.Runtime.InteropServices.DllImport("UXTheme.dll", SetLastError = true, EntryPoint = "#138")]
        public static extern bool ShouldSystemUseDarkMode();

        private void confirmClose(object sender, CancelEventArgs cancelEventArgs)
        {
            if (MessageBox.Show(this, "Are you sure you want to close the application?", "Confirm", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
            {
                cancelEventArgs.Cancel = true;
            }
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            AddExpenseWindow expenseWindow = new AddExpenseWindow(presenter);
            expenseWindow.ShowDialog();
        }

        public void ShowBudgetItems()
        {
            throw new NotImplementedException();
        }

        public void ShowError(string error)
        {
            //throw new NotImplementedException();
            MessageBox.Show(error, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public void ClearError()
        {
            throw new NotImplementedException();
        }

        public void Refresh()
        {
            throw new NotImplementedException();
        }

        public void ClearForm()
        {
            throw new NotImplementedException();
        }

        public void ClearSelection()
        {
            throw new NotImplementedException();
        }

        public void turnDark()
        {
            Properties.Settings.Default.ThemeColor = "DarkMode";

            Properties.Settings.Default.Save();
        }

        public void turnLight()
        {
            Properties.Settings.Default.ThemeColor = "LightMode";

            Properties.Settings.Default.Save();
        }
    }
}
