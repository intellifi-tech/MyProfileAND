using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Graphics;

namespace MyProfileAND.GenericClass
{
   public class DinamikStatusBarColor
    {
        public void Beyaz(Activity Act )
        {
            Window window = Act.Window;
            window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);
            window.SetStatusBarColor(Color.Rgb(238,238,238));
            window.SetNavigationBarColor(Color.Rgb(255,255,255));
        }
        public void ShowCase(Activity Act)
        {
            if (Build.VERSION.SdkInt >= Build.VERSION_CODES.Lollipop)
            {
                Window window = Act.Window;
                window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);
                window.SetStatusBarColor(Color.Rgb(223, 8, 65));
                window.SetNavigationBarColor(Color.Rgb(223, 8, 65));
            }
        }
        public void Trans(Activity actt,bool makeTranslucent)
        {
            if (makeTranslucent)
            {

                Window window = actt.Window;
                window.AddFlags(WindowManagerFlags.Fullscreen);
                // setStatusBarTextColor(getResources().getColor(R.color.orange));

            }

            else {
                actt.Window.ClearFlags(WindowManagerFlags.TranslucentStatus);
            }
        }

    }
}