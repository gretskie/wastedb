using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Xml.Schema;
using System.Drawing;
using Microsoft.SqlServer.Server;
using System.ComponentModel;
using WasteApp.APICLasses;

namespace WasteApp.classes
{
    class dbConnection
    {
        private string _conStr;
        
        public dbConnection()
        {
            _conStr = ConfigurationManager.ConnectionStrings["wasteConnection"].ConnectionString;
        }

        //internally used functions
        private DataTable queryDB(string SQLQuery)
        {
            try
            {
                using(SqlConnection con = new SqlConnection(_conStr)) 
                {
                    con.Open();
                    using(SqlDataAdapter a = new SqlDataAdapter(SQLQuery, con))
                    {
                        DataTable t = new DataTable();
                        a.Fill(t);
                        con.Close();
                        return t;
                    }
                }
            }catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }

        private bool addUpdateTable(string SQL)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(_conStr))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(SQL, con))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
                return true;
            }catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        private List<wasteCat> convertToWasteList(DataTable t)
        {
            List<wasteCat> output = new List<wasteCat>();
            for(int i = 0; i < t.Rows.Count;i++)
            {
                wasteCat waste = new wasteCat();
                waste.id = (long)t.Rows[i]["id"];
                waste.name = (string)t.Rows[i]["name"];
                waste.limitQuant = (double)t.Rows[i]["limitQuant"];
                waste.limitDays = (int)t.Rows[i]["limitDays"];
                output.Add(waste);
            }
            return output;
        }

        private List<Location> convertToLocationList(DataTable t)
        {
            List<Location> output = new List<Location>();
            for(int i =0;i<t.Rows.Count; i++)
            {
                Location loc = new Location();
                loc.ID = int.Parse(t.Rows[i]["ID"].ToString());
                loc.Name = t.Rows[i]["Name"].ToString();
                output.Add(loc);
            }
            return output;
        }

        //wasteCat member functions calls 

        public List<wasteCat> getWasteCatList() {
                string SQL = "SELECT id, name, limitQuant, limitDays FROM wasteCatagory WHERE inUSe = 1";
                return this.convertToWasteList(this.queryDB(SQL));
        }

        public bool updateWasteCat(string wasteCatName, string wasteCatLimitQuant, string wasteCatLimitDays, string id)
        {
                string SQL = "UPDATE wasteCatagory SET name='" + wasteCatName + "', limitQuant=" + wasteCatLimitQuant + ", limitDays=" + wasteCatLimitDays + " WHERE id =" + id;
                return this.addUpdateTable(SQL);
        }

        public bool addWasteCat(string wasteCatName, string wasteCatLimitQuant, string wasteCatLimitDays)
        {
                string SQL = "INSERT INTO wasteCatagory (name, limitQuant, limitDays) VALUES ('" + wasteCatName + "'," + wasteCatLimitQuant + "," + wasteCatLimitDays + ")";
                return this.addUpdateTable(SQL);
        }

        public int getWasteLimitDaysByIsotope(string isotopeId)
        {
            string SQL = "SELECT wasteCatagory.limitDays FROM isotopes INNER JOIN wasteCatagory ON isotopes.wasteCatagory = wasteCatagory.id WHERE isotopes.id = " + isotopeId;
            DataTable t = this.queryDB(SQL);
            if(t.Rows.Count == 1)
            {
                int output = (int)t.Rows[0]["limitDays"];
                return output;
            }
            else
            {
                throw new Exception("Error loading WasteCat from DB in getWasteLimitDaysByIsotope, dbconnection object");
            }
        }
        public double getWasteLimitQuantByIsotope(string isotopeId)
        {
            string SQL = "SELECT wasteCatagory.limitQuant FROM isotopes INNER JOIN wasteCatagory ON isotopes.wasteCatagory = wasteCatagory.id WHERE isotopes.id = " + isotopeId;
            DataTable t = this.queryDB(SQL);
            if (t.Rows.Count == 1)
            {
                double output = (double)t.Rows[0]["limitDays"];
                return output;
            }
            else
            {
                throw new Exception("Error loading WasteCat from DB in getWasteLimitDaysByIsotope, dbconnection object");
            }
        }

        public bool removeWasteCat(string id)
        {
                string SQL = "UPDATE wasteCatagory SET inUse=0 WHERE id =" + id;
                return this.addUpdateTable(SQL);
        }

        //location member functions

        public List<Location> getLocationsListBox()
        {
                string SQL = "SELECT id, name FROM location WHERE inUse =1";
                return this.convertToLocationList(this.queryDB(SQL));

        }

        public bool updateLocation(string name, string id)
        {
                string SQL = "UPDATE location SET name='" + name + "' WHERE id =" + id;
                return this.addUpdateTable(SQL);
        }

        public bool addLocation(string name)
        {
                string SQL = "INSERT INTO location (name) VALUES ('" + name + "')";
                return this.addUpdateTable(SQL);
        }

        public bool removeLocation(string id)
        {
                string SQL = "UPDATE location SET inUse=0 WHERE id =" + id;
                return this.addUpdateTable(SQL);
        }

        // isotopes member functions

        public isotope getAllIsotopeInfo(string id)
        {
                string SQL ="SELECT * from isotopes WHERE id =" + id;
                DataTable t =this.queryDB(SQL);
            try
            {
                DataRow r = t.Rows[0];
                isotope iso = new isotope(int.Parse(id), (string)r["symbol"], (int)r["atomicMass"], (double)r["halflifeHours"], (long)r["wasteCatagory"], (double)r["cpsToMBq"]);
                return iso;
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message);
                throw new Exception(e.Message);
            }
        }

        public List<isotope> getIsotopeList()
        {
            string SQL = "SELECT * FROM isotopes WHERE inUse =1";
            List<isotope> output = new List<isotope>();
            DataTable t = this.queryDB(SQL);
            for (int i = 0; i < t.Rows.Count; i++) 
            {
                DataRow r = t.Rows[i];
                isotope iso = new isotope((int)r["id"], (string)r["symbol"], (int)r["atomicMass"], (double)r["halflifeHours"], (long)r["wasteCatagory"], (double)r["cpsToMBq"]);
                output.Add(iso);
            }

            return output;
        }

        public bool updateIsotope(string symbol, string cpstombq, string halflife, string atomicmass, string wastecat, string id)
        {
            string SQL = "UPDATE isotopes SET symbol='" + symbol + "',atomicMass=" + atomicmass +
                ",halflifeHours=" + halflife + ", wasteCatagory=" + wastecat +
                ", cpsToMBq=" + cpstombq + " WHERE id=" + id;          
                return this.addUpdateTable(SQL);
        }

        public  bool addIsotope(string symbol, string cpstombq, string halflife, string atomicmass, string wastecat)
        {
            string SQL = "INSERT INTO isotopes (symbol, atomicMass, halflifeHours, wasteCatagory, cpsTOMBq) VALUES ('"+
                symbol+"',"+atomicmass+","+halflife+","+wastecat+","+cpstombq+")";
            return this.addUpdateTable(SQL);
        }

        public bool removeIsotope(string id)
        {
            string SQL = "UPDATE isotopes SET inUse=0 WHERE id=" + id;
            return this.addUpdateTable(SQL);
        }

        // items member function conncetions
        public List<item> getCurrentItems()
        {
                string SQL = "SELECT items.id, items.activityOnRef, items.referenceDateTime as refDate, items.dateAdded as dateAdded, items.weightKg as weight, items.dateToBeRemoved, location.id as locationId, location.name as location, isotopes.halflifeHours as halflife, isotopes.cpsToMBq as cpsToMBq, isotopes.id as isotopeId, isotopes.symbol as isotope, wasteCatagory.name as wasteCat, wasteCatagory.limitQuant as wasteLimit, wasteCatagory.limitDays as timeLimit, items.tagNo as tagNo FROM items INNER JOIN location ON location.id = items.location INNER JOIN isotopes ON isotopes.id = items.isotope LEFT JOIN wasteCatagory ON wasteCatagory.id = isotopes.wasteCatagory WHERE items.disposed = 0 ORDER BY items.dateToBeRemoved";
            try
            {
                DataTable t = this.queryDB(SQL);
                List<item> output = new List<item>();
            
                for(int i = 0; i < t.Rows.Count; i++)
                {
                    DataRow r = t.Rows[i];
                    item it = new item();
                    it.id = (long)r["id"];
                    it.refActivity = (double)r["activityOnRef"];
                    it.refDate = DateTime.Parse(r["refDate"].ToString());
                    it.dateAdded = DateTime.Parse(r["dateAdded"].ToString());
                    it.weight = (double)r["weight"];
                    it.dateToBeRemoved = DateTime.Parse(r["dateToBeRemoved"].ToString());
                    it.locationId = (long)r["locationId"];
                    it.locationName = r["location"].ToString();
                    it.halflife = (double)r["halflife"];
                    it.cpsToMBq = (double)r["cpsToMBq"];
                    it.isotopeId = (int)r["isotopeId"];
                    it.isotopeSymbol = r["isotope"].ToString();
                    it.wasteCatName = r["wasteCat"].ToString();
                    it.limitQuant = (double)r["wasteLimit"];
                    it.limitDays = (int)r["timeLimit"];
                    it.tagNo = r["tagNo"].ToString();
                    it.currentActivity = it.getCurrentActivity();
                    it.ActivtyPerGram = it.currentActivity/ (it.weight*1000);
                    output.Add(it);
                }
                return output;

            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message);
                throw e;
            }


            
        }

        public BindingList<item> getCurrentItemsBL()
        {
            string SQL = "SELECT items.id, items.activityOnRef, items.referenceDateTime as refDate, items.dateAdded as dateAdded, items.weightKg as weight, items.dateToBeRemoved, location.id as locationId, location.name as location, isotopes.halflifeHours as halflife, isotopes.cpsToMBq as cpsToMBq, isotopes.id as isotopeId, isotopes.symbol as isotope, wasteCatagory.name as wasteCat, wasteCatagory.limitQuant as wasteLimit, wasteCatagory.limitDays as timeLimit, items.tagNo as tagNo FROM items INNER JOIN location ON location.id = items.location INNER JOIN isotopes ON isotopes.id = items.isotope LEFT JOIN wasteCatagory ON wasteCatagory.id = isotopes.wasteCatagory WHERE items.disposed = 0 ORDER BY items.dateToBeRemoved";
            try
            {
                DataTable t = this.queryDB(SQL);
                BindingList<item> output = new BindingList<item>();

                for (int i = 0; i < t.Rows.Count; i++)
                {
                    DataRow r = t.Rows[i];
                    item it = new item();
                    it.id = (long)r["id"];
                    it.refActivity = (double)r["activityOnRef"];
                    it.refDate = DateTime.Parse(r["refDate"].ToString());
                    it.dateAdded = DateTime.Parse(r["dateAdded"].ToString());
                    it.weight = (double)r["weight"];
                    it.dateToBeRemoved = DateTime.Parse(r["dateToBeRemoved"].ToString());
                    it.locationId = (long)r["locationId"];
                    it.locationName = r["location"].ToString();
                    it.halflife = (double)r["halflife"];
                    it.cpsToMBq = (double)r["cpsToMBq"];
                    it.isotopeId = (int)r["isotopeId"];
                    it.isotopeSymbol = r["isotope"].ToString();
                    it.wasteCatName = r["wasteCat"].ToString();
                    it.limitQuant = (double)r["wasteLimit"];
                    it.limitDays = (int)r["timeLimit"];
                    it.tagNo = r["tagNo"].ToString();
                    it.currentActivity = it.getCurrentActivity();
                    output.Add(it);
                }
                return output;

            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                throw e;
            }
        }

        public bool addItem(string refDateTime, string isotopeId, string tagNo, string activity, string weight, string locaitonID)
        {
            DateTime dateTimeNow = DateTime.Now;
            DateTime datetimeRefDate = new DateTime();
            datetimeRefDate = DateTime.Parse(refDateTime);
            refDateTime = datetimeRefDate.ToString("yyyy-MM-dd HH:mm:ss");

            string dateToBeRemoved;
            string dateToBeAdded;
            string userName = "-1";
           //isotope iso = new isotope(int.Parse(isotopeId));


            dateToBeAdded = dateTimeNow.ToString("yyyy-MM-dd HH:mm:ss");
            TimeSpan offset = new TimeSpan(this.getWasteLimitDaysByIsotope(isotopeId), 0, 0,0);

            DateTime dateTimeToBeRemoved = new DateTime();
            dateTimeToBeRemoved = DateTime.Parse(refDateTime) + offset;
            dateToBeRemoved =  dateTimeToBeRemoved.ToString("yyyy-MM-dd HH:mm:ss");

            string SQL = "INSERT INTO items (isotope, location," +
                " weightKg, activityOnRef, referenceDateTime," +
                " dateAdded, dateToBeRemoved, userAddingItem,tagNo)" +
                "VALUES (" + isotopeId + ", " + locaitonID +", " + weight + ", "+
                activity + ", CONVERT(datetime,'" + refDateTime +"'), CONVERT(datetime, '" + dateToBeAdded + "'),CONVERT(datetime, '"+ dateToBeRemoved +"'), " + userName + ", '" + tagNo + "')";
            
            return this.addUpdateTable(SQL);
        }
        public bool changeItemLocation(string itemId, string locationId)
        {
            string SQL = " UPDATE items SET location=" + locationId + " WHERE id=" + itemId;
            return this.addUpdateTable(SQL);
        }




        // units member finctions
        public List<unit> getUnits()
        {
            string SQL = "SELECT * FROM units";
            List<unit> list = new List<unit>();
            DataTable t = this.queryDB(SQL);
            for(int i = 0; i< t.Rows.Count; i++)
            {
                DataRow r = t.Rows[i];
                unit un = new unit();
                un.id = (long)r["id"];
                un.symbol = r["symbol"].ToString();
                un.conversionToMBq = (double)r["conversionToMBq"];
                list.Add(un);
            }
            return list;
            
        }

        public long getUnitConvertion(string id)
        {
            string SQL = "SELECT * FROM units WHERE id=" + id;
            DataTable t = this.queryDB(SQL);
            if(t.Rows.Count == 1)
            {
                DataRow r = t.Rows[0];
                return (long)r["conversionToMBq"];
            }
            else
            {
                throw new Exception("id " + id + " does not return a correct response from units table.");
            }
        }


        //summary functions
        public List<currentActivityTotalByIsotope> getTotalActivityByIsotope()
        {
            string SQL = "SELECT SUM(items.activityOnRef*EXP(-(LOG(2)/isotopes.halflifeHours)*DATEDIFF(hour,items.referenceDateTime,GETDATE()))) AS totalActivity, isotopes.symbol, wasteCatagory.limitQuant, (SUM(items.activityOnRef*EXP(-(LOG(2)/isotopes.halflifeHours)*DATEDIFF(hour,items.referenceDateTime,GETDATE())))/wasteCatagory.limitQuant) AS limitPercentage FROM items INNER JOIN isotopes ON isotopes.id = items.isotope LEFT JOIN wasteCatagory ON wasteCatagory.id = isotopes.wasteCatagory WHERE items.disposed = 0 GROUP BY isotopes.symbol, wasteCatagory.limitQuant";
            DataTable t = this.queryDB(SQL);
            List<currentActivityTotalByIsotope> output = new List<currentActivityTotalByIsotope>();
            for(int i = 0; i< t.Rows.Count; i++)
            {
                DataRow r = t.Rows[i];
                currentActivityTotalByIsotope iso = new currentActivityTotalByIsotope();
                iso.totalActivity = (double)r["totalActivity"];
                iso.limitQuant = (double)r["limitQuant"];
                iso.isotopeSymbol = r["symbol"].ToString();
                iso.percentOfLimit = (double)r["limitPercentage"];
                output.Add(iso);
            }
            return output;
        }

        public List<item> getItemsForDisposal()
        {
            string SQL = "SELECT isotopes.symbol, items.activityOnRef*EXP(-(LOG(2)/isotopes.halflifeHours)*DATEDIFF(hour,items.referenceDateTime,GETDATE())) AS currentActivity, "+
                "items.tagNo, items.dateToBeRemoved FROM items "+
                "INNER JOIN isotopes ON isotopes.id = items.isotope "+
                "WHERE items.disposed = 0 ORDER BY items.dateToBeRemoved";
            DataTable t = this.queryDB(SQL);
            List<item> output = new List<item>();
            for(int i = 0; i < t.Rows.Count; i++)
            {
                DataRow r = t.Rows[i];
                item it= new item();
                it.isotopeSymbol = r["symbol"].ToString();
                it.currentActivity = (double)r["currentActivity"];
                it.tagNo = r["tagNo"].ToString();
                it.dateToBeRemoved = DateTime.Parse(r["dateToBeRemoved"].ToString());
                output.Add(it);
            }
            return output;
        }

    }
}
