using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Net.Http;
using WasteApp.classes;

namespace WasteApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static readonly HttpClient client = new HttpClient();
        public static user usr;

        private void Trigger_Selected(object sender, RoutedEventArgs e)
        {

        }
    }
}
