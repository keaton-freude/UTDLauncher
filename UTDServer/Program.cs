using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;

namespace UTDServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Server server = new Server();

            bool value = Database.Instance.UserLogin("matemeo", "doom132");

            Console.WriteLine("Result: " + value);
        }
    }
}
