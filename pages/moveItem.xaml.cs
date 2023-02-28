using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
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
    /// Interaction logic for moveItem.xaml
    /// </summary>
    public partial class moveItem : Page
    {
        private static System.Timers.Timer atimer;
        private static List<item> items;
        public moveItem()
        {
            InitializeComponent();
            items = new List<item>();
            setLIstView();
            SetCboBox();

        }

        private void CurrentItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (currentItems.SelectedIndex != -1)
            {
                item itm = (item)currentItems.SelectedItem;
                locationName.SelectedValue = itm.locationId;
            }
        }

        private void refresh_CLick(object sender, EventArgs e)
        {

        }

        private void updateLocation_Click(object sender, EventArgs e)
        {
            if(currentItems.SelectedIndex == -1) { return; }
            if(locationName.SelectedIndex == -1 || locationName.SelectedValue == null) { return; }
            
            item itm = (item)currentItems.SelectedItem;
            dbConnection con = new dbConnection();
            if (con.changeItemLocation(itm.id.ToString(), locationName.SelectedValue.ToString()))
            {
                setLIstView();
            }
            else
            {
                MessageBox.Show( "Error moving " + itm.tagNo + " from " + itm.locationName + " to " + locationName.DisplayMemberPath);
            }
        }

        

        private void updateItemsListView()
        {

            currentItems.ItemsSource = null;
            currentItems.ItemsSource = items;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            setLIstView();
        }

        private async void setLIstView()
        {
            phpApiDbConnection con= new phpApiDbConnection();
            items = await con.GetItemsToBeRemovedAsync();
            if(items == null) { return; }
            for (int i = 0; i < items.Count; i++)
            {
                items[i].currentActivity = items[i].getCurrentActivity();
                items[i].updateCloseToDateBool();
            }
            sortItems();
            currentItems.ItemsSource = null;
            currentItems.ItemsSource = items;
            locationName.SelectedIndex = -1;
        }
        private async void SetCboBox()
        {
            phpApiDbConnection con = new phpApiDbConnection();
            //uses connecion to get locaitons form combo box 
            locationName.ItemsSource = await con.GetLocationListAsync();
            locationName.DisplayMemberPath = "Name";
            locationName.SelectedValuePath = "ID";
        }

        private void sortItems()
        {
            items.Sort(delegate (item x, item y)
            {
                if (x.tagNo == null && y.tagNo == null)
                {
                    return 0;
                }
                else if (x.tagNo == null)
                {
                    return -1;
                }
                else if (y.tagNo == null)
                {
                    return 1;
                }
                else
                {
                    return x.tagNo.CompareTo(y.tagNo);
                }

            });
        }

        private void locationName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }
    }
}
