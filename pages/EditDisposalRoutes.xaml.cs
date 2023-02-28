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
    /// Interaction logic for EditDisposalRoutes.xaml
    /// </summary>
    public partial class EditDisposalRoutes : Page
    {

        List<DisposalRoute> routes;
        public EditDisposalRoutes()
        {
            InitializeComponent();
            routes = new List<DisposalRoute>();
            UpdateListBox();
        }

        private async Task UpdateListBox()
        {
            phpApiDbConnection con = new phpApiDbConnection();
            routes.Clear();
            routes = await con.GetActiveDisposalRoutesAsync();
            currentRoutes.ItemsSource = routes;
            currentRoutes.SelectedValuePath = "ID";
            currentRoutes.DisplayMemberPath = "name";
        }
        private void RefreshListBox()
        {
            currentRoutes.ItemsSource = null;
            currentRoutes.ItemsSource = routes;
        }
        private void ClearForm()
        {
            routeName.Text = string.Empty;
        }

        private void CurrentRoutes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (currentRoutes.SelectedItem != null || currentRoutes.SelectedIndex != -1)
            {
                DisposalRoute route = currentRoutes.SelectedItem as DisposalRoute;
                routeName.Text = route.name;
            }
        }
        private async void UpdateRoute_Click(object sender, RoutedEventArgs e)
        {
            if (currentRoutes.SelectedItem != null || currentRoutes.SelectedIndex != -1)
            {
                DisposalRoute route = currentRoutes.SelectedItem as DisposalRoute;
                route.name = routeName.Text;
                phpApiDbConnection con = new phpApiDbConnection();
                if(await con.UpdateDisposalRoute(route))
                {
                   await UpdateListBox();
                    RefreshListBox();
                    ClearForm();
                }
                else
                {
                    MessageBox.Show("Error in updating disposal route in DB");
                }
            }
            else
            {
                MessageBox.Show("There is somthing wrong with the form please check");
            }
        }
        private async void AddRoute_Click(Object sender, RoutedEventArgs e)
        {
            DisposalRoute route = new DisposalRoute();
            route.name = routeName.Text;
            route.inUse = true;
            phpApiDbConnection con = new phpApiDbConnection();
            if (await con.AddDisposalRoute(route))
            {
                await UpdateListBox();
                RefreshListBox();
                ClearForm();
            }
            else
            {
                MessageBox.Show("Error in updating disposal route in DB");
            }
        }
        private async void RemoveRoute_CLick(object sender, RoutedEventArgs e)
        {
            if (currentRoutes.SelectedItem != null || currentRoutes.SelectedIndex != -1)
            {
                DisposalRoute route = currentRoutes.SelectedItem as DisposalRoute;
                phpApiDbConnection con = new phpApiDbConnection();
                if (await con.RemoveDisposalRoute(route))
                {
                    await UpdateListBox();
                    RefreshListBox();
                    ClearForm();
                }
                else
                {
                    MessageBox.Show("Error in updating disposal route in DB");
                }
            }
        }
    }
}
