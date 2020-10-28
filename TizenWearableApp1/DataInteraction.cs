using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections;
using System.Security.Cryptography;
using Tizen.Wearable.CircularUI.Forms;
using Tizen.Security;

namespace TizenWearableApp1
{
    class DataInteraction
    {
        public bool writeRec(String desc, String tagPic, double amount)
        {
            String write;
            if (amount > 0)
                write = "+" + amount.ToString();
            else
                write = amount.ToString();
            String curr = DateTime.Now.ToString("yyyy-MM-dd");
            String x = "";//Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            String currTime = DateTime.Now.ToLongTimeString().ToString();
            String[] ap = new String[4];
            ap[0] = currTime;
            ap[1] = tagPic;
            ap[2] = desc;
            ap[3] = write;
            try
            {
                String priv = "http://tizen.org/privilege/externalstorage.appdata";
                CheckResult cr = PrivacyPrivilegeManager.CheckPermission(priv);
                Toast.DisplayText(Environment.CurrentDirectory);
                if (cr == CheckResult.Deny)
                    PrivacyPrivilegeManager.RequestPermission(priv);
                File.AppendAllLines(x + curr + ".txt", ap);
            }
            catch(Exception e)
            {
                Toast.DisplayText(e.Message);
                return false;
            }  
            return true;
        }
        public ArrayList readRec(String date)
        {
            
            String x = "";//Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            //处理文件不存在的异常
            ArrayList tag = new ArrayList();
            ArrayList al = new ArrayList();
            ArrayList descArray = new ArrayList();
            if (!File.Exists(x + date + ".txt"))
            {
                return al;
            }
            StreamReader s = new StreamReader(x + date + ".txt", Encoding.UTF8);
            string nextLine, time = "", desc = "", tagPic = "";
            int i = 0;
            TagProvider tp = new TagProvider();
            while ((nextLine = s.ReadLine()) != null)
            {
                if(i%4 == 0)//time
                {
                    time = nextLine;
                    i++;
                }
                else if(i%4 == 1)//tag
                {
                    tagPic = tp.getContentByID(nextLine);
                    i++;
                }
                else if(i % 4 == 2)//desc
                {
                    desc = nextLine;
                    i++;
                }
                else
                {
                    tag.Add(tagPic);
                    al.Add(time + " | " + nextLine);
                    descArray.Add(desc);
                    i = 0;
                }
            }
            s.Close();
            ArrayList ret = new ArrayList();
            ret.Add(tag);
            ret.Add(al);
            ret.Add(descArray);
            return ret;
        }
        public bool deleteRec(String date, String time)
        {
            String x = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            //check if file exists
            if (!File.Exists(x + date + ".txt"))
            {
                Tizen.Wearable.CircularUI.Forms.Toast.DisplayText("Not Found");
                return false;
            }

            int i = 0, j;// make sure it is a proper time record
            string[] context = File.ReadAllLines(@x + date + ".txt", System.Text.Encoding.UTF8);//获取文本中每一行数据存在数组中
            for(i=0;i<context.Length;i++)
            {
                if(i%4==0 && String.Equals(context[i], time))
                {
                    break;
                }
            }
            if(i == context.Length)
            {
                Tizen.Wearable.CircularUI.Forms.Toast.DisplayIconText("Record Not Found", "cancel.png");
                return false;
            }
            for(j = i; j < context.Length; j++)
            {
                if (j + 4 == context.Length)
                    break;
                context[j] = context[j + 4];
            }
            string[] newStr = context.Take(context.Length-4).ToArray();
            File.WriteAllLines(x + date + ".txt", newStr);
            return true;
        }
    }
}
