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

        public AddCategoryWindow(Presenter presenter)
        {
            InitializeComponent();
            this.presenter = presenter;
            cmbCategoryType.ItemsSource = presenter.GetCategoryTypes();
        }

        /// <summary>
        /// Adds a new category to the budget with the provided user input.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            string description = descriptionBox.Text;
            int categoryType = cmbCategoryType.SelectedIndex;

            presenter.CreateNewCategory(description, categoryType);
            descriptionBox.Clear();
            cmbCategoryType.SelectedIndex = -1;
        }

        /// <summary>
        /// Adds a new category to the budget and then closes the window. 
        /// Must have entered all required data before closing.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addCloseButton_Click(object sender, RoutedEventArgs e)
        {
            string description = descriptionBox.Text;
            int categoryType = cmbCategoryType.SelectedIndex;

            bool toClose = presenter.CreateNewCategory(description, categoryType, true);
            descriptionBox.Clear();
            cmbCategoryType.SelectedIndex = -1;

            if (toClose == true)
            {
                Close();
            }
        }
    }
}
