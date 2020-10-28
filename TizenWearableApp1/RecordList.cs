using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tizen.Wearable.CircularUI.Forms;
using Xamarin.Forms;

namespace TizenWearableApp1
{
    class TagDescBind
    {
        public string tagDesc { get; set; }
        public string detailDesc { get; set; }
        public string descDesc { get; set; }
        public int id { get; set; }
    }
    class RecordList : CirclePage
    {
        int deleteId;
        DateTime cCurrTime;
        Button next = new Button() { ImageSource = new FileImageSource() { File = "next.png" }, BackgroundColor = Color.Transparent };
        Button prev = new Button() { ImageSource = new FileImageSource() { File = "Back.png" }, BackgroundColor = Color.Transparent };
        Button goHome = new Button() { Text = "主页", BackgroundColor = Color.Transparent };
        public RecordList(DateTime pCurrTime)
        {
            String date = pCurrTime.ToString("yyyy-MM-dd");
            cCurrTime = pCurrTime;
            DataInteractionV2 di = new DataInteractionV2();
            ArrayList show = di.readRec(date);
            PrevBtnClicked += prevClickedFunc;
            NextBtnClicked += nextClickedFunc;
            prev.Clicked += PrevBtnClicked;
            next.Clicked += NextBtnClicked;
            homeBtnClicked += homeClickedFunc;
            goHome.Clicked += homeBtnClicked;
            //尚无记录的page
            //CirclePage noRec = new CirclePage();
            //StackLayout noRecLayout = new StackLayout() 
            //{
            //    Orientation = StackOrientation.Vertical,
            //    VerticalOptions = LayoutOptions.Center,
            //    Children =
            //    {
            //        prev,
            //        new Label(){ Text = pCurrTime.ToString("yyyy-MM-dd").Substring(5), HorizontalTextAlignment = TextAlignment.Center, FontSize=8},
            //        new Image(){ Source = "Empty.png", Scale = 0.5, HeightRequest=100  },
            //        new Label(){ Text = "今日尚无记录哦", HorizontalTextAlignment = TextAlignment.Center, FontSize = 5},
            //        goHome,
            //        next,
            //    }
            //};
            //noRec.Content = noRecLayout;
            //if (show.Count == 0 || ((ArrayList)show[0]).Count == 0)
            //{
            //    Navigation.PushModalAsync(noRec);
            //    return;
            //}
                //show.Add();
            DataTemplate dt = new DataTemplate(() =>
            {
                var tagLable = new Label() { FontSize = 12, HorizontalTextAlignment = TextAlignment.Center };
                var detailLable = new Label() { FontSize = 9, HorizontalTextAlignment= TextAlignment.Center };
                tagLable.SetBinding(Label.TextProperty, "tagDesc");
                detailLable.SetBinding(Label.TextProperty, "detailDesc");
                StackLayout sl = new StackLayout()
                {
                    Orientation = StackOrientation.Vertical,
                    VerticalOptions = LayoutOptions.Center,
                    Children = { tagLable, detailLable },
                };
                return new ViewCell { View = sl };
            });
            DataTemplate headerTemplate = new DataTemplate(() =>
            {
                var headLable = new Label() { TextColor = Color.Blue, HorizontalTextAlignment = TextAlignment.Center };
                StackLayout sl = new StackLayout()
                {
                    HorizontalOptions = LayoutOptions.Center,
                    Children = { headLable },
                };
                return new ViewCell() { View = sl };
            });
            var listGroup = new List<TagDescBind>();
         
            ArrayList tagArray;
            ArrayList descArray;
            ArrayList detailedDescArray;
            ArrayList idArray;
            string head = "    " + date.Substring(5);
            double sum = di.readTodaySum(date);

            if (show.Count == 0 || ((ArrayList)show[0]).Count == 0)
            {
                tagArray = new ArrayList();
                descArray = new ArrayList();
                detailedDescArray = new ArrayList();
                idArray = new ArrayList();
                head = head + "日尚无记录。";
            }
            else
            {
                tagArray = (ArrayList)show[0];
                descArray = (ArrayList)show[1];
                detailedDescArray = (ArrayList)show[2];
                idArray = (ArrayList)show[3];
                if (sum >= 0)
                    head += ": +" + sum.ToString();
                else
                    head += ": " + sum.ToString();
            }
            for (int i = 0; i < tagArray.Count; i++)
            {
                listGroup.Add(new TagDescBind()
                { 
                    tagDesc = (string)tagArray[i], 
                    detailDesc = (string)descArray[i], 
                    descDesc = (string)detailedDescArray[i],
                    id = (int)idArray[i]
                });
            }
            CircleListView clv = new CircleListView() { ItemsSource = listGroup, Header = head, ItemTemplate = dt};
            clv.ItemTapped += showDetails;
            StackLayout page = new StackLayout() { Children = { prev, clv, next} };
            Content = page;
        }

        private event EventHandler DeleteBtnClicked;
        private event EventHandler PrevBtnClicked;
        private event EventHandler NextBtnClicked;
        private event EventHandler homeBtnClicked;
        private void homeClickedFunc(Object sender, EventArgs e)
        {
            Navigation.PushModalAsync(new FeatureSelection());
        }
        private void deleteClickedFunc(Object sender, EventArgs e)
        {
            DataInteractionV2 di = new DataInteractionV2();
            Navigation.PopModalAsync();
            Navigation.PopModalAsync();
            bool succ = di.deleteRec(deleteId);
            if (succ)
                Toast.DisplayIconText("删除记录成功", "Accept.png");
            else
                Toast.DisplayIconText("删除记录失败", "cancel.png");
        }
        private void prevClickedFunc(Object sender, EventArgs e)
        {
            DateTime goPrev = cCurrTime.AddDays(-1);
            RecordList prevRL = new RecordList(goPrev);
            Navigation.PushModalAsync(prevRL);
        }
        private void nextClickedFunc(Object sender, EventArgs e)
        {
            DateTime goNext = cCurrTime.AddDays(1);
            RecordList nextRL = new RecordList(goNext);
            Navigation.PushModalAsync(nextRL);
        }

        public void showDetails(Object sender, ItemTappedEventArgs e)
        {
            TagDescBind tdb = e.Item as TagDescBind;
            String time = tdb.detailDesc.Substring(0,8);
            String date = cCurrTime.ToString("yyyy-MM-dd").Substring(5);
            String amount;
            if (tdb.detailDesc.Contains("M"))
                amount = tdb.detailDesc.Substring(10);
            else
                amount = tdb.detailDesc.Substring(8);
            deleteId = tdb.id;
            DeleteBtnClicked += deleteClickedFunc;
            Label dateLabel = new Label() { Text = date, HorizontalTextAlignment = TextAlignment.Center, FontSize = 8 };
            Label timeLabel = new Label() { Text = time.Substring(0,5) + " | " + tdb.tagDesc, HorizontalTextAlignment = TextAlignment.Center, FontSize = 10 };
            Label descLabel = new Label() { Text = tdb.descDesc, HorizontalTextAlignment = TextAlignment.Center, FontSize = 7 };
            Label amountLabel = new Label()
            {
                Text = amount,
                HorizontalTextAlignment = TextAlignment.Center,
                FontSize = 20,
                TextColor = amount.Contains("+") ? Color.Green : Color.Red,
            };
            Button deleteBtn = new Button()
            {
                Text = "删除",
                TextColor = Color.Red,
                BackgroundColor = Color.Transparent,
            };
            deleteBtn.Clicked += DeleteBtnClicked;
            StackLayout sl = new StackLayout()
            { 
                Orientation = StackOrientation.Vertical,
                VerticalOptions = LayoutOptions.Center,
                Children = { dateLabel, timeLabel ,descLabel, amountLabel, deleteBtn},
            };
            CirclePage cp = new CirclePage() { Content = sl };
            Navigation.PushModalAsync(cp);
        }
    }
}
