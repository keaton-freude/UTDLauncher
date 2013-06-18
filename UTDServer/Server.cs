using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace UTDServer
{
    public class Server
    {
        private TcpListener tcpListener;
        private Thread listenThread;
        private List<TcpClient> clients;

        public List<Room> Rooms;

        public Server()
        {
            this.tcpListener = new TcpListener(IPAddress.Any, 3000);
            this.listenThread = new Thread(new ThreadStart(ListenForClients));
            this.listenThread.Start();
            clients = new List<TcpClient>();
            Console.WriteLine("Server Started");
            Rooms = new List<Room>();
        }

        private void ListenForClients()
        {
            this.tcpListener.Start();

            while (true)
            {
                TcpClient client = this.tcpListener.AcceptTcpClient();
                clients.Add(client);
                Thread clientThread = new Thread(new ParameterizedThreadStart(HandleClientComm));
                clientThread.Start(client);
            }
        }

        private void HandleClientComm(object client)
        {
            TcpClient tcpClient = (TcpClient)client;
            NetworkStream clientStream = tcpClient.GetStream();

            byte[] message = new byte[4096];
            int bytesRead;

            while (true)
            {
                bytesRead = 0;

                try
                {
                    bytesRead = clientStream.Read(message, 0, 4096);


                }
                catch
                {
                    break;
                }

                if (bytesRead == 0)
                {
                    break;
                }

                ASCIIEncoding encoder = new ASCIIEncoding();

                string packet = encoder.GetString(message, 0, bytesRead);
                string[] commandParams;
                /* if we have something */

                if (packet.Length > 0)
                {
                    /* Peel the first command off */
                    string command = packet.Split(';')[0];

                    switch (command.Split(',')[0])
                    {
                        case "CreateUser":
                            /* Username = 1, UserHash = 2, UserSalt = 3 */
                            commandParams = command.Split(',');
                            if (Database.Instance.CreateUser(commandParams[1], commandParams[3], commandParams[2]) == 1)
                                Console.WriteLine("Database is creating a new user with Username: " + commandParams[1]);
                            else
                                Console.WriteLine("User Create Failed for Message: " + command);
                            break;
                        case "LoginUser":
                            /* Username = 1, Userpassword = 2 */
                            commandParams = command.Split(',');
                            if (Database.Instance.UserLogin(commandParams[1], commandParams[2]))
                            {
                                Console.WriteLine("Login successful!");
                                byte[] ToSend = new byte[64];
                                ToSend = encoder.GetBytes("LogonAccepted;");
                                clientStream.Write(ToSend, 0, 14);
                            }
                            else
                            {
                                Console.WriteLine("Login Failed!");
                                byte[] ToSend = new byte[64];
                                ToSend = encoder.GetBytes("LogonRejected;");
                                clientStream.Write(ToSend, 0, 14);
                            }

                            break;
                        case "CreateRoom":
                            commandParams = command.Split(',');
                            if (CreateRoom(commandParams[1]))
                            {
                                /* Send off confirmation to server to add room */
                                /* Every client needs to know that a new room is available */
                                Console.WriteLine("Room create successful! Room Name: " + commandParams[1]);

                                byte[] toSend = new Byte[64];
                                toSend = encoder.GetBytes("RoomCreated," + commandParams[1] + ";");
                                clientStream.Write(toSend, 0, 13 + commandParams[1].Length);
                            }
                            break;
                        default:
                            Console.WriteLine("Packet unhandled. command=" + command);
                            break;
                    }
                }
            }

            tcpClient.Close();
        }

        public bool CreateRoom(string roomName)
        {
            if (Rooms.Count != 0)
            {
                foreach (Room rm in Rooms)
                {
                    if (rm.RoomName == roomName)
                        return false;
                }
            }
            Rooms.Add(new Room(roomName));

            return true;
        }
    }
}
