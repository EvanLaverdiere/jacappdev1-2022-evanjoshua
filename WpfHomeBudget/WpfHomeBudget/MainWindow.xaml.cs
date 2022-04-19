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

        //private DateTime? start;
        //private DateTime? end;
        //private bool filterFlag;
        //private int categoryId;

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

                //// By default, these fields will have the following values:
                //start = end = null;
                //filterFlag = false;
                //categoryId = 0;

                //mainDisplayGrid.ItemsSource = presenter.GetBudgetItems(start, end, filterFlag, categoryId);
                //presenter.GetBudgetItemsv2(start, end, filterFlag, categoryId);
                UpdateGrid();

                cmb_Categories.ItemsSource = presenter.GetCategories();

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
            if (presenter.Modified())
            {
                if (MessageBox.Show(this, "Are you sure you want to close the application?", "Confirm", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                {
                    cancelEventArgs.Cancel = true;
                }
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
            //presenter.GetBudgetItemsv2(start, end, filterFlag, categoryId);
            UpdateGrid();
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

        public void Clear()
        {
            throw new NotImplementedException();
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
            //presenter.GetBudgetItemsv2(start, end, filterFlag, categoryId);
            UpdateGrid();
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
            // Clear out the existing columns.
            mainDisplayGrid.Columns.Clear();

            // If passed list is a list of BudgetItems, configure the grid's columns as follows.
            if(typeof(T) == typeof(Budget.BudgetItem))
            {
                var idColumn = new DataGridTextColumn();
                idColumn.Header = "Expense ID";
                //idColumn.Binding = new Binding("ExpenseId");
                idColumn.Binding = new Binding("ExpenseID");
                mainDisplayGrid.Columns.Add(idColumn);

                var dateColumn = new DataGridTextColumn();
                dateColumn.Header = "Date";
                dateColumn.Binding = new Binding("Date");
                mainDisplayGrid.Columns.Add(dateColumn);

                var categoryColumn = new DataGridTextColumn();
                categoryColumn.Header = "Category";
                categoryColumn.Binding = new Binding("Category");
                mainDisplayGrid.Columns.Add(categoryColumn);

                var descriptionColumn = new DataGridTextColumn();
                descriptionColumn.Header = "Description";
                descriptionColumn.Binding = new Binding("ShortDescription");
                mainDisplayGrid.Columns.Add(descriptionColumn);

                var amountColumn = new DataGridTextColumn();
                amountColumn.Header = "Amount";
                amountColumn.Binding = new Binding("Amount");
                amountColumn.Binding.StringFormat = "C";
                mainDisplayGrid.Columns.Add(amountColumn);

                var balanceColumn = new DataGridTextColumn();
                balanceColumn.Header = "Budget Balance";
                balanceColumn.Binding = new Binding("Balance");
                balanceColumn.Binding.StringFormat = "C";
                mainDisplayGrid.Columns.Add(balanceColumn);
            }
        }

        /// <summary>
        /// Passes information about current filters to the Presenter, so the Presenter can update the grid.
        /// </summary>
        private void UpdateGrid()
        {
            // These variables have fixed values at the moment because the UI elements needed to set them have not been implemented yet.
            DateTime? start = startDate.SelectedDate; // Specified by a DatePicker.
            DateTime? end = endDate.SelectedDate;   // Specified by a second DatePicker.
            bool filterFlag = false;    // Specified by a checkbox, or by picking a value from the list of categories?
            int categoryId = cmb_Categories.SelectedIndex + 1;     // Specified by a drop-down list of categories. Offset is necessary, as indices start from 0 while the Category IDs start from 1.

            presenter.GetBudgetItemsv2(start, end, filterFlag, categoryId);
        }

        private void startDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateGrid();
        }
    }
}
