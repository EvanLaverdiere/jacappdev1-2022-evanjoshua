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

namespace WpfHomeBudget
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IViewable
    {
        string directory;

        private Presenter presenter;
        public MainWindow()
        {
            // Create the entry window
            EntryWindow entryWindow = new EntryWindow();

            // Open the new entry window
            _ = entryWindow.ShowDialog();


            // The following chunk of code is to get the result of the startup window
            //// Get the location specified by the user
            //location = entryWindow.dbLocation;

            InitializeComponent();

            presenter = new Presenter(this);

            //presenter.CreateBudget(directory, entryWindow.IsNewDatabase); [UNCOMMENT ME LATER WHEN WE HAVE A WAY TO PASS AN ACTUAL FILE TO THE DATABASE]
            
            Closing += confirmClose;
        }

        private void confirmClose(object sender, CancelEventArgs cancelEventArgs)
        {
            if (MessageBox.Show(this, "Are you sure you want to close the application?", "Confirm", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
            {
                cancelEventArgs.Cancel = true;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
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
    }
}
