using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace WasteApp.classes
{
    public class item : IEquatable<item>, IComparable<item>
    {

        public long id { get; set; } = 0;
        public int isotopeId { get; set; } = 0;
        public string isotopeSymbol { get; set; }
        public double halflife { get; set; } =0;
        public long locationId { get; set; } = 0;
        public string locationName { get; set; }
        public double weight { get; set; } = 0;
        public double refActivity { get; set; } = 0;  
        public DateTime refDate { get; set; }
        public DateTime dateAdded { get; set; }
        public DateTime dateDisposed { get; set; }
        public bool disposed { get; set; } = false;
        public long disposalRouteId { get; set; } = 0;
        public DateTime dateToBeRemoved { get; set; }
        public string tagNo { get; set;}
        public double currentActivity { get; set; } = 0;
        public double cpsToMBq { get; set; } = 0;
        public string wasteCatName { get; set; }
        public double limitQuant { get; set; } = 0;
        public int limitDays { get; set; } = 0;
        public bool closeToDisposalDate { get; set; } = false;
        public double ActivtyPerGram { get; set; }

        public item()
        {

        }

        public int CompareTo(item other)
        {
            if (other == null)
            {
                return 1;
            }
            else 
            {
                return this.dateToBeRemoved.CompareTo(other.dateToBeRemoved);
            }
        }

        public bool Equals(item other)
        {
            if(other == null) { return false; }
            return (this.id== other.id);
        }

        public double getCurrentActivity()
        {
            double lamb = Math.Log(2) / this.halflife;
            TimeSpan t = DateTime.Now - refDate;
            return this.refActivity*1000000 * (float)Math.Exp(-lamb * t.TotalHours);
        }

        public double getCurrentActivityPerGram()
        {
            return this.currentActivity / (this.weight * 1000);
        }

        public void updateCloseToDateBool()
        {
            TimeSpan t = this.dateToBeRemoved - DateTime.Now;
            if(t.Days < 30)
            {
                this.closeToDisposalDate = true;
            }
            else
            {
                this.closeToDisposalDate=false;
            }
        }

    }
}
