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
using DocumentFormat.OpenXml.Drawing.Charts;


namespace TechInventory._src.pages.statistics
{
    /// <summary>
    /// Логика взаимодействия для TypeEquipmentStatisticPage.xaml
    /// </summary>

    //public class EquipmentData
    //{
    //    public string EquipmentType { get; set; }
    //    public int Count { get; set; }
    //}

    public partial class TypeEquipmentStatisticPage : Page
    {
    //    PieChart.Series = seriesCollection;
    //    PieChart.LegendLocation = LegendLocation.Right;


    //        private List<EquipmentData> GetDataFromDatabase() { 
        
    //        // Здесь вам нужно запросить данные из базы данных
    //        // и заполнить коллекцию equipmentData
    //        // Замените этот код на ваш собственный запрос к базе данных
    //        // Пример:
    //        var equipmentData = new List<EquipmentData>
    //        {
    //            new EquipmentData { EquipmentType = "Type1", Count = 10 },
    //            new EquipmentData { EquipmentType = "Type2", Count = 20 },
    //            new EquipmentData { EquipmentType = "Type3", Count = 15 },
    //        };

    //        return equipmentData;
    //    }

    //    public TypeEquipmentStatisticPage()
    //    {
    //        InitializeComponent();


    //        // Получите данные об оборудовании из базы данных (пример)
    //        var equipmentData = GetDataFromDatabase();

    //        // Создайте Series для круговой диаграммы
    //        SeriesCollection seriesCollection = new SeriesCollection();
    //        foreach (var equipmentType in equipmentData.Select(e => e.EquipmentType).Distinct())
    //        {
    //            var count = equipmentData.Where(e => e.EquipmentType == equipmentType).Sum(e => e.Count);
    //            seriesCollection.Add(new PieSeries
    //            {
    //                Title = equipmentType,
    //                Values = new ChartValues<int> { count }
    //            });
    //        }
    //    }
    }
}
