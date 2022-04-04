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
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.IO;

namespace WpfHomeBudget
{
    /// <summary>
    /// Interaction logic for EntryWindow.xaml
    /// </summary>
    public partial class EntryWindow : Window
    {
        public string dbLocation;

        /// <summary>
        /// True if the window is creating a new database, false if it is loading an existing one.
        /// </summary>
        public bool IsNewDatabase { get; private set; }
        public EntryWindow()
        {
            InitializeComponent();
        }

        private void CreateDbBtn_Click(object sender, RoutedEventArgs e)
        {
            // Create a new FolderBrowserDialog object
            FolderBrowserDialog openFolderDlg = new FolderBrowserDialog()
            {
                RootFolder = Environment.SpecialFolder.MyDocuments,
                Description = "Select the folder in which you want to store your new Budget",
                UseDescriptionForTitle = true,
            };

            // Show the FolderBrowserDialog by calling ShowDialog method
            _ = openFolderDlg.ShowDialog();

            // Get the selected file name
            string obtainedDirectory = openFolderDlg.SelectedPath;

            //Check if the folder exists and if it does set it as the dbDirectory
            if (Directory.Exists(obtainedDirectory))
            {
                dbLocation = obtainedDirectory;
                IsNewDatabase = true;
                this.Close();
            }
        }

        private void ExistingDbBtn_Click(object sender, RoutedEventArgs e)
        {
            // Create a new OpenFileDialog object
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                CheckFileExists = true,
                CheckPathExists = true,
                DefaultExt = "db",
                Filter = "DB files (*.db)|*.db",
                FilterIndex = 1,
                InitialDirectory = Regex.Replace(Directory.GetCurrentDirectory(), @"(\\.[^\\]*){4}$", "TestFolder"),
                Title = "Select a database file"
            };

            // Show the OpenFileDialog by calling ShowDialog method
            _ = openFileDialog.ShowDialog();

            // Get the selected file name
            dbLocation = openFileDialog.FileName;

            // Close the window
            this.Close();
        }
    }
}
