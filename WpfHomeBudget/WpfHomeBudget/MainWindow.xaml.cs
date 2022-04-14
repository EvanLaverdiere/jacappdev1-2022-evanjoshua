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

namespace WpfHomeBudget
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window, IViewable
    {
        private Presenter presenter;
        private bool isDarkMode;
        private DateTime? start;
        private DateTime? end;
        private bool filterFlag;
        private int categoryId;
        public MainWindow()
        {
            isDarkMode = ShouldSystemUseDarkMode();

            // Create the entry window
            EntryWindow entryWindow = new EntryWindow(isDarkMode);

            // Open the new entry window
            _ = entryWindow.ShowDialog();

            if (entryWindow.dbLocation == null)
            {
                this.Close();
            }
            else
            {
                InitializeComponent();

                if (isDarkMode)
                {
                    turnDark();
                }

                presenter = new Presenter(this);

                presenter.CreateBudget(entryWindow.dbLocation, entryWindow.IsNewDatabase);

                // By default, these fields will have the following values:
                start = end = null;
                filterFlag = false;
                categoryId = 0;

                mainDisplayGrid.ItemsSource = presenter.GetBudgetItems(start, end, filterFlag, categoryId);

                Closing += confirmClose;

                txtStatusBar.Text = entryWindow.dbLocation;
            }
        }

        [System.Runtime.InteropServices.DllImport("UXTheme.dll", SetLastError = true, EntryPoint = "#138")]
        public static extern bool ShouldSystemUseDarkMode();

        /// <summary>
        /// Requests confirmation from the user to close the application.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="cancelEventArgs"></param>


        private void confirmClose(object sender, CancelEventArgs cancelEventArgs)
        {
            if (MessageBox.Show(this, "Are you sure you want to close the application?", "Confirm", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
            {
                cancelEventArgs.Cancel = true;
            }
        }

        /// <summary>
        /// Creates and opens a new window for adding expenses to the budget.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            AddExpenseWindow expenseWindow = new AddExpenseWindow(presenter);
            expenseWindow.ShowDialog();
            // The presenter should update the view after an expense is added.
            presenter.GetBudgetItemsv2(start, end, filterFlag, categoryId);
        }

        public void ShowBudgetItems()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Displays a message box alerting the user of an error.
        /// </summary>
        /// <param name="error">The error encountered.</param>
        public void ShowError(string error)
        {
            //throw new NotImplementedException();
            MessageBox.Show(error, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public void turnDark()
        {
            Properties.Settings.Default.ThemeColor = "DarkMode";

            Properties.Settings.Default.Save();

            isDarkMode = true;

            Image themeLogo = (Image)this.FindName("ThemeLogo");

            BitmapImage bitImage = new BitmapImage();
            bitImage.BeginInit();

            bitImage.UriSource = new Uri("images\\LightTheme.png", UriKind.Relative);

            themeLogo.Source = bitImage;
        }

        public void turnLight()
        {
            Properties.Settings.Default.ThemeColor = "LightMode";

            Properties.Settings.Default.Save();

            isDarkMode = false;

            Image themeLogo = (Image)this.FindName("ThemeLogo");

            BitmapImage bitImage = new BitmapImage();
            bitImage.BeginInit();

            bitImage.UriSource = new Uri("images\\LightTheme.png", UriKind.Relative);

            themeLogo.Source = bitImage;
        }

        private void theme_Click(object sender, RoutedEventArgs e)
        {
            if (isDarkMode)
            {
                turnLight();
            }
            else
            {
                turnDark();
            }
        }

        /// <summary>
        /// Creates and opens a window for adding new categories to the budget.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addCategory(object sender, RoutedEventArgs e)
        {
            AddCategoryWindow categoryWindow = new AddCategoryWindow(presenter);
            categoryWindow.ShowDialog();
            // Tell the presenter to update the view after a successful operation.
            presenter.GetBudgetItemsv2(start, end, filterFlag, categoryId);
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public void ShowSuccess(string message)
        {
            //throw new NotImplementedException();
            MessageBox.Show(message, "SUCCESS", MessageBoxButton.OK);
        }

        public void ShowBudgetItems<T>(List<T> budgetItems)
        {
            //throw new NotImplementedException();
            mainDisplayGrid.ItemsSource = budgetItems;
        }
    }
}
