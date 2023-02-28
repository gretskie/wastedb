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
    /// Interaction logic for editCat.xaml
    /// </summary>
    /// 

    

    public partial class editCat : Page
    {

        wasteCatListBox wst;
        public editCat()
        {
            InitializeComponent();
             wst = new wasteCatListBox(currentWasteCats);
            //wst.updateListBox();
            
        }

        private void CurrentItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(currentWasteCats.SelectedIndex == -1) { return; }
            wst.select(currentWasteCats.SelectedIndex);
            wasteCatName.Text = wst.getName(currentWasteCats.SelectedIndex);
            wasteCatLimitDays.Text =    wst.getLimitDays(currentWasteCats.SelectedIndex).ToString();
            wasteCatLimitQuant.Text = wst.getLinitQuant(currentWasteCats.SelectedIndex).ToString();
            wasteCatAlertDays.Text = wst.GetAlertLimit().ToString();
        }
        
        public async void updateWasteCat_Click(object sender, RoutedEventArgs e)
        {
            //sanity checking of input
            if (wasteCatName.Text == null || wasteCatName.Text == "" || wasteCatLimitDays.Text == null || wasteCatLimitDays.Text == "" || wasteCatLimitQuant.Text==null || wasteCatLimitQuant.Text == "") { return; }
            double testDouble;
            if (!double.TryParse(wasteCatLimitQuant.Text, out testDouble))
            {
                MessageBox.Show("A valid activity limnit has not been entered");
                return;
            }
            int testInt;
            if (!int.TryParse(wasteCatLimitDays.Text, out testInt))
            {
                MessageBox.Show("Not a valid number of days");
                return;
            }


            if(!int.TryParse(wasteCatAlertDays.Text, out testInt)){
                MessageBox.Show("Not a valid number of days");
                return;
            }
            if (wst.getSelected() == false)
            {
                MessageBox.Show("Please select a location to update");
                return;
            }


            //proccessing the imput to db
 
            phpApiDbConnection con = new phpApiDbConnection();
            WasteGroup inputGroup = new WasteGroup();
            inputGroup.decayAlertDays = int.Parse(wasteCatAlertDays.Text);
            inputGroup.inUse = true;
            inputGroup.ID = wst.id();
            inputGroup.activityLimit = double.Parse(wasteCatLimitQuant.Text);
            inputGroup.maxAccumulationDays = int.Parse(wasteCatLimitDays.Text);
            inputGroup.Name = wasteCatName.Text;
            bool testBool = await con.UpdateWasteGroupAsync(inputGroup);
            if (!testBool)
            {
                MessageBox.Show("An error occured updating the Catagory");
            }
             wst.updateListBox();
             wasteCatName.Text = "";
             wasteCatLimitDays.Text = "";
             wasteCatLimitQuant.Text = "";

        }
        public async void addWasteCat_Click(object sender, RoutedEventArgs e)
        {
            // sanity checks
            if (wasteCatName.Text == null || wasteCatName.Text == "" || wasteCatLimitDays.Text == null || wasteCatLimitDays.Text == "" || wasteCatLimitQuant.Text == null || wasteCatLimitQuant.Text == "") { return; }
            double testDouble;
            if (!double.TryParse(wasteCatLimitQuant.Text, out testDouble))
            {
                MessageBox.Show("A valid activity limnit has not been entered");
                return;
            }
            int testInt;
            if (!int.TryParse(wasteCatLimitDays.Text, out testInt))
            {
                MessageBox.Show("Not a valid number of days");
                return;
            }

            //process data to db
                phpApiDbConnection con = new phpApiDbConnection();
                WasteGroup inputWasteGroup= new WasteGroup();
                inputWasteGroup.Name= wasteCatName.Text;
            inputWasteGroup.maxAccumulationDays = int.Parse(wasteCatLimitDays.Text);
            inputWasteGroup.inUse = true;
            inputWasteGroup.decayAlertDays = int.Parse(wasteCatAlertDays.Text);
            inputWasteGroup.activityLimit = double.Parse(wasteCatLimitQuant.Text); 
            if(!await con.AddWasteGroupAsync(inputWasteGroup))
            {
                MessageBox.Show("error in adding new catagory");
            }
            else
            {
                wst.updateListBox();
                wasteCatName.Text = "";
                wasteCatLimitDays.Text = "";
                wasteCatLimitQuant.Text = "";
                wasteCatAlertDays.Text = "";
            }
        }

        public async void removeWasteCat_CLick(object sender, RoutedEventArgs e)
        {
            if (wst.getSelected() & MessageBox.Show("You are going to remove the Waste Catagory "+wst.getName(currentWasteCats.SelectedIndex), "?", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {


                    phpApiDbConnection con = new phpApiDbConnection();
                if (await con.removeWasteGroup(wst.id()))
                {
                    wst.updateListBox();
                    wasteCatName.Text = "";
                    wasteCatLimitDays.Text = "";
                    wasteCatLimitQuant.Text = "";
                }
                else
                {
                    MessageBox.Show("error removing Waste Catagory");
                }

            }
        }
    }
}
