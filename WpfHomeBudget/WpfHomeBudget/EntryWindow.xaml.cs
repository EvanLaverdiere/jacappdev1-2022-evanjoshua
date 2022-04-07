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
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;

namespace WpfHomeBudget
{
    /// <summary>
    /// Interaction logic for EntryWindow.xaml
    /// </summary>
    public partial class EntryWindow : Window
    {
        public string dbLocation { get; private set; }

        /// <summary>
        /// True if the window is creating a new database, false if it is loading an existing one.
        /// </summary>
        public bool IsNewDatabase { get; private set; }
        public EntryWindow(bool isDarMode)
        {
            if (isDarMode)
            {
                turnDark();
            }
            
            InitializeComponent();
        }

        private void CreateDbBtn_Click(object sender, RoutedEventArgs e)
        {
            NewBudgetWindow newBudgetWindow = new NewBudgetWindow();
            newBudgetWindow.ShowDialog();
            if (newBudgetWindow.path != string.Empty)
            {
                dbLocation = newBudgetWindow.path;
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

            if (dbLocation != string.Empty)
            {
                IsNewDatabase = false;
                this.Close();
            }
        }

        public void turnDark()
        {
            Properties.Settings.Default.ThemeColor = "DarkMode";

            Properties.Settings.Default.Save();
        }
    }
}
