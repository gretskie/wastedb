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

namespace WasteApp.pages
{
    /// <summary>
    /// Interaction logic for adminPage.xaml
    /// </summary>
    public partial class adminPage : Page
    {
        public adminPage()
        {
            InitializeComponent();
        }

        private void editIsotope_click(object sender, RoutedEventArgs e)
        {
            adminMainFrame.Content = new editIsotope();
        }

        private void editCat_click(object sender, RoutedEventArgs e)
        {
            adminMainFrame.Content = new editCat();
        }

        private void editLocation_click(object sender, RoutedEventArgs e)
        {
            adminMainFrame.Content = new editLocations();
        }
        private void editDisposal_click(object sender, RoutedEventArgs e)
        {
            adminMainFrame.Content = new EditDisposalRoutes();
        }
        private void editUsers_click(object sender, RoutedEventArgs e)
        {
            adminMainFrame.Content = new EditUsers();
        }
    }
}
