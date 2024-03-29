﻿using System;
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
    /// Interaction logic for AddExpenseWindow.xaml
    /// </summary>
    public partial class AddExpenseWindow : Window
    {
        private Presenter presenter;
        public AddExpenseWindow(Presenter presenter)
        {
            InitializeComponent();
            this.presenter = presenter;
            cmbCategory.ItemsSource = presenter.GetCategories(); //[UNCOMMENT ME ONCE WE CAN ACTUALLY INITIALIZE A HOMEBUDGET]
            dateExpDate.SelectedDate = DateTime.Today;
            Closing += ConfirmExit;
        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            DateTime? date = dateExpDate.SelectedDate;
            int categoryId = cmbCategory.SelectedIndex + 1;
            string amount = txtExpAmount.Text;
            string description = txtExpDescription.Text;
            // Must wait until view interface has been implemented in the main window before more can be done with this.
            // [Program will crash here because the HomeBudget has not been initialized yet.] [04/04/2022: Disregard. Program does not crash thanks to try-catch block.]
            presenter.CreateNewExpense(date, categoryId, amount, description);
            ClearExpenseForm();
        }

        /// <summary>
        /// Prompts user to confirm that they want to cancel adding a new expense to the budget.
        /// If user chooses yes, method closes this window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            if (cmbCategory.SelectedIndex != -1 || txtExpAmount.Text != string.Empty || txtExpDescription.Text != string.Empty)
            {
                if (MessageBox.Show("Are you sure you want to cancel adding this new expense?", "CONFIRM CANCELATION", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    // clear the form so we won't trigger the closing verification function.
                    ClearExpenseForm();
                    Close();
                }
            }
            else
                Close();
        }

        /// <summary>
        /// Opens a dialog window to let the user add a new Category to the database.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnNewCategory_Click(object sender, RoutedEventArgs e)
        {
            AddCategoryWindow addCategory = new AddCategoryWindow(presenter);
            addCategory.ShowDialog();
            // Refresh the list of categories.
            cmbCategory.ItemsSource = presenter.GetCategories();
        }

        /// <summary>
        /// Clears the Add Expense form so that the same Expense cannot be added multiple times.
        /// </summary>
        private void ClearExpenseForm()
        {
            dateExpDate.SelectedDate = DateTime.Today;
            cmbCategory.SelectedIndex = -1;
            txtExpAmount.Clear();
            txtExpDescription.Clear();
        }

        /// <summary>
        /// Prompts the user to confirm closing the window if there are unsaved changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="cancelEventArgs"></param>
        private void ConfirmExit(object sender, System.ComponentModel.CancelEventArgs cancelEventArgs)
        {
            if (cmbCategory.SelectedIndex != -1 || txtExpAmount.Text != string.Empty || txtExpDescription.Text != string.Empty)
            {
                if (MessageBox.Show(this, "There are unsaved changes. Do you wish to proceed?", "Confirm", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                {
                    cancelEventArgs.Cancel = true;
                }
            }

        }

    }
}
