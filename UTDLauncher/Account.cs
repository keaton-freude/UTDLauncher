using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UTDLauncher
{
    public class Account
    {
        public int AccountId;
        public string AccountName;
        public int Level;

        public Account()
        {
        }

        public Account(int acctID, string acctName, int lvl)
        {
            this.AccountId = acctID;
            this.AccountName = acctName;
            this.Level = lvl;
        }

        public override string ToString()
        {
            return AccountName + " | " + Level.ToString();
        }
    }
}
