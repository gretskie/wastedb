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
using WasteApp.classes;
using System.Timers;
using System.ComponentModel;

namespace WasteApp.pages
{
    /// <summary>
    /// Interaction logic for Page1.xaml
    /// </summary>
    public partial class summary : Page
    {
        private static System.Timers.Timer atimer;
        private static List<item> items;
        graph chart;
        public summary()
        {
            InitializeComponent();
            
            items= new List<item>();
            //get items for list from server
            UpdateItems();
            UpdateGraph();
            setTimer();

        }

        //refreshes data from DB and loads to the display. 
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateItems();
            UpdateGraph();
            atimer.Start();
        }

        //sets the timed event for updating the activity in the list box
        private void setTimer()
        {  
            atimer = new System.Timers.Timer(500);
            atimer.Elapsed += OnTimedEvent;
            atimer.Start();

        }

        //recalculates the current activity for the display. 
        private void OnTimedEvent(object obj, ElapsedEventArgs e)
        {
            if(items is null) { atimer.Interval = 50; return; } else { atimer.Interval = 5000; }

            for(int i = 0; i< items.Count; i++)
            {
                items[i].currentActivity = items[i].getCurrentActivity();
                items[i].updateCloseToDateBool();
                items[i].ActivtyPerGram = items[i].getCurrentActivityPerGram();
            }
            this.Dispatcher.Invoke(new Action(updateItemsListView));
            this.Dispatcher.Invoke(new Action(UpdateGraph));
        }

        private void updateItemsListView()
        {

            itemsForDisposal.ItemsSource = null;
            itemsForDisposal.ItemsSource = items;           
        }

        //halts the timed event if the page is not visable. 
        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            atimer.Stop();
            atimer.Close();
        }

        private void UpdateGraph()
        {
            if(App.usr is null) { return; }
            if (items is null) { return; }
            UpdateItemsActivities();
            var listOfIsotopes = from lstO in items
                             group lstO by lstO.isotopeId into newlst
                             select newlst;

            
            if(listOfIsotopes.Count() == 0)
            {
                return;
            }
            List<currentActivityTotalByIsotope> outTotalList = new List<currentActivityTotalByIsotope>();
            
            foreach (var isotopeID in listOfIsotopes)
            {
                currentActivityTotalByIsotope cur = new currentActivityTotalByIsotope();
                double total = items.Where(tot => tot.isotopeId == int.Parse(isotopeID.Key.ToString())).Sum(tot => tot.currentActivity)/1000000;
                var symbol = items.Where(grp => grp.isotopeId == int.Parse(isotopeID.Key.ToString())).GroupBy(grp => grp.isotopeSymbol);
                var limitQuant =items.Where(lmt => lmt.isotopeId == int.Parse(isotopeID.Key.ToString())).GroupBy(lmt => lmt.limitQuant);
                cur.totalActivity= total;
                cur.isotopeSymbol = symbol.First().Key;
                cur.limitQuant = limitQuant.First().Key;
                cur.percentOfLimit = cur.totalActivity / cur.limitQuant;
                outTotalList.Add(cur);
            }

            chart = new graph(summaryChart, outTotalList );
            chart.displaySummaryGraph();
        }
        private async void UpdateItems()
        {
            if (App.usr is null) { return; }
            phpApiDbConnection con = new phpApiDbConnection();
            List<item> testItemsList = await con.GetItemsToBeRemovedAsync();
            if (!this.IsLoaded) { return; }
            if(testItemsList is null) { return; }

            items = testItemsList;
            itemsForDisposal.ItemsSource = null;
            itemsForDisposal.ItemsSource = items;
        }

        private void UpdateItemsActivities()
        {
            if (App.usr is null || items is null) return;
            foreach(item itm in items)
            {
                itm.currentActivity = itm.getCurrentActivity();
            }
        }
    }
}
