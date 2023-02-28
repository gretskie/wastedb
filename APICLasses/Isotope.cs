using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WasteApp.classes;

namespace WasteApp.APICLasses
{
    public class Isotope
    {
        public string Name { get; set; }
        public long ID { get; set; }
        public double HalfLifeHours { get; set; }
        public long WasteGroupId { get; set; }
        public double CpsToMBq { get; set; }
        public bool InUse { get; set; }

        public Isotope()
        {
        }
        public double Lambda()
        {
            return ((double)Math.Log(2)/this.HalfLifeHours);
        }

        public double CurrentActivity(double activityAtRef, DateTime refDateTime)
        {
            DateTime currentDateTime = DateTime.Now;
            TimeSpan timeDiff = currentDateTime.Subtract(refDateTime);
            double totalHours = timeDiff.TotalHours;

            double currentActivity = activityAtRef * Math.Exp(-this.Lambda() * totalHours);
            return currentActivity;
        }
    }
}
