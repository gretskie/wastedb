using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Windows.Controls;
using System.Windows;
using WasteApp.APICLasses;

namespace WasteApp.classes
{

    internal class IsotopeListBox
    {

        private List<Isotope> isotopeList;
        private ListBox lst;
        private bool errorLoad = false;
        private bool selected = false;
        private long selectedPosition;

        public IsotopeListBox(ListBox lst)
        {
            isotopeList = new List<Isotope>();
            this.lst = lst;
            Task.Run(loadIsotopes);
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
                if (this.selectedPosition == -1) { return null; }
                return this.isotopeList[(int)this.selectedPosition].Name;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return null;
            }
        }
        public string getHalfLife(long index)
        {
            try
            {
                if (this.selectedPosition == -1) { return null; }
                return this.isotopeList[(int)this.selectedPosition].HalfLifeHours.ToString();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return null;
            }
        }
        public string getCpsToMBq(long index)
        {
            try
            {
                if (this.selectedPosition == -1) { return null; }
                return this.isotopeList[(int)this.selectedPosition].CpsToMBq.ToString();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return null;
            }
        }
        public long getWasteCatID(long index)
        {
            try
            {
                if (this.selectedPosition == -1) { return -1; }
                return this.isotopeList[(int)this.selectedPosition].WasteGroupId;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return -1;
            }
        }
        public long id()
        {
            return this.selected ? this.isotopeList[(int)this.selectedPosition].ID : -1;
        }
        public Isotope GetSelectedIsotope()
        {
            if(!this.selected) return null;
            return this.isotopeList[(int)this.selectedPosition];
        }

        public async Task loadIsotopes()
        {
            try
            {
                if (isotopeList != null) { isotopeList.Clear(); }
                phpApiDbConnection con = new phpApiDbConnection();
                
                isotopeList = await con.GetIsotopeListAsync();
            }catch(Exception e)
            {
                MessageBox.Show(e.Message); throw;
            }
        }
   
        public async Task updateListBox()
        {
            await this.loadIsotopes();
            if (!this.errorLoad)
            {
                this.lst.Items.Clear();
                foreach (Isotope isotope in isotopeList)
                {
                    this.lst.Items.Add(isotope.Name);
                }
                this.lst.SelectedIndex = -1;
                this.selected = false;
            }
        }
    }
}
