using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Text;
using Android.Views;
using Android.Widget;
using MyProfileAND.AnaMenu;
using MyProfileAND.DataBasee;
using MyProfileAND.GenericClass;
using MyProfileAND.WebServiceHelper;
using Newtonsoft.Json;

namespace MyProfileAND.Login
{
    [Activity(Label = "MyProfile")]
    public class LoginBaseActivty : Android.Support.V7.App.AppCompatActivity
    {
        EditText EmailText, SifreText;
        Button GirisYap;
        USER_INFO MemberInfo1 = new USER_INFO();
        TextView Sozlesmeler,SifremiUnuttum,Kayitol;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.LoginBaseActivty);
            DinamikStatusBarColor dinamikStatusBarColor = new DinamikStatusBarColor();
            dinamikStatusBarColor.Beyaz(this);
            EmailText = FindViewById<EditText>(Resource.Id.editText1);
            SifreText = FindViewById<EditText>(Resource.Id.editText2);
            GirisYap = FindViewById<Button>(Resource.Id.button1);
            Sozlesmeler = FindViewById<TextView>(Resource.Id.sozlesme);
            SifremiUnuttum = FindViewById<TextView>(Resource.Id.textView2);
            Kayitol = FindViewById<TextView>(Resource.Id.kayitoltext);

            Kayitol.Click += Kayitol_Click;

            Sozlesmeler.TextFormatted = Html.FromHtml(
            " Giriş yaparak " + "<font color=" + "#E30715" + "><b><a href=\"https://www.intellifi.tech\">Kullanım koşulları</a></b></font>  ve  <font color=" + "#E30715" + "><b><a href=\"https://www.intellifi.tech\">Gizlilik politikasını</a></b></font>" + " kabul etmiş olursunuz.");

            GirisYap.Click += GirisYap_Click;

            EmailText.Text = "mesut@intellifi.tech";
            SifreText.Text = "1234qwer";

        }

        private void Kayitol_Click(object sender, EventArgs e)
        {
            StartActivity(typeof(KayitOlBaseActivity));
        }

        private void GirisYap_Click(object sender, EventArgs e)
        {
            if (Bosmu())
            {
                WebService webService = new WebService();
                UserLogin userLogin = new UserLogin() {
                    email= EmailText.Text.Trim(),
                    password = SifreText.Text
                };
                string jsonString = JsonConvert.SerializeObject(userLogin);
                var Donus = webService.ServisIslem("user/login", jsonString);
                if (Donus == "Hata")
                {
                    Toast.MakeText(this, "Hata", ToastLength.Long).Show();
                    return;
                }
                else
                {
                    MemberInfo1 = Newtonsoft.Json.JsonConvert.DeserializeObject<USER_INFO>(Donus);
                    DataBase.USER_INFO_EKLE(MemberInfo1);
                    StartActivity(typeof(AnaMenuBaseActivitty));
                }
            }
        }

        bool Bosmu()
        {
            if (EmailText.Text.Trim() == "")
            {

                return false;
            }
            else if (SifreText.Text.Trim() == "")
            {
                return false;
            }
            else if (SifreText.Text.Length < 6)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        #region DtaaModels
        public class UserLogin
        {
            public string email { get; set; }
            public string password { get; set; }
        }
        #endregion

    }
}