using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tizen.Wearable.CircularUI.Forms;
using System.Drawing;
using Tizen.Applications.AttachPanel;

namespace TizenWearableApp1
{
    class DataVisualization : CirclePage
    {
        public DataVisualization()
        {
            DataInteractionV2 div2 = new DataInteractionV2();
            //Toast.DisplayText(div2.showCurrWeekNum().ToString());
            KeyValuePair<string, double>[] show = div2.readWeeklyTagPercentage(DateTime.Now.Year, div2.showCurrWeekNum());
            CircleProgressBarSurfaceItem first = new CircleProgressBarSurfaceItem()
            {
                Value = 0.5,
                BarLineWidth = 20,
                BackgroundLineWidth = 20,
                BarColor = Color.Crimson,
                BackgroundColor = Color.Transparent,
                BarRadius = 60,
            };
            CircleProgressBarSurfaceItem second = new CircleProgressBarSurfaceItem()
            {
                Value = show[1].Value,
                BarLineWidth = 20,
                BackgroundLineWidth = 20,
                BarColor = Color.Magenta,
                BackgroundColor = Color.Transparent,
                BarRadius = 80,
            };
            CircleSurfaceItems.Add(first);
            CircleSurfaceItems.Add(second);
        }
        
    }
}
