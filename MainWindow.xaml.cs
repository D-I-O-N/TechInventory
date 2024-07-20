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
using TechInventory._src.pages;
using TechInventory._src.pages.rooms;

namespace TechInventory
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            //MainFrameNavigation.Navigate(new MainPage());
            MainFrameNavigation.Navigate(new AuthorizationPage());
            //MainFrameNavigation.Navigate(new RoomsPage());

        }
    }
}
