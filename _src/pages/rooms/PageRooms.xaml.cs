using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
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

namespace TechInventory._src.pages.rooms
{
    /// <summary>
    /// Логика взаимодействия для PageRooms.xaml
    /// </summary>

    //[Table("Rooms")]
    //public class Room
    //{
    //    public int RoomNumber { get; set; }
    //    public string Description { get; set; }
    //    // Добавьте другие свойства по мере необходимости
    //}

    public partial class PageRooms : Page
    {
        private Entities entities = new Entities();

        //public ObservableCollection<Room> Rooms { get; set; } = new ObservableCollection<Room>();
        //public int SelectedRoomIndex { get; set; }

        //public PageRooms()
        //{
        //    InitializeComponent();
        //    LoadRooms(); // Загрузка кабинетов из базы данных
        //}

        //private void LoadRooms()
        //{
        //    Rooms.Clear();
        //    foreach (var room in entities.Rooms)
        //    {
        //        Rooms.Add(new Room
        //        {
        //            RoomNumber = (int)room.RoomNumber,
        //            Description = room.Description
        //            // Добавьте другие свойства по мере необходимости
        //        });
        //    }
        //    RoomsListView.ItemsSource = Rooms;
        //}

        private void AddCabinet_Click(object sender, RoutedEventArgs e)
        {
 
        }

        private void EditCabinet_Click(object sender, RoutedEventArgs e)
        {
            //if (SelectedRoomIndex >= 0 && SelectedRoomIndex < Rooms.Count)
            //{
            //    // Логика редактирования кабинета
            //    Room selectedRoom = Rooms[SelectedRoomIndex];
            //    // Откройте окно редактирования, передавая selectedRoom
            //}
            //else
            //{
            //    MessageBox.Show("Выберите кабинет для редактирования.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
            //}
        }

        private void DeleteCabinet_Click(object sender, RoutedEventArgs e)
        {

        }
    }

}
