using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms.Integration;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TechInventory._src.pages.employees;
using TechInventory._src.pages.equipment;
using TechInventory._src.pages.equipmentHistory;
using TechInventory._src.pages.rooms;
using TechInventory._src.pages.statistics;

namespace TechInventory._src.pages
{
    /// <summary>
    /// Логика взаимодействия для MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void PageRoomsOpen_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new RoomsPage());
        }

        private void PageEmployeeOpen_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new EmployeesPage());
        }

        private void PageEquipmentOpen_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new EquipmentPage());
        }

        private void PageEquipmentHistoryOpen_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new EquipmentHistoryPage());
        }

        private void OpenStatic_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new EquipmentReportPage());
        }

    }
}
