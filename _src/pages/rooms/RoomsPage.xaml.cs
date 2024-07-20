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
using TechInventory._src.database;
using System.Data.SqlClient;
using System.Data;
using System.Runtime.Remoting.Contexts;
using TechInventory._src.pages.employees;

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
        //int selectedRow;

        public RoomsPage()
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

            DataGridTextColumn roomNumberСolumn = new DataGridTextColumn
            {
                Header = "Номер аудитории",
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
        }

        private void ReadSingleRow(DataGrid dataGrid, IDataRecord record)
        {
            dataGrid.Items.Add(new Room
            {
                ID = record.GetInt32(0),
                RoomNumber = record.GetInt32(1),
                Description = record.GetString(2),
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
                    string ID = selectedRoom.ID.ToString();
                    txtID.Text = "Выбранная запись о аудитории № " + ID;
                    txtBoxRoomNumber.Text = selectedRoom.RoomNumber.ToString();
                    txtBoxDescription.Text = selectedRoom.Description;
                }
                else
                {
                    txtID.Text = "Выбранная запись о аудитории № ....";
                    txtBoxRoomNumber.Text = "Не выбрано";
                    txtBoxDescription.Text = "Не выбрано";
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
                    ClearFields();
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
                txtBoxRoomNumber.IsReadOnly = false;
                txtBoxDescription.IsReadOnly = false;
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
            if (int.TryParse(txtBoxRoomNumber.Text, out int roomNumber))
            {
                room.RoomNumber = roomNumber;
            }
            else
            {
                MessageBox.Show("Неверный формат номера кабинета.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            room.Description = txtBoxDescription.Text;
            room.State = RowState.Modified;
        }

        private void ClearFields()
        {
            txtBoxRoomNumber.Text = string.Empty;
            txtBoxDescription.Text = string.Empty;
        }

        private bool ValidateEquipment(Room room)
        {
            if (string.IsNullOrEmpty(room.Description) || string.IsNullOrEmpty(txtBoxRoomNumber.Text))
            {
                MessageBox.Show("Заполните все обязательные поля правильно.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
        }

        private void SaveCabinet_Click(object sender, RoutedEventArgs e)
        {
            Room selectedRoom = (Room)dataGridView1.SelectedItem;

            if (selectedRoom != null)
            {
                if (ValidateEquipment(selectedRoom))
                {
                    UpdateRoomFromFields(selectedRoom);
                    UpdateRoomInDatabase(selectedRoom);

                    infoEdit.Visibility = Visibility.Hidden;
                    btnEdit.Visibility = Visibility.Visible;
                    btnSave.Visibility = Visibility.Hidden;

                }

                dataGridView1.Items.Refresh();
                ClearFields();
            }
            else
            {
                MessageBox.Show("Кабинет не выбран.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void clearImg_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ClearFields();
        }

        private void txtBoxRoomNumber_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null)
            {
                string text = textBox.Text;
                string filteredText = new string(text.Where(char.IsDigit).ToArray());

                if (filteredText != text)
                {
                    textBox.Text = filteredText;
                    textBox.CaretIndex = textBox.Text.Length;
                }
            }
        }

        private void BackToPage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
