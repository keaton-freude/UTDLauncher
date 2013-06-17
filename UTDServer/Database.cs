using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Security;
using System.Data.SqlClient;
using System.Security;
using System.Web;
using System.Security.Cryptography;

namespace UTDServer
{
    public class Database
    {
        private static Database _instance;
        SqlConnection myConnection;

        public static Database Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Database();
                return _instance;
            }
        }

        private Database()
        {
            myConnection = new SqlConnection("Server=MATILDA\\MYRTLESERVER;Database=UTD_DB;Trusted_Connection=True;");

            try
            {
                myConnection.Open();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public int CreateUser(string Username, string UserSalt, string UserHash)
        {
            /* Creates a user in the database */
            SqlCommand command = new SqlCommand("INSERT INTO utd_Accounts(AccountName, AccountHash, AccountSalt) " +
                                                "VALUES('" + Username + "', '"+UserHash+"', '"+UserSalt+"');");

            Console.WriteLine("Executing: " + command.CommandText);

            command.Connection = myConnection;

            return command.ExecuteNonQuery();
        }

        public bool UserLogin(string Username, string Password)
        {
            string salt = ""; // get salt
            string CorrectHashedPwd = ""; // get the correct hash
            /* Lets see if Username exists, if so, lets get the CorrectHash and the salt */
            SqlCommand command = new SqlCommand();
            command.Connection = myConnection;
            command.CommandText = "select AccountHash, AccountSalt from " +
                                  "utd_Accounts where AccountName = '"+Username+"';";

            Console.WriteLine("Requesting Login Verification for: " + Username + "\nSQL Command: " + command.CommandText);

            SqlDataReader reader = command.ExecuteReader();

            int count = 0;

            while (reader.Read())
            {
                salt = reader["AccountSalt"].ToString();
                CorrectHashedPwd = reader["AccountHash"].ToString();

                count++;
            }

            if (count != 1)
            {
                /* Either the account does not exist, or a duplicate is in the data-base
                 * lets find out which and report an error to the console
                 */
                if (count == 0)
                {
                    Console.WriteLine("Username: " + Username + " does not exist in the database. No records returned.");
                }
                else
                    Console.WriteLine("Username: " + Username + " returned too many records 2+, suggesting duplicate in database");
            }



            HashAlgorithm algo = new SHA1Managed();

            string saltAndPwd = String.Concat(Password, salt);

            string hashedPwd = Convert.ToBase64String(algo.ComputeHash(System.Text.Encoding.ASCII.GetBytes(saltAndPwd)));

            reader.Close();

            return (hashedPwd == CorrectHashedPwd);
        }


    }
}
