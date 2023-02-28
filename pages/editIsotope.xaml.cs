using System;
using System.Collections.Generic;
using System.Drawing;
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
    /// Interaction logic for editIsotope.xaml
    /// </summary>
    public partial class editIsotope : Page
    {
        IsotopeListBox iso;
        public editIsotope()
        {
            InitializeComponent();
            iso = new IsotopeListBox(currentIsotopes);
            setWasteCboBox();

            iso.updateListBox();
        }

        private async Task setWasteCboBox()
        {
            phpApiDbConnection con = new phpApiDbConnection();
            List<WasteGroup> lst = new List<WasteGroup>();
            lst = await con.GetWasteCatListAsync();
            isotopeWasteCat.ItemsSource = lst;
            isotopeWasteCat.DisplayMemberPath = "Name";
            isotopeWasteCat.SelectedValuePath = "ID";
        }
        public void currentIsotopes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            iso.select(currentIsotopes.SelectedIndex);
            isotopeSymbol.Text = iso.getName(currentIsotopes.SelectedIndex);
            isotopeHalflife.Text = iso.getHalfLife(currentIsotopes.SelectedIndex);
            isotopeCPSToMBq.Text = iso.getCpsToMBq(currentIsotopes.SelectedIndex);
            //isotopeMass.Text = iso.getAtomicMass(currentIsotopes.SelectedIndex);
            isotopeWasteCat.SelectedValue = iso.getWasteCatID(currentIsotopes.SelectedIndex);

        }
        public async void updateIsotope_Click(object sender, RoutedEventArgs e)
        {
            if (inputSanityCheck())
            {
                if (!iso.getSelected()) {
                    MessageBox.Show("Please select an isotope to update");
                    return;
                }
                
                phpApiDbConnection con = new phpApiDbConnection();
                Isotope isoInput = new Isotope();
                isoInput.Name= isotopeSymbol.Text;
                isoInput.CpsToMBq = double.Parse(isotopeCPSToMBq.Text);
                isoInput.HalfLifeHours = double.Parse(isotopeHalflife.Text);
                isoInput.ID = iso.GetSelectedIsotope().ID;
                isoInput.WasteGroupId = (long)isotopeWasteCat.SelectedValue;
                isoInput.InUse = true;


                if ( await con.UpdateIsotopeAsync(isoInput))
                {
                    await iso.updateListBox();
                    resetForm();
                }
                else
                {
                    MessageBox.Show("Error updating Isotope");
                }
            }
            else
            {
                MessageBox.Show("Error on form please check");
            }
        }
        public async void addIsotope_Click(object sender, RoutedEventArgs e)
        {
            if (inputSanityCheck())
            {
                phpApiDbConnection con = new phpApiDbConnection();
                Isotope isoInput = new Isotope();
                isoInput.Name= isotopeSymbol.Text;
                isoInput.HalfLifeHours = double.Parse(isotopeHalflife.Text);
                isoInput.WasteGroupId = (long)isotopeWasteCat.SelectedValue;
                isoInput.InUse = true;
                isoInput.CpsToMBq = double.Parse(isotopeCPSToMBq.Text);

                if (await con.AddIsotopeAsync(isoInput))
                {
                    await iso.updateListBox();
                    resetForm();
                }
                else
                {
                    MessageBox.Show("Error adding isotope");
                }
            }
            else
            {
                MessageBox.Show("Error on form please check");
            }
        }
        public async void removeIsotope_CLick(object sender, RoutedEventArgs e)
        {
            if(iso.getSelected() && MessageBox.Show("You are going to remove Isotope " +iso.getName(currentIsotopes.SelectedIndex),"!",MessageBoxButton.YesNoCancel) == MessageBoxResult.Yes)
            {
                phpApiDbConnection con = new phpApiDbConnection();
                
                await con.RemoveIsotopeFromUseAsync(iso.GetSelectedIsotope());
                resetForm();
                await iso.updateListBox();
            }
        }

        private void isotopeWasteCat_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

          
        }

        private bool inputSanityCheck()
        {
            //output on checks
            bool checks = true;
            //generic iteration through controls
            for(int i = 0; i < VisualTreeHelper.GetChildrenCount(mainGrid); i++)
            {
                if(VisualTreeHelper.GetChild(mainGrid, i).GetType().Name == "TextBox")
                {
                    TextBox textboxcheck = (TextBox)VisualTreeHelper.GetChild(mainGrid, i);
                    if((textboxcheck.Text == "") || textboxcheck.Text == null)
                    {
                        checks= false;
                    }
                }
                if (VisualTreeHelper.GetChild(mainGrid, i).GetType().Name == "ComboBox")
                {
                    ComboBox comboCheck = (ComboBox)VisualTreeHelper.GetChild(mainGrid, i);
                    if(comboCheck.SelectedIndex == -1)
                    {
                        checks= false;
                    }
                }
            }
            
            //specific to page checks
            double testDoubleOut;
            //int testIntOut;
            if(!double.TryParse(isotopeCPSToMBq.Text, out testDoubleOut) || 
                !double.TryParse(isotopeHalflife.Text , out testDoubleOut) )
            {
                checks= false;
            }

            return checks;
        }
        private void resetForm()
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
        }
    }
}
