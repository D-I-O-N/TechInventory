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
using System.Data;
using System.IO;
using DocumentFormat.OpenXml.Spreadsheet;

namespace TechInventory._src.pages.statistics
{
    /// <summary>
    /// Логика взаимодействия для PageStatisticReport.xaml
    /// </summary>
    /// 


    
    public partial class PageStatisticReport 
    {
        Entities entities = new Entities();
        public SeriesCollection SeriesCollection { get; set; }
        public Func<ChartPoint, string> Pointlabel { get; set; }


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
            return entities.Equipment.ToList();
        }

        private IList<Rooms> GetRoomData()
        {
            return entities.Rooms.ToList();
        }

        private IList<Employees> GetEmployeeData()
        {
            return entities.Employees.ToList();
        }

        public void doughnut()
        {

            IList<Equipment> equipmentList = GetEquipmentData();
            IList<Rooms> roomList = GetRoomData();
            IList<Employees> employeeList = GetEmployeeData();

            ChartValues<int> equipmentValues = new ChartValues<int> { equipmentList.Count };
            ChartValues<int> roomValues = new ChartValues<int> { roomList.Count };
            ChartValues<int> employeeValues = new ChartValues<int> { employeeList.Count };

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

        }


        private void BackToPage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void EqAct_Click(object sender, RoutedEventArgs e)
        {

            WindowListViewEquipment equipmentWindow = new WindowListViewEquipment();
            equipmentWindow.Show();
        }

        private void EqList_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                var path = System.IO.Path.Combine(desktopPath, "Список оборудования.csv");

                var rows = entities.Equipment.ToList();

                var lines = new List<string>();

                lines.Add("Название оборудования;Тип;Количество;Серийный номер;Дата покупки");

                // Преобразуем каждый объект Equipment в строку CSV и добавляем в список lines
                foreach (var equipment in rows)
                {
                    DateTime purchaseDate = (DateTime)equipment.PurchaseDate;
                    string formattedDate = purchaseDate.ToString("dd-MM-yyyy");

                    // Преобразуем кириллические символы в кодировку UTF-8
                    var equipmentName = Encoding.UTF8.GetBytes(equipment.EquipmentName);
                    var equipmentType = Encoding.UTF8.GetBytes(equipment.EquipmentType);

                    // Формируем строку CSV с учетом кодировки UTF-8
                    var line = $"{Encoding.UTF8.GetString(equipmentName)};{Encoding.UTF8.GetString(equipmentType)};{equipment.Count};{equipment.SerialNumber};{formattedDate}";
                    lines.Add(line);
                }

                File.WriteAllLines(path, lines, Encoding.UTF8);

                MessageBox.Show("Данные об оборудовании успешно экспортированы в CSV файл. Открывайте документ со списком через программу Excel.", "Успешное выполнение", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при экспорте данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RoomsList_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                var path = System.IO.Path.Combine(desktopPath, "Список аудиторий.csv");

                var rows = entities.Rooms.ToList();
                var lines = new List<string>();

                lines.Add("ID;Номер аудитории;Описание аудитории");

                foreach (var rooms in rows)
                {

                    // Преобразуем кириллические символы в кодировку UTF-8
                    var roomsDescription = Encoding.UTF8.GetBytes(rooms.Description);

                    // Формируем строку CSV с учетом кодировки UTF-8
                    var line = $"{rooms.ID};{rooms.RoomNumber};{Encoding.UTF8.GetString(roomsDescription)}";
                    lines.Add(line);
                }

                File.WriteAllLines(path, lines, Encoding.UTF8);

                MessageBox.Show("Данные об аудиториях успешно экспортированы в CSV файл. Открывайте документ со списком через программу Excel.", "Успешное выполнение", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при экспорте данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EmployeesList_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                var path = System.IO.Path.Combine(desktopPath, "Список сотрудников.csv");

                var rows = entities.Employees.ToList();
                var roomRows = entities.Rooms.ToList();

                var lines = new List<string>();
                lines.Add("ID;Имя;Фамилия;Должность;Аудитория;Номер аудитории");

                // Преобразуем каждый объект Equipment в строку CSV и добавляем в список lines
                foreach (var employees in rows)
                {
                    foreach (var rooms in roomRows) 
                    {
                        if (rooms.ID == employees.RoomID) 
                        {
                            var firstName = Encoding.UTF8.GetBytes(employees.FirstName);
                            var lastName = Encoding.UTF8.GetBytes(employees.LastName);
                            var position = Encoding.UTF8.GetBytes(employees.Position);
                            var room = Encoding.UTF8.GetBytes(rooms.Description);

                            // Формируем строку CSV с учетом кодировки UTF-8
                            var line = $"{employees.ID};{Encoding.UTF8.GetString(firstName)};{Encoding.UTF8.GetString(lastName)};{Encoding.UTF8.GetString(position)};{Encoding.UTF8.GetString(room)};{rooms.RoomNumber}";
                            lines.Add(line);
                        }
                    }
                }

                // Записываем строки в файл CSV с кодировкой UTF-8
                File.WriteAllLines(path, lines, Encoding.UTF8);

                MessageBox.Show("Данные об сотрудниках успешно экспортированы в CSV файл. Открывайте документ со списком через программу Excel.", "Успешное выполнение", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при экспорте данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void HistoryList_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                var path = System.IO.Path.Combine(desktopPath, "Список истории оборудования.csv");

                var rows = entities.Employees.ToList();
                var equipmentRows = entities.Equipment.ToList();
                var historyRows = entities.EquipmentHistory.ToList();

                // Создаем список строк для записи в файл
                var lines = new List<string>();

                // Добавляем заголовок CSV файла
                lines.Add("ID;Оборудование;Дата выдачи;Сотрудник;Дата возврата");

                // Преобразуем каждый объект Equipment в строку CSV и добавляем в список lines
                foreach (var employees in rows)
                {
                    foreach (var equipment in equipmentRows)
                    {
                        foreach (var history in historyRows)
                        {
                            if (equipment.ID == history.EquipmentID && employees.ID == history.EmployeeID) 
                            {
                                // Преобразуем кириллические символы в кодировку UTF-8
                                var equipmentHistory = Encoding.UTF8.GetBytes(equipment.EquipmentName);
                                var lastName = Encoding.UTF8.GetBytes(employees.LastName);

                                // Формируем строку CSV с учетом кодировки UTF-8
                                var line = $"{history.ID};{Encoding.UTF8.GetString(equipmentHistory)};{history.CheckoutDate};{Encoding.UTF8.GetString(lastName)};{history.ReturnDate}";
                                lines.Add(line);
                            }
                        }
                    }
                }

                // Записываем строки в файл CSV с кодировкой UTF-8
                File.WriteAllLines(path, lines, Encoding.UTF8);

                MessageBox.Show("Данные об истории оборудования успешно экспортированы в CSV файл. Открывайте документ со списком через программу Excel.", "Успешное выполнение", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при экспорте данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
