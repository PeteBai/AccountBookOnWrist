using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Input;
using Tizen.Wearable.CircularUI.Forms;
using Xamarin.Forms;
//using Xamarin.Forms.Platform.Tizen.Native;

namespace TizenWearableApp1
{
    public class IconDescBind
    {
        public string path { get; set; }
        public string tagDesc { get; set; }
    }
    class AddRecord : CirclePage
    {
        Double amount;
        String tagInput;
        String descInput;
        bool isOut = false;
        PopupEntry pex = new PopupEntry() { Text = "请输入金额", Keyboard = Keyboard.Numeric, HorizontalTextAlignment = TextAlignment.Center };
        PopupEntry desc = new PopupEntry() { Text = "输入描述(可选)", HorizontalTextAlignment = TextAlignment.Center };
        Button inBtn = new Button() { Text = "收入", BackgroundColor = Color.Green };
        Button outBtn = new Button() { Text = "支出", BackgroundColor = Color.Red };
        Button tagBtn = new Button() { Text = "点击这里选择标签" };
        Button b = new Button() { ImageSource = new FileImageSource() { File = "next.png" }, BackgroundColor = Color.Transparent };
        Button submit = new Button() { ImageSource = new FileImageSource() { File = "Accept.png" }, BackgroundColor = Color.Transparent };
        public AddRecord()
        {
            ActionButtonItem abi = new ActionButtonItem() { Text = "OK" };
            inBtn.IsVisible = false;
            outBtn.IsVisible = false;
            b.IsEnabled = false;
            OnButtonClicked += new EventHandler(OnButtonClick);
            inBtnClicked += inClickedFunc;
            outBtnClicked += outClickedFunc;
            tagBtnClicked += tagClickedFunc;
            submitBtnClicked += submitClickedFunc;
            b.Clicked += OnButtonClicked;
            inBtn.Clicked += inBtnClicked;
            outBtn.Clicked += outBtnClicked;
            tagBtn.Clicked += new EventHandler(tagBtnClicked);
            submit.Clicked += submitBtnClicked;
            StackLayout inOutBtnLayout = new StackLayout()
            {
                HorizontalOptions = LayoutOptions.Center,
                Orientation = StackOrientation.Horizontal,
                Children = { inBtn, outBtn }
            };
            Content = new StackLayout {
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                Orientation = StackOrientation.Vertical,
                Children = { pex, tagBtn, new Label() { Text = "  "}, inOutBtnLayout, b, }, };
        }

        private event EventHandler OnButtonClicked;
        private event EventHandler inBtnClicked;
        private event EventHandler outBtnClicked;
        private event EventHandler tagBtnClicked;
        private event EventHandler submitBtnClicked;

        private void inClickedFunc(Object sender, EventArgs e)
        {
            outBtn.IsVisible = false;
            inBtn.BackgroundColor = Color.Transparent;
            inBtn.TextColor = Color.LightGreen;
            this.b.IsEnabled = true;
        }
        private void outClickedFunc(Object sender, EventArgs e)
        {
            pex.Text = "-" + pex.Text;
            inBtn.IsVisible = false;
            outBtn.BackgroundColor = Color.Transparent;
            outBtn.TextColor = Color.LightPink;
            isOut = true;
            this.b.IsEnabled = true;
        }
        private void submitClickedFunc(Object sender, EventArgs e)
        {
            try
            {
                amount = System.Convert.ToDouble(pex.Text);
            }
            catch(FormatException)
            {
                Toast.DisplayIconText("您需要输入一个数字", "warning.png");
                Navigation.PopModalAsync();
                Navigation.PopModalAsync();
                Navigation.PushModalAsync(new AddRecord());
                return;
            }
            if (isOut == true && amount > 0)
                amount = 0 - amount;
            //TagProvider tp = new TagProvider();
            tagInput = idb.path;
            descInput = desc.Text;
            DataInteractionV2 di = new DataInteractionV2();
            TwoButtonPopup twoButtonPopup = new TwoButtonPopup();
            var confirmButton = new MenuItem()
            {
                IconImageSource = new FileImageSource() { File = "Accept.png" },
                Command = new Command(() => {
                    if(di.writeRec(descInput, tagInput, amount))
                        Toast.DisplayIconText("已成功添加", "Accept.png");
                    twoButtonPopup.Dismiss();
                    pex.Text = "请输入金额";
                    desc.Text = "输入描述(可选)";
                })
            };
            var cancelButton = new MenuItem()
            {
                IconImageSource = new FileImageSource() { File = "cancel.png" },
                Command = new Command(() => { twoButtonPopup.Dismiss(); }),
            };
            twoButtonPopup.FirstButton = confirmButton;
            twoButtonPopup.SecondButton = cancelButton;
            twoButtonPopup.Title = "提交确认";
            twoButtonPopup.Content = new StackLayout
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Children = { new Xamarin.Forms.Label { Text = "您确定要提交吗？" } }
            };
            inBtn.IsVisible = true;
            inBtn.BackgroundColor = Color.Green;
            isOut = false;
            outBtn.IsVisible = true;
            outBtn.BackgroundColor = Color.Red;
            twoButtonPopup.Show();
            Navigation.PopModalAsync();
        }
        private void OnButtonClick(Object sender, EventArgs e)
        {
            StackLayout descEntryLayout = new StackLayout()
            {
                Orientation = StackOrientation.Vertical,
                VerticalOptions = LayoutOptions.Center,
                Children = { desc, submit },
            };
            //an optional input description page
            CirclePage descEntryPage = new CirclePage()
            {
                Content = descEntryLayout,
            };
            Navigation.PushModalAsync(descEntryPage);
        }
        private void tagClickedFunc(Object sender, EventArgs e)
        {
            TagProvider tp = new TagProvider();
            Dictionary<string, string> d = tp.getAllTag();
            int tagCount = tp.getNum();
            var tagGroup = new List<IconDescBind>();
            foreach (string key in d.Keys)
            {
                tagGroup.Add(new IconDescBind { path = key, tagDesc = d[key] });
            }
            DataTemplate dt = new DataTemplate(() =>
            {
                var icon = new Image() { HorizontalOptions = LayoutOptions.End };
                var tagDescLable = new Label() { HorizontalTextAlignment = TextAlignment.Center };
                //var space = new Label() { Text = "    " };
                icon.SetBinding(Image.SourceProperty, "path");
                tagDescLable.SetBinding(Label.TextProperty, "tagDesc");
                StackLayout sl = new StackLayout()
                {
                    Orientation = StackOrientation.Horizontal,
                    HorizontalOptions = LayoutOptions.Center,
                    Children = { icon, tagDescLable, }
                };
                return new ViewCell { View = sl };
            });
            CircleListView clv = new CircleListView
            {
                ItemsSource = tagGroup,
                ItemTemplate = dt,
                HorizontalOptions = LayoutOptions.Center,
            };
            clv.ItemTapped += TagTapped;
            cp = new CirclePage()
            {
                Content = clv
            };
            //np.PushAsync(cp);
            Navigation.PushModalAsync(cp);
        }
        IconDescBind idb;
        CirclePage cp;
        private async void TagTapped(Object sender, ItemTappedEventArgs e)
        {
            idb = e.Item as IconDescBind;
            this.tagBtn.Text = idb.tagDesc;
            this.tagBtn.BackgroundColor = Color.Transparent;
            this.inBtn.IsVisible = true;
            this.outBtn.IsVisible = true;
            await Navigation.PopModalAsync();
        }
    }
}
