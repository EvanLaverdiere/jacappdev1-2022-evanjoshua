using System;
using System.Collections.Generic;
using System.Text;
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
    /// Interaction logic for EditExpenseWindow.xaml
    /// </summary>
    public partial class EditExpenseWindow : Window
    {
        private Presenter presenter;

        public EditExpenseWindow(Presenter presenter)
        {
            InitializeComponent();
            this.presenter = presenter;
            cmbCategory.ItemsSource = presenter.GetCategories();
            dateExpDate.SelectedDate = DateTime.Today;
        }

        private void btnNewCategory_Click(object sender, RoutedEventArgs e)
        {
            AddCategoryWindow addCategory = new AddCategoryWindow(presenter);
            addCategory.ShowDialog();
            cmbCategory.ItemsSource = presenter.GetCategories();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            if (cmbCategory.SelectedIndex != -1 || txtExpAmount.Text != string.Empty || txtExpDescription.Text != string.Empty)
            {
                if (MessageBox.Show("Are you sure you want to cancel editing this expense?", "CONFIRM CANCELATION", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    Close();
                }
            }
            else
                Close();
        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            DateTime? date = dateExpDate.SelectedDate;
            int categoryId = cmbCategory.SelectedIndex + 1;
            string amount = txtExpAmount.Text;
            string description = txtExpDescription.Text;
            //presenter.EditExpense(date, categoryId, amount, description);
        }
    }
}
