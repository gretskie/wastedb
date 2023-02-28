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
using WasteApp.classes;

namespace WasteApp.pages
{
    /// <summary>
    /// Interaction logic for EditUsers.xaml
    /// </summary>
    public partial class EditUsers : Page
    {
        private List<user> usr;
        public EditUsers()
        {
            InitializeComponent();
            usr = new List<user>();
            UpdateUsrList();
            UpdateUserLevel();
        }

        private async Task UpdateUsrList()
        {
            if (usr != null) { usr.Clear(); }
            phpApiDbConnection con = new phpApiDbConnection();
            List<user> testList = await con.GetUsersAsync();
            usr = testList.OrderBy(us =>us.name).ToList();
            lvAllUsers.ItemsSource = usr;
        }
        private void UpdateUserLevel()
        {
            Dictionary<string,long> level = new Dictionary<string,long>();
            level.Add(key: "User", value: 0);
            level.Add(key: "Admin", value: 1);
            cboUserLevel.ItemsSource = level;
            cboUserLevel.SelectedValuePath = "Value";
            cboUserLevel.DisplayMemberPath = "Key";
            cboUserLevel.SelectedIndex = 0;
        }
        private void ClearForm()
        {
            txEmail.Text = string.Empty;
            txUserName.Text = string.Empty;
            txPassword.Password = string.Empty;
            chkIsActive.IsChecked= false;
            chkIsInUse.IsChecked= false;
            cboUserLevel.SelectedIndex = 0;
        }
        private void ResetForm()
        {
            ClearForm();
            UpdateUsrList();
            txUserName.Focus();
        }
        private void SetFormInfo(user us)
        {
            //if things are not right ditchout
            if (us == null) { return; }
            if(lvAllUsers.SelectedIndex == -1) { return; }

            //pick up the user info from incoming user an put into page
            txUserName.Text = us.name;
            txEmail.Text = us.email;
            chkIsActive.IsChecked = us.active;
            chkIsInUse.IsChecked = us.inUse;
            cboUserLevel.SelectedValue = us.userLevel;
        }
        private bool SanitiseForm(bool isPasswordRequired, bool isPasswordOnly = false)
        {
            bool checks = true;
            // have to individully check boxes on this form as we need to check different inputs dep[ending on the button
            if(isPasswordOnly)
            {
                if(txPassword.Password == "" || txPassword.Password is null) { checks = false; }
            }else 
            {
                if (isPasswordRequired) 
                {
                    if (txPassword.Password == "" || txPassword.Password is null) { checks = false; }
                }
                if(txEmail.Text=="" || txEmail.Text is null || txUserName.Text =="" || txUserName is null) { checks = false; }
                if(cboUserLevel.SelectedIndex == -1) { checks = false; }
            }

            return checks;
        }
        private void lvAllUsers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            user us = (user)lvAllUsers.SelectedItem;
            if (us == null) { return; }
            SetFormInfo(us);
        }
        private bool CheckUserName()
        {
            List<user> testList = usr.Where(us=>us.name.ToString() == txUserName.Text).ToList();
            if(testList.Count > 0) { return false; } else { return true; }
        }
        private async void SetNewPassword_Click(object sender, RoutedEventArgs e)
        {
            if(lvAllUsers.SelectedIndex == -1) { return; }
            if (this.SanitiseForm(false, true))
            {
                string newPass = txPassword.Password;
                user us = (user)lvAllUsers.SelectedItem;
                phpApiDbConnection con = new phpApiDbConnection();
                if(await con.SetNewPasswordAsync(us, newPass))
                {
                    ResetForm();
                }
                else
                {
                    MessageBox.Show("Error updating password");
                }
            }
        }
        private async void AddUser_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckUserName())
            {
                MessageBox.Show("Username is already taken");
                return;
            }
            if (this.SanitiseForm(true)){
                user us = new user();
                us.name = txUserName.Text;
                us.email = txEmail.Text;
                us.active = chkIsActive.IsChecked == true?true:false;
                us.inUse = chkIsInUse.IsChecked == true?true:false;
                us.userLevel = (long)cboUserLevel.SelectedValue;
                phpApiDbConnection con = new phpApiDbConnection();
                if (await con.AddNewUserAsync(us,txPassword.Password))
                {
                    ResetForm();
                }
                else
                {
                    MessageBox.Show("Error adding new user to DB");
                }

            }
            else
            {
                MessageBox.Show("Error in form please check");
            }
        }
        private async void UpdateUser_Click(object sender, RoutedEventArgs e)
        {
            if(lvAllUsers.SelectedIndex == -1) { return; }
            if (this.SanitiseForm(false))
            {
                user us = (user)lvAllUsers.SelectedItem;
                us.name = txUserName.Text;
                us.email = txEmail.Text;
                us.active = chkIsActive.IsChecked == true ? true : false;
                us.inUse = chkIsInUse.IsChecked == true ? true : false;
                us.userLevel = (long)cboUserLevel.SelectedValue;
                phpApiDbConnection con = new phpApiDbConnection();
                if(await con.UpdateUserInfoAsync(us))
                {
                    ResetForm();
                }
                else
                {
                    MessageBox.Show("Error updating user info in DB");
                }
            }
            else
            {
                MessageBox.Show("Please fill in form correctly");
            }
        }
        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            ResetForm();
        }


    }
}
