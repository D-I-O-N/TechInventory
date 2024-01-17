using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Data;
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
using TechInventory._src.pages.equipment;
using Equipment = TechInventory._src.database.Equipment;
using System.Globalization;

namespace TechInventory._src.pages.equipmentHistory
{
    /// <summary>
    /// Логика взаимодействия для EquipmentHistoryPage.xaml
    /// </summary>



    public class RoomViewModelEquipment
    {
        public int EquipmentID { get; set; }
        public string EquipmentName { get; set; }

        public override string ToString()
        {
            return EquipmentName;
        }
    }

    public class RoomViewModelEmployee
    {
        public int EmployeeID { get; set; }
        public string LastName { get; set; }

        public override string ToString()
        {
            return LastName;
        }
    }

    public partial class EquipmentHistoryPage : Page
    {
        Entities entities = new Entities();



        public EquipmentHistoryPage()
        {
            InitializeComponent();

        }

        public ObservableCollection<RoomViewModelEmployee> EmployeeList { get; set; } = new ObservableCollection<RoomViewModelEmployee>();
        public ObservableCollection<RoomViewModelEquipment> EquipmentList { get; set; } = new ObservableCollection<RoomViewModelEquipment>();

        private List<Employees> GetRoomsFromDatabaseEmployees()
        {
            using (Entities entities = new Entities())
            {
                return entities.Employees.ToList();
            }
        }

        private List<Equipment> GetRoomsFromDatabaseEquipment()
        {
            using (Entities entities = new Entities())
            {
                return entities.Equipment.ToList();
            }

        }

        private void ComboBoxEmployees_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
                if (comboBoxEmployee.SelectedItem is RoomViewModelEmployee selectedEmployee)
                {
                    txtBoxEquipmentHistoryEmployee.Text = selectedEmployee.EmployeeID.ToString();
                }
        }

        private void ComboBoxEquipment_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBoxEquipmentName.SelectedItem is RoomViewModelEquipment selectedEquipment)
            {
                txtBoxEquipmentHistoryName.Text = selectedEquipment.EquipmentID.ToString();
            }
        }


        private void LoadRoomsFromDatabaseEmployee()
        {
                List<Employees> roomsFromDatabase = GetRoomsFromDatabaseEmployees();
                EmployeeList.Clear();
                foreach (var employee in roomsFromDatabase)
                {
                    EmployeeList.Add(new RoomViewModelEmployee { EmployeeID = employee.ID, LastName = employee.LastName });
                }
        }

        private void LoadRoomsFromDatabaseEquipment()
        {
            List<Equipment> roomsFromDatabase = GetRoomsFromDatabaseEquipment();
            EquipmentList.Clear();
            foreach (var equipment in roomsFromDatabase)
            {
                EquipmentList.Add(new RoomViewModelEquipment { EquipmentID = equipment.ID, EquipmentName = equipment.EquipmentName });
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
                Header = "ID Оборудования",
                Binding = new Binding("EquipmentID")
            };

            DataGridTextColumn assignedEquipmentColumn = new DataGridTextColumn
            {
                Header = "Оборудование",
                Binding = new Binding("AssignedEquipment.EquipmentName") // Предположим, что у кабинета есть свойство EquipmentInfo
            };

            DataGridTextColumn equipmentCheckoutDate = new DataGridTextColumn
            {
                Header = "Дата выдачи",
                Binding = new Binding("CheckoutDate")
            };

            DataGridTextColumn employeeIDColumn = new DataGridTextColumn
            {
                Header = "ID Сотрудника",
                Binding = new Binding("EmployeeID")
            };

            DataGridTextColumn assignedEmployeeColumn = new DataGridTextColumn
            {
                Header = "Сотрудник",
                Binding = new Binding("AssignedEmployee.LastName") // Предположим, что есть свойство EmployeeInfo
            };

            DataGridTextColumn returnDateColumn = new DataGridTextColumn
            {
                Header = "Дата выдачи",
                Binding = new Binding("ReturnDate")
            };


            DataGridTextColumn newColumn = new DataGridTextColumn
            {
                Header = "IsNew",
                Binding = new Binding("IsNew")
            };

            dataGridView1.Columns.Add(idColumn);

            dataGridView1.Columns.Add(equipmentNameColumn);
            dataGridView1.Columns.Add(assignedEquipmentColumn);

            dataGridView1.Columns.Add(equipmentCheckoutDate);

            dataGridView1.Columns.Add(employeeIDColumn);
            dataGridView1.Columns.Add(assignedEmployeeColumn);

            dataGridView1.Columns.Add(returnDateColumn);

            dataGridView1.Columns.Add(newColumn);
        }

        private void ReadSingleRow(DataGrid dataGrid, IDataRecord record)
        {
            int employeeID = record.GetInt32(3); // Измените на соответствующий индекс столбца для EmployeeID
            var selEmployee = entities.Employees.FirstOrDefault(emp => emp.ID == employeeID);

            int equipmentID = record.GetInt32(1); // Измените на соответствующий индекс столбца для EquipmentID
            var selEquipment = entities.Equipment.FirstOrDefault(eq => eq.ID == equipmentID);

            dataGrid.Items.Add(new EquipmentHistory
            {
                ID = record.GetInt32(0),
                EquipmentID = equipmentID,
                AssignedEquipment = selEquipment,
                CheckoutDate = record.GetDateTime(2),
                EmployeeID = employeeID,
                AssignedEmployee = selEmployee,
                ReturnDate = record.GetDateTime(4),
                IsNew = true
            });
        }

        private void RefreshDataGrid(DataGrid dataGrid)
        {
            try
            {
                dataGrid.Items.Clear();

                string queryString = "select * from EquipmentHistory";

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

            LoadRoomsFromDatabaseEmployee();
            comboBoxEmployee.ItemsSource = EmployeeList;

            LoadRoomsFromDatabaseEquipment();
            comboBoxEquipmentName.ItemsSource = EquipmentList;
        }

        private void txtBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            Search(dataGridView1);
        }

        private void Search(DataGrid dataGrid)
        {
            dataGrid.Items.Clear();

            string searchString = $"SELECT * FROM EquipmentHistory WHERE CONCAT(EquipmentID, CheckoutDate, EmployeeID, ReturnDate) LIKE N'%{txtBoxSearch.Text}%'";

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


        private void dataGridView1_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            EquipmentHistory selectedEquipmentHistory = (EquipmentHistory)dataGridView1.SelectedItem;
            if (selectedEquipmentHistory != null)
            {
                string ID = selectedEquipmentHistory.ID.ToString();
                txtID.Text = "Выбранная запись № " + ID;
                txtBoxEquipmentHistoryName.Text = selectedEquipmentHistory.AssignedEquipment.EquipmentName.ToString();
                txtBoxEquipmentHistoryEmployee.Text = selectedEquipmentHistory.AssignedEmployee.LastName.ToString();
                txtBoxEquipmentHistoryCheckoutDate.Text = selectedEquipmentHistory.CheckoutDate.ToString();
                txtBoxEquipmentHistoryReturnDate.Text = selectedEquipmentHistory.ReturnDate.ToString();
            } 
            else 
            {
                txtBoxEquipmentHistoryName.Text = "Не выбрано";
                txtBoxEquipmentHistoryEmployee.Text = "Не выбрано";
                txtBoxEquipmentHistoryCheckoutDate.Text = "Не выбрано";
                txtBoxEquipmentHistoryReturnDate.Text = "Не выбрано";
                comboBoxEquipmentName.IsEnabled = false;
                comboBoxEmployee.IsEnabled = false;
                comboBoxEmployee.SelectedItem = null;
                comboBoxEquipmentName.SelectedItem = null;
            }
        }

        private void UpdateEquipmentHistoryInDatabase(EquipmentHistory equipmentHistoryToUpdate)
        {
            string updateQuery = $"UPDATE EquipmentHistory SET " +
                                 $"EquipmentID = {equipmentHistoryToUpdate.EquipmentID}, " +
                                 $"CheckoutDate = '{equipmentHistoryToUpdate.CheckoutDate:s}', " +
                                 $"EmployeeID = {equipmentHistoryToUpdate.EmployeeID}, " +
                                 $"ReturnDate = '{equipmentHistoryToUpdate.ReturnDate:s}' " +
                                 $"WHERE ID = {equipmentHistoryToUpdate.ID}";

            using (SqlCommand command = new SqlCommand(updateQuery, (SqlConnection)entities.Database.Connection))
            {
                entities.Database.Connection.Open();
                command.ExecuteNonQuery();
                entities.Database.Connection.Close();
            }
        }

        private void UpdateEquipmentHistoryFromFields(EquipmentHistory equipmentHistory)
        {
            //string checkoutDateInput = txtBoxEquipmentHistoryCheckoutDate.Text;
            //string returnDateInput = txtBoxEquipmentHistoryReturnDate.Text;

            if (int.TryParse(txtBoxEquipmentHistoryName.Text, out int equipmentID))
            {
                equipmentHistory.EquipmentID = equipmentID;
            }
            else
            {
                MessageBox.Show("Неверный формат ID оборудования.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            //if (DateTime.TryParseExact(checkoutDateInput, "s", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime checkoutDate))
            //{
            //    equipmentHistory.CheckoutDate = checkoutDate;
            //}
            //else
            //{
            //    MessageBox.Show("Неверный формат даты выдачи. Используйте формат 'год-месяц-день час:минута:секунда'", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            //    return;
            //}

            if (int.TryParse(txtBoxEquipmentHistoryEmployee.Text, out int employeeID))
            {
                equipmentHistory.EmployeeID = employeeID;
            }
            else
            {
                MessageBox.Show("Неверный формат ID сотрудника.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            //if (DateTime.TryParseExact(returnDateInput, "s", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime returnDate))
            //{
            //    equipmentHistory.ReturnDate = returnDate;
            //}
            //else
            //{
            //    MessageBox.Show("Неверный формат даты возврата. Используйте формат 'год-месяц-день час:минута:секунда'", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            //    return;
            //}

            //equipmentHistory.IsNew = true; // или другая логика присвоения значения
            equipmentHistory.State = employees.RowState.Modified;
            
        }


        private bool ValidateEquipmentHistory(EquipmentHistory equipmentHistory)
        {

            // Пример проверки на пустые поля
            if (equipmentHistory.AssignedEquipment == null || equipmentHistory.AssignedEmployee == null || equipmentHistory.CheckoutDate == null || equipmentHistory.ReturnDate == null)
            {
                MessageBox.Show("Заполните все обязательные поля правильно.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            // Добавьте другие проверки, которые вам необходимы

            return true; // Если все проверки прошли успешно
        }

        private void SaveEquipmentHistory_Click(object sender, RoutedEventArgs e)
        {
            EquipmentHistory selectedEquipmentHistory = (EquipmentHistory)dataGridView1.SelectedItem;

            if (selectedEquipmentHistory != null)
            {
                try
                {
                    if (ValidateEquipmentHistory(selectedEquipmentHistory))
                    {
                        UpdateEquipmentHistoryFromFields(selectedEquipmentHistory);
                        UpdateEquipmentHistoryInDatabase(selectedEquipmentHistory);
                        

                        infoEdit.Visibility = Visibility.Hidden;
                        btnEdit.Visibility = Visibility.Visible;
                        btnSave.Visibility = Visibility.Hidden;

                        dataGridView1.Items.Refresh();
                        ClearFields();

                        comboBoxEquipmentName.IsEnabled = false;
                        comboBoxEmployee.IsEnabled = false;
                        comboBoxEquipmentName.SelectedItem = null;
                        comboBoxEmployee.SelectedItem = null;
                    }
                }
                catch (Exception ex)
                {
                    // Обработка ошибки
                    MessageBox.Show($"Произошла ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("История оборудования не выбрана.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }



        private void AddEquipmentHistory_Click(object sender, RoutedEventArgs e)
        {
            AddEquipmentHistory_Form createEquipmentHistoryWindow = new AddEquipmentHistory_Form();
            createEquipmentHistoryWindow.ShowDialog();
        }

        private void EditEquipmentHistory_Click(object sender, RoutedEventArgs e)
        {
            EquipmentHistory selectedEquipmentHistory = (EquipmentHistory)dataGridView1.SelectedItem;

            if (selectedEquipmentHistory != null)
            {
                
                infoEdit.Visibility = Visibility.Visible;
                btnEdit.Visibility = Visibility.Hidden;
                btnSave.Visibility = Visibility.Visible;
                comboBoxEquipmentName.IsEnabled = true;
                comboBoxEmployee.IsEnabled = true;

                // Установка значений в текстовые поля
                //txtBoxEquipmentHistoryCheckoutDate.Text = selectedEquipmentHistory.CheckoutDate.ToString("s");
                //txtBoxEquipmentHistoryReturnDate.Text = selectedEquipmentHistory.ReturnDate.ToString("s");
            }
            else
            {
                MessageBox.Show("Сотрудник не выбран.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void DeleteEquipmentHistory_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = dataGridView1.SelectedItem;
            if (selectedItem != null)
            {
                EquipmentHistory selectedEquipmentHistory = (EquipmentHistory)selectedItem;
                int equipmentHistoryId = selectedEquipmentHistory.ID;

                // Удаление из базы данных по ID
                var equipmentHistoryToDelete = entities.EquipmentHistory.Find(equipmentHistoryId);
                if (equipmentHistoryToDelete != null)
                {
                    entities.EquipmentHistory.Remove(equipmentHistoryToDelete);
                    entities.SaveChanges();

                    // Обновление DataGrid
                    RefreshDataGrid(dataGridView1);
                    ClearFields();
                }
            }

        }

        private void refreshImg_MouseDown(object sender, MouseButtonEventArgs e)
        {
            RefreshDataGrid(dataGridView1);
        }

        private void ClearFields()
        {
            txtBoxEquipmentHistoryName.Text = string.Empty;
            txtBoxEquipmentHistoryEmployee.Text = string.Empty;
            txtBoxEquipmentHistoryCheckoutDate.Text = string.Empty;
            txtBoxEquipmentHistoryReturnDate.Text = string.Empty;
        }

        private void clearImg_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ClearFields();
        }

        private void BackToPage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            NavigationService.GoBack();
        }

    } 
}
    

