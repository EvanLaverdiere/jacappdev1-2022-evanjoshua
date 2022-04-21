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
using Budget;

namespace WpfHomeBudget
{
    /// <summary>
    /// Interaction logic for EditExpenseWindow.xaml
    /// </summary>
    public partial class EditExpenseWindow : Window
    {
        private Presenter presenter;
        private BudgetItem selected;

        public EditExpenseWindow(Presenter presenter, BudgetItem selected)
        {
            InitializeComponent();
            this.presenter = presenter;
            this.selected = selected;
            SetData();
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
            presenter.EditExpense(selected.ExpenseID, date, categoryId, amount, description);
        }

        private void SetData()
        {
            cmbCategory.ItemsSource = presenter.GetCategories();

            for (int i = 0; i < cmbCategory.Items.Count; i++)
            {
                if (cmbCategory.Items[i].ToString() == selected.Category)
                {
                    cmbCategory.SelectedIndex = i;
                    break;
                }
            }

            dateExpDate.SelectedDate = selected.Date;
            txtExpAmount.Text = selected.Amount.ToString();
            txtExpDescription.Text = selected.ShortDescription;
        }
    }
}
