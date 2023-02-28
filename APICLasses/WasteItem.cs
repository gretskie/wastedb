using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WasteApp.classes;

namespace WasteApp.APICLasses
{
    public class WasteItem
    {
        public long ID { get; set; }
        public string TagNo { get; set; }
        public long IsotopeID { get; set; }
        public double ActivityOnAddition { get; set; }
        public DateTime DateOfAddition { get; set; }
        public int DisposalRouteID { get; set; }
        public long LocationID { get; set; }
        public bool Disposed { get; set; }
        public DateTime dateDisposed { get; set; }
        public long UserIDDisposing { get; set; }
        public long UserIDLoggingDispose { get; set; }
        public long UserIDLoggingAddition { get; set; }
        public long PreviousID { get; set; }
        public DateTime DateToBeRemoved { get; set; }
        public double Weight { get; set; }

        public void LoadPropertiesFromAltItem(item itm)
        {
            this.ID = itm.id;
            this.ActivityOnAddition = itm.refActivity;
            this.Weight = itm.weight;
            this.DateOfAddition = itm.dateAdded;
            this.DateToBeRemoved = itm.dateToBeRemoved;
            this.IsotopeID = itm.isotopeId;
            this.LocationID = itm.locationId;
            this.TagNo = itm.tagNo;
        }
    }


}
