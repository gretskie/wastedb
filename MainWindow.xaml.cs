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
using WasteApp.pages;

namespace WasteApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            if(App.usr == null || App.usr.loaded == false)
            {
                login log = new login(this);
                log.Visibility = Visibility.Visible;
                this.Visibility = Visibility.Hidden;
            }
            else
            {
                if (App.usr.userLevel != 1) { btnAdmin.Visibility = Visibility.Hidden; } else { btnAdmin.Visibility = Visibility.Visible; }
            }
            InitializeComponent();
            
            //summary summaryPage = new summary();
            //mainFramePage.Content = summaryPage;
        }

        private void page1_click(object sender, RoutedEventArgs e)
        {
            summary summaryPage = new summary();
            mainFramePage.Content = summaryPage;
        }

        private void page2_click(object sender, RoutedEventArgs e)
        {
            addItem additemPage = new addItem();
            mainFramePage.Content = additemPage;
        }

        private void page3_click(object sender, RoutedEventArgs e)
        {
            moveItem moveItemPage = new moveItem();
            mainFramePage.Content = moveItemPage;
        }
        private void page4_click(object sender, RoutedEventArgs e)
        {
            disposeItem disposePage = new disposeItem();
            mainFramePage.Content = disposePage;
        }

        private void page5_click(object sender, RoutedEventArgs e)
        {
            //tests for the admin users level to be present
            if (App.usr.userLevel == 1)
            {
                adminPage admin = new adminPage();
                mainFramePage.Content = admin;
            }
        }
        private void btnReports_click(object sender, RoutedEventArgs e)
        {            //tests for the admin users level to be present
            if (App.usr.userLevel == 1)
            {
                ReportsPage page = new ReportsPage();
                mainFramePage.Content = page;
            }
        }
        
        //operates the logout call from other pages in main frame but keeps this mainwindow alive but hidden. 
        public void LogOut_Click(object sender, RoutedEventArgs e)
        {
            login log = new login(this);
            log.Visibility = Visibility.Visible;
            this.Visibility = Visibility.Hidden;
            App.usr = null;
        }

        private void Window_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if(this.Visibility == Visibility.Visible)
            {
                if(App.usr != null)
                {
                    if (App.usr.userLevel != 1) { btnAdmin.Visibility = Visibility.Hidden; btnReports.Visibility = Visibility.Hidden; } else { btnAdmin.Visibility = Visibility.Visible; btnReports.Visibility = Visibility.Visible; }
                }
                summary summaryPage = new summary();
                mainFramePage.Content = summaryPage;
            }
        }
    }
}
