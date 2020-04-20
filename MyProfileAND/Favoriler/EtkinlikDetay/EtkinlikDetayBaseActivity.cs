using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Location;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Content;
using Android.Views;
using Android.Widget;
using FFImageLoading;
using FFImageLoading.Transformations;
using FFImageLoading.Views;
using MyProfileAND.Favoriler.EtkinlikDetay.EtkinlikKatilimcilari;
using MyProfileAND.Favoriler.MevcutEtkinligeKatil;
using MyProfileAND.Favoriler.YeniEtkinlikOlustur;
using MyProfileAND.GenericClass;
using MyProfileAND.GenericUI;
using MyProfileAND.Profil;
using MyProfileAND.Profil.FarkliKullanici;
using MyProfileAND.WebServiceHelper;
using Newtonsoft.Json;

namespace MyProfileAND.Favoriler.EtkinlikDetay
{
    [Activity(Label = "MyProfile")]
    public class EtkinlikDetayBaseActivity : Android.Support.V7.App.AppCompatActivity
    {
        #region Tanimlamalar
        Android.Support.V7.Widget.Toolbar toolbar;
        EtkinlikDetayi_RootObject EtkinlikDetayi_RootObject1;
        ImageViewAsync EtkinlikImage,UserImage;
        TextView UserName, TitleSirket,EtkinlikTitle,EtkinlikAciklama,GirisCikisSaati,YorumlarText;
        LinearLayout YorumLinear;
        ImageButton EtkinligeKatilButton, EtkinlikKatilimcilariButton;

        CustomLoadingScreen CustomLoadingScreen1;
        FusedLocationProviderClient FusedLocationProviderClient1;
        LocationCallback LocationCallback1;
        LocationRequest LocationRequest1;
        #endregion
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.EtkinlikDetayBaseActivity);
            DinamikStatusBarColor DinamikStatusBarColor1 = new DinamikStatusBarColor();
            DinamikStatusBarColor1.Beyaz(this);
            toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.app_bar);
            SetSupportActionBar(toolbar);
            if (SupportActionBar != null)
            {
                SupportActionBar.SetDisplayHomeAsUpEnabled(true);
                SupportActionBar.Title = "";
                toolbar.NavigationIcon.SetColorFilter(Android.Graphics.Color.Black, PorterDuff.Mode.SrcAtop);
            }

            EtkinlikImage = FindViewById<ImageViewAsync>(Resource.Id.ımageView1);
            UserImage = FindViewById<ImageViewAsync>(Resource.Id.imgPortada_item2);
            UserName = FindViewById<TextView>(Resource.Id.textView3);
            TitleSirket = FindViewById<TextView>(Resource.Id.textView4);
            EtkinlikTitle = FindViewById<TextView>(Resource.Id.etkinliktitle);
            EtkinlikAciklama = FindViewById<TextView>(Resource.Id.textView2);
            GirisCikisSaati = FindViewById<TextView>(Resource.Id.textView6);
            YorumlarText = FindViewById<TextView>(Resource.Id.textView7);
            YorumLinear = FindViewById<LinearLayout>(Resource.Id.yorumlinear);
            EtkinligeKatilButton = FindViewById<ImageButton>(Resource.Id.etkinligekatil);
            EtkinlikKatilimcilariButton = FindViewById<ImageButton>(Resource.Id.etkinlikkatilimci);
            EtkinligeKatilButton.Click += EtkinligeKatilButton_Click;
            EtkinlikKatilimcilariButton.Click += EtkinlikKatilimcilariButton_Click;

            GetEventInfo();
            UserName.Click += UserName_Click;
            UserImage.Click += UserImage_Click;
           
        }

        private void EtkinlikKatilimcilariButton_Click(object sender, EventArgs e)
        {
            StartActivity(typeof(EtkinlikKatilimcilariBaseActivty));
        }

        private void EtkinligeKatilButton_Click(object sender, EventArgs e)
        {
            if (sonLokasyon != null)
            {
                EtkinligeKatilIslemleri();
            }
            else
            {
                KonumGetir();
            }
        }

        void EtkinligeKatilIslemleri()
        {
            SecilenEventt.EventID = EtkinlikDetayi_RootObject1.id;
            SecilenEventt.EventTitle = EtkinlikDetayi_RootObject1.title;
            SecilenEventt.Konum = sonLokasyon;
            StartActivity(typeof(YeniEtkinlikOlusturBaseActivity));
            this.Finish();
        }

        #region Kullanıcı Konumunu Al
        void KonumGetir()
        {
            
            if (ContextCompat.CheckSelfPermission(this, Android.Manifest.Permission.AccessFineLocation) == Permission.Granted)
            {
                
                BuildLocationRequest();
                LocationCallBack();
                FusedLocationProviderClient1 = LocationServices.GetFusedLocationProviderClient(this);
                KonumKontrol();
                OpenLoading();

                if (FusedLocationProviderClient1 != null)
                {
                    FusedLocationProviderClient1.RequestLocationUpdates(LocationRequest1, LocationCallback1, Looper.MyLooper());
                }
            }
            else
            {
                RequestPermissions(new String[] { Android.Manifest.Permission.AccessFineLocation }, 1);
            }
        }
        Android.Locations.Location sonLokasyon = null;
        public void GelenKonumuYansit(Android.Locations.Location gelenloc)
        {
            try
            {
                if (CustomLoadingScreen1 != null)
                {
                    CustomLoadingScreen1.Dismiss();
                    CustomLoadingScreen1 = null;
                }
            }
            catch
            {
            }
            if (sonLokasyon == null)
            {
                sonLokasyon = gelenloc;
                EtkinligeKatilIslemleri();
            }
            
        }

        void LocationCallBack()
        {
            LocationCallback1 = new MyLocationCallBack(this);
        }
        void BuildLocationRequest()
        {
            LocationRequest1 = new LocationRequest();
            LocationRequest1.SetPriority(LocationRequest.PriorityHighAccuracy);
            LocationRequest1.SetInterval(5000);
            LocationRequest1.SetFastestInterval(3000);
            LocationRequest1.SetSmallestDisplacement(10f);
        }

        internal class MyLocationCallBack : LocationCallback
        {
            private EtkinlikDetayBaseActivity EtkinlikDetayBaseActivity1;

            public MyLocationCallBack(EtkinlikDetayBaseActivity EtkinlikDetayBaseActivity2)
            {
                EtkinlikDetayBaseActivity1 = EtkinlikDetayBaseActivity2;
            }

            public override void OnLocationResult(LocationResult result)
            {
                base.OnLocationResult(result);
                EtkinlikDetayBaseActivity1.GelenKonumuYansit(result.LastLocation);
                //Toast.MakeText(EtkinlikDetayBaseActivity1, "Güncellendiiiiiiiiiiiiiii", ToastLength.Long).Show();
            }
        }


        void OpenLoading()
        {
            CustomLoadingScreen1 = new CustomLoadingScreen("Konumunuz Alıyor...");
            CustomLoadingScreen1.Cancelable = false;
            CustomLoadingScreen1.Show(this.SupportFragmentManager, "CustomLoadingScreen1");
        }

        bool KonumKontrol()
        {
            bool gps_enabled = false;
            bool network_enabled = false;
            return true;
            /*
            try
            {
                gps_enabled = locationManager.IsProviderEnabled(LocationManager.GpsProvider);
            }
            catch { }
            try
            {
                network_enabled = locationManager.IsProviderEnabled(LocationManager.NetworkProvider);
            }
            catch { }

            if (!gps_enabled && !network_enabled)
            {

                AlertHelper.AlertGoster("Lütfen konum ayarlarınızı açın.", this.Activity);
                return false;
            }
            else
            {
                return true;
            }*/
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            if (permissions.Length == 1 &&
                permissions[0] == Android.Manifest.Permission.AccessFineLocation && grantResults[0] == Permission.Granted)
            {
                if (ContextCompat.CheckSelfPermission(this, Android.Manifest.Permission.AccessFineLocation) != Permission.Granted && ContextCompat.CheckSelfPermission(this, Android.Manifest.Permission.AccessFineLocation) != Permission.Granted)
                {
                    return;
                }
                KonumGetir();
            }
            else
            {
                // Permission was denied. Display an error message.
            }
        }

        protected override void OnResume()
        {
            base.OnResume();
            if (FusedLocationProviderClient1 != null)
            {
                FusedLocationProviderClient1.RequestLocationUpdates(LocationRequest1, LocationCallback1, Looper.MyLooper());

            }
        }
        #endregion

        private void UserImage_Click(object sender, EventArgs e)
        {
            BilgileriGosterilecekKullanici.UserID = EtkinlikDetayi_RootObject1.user_attended_event[0].user.id;
            this.StartActivity(typeof(FarkliKullaniciBaseActivity));
        }

        private void UserName_Click(object sender, EventArgs e)
        {
            BilgileriGosterilecekKullanici.UserID = EtkinlikDetayi_RootObject1.user_attended_event[0].user.id;
            this.StartActivity(typeof(FarkliKullaniciBaseActivity));
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
        
        void GetEventInfo()
        {
            WebService webService = new WebService();
            var Donus = webService.OkuGetir("event/"+SecilenEtkinlik.EtkinlikID+"/show");
            if (Donus != null)
            {
                EtkinlikDetayi_RootObject1 = Newtonsoft.Json.JsonConvert.DeserializeObject<EtkinlikDetayi_RootObject>(Donus);
                ImageService.Instance.LoadUrl("http://23.97.222.30"+EtkinlikDetayi_RootObject1.user_attended_event[0].event_image)
                                                  .Into(EtkinlikImage);

                ImageService.Instance.LoadUrl("http://23.97.222.30"+EtkinlikDetayi_RootObject1.user_attended_event[0].user.profile_photo)
                                                  .Transform(new CircleTransformation(15, "#FFFFFF"))
                                                  .Into(UserImage);

                UserName.Text = EtkinlikDetayi_RootObject1.user_attended_event[0].user.name + " " + EtkinlikDetayi_RootObject1.user_attended_event[0].user.surname;
               
                EtkinlikTitle.Text = EtkinlikDetayi_RootObject1.title;
                EtkinlikAciklama.Text = EtkinlikDetayi_RootObject1.user_attended_event[0].event_description;
                string GirisSaati = "", CikisSaati = "";
                GirisSaati = Convert.ToDateTime(EtkinlikDetayi_RootObject1.user_attended_event[0].date_of_participation).ToShortTimeString();
                if (!String.IsNullOrEmpty(EtkinlikDetayi_RootObject1.user_attended_event[0].end_date))
                {
                    CikisSaati = Convert.ToDateTime(EtkinlikDetayi_RootObject1.user_attended_event[0].end_date).ToShortTimeString();
                    GirisCikisSaati.Text = GirisSaati + " - " + CikisSaati;
                }
                else
                {
                    GirisCikisSaati.Text = GirisSaati;
                }

                if (EtkinlikDetayi_RootObject1.user_attended_event[0].comments != null)
                {
                    if (EtkinlikDetayi_RootObject1.user_attended_event[0].comments.Count > 0)
                    {
                        YorumlarText.Text = "Yorumlar (" + EtkinlikDetayi_RootObject1.user_attended_event[0].comments.Count.ToString() + ")";
                    }
                    else
                    {
                        YorumlarText.Text = "İlk yorumu sen yap.";
                    }
                }

                if (EtkinlikDetayi_RootObject1.user_attended_event[0].user.company_id != null)
                {
                    GetCompanyID(EtkinlikDetayi_RootObject1.user_attended_event[0].user.company_id.ToString());
                }

                YorumBilgileriniYansit();
                KatilimcilariOlustur();
            }
        }

        void KatilimcilariOlustur()
        {
            EtkinlikKatilimcilariKisiler.Kisiler = EtkinlikDetayi_RootObject1.user_attended_event.Select(item => item.user).ToList();
        }

        void GetCompanyID(string ID)
        {
            new System.Threading.Thread(new System.Threading.ThreadStart(delegate
            {//EtkinlikDetayi_RootObject1.Event[0].user_user_attended_event.user.title
                try
                {
                    WebService webService = new WebService();
                    var Donus = webService.OkuGetir("company/" + ID.ToString() + "/show");
                    if (Donus != null)
                    {
                        var Modell = Newtonsoft.Json.JsonConvert.DeserializeObject<SirketModel>(Donus.ToString());
                        this.RunOnUiThread(() =>
                        {
                            TitleSirket.Text = EtkinlikDetayi_RootObject1.user_attended_event[0].user.title + " - " + Modell.name;
                            TitleSirket.Selected = true;
                        });
                    }
                }
                catch
                {
                }

            })).Start();

        }

        ImageButton YorumEkleButton;
        EditText YorumEdit;
        void YorumBilgileriniYansit()
        {
            YorumLinear.RemoveAllViews();

            for (int i = 0; i < EtkinlikDetayi_RootObject1.user_attended_event[0].comments.Count; i++)
            {
                LayoutInflater inflater = LayoutInflater.From(this);
                View parcaView = inflater.Inflate(Resource.Layout.YorumlarCustomView, YorumLinear, false);
                parcaView.FindViewById<TextView>(Resource.Id.textView2).Text = EtkinlikDetayi_RootObject1.user_attended_event[0].comments[i].comment;
                var UserNameee = parcaView.FindViewById<TextView>(Resource.Id.textView3);
                UserNameee.Text =  EtkinlikDetayi_RootObject1.user_attended_event[0].user.name + " " + EtkinlikDetayi_RootObject1.user_attended_event[0].user.surname;
                parcaView.FindViewById<TextView>(Resource.Id.textView4).Text = EtkinlikDetayi_RootObject1.user_attended_event[0].user.title;

                var UserPhotoo = parcaView.FindViewById<ImageViewAsync>(Resource.Id.imgPortada_item2);

                ImageService.Instance.LoadUrl("http://23.97.222.30"+EtkinlikDetayi_RootObject1.user_attended_event[0].user.profile_photo)
                                                .Transform(new CircleTransformation(15, "#FFFFFF"))
                                                .Into(UserPhotoo);
                YorumLinear.AddView(parcaView);

                UserNameee.Tag = EtkinlikDetayi_RootObject1.user_attended_event[0].user.id;
                UserPhotoo.Tag = EtkinlikDetayi_RootObject1.user_attended_event[0].user.id;
                UserNameee.Click += UserNameee_Click;
                UserPhotoo.Click += UserNameee_Click;
            }

            LayoutInflater inflater2 = LayoutInflater.From(this);
            View parcaView2 = inflater2.Inflate(Resource.Layout.YeniYorumEkleView, YorumLinear, false);
            YorumEkleButton = parcaView2.FindViewById<ImageButton>(Resource.Id.ımageButton1);
            YorumEdit = parcaView2.FindViewById<EditText>(Resource.Id.editText1);
            YorumEkleButton.Click += YorumEkleButton_Click;
            YorumLinear.AddView(parcaView2);
        }
        private void UserNameee_Click(object sender, EventArgs e)
        {
            BilgileriGosterilecekKullanici.UserID = (int)((View)sender).Tag;
            this.StartActivity(typeof(FarkliKullaniciBaseActivity));
        }

        private void YorumEkleButton_Click(object sender, EventArgs e)
        {
            WebService webService = new WebService();
            Yorum_Gonder_RootObject Yorum_Gonder_RootObject1 = new Yorum_Gonder_RootObject()
            {
                attended_id= Convert.ToInt32(EtkinlikDetayi_RootObject1.user_attended_event[0].id),
                comment = YorumEdit.Text
            };
            string jsonString = JsonConvert.SerializeObject(Yorum_Gonder_RootObject1);
            var Donus = webService.ServisIslem("comment/create", jsonString);
            if (Donus != "Hata")
            {
                var NewComments = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Comment>>(Donus);
                EtkinlikDetayi_RootObject1.user_attended_event[0].comments = NewComments;
                YorumBilgileriniYansit();
            }
        }

        public class Yorum_Gonder_RootObject
        {
            public int attended_id { get; set; }
            public string comment { get; set; }
        }

        #region DataModels

        public class SirketModel
        {
            public int id { get; set; }
            public string name { get; set; }
        }
       

        public class Comment
        {
            public int id { get; set; }
            public int user_id { get; set; }
            public int attended_id { get; set; }
            public string comment { get; set; }
            public string created_at { get; set; }
            public string updated_at { get; set; }
        }
        public class User
        {
            public int id { get; set; }
            public int type { get; set; }
            public string profile_photo { get; set; }
            public string cover_photo { get; set; }
            public string title { get; set; }
            public string name { get; set; }
            public string surname { get; set; }
            public string career_history { get; set; }
            public string short_biography { get; set; }
            public string credentials { get; set; }
            public string date_of_birth { get; set; }
            public string company_id { get; set; }
            public string sector_id { get; set; }
            public string email { get; set; }
            public string status { get; set; }
            public string package { get; set; }
            public string created_at { get; set; }
            public string updated_at { get; set; }
        }

        public class UserAttendedEvent
        {
            public int id { get; set; }
            public int event_id { get; set; }
            public int user_id { get; set; }
            public string event_description { get; set; }
            public string event_image { get; set; }
            public string date_of_participation { get; set; }
            public string end_date { get; set; }
            public int rating { get; set; }
            public string created_at { get; set; }
            public string updated_at { get; set; }
            public User user { get; set; }
            public List<Comment> comments { get; set; }
        }

        public class EtkinlikDetayi_RootObject
        {
            public int id { get; set; }
            public string title { get; set; }
            public double latitude { get; set; }
            public double longitude { get; set; }
            public string created_at { get; set; }
            public string updated_at { get; set; }
            public List<UserAttendedEvent> user_attended_event { get; set; }
        }

        #endregion


        public static class SecilenEtkinlik
        {
            public static string EtkinlikID { get; set; }
            public static string UserID { get; set; }
        }
    }


     
}