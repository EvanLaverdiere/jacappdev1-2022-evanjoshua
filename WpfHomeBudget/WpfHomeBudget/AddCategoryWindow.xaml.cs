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

namespace WpfHomeBudget
{
    /// <summary>
    /// Interaction logic for AddCategoryWindow.xaml
    /// </summary>
    ///
    public partial class AddCategoryWindow : Window
    {
        private Presenter presenter;
        private string[] categoryTypes = new string[] { "Income", "Expense", "Credit", "Savings" };

        public AddCategoryWindow(Presenter presenter)
        {
            InitializeComponent();
            this.presenter = presenter;
            cmbCategoryType.ItemsSource = categoryTypes;
        }

        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            string description = descriptionBox.Text;
            int categoryType = cmbCategoryType.SelectedIndex;

            presenter.CreateNewCategory(description, categoryType);
            descriptionBox.Clear();
            cmbCategoryType.SelectedIndex = -1;
        }
    }
}
