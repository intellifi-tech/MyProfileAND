using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using MyProfileAND.AnaMenu;
using MyProfileAND.DataBasee;
using MyProfileAND.GenericClass;
using MyProfileAND.Login;

namespace MyProfileAND.Splash
{
    [Activity(Label = "MyProfile", MainLauncher = true)]
    public class SplashBaseActivity : Android.Support.V7.App.AppCompatActivity
    {
        #region Tanımlamalar
                #endregion
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.SplashBaseActivity);
            DinamikStatusBarColor dinamikStatusBarColor = new DinamikStatusBarColor();
            dinamikStatusBarColor.Beyaz(this);

            //string path;
            //path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            //File.Delete(System.IO.Path.Combine(path, "MyProfile.db"));

            HazirlikYap();
        }

        async void HazirlikYap()
        {
            new DataBase();
            await Task.Run(() => {
                Task.Delay(2000);
            });

            var Kullanici = DataBase.USER_INFO_GETIR();

            if (Kullanici.Count>0)
            {
                StartActivity(typeof(AnaMenuBaseActivitty));
            }
            else
            {
                //StartActivity(typeof(AnaMenuBaseActivitty));
                //return;
                StartActivity(typeof(LoginBaseActivty));
            }
        }
    }
}