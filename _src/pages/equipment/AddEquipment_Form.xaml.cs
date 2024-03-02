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

namespace TechInventory._src.pages.equipment
{
    /// <summary>
    /// Логика взаимодействия для AddEquipment_Form.xaml
    /// </summary>
    /// 

    public partial class AddEquipment_Form : Window
    {
        Entities entities = new Entities();

        public AddEquipment_Form()
        {
            InitializeComponent();
        }

        public ObservableCollection<RoomViewModel> RoomsList { get; set; } = new ObservableCollection<RoomViewModel>();

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

        private void AddEquipmentFormButton_Click(object sender, RoutedEventArgs e)
        {
            entities.Database.Connection.Open();

            string equipmentName = txtEquipmentName.Text;
            string equipmentType = txtEquipmentType.Text;
            int equipmentCount;


            //if (string.IsNullOrEmpty(txtEquipmentName.Text) || string.IsNullOrEmpty(txtEquipmentType.Text) || string.IsNullOrEmpty(txtSN.Text) || string.IsNullOrEmpty(txtStatus.Text) || string.IsNullOrEmpty(txtEquipmentCount.Text) || string.IsNullOrEmpty(txtPurchaseDate.Text) )
            //{

            

            if (int.TryParse(txtEquipmentCount.Text, out equipmentCount))
            {
                string serialNumber = txtSN.Text;
                    DateTime purchaseDate = DateTime.Now;
                    //DateTime purchaseDate = DateTime.Now; // Установим текущую дату, вы можете изменить это в соответствии с вашими требованиями
                string status = txtStatus.Text;
                int roomID;

                if (int.TryParse(txtRoomID.Text, out roomID))
                {
                    var addQuery = $"INSERT INTO Equipment (EquipmentName, EquipmentType, Count, SerialNumber, PurchaseDate, Status, RoomID) " +
                                   $"VALUES (N'{equipmentName}', N'{equipmentType}', {equipmentCount}, N'{serialNumber}', " +
                                   $"'{purchaseDate:yyyy-MM-dd H:mm:ss}', N'{status}', {roomID})";

                    SqlCommand command = new SqlCommand(addQuery, (SqlConnection)entities.Database.Connection);
                    command.ExecuteReader();

                    MessageBox.Show("Оборудование успешно добавлено!", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("ID кабинета должен иметь числовой формат!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                MessageBox.Show("Количество должно иметь числовой формат!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            //} else
            //{
            //    MessageBox.Show("Заполните все обязательные поля правильно.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
               
            //}

            entities.Database.Connection.Close();
        }

        private void ClearFields()
        {
            txtEquipmentName.Text = string.Empty;
            txtEquipmentType.Text = string.Empty;
            txtEquipmentCount.Text = string.Empty;
            txtSN.Text = string.Empty;
            txtPurchaseDate.Text = string.Empty;
            txtStatus.Text = string.Empty;
            txtRoomID.Text = string.Empty;
        }

        private void AddEquipmentFormButton_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            ClearFields();
        }

        private void txtPurchaseDate_TextChanged(object sender, TextChangedEventArgs e)
        {
            inputMaskForPurchaseDateHardware(sender);
        }

        private void inputMaskForPurchaseDateHardware(object sender)
        {
            //Пример: 20.01.2024
            TextBox textBox = sender as TextBox;
            int maxMaskLength = 10;

            // Удаляем все недопустимые символы
            textBox.Text = new string(textBox.Text
                .Where(c => char.IsDigit(c) || c == '.')
                .ToArray());

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
            // Каретку в конец текста
            textBox.CaretIndex = textBox.Text.Length;
        }
    }
}

