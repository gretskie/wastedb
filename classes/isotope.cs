using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WasteApp.classes;

namespace WasteApp.classes
{
    public class isotope
    {

            public int Id { get; set; }
            public string Symbol { get; set; }
            public int AtomicMass { get; set; }
            public double HalflifeHours { get; set; }
            private long wasteCat;
            public double CpsToMBq { get; set; }

            public isotope(int id, string sym, int atomic, double half, long waste, double cps)
            {
                this.Id = id;
                this.Symbol = sym;
                this.AtomicMass = atomic;
                this.HalflifeHours = half;
                this.wasteCat = waste;
                this.CpsToMBq = cps;
            }
            public isotope(int id)
            {
                dbConnection con = new dbConnection();
                var iso = con.getAllIsotopeInfo(id.ToString());
                this.Id = iso.Id;
                this.Symbol = iso.Symbol;
                this.AtomicMass = iso.AtomicMass;
                this.HalflifeHours = iso.HalflifeHours;
                this.wasteCat= iso.wasteCat;
                this.CpsToMBq= iso.CpsToMBq;
            }
            public double lambda()
            {
                return ((double)Math.Log(2) / this.HalflifeHours);
            }
        public long getWasteCatId()
        {
            return this.wasteCat;
        }
        public double currentActivity(double activityAtRef, DateTime refDate)
        {
            DateTime currentDateTime = DateTime.Now;
            TimeSpan timeDiff = currentDateTime.Subtract(refDate);
            double totalHours = timeDiff.TotalHours;

            double currentActivity = activityAtRef * Math.Exp(-this.lambda() * totalHours);
            return currentActivity;
        }
       
         
    }
}
