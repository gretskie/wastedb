using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WasteApp.classes;

namespace WasteApp
{
    /// <summary>
    /// Interaction logic for login.xaml
    /// </summary>
    public partial class login : Window
    {
        MainWindow main;
        private bool isLoggedIn = false;

        public login(MainWindow main)
        {
            InitializeComponent();
            this.main = main;
        }

        //sanity checks and starts the async user login (in user class) and starts thread (cannot run it in this thread as it will halt the program and the response will not be processed) to wait for a user to be logged in
        private void LoginClick(object sender, RoutedEventArgs e)
        {
            if(txUserName == null || txPassword == null || txUserName.Text == "" || txPassword.Password == "") { return; }
            App.usr = new user(txUserName.Text, txPassword.Password);
            //Thread.Sleep(1000);
            Thread tr = new Thread(WaitForUserToLoad);
            tr.Start();
        }

        //holds the program for about 20 secs waiting for the DB to respond. If a user is loaded continue to mainwindow otherwise go back to try again.
        private void WaitForUserToLoad()
        {
            Application.Current.Dispatcher.Invoke(HideForm);
            try
            {
                for (int i = 0; i < 20; i++)
                {


                    if (App.usr.loaded == true)
                    {
                        if (App.usr.id == -1)
                        {
                            MessageBox.Show("Error in logon please try again");
                            Application.Current.Dispatcher.Invoke(ResetForm);
                        }
                        else
                        {
                            isLoggedIn = true;
                            var dis = Application.Current.Dispatcher;
                            dis.Invoke(this.Close);
                        }
                        return;
                    }
                    else
                    {
                        Thread.Sleep(1000);
                    }
                }
                MessageBox.Show("taking a long time to for response from DB please check connections");
                return;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
                this.Close();
            }
        }

        private void ResetForm()
        {
            txUserName.Text = string.Empty;
            txPassword.Password= string.Empty;
            txUserName.IsEnabled = true;
            txPassword.IsEnabled= true; 
            bnExit.IsEnabled = true;
            bnLogin.IsEnabled = true;
        }

        private void HideForm()
        {
            txPassword.IsEnabled= false;
            txUserName.IsEnabled= false;
            bnLogin.IsEnabled= false;
            bnExit.IsEnabled= false;
        }

        private void ExitClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if(!isLoggedIn)
            {
                main.Close();
            }
            else
            {
                string titleString = "Waste App  - " + App.usr.name;
                if(App.usr.userLevel == 1)
                {
                    titleString += "  - Admin";
                }

                main.Title = titleString;
                main.Show();
            }
            
        }

        private void WindowKeyUP(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                
                LoginClick(this, e);
            }
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            txUserName.Focus();
        }
    }
}
