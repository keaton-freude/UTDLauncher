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

namespace UTDLauncher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public List<Room> Rooms;

        public void CreateRoom(Room room)
        {
            lstRooms.Items.Add(room);
        }


        public MainWindow()
        {
            InitializeComponent();

            Network.Instance.mainWindow = this;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            /* Pop the log in window and disable */

            LogInWindow LogWindow = new LogInWindow();

            LogWindow.mainWindow = this;

            LogWindow.Show();

            this.IsEnabled = false;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            string RoomName = txtCreateRoomName.Text;

            Network.Instance.CreateRoom(RoomName);
        }
    }
}
