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
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WasteApp.APICLasses;
using WasteApp.classes;

namespace WasteApp.pages
{
    /// <summary>
    /// Interaction logic for disposeItem.xaml
    /// </summary>
    public partial class disposeItem : Page
    {

        List<item> itm;
        public disposeItem()
        {
            itm = new List<item>();
            InitializeComponent();
            RefreshPage();
        }

        public async void dispose_Click(object sender, RoutedEventArgs e)
        {
            if (SanityCheckForm())
            {
                item itm = currentItems.SelectedItem as item;
                phpApiDbConnection con= new phpApiDbConnection();
                WasteItem wstItm = new WasteItem();
                wstItm.LoadPropertiesFromAltItem(itm);
                wstItm.ID = itm.id;
                wstItm.DisposalRouteID = (int)disposalRoute.SelectedValue;
                wstItm.UserIDDisposing = (long)userCbo.SelectedValue;
                wstItm.UserIDLoggingDispose = App.usr.id;
                wstItm.dateDisposed = DisposeDateTimePicker.GetDateTime();


                if(await con.DisposeItemAsync(wstItm))
                {
                    await RefreshPage();
                }
                else
                {
                    MessageBox.Show("Error in disposing of item");
                }
            }
            else
            {
                MessageBox.Show("Please check the form");
            }
        }


        public void refresh_CLick(object sender, EventArgs e)
        {
            
        }
        public void Item_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        public void CurrentItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        private async Task RefreshPage()
        {
            UpdateCboRouteBox();
            UpdateUserCbo();
            await UpdateListView();
            RefreshListView();
        }
        private async Task UpdateCboRouteBox()
        {
            phpApiDbConnection phpcon = new phpApiDbConnection();
            List<DisposalRoute> result = await phpcon.GetActiveDisposalRoutesAsync();
            disposalRoute.ItemsSource = result;
            disposalRoute.DisplayMemberPath = "name";
            disposalRoute.SelectedValuePath = "ID";
            if (disposalRoute.Items.Count > 0) {
                disposalRoute.SelectedIndex= 0;
             }
        }
        private async Task UpdateListView()
        {
            phpApiDbConnection con = new phpApiDbConnection();
            itm= await con.GetItemsToBeRemovedAsync();
            RefreshListView();
        }
        private void RefreshListView()
        {
            currentItems.ItemsSource = null;
            currentItems.ItemsSource = itm;
        }
        private async Task UpdateUserCbo()
        {
            phpApiDbConnection con = new phpApiDbConnection();
            List<user> result = await con.GetUsersAsync();
            List<user> users = result.Where(ur =>ur.inUse== true).ToList();
            userCbo.ItemsSource= users;
            userCbo.DisplayMemberPath = "name";
            userCbo.SelectedValuePath= "id";

            for (int i = 0; i < userCbo.Items.Count; i++)
            {
                user testUser = (user)userCbo.Items[i];
                if (testUser.id == App.usr.id)
                {
                    userCbo.SelectedIndex= i;
                    break;
                }
            }
        }

        private void UserCBo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }
        private bool SanityCheckForm()
        {
            //output on checks
            bool checks = true;
            //generic iteration through controls
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(MainGrid); i++)
            {
                if (VisualTreeHelper.GetChild(MainGrid, i).GetType().Name == "TextBox")
                {
                    TextBox textboxcheck = (TextBox)VisualTreeHelper.GetChild(MainGrid, i);
                    if ((textboxcheck.Text == "") || textboxcheck.Text == null)
                    {
                        checks = false;
                    }
                }
                if (VisualTreeHelper.GetChild(MainGrid, i).GetType().Name == "ComboBox")
                {
                    ComboBox comboCheck = (ComboBox)VisualTreeHelper.GetChild(MainGrid, i);
                    if (comboCheck.SelectedIndex == -1)
                    {
                        checks = false;
                    }

                }

            }
            //form specific check
            if(currentItems.SelectedIndex == -1) { checks = false; }    
            return checks; 
        }
    }
}
