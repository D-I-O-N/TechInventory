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
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            CreateColumns();
            RefreshDataGrid(dataGridView1);
        }

        private void dataGridView1_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {

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

        private void AddCabinet_Click(object sender, RoutedEventArgs e)
        {

        }

        private void EditCabinet_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DeleteCabinet_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SaveCabinet_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BackToPage_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void refreshImg_MouseDown(object sender, MouseButtonEventArgs e)
        {
            RefreshDataGrid(dataGridView1);
        }
    }
}
