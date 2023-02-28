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
using WasteApp.APICLasses;
using WasteApp.classes;

namespace WasteApp.pages
{
    /// <summary>
    /// Interaction logic for editLocations.xaml
    /// </summary>
    public partial class editLocations : Page
    {

        locationListBox loc;
        
        public editLocations()
        {
            InitializeComponent();
            loc = new locationListBox(currentLocations);
            Task.Run(loc.updateListBox);
        }

        private void CurrentItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            loc.select(currentLocations.SelectedIndex);
            locationName.Text = loc.getName(currentLocations.SelectedIndex);
        }

        public async void updateLocation_Click(object sender, RoutedEventArgs e)
        {
            if(locationName.Text==null || locationName.Text=="") { return; }
            if(loc.getSelected()== false)
            {
                MessageBox.Show("Please select a location to update");
                return;
            }
            if(loc.GetSelectedLocationObj() is null) { return; }

            phpApiDbConnection con = new phpApiDbConnection();
            Location inputLocaiton = new Location();

            inputLocaiton.Name= locationName.Text;
            inputLocaiton.InUse= true;
            inputLocaiton.ID = loc.GetSelectedLocationObj().ID;
                if(await con.UpdateLocationAsync(inputLocaiton))
                {
                    await loc.updateListBox();
                    locationName.Text = "";
                }
                else
                {
                    MessageBox.Show("Error updating location");
                }

        }
        public async void addLocation_Click(object sender, RoutedEventArgs e)
        {
            if (locationName.Text == null || locationName.Text == "") { return; }

                phpApiDbConnection con = new phpApiDbConnection();
            Location inputLocaiton = new Location();
            inputLocaiton.Name= locationName.Text;
            inputLocaiton.InUse= true;
            if ( await con.AddLocationAsync(inputLocaiton))
            {
                await loc.updateListBox();
                locationName.Text = "";
            }
            else
            {
                MessageBox.Show("error adding location");
            }
        }

        public async void removeLocation_CLick(object sender, RoutedEventArgs e)
        {
            if (loc.getSelected())
            {

                    phpApiDbConnection con = new phpApiDbConnection();
                    Location inputLocaiton = loc.GetSelectedLocationObj();
                    if (await con.RemoveLocationFromUse(inputLocaiton))
                    {
                        await loc.updateListBox();
                    }
                    else
                    {
                        MessageBox.Show("Error in removing locaiton");
                    }
                
            }
        }

 
    }
}
