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
                entities.Database.Connection.Open();

                // Считывание значений из элементов управления
                // Создание нового объекта EquipmentHistory и заполнение его данными

                // Пример:
                DateTime checkoutDate;
                if (!DateTime.TryParseExact(txtBoxEquipmentHistoryCheckoutDate.Text, "s", CultureInfo.InvariantCulture, DateTimeStyles.None, out checkoutDate))
                {
                    MessageBox.Show("Неверный формат даты выдачи. Используйте формат 'год-месяц-день час:минута:секунда'", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                //DateTime returnDate;
                //if (DateTime.TryParseExact(txtBoxEquipmentHistoryReturnDate.Text, "s", CultureInfo.InvariantCulture, DateTimeStyles.None, out returnDate))
                //{
                //    MessageBox.Show("Неверный формат даты выдачи. Используйте формат 'год-месяц-день час:минута:секунда'", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                //    return;
                //}

                var newEquipmentHistory = new EquipmentHistory
                {
                    EquipmentID = int.Parse(txtBoxEquipmentHistoryName.Text),
                    CheckoutDate = checkoutDate,
                    EmployeeID = int.Parse(txtBoxEquipmentHistoryEmployee.Text),
                    ReturnDate = DateTime.ParseExact(txtBoxEquipmentHistoryReturnDate.Text, "s", CultureInfo.InvariantCulture)
                     // или другая логика
                };

                // Добавление новой записи в базу данных
                entities.EquipmentHistory.Add(newEquipmentHistory);
                entities.SaveChanges();

                MessageBox.Show("Запись успешно добавлена.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                // Закрываем окно добавления после успешного добавления записи
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


        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Запрещаем ввод недопустимых символов
            if (!char.IsDigit(e.Text[0]) && e.Text[0] != ':' && e.Text[0] != 'T' && e.Text[0] != '-' && e.Text[0] != ' ')
            {
                e.Handled = true;
            }
        }

        private void TextBox_TextChangedCheckoutDate(object sender, TextChangedEventArgs e)
        {
            // Форматируем ввод в соответствии с маской
            // Определите максимальную длину маски
            int maxMaskLength = 19;

            // Удалите все недопустимые символы
            txtBoxEquipmentHistoryCheckoutDate.Text = new string(txtBoxEquipmentHistoryCheckoutDate.Text
                .Where(c => char.IsDigit(c) || c == ':' || c == 'T' || c == '-')
                .ToArray());

            // Ограничьте длину строки максимальной длиной маски
            if (txtBoxEquipmentHistoryCheckoutDate.Text.Length > maxMaskLength)
            {
                txtBoxEquipmentHistoryCheckoutDate.Text = txtBoxEquipmentHistoryCheckoutDate.Text.Substring(0, maxMaskLength);
            }

            // Форматируйте ввод в соответствии с маской
            if (txtBoxEquipmentHistoryCheckoutDate.Text.Length >= 5 && txtBoxEquipmentHistoryCheckoutDate.Text[4] != '-')
            {
                txtBoxEquipmentHistoryCheckoutDate.Text = txtBoxEquipmentHistoryCheckoutDate.Text.Insert(4, "-");
            }
            if (txtBoxEquipmentHistoryCheckoutDate.Text.Length >= 8 && txtBoxEquipmentHistoryCheckoutDate.Text[7] != '-')
            {
                txtBoxEquipmentHistoryCheckoutDate.Text = txtBoxEquipmentHistoryCheckoutDate.Text.Insert(7, "-");
            }
            if (txtBoxEquipmentHistoryCheckoutDate.Text.Length >= 11 && txtBoxEquipmentHistoryCheckoutDate.Text[10] != 'T')
            {
                txtBoxEquipmentHistoryCheckoutDate.Text = txtBoxEquipmentHistoryCheckoutDate.Text.Insert(10, "T");
            }
            if (txtBoxEquipmentHistoryCheckoutDate.Text.Length >= 14 && txtBoxEquipmentHistoryCheckoutDate.Text[13] != ':')
            {
                txtBoxEquipmentHistoryCheckoutDate.Text = txtBoxEquipmentHistoryCheckoutDate.Text.Insert(13, ":");
            }
            if (txtBoxEquipmentHistoryCheckoutDate.Text.Length >= 17 && txtBoxEquipmentHistoryCheckoutDate.Text[16] != ':')
            {
                txtBoxEquipmentHistoryCheckoutDate.Text = txtBoxEquipmentHistoryCheckoutDate.Text.Insert(16, ":");
            }

            // Установите каретку в конец текста
            txtBoxEquipmentHistoryCheckoutDate.CaretIndex = txtBoxEquipmentHistoryCheckoutDate.Text.Length;
        }

        private void TextBox_TextChangedReturnDate(object sender, TextChangedEventArgs e)
        {
            // Форматируем ввод в соответствии с маской
            // Определите максимальную длину маски
            int maxMaskLength = 19;

            // Удалите все недопустимые символы
            txtBoxEquipmentHistoryReturnDate.Text = new string(txtBoxEquipmentHistoryReturnDate.Text
                .Where(c => char.IsDigit(c) || c == ':' || c == 'T' || c == '-')
                .ToArray());

            // Ограничьте длину строки максимальной длиной маски
            if (txtBoxEquipmentHistoryReturnDate.Text.Length > maxMaskLength)
            {
                txtBoxEquipmentHistoryReturnDate.Text = txtBoxEquipmentHistoryReturnDate.Text.Substring(0, maxMaskLength);
            }

            // Форматируйте ввод в соответствии с маской
            if (txtBoxEquipmentHistoryReturnDate.Text.Length >= 5 && txtBoxEquipmentHistoryReturnDate.Text[4] != '-')
            {
                txtBoxEquipmentHistoryReturnDate.Text = txtBoxEquipmentHistoryReturnDate.Text.Insert(4, "-");
            }
            if (txtBoxEquipmentHistoryReturnDate.Text.Length >= 8 && txtBoxEquipmentHistoryReturnDate.Text[7] != '-')
            {
                txtBoxEquipmentHistoryReturnDate.Text = txtBoxEquipmentHistoryReturnDate.Text.Insert(7, "-");
            }
            if (txtBoxEquipmentHistoryReturnDate.Text.Length >= 11 && txtBoxEquipmentHistoryReturnDate.Text[10] != 'T')
            {
                txtBoxEquipmentHistoryReturnDate.Text = txtBoxEquipmentHistoryReturnDate.Text.Insert(10, "T");
            }
            if (txtBoxEquipmentHistoryReturnDate.Text.Length >= 14 && txtBoxEquipmentHistoryReturnDate.Text[13] != ':')
            {
                txtBoxEquipmentHistoryReturnDate.Text = txtBoxEquipmentHistoryReturnDate.Text.Insert(13, ":");
            }
            if (txtBoxEquipmentHistoryReturnDate.Text.Length >= 17 && txtBoxEquipmentHistoryReturnDate.Text[16] != ':')
            {
                txtBoxEquipmentHistoryReturnDate.Text = txtBoxEquipmentHistoryReturnDate.Text.Insert(16, ":");
            }

            // Установите каретку в конец текста
            txtBoxEquipmentHistoryReturnDate.CaretIndex = txtBoxEquipmentHistoryReturnDate.Text.Length;
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
