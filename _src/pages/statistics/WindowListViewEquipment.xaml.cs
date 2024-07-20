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
using TechInventory._src.database;

namespace TechInventory._src.pages.statistics
{
    /// <summary>
    /// Логика взаимодействия для WindowListViewEquipment.xaml
    /// </summary>
    /// 

    public class EquipmentItem
    {
        public string EquipmentName { get; set; }
        public string EquipmentType { get; set; }
        public string SerialNumber { get; set; }
        public int Count { get; set; }
        public DateTime PurchaseDate { get; set; }
        public bool IsSelected { get; set; } // Для чекбокса
    }

    public partial class WindowListViewEquipment : Window
    {
        public WindowListViewEquipment()
        {
            InitializeComponent();

            // Получение данных из базы данных
            List<EquipmentItem> equipmentList = GetEquipmentItemsFromDatabase();

            // Привязка списка к ListView
            equipmentListView.ItemsSource = equipmentList;

        }

        public List<EquipmentItem> GetEquipmentItemsFromDatabase()
        {
            List<EquipmentItem> equipmentList = new List<EquipmentItem>();

            using (var dbContext = new Entities())
            {

                var dbEquipmentItems = dbContext.Equipment.ToList();

                foreach (var dbEquipmentItem in dbEquipmentItems)
                {
                    EquipmentItem equipmentItem = new EquipmentItem
                    {
                        EquipmentName = dbEquipmentItem.EquipmentName,
                        EquipmentType = dbEquipmentItem.EquipmentType,
                        Count = (int)dbEquipmentItem.Count,
                        PurchaseDate = ((DateTime)dbEquipmentItem.PurchaseDate).Date,
                        SerialNumber = dbEquipmentItem.SerialNumber,
                        IsSelected = false

                    };
                    equipmentList.Add(equipmentItem);
                }
            }

            return equipmentList;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            Report report = new Report();
            //report.CheckTechGen(entities.Equipment.ToList());
            List<EquipmentItem> selectedItems = GetSelectedItems();
            report.CheckTechGen(selectedItems);

            WindowListViewEquipment windowListViewEquipment = new WindowListViewEquipment();
            windowListViewEquipment.Close();
        }

        private List<EquipmentItem> GetSelectedItems()
        {
            List<EquipmentItem> selectedItems = new List<EquipmentItem>();

            // Проходим по всем элементам в ListView
            foreach (EquipmentItem item in equipmentListView.Items)
            {
                // Если элемент отмечен, добавляем его в список отмеченных элементов
                if (item.IsSelected)
                {
                    selectedItems.Add(item);
                }
            }

            return selectedItems;
        }

    }
}
