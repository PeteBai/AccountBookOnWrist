using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TizenWearableApp1
{
    class TagProvider
    {
        private Dictionary<string, string> tagDict = new Dictionary<string, string>();
        public TagProvider()
        {
            tagDict.Add("nothing", "   ");
            tagDict.Add("Devices2.png", "电子产品");
            tagDict.Add("CashDrawer.png", "工资收入");
            tagDict.Add("food.png", "食品");
            tagDict.Add("Broom.png", "日用品");
            tagDict.Add("Education.png", "教育");
            tagDict.Add("Health.png", "医疗");
            tagDict.Add("Airplane.png", "旅行");
            tagDict.Add("Bus.png", "出行");
            tagDict.Add("Home.png", "贷款与租赁");
            tagDict.Add("Tiles.png", "其他");
            tagDict.Add("nothing2", "  ");
        }
        public Dictionary<string, string> getAllTag()
        {
            return tagDict;
        }

        public string getContentByID(string picName)
        {
            return tagDict[picName];
        }
        public int getNum()
        {
            return tagDict.Count;
        }
    }
}
