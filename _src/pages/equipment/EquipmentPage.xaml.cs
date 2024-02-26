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
using TechInventory._src.pages.employees;
using TechInventory._src.pages.rooms;

namespace TechInventory._src.pages.equipment
{
    /// <summary>
    /// Логика взаимодействия для EquipmentPage.xaml
    /// </summary>

    enum RowState
    {
        Existed,
        New,
        Modified,
        ModifiedNew,
        Deleted
    }

    public class Equipment
    {
        public int ID { get; set; }
        public string EquipmentName { get; set; }
        public string EquipmentType { get; set; }
        public int Count { get; set; }
        public string SerialNumber { get; set; }
        public DateTime PurchaseDate { get; set; }
        public string Status { get; set; }
        public int RoomID { get; set; }
        public bool IsNew { get; set; }
        public Rooms AssignedRoom { get; internal set; }
        internal RowState State { get; set; }

    }

    public class RoomViewModel
    {
        public int RoomID { get; set; }
        public string Description { get; set; }

        public override string ToString()
        {
            return Description;
        }
    }


    public partial class EquipmentPage : Page
    {
        Entities entities = new Entities();

        public ObservableCollection<RoomViewModel> RoomsList { get; set; } = new ObservableCollection<RoomViewModel>();

        public EquipmentPage()
        {
            InitializeComponent();

        }

        private List<Rooms> GetRoomsFromDatabase()
        {
            using (Entities entities = new Entities())
            {
                return entities.Rooms.ToList();
            }
        }

        private void ComboBoxRooms_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBoxRooms.SelectedItem is RoomViewModel selectedRoom)
            {
                txtBoxRoomID.Text = selectedRoom.RoomID.ToString();
            }
        }

        private void LoadRoomsFromDatabase()
        {
            List<Rooms> roomsFromDatabase = GetRoomsFromDatabase();
            RoomsList.Clear();
            foreach (var room in roomsFromDatabase)
            {
                RoomsList.Add(new RoomViewModel { RoomID = room.ID, Description = room.Description });
            }
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

            DataGridTextColumn statusColumn = new DataGridTextColumn
            {
                Header = "Статус",
                Binding = new Binding("Status")
            };

            DataGridTextColumn roomIDColumn = new DataGridTextColumn
            {
                Header = "ID Аудитории",
                Binding = new Binding("RoomID")
            };
            DataGridTextColumn roomColumn = new DataGridTextColumn
            {
                Header = "Информация о аудитории",
                Binding = new Binding("AssignedRoom.Description") // Предположим, что у кабинета есть свойство RoomInfo
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
            dataGridView1.Columns.Add(statusColumn);
            dataGridView1.Columns.Add(roomIDColumn);
            dataGridView1.Columns.Add(roomColumn);
            dataGridView1.Columns.Add(newColumn);
        }


        private void ReadSingleRow(DataGrid dataGrid, IDataRecord record)
        {
            int roomID = record.GetInt32(7); // Измените на соответствующий индекс столбца для RoomID
            var selRoom = entities.Rooms.FirstOrDefault(r => r.ID == roomID);

            dataGrid.Items.Add(new Equipment
            {
                ID = record.GetInt32(0),
                EquipmentName = record.GetString(1),
                EquipmentType = record.GetString(2),
                Count = record.GetInt32(3),
                SerialNumber = record.GetString(4),
                PurchaseDate = record.GetDateTime(5),
                Status = record.GetString(6),
                RoomID = roomID,
                AssignedRoom = selRoom,
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

            LoadRoomsFromDatabase();
            comboBoxRooms.ItemsSource = RoomsList;

           
        }

        private void txtBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            Search(dataGridView1);
        }

        private void Search(DataGrid dataGrid)
        {
            dataGrid.Items.Clear();

            string searchString = $"SELECT * FROM Equipment WHERE CONCAT(EquipmentName, EquipmentType, Count, SerialNumber, PurchaseDate, Status, RoomID) LIKE N'%{txtBoxSearch.Text}%'";

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

        

        private void UpdateEquipmentInDatabase(Equipment equipmentToUpdate)
        {
            string status = (txtBoxStatus.SelectedItem as ComboBoxItem)?.Content.ToString();

            string updateQuery = $"UPDATE Equipment SET " +
                                 $"EquipmentName = N'{equipmentToUpdate.EquipmentName}', " +
                                 $"EquipmentType = N'{equipmentToUpdate.EquipmentType}', " +
                                 $"Count = {equipmentToUpdate.Count}, " +
                                 $"SerialNumber = N'{equipmentToUpdate.SerialNumber}', " +
                                 $"PurchaseDate = '{equipmentToUpdate.PurchaseDate:s}', " +
                                 $"Status = N'{status}', " +
                                 $"RoomID = {equipmentToUpdate.RoomID} " +
                                 $"WHERE ID = {equipmentToUpdate.ID}";

            using (SqlCommand command = new SqlCommand(updateQuery, (SqlConnection)entities.Database.Connection))
            {
                entities.Database.Connection.Open();
                command.ExecuteNonQuery();
                entities.Database.Connection.Close();
            }
        }

        private void UpdateEquipmentFromFields(Equipment equipment)
        {
            string PurchaseDateInput = txtBoxPurchaseDate.Text;

            // Формат входной строки
            string inputFormat = "dd.MM.yyyy";

            // Преобразование строки в объект DateTime
            if (DateTime.TryParseExact(PurchaseDateInput, inputFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime purchaseDate))
            {
                equipment.PurchaseDate = purchaseDate;
            }
            else
            {
                // Введенное значение не соответствует ожидаемому формату
                MessageBox.Show("Неверный формат даты. Используйте формат 'день.месяц.год час:минута:секунда'", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (int.TryParse(txtBoxRoomID.Text, out int roomID))
            {
                equipment.RoomID = roomID;
            }
            else
            {
                MessageBox.Show("Неверный формат ID кабинета.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            equipment.State = RowState.Modified;
        }

        private void dataGridView1_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            // добавлена сортировка и выбор
            if (dataGridView1.SelectedItem != null)
            {
                Equipment selectedEquipment = (Equipment)dataGridView1.SelectedItem;
                if (selectedEquipment != null)
                {
                    string ID = selectedEquipment.ID.ToString();
                    string Status = selectedEquipment.Status;
                    txtID.Text = "Выбранная запись об оборудовании № " + ID;
                    txtBoxEquipmentName.Text = selectedEquipment.EquipmentName;
                    txtBoxEquipmentType.Text = selectedEquipment.EquipmentType;
                    txtBoxEquipmentCount.Text = selectedEquipment.Count.ToString();
                    

                    // Установка значения статуса
                    if (Status == "Используется")
                    {
                        txtBoxStatus.SetValue(ComboBox.SelectedItemProperty, txtBoxStatus.Items[0]);
                    }
                    else if (Status == "Доступно")
                    {
                        txtBoxStatus.SetValue(ComboBox.SelectedItemProperty, txtBoxStatus.Items[1]);
                    }

                    //txtBoxStatus.Text = selectedEquipment.Status.ToString();
                    txtBoxSN.Text = selectedEquipment.SerialNumber.ToString();
                    txtBoxPurchaseDate.Text = selectedEquipment.PurchaseDate.ToString("dd.MM.yyyy");
                    txtBoxRoomID.Text = selectedEquipment.RoomID.ToString();
   
                    //comboBoxRooms.IsEnabled = true;
                    comboBoxRooms.SelectedItem = selectedEquipment.AssignedRoom.Description.ToString();
                }
                else
                {
                    txtID.Text = "Выбранная запись об оборудовании № ....";
                    txtBoxEquipmentName.Text = "Не выбрано";
                    txtBoxEquipmentType.Text = "Не выбрано";
                    txtBoxEquipmentCount.Text = "Не выбрано";
                    //txtBoxStatus.Text = "Не выбрано";
                    txtBoxSN.Text = "Не выбрано";
                    txtBoxPurchaseDate.Text = "Не выбрано";
                    txtBoxRoomID.Text = "Не выбрано";
                    comboBoxRooms.IsEnabled = false;
                    comboBoxRooms.SelectedItem = null;
                }
            }
        }

        private bool ValidateEquipment(Equipment equipment)
        {
            // Добавьте здесь логику проверки
            // Например, проверка на пустые поля, корректность даты и т.д.

            // Пример проверки на пустые поля
            if (string.IsNullOrEmpty(equipment.EquipmentName) || string.IsNullOrEmpty(equipment.EquipmentType) || string.IsNullOrEmpty(equipment.Status) || string.IsNullOrEmpty(equipment.SerialNumber) || string.IsNullOrEmpty(txtBoxPurchaseDate.Text) || string.IsNullOrEmpty(txtBoxRoomID.Text))
            {
                MessageBox.Show("Заполните все обязательные поля правильно.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            // Добавьте другие проверки, которые вам необходимы

            return true; // Если все проверки прошли успешно
        }

        private void SaveEquipment_Click(object sender, RoutedEventArgs e)
        {
            Equipment selectedEquipment = (Equipment)dataGridView1.SelectedItem;

            if (selectedEquipment != null)
            {
                if (ValidateEquipment(selectedEquipment)) // Добавлен вызов метода проверки
                {
                    UpdateEquipmentFromFields(selectedEquipment);
                    UpdateEquipmentInDatabase(selectedEquipment);

                    infoEdit.Visibility = Visibility.Hidden;
                    btnEdit.Visibility = Visibility.Visible;
                    btnSave.Visibility = Visibility.Hidden;

                    dataGridView1.Items.Refresh();
                    ClearFields();

                    comboBoxRooms.IsEnabled = false;
                    comboBoxRooms.SelectedItem = null;
                }
                // Если проверка не прошла, ничего не делаем
            }
            else
            {
                MessageBox.Show("Оборудование не выбрано.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void ClearFields()
        {
            txtBoxEquipmentName.Text = string.Empty;
            txtBoxEquipmentType.Text = string.Empty;
            txtBoxEquipmentCount.Text = string.Empty;
            txtBoxStatus.Text = string.Empty;
            txtBoxSN.Text = string.Empty;
            txtBoxPurchaseDate.Text = string.Empty;
            txtBoxRoomID.Text = string.Empty;
        }

        private void clearImg_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ClearFields();
        }

        private void AddEquipment_Click(object sender, RoutedEventArgs e)
        {
            AddEquipment_Form createEquipmentWindow = new AddEquipment_Form();
            createEquipmentWindow.ShowDialog();
        }

        private void EditEquipment_Click(object sender, RoutedEventArgs e)
        {
            Equipment selectedEquipment = (Equipment)dataGridView1.SelectedItem;

            if (selectedEquipment != null)
            {

                infoEdit.Visibility = Visibility.Visible;
                btnEdit.Visibility = Visibility.Hidden;
                btnSave.Visibility = Visibility.Visible;
                comboBoxRooms.IsEnabled = true;
            }
            else
            {
                MessageBox.Show("Сотрудник не выбран.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void DeleteEquipment_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = dataGridView1.SelectedItem;
            if (selectedItem != null)
            {
                Equipment selectedEquipment = (Equipment)selectedItem;
                int EquipmentId = selectedEquipment.ID;

                // Удаление из базы данных по ID
                var EquipmentToDelete = entities.Employees.Find(EquipmentId);
                if (EquipmentToDelete != null)
                {
                    entities.Employees.Remove(EquipmentToDelete);
                    entities.SaveChanges();

                    // Обновление DataGrid
                    RefreshDataGrid(dataGridView1);
                    ClearFields();
                }
            }
        }

        private void BackToPage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            NavigationService.GoBack();
        }
    }

}
