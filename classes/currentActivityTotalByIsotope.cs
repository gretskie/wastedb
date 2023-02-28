using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WasteApp.classes
{
    public class currentActivityTotalByIsotope
    {
        public string isotopeSymbol { get; set; }
        public double totalActivity { get; set; }
        public double limitQuant { get; set; } 
        public double percentOfLimit { get; set; }  

    }
}
