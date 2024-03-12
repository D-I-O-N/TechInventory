using DocumentFormat.OpenXml.Packaging;
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
using LiveCharts;
using LiveCharts.Wpf;
using System.Data.Entity;
using DocumentFormat.OpenXml.Drawing.Charts;
using TechInventory._src.pages.employees;
using TechInventory._src.pages.rooms;

namespace TechInventory._src.pages.statistics
{
    /// <summary>
    /// Логика взаимодействия для PageStatisticReport.xaml
    /// </summary>
    /// 


    
    public partial class PageStatisticReport : Page
    {
        Entities entities = new Entities();
        public SeriesCollection SeriesCollection { get; set; }
        public Func<ChartPoint, string> Pointlabel { get; set; }
        //public SeriesCollection seriesCollection = new SeriesCollection();
        //public List<Entities> Equipment { get; set; }
        //public List<Entities> Rooms { get; set; }
        //public List<Entities> Employees { get; set; }

        public PageStatisticReport()
        {
            InitializeComponent();
            pieChart();
            doughnut();
        }

        public void pieChart()
        {
            Pointlabel = chartPoint => string.Format("{0}({1:P})", chartPoint.Y, chartPoint.Participation);
            DataContext = this;
        }


        private IList<Equipment> GetEquipmentData()
        {
            // Здесь должен быть ваш код для получения данных из таблицы оборудования
            // Пример:
            return entities.Equipment.ToList();
            //return null;
        }

        private IList<Rooms> GetRoomData()
        {
            // Здесь должен быть ваш код для получения данных из таблицы аудиторий
            // Пример:
            return entities.Rooms.ToList();
            //return null;
        }

        private IList<Employees> GetEmployeeData()
        {
            // Здесь должен быть ваш код для получения данных из таблицы преподавателей
            // Пример:
            return entities.Employees.ToList();
            //return null;
        }

        public void doughnut()
        {

            IList<Equipment> equipmentList = GetEquipmentData();
            IList<Rooms> roomList = GetRoomData();
            IList<Employees> employeeList = GetEmployeeData();

            //Создание коллекций значений для графика
            ChartValues<int> equipmentValues = new ChartValues<int> { equipmentList.Count };
            ChartValues<int> roomValues = new ChartValues<int> { roomList.Count };
            ChartValues<int> employeeValues = new ChartValues<int> { employeeList.Count };

            // //Создание серий графика для каждой категории данных
            //PieSeries equipmentSeries = new PieSeries
            //{
            //    Title = "Оборудование",
            //    Values = equipmentValues,
            //    DataLabels = true
            //};

            // PieSeries roomSeries = new PieSeries
            // {
            //     Title = "Аудитории",
            //     Values = roomValues,
            //     DataLabels = true
            // };

            // PieSeries employeeSeries = new PieSeries
            // {
            //     Title = "Преподаватели",
            //     Values = employeeValues,
            //     DataLabels = true
            // };

            // // Добавление серий в коллекцию Series для отображения на графике
            // SeriesCollection SeriesCollection = new SeriesCollection { equipmentSeries, roomSeries, employeeSeries };

            // Привязка коллекции серий к графику




            SeriesCollection = new SeriesCollection
            {
                new PieSeries
                {
                    Title="Оборудование",
                    Values=equipmentValues,
                    DataLabels=true,
                },
                new PieSeries
                {
                    Title="Аудитории",
                    Values=roomValues,
                    DataLabels=true,
                },
                new PieSeries
                {
                    Title="Преподаватели",
                    Values=employeeValues,
                    DataLabels=true,
                }

            };

            //// Получить количество строк в коллекции Equipment
            //int equipmentCount = entities.Equipment.Count();

            //// Получить количество аудиторий
            //int roomsCount = entities.Rooms.Count();

            //// Получить количество преподавателей
            //int employeesCount = entities.Employees.Count();

            //// Создать коллекцию значений для графика
            //ChartValues<int> values = new ChartValues<int> { equipmentCount, roomsCount, employeesCount };

            //// Создать объекты PieSeries и добавить коллекцию значений

            //PieSeries equipmentSeries = new PieSeries
            //{
            //    Title = "Оборудование",
            //    Values = new ChartValues<int> { equipmentCount },
            //    DataLabels = true
            //};

            //PieSeries roomsSeries = new PieSeries
            //{
            //    Title = "Аудитории",
            //    Values = new ChartValues<int> { roomsCount },
            //    DataLabels = true
            //};

            //PieSeries employeesSeries = new PieSeries
            //{
            //    Title = "Преподаватели",
            //    Values = new ChartValues<int> { employeesCount },
            //    DataLabels = true
            //};

            //// Добавить PieSeries в коллекцию Series
            //SeriesCollection seriesCollection = new SeriesCollection { equipmentSeries, roomsSeries, employeesSeries };
        }


        private void BackToPage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void EqAct_Click(object sender, RoutedEventArgs e)
        {
            Report report = new Report();
            report.CheckTechGen(entities.Equipment.ToList());
        }

        private void EqList_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new EquipmentReportPage());
        }
    }
}
