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
using TechInventory._src.pages.rooms;
using System.Data.SqlClient;
using System.Data;
using TechInventory._src.database;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace TechInventory._src.pages.employees
{
    /// <summary>
    /// Логика взаимодействия для EmployeesPage.xaml
    /// </summary>

    enum RowState
    {
        Existed,
        New,
        Modified,
        ModifiedNew,
        Deleted
    }

    public class Employee
    {
        public int ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Position { get; set; }
        public int RoomID { get; set; }
        public bool IsNew { get; set; }
        public Rooms AssignedRoom { get; internal set; }

        //public Employees AssignedRoom { get; internal set; }

        //public Room AssignedRoom { get; internal set; }
        internal RowState State { get; set; }

    }

    //public class RoomViewModel
    //{
    //    public int RoomID { get; set; }
    //    public string Description { get; set; }

    //    public override string ToString()
    //    {
    //        return Description;
    //    }
    //}


    public partial class EmployeesPage : Page
    {
        Entities entities = new Entities();

        //public ObservableCollection<string> Names { get; set; } = new ObservableCollection<string>()
        //{
        //    "sadasd",
        //    "adasdasd",
        //    "adasdasd"
        //};

        public EmployeesPage()
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
                txtBoxRoomID.Text = selectedRoom.RoomID.ToString();
            }
        }


        // Метод для загрузки комнат из базы данных
        private void LoadRoomsFromDatabase()
        {
            // Здесь вам нужно использовать ваш код для получения данных из базы данных
            // Пример: roomsList = GetRoomsFromDatabase();
            // Предполагается, что у вас есть метод GetRoomsFromDatabase, который возвращает List<Rooms>
            // Затем преобразуйте его в ObservableCollection<RoomViewModel>
            // Например:
            List<Rooms> roomsFromDatabase = GetRoomsFromDatabase();
            RoomsList.Clear();
            foreach (var room in roomsFromDatabase)
            {
                RoomsList.Add(new RoomViewModel { RoomID = room.ID, Description = room.Description });
            }
        }

        // Класс RoomViewModel для отображения данных в ComboBox
        public class RoomViewModel
        {
            public int RoomID { get; set; }
            public string Description { get; set; }

            public override string ToString()
            {
                return Description;
            }
        }





        private void CreateColumns()
        {
            DataGridTextColumn idColumn = new DataGridTextColumn
            {
                Header = "ID",
                Binding = new Binding("ID")
            };

            DataGridTextColumn firstNameColumn = new DataGridTextColumn
            {
                Header = "Имя",
                Binding = new Binding("FirstName")
            };

            DataGridTextColumn lastNameColumn = new DataGridTextColumn
            {
                Header = "Фамилия",
                Binding = new Binding("LastName")
            };

            DataGridTextColumn positionColumn = new DataGridTextColumn
            {
                Header = "Должность",
                Binding = new Binding("Position")
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
            dataGridView1.Columns.Add(firstNameColumn);
            dataGridView1.Columns.Add(lastNameColumn);
            dataGridView1.Columns.Add(positionColumn);
            dataGridView1.Columns.Add(roomIDColumn);
            dataGridView1.Columns.Add(roomColumn);
            dataGridView1.Columns.Add(newColumn);
        }

        private void ReadSingleRow(DataGrid dataGrid, IDataRecord record)
        {
            int roomID = record.GetInt32(4);
            var selRoom = entities.Rooms.FirstOrDefault(r => r.ID == roomID);

            dataGrid.Items.Add(new Employee
            {
                ID = record.GetInt32(0),
                FirstName = record.GetString(1),
                LastName = record.GetString(2),
                Position = record.GetString(3),
                RoomID = roomID,
                AssignedRoom = selRoom, // Обновляем AssignedRoom
                IsNew = true // Помечаем как новую запись
            });
        }


        private void RefreshDataGrid(DataGrid dataGrid)
        {
            try
            {
                dataGrid.Items.Clear();

                string queryString = "select * from Employees";

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
                entities.Database.Connection.Close();
            }
        }


        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            CreateColumns();
            RefreshDataGrid(dataGridView1);

            LoadRoomsFromDatabase();
            //comboBoxRooms.DisplayMemberPath = "Description";
            comboBoxRooms.ItemsSource = RoomsList;
        }

        private void dataGridView1_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            // добавлена сортировка и выбор
            if (dataGridView1.SelectedItem != null)
            {
                Employee selectedEmployee = (Employee)dataGridView1.SelectedItem;
                if (selectedEmployee != null)
                {
                    string ID = selectedEmployee.ID.ToString();
                    txtID.Text = "Выбранная запись о сотруднике № " + ID;
                    txtBoxFirstName.Text = selectedEmployee.FirstName;
                    txtBoxLastName.Text = selectedEmployee.LastName;
                    txtBoxPosition.Text = selectedEmployee.Position;
                    txtBoxRoomID.Text = selectedEmployee.RoomID.ToString();
                    //txtBoxRoomID.Text = selectedEmployee.AssignedRoom.Description.ToString();
                    //comboBoxRooms.IsEnabled = true;
                    //comboBoxRooms.SelectedItem = selectedEmployee.AssignedRoom.Description.ToString();
                }
                else
                {
                    txtID.Text = "Выбранная запись о сотруднике № ....";
                    txtBoxFirstName.Text = "Не выбрано";
                    txtBoxLastName.Text = "Не выбрано";
                    txtBoxPosition.Text = "Не выбрано";
                    txtBoxRoomID.Text = "Не выбрано";
                    //comboBoxRooms.IsEnabled = false;
                    //comboBoxRooms.SelectedItem = null;
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

            string searchString = $"SELECT * FROM Employees WHERE CONCAT(ID, FirstName, LastName, Position, RoomID) LIKE N'%{txtBoxSearch.Text}%'";

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

        private void DeleteEmployee_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = dataGridView1.SelectedItem;
            if (selectedItem != null)
            {
                Employee selectedEmployee = (Employee)selectedItem;
                int employeeId = selectedEmployee.ID;

                // Удаление из базы данных по ID
                var employeeToDelete = entities.Employees.Find(employeeId);
                if (employeeToDelete != null)
                {
                    entities.Employees.Remove(employeeToDelete);
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

        private void AddEmployee_Click(object sender, RoutedEventArgs e)
        {
            // Код для открытия формы добавления сотрудника
            AddEmployees_Form createEmployeeWindow = new AddEmployees_Form();
            createEmployeeWindow.ShowDialog();
        }

        private void EditEmployee_Click(object sender, RoutedEventArgs e)
        {
            Employee selectedEmployee = (Employee)dataGridView1.SelectedItem;

            if (selectedEmployee != null)
            {
                infoEdit.Visibility = Visibility.Visible;
                btnEdit.Visibility = Visibility.Hidden;
                btnSave.Visibility = Visibility.Visible;
                comboBoxRooms.IsEnabled = true;
                txtBoxFirstName.IsReadOnly = false;
                txtBoxLastName.IsReadOnly = false;
                txtBoxPosition.IsReadOnly = false;
            }
            else
            {
                MessageBox.Show("Сотрудник не выбран.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void UpdateEmployeeInDatabase(Employee employeeToUpdate)
        {
            string updateQuery = $"UPDATE Employees SET FirstName = N'{employeeToUpdate.FirstName}', LastName = N'{employeeToUpdate.LastName}', " +
                                 $"Position = N'{employeeToUpdate.Position}', RoomID = {employeeToUpdate.RoomID} WHERE ID = {employeeToUpdate.ID}";

            using (SqlCommand command = new SqlCommand(updateQuery, (SqlConnection)entities.Database.Connection))
            {
                entities.Database.Connection.Open();
                command.ExecuteNonQuery();
                entities.Database.Connection.Close();
            }
        }

        private void UpdateEmployeeFromFields(Employee employee)
        {
            employee.FirstName = txtBoxFirstName.Text;
            employee.LastName = txtBoxLastName.Text;
            employee.Position = txtBoxPosition.Text;


            if (int.TryParse(txtBoxRoomID.Text, out int roomID))
            {
                employee.RoomID = roomID;
            }
            else
            {
                // Обработка ошибки, если введенное значение для RoomID не является числом
                MessageBox.Show("Неверный формат ID кабинета.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return; // Прерываем метод, так как введенные данные некорректны
            }

            // Помечаем запись как измененную
            employee.State = RowState.Modified;
        }

        private bool ValidateEquipment(Employee employee)
        {
            // Добавьте здесь логику проверки
            // Например, проверка на пустые поля, корректность даты и т.д.

            // Пример проверки на пустые поля
            if (string.IsNullOrEmpty(employee.FirstName) || string.IsNullOrEmpty(employee.LastName) || string.IsNullOrEmpty(employee.Position) || string.IsNullOrEmpty(txtBoxRoomID.Text))
            {
                MessageBox.Show("Заполните все обязательные поля правильно.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            // Добавьте другие проверки, которые вам необходимы

            return true; // Если все проверки прошли успешно
        }

        private void SaveEmployee_Click(object sender, RoutedEventArgs e)
        {
            Employee selectedEmployee = (Employee)dataGridView1.SelectedItem;

            if (selectedEmployee != null)
            {
                if (ValidateEquipment(selectedEmployee)) { 
                    // Обновляем данные в объекте сотрудника на основе полей формы
                    UpdateEmployeeFromFields(selectedEmployee);

                // Обновляем данные в базе данных
                UpdateEmployeeInDatabase(selectedEmployee);

                // Отключаем режим редактирования
                infoEdit.Visibility = Visibility.Hidden;

                // Включаем кнопку "Редактировать", отключаем кнопку "Сохранить"
                btnEdit.Visibility = Visibility.Visible;
                btnSave.Visibility = Visibility.Hidden;

                    comboBoxRooms.IsEnabled = false;
                    comboBoxRooms.SelectedItem = null;
                }

                // Обновляем DataGrid
                dataGridView1.Items.Refresh();

                // Очищаем поля формы
                ClearFields();
            }
            else
            {
                MessageBox.Show("Сотрудник не выбран.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void ClearFields()
        {
            txtBoxFirstName.Text = string.Empty;
            txtBoxLastName.Text = string.Empty;
            txtBoxPosition.Text = string.Empty;
            txtBoxRoomID.Text = string.Empty;
        }

        private void BackToPage_MouseLeftButtonUp(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void clearImg_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ClearFields();
        }
    }

}
