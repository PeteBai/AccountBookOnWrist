using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;
using Tizen.Wearable.CircularUI.Forms;

namespace TizenWearableApp1
{
    public class App : Application
    {
        public App()
        {
            // The root page of your application
            /** TO-DO:
             * 
             * 
             * 添加HeaderTemplate问题
             * 显示居中问题
             * 显示的收入支出用颜色表示
             * 调整字号的问题
             * 
             * 添加选择日期查看的页面
             * 
             * 
             
             */
            MainPage = new NavigationPage(new FeatureSelection());
            
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
