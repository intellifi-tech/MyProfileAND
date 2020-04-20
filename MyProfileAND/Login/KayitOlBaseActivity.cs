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
    public class KayitOlBaseActivity : Android.Support.V7.App.AppCompatActivity
    {
        #region Tanimlamalart
        TextView GirisYap;
        EditText AdText, SoyadText, EmialText, SifreText;
        Button Kaydet;
        USER_INFO MemberInfo1 = new USER_INFO();
        TextView Sozlesmeler;
        #endregion
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.KayitOlBaseActivity);
            GirisYap = FindViewById<TextView>(Resource.Id.textView3);
            DinamikStatusBarColor dinamikStatusBarColor = new DinamikStatusBarColor();
            dinamikStatusBarColor.Beyaz(this);
            AdText = FindViewById<EditText>(Resource.Id.editText1);
            SoyadText = FindViewById<EditText>(Resource.Id.editText2);
            EmialText = FindViewById<EditText>(Resource.Id.editText3);
            SifreText = FindViewById<EditText>(Resource.Id.editText4);
            Kaydet = FindViewById<Button>(Resource.Id.button1);
            Sozlesmeler = FindViewById<TextView>(Resource.Id.sozlesme);
            Kaydet.Click += Kaydet_Click;

            GirisYap.Click += GirisYap_Click;

            Sozlesmeler.TextFormatted = Html.FromHtml(
            " Giriş yaparak " + "<font color=" + "#E30715" + "><b><a href=\"http://23.97.222.30\">Kullanım koşulları</a></b></font>  ve  <font color=" + "#E30715" + "><b><a href=\"http://23.97.222.30\">Gizlilik politikasını</a></b></font>" + " kabul etmiş olursunuz.");


            //AdText.Text = "Mesut";
            //SoyadText.Text = "Polat";
            //EmialText.Text = "demomobiluser2@intellifi.tech";
            //SifreText.Text = "qwer1234";

        }

        private void Kaydet_Click(object sender, EventArgs e)
        {
            if (Bosmu())
            {
                WebService webService = new WebService();
                KayitRootObject userLogin = new KayitRootObject()
                {
                    email = EmialText.Text.Trim(),
                    password = SifreText.Text,
                    name=AdText.Text,
                    surname = SoyadText.Text
                };
                string jsonString = JsonConvert.SerializeObject(userLogin);
                var Donus = webService.ServisIslem("user/register", jsonString);
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
            if (EmialText.Text.Trim() == "")
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
            else if (AdText.Text.Trim() == "")
            {
                return false;
            }
            else if (SoyadText.Text.Trim() == "")
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        private void GirisYap_Click(object sender, EventArgs e)
        {
            this.Finish();
        }
        public class KayitRootObject
        {
            public string name { get; set; }
            public string surname { get; set; }
            public string email { get; set; }
            public string password { get; set; }
        }
    }
}