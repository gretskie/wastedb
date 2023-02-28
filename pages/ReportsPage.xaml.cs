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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WasteApp.APICLasses;
using WasteApp.classes;

namespace WasteApp.pages
{
    /// <summary>
    /// Interaction logic for ReportsPage.xaml
    /// </summary>
    public partial class ReportsPage : Page
    {
        List<item> items;
        public ReportsPage()
        {
            InitializeComponent();
            items = new List<item>();
            loadingScreen.Visibility = Visibility.Visible;
            LoadAllResources();
        }

        private async Task LoadAllResources()
        {
            Dictionary<int,string> disposedCbo= new Dictionary<int,string>();
            disposedCbo.Add( 1,"Disposed");
            disposedCbo.Add(2, "Not Disposed");
            disposedCbo.Add(3, "All items");
            cboDisposed.ItemsSource= disposedCbo;
            cboDisposed.DisplayMemberPath = "Value";
            cboDisposed.SelectedValuePath= "Key";
            if (await LoadIsotopesCbo() != true)
            {
                MessageBox.Show("Error loading isotopes from DB \n Please try again by reloading this page");
                return;
            }

            if(await LoadAllItems() != true)
            {
                MessageBox.Show("Error loading items from DB \nPlease try again by reloading this page");
            }

            loadingScreen.Visibility = Visibility.Hidden;
        }

        private async Task<bool> LoadIsotopesCbo()
        {
            phpApiDbConnection con = new phpApiDbConnection();
            List<Isotope> isotopeList = await con.GetIsotopeListAsync();
            cboIsotope.ItemsSource= isotopeList;
            cboIsotope.DisplayMemberPath = "Name";
            cboIsotope.SelectedValuePath = "ID";
            if(isotopeList == null || isotopeList.Count < 1) { return false; }
            return true;
        }
        private async Task<bool> LoadAllItems()
        {
            bool checks = false;
            phpApiDbConnection con = new phpApiDbConnection();           
            items = await con.GetAllItemsInDB();
            if(items != null && items.Count >0) {
                checks = true;
            }
            return checks;
        }

        private void btnGo_Click(object sender, RoutedEventArgs e)
        {
            if(cboDisposed.SelectedIndex== -1) { return; }
            if(cboIsotope.SelectedIndex== -1) { return; }
            if(dpStartDate.SelectedDate == null || dpEndDate.SelectedDate == null) { return; }
            List<item> outList = new List<item>();
            switch(cboDisposed.SelectedValue) {
                case(1):
                    outList = items.Where(itm => itm.isotopeId == int.Parse(cboIsotope.SelectedValue.ToString())).Where(itm=>itm.dateDisposed < dpEndDate.SelectedDate  && itm.dateDisposed > dpStartDate.SelectedDate).Where(itm=>itm.disposed == true).ToList();
                    break;
                case(2):
                    outList = items.Where(itm => itm.isotopeId == int.Parse(cboIsotope.SelectedValue.ToString())).Where(itm => itm.dateAdded < dpEndDate.SelectedDate && itm.dateAdded > dpStartDate.SelectedDate).Where(itm => itm.disposed == false).ToList();
                    break;
                case(3):
                    outList = items.Where(itm => itm.isotopeId == int.Parse(cboIsotope.SelectedValue.ToString())).Where(itm => itm.dateAdded < dpEndDate.SelectedDate && itm.dateAdded > dpStartDate.SelectedDate).ToList();
                    break;
                default:
                    break;
            }
            
            lvOutput.ItemsSource = outList;
        }
    }
}
