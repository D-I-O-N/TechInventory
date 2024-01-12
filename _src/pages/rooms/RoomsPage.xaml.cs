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
using TechInventory._src.database;
using System.Data.SqlClient; // добавили
using System.Data;
using System.Runtime.Remoting.Contexts;

namespace TechInventory._src.pages.rooms
{
    enum RowState
    {
        Existed,
        New,
        Modified,
        ModifiedNew,
        Deleted
    }

    public class Room
    {
        public int ID { get; set; }
        public int RoomNumber { get; set; }
        public string Description { get; set; }
        public bool IsNew { get; set; }
        internal RowState State { get; set; }
    }

    /// <summary>
    /// Логика взаимодействия для RoomsPage.xaml
    /// </summary>
    public partial class RoomsPage : Page
    {
        Entities entities = new Entities();

        int selectedRow;

        public RoomsPage()
        {
            InitializeComponent();
        }

        private void CreateColumns()
        {
            DataGridTextColumn idColumn = new DataGridTextColumn
            {
                Header = "ИД",
                Binding = new Binding("ID")
            };

            DataGridTextColumn roomNumberСolumn = new DataGridTextColumn
            {
                Header = "Номер кабинета",
                Binding = new Binding("RoomNumber")
            };

            DataGridTextColumn descriptionСolumn = new DataGridTextColumn
            {
                Header = "Название",
                Binding = new Binding("Description")
            };

            DataGridTextColumn newСolumn = new DataGridTextColumn
            {
                Header = "IsNew",
                Binding = new Binding("IsNew")
            };

            dataGridView1.Columns.Add(idColumn);
            dataGridView1.Columns.Add(roomNumberСolumn);
            dataGridView1.Columns.Add(descriptionСolumn);
            dataGridView1.Columns.Add(newСolumn);
        }

        private void ReadSingleRow(DataGrid dataGrid, IDataRecord record)
        {
            dataGrid.Items.Add(new Room
            {
                ID = record.GetInt32(0),
                RoomNumber = record.GetInt32(1),
                Description = record.GetString(2),
                IsNew = true // Помечаем как новую запись
            });
        }

        private void RefreshDataGrid(DataGrid dataGrid)
        {
            dataGrid.Items.Clear();

            string queryString = $"select * from Rooms";

            SqlCommand command = new SqlCommand(queryString, (SqlConnection)entities.Database.Connection);

            entities.Database.Connection.Open();

            SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                ReadSingleRow(dataGrid, reader);
            }
            reader.Close();

            entities.Database.Connection.Close();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            CreateColumns();
            RefreshDataGrid(dataGridView1);
        }

        private void dataGridView1_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            // добавлена сортировка и выбор
            if (dataGridView1.SelectedItem != null)
            {
                Room selectedRoom = (Room)dataGridView1.SelectedItem;
                if (selectedRoom != null)
                {
                    txtBoxID.Text = selectedRoom.ID.ToString();
                    txtBoxRoomNumber.Text = selectedRoom.RoomNumber.ToString();
                    txtBoxDescription.Text = selectedRoom.Description;
                }
                else
                {
                    txtBoxID.Text = "не выбрано";
                    txtBoxRoomNumber.Text = "не выбрано";
                    txtBoxDescription.Text = "не выбрано";
                }

            }
        }

        private void refreshImg_MouseDown(object sender, MouseButtonEventArgs e)
        {
            RefreshDataGrid(dataGridView1);
        }

        private void Search(DataGrid dataGrid)
        {
            dataGrid.Items.Clear();

            string searchString = $"select * from Rooms where concat (RoomNumber, Description) like N'%" + txtBoxSearch.Text + "%'";

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

        //private void deleteItem()
        //{
        //    Room selectedRoom = (Room)dataGridView1.SelectedItem;
        //    if (selectedRoom != null)
        //    {
        //        // Установите состояние объекта как удаленный
        //        selectedRoom.State = RowState.Deleted;
        //        // Обновите DataGrid
        //        dataGridView1.Items.Refresh();
        //    }
        //}

        //private void Update()
        //{
        //    entities.Database.Connection.Open();

        //    foreach (Room room in dataGridView1.ItemsSource)
        //    {
        //        var rowState = room.State;

        //        if (rowState == RowState.Existed)
        //        {
        //            continue;
        //        }

        //        if (rowState == RowState.Deleted)
        //        {
        //            var id = room.ID;
        //            var deleteQuery = $"delete from Rooms where ID = {id}";

        //            var command = new SqlCommand(deleteQuery, (SqlConnection)entities.Database.Connection);

        //            command.ExecuteNonQuery();
        //        }
        //    }

        //    entities.Database.Connection.Close();
        //}


        private void DeleteCabinet_Click(object sender, RoutedEventArgs e)
        {

            var selectedItem = dataGridView1.SelectedItem;
            if (selectedItem != null)
            {
                Room selectedRoom = (Room)selectedItem;
                int roomId = selectedRoom.ID;

                // Удаление из базы данных по ID
                var roomToDelete = entities.Rooms.Find(roomId);
                if (roomToDelete != null)
                {
                    entities.Rooms.Remove(roomToDelete);
                    entities.SaveChanges();

                    // Обновление DataGrid
                    RefreshDataGrid(dataGridView1);
                }
            }
        }

        private void txtBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            Search(dataGridView1);
        }

        private void AddCabinet_Click(object sender, RoutedEventArgs e)
        {
            AddRoom_Form createRoomWindow = new AddRoom_Form();
            createRoomWindow.ShowDialog();
            
        }

        private void EditCabinet_Click(object sender, RoutedEventArgs e)
        {
            Room selectedRoom = (Room)dataGridView1.SelectedItem;

            if (selectedRoom != null)
            {
                infoEdit.Visibility = Visibility.Visible;
                btnEdit.Visibility = Visibility.Hidden;
                btnSave.Visibility = Visibility.Visible;
            }
            else
            {
                MessageBox.Show("Кабинет не выбран.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void UpdateRoomInDatabase(Room roomToUpdate)
        {
            
            string updateQuery = $"UPDATE Rooms SET RoomNumber = {roomToUpdate.RoomNumber}, Description = N'{roomToUpdate.Description}' WHERE ID = {roomToUpdate.ID}";

            using (SqlCommand command = new SqlCommand(updateQuery, (SqlConnection)entities.Database.Connection))
            {
                entities.Database.Connection.Open();
                command.ExecuteNonQuery();
                entities.Database.Connection.Close();
            }
        }

        private void UpdateRoomFromFields(Room room)
        {
            // Предполагается, что у вас есть поля для ввода данных на форме,
            // например, txtBoxRoomNumber и txtBoxDescription
            if (int.TryParse(txtBoxRoomNumber.Text, out int roomNumber))
            {
                room.RoomNumber = roomNumber;
            }
            else
            {
                // Обработка ошибки, если введенное значение для RoomNumber не является числом
                MessageBox.Show("Неверный формат номера кабинета.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                // Возможно, вы захотите добавить другую логику обработки ввода данных
                return; // Прерываем метод, так как введенные данные некорректны
            }

            // Обновляем описание из поля ввода
            room.Description = txtBoxDescription.Text;

            // Помечаем запись как измененную
            room.State = RowState.Modified;
        }

        private void ClearFields()
        {
            txtBoxRoomNumber.Text = string.Empty;
            txtBoxDescription.Text = string.Empty;
            // Добавьте другие элементы управления, которые вы хотите очистить
        }

        private void SaveCabinet_Click(object sender, RoutedEventArgs e)
        {
            Room selectedRoom = (Room)dataGridView1.SelectedItem;

            if (selectedRoom != null)
            {
               
                // Обновляем данные в объекте комнаты на основе полей формы
                UpdateRoomFromFields(selectedRoom);

                // Обновляем данные в базе данных
                UpdateRoomInDatabase(selectedRoom);

                // Отключаем режим редактирования
                infoEdit.Visibility = Visibility.Hidden;

                // Включаем кнопку "Редактировать", отключаем кнопку "Сохранить"
                btnEdit.Visibility = Visibility.Visible;
                btnSave.Visibility = Visibility.Hidden;

                // Обновляем DataGrid
                dataGridView1.Items.Refresh();

                // Очищаем поля формы или отключаем режим редактирования
                ClearFields(); // Метод для очистки полей, если необходимо
            }
            else
            {
                MessageBox.Show("Кабинет не выбран.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BackToPage_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

    }
}
