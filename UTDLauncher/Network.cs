using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace UTDLauncher
{
    public class Network
    {
        private static Network _network;
        public LogInWindow window;
        public MainWindow mainWindow;
        private TcpClient client;
        IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 3000);
        NetworkStream clientStream;
        ASCIIEncoding encoder = new ASCIIEncoding();
        private Thread thread;
        public bool Authenticated = false;
        private string CommandList = "";

        private byte[] ReceivedBytes = new byte[512];

        public static Network Instance
        {
            get
            {
                if (_network == null)
                    _network = new Network();
                return _network;
            }
        }

        private Network()
        {
            client = new TcpClient();

            client.Connect(serverEndPoint);

            clientStream = client.GetStream();

            clientStream.BeginRead(ReceivedBytes, 0, 512, receiveCallback, null);
        }

        public void receiveCallback(IAsyncResult result)
        {
            /* Get the result, save out to string, then append to our list of commands */
            string data = encoder.GetString(ReceivedBytes);



            

            //data.Trim(

            CommandList += data.TrimEnd(new char[] { '\0' });

            /* Now we need to decide what we need to do */

            string command = CommandList.Split(';')[0];

            /* Now we need to remove that item from the string */
            string[] RemainingItems = CommandList.Split(';');
            CommandList = "";
            for (int i = 1; i < RemainingItems.Length; ++i)
            {
                CommandList += RemainingItems[i];
            }


            string[] commandParams = command.Split(',');

            switch (commandParams[0])
            {
                case "LogonAccepted":
                    Authenticated = true;
                    Console.WriteLine("Accepted");
                    /* need to report out to window */
                    window.Dispatcher.Invoke(window.CheckAuthentication);
                    
                    //window.CheckAuthentication();
                    break;
                case "LogonRejected":
                    Authenticated = false;
                    Console.WriteLine("Rejected");
                    /* Need to report out to window */
                    window.Dispatcher.Invoke(window.CheckAuthentication);
                    break;
                case "RoomCreated":
                    //System.Action action = new System.Action(mainWindow.CreateRoom);

                    //object[] param = new object[] { new Room(commandParams[1]) };
                    //mainWindow.Dispatcher.Invoke(mainWindow.CreateRoom, param);
                    mainWindow.Dispatcher.Invoke(new Action(() => mainWindow.CreateRoom(new Room(commandParams[1]))));
                    
                    
                    break;
            }

            clientStream.BeginRead(ReceivedBytes, 0, 512, receiveCallback, null);
            
        }

        public void CreateRoom(string roomName)
        {
            string message = "CreateRoom," + roomName + ";";
            SendMessage(message);
        }

        public void CreateUser(string Username, string UserHash, string UserSalt)
        {
            string message = "CreateUser," + Username + "," + UserHash + "," + UserSalt + ";";
            //Encodin
            SendMessage(message);
        }

        private void SendMessage(string message)
        {
            byte[] buffer = encoder.GetBytes(message);

            clientStream.Write(buffer, 0, buffer.Length);
            clientStream.Flush();
        }

        public void LoginUser(string Username, string Password)
        {
            string message = "LoginUser," + Username + "," + Password + ";";

            SendMessage(message);
        }
    }
}
