using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Data;
using System.Globalization;
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
using TechInventory._src.pages.equipment;
using System.ComponentModel;
using Equipment = TechInventory._src.pages.equipment.Equipment;
using CheckBox = System.Windows.Controls.CheckBox;
using Style = System.Windows.Style;

namespace TechInventory._src.pages.statistics
{
    /// <summary>
    /// Логика взаимодействия для EquipmentPageForAct.xaml
    /// </summary>
    public partial class EquipmentPageForAct : Page
    {

        public event PropertyChangedEventHandler PropertyChanged;

        private bool isSelected;
        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                if (isSelected != value)
                {
                    isSelected = value;
                    OnPropertyChanged(nameof(IsSelected));
                }
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox checkBox && checkBox.DataContext is Equipment equipment)
            {
                equipment.IsSelected = true;
            }
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox checkBox && checkBox.DataContext is Equipment equipment)
            {
                equipment.IsSelected = false;
            }
        }



        Entities entities = new Entities();

        public ObservableCollection<RoomViewModel> RoomsList { get; set; } = new ObservableCollection<RoomViewModel>();

        public EquipmentPageForAct()
        {
            InitializeComponent();

        }


        private void CreateColumns()
        {
            DataGridTextColumn idColumn = new DataGridTextColumn
            {
                Header = "ID",
                Binding = new Binding("ID")
            };

            DataGridTextColumn equipmentNameColumn = new DataGridTextColumn
            {
                Header = " Название оборудования ",
                Binding = new Binding("EquipmentName")
            };

            DataGridTextColumn equipmentTypeColumn = new DataGridTextColumn
            {
                Header = " Тип оборудования ",
                Binding = new Binding("EquipmentType")
            };

            DataGridTextColumn countColumn = new DataGridTextColumn
            {
                Header = " Кол-во ",
                Binding = new Binding("Count")
            };

            DataGridTextColumn serialNumberColumn = new DataGridTextColumn
            {
                Header = " Серийный номер ",
                Binding = new Binding("SerialNumber")
            };

            DataGridTextColumn purchaseDateColumn = new DataGridTextColumn
            {
                Header = "Дата покупки",
                Binding = new Binding("PurchaseDate")

            };

            DataGridCheckBoxColumn checkBoxColumn = new DataGridCheckBoxColumn
            {
                Header = "Выбор",
                Binding = new Binding("IsSelected")

            };


            DataGridTextColumn newColumn = new DataGridTextColumn
            {
                Header = "IsNew",
                Binding = new Binding("IsNew")
            };

            dataGridView1.Columns.Add(idColumn);
            dataGridView1.Columns.Add(equipmentNameColumn);
            dataGridView1.Columns.Add(equipmentTypeColumn);
            dataGridView1.Columns.Add(countColumn);
            dataGridView1.Columns.Add(serialNumberColumn);
            dataGridView1.Columns.Add(purchaseDateColumn);
            dataGridView1.Columns.Add(checkBoxColumn);
            dataGridView1.Columns.Add(newColumn);
        }


        private void ReadSingleRow(DataGrid dataGrid, IDataRecord record)
        {
            int roomID = record.GetInt32(7); // Измените на соответствующий индекс столбца для RoomID
            var selRoom = entities.Rooms.FirstOrDefault(r => r.ID == roomID);

            dataGrid.Items.Add(new equipment.Equipment
            {
                ID = record.GetInt32(0),
                EquipmentName = record.GetString(1),
                EquipmentType = record.GetString(2),
                Count = record.GetInt32(3),
                SerialNumber = record.GetString(4),
                PurchaseDate = record.GetDateTime(5),

                IsNew = true
            });
        }

        private void refreshImg_MouseDown(object sender, MouseButtonEventArgs e)
        {
            RefreshDataGrid(dataGridView1);
        }

        //try: Этот оператор используется для обработки кода, который может вызвать исключения(ошибки) 
        //    во время выполнения.Если внутри блока try возникает исключение, выполнение переходит к блоку catch.


        private void RefreshDataGrid(DataGrid dataGrid)
        {
            try
            {
                dataGrid.Items.Clear();

                string queryString = "select * from Equipment";

                SqlCommand command = new SqlCommand(queryString, (SqlConnection)entities.Database.Connection);

                entities.Database.Connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    ReadSingleRow(dataGrid, reader);
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                entities.Database.Connection.Close(); /*Закрывает соединение с базой данных, даже если произошло исключение в блоке try. 
                                                       * Это важно для правильной работы с ресурсами и предотвращения утечек.*/
            }
        }

        //цикл while, чтобы прочитать каждую строку данных из результата запроса.
        //Для каждой строки вызывается метод ReadSingleRow, который, вероятно, добавляет данные в DataGrid.

        //finally: Этот блок содержит код, который выполняется всегда, независимо от того, произошло исключение или нет.
        //В данном случае, он закрывает соединение с базой данных в блоке finally.


        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            CreateColumns();

            (dataGridView1.Columns[5] as DataGridTextColumn).Binding.StringFormat = "dd.MM.yyyy";

            RefreshDataGrid(dataGridView1);

            foreach (var item in dataGridView1.Items)
            {
                if (item is DataRowView row)
                {
                    CheckBox checkBox = FindVisualChild<CheckBox>(row) as CheckBox;
                    if (checkBox != null)
                    {
                        checkBox.Checked += CheckBox_Checked;
                        checkBox.Unchecked += CheckBox_Unchecked;
                    }
                }
            }
        }

        private T FindVisualChild<T>(DataRowView row)
        {
            throw new NotImplementedException();
        }

        private DependencyObject FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);
                if (child != null && child is T)
                {
                    return child;
                }
                else
                {
                    DependencyObject childOfChild = FindVisualChild<T>(child);
                    if (childOfChild != null)
                    {
                        return childOfChild;
                    }
                }
            }
            return null;

        }

        private void txtBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            Search(dataGridView1);
        }

        private void Search(DataGrid dataGrid)
        {
            dataGrid.Items.Clear();

            string searchString = $"SELECT * FROM Equipment WHERE CONCAT(EquipmentName, EquipmentType, Count, SerialNumber, PurchaseDate) LIKE N'%{txtBoxSearch.Text}%'";

            SqlCommand com = new SqlCommand(searchString, (SqlConnection)entities.Database.Connection);

            entities.Database.Connection.Open();

            SqlDataReader reader = com.ExecuteReader();

            while (reader.Read())
            {
                ReadSingleRow(dataGrid, reader);
            }
            reader.Close();

            entities.Database.Connection.Close();
        }


        private void ClearFields()
        {

        }

        private void clearImg_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ClearFields();
        }


        private void BackToPage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void SaveEquipment_Click(object sender, RoutedEventArgs e)
        {

        }
    }

}

