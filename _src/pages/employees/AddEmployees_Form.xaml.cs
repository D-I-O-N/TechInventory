using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
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
using static TechInventory._src.pages.employees.EmployeesPage;

namespace TechInventory._src.pages.employees
{
    /// <summary>
    /// Логика взаимодействия для AddEmployees_Form.xaml
    /// </summary>

    public class RoomViewModel
    {
        public int RoomID { get; set; }
        public string Description { get; set; }

        public override string ToString()
        {
            return Description;
        }
    }

    public partial class AddEmployees_Form : Window
    {
        Entities entities = new Entities();

        public AddEmployees_Form()
        {
            InitializeComponent();
        }

        public ObservableCollection<RoomViewModel> RoomsList { get; set; } = new ObservableCollection<RoomViewModel>();

        private List<Rooms> GetRoomsFromDatabase()
        {
            using (Entities entities = new Entities())
            {
                // Предположим, что у вас есть DbSet<Rooms> в вашем классе Entities
                return entities.Rooms.ToList();
            }
        }

        private void ComboBoxRooms_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBoxRooms.SelectedItem is RoomViewModel selectedRoom)
            {
                txtRoomID.Text = selectedRoom.RoomID.ToString();
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

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadRoomsFromDatabase();
            comboBoxRooms.ItemsSource = RoomsList;
        }

        private void AddEmployeeFormButton_Click(object sender, RoutedEventArgs e)
        {
            entities.Database.Connection.Open();

            string firstName = txtFirstName.Text;
            string lastName = txtLastName.Text;
            string position = txtPosition.Text;
            int roomID;

            if (int.TryParse(txtRoomID.Text, out roomID))
            {
                var addQuery = $"INSERT INTO Employees (FirstName, LastName, Position, RoomID) " +
                               $"VALUES (N'{firstName}', N'{lastName}', N'{position}', '{roomID}')";

                SqlCommand command = new SqlCommand(addQuery, (SqlConnection)entities.Database.Connection);
                command.ExecuteReader();

                MessageBox.Show("Сотрудник успешно добавлен!!", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("ID кабинета должен иметь числовой формат!!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            entities.Database.Connection.Close();
        }


        private void ClearFields()
        {
            txtFirstName.Text = string.Empty;
            txtLastName.Text = string.Empty;
            txtPosition.Text = string.Empty;
            txtRoomID.Text = string.Empty;
        }

        private void AddEmployeeFormButton_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            ClearFields();
        }
    }
}
