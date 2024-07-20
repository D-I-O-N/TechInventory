using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Shapes;
using TechInventory._src.database;

namespace TechInventory._src.pages.equipmentHistory
{
    /// <summary>
    /// Логика взаимодействия для AddEquipmentHistory_Form.xaml
    /// </summary>
    /// 
    public partial class AddEquipmentHistory_Form : Window
    {
        Entities entities = new Entities();

        public AddEquipmentHistory_Form()
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

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadRoomsFromDatabaseEmployee();
            comboBoxEmployee.ItemsSource = EmployeeList;

            LoadRoomsFromDatabaseEquipment();
            comboBoxEquipmentName.ItemsSource = EquipmentList;
        }

        private void AddEmployeeFormButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string inputFormat = "dd.MM.yyyy H:mm:ss";

                DateTime checkoutDate;
                if (!DateTime.TryParseExact(txtBoxEquipmentHistoryCheckoutDate.Text, inputFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out checkoutDate))
                {
                    MessageBox.Show("Неверный формат даты выдачи. Используйте формат 'день.месяц.год час:минута:секунда' Пример: 20.01.2024 18:00:00", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                DateTime returnDate;
                if (!DateTime.TryParseExact(txtBoxEquipmentHistoryReturnDate.Text, inputFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out returnDate))
                {
                    MessageBox.Show("Неверный формат даты возврата. Используйте формат 'день.месяц.год час:минута:секунда' Пример: 20.01.2024 18:00:00", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                int equipmentId = int.Parse(txtBoxEquipmentHistoryName.Text);
                int employeeId = int.Parse(txtBoxEquipmentHistoryEmployee.Text);

                string insertQuery = "INSERT INTO EquipmentHistory (EquipmentID, CheckoutDate, EmployeeID, ReturnDate) " +
                                     "VALUES (@EquipmentID, @CheckoutDate, @EmployeeID, @ReturnDate)";

                using (Entities entities = new Entities())
                {
                    entities.Database.Connection.Open();
                    var command = entities.Database.Connection.CreateCommand();
                    command.CommandText = insertQuery;
                    command.Parameters.Add(new System.Data.SqlClient.SqlParameter("@EquipmentID", equipmentId));
                    command.Parameters.Add(new System.Data.SqlClient.SqlParameter("@CheckoutDate", checkoutDate));
                    command.Parameters.Add(new System.Data.SqlClient.SqlParameter("@EmployeeID", employeeId));
                    command.Parameters.Add(new System.Data.SqlClient.SqlParameter("@ReturnDate", returnDate));

                    command.ExecuteNonQuery();
                }

                MessageBox.Show("Запись успешно добавлена.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка при добавлении записи: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                entities.Database.Connection.Close();
            }
        }

        private void TextBox_TextChangedCheckoutDate(object sender, TextChangedEventArgs e)
        {
            DateTimeFormatter.FormatDateTimeTextBox(txtBoxEquipmentHistoryCheckoutDate);
            inputMaskForHardwareDateHistory(sender);
        }

        private void TextBox_TextChangedReturnDate(object sender, TextChangedEventArgs e)
        {
            DateTimeFormatter.FormatDateTimeTextBox(txtBoxEquipmentHistoryReturnDate);
            inputMaskForHardwareDateHistory(sender);
        }

        private void inputMaskForHardwareDateHistory(object sender)
        {
             //Пример: 20.01.2024 18:00:00
            TextBox textBox = sender as TextBox;
            int maxMaskLength = 19;

            textBox.Text = new string(textBox.Text
                .Where(c => char.IsDigit(c) || c == ':' || c == ' ' || c == '.')
                .ToArray());

            // длина строки с максимальной длиной маски
            if (textBox.Text.Length > maxMaskLength)
            {
                textBox.Text = textBox.Text.Substring(0, maxMaskLength);
            }

            if (textBox.Text.Length >= 3 && textBox.Text[2] != '.')
            {
                textBox.Text = textBox.Text.Insert(2, ".");
            }
            if (textBox.Text.Length >= 6 && textBox.Text[5] != '.')
            {
                textBox.Text = textBox.Text.Insert(5, ".");
            }
            if (textBox.Text.Length >= 11 && textBox.Text[10] != ' ')
            {
                textBox.Text = textBox.Text.Insert(10, " ");
            }
            if (textBox.Text.Length >= 14 && textBox.Text[13] != ':')
            {
                textBox.Text = textBox.Text.Insert(13, ":");
            }
            if (textBox.Text.Length >= 17 && textBox.Text[16] != ':')
            {
                textBox.Text = textBox.Text.Insert(16, ":");
            }

            // Каретку в конец текста
            textBox.CaretIndex = textBox.Text.Length;
        }

        private void ClearFields()
        {
            txtBoxEquipmentHistoryName.Text = string.Empty;
            txtBoxEquipmentHistoryEmployee.Text = string.Empty;
            txtBoxEquipmentHistoryCheckoutDate.Text = string.Empty;
            txtBoxEquipmentHistoryReturnDate.Text = string.Empty;
        }
        private void AddEmployeeFormButton_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            ClearFields();
        }

    }
}
