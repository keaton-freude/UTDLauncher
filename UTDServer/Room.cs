using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UTDServer
{
    public class Room
    {
        /* A room contains a list of players split into 2 teams */
        public List<Account> OpenList;
        public List<Account> Slots; /* One -> Six */

        public String RoomName;

        public Room()
        {
        }

        public Room(string name)
        {
            this.RoomName = name;
        }
    }
}
