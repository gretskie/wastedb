using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.CodeDom;
using WasteApp;
using System.Text.Json;
using ScottPlot.Renderable;
using System.Windows;

namespace WasteApp.classes
{
    public class user
    {
        public long id { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        private string password { get; set; }
        public bool active { get; set; }
        public long userLevel { get; set; }
        public bool emailAlert { get; set; }
        public string authCode { get; set; }
        public bool inUse { get; set; }
        public bool loaded { get; set; } = false;
        public user()
        {

        }
        public user(string username, string pass) 
        {
            this.checkUserAuth(username, pass);
        }

        public void setPassword(string password)
        {
            this.password = password;
        }

        public async void checkUserAuth(string username, string pass)
        {
            var values = new Dictionary<string, string>() { { "username", username }, { "password", pass } };
            var content = new FormUrlEncodedContent(values);
            var response = await App.client.PostAsync(phpApiDbConnection.GetLogonURL(), content);
            var responseString = await response.Content.ReadAsStringAsync();
            if (responseString.Substring(0, 5) == "Error")
            {
                this.id = -1;
                this.loaded= true;
            }
            else
            {
                Dictionary<string,string> output = JsonSerializer.Deserialize<Dictionary<string,string>>(responseString);

                
                this.id = long.Parse(output["id"]);
                this.name = output["name"];
                this.email = output["email"];
                this.password = pass;
                this.active = this.checkStringToBool(output["active"]);
                this.userLevel = long.Parse(output["level"]);
                this.emailAlert = this.checkStringToBool(output["alert"]);
                this.authCode = output["auth"];
                this.loaded = true;
                
            }
        }

        private bool checkStringToBool(string value)
        {
            if (value == "1")
            {
                return true;
            }
            else if (value == "0")
            {
                return false;
            }
            else
            {
                throw new Exception("problem converting json string to bool");
            }
        }
    }
}
