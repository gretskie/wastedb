using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WasteApp.APICLasses;

namespace WasteApp.classes
{
    public class wasteCat 
    { 
        public long id { get; set; }
        public string name { get; set; }
        public double limitQuant { get; set; }
        public int limitDays { get; set; }
    }

    class wasteCatListBox
    {
        private List<WasteGroup> wasteList;
        private ListBox lst;
        private bool errorLoad = false;
        private bool selected = false;
        private long selectedPosition;

        public wasteCatListBox(ListBox lst)
        { 
            wasteList = new List<WasteGroup>();
            this.lst = lst;
            this.lst.DisplayMemberPath = "Name";
            this.lst.SelectedValuePath = "ID";
            this.updateListBox();
        
        }
        public bool getSelected()
        {
            return this.selected;
        }
        public void select(long index)
        {
            this.selectedPosition = index;
            this.selected = true;
        }
       public string getName(long index)
        {
            try
            {
                if(this.selectedPosition == -1) { return null; }
                return this.wasteList[(int)this.selectedPosition].Name;
            } 
            catch(Exception e)
            {
                MessageBox.Show(e.Message);
                return null;
            }
       }
        public double getLinitQuant(long index)
        {
            try
            {
                if (this.selectedPosition == -1) { return -1; }
                return this.wasteList[(int)this.selectedPosition].activityLimit;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return -1;
            }
        }

        public int getLimitDays(long index)
        {
            try
            {
                if (this.selectedPosition == -1) { return -1; }
                return this.wasteList[(int)this.selectedPosition].maxAccumulationDays;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return -1;
            }
        }

        public int GetAlertLimit()
        {
            try
            {
                if (this.selectedPosition == -1) { return -1; }
                //return this.wasteList[(int)this.selectedPosition].;
                 WasteGroup wasteGroup = (WasteGroup)this.lst.SelectedItem;
                return wasteGroup.decayAlertDays;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return -1;
            }
        }
        public long id()
        {
            return this.selected ? this.wasteList[(int)this.selectedPosition].ID : -1;
        }

        private async Task loadWasteCat()
        {
            try
            {

                if(wasteList != null) { wasteList.Clear(); }
                phpApiDbConnection con = new phpApiDbConnection();
                wasteList = await con.GetWasteCatListAsync();
            }
            catch (Exception e) 
            { 
                MessageBox.Show(e.Message);
                throw;
            }
        }

        public async void updateListBox()
        {
            await this.loadWasteCat();
            this.lst.ItemsSource = null;
            this.lst.ItemsSource = wasteList;
            //if(!this.errorLoad)
            //{
            //    this.lst.Items.Clear();
            //    foreach(WasteGroup waste in wasteList)
            //    {
            //        this.lst.Items.Add(waste.Name);
            //    }
            //    this.lst.SelectedIndex = -1;
            //    this.selected = false;
            //}
        }
    }
}
