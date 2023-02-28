using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Data;
using WasteApp.APICLasses;

namespace WasteApp.classes
{
    //class location
    //{
    //    public long id { get; set; }
    //    public string name { get; set; }      
    //}
    class locationListBox
    {
        private ListBox lst;
        private bool errorLoad = false;
        private bool selected = false;
        private long selectedPosition;
        private List<Location> locationList;

        public locationListBox(ListBox lst)
        {
            locationList = new List<Location>();
            this.lst = lst;
            this.lst.DisplayMemberPath = "Name";
            this.lst.SelectedValuePath= "ID";
            this.updateListBox();
        }
        public bool getSelected()
        {
            return this.selected;
        }

        public Location GetSelectedLocationObj()
        {
            if(!selected || this.selectedPosition == -1) { return null; }
            return locationList[(int)this.selectedPosition];
        }

        private async Task loadlocations()
        {
            try
            {
                if (locationList != null) { locationList.Clear(); }
                phpApiDbConnection con = new phpApiDbConnection();
                locationList = await con.GetLocationListAsync();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                throw;
            }
        }
        public async Task updateListBox()
        {
            await this.loadlocations();
            this.lst.ItemsSource = null;
            this.lst.ItemsSource = locationList;
        }
        private void SetListBox()
        {
            this.lst.ItemsSource = this.locationList;
            this.lst.DisplayMemberPath = "Name";
            this.lst.SelectedValuePath = "ID";
            this.lst.SelectedIndex = -1;
            this.selected = false;
        }
        public void select(long index)
        {
            this.selectedPosition = index;
            this.selected = true;
            //MessageBox.Show(this.locationList[(int)this.selectedPosition].id.ToString() + " , " + this.locationList[(int)this.selectedPosition].name.ToString());
        }
        public string getName(long index)
        {
            try
            {
                if(this.selectedPosition == -1) { return null; }
                return this.locationList[(int)this.selectedPosition].Name;
            }

            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return null;
            }
        }

        public long id()
        {
            if (this.selected)
            {
                return this.locationList[(int)this.selectedPosition].ID;
            }
            else
            {
                MessageBox.Show("Nothing Selected");
                return -1;
            }
        }              
    }
}
