using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace WasteApp.classes
{
    [ValueConversion(typeof(object), typeof(int))]
    public class listviewDateConvertion : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            TimeSpan t = DateTime.Now - (DateTime)value;
            if (t.Days < 30)
            {
                return 1;
            }
            else if(t.Days < 3)
            {
                return 2;
            }
            else
            {
                return 0;
            }

               
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
