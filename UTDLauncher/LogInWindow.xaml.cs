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
using System.Windows.Shapes;
using System.Security.Cryptography;
using System.Security;
using System.Web;

namespace UTDLauncher
{
    /// <summary>
    /// Interaction logic for LogInWindow.xaml
    /// </summary>
    public partial class LogInWindow : Window
    {
        public LogInWindow()
        {
            InitializeComponent();

            Network.Instance.window = this;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        public void CheckAuthentication()
        {
            if (Network.Instance.Authenticated)
            {
                MessageBox.Show("Login Successful!", "Success", MessageBoxButton.OK, MessageBoxImage.None);
            }
            else
            {
                MessageBox.Show("Login Failed! The Username/Password combination does not match any known records.", "Failure", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (txtUsername.Text != "" && txtConfirm.Text != "" && txtPassword.Text != "")
            {
                if (txtPassword.Text == txtConfirm.Text)
                {
                    /* good to go, tell DB to build */
                    string salt = CreateSalt(12);
                    string hash = CreatePasswordHash(txtPassword.Text, salt);

                    //MessageBox.Show("Salt: " + salt + "\nHash: " + hash);

                    Network.Instance.CreateUser(txtUsername.Text, hash, salt);
                    
                }
                else
                {
                    MessageBox.Show("Password and Repeat Password textboxes must match!", "ERROR!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
            }
            else
            {
                MessageBox.Show("Please fill out each field.", "ERROR!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        private static string CreateSalt(int size)
        {
            //Generate a cryptographic random number.
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] buff = new byte[size];
            rng.GetBytes(buff);

            // Return a Base64 string representation of the random number.
            return Convert.ToBase64String(buff);
        }

        private static string CreatePasswordHash(string pwd, string salt)
        {
            HashAlgorithm algo = new SHA1Managed();

            string saltAndPwd = String.Concat(pwd, salt);

            string hashedPwd = Convert.ToBase64String(algo.ComputeHash(System.Text.Encoding.ASCII.GetBytes(saltAndPwd)));

            return hashedPwd;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            string username = txtLogUsername.Text;
            string password = txtLogPW.Text;

            if (username != "" && password != "")
            {
                /* Send off request to server */
                Network.Instance.LoginUser(username, password);
            }
            else
            {
                MessageBox.Show("Please fill out both the Username and Password textboxes", "ERROR!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
