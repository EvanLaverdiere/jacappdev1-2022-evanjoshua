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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Budget;



namespace WpfHomeBudget
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string directory;
        public MainWindow()
        {
            // Create the entry window
            EntryWindow entryWindow = new EntryWindow();

            // Open the new entry window
            _ = entryWindow.ShowDialog();

            // Get the directory that the user gave in
            directory = entryWindow.dbDirectory;

            InitializeComponent();
        }

    }
}
