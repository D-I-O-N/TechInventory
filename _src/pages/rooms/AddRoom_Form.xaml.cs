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
using System.Windows.Shapes;
using TechInventory._src.database;
using System.Data.SqlClient;

namespace TechInventory._src.pages.rooms
{
    /// <summary>
    /// Логика взаимодействия для AddRoom_Form.xaml
    /// </summary>
    public partial class AddRoom_Form : Window
    {
        Entities entities = new Entities();

        public AddRoom_Form()
        {
            InitializeComponent();
        }

        private void AddRoomFormButton_Click(object sender, RoutedEventArgs e)
        {
            entities.Database.Connection.Open();

            int roomNumber;
            var description = txtDescription.Text;

            if (int.TryParse(txtRoomNumber.Text, out roomNumber))
            {
                var addQuery = $"insert into Rooms (RoomNumber, Description) values ('{roomNumber}', N'{description}')";

                SqlCommand command = new SqlCommand(addQuery, (SqlConnection)entities.Database.Connection);
                command.ExecuteReader();

                MessageBox.Show("Запись успешно добавлена!!", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Номер кабинета должен иметь числовой формат!!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            entities.Database.Connection.Close();
        }

        private void ClearFields()
        {
            txtRoomNumber.Text = string.Empty;
            txtDescription.Text = string.Empty;
        }

        private void AddRoomFormButton_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            ClearFields();
        }

        private void txtRoomNumber_TextChanged(object sender, TextChangedEventArgs e)
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
    }
}
