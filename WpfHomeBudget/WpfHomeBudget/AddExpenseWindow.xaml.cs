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
using Budget;

namespace WpfHomeBudget
{
    /// <summary>
    /// Interaction logic for AddExpenseWindow.xaml
    /// </summary>
    public partial class AddExpenseWindow : Window
    {
        private string[] categories = new string[] { "Expense", "Credit", "Savings", "Income" };

        public AddExpenseWindow()
        {
            InitializeComponent();
            ComboBox1.ItemsSource = categories;
        }
    }
}
