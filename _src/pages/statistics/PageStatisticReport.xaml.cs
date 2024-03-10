using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using TechInventory._src.database;


namespace TechInventory._src.pages.statistics
{
    /// <summary>
    /// Логика взаимодействия для PageStatisticReport.xaml
    /// </summary>
    public partial class PageStatisticReport : Page
    {
        Entities entities = new Entities();
        public PageStatisticReport()
        {
            InitializeComponent();
        }

        private void BackToPage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            NavigationService.GoBack();
        }

        //private void EqCertificate_Click(object sender, RoutedEventArgs e)
        //{


        //}

        private void EqAct_Click(object sender, RoutedEventArgs e)
        {
            Report report = new Report();

            report.CheckTechGen(entities.Equipment.ToList());
        }
    }
}
