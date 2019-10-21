using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Text;
using Android.Text.Style;
using Android.Views;
using Android.Widget;
using MyProfileAND.DataBasee;
using MyProfileAND.GenericClass;
using MyProfileAND.GenericUI;
using MyProfileAND.WebServiceHelper;
using Newtonsoft.Json;

namespace MyProfileAND.Profil.FarkliKullanici
{
    [Activity(Label = "MyProfile")]
    public class FarkliKullaniciBaseActivity : Android.Support.V7.App.AppCompatActivity
    {
        #region Tanimlamalar
        Android.Support.V7.Widget.Toolbar toolbar;
        ImageButton TakipButton;
        List<TakipEttiklerim_RootObject> TakipEttiklerimDataModel1 = new List<TakipEttiklerim_RootObject>();
        #endregion
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.FarkliKullaniciBaseActivity);
            DinamikStatusBarColor DinamikStatusBarColor1 = new DinamikStatusBarColor();
            DinamikStatusBarColor1.Beyaz(this);
            toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.app_bar);
            TakipButton = FindViewById<ImageButton>(Resource.Id.takipbuttonn);
            TakipButton.Click += TakipButton_Click;
            SetSupportActionBar(toolbar);
            if (SupportActionBar != null)
            {
                SupportActionBar.SetDisplayHomeAsUpEnabled(true);
                SupportActionBar.Title = "";
                toolbar.NavigationIcon.SetColorFilter(Android.Graphics.Color.Black, PorterDuff.Mode.SrcAtop);
            }
            ProfilfaYerlestir();
            TakipcileriGetir();
        }

        private void TakipButton_Click(object sender, EventArgs e)
        {
            if (!TakipDurumm)
            {
                Android.App.AlertDialog.Builder cevap = new Android.App.AlertDialog.Builder(this);
                cevap.SetIcon(Resource.Mipmap.ic_launcher);
                cevap.SetTitle(Spannla(Color.Black, "MyProfile"));
                cevap.SetMessage(Spannla(Color.DarkGray, "Takip etmek istediğinize emin misiniz?"));
                cevap.SetPositiveButton(Spannla(Color.Black, "Evet"), delegate
                {
                    TakipEt(BilgileriGosterilecekKullanici.UserID);
                    TakipButton.SetImageResource(Resource.Drawable.user_error);
                    cevap.Dispose();
                    
                });
                cevap.SetNegativeButton(Spannla(Color.Black, "Hayır"), delegate
                {
                    cevap.Dispose();
                });
                cevap.Show();
            }
            else
            {
                Android.App.AlertDialog.Builder cevap = new Android.App.AlertDialog.Builder(this);
                cevap.SetIcon(Resource.Mipmap.ic_launcher);
                cevap.SetTitle(Spannla(Color.Black, "MyProfile"));
                cevap.SetMessage(Spannla(Color.DarkGray, "Takibi bırakmak istediğinizden emin misiniz?"));
                cevap.SetPositiveButton(Spannla(Color.Black, "Evet"), delegate
                {
                    TakibiBirak(BilgileriGosterilecekKullanici.UserID);
                    TakipButton.SetImageResource(Resource.Drawable.users_add);
                    cevap.Dispose();
                    
                });
                cevap.SetNegativeButton(Spannla(Color.Black, "Hayır"), delegate
                {
                    cevap.Dispose();
                });
                cevap.Show();
            }
        }


        void TakipEt(int UserId)
        {
            TakipClass TakipClass1 = new TakipClass()
            {
                to_user_id = UserId
            };
            var jsonstring = JsonConvert.SerializeObject(TakipClass1);
            WebService webService = new WebService();
            var Donus = webService.ServisIslem("user/follow", jsonstring);
            if (Donus != "Hata")
            {
                AlertHelper.AlertGoster("Takip edildi.", this);
                return;
            }

        }
        void TakibiBirak(int UserId)
        {
            TakipClass TakipClass1 = new TakipClass()
            {
                to_user_id = UserId
            };
            var jsonstring = JsonConvert.SerializeObject(TakipClass1);
            WebService webService = new WebService();
            var Donus = webService.ServisIslem("user/stopFollowing", jsonstring);
            if (Donus != "Hata")
            {
                AlertHelper.AlertGoster("Takip durduruldu.", this);
                return;
            }
        }
        public class TakipClass
        {
            public int to_user_id { get; set; }
        }




        SpannableStringBuilder Spannla(Color Renk, string textt)
        {
            ForegroundColorSpan foregroundColorSpan = new ForegroundColorSpan(Renk);

            string title = textt;
            SpannableStringBuilder ssBuilder = new SpannableStringBuilder(title);
            ssBuilder.SetSpan(
                    foregroundColorSpan,
                    0,
                    title.Length,
                    SpanTypes.ExclusiveExclusive
            );

            return ssBuilder;
        }

        bool TakipDurumm = false;
        void TakipDurum()
        {
            var Durum = DataBase.USER_INFO_GETIR()[0];
            var TakipEdiyormuyum = TakipEttiklerimDataModel1.FindAll(item => item.to_user_id == BilgileriGosterilecekKullanici.UserID);

            if (TakipEdiyormuyum.Count > 0) //Takip Ediyorum
            {
                TakipButton.SetImageResource(Resource.Drawable.user_error);
                TakipDurumm = true;
            }
            else
            {
                TakipButton.SetImageResource(Resource.Drawable.users_add);
                TakipDurumm = false;
            }
        }
        void TakipcileriGetir()
        {

            WebService webService = new WebService();
            var Donus = webService.ServisIslem("user/followings", "");
            if (Donus != "Hata")
            {
                TakipEttiklerimDataModel1 = Newtonsoft.Json.JsonConvert.DeserializeObject<List<TakipEttiklerim_RootObject>>(Donus);
                TakipDurum();
            }
        }


        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    this.Finish();
                    return true;
                default:
                    return base.OnOptionsItemSelected(item);
            }
        }
        void ProfilfaYerlestir()
        {
            Android.Support.V4.App.FragmentTransaction ft;
            ProfilBaseFragment ProfilBaseFragment1 = new ProfilBaseFragment();
            ft = SupportFragmentManager.BeginTransaction();
            ft.AddToBackStack(null);
            ft.Replace(Resource.Id.content_frame, ProfilBaseFragment1);//
            ft.Commit();
        }



        public override void OnBackPressed()
        {
            this.Finish();
        }





        public class TakipEttiklerim_RootObject
        {
            public int from_user_id { get; set; }
            public int to_user_id { get; set; }
        }

    }
}