using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using FFImageLoading;
using FFImageLoading.Transformations;
using FFImageLoading.Views;
using MyProfileAND.DataBasee;
using MyProfileAND.GenericClass;
using MyProfileAND.GenericUI;
using MyProfileAND.WebServiceHelper;
using Newtonsoft.Json;

namespace MyProfileAND.Profil.ProfilDuzenle
{
    [Activity(Label = "MyProfile")]
    public class ProfilDuzenleBaseActivity : Android.Support.V7.App.AppCompatActivity
    {
        #region Tanimalamlar
        EditText Biografi,SirketAdi,Titlee,AdText,SoyadText;
        TextView DogumTarihi, SektorlerSpin;
        ImageViewAsync ProfilFoto;
        ImageButton KaydetButton;
        List<KariyerGecmisi_RootObject> KariyerGecmisi_RootObject1 = new List<KariyerGecmisi_RootObject>();
        List<View> KariyerGecmisiViews = new List<View>();
        LinearLayout KariyerLinear;
        ImageButton YeniKariyerGecmisiEkleButton;
        Android.Support.V7.Widget.Toolbar toolbar;
        #endregion
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.ProfilGuncelleBaseActivityy);
            DinamikStatusBarColor DinamikStatusBarColor1 = new DinamikStatusBarColor();
            DinamikStatusBarColor1.Beyaz(this);
            Biografi = FindViewById<EditText>(Resource.Id.textView2);
            SirketAdi = FindViewById<EditText>(Resource.Id.sirkettxt);
            Titlee = FindViewById<EditText>(Resource.Id.titletxt);
            DogumTarihi = FindViewById<TextView>(Resource.Id.dogumtarihitxt);
            DogumTarihi.Click += DogumTarihi_Click;
            SektorlerSpin = FindViewById<TextView>(Resource.Id.sektortxt);
            AdText = FindViewById<EditText>(Resource.Id.adtext);
            SoyadText = FindViewById<EditText>(Resource.Id.soyadtxt);
            ProfilFoto = FindViewById<ImageViewAsync>(Resource.Id.imgPortada_item2);
            KaydetButton = FindViewById<ImageButton>(Resource.Id.kaydetbutton);
            KariyerLinear = FindViewById<LinearLayout>(Resource.Id.kariyerlinear);
            YeniKariyerGecmisiEkleButton = FindViewById<ImageButton>(Resource.Id.ımageButton1);
            YeniKariyerGecmisiEkleButton.Click += YeniKariyerGecmisiEkleButton_Click;
            Biografi.Text = "";
            SirketAdi.Text = "";
            Titlee.Text = "";
            DogumTarihi.Text = "";
            SektorlerSpin.Text = "";
            AdText.Text = "";
            SoyadText.Text = "";
            KaydetButton.Click += KaydetButton_Click;
            KullaniciBilgileriniYansit();
            KariyerGecmisiniYansit();
            SektorlerSpin.Click += SektorlerSpin_Click;
            toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.app_bar);
            SetSupportActionBar(toolbar);
            if (SupportActionBar != null)
            {
                SupportActionBar.SetDisplayHomeAsUpEnabled(true);
                SupportActionBar.Title = "";
                toolbar.NavigationIcon.SetColorFilter(Android.Graphics.Color.Black, PorterDuff.Mode.SrcAtop);
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


        private void YeniKariyerGecmisiEkleButton_Click(object sender, EventArgs e)
        {
            var KariyerGecmisiEkle1 = new KariyerGecmisiEkle(this);
            KariyerGecmisiEkle1.Show(this.SupportFragmentManager, "KariyerGecmisiEkle1");
        }

        private void DogumTarihi_Click(object sender, EventArgs e)
        {
            Tarih_Cek frag = Tarih_Cek.NewInstance(delegate (DateTime time)
            {
                DogumTarihi.Text = time.ToShortDateString();
            });
            frag.Show(this.FragmentManager, Tarih_Cek.TAG);
        }

        private void KaydetButton_Click(object sender, EventArgs e)
        {
            WebService webService = new WebService();
            var Kullanici = DataBase.USER_INFO_GETIR()[0];
            Kullanici.name = AdText.Text;
            Kullanici.surname = SoyadText.Text;
            Kullanici.short_biography = Biografi.Text;
            if (!String.IsNullOrEmpty(DogumTarihi.Text))
            {
                Kullanici.date_of_birth = Convert.ToDateTime(DogumTarihi.Text).ToString("yyyy-MM-dd");
            }
            Kullanici.company_title = SirketAdi.Text;
            Kullanici.sector_id = SektorlerSpin.Tag.ToString();
            Kullanici.title = Titlee.Text;

            string jsonString = JsonConvert.SerializeObject(Kullanici);
            var Donus = webService.ServisIslem("user/update", jsonString, Method: "PUT");
            if (Donus != "Hata")
            {
                var GuncelKullanici = Newtonsoft.Json.JsonConvert.DeserializeObject<UserGuncellenmisDataModel>(Donus);
                USER_INFO GuncellenecekDTO = new USER_INFO()
                {
                    cover_photo = GuncelKullanici.user.cover_photo,
                    api_token = Kullanici.api_token,
                    career_history = GuncelKullanici.user.career_history,
                    company_id = GuncelKullanici.user.company_id,
                    credentials = GuncelKullanici.user.credentials,
                    date_of_birth = GuncelKullanici.user.date_of_birth,
                    email = GuncelKullanici.user.email,
                    id = GuncelKullanici.user.id,
                    localid = Kullanici.localid,
                    name = GuncelKullanici.user.name,
                    profile_photo = GuncelKullanici.user.profile_photo,
                    sector_id = GuncelKullanici.user.sector_id,
                    short_biography = GuncelKullanici.user.short_biography,
                    status = GuncelKullanici.user.status,
                    surname = GuncelKullanici.user.surname,
                    title = GuncelKullanici.user.title,

                };

                DataBase.USER_INFO_Guncelle(GuncellenecekDTO);
                var NewUser = DataBase.USER_INFO_GETIR()[0];
                
                AlertHelper.AlertGoster("Profil Bilgileriniz Güncellendi.", this);
                this.Finish();
                return;
            }
        }

        private void SektorlerSpin_Click(object sender, EventArgs e)
        {
            var SektorSecDialogView1 = new SektorSecDialogView(this);
            SektorSecDialogView1.Show(this.SupportFragmentManager, "SektorSecDialogView1");
        }
        public  void SecilenSektorYansit(Sector Secim)
        {
            SektorlerSpin.Text = Secim.name;
            SektorlerSpin.Tag = Secim.id;
        }
        public void KariyerGecmisiniYansit()
        {
            KariyerLinear.RemoveAllViews();
            WebService webService = new WebService();
            var Donus = webService.OkuGetir("user/indexExperiences");
            if (Donus != null)
            {
                KariyerGecmisi_RootObject1 = Newtonsoft.Json.JsonConvert.DeserializeObject<List<KariyerGecmisi_RootObject>>(Donus);
                KariyerGecmisi_RootObject1.Reverse();
                for (int i = 0; i < KariyerGecmisi_RootObject1.Count; i++)
                {
                    LayoutInflater inflater = LayoutInflater.From(this);
                    View parcaView = inflater.Inflate(Resource.Layout.KariyerGecmisiCustomView, KariyerLinear, false);

                    parcaView.FindViewById<TextView>(Resource.Id.textView1).Text = KariyerGecmisi_RootObject1[i].company.name;
                    parcaView.FindViewById<TextView>(Resource.Id.textView2).Text = Convert.ToDateTime(KariyerGecmisi_RootObject1[i].start_time).Year.ToString() +
                                                                                     " - " +
                                                                                    Convert.ToDateTime(KariyerGecmisi_RootObject1[i].end_time).Year.ToString();
                    parcaView.FindViewById<TextView>(Resource.Id.textView3).Text = KariyerGecmisi_RootObject1[i].title;
                    KariyerLinear.AddView(parcaView);
                    KariyerGecmisiViews.Add(parcaView);
                }
            }
        }

        void KullaniciBilgileriniYansit()
        {
            var Kullanici = DataBase.USER_INFO_GETIR();
            if (Kullanici.Count > 0)
            {
                AdText.Text = Kullanici[0].name;
                SoyadText.Text = Kullanici[0].surname;
                Biografi.Text = Kullanici[0].short_biography;

                if (!String.IsNullOrEmpty(Kullanici[0].date_of_birth))
                {
                    try
                    {
                        DogumTarihi.Text = Convert.ToDateTime(Kullanici[0].date_of_birth).ToShortDateString();
                    }
                    catch { }
                }


                if (!string.IsNullOrEmpty(Kullanici[0].profile_photo))
                {
                    ImageService.Instance.LoadUrl(Kullanici[0].profile_photo)
                                                    .Transform(new CircleTransformation(15, "#FFFFFF"))
                                                    .Into(ProfilFoto);
                }

                if (!String.IsNullOrEmpty(Kullanici[0].sector_id))
                {
                    GetSektor(Kullanici[0].sector_id);
                }
                if (!String.IsNullOrEmpty(Kullanici[0].company_id))
                {
                    GetCompanyID(Kullanici[0].company_id);
                }

                if (!String.IsNullOrEmpty(Kullanici[0].title))
                {
                    Titlee.Text = Kullanici[0].title;
                }
            }
        }
        void GetCompanyID(string ID)
        {
            new System.Threading.Thread(new System.Threading.ThreadStart(delegate
            {
                try
                {
                    WebService webService = new WebService();
                    var Donus = webService.OkuGetir("company/" + ID.ToString() + "/show");
                    if (Donus != null)
                    {
                        var Modell = Newtonsoft.Json.JsonConvert.DeserializeObject<SirketModel>(Donus.ToString());
                        this.RunOnUiThread(() =>
                        {
                            SirketAdi.Text = Modell.name;
                        });
                    }

                }
                catch
                {


                }

            })).Start();

        }
        void GetSektor(string ID)
        {
            new System.Threading.Thread(new System.Threading.ThreadStart(delegate
            {
                try
                {
                    WebService webService = new WebService();
                    var Donus = webService.OkuGetir("sector/" + ID.ToString() + "/show");
                    if (Donus != null)
                    {
                        var Modell = Newtonsoft.Json.JsonConvert.DeserializeObject<SektorModel>(Donus.ToString());
                        this.RunOnUiThread(() =>
                        {
                            SektorlerSpin.Text = Modell.name;
                            SektorlerSpin.Tag = Modell.id;
                        });
                    }

                }
                catch
                {


                }

            })).Start();
        }
        public class SirketModel
        {
            public int id { get; set; }
            public string name { get; set; }
        }
        public class SektorModel
        {
            public int id { get; set; }
            public string name { get; set; }
        }

        public class User
        {
            public string id { get; set; }
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
            public string company_title { get; set; }
            public string sector_id { get; set; }
            public string email { get; set; }
            public string email_verified_at { get; set; }
            public string status { get; set; }
            public string package { get; set; }
            public string created_at { get; set; }
            public string updated_at { get; set; }
        }
        public class UserGuncellenmisDataModel
        {
            public int status { get; set; }
            public string message { get; set; }
            public User user { get; set; }
        }

        public class Company
        {
            public int id { get; set; }
            public string name { get; set; }
            public string created_at { get; set; }
            public string updated_at { get; set; }
        }
        public class KariyerGecmisi_RootObject
        {
            public int id { get; set; }
            public int user_id { get; set; }
            public string title { get; set; }
            public int company_id { get; set; }
            public string start_time { get; set; }
            public string end_time { get; set; }
            public string description { get; set; }
            public string created_at { get; set; }
            public string updated_at { get; set; }
            public Company company { get; set; }
        }
    }
}