using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SQLite;
using SQLitePCL;
using System.IO;
using System.Collections;
using System.ComponentModel;
using Tizen.Wearable.CircularUI.Forms;
using Tizen.NUI;

namespace TizenWearableApp1
{
    public class Item
    {
        [PrimaryKey, AutoIncrement]
        public int id{ get; set; }
        public string date { get; set; }
        public string time { get; set; }
        public string tagDesc { get; set; }
        public string amount { get; set; }
        public string desc { get; set; }
    }
    class DataInteractionV2
    {
        static public string databaseFileName = "SQLite3.db3";
        static public SQLiteConnection dbConnection;
        static public string databasePath;
        public DataInteractionV2()
        {
            bool needCreateTable = false;
            // Need to initialize SQLite
            SQLitePCL.raw.SetProvider(new SQLite3Provider_sqlite3());
            SQLitePCL.raw.FreezeProvider(true);
            // Get writable directory info for this app.
            string dataPath = global::Tizen.Applications.Application.Current.DirectoryInfo.Data;
            // Combine with database path and name
            databasePath = System.IO.Path.Combine(dataPath, databaseFileName);

            // Check the database file to decide table creation.
            if (!File.Exists(databasePath))
            {
                needCreateTable = true;
            }

            dbConnection = new SQLiteConnection(databasePath);
            if (needCreateTable)
            {
                dbConnection.CreateTable<Item>();
            }
        }
        public bool writeRec(String pDesc, String tagPic, double pAmount)
        {
            String write;
            if (pAmount > 0)
                write = "+" + pAmount.ToString();
            else
                write = pAmount.ToString();
            Item insert = new Item
            {
                date = DateTime.Now.ToString("yyyy-MM-dd"),
                time = DateTime.Now.ToShortTimeString().ToString(),
                tagDesc = tagPic,
                amount = write,
                desc = pDesc
            };
            int res = dbConnection.Insert(insert);
            return true;
        }
        public ArrayList readRec(String date)
        {
            ArrayList tag = new ArrayList();
            ArrayList details = new ArrayList();
            ArrayList descArray = new ArrayList();
            ArrayList id = new ArrayList();
            ArrayList ret = new ArrayList();
            TagProvider tp = new TagProvider();
            var li = dbConnection.Query<Item>("select * from Item where date = '" + date+"'");
            if (li.Count != 0)
            {
                foreach (Item i in li)
                {
                    tag.Add(tp.getContentByID(i.tagDesc));
                    details.Add(i.time+" | "+i.amount);
                    descArray.Add(i.desc);
                    id.Add(i.id);
                }
                ret.Add(tag);
                ret.Add(details);
                ret.Add(descArray);
                ret.Add(id);
            }
            return ret;
        }
        public bool deleteRec(int id)
        {
            string sql = "delete from Item where id = " +id;
            if (dbConnection.Execute(sql) != 1)
                return false;
            else
                return true;
        }
        public double readTodaySum(String date)
        {
            var li = dbConnection.Query<Item>("select amount from Item where date = '" + date + "'");
            double ret = 0;
            if (li.Count != 0)
            {
                foreach (Item i in li)
                {
                    if (i.amount.Contains("+"))
                        ret += Convert.ToDouble(i.amount.Substring(1));
                    else
                        ret += Convert.ToDouble(i.amount);
                }
            }
            return ret;
        }
        public double readWeeklySum(int year, int weekNum)
        {
            DateTime Jan1 = new DateTime(year, 1, 1);
            DateTime firstWeekDay = Jan1.AddDays(7 - (int)Jan1.DayOfWeek + 1);
            DateTime weekStart = firstWeekDay.AddDays((weekNum - 1) * 7);
            //Toast.DisplayText(weekStart.ToString("yyyy-MM-dd"));
            double ret = 0;
            for (int i=0;i<7;i++)
            {
                string qDate = weekStart.ToString("yyyy-MM-dd");
                //Toast.DisplayText(qDate);
                var qRes = dbConnection.Query<Item>("select * from Item where date = ?", qDate);
                if(qRes.Count != 0)
                {
                    foreach (Item it in qRes)
                    {
                        if (it.amount.Contains("+"))
                            ret += Convert.ToDouble(it.amount.Substring(1));
                        else
                            ret += Convert.ToDouble(it.amount);
                    }
                }
                weekStart = weekStart.AddDays(1);
                //Toast.DisplayText(qRes.Count.ToString());
            }
            return ret;
        }
        //public double readWeeklyOutSum(int year, int weekNum)
        //public double readWeeklyInSum(int year, int weekNum)
        //public Dictionary<string, double> readWeeklyOutTagAmount()
        //public Dictionary<string, double> readWeeklyInTagAmount()
        //public Dictionary<string, double> readWeeklyOutPercentage()
        //public Dictionary<string, double> readWeeklyInPercentage()
        //public ArrayList sort(Dictionary<string, double> in)
        public int showCurrWeekNum()
        {
            DateTime Jan1 = new DateTime(DateTime.Now.Year, 1, 1);
            int currDayInYear = DateTime.Now.DayOfYear;
            int res = (currDayInYear-(7 - (int)Jan1.DayOfWeek + 1)) / 7 + 1;
            return res;
        }
        public Dictionary<string, double> readWeeklyTagAmount(int year, int weekNum)
        {
            TagProvider tp = new TagProvider();
            Dictionary<string, string> tags = tp.getAllTag();
            Dictionary<string, double> ret = new Dictionary<string, double>();
            //initialize the return dict
            foreach(var item in tags)
            {
                ret.Add(item.Value, 0);
            }
            //calculate by date
            DateTime Jan1 = new DateTime(year, 1, 1);
            DateTime firstWeekDay = Jan1.AddDays(7 - (int)Jan1.DayOfWeek + 1);
            DateTime weekStart = firstWeekDay.AddDays((weekNum - 1) * 7);
            //Toast.DisplayText(weekStart.ToString("yyyy-MM-dd"));
            for (int i = 0; i < 7; i++)
            {
                //DateTime weekStart = weekStartX;
                string qDate = weekStart.ToString("yyyy-MM-dd");
                var qRes = dbConnection.Query<Item>("select * from Item where date = '" + qDate + "'");
                if (qRes.Count != 0)
                {
                    foreach (Item it in qRes)
                    {
                        double adder;
                        if (it.amount.Contains("+"))
                            adder = Convert.ToDouble(it.amount.Substring(1));
                        else
                            adder = Convert.ToDouble(it.amount);
                        //Toast.DisplayText(adder.ToString());
                        ret[tags[it.tagDesc]] += adder;
                        //Toast.DisplayText(ret[tags[it.tagDesc]].ToString());
                    }
                }
                weekStart = weekStart.AddDays(1);
            }
            //Toast.DisplayText(ret["电子产品"].ToString());
            return ret;
        }
        public KeyValuePair<string, double>[] readWeeklyTagPercentage(int year, int weekNum)
        {
            Dictionary<string, double> val = readWeeklyTagAmount(year, weekNum);
            Dictionary<string, double> ret = new Dictionary<string, double>();
            double weekSum = readWeeklySum(year, weekNum);
            foreach(var item in val)
            {
                ret.Add(item.Key, item.Value / weekSum);
            }
            int i = 0;
            KeyValuePair<string, double>[] sortedRet = new KeyValuePair<string, double>[ret.Count];
            var dicSort = from objDic in ret orderby ret.Values descending select objDic;
            //int j = sortedRet.Length;
            foreach(KeyValuePair<string, double> kvp in dicSort)
            {
                sortedRet[i] = kvp;
                i++;
            }
            return sortedRet;
        }

        public string printDict(Dictionary<string, double> inx)
        {
            string ret = "";
            foreach(KeyValuePair<string, double> kvp in inx)
            {
                ret += kvp.Key + ": " + kvp.Value.ToString() + " ; ";
            }
            return ret;
        }
    }
}
