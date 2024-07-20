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
using TechInventory._src.pages.statistics;

namespace TechInventory._src.pages
{
    /// <summary>
    /// Логика взаимодействия для AuthorizationPage.xaml
    /// </summary>
    public partial class AuthorizationPage : Page
    {
        Entities entities = new Entities();
        public AuthorizationPage()
        {
            InitializeComponent();
        }

        private void btnEnter_Click(object sender, RoutedEventArgs e)
        {
            // Получаем значения из текстовых полей
            string username = userName.Text;
            string password = userPassword.Password;

            // Проверяем имя пользователя и пароль в базе данных
            var user = entities.Users.FirstOrDefault(u => u.Name == username && u.Password == password);

            if (user != null)
            {
                // Определяем роль пользователя
                switch (user.Urole)
                {
                    case "Admin":
                        // Перенаправляем на страницу администратора
                        NavigationService.Navigate(new MainPage());
                        break;

                    case "User":
                        // Перенаправляем на страницу пользователя
                        NavigationService.Navigate(new PageStatisticReport());
                        break;

                    default:
                        MessageBox.Show("Неизвестная роль пользователя.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        break;
                }
            }
            else
            {
                MessageBox.Show("Неверное имя пользователя или пароль.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
