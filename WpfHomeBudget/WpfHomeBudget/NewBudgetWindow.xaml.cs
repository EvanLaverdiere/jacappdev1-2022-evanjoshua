using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfHomeBudget
{
    /// <summary>
    /// Interaction logic for NewBudgetWindow.xaml
    /// </summary>
    public partial class NewBudgetWindow : Window
    {
        string dbLocation = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        public string path = "";
        public NewBudgetWindow()
        {
            InitializeComponent();
            displayCurrentLocation();
        }

        private void browseFoldersBtn_Click(object sender, RoutedEventArgs e)
        {
            // Create a new FolderBrowserDialog object
            FolderBrowserDialog openFolderDlg = new FolderBrowserDialog()
            {
                SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                Description = "Select the folder in which you want to store your new Budget",
                UseDescriptionForTitle = true,
            };

            // Show the FolderBrowserDialog by calling ShowDialog method
            _ = openFolderDlg.ShowDialog();

            // Get the selected file name
            dbLocation = openFolderDlg.SelectedPath;

            displayCurrentLocation();
        }

        private void displayCurrentLocation()
        {
            // fill the textbox of the location with the default location
            System.Windows.Controls.TextBox textbox = (System.Windows.Controls.TextBox)this.FindName("inputLocation");
            textbox.Text = dbLocation;
        }

        private void cancelCreateBudgetBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void proceedCreateBudgetBtn_Click(object sender, RoutedEventArgs e)
        {
            string inputLocation = ((System.Windows.Controls.TextBox)this.FindName("inputLocation")).Text;
            string inputFileName = ((System.Windows.Controls.TextBox)this.FindName("inputName")).Text;

            if (inputFileName != string.Empty && inputLocation != string.Empty)
            {
                path = inputLocation + "\\" + inputFileName;
                this.Close();
            }
        }
    }
}
