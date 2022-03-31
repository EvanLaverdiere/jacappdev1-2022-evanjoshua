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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Budget;
using System.Text.RegularExpressions;
using System.Windows.Forms;


namespace WpfHomeBudget
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {


        public MainWindow()
        {
            InitializeComponent();
        }

        private void CreateDbBtn_Click(object sender, RoutedEventArgs e)
        {
            // Create a new FolderBrowserDialog object
            FolderBrowserDialog openFolderDlg = new FolderBrowserDialog()
            {
                //SelectedPath = "%userprofile%\\documents",
                RootFolder = Environment.SpecialFolder.MyDocuments,
                Description = "Select the folder in which you want to store your new Budget",
            };

            // Show the FolderBrowserDialog by calling ShowDialog method
            DialogResult result = openFolderDlg.ShowDialog();

            // Get the selected file name
            if (result.ToString() != string.Empty)
            {
                string directory = openFolderDlg.SelectedPath;
            }

            // Pass the value to the entry window so that it knows what to give the presenter
            // open the new entry winow and close this one
        }

        private void existingDbBtn_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
