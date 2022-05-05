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
using System.Windows.Controls.DataVisualization.Charting;
using System.Linq;
using WpfHomeBudget.Interfaces;

namespace WpfHomeBudget
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window, IViewable, IDisplayable
    {
        private Presenter presenter;    // The presenter object of the application

        private bool isDarkMode;

        private List<object> _dataSource;
        public enum ChartType
        {
            Standard,
            ByCategory,
            ByMonth,
            ByMonthAndCategory
        }
        public ChartType chartType = ChartType.Standard;
        private List<string> Categories;

        public MainWindow()
        {
            //Dark mode temporarily disabled
            //isDarkMode = ShouldSystemUseDarkMode();
            isDarkMode = false;

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

                presenter = new Presenter(this, this);

                presenter.CreateBudget(entryWindow.dbLocation, entryWindow.IsNewDatabase);

                UpdateView();

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
            UpdateView();
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
            //if (isDarkMode)
            //{
            //    turnLight();
            //}
            //else
            //{
            //    turnDark();
            //}
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
            UpdateView();
        }

        public void ShowSuccess(string message)
        {
            //throw new NotImplementedException();
            MessageBox.Show(message, "SUCCESS", MessageBoxButton.OK);
        }

        #region PieChart
        public List<object> DataSource
        {
            get { return _dataSource; }
            set

            {
                // if changing data source, then redraw chart
                _dataSource = value;
                if (chartType == ChartType.ByMonthAndCategory) drawByMonthPieChart();

                if (chartType == ChartType.ByMonth) drawByMonthLineChart();
            }
        }

        // clear the current data
        public void DataClear()
        {
            ((PieSeries)chPie.Series[0]).ItemsSource = null;
        }

        // Get prepared for displaying Month and Category
        // Inputs: usedCategoryList... a list of categories
        public void InitializeByCategoryAndMonthDisplay(List<string> CategoryList)
        {
            txtTitle.Text = "By Month";
            chartType = ChartType.ByMonthAndCategory; // set chart type appropriately
            chPie.Visibility = Visibility.Visible; // show the pie chart
            txtInvalid.Visibility = Visibility.Hidden; // hide the "invalid parameters" text

            this.Categories = CategoryList; // save the categories list
        }

        // prepare for 'byMonth',
        // NOTE: just show invalid text... this chart is not implemented

        private void drawByMonthLineChart()
        {
        }

        // Draw the 'ByMonth' chart
        private void drawByMonthPieChart()
        {
            // create a list of months from the source data
            List<String> months = new List<String>();
            foreach (object obj in _dataSource)
            {
                var item = obj as Dictionary<String, object>;
                if (item != null)
                {
                    months.Add(item["Month"].ToString());
                }

            }
            // add the months to the combobox dropdown
            cbMonths.ItemsSource = months;
            // reset selected index to last 'month' in list
            cbMonths.SelectedIndex = -1;
            // set the data for the pie-chart
            set_MonthCategory_Data();
        }


        // define the data for the given month from the datasoure,
        // ... which in this case is a list of Dictionary<String,object>
        // defining totals for each category for a given month
        private void set_MonthCategory_Data()
        {
            DataClear();
            // bail out if there are no 'month' items in the drop down
            if (cbMonths.Items.Count == 0) return;
            // set the default selection to the last in the list
            if (cbMonths.SelectedIndex < 0 || cbMonths.SelectedIndex >

            cbMonths.Items.Count - 1)

            {
                cbMonths.SelectedIndex = cbMonths.Items.Count - 1;
            }
            // what is the selected month?
            String selectedMonth = cbMonths.SelectedItem.ToString();
            // ---------------------------------------------------------------
            // define which data is to be displayed
            // ---------------------------------------------------------------
            var DisplayData = new List<KeyValuePair<String, double>>();
            foreach (object obj in _dataSource)
            {
                var item = obj as Dictionary<String, object>;
                // is the item listed in the _dataSource part of the selected month ?

                if (item != null && (string)item["Month"] == selectedMonth)
                {
                    // go through each key/value pair in this item (item is a dictionary)

                    foreach (var pair in item)
                    {
                        String category = pair.Key;
                        String value = pair.Value.ToString();
                        // if the key is not a category, skip processing
                        if (!Categories.Contains(category)) continue;
                        // what is the amount of money for this category (item[category])

                        var amount = 0.0;
                        double.TryParse(value, out amount);
                        // only display expenses (i.e., amount < 0)
                        if (amount < 0)
                        {
                            DisplayData.Add(new KeyValuePair<String, double>

                            (category, -amount));
                        }
                    }
                    // we found the month we wanted, no need to loop through other months, so

                    // stop looking
                    break;
                }

            }
            // set the data for the pie-chart
            ((PieSeries)chPie.Series[0]).ItemsSource = DisplayData;
        }

        private void cbMonths_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            set_MonthCategory_Data();
        }
        #endregion

        /// <summary>
        /// Passes information about current filters to the Presenter, so the Presenter can update the view.
        /// </summary>
        private void UpdateView()
        {
            // These variables have fixed values at the moment because the UI elements needed to set them have not been implemented yet.
            DateTime? start = startDate.SelectedDate; // Specified by a DatePicker.
            DateTime? end = endDate.SelectedDate;   // Specified by a second DatePicker.
            bool filterFlag = (bool)chk_FilterCategories.IsChecked;    // Specified by a checkbox, or by picking a value from the list of categories?
            int categoryId = cmb_Categories.SelectedIndex + 1;     // Specified by a drop-down list of categories. Offset is necessary, as indices start from 0 while the Category IDs start from 1.

            bool orderByCategory = (bool)chk_OrderByCategory.IsChecked;
            bool orderByMonth = (bool)chk_OrderByMonth.IsChecked;

            ToggleMenuItems();

            presenter.UpdateDisplay(start, end, filterFlag, categoryId, orderByCategory, orderByMonth);
        }

        /// <summary>
        /// Event handler which calls the UpdateGrid() method whenever the value of startDate or endDate changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void startDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateView();
        }

        /// <summary>
        /// Event handler which calls the UpdateGrid() method whenever the chk_FilterCategories button is checked or unchecked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chk_FilterCategories_Checked(object sender, RoutedEventArgs e)
        {
            UpdateView();
        }

        private void editItem_Click(object sender, RoutedEventArgs e)
        {
            var selected = mainDisplayGrid.SelectedItem as BudgetItem;
            int index = mainDisplayGrid.SelectedIndex;

            EditExpenseWindow editExpenseWindow = new EditExpenseWindow(presenter, selected);
            editExpenseWindow.ShowDialog();

            UpdateView();
            Select(index);
        }

        private void deleteItem_Click(object sender, RoutedEventArgs e)
        {
            var selected = mainDisplayGrid.SelectedItem as BudgetItem;
            int index = mainDisplayGrid.SelectedIndex;

            if (MessageBox.Show(this, "Are you sure you want to delete this expense?", "Confirm", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                presenter.DeleteExpense(selected.ExpenseID);
            }

            UpdateView();
            Select(index);
        }

        /// <summary>
        /// Enables or disables the main display grid's ContextMenu items, based on whether user has chosen to 
        /// summarize results by category and/or month.
        /// </summary>
        private void ToggleMenuItems()
        {
            bool orderByCategory = (bool)chk_OrderByCategory.IsChecked;
            bool orderByMonth = (bool)chk_OrderByMonth.IsChecked;

            // If either or both controls are checked, disable the menu buttons.
            if (orderByCategory || orderByMonth)
            {
                editItem.IsEnabled = false;
                deleteItem.IsEnabled = false;
            }
            // If neither control is checked, re-enable the menu buttons.
            else
            {
                editItem.IsEnabled = true;
                deleteItem.IsEnabled = true;
            }
        }

        /// <summary>
        /// Sets focus to an item in the DataGrid.
        /// </summary>
        /// <param name="index">The index of item</param>
        public void Select(int index)
        {
            if (!mainDisplayGrid.Items.IsEmpty && (mainDisplayGrid.Items[index] != null))
            {
                mainDisplayGrid.SelectedItem = mainDisplayGrid.Items[index];
                mainDisplayGrid.ScrollIntoView(mainDisplayGrid.SelectedItem);
            }
        }

        private void searchButton_Click(object sender, RoutedEventArgs e)
        {
            string text = searchBox.Text;
            List<int> indexes = new List<int>();
            List<string> items = new List<string>();
            List<string> amounts = new List<string>();

            if (!mainDisplayGrid.Items.IsEmpty)
            {
                for (int i = 0; i < mainDisplayGrid.Items.Count; i++)
                {
                    BudgetItem item = mainDisplayGrid.Items[i] as BudgetItem;
                    items.Add(item.ShortDescription);
                    amounts.Add(item.Amount.ToString());
                }

                presenter.Search(text, indexes, items, amounts);
            }
        }

        public string GetDisplayType()
        {
            // Get the selected tab
            TabItem selectedTab = (TabItem)TabControl.SelectedItem;

            // If the selected tab is the Table display the Data Grid
            return selectedTab.Header as string;
        }

        public bool isOrderedByMonthAndCategory()
        {
            return (bool)chk_OrderByCategory.IsChecked && (bool)chk_OrderByMonth.IsChecked;
        }

        void IDisplayable.DisplayToGrid<T>(List<T> budgetItems)
        {
            //throw new NotImplementedException();
            mainDisplayGrid.ItemsSource = budgetItems;
            // Clear out the existing columns.
            mainDisplayGrid.Columns.Clear();

            // Create a right-aligned style to be applied to any columns containing a monetary amount.
            Style rightAligned = new Style();
            rightAligned.Setters.Add(new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Right));

            // If passed list is a list of BudgetItems, configure the grid's columns as follows.
            if (typeof(T) == typeof(Budget.BudgetItem))

            // If passed list is a list of BudgetItems, configure the grid's columns as follows.
            if (typeof(T) == typeof(Budget.BudgetItem))
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
                
                amountColumn.CellStyle = rightAligned;
                balanceColumn.CellStyle = rightAligned;
            }
            // If passed list is a list of BudgetItemsByCategory, display each category and the total for each.
            else if (typeof(T) == typeof(Budget.BudgetItemsByCategory))
            {
                // do something
                var categoryColumn = new DataGridTextColumn();
                categoryColumn.Header = "Category";
                categoryColumn.Binding = new Binding("Category");
                mainDisplayGrid.Columns.Add(categoryColumn);

                var totalsColumn = new DataGridTextColumn();
                totalsColumn.Header = "Total";
                totalsColumn.Binding = new Binding("Total");
                totalsColumn.Binding.StringFormat = "C";
                mainDisplayGrid.Columns.Add(totalsColumn);
                
                totalsColumn.CellStyle = rightAligned;
            }

            // If The list is a list of BudgetItemsByMonth, display the totals earned for each month.
            else if (typeof(T) == typeof(Budget.BudgetItemsByMonth))
            {
                // format the display 
                var monthColumn = new DataGridTextColumn();
                monthColumn.Header = "Month";
                monthColumn.Binding = new Binding("Month");
                mainDisplayGrid.Columns.Add(monthColumn);

                var totalsColumn = new DataGridTextColumn();
                totalsColumn.Header = "Total";
                totalsColumn.Binding = new Binding("Total");
                totalsColumn.Binding.StringFormat = "C";
                mainDisplayGrid.Columns.Add(totalsColumn);
                
                totalsColumn.CellStyle = rightAligned;
            }

            // If the list is a list of dictionaries, create a column for "Months", a column for each Category,
            // and a column for "Totals".
            else if (typeof(T) == typeof(Dictionary<string, object>))
            {
                List<Budget.Category> categories = presenter.GetCategories();

                List<Dictionary<string, object>> dictionaries = budgetItems as List<Dictionary<string, object>>;

                var monthColumn = new DataGridTextColumn();
                monthColumn.Header = "Month";
                monthColumn.Binding = new Binding("[Month]");
                mainDisplayGrid.Columns.Add(monthColumn);

                foreach (Category category in categories)
                {
                    string header = category.Description;
                    var column = new DataGridTextColumn();
                    column.Header = header;
                    column.Binding = new Binding($"[{header}]");
                    column.Binding.StringFormat = "C";
                    mainDisplayGrid.Columns.Add(column);
                    
                    totalsColumn.CellStyle = rightAligned;
                }

                var totalsColumn = new DataGridTextColumn();
                totalsColumn.Header = "Total";
                totalsColumn.Binding = new Binding("[Total]");
                totalsColumn.Binding.StringFormat = "C";
                mainDisplayGrid.Columns.Add(totalsColumn);
                
                totalsColumn.CellStyle = rightAligned;
            }
        }

        public void DisplayToChart(List<object> budgetItems)
        {
            DataSource = budgetItems;
        }
    }
}