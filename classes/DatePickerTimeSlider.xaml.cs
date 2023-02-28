using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WasteApp.classes
{
    /// <summary>
    /// Interaction logic for DatePickerTimeSlider.xaml
    /// </summary>
    public partial class DatePickerTimeSlider : UserControl
    {
        public DatePickerTimeSlider()
        {
            InitializeComponent();
            dateAdded.Text = DateTime.Now.ToString();
            timeAdded.Content = DateTime.Now.ToString("HH:mm");
            double hours = double.Parse(DateTime.Now.ToString("HH"));
            double mins = double.Parse(DateTime.Now.ToString("mm")) / 60;
            timeScroll.Value = (hours + mins) / 24;
        }

        private void time_valueChange(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double hours;
            double mins;
            int hoursInt;
            int minsInt;
            string minsString;
            string hoursString;
            hours = timeScroll.Value * 24;
            hoursInt = (int)hours;
            mins = ((timeScroll.Value * 24) - hoursInt) * 60;
            minsInt = (int)mins;

            hoursString = (hoursInt == 24) ? "00" : (hoursInt < 10) ? "0" + hoursInt.ToString() : hoursInt.ToString();
            minsString = (minsInt < 10) ? "0" + minsInt.ToString() : minsInt.ToString();
            string timeOutput = hoursString + ":" + minsString;
            timeAdded.Content = timeOutput;
        }

        public DateTime GetDateTime()
        {
            string refDate = dateAdded.Text + " " + timeAdded.Content + ":00";
            return DateTime.Parse(refDate);
        }
    }
}
