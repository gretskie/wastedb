using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Net.Http;
using System.Windows;
using System.Data;
using WasteApp.APICLasses;
using System.Windows.Documents;
using System.Windows.Media.Animation;

namespace WasteApp.classes
{
    // requires app.user to be in usr variable.
    // requires wasteCatAPI classes to transalte. 
    public class phpApiDbConnection
    {
        private static string _connectionServer = @"https://wastedb.co.uk/php/API";
        private static string _logonScript = "logon.php";
        private static string _indexScript = "index.php";
        private HttpClient _httpClient;
        public phpApiDbConnection()
        {
            _httpClient = new HttpClient();
        }

        public static string GetLogonURL()
        {
            return _connectionServer + @"/" + _logonScript;
        }

        private static string GetURL()
        {
            return _connectionServer + @"/" + _indexScript;
        }

        //----------------------------------------------------------------
        // Main two API connection calls returning a list of dictionary objects 

        private async Task<List<Dictionary<string, string>>> QueryDBAsync(string type, string function, string param)
        {
            //check if no user has been loaded
            if(App.usr is null) { return null; }
            Dictionary<string, string> errorOutPut = new Dictionary<string, string>() { { "Error", "Error" } };
            try
            {
                user currentUser = App.usr;
                var values = new Dictionary<string, string>(){ { "username", currentUser.name },
                {"auth" , currentUser.authCode } ,{"type",type },
                    {"userid",currentUser.id.ToString() }, {"function", function } , {"params",param } };
                var content = new FormUrlEncodedContent(values);
                var response = await _httpClient.PostAsync(GetURL(), content);
                var responseString = await response.Content.ReadAsStringAsync();

                if (responseString.Substring(0, 5) == "Error")
                {
                    MessageBox.Show("Error in DB connection \n" + responseString);
                    
                    return new List<Dictionary<string, string>>() { errorOutPut };
                }
                else if(responseString.Substring(0, 9) == "USERERROR")
                {
                    MessageBox.Show("User authorisation faliure\nPlease logout and back in!");
                    return new List<Dictionary<string, string>>() { errorOutPut };
                }
                else
                {
                    List<Dictionary<string, string>> output = JsonSerializer.Deserialize<List<Dictionary<string, string>>>(responseString);
                    return output;
                }
            }catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
                return new List<Dictionary<string, string>>() { errorOutPut };
            }

        }

        private async Task<bool> AddUpdateTableAsync(string type, string function, string param)
        {
            try
            {
                user currentUser = App.usr;
                var values = new Dictionary<string, string>(){ { "username", currentUser.name },
                {"auth" , currentUser.authCode } ,
                    {"userid",currentUser.id.ToString() },{"type",type },
                    {"function",function },
                       {"params", param } };
                var content = new FormUrlEncodedContent(values);
                var response = await _httpClient.PostAsync(GetURL(), content);
                var responseString = await response.Content.ReadAsStringAsync();
                if (responseString.Substring(0, 9) == "USERERROR")
                {
                    MessageBox.Show("user authorisation faliure\nPlease logout and back in!");
                    return false;
                }
                else if (responseString.Substring(0, 8) != "Complete") 
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        //---------------------------------------------------------------------
        // Convertion to C# objects from json response
        //-----------------------------------------------------------------------

        private List<WasteGroup> convertToWasteList(List<Dictionary<string, string>> map)
        {
            try
            {
                List<WasteGroup> output = new List<WasteGroup>();
                Dictionary<string,string> dic = new Dictionary<string,string>();
                for (int i = 0; i < map.Count; i++)
                {
                    WasteGroup wstcat = new WasteGroup();
                    dic = map[i];
                    wstcat.ID = long.Parse(dic["ID"]);
                    wstcat.activityLimit = double.Parse(dic["activityLimit"]);
                    wstcat.Name = dic["Name"];
                    wstcat.maxAccumulationDays = int.Parse(dic["maxAccumulationDays"]);
                    wstcat.decayAlertDays = int.Parse(dic["decayAlertDays"]);
                    output.Add(wstcat);
                }
                return output;
            }catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }
        private List<Location> ConvertToLocationList(List<Dictionary<string, string>> map)
        {
            try
            {
                List<Location> output = new List<Location>();
                Dictionary<string, string> dic = new Dictionary<string, string>();
                for(int i = 0; i < map.Count; i++)
                {
                    Location loc = new Location();
                    dic = map[i];
                    loc.Name= dic["Name"];
                    loc.ID = int.Parse( dic["ID"]);
                    loc.InUse = true;
                    output.Add(loc);
                }
                return output;

            }catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
            
        }
        private List<Isotope> ConverToIsotopeList(List<Dictionary<string, string>> map)
        {
            try
            {
                List<Isotope> output = new List<Isotope>();
                Dictionary<string, string> dic = new Dictionary<string, string>();
                for(int i = 0;i< map.Count; i++)
                {
                    dic = map[i];
                    Isotope iso = new Isotope();
                    iso.ID = long.Parse(dic["ID"]);
                    iso.Name= dic["Name"];
                    iso.WasteGroupId = long.Parse(dic["wasteGroupID"]);
                    iso.CpsToMBq = double.Parse(dic["cpsConvertion"]);
                    iso.HalfLifeHours = double.Parse(dic["halfLifeHours"]);
                    iso.InUse = true;
                    output.Add(iso);
                }
                return output;

            }catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }
        private List<currentActivityTotalByIsotope> ConvertToTotalActivityByIsotope(List<Dictionary<string,string>> map)
        {
            try
            {
                List<currentActivityTotalByIsotope> output = new List<currentActivityTotalByIsotope>();
                Dictionary<string,string> dic = new Dictionary<string,string>();
                for(int i = 0; i < map.Count ; i++)
                {
                    dic= map[i];
                    currentActivityTotalByIsotope curIso = new currentActivityTotalByIsotope();
                    curIso.totalActivity = double.Parse(dic["totalActivity"]);
                    curIso.limitQuant = double.Parse(dic["activityLimit"]);
                    curIso.percentOfLimit = double.Parse(dic["limitPercentage"]);
                    curIso.isotopeSymbol = dic["Name"];
                    output.Add(curIso);
                }
                return output;

            }catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }
        private List<item> ConvertToItemsList(List<Dictionary<string,string>> map) 
        {
            try
            {
                List<item> output = new List<item>();
                Dictionary<string,string> dic = new Dictionary<string,string>();
                for (int i = 0; i < map.Count; i++)
                {
                    dic = map[i];
                    item outItem = new item();
                    outItem.id = int.Parse(dic["id"]);
                    outItem.isotopeId = int.Parse(dic["isotopeID"]);
                    outItem.isotopeSymbol = dic["isotopeSymbol"];
                    outItem.halflife = double.Parse(dic["halflife"]);
                    outItem.locationId = long.Parse(dic["locationID"]);
                    outItem.locationName = dic["locationName"];
                    outItem.weight = double.Parse(dic["weight"]);
                    outItem.refActivity = double.Parse(dic["refActivity"]);
                    outItem.refDate = DateTime.Parse(dic["refDate"]);
                    outItem.dateAdded = DateTime.Parse(dic["refDate"]);
                    outItem.disposed = dic["disposed"] == "1"?true:false;
                    DateTime testDateTime;
                    outItem.dateToBeRemoved = DateTime.TryParse(dic["dateToBeRemoved"], out testDateTime)?testDateTime:new DateTime(0);
                    outItem.dateDisposed = DateTime.TryParse(dic["dateDisposed"], out testDateTime) ? testDateTime : new DateTime(0);
                    outItem.tagNo = dic["tagNo"];
                    outItem.cpsToMBq = double.Parse(dic["cpsToMBq"]);
                    outItem.wasteCatName = dic["wasteGroupName"];
                    outItem.limitQuant = double.Parse(dic["limitQuant"]);
                    outItem.limitDays = int.Parse(dic["limitDays"]);
                    outItem.currentActivity = outItem.getCurrentActivity();
                    outItem.updateCloseToDateBool();
                    outItem.ActivtyPerGram =  outItem.getCurrentActivityPerGram();
                    output.Add(outItem);
                }
                return output;
            }catch (Exception ex)
            {
                MessageBox.Show(ex.Message);   
                return null;
            }
        }
        private List<DisposalRoute> ConvertToDisposalRoute(List<Dictionary<string, string>> map)
        {
            try
            {
                List<DisposalRoute> output = new List<DisposalRoute>();
                Dictionary<string,string> dic = new Dictionary<string,string>();
                for(int i = 0;i<map.Count;i++)
                {
                    dic = map[i];
                    DisposalRoute rt= new DisposalRoute();
                    rt.name = dic["name"];
                    rt.ID = int.Parse(dic["ID"]);
                    rt.inUse = dic["inUse"] =="1"? true:false;
                    output.Add(rt);
                }
                return output;
            }catch(Exception ex) {
                MessageBox.Show(ex.Message);
                return null;
            }
        }
        private List<user> ConvertToUserList(List<Dictionary<string, string>> map)
        {
            try
            {
                List<user> output = new List<user>();
                Dictionary<string,string> dic = new Dictionary<string,string>();
                for(int i =0; i < map.Count; i++)
                {
                    dic = map[i];
                    user ur = new user();
                    ur.name = dic["username"];
                    ur.email = dic["email"];
                    ur.id= int.Parse(dic["ID"]);
                    ur.active = dic["active"] == "1"?true:false;
                    ur.userLevel = long.Parse(dic["userLevel"]);
                    ur.emailAlert = dic["emailAlert"] == "1"?true:false;
                    ur.authCode = dic["authCode"];
                    ur.inUse = dic["inUse"]=="1"?true:false;
                    output.Add(ur);
                }
                return output;
            }
            catch (Exception ex) 
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }
        
        //------------------------------------------
        //wasteGroup member function calls 

        public async Task<List<WasteGroup>> GetWasteCatListAsync()
        {
            List<Dictionary<string, string>> result = await this.QueryDBAsync("query","getWasteCatList","");
            List<WasteGroup> output = this.convertToWasteList(result);
            return output;
        }

        public async Task<bool> UpdateWasteGroupAsync(WasteGroup waste)
        {
            string param = JsonSerializer.Serialize(waste);
            bool result = await AddUpdateTableAsync("update", "updateWasteGroup", param);
            return result;
        }

        public async Task<bool> AddWasteGroupAsync(WasteGroup waste)
        {
            string param = JsonSerializer.Serialize(waste);
            bool result = await AddUpdateTableAsync("update", "addWasteGroup", param);
            return result;
        }

        public async Task<int> getWasteLimitDaysByIsotopeAsync(long id)
        {
            List<Dictionary<string,string>> result = await this.QueryDBAsync("query", "getWasteLimitByIsotopeID", id.ToString());
            if(result.Count != 1)
            {
                MessageBox.Show("Error in returning the limit from DB");
                return 0;
            }
            else
            {
                int output;
                if (int.TryParse(result[0]["maxAccumulationDays"], out output)){
                    return output;
                }
                else
                {
                    MessageBox.Show("Error in returning the limit from DB");
                    return 0;
                }
            }
        }

        public async Task<double> getWasteLimitQuantByIsotopeAsync(long id)
        {
            List<Dictionary<string, string>> result = await this.QueryDBAsync("query", "getWasteLimitQuantByIsotope", id.ToString());
            if (result.Count != 1)
            {
                MessageBox.Show("Error in returning the limit from DB");
                return 0;
            }
            else
            {
                double output;
                if (double.TryParse(result[0]["activityLimit"], out output))
                {
                    return output;
                }
                else
                {
                    MessageBox.Show("Error in returning the limit from DB");
                    return 0;
                }
            }
        }

        public async Task<bool> removeWasteGroup(long id)
        {
            bool output = await this.AddUpdateTableAsync("update", "removeWasteGroup", id.ToString());
            return output;
        }

        //----------------------------------------------------------------
        //Location functions

        public async Task<List<Location>> GetLocationListAsync()
        {
            List<Dictionary<string, string>> result = await this.QueryDBAsync("query", "GetLocationList", "");
            List<Location> output = this.ConvertToLocationList(result);
            return output;
        }
        public async Task<bool> UpdateLocationAsync(Location loc)
        {
            string jsonLocation = JsonSerializer.Serialize(loc);
            bool result = await this.AddUpdateTableAsync("update", "UpdateLocationAsync", jsonLocation);
            return result;
        }
        public async Task<bool> AddLocationAsync(Location loc)
        {
            string jsonLocation = JsonSerializer.Serialize(loc);
            bool result = await this.AddUpdateTableAsync("update", "AddLocationAsync", jsonLocation);
            return result;
        }
        public async Task<bool> RemoveLocationFromUse(Location loc)
        {
            string jsonLocation = JsonSerializer.Serialize(loc);
            bool result = await this.AddUpdateTableAsync("update", "RemoveLocationFromUse", jsonLocation);
            return result;
        }

        //------------------------------------------------------------------
        //isotope calls
        public async Task<Isotope> GetIsotopeInfoByIdAsync(long id) 
        {
            List<Dictionary<string, string>> result = await this.QueryDBAsync("query", "GetIsotopeInfoById", id.ToString());
            List<Isotope> output = this.ConverToIsotopeList(result);
            if(output.Count != 1) 
            {
                MessageBox.Show("issue in getting isotope infor conact Admin");
                return null;
            }
            else
            {
                return output[0];
            }
        }
        public async Task<List<Isotope>> GetIsotopeListAsync()
        {
            List<Dictionary<string, string>> result = await this.QueryDBAsync("query", "GetIsotopeList", "");
            List<Isotope> output = this.ConverToIsotopeList(result);
            return output;
        }
        public async Task<bool> UpdateIsotopeAsync(Isotope isotope)
        {
            string param = JsonSerializer.Serialize(isotope);
            bool result = await AddUpdateTableAsync("update", "UpdateIsotope", param);
            return result;
        }
        public async Task<bool> AddIsotopeAsync(Isotope iso)
        {
            string param = JsonSerializer.Serialize(iso);
            bool result = await AddUpdateTableAsync("update", "AddIsotope", param);
            return result;
        }
        public async Task<bool> RemoveIsotopeFromUseAsync(Isotope iso)
        {
            string param = JsonSerializer.Serialize(iso);
            bool result = await AddUpdateTableAsync("update", "RemoveIsotopeFromUse", param);
            return result;
        }

        //---------------------------------------------------------------------------------------------
        // Summary page functions
        public async Task<List<currentActivityTotalByIsotope>> GetTotalActivityByIsotopeAsync()
        {
            List<Dictionary<string, string>> result = await this.QueryDBAsync("query", "GetTotalActivityByIsotopeAsync", "");
            if (result == null) { return null; }
            List<currentActivityTotalByIsotope> output = this.ConvertToTotalActivityByIsotope(result);
            return output;
        }
        public async Task<List<item>> GetItemsToBeRemovedAsync()
        {
            List<Dictionary<string, string>> result = await this.QueryDBAsync("query", "GetItemsToBeRemovedAsync", "");
                if (result == null) { return null;}
                List<item> output = this.ConvertToItemsList(result);
                return output;
        }
        
        //---------------------------------------------------------------------------------------------
        //items function calls 
        //
        // add an item to the DB id will be null.
        public async Task<bool> AddItemAsync(WasteItem itm)
        {
            if (itm != null)
            {
                string param = JsonSerializer.Serialize(itm);
                bool result = await AddUpdateTableAsync("update", "AddItemAsync", param);
                return result;
            }
            else
            {
                return false;
            }
            
        }
        public async Task<bool> DisposeItemAsync(WasteItem itm)
        {
            if (itm != null)
            {
                string param = JsonSerializer.Serialize(itm);
                bool result = await this.AddUpdateTableAsync("update", "DisposeItemAsync", param);
                return result;
            }
            else
            {
                return false;
            }
        }

        //----------------------------------------------------------------------------------------------
        //Disposal route calls

        //get list of disposal routes
        public async Task<List<DisposalRoute>> GetActiveDisposalRoutesAsync()
        {
            List<Dictionary<string, string>> result = await this.QueryDBAsync("query", "GetActiveDisposalRoutesAsync", "");
            List<DisposalRoute> output = this.ConvertToDisposalRoute(result);
            return output;
        }
        public async Task<bool> AddDisposalRoute(DisposalRoute route)
        {
            if (route == null) return false;
            string param = JsonSerializer.Serialize(route);
            bool result = await this.AddUpdateTableAsync("update", "AddDisposalRoute", param);
            return result;
        }
        public async Task<bool> UpdateDisposalRoute(DisposalRoute route)
        {
            if (route == null) return false;
            string param = JsonSerializer.Serialize(route);
            bool result = await this.AddUpdateTableAsync("update", "UpdateDisposalRoute", param);
            return result;
        }
        public async Task<bool> RemoveDisposalRoute(DisposalRoute route)
        {
            if(route == null ) return false;
            string param = JsonSerializer.Serialize(route);
            bool result = await this.AddUpdateTableAsync("update", "RemoveDisposalRoute", param);
            return result;
        }

        //----------------------------------------------------------------------------------------------
        //User functions

        //get list of current users
        public async Task<List<user>> GetUsersAsync()
        {
            List<Dictionary<string, string>> result = await this.QueryDBAsync("query", "GetUsersAsync", "");
            List<user> output = this.ConvertToUserList(result);
            return output;
        }
        //set a new password for a user
        public async Task<bool> SetNewPasswordAsync(user us, string password)
        {
            if (us == null) { return false; }
            Dictionary<string,string> dic = new Dictionary<string, string>() { { "ID", us.id.ToString() },{ "password", password } };
            string param = JsonSerializer.Serialize(dic);   
            bool result = await this.AddUpdateTableAsync("update", "SetNewPasswordAsync", param);
            return result;
        }
        // add new user with new password sent seperate
        public async Task<bool> AddNewUserAsync(user us, string NewPassword)
        {
            if (us == null) { return false; }
            string userParam = JsonSerializer.Serialize(us);
            Dictionary<string, string> dic = new Dictionary<string, string>() { { "user", userParam }, { "password", NewPassword } };
            string param = JsonSerializer.Serialize(dic);
            bool result = await this.AddUpdateTableAsync("update", "AddNewUserAsync", param);
            return result;
        }
        public async Task<bool> UpdateUserInfoAsync(user us)
        {
            if (us == null) { return false; }
            string param = JsonSerializer.Serialize(us);
            bool result = await this.AddUpdateTableAsync("update","UpdateUserInfoAsync",param);
            return result;
        }

        //------------------------------------------------------------------------------------------------
        // Report functions
        //------------------------------------------------------------------------------------------------
        //get all items in DB to generate reports
        public async Task<List<item>> GetAllItemsInDB()
        {
            List<Dictionary<string, string>> result = await this.QueryDBAsync("query", "GetAllItemsInDB", "");
            List<item> items =this.ConvertToItemsList(result);
            return items;
        }


        
    }
}
