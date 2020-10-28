using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tizen.Wearable.CircularUI.Forms;
using Xamarin.Forms;
using System.Collections;
using Xamarin.Forms.Platform.Tizen;
using Tizen.NUI.BaseComponents;
using System.Reflection.Metadata;
using Tizen.Content.MediaContent;
using Tizen.Pims.Calendar.CalendarViews;
using System.Xml.Linq;
using System.IO;

namespace TizenWearableApp1
{
    class FeatureSelection : CirclePage
    {
        public FeatureSelection()
        {
            Button addBtn = new Button() { Text = "添加账目" };
            Button viewBtn = new Button() { Text = "查看记录" };
            Button debugBtn = new Button() { Text = "DebugSelectTag" };
            debugBtn.IsVisible = false;
            AddClicked += new EventHandler(Add);
            ViewClicked += new EventHandler(View);
            debugClicked += new EventHandler(debugFunc);
            addBtn.Clicked += AddClicked;
            viewBtn.Clicked += ViewClicked;
            debugBtn.Clicked += debugClicked;
            Content = new StackLayout()
            {
                HorizontalOptions = LayoutOptions.Center,
                //VerticalOptions = LayoutOptions.Center,
                Orientation = StackOrientation.Vertical,
                Children =
                {
                    new Label() {HorizontalTextAlignment = TextAlignment.Center, Text = "欢迎!"},
                    addBtn, viewBtn, debugBtn
                },
            };
        }
        private event EventHandler AddClicked;
        private event EventHandler ViewClicked;
        private event EventHandler debugClicked;
        private void Add(Object sender, EventArgs e)
        {
            AddRecord addRecord = new AddRecord();
            Navigation.PushModalAsync(addRecord);
        }

        private void View(Object sender, EventArgs e)
        {
            //String today = DateTime.Now.ToString("yyyy-MM-dd");
            DateTime dt = DateTime.Now;
            RecordList recordList = new RecordList(dt);
            Navigation.PushModalAsync(recordList);
        }
        private void debugFunc(Object sender, EventArgs e)
        {
            //wait for test
            //DataInteraction di = new DataInteraction();
            //di.deleteRec("2020-06-05", "18:29:57");
            //deleteDB();
            //getWeek();
            cpsi();
        }
        private void deleteDB()
        {
            string dataPath = global::Tizen.Applications.Application.Current.DirectoryInfo.Data;
            string DBFileName = "SQLite3.db3";
            string databasePath = Path.Combine(dataPath, DBFileName);
            if(File.Exists(databasePath))
            {
                try { File.Delete(databasePath); }
                catch { Toast.DisplayText("DB_DELETE_ERR"); }
            }
        }
        private void getWeek()
        {
            DataInteractionV2 div2 = new DataInteractionV2();
            //Toast.DisplayText(div2.readWeeklySum(2020, 24).ToString());
            //Toast.DisplayText(div2.showCurrWeekNum().ToString());
            div2.readWeeklyTagPercentage(2020, 24);
            //CircleScrollView csv = new CircleScrollView();
            //csv.Content = div2.readWeeklyTagPercentage(2020, 25);
        }
        private void cpsi()
        {
            Navigation.PushModalAsync(new DataVisualization());
        }

    }
}
