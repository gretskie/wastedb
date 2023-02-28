using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using WasteApp.APICLasses;
using WasteApp.classes;

namespace WasteApp.pages
{
    /// <summary>
    /// Interaction logic for addItem.xaml
    /// </summary>
    public partial class addItem : Page
    {
        public addItem()
        {
            InitializeComponent();

            initialisePage();

        }

        private async void addItem_Click(object sender, RoutedEventArgs e)
        {
            if(this.sanityCheckForm() == true)
            {
                DateTime refDate = DateTimePicker.GetDateTime();
                //MessageBox.Show(unit.SelectedValue.ToString());
                phpApiDbConnection con = new phpApiDbConnection();
                WasteItem itm= new WasteItem();
                user usr = App.usr;
                itm.TagNo = tagNo.Text;
                itm.ActivityOnAddition = double.Parse(activity.Text) * double.Parse(unit.SelectedValue.ToString());
                itm.DateOfAddition = refDate;
                itm.IsotopeID = (long)isotopeCbo.SelectedValue;
                itm.Weight= double.Parse(weight.Text);
                itm.LocationID = (int)location.SelectedValue;
                Isotope iso = isotopeCbo.SelectedItem as Isotope;
                int daysLimit = await con.getWasteLimitDaysByIsotopeAsync(iso.ID);
                TimeSpan ts = TimeSpan.FromDays(daysLimit);
                itm.DateToBeRemoved = refDate + ts;
                itm.UserIDLoggingAddition = usr.id;
                if (await con.AddItemAsync(itm))
                {
                    clearForm();
                }
                else
                {
                    MessageBox.Show("Error in adding item to database");
                }
            }
            else
            {
                MessageBox.Show("The form is not completed correctly");
            }
        }

        private bool sanityCheckForm()
        {
            //output on checks
            bool checks = true;
            //generic iteration through controls
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(mainGrid); i++)
            {
                if (VisualTreeHelper.GetChild(mainGrid, i).GetType().Name == "TextBox")
                {
                    TextBox textboxcheck = (TextBox)VisualTreeHelper.GetChild(mainGrid, i);
                    if ((textboxcheck.Text == "") || textboxcheck.Text == null)
                    {
                        checks = false;
                    }
                }
                if (VisualTreeHelper.GetChild(mainGrid, i).GetType().Name == "ComboBox")
                {
                    ComboBox comboCheck = (ComboBox)VisualTreeHelper.GetChild(mainGrid, i);
                    if (comboCheck.SelectedIndex == -1)
                    {
                        checks = false;
                    }

                }

            }

            //form specific check

            double testDouble;

            if(!double.TryParse(activity.Text, out testDouble) || !double.TryParse(weight.Text, out testDouble))
            {
                checks = false;
            }

            return checks;
        }

        private void clearForm()
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(mainGrid); i++)
            {
                if (VisualTreeHelper.GetChild(mainGrid, i).GetType().Name == "TextBox")
                {
                    TextBox textboxcheck = (TextBox)VisualTreeHelper.GetChild(mainGrid, i);
                        textboxcheck.Clear();
                }
                if (VisualTreeHelper.GetChild(mainGrid, i).GetType().Name == "ComboBox")
                {
                    ComboBox comboCheck = (ComboBox)VisualTreeHelper.GetChild(mainGrid, i);
                        comboCheck.SelectedIndex = -1;
                }

            }
            unit.SelectedIndex = 0;
        }

        private async void initialisePage()
        {
            if(App.usr is null) { return; }
            //set dates to displays
            //dateAdded.Text = DateTime.Now.ToString();
            //timeAdded.Content = DateTime.Now.ToString("HH:mm");
            //set the scroll bar to correct position
            double hours = double.Parse(DateTime.Now.ToString("HH"));
            double mins = double.Parse(DateTime.Now.ToString("mm")) / 60;
            //timeScroll.Value = (hours + mins) / 24;

            //start connection object
            phpApiDbConnection con = new phpApiDbConnection();

            //db id's set to combo boxes values

            //Set Units for combo box
            Dictionary<string,double> map = new Dictionary<string, double>();
            map.Add(key:"MBq",value: 1);
            map.Add(key:"GBq",value:1000);
            map.Add(key:"KBq",value:0.001);
            map.Add(key:"Bq",value:0.000001);
            map.Add(key:"TBq",value:1000000);
            unit.ItemsSource= map;
            unit.SelectedValuePath = "Value";
            unit.DisplayMemberPath = "Key";
            unit.SelectedValue = 1;

            //uses connection to get isotope list for combo box 
            isotopeCbo.ItemsSource = await con.GetIsotopeListAsync();
            isotopeCbo.DisplayMemberPath = "Name";
            isotopeCbo.SelectedValuePath = "ID";

            //uses connecion to get locaitons form combo box 
            location.ItemsSource = await con.GetLocationListAsync();
            location.DisplayMemberPath = "Name";
            location.SelectedValuePath = "ID";

            isotopeCbo.Focus();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            clearForm();
            initialisePage();
        }

        private void resetForm_Click(object sender, RoutedEventArgs e)
        {
            clearForm();
        }

        private void Border_MouseEnter(object sender, MouseEventArgs e)
        {
            Border border = (Border)e.Source;
            var fill = (SolidColorBrush)new BrushConverter().ConvertFromString("Red");
            border.BorderBrush = fill;
       
        }
        private void Border_MouseLeave(object sender, MouseEventArgs e)
        {
            Border border = (Border)e.Source;
            var fill = (SolidColorBrush)new BrushConverter().ConvertFromString("White");
            border.BorderBrush = fill;
        }
    }

}
