using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WasteApp.APICLasses
{
    public class WasteGroup
    {
        public long ID { get; set; }
        public string Name { get; set; }
        public int decayAlertDays { get; set; }
        public int maxAccumulationDays { get; set; }
        public double activityLimit { get; set; }
        public bool inUse { get; set; }
        
    }
}
