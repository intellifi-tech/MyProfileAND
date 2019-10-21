using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using MyProfileAND.DataBasee;
using MyProfileAND.GenericClass;
using MyProfileAND.WebServiceHelper;
using Square.Picasso;

namespace MyProfileAND.Profil.ProfilDetaylari
{
    public class ProfilDetaylariBaseFragment : Android.Support.V4.App.Fragment
    {
        #region Tanimlamalar
        TextView Biografi,DogumTarihi,Sirket,Sektor;
        List<UserExperience> UserExperience1 = new List<UserExperience>();
        List<View> KariyerGecmisiViews = new List<View>();
        LinearLayout KariyerLinear;
        USER_INFO Kullanici;
        #endregion

        public ProfilDetaylariBaseFragment(USER_INFO Kullanici2)
        {
            Kullanici = Kullanici2;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            View RootView = inflater.Inflate(Resource.Layout.ProfilDetaylariBaseFragment, container, false);
            Biografi = RootView.FindViewById<TextView>(Resource.Id.textView2);
            DogumTarihi = RootView.FindViewById<TextView>(Resource.Id.dogumtarihitxt);
            Sirket = RootView.FindViewById<TextView>(Resource.Id.sirkettxt);
            Sektor = RootView.FindViewById<TextView>(Resource.Id.sektortxt);
            KariyerLinear = RootView.FindViewById<LinearLayout>(Resource.Id.kariyerlinear);

            Biografi.Text = "";
            DogumTarihi.Text = "";
            Sirket.Text = "";
            Sektor.Text = "";
            return RootView;
        }


        public override void OnStart()
        {
            base.OnStart();
            KariyerLinear.RemoveAllViews();
            KariyerGecmisiViews = new List<View>();
            KullaniciBilgileriYansit();
        }

        void KariyerGecmisiniYansit()
        {
            WebService webService = new WebService();
            var Donus = webService.OkuGetir("user/"+ Kullanici.id.ToString()+ "/userExperiences");
            if (Donus != null)
            {
                var Modell = Newtonsoft.Json.JsonConvert.DeserializeObject<KariyerGecmisi_RootObject>(Donus);
                if (Modell.status == 200)
                {
                    UserExperience1 = Modell.userExperiences;
                    UserExperience1.Reverse();
                    for (int i = 0; i < UserExperience1.Count; i++)
                    {
                        LayoutInflater inflater = LayoutInflater.From(this.Activity);
                        View parcaView = inflater.Inflate(Resource.Layout.KariyerGecmisiCustomView, KariyerLinear, false);

                        parcaView.FindViewById<TextView>(Resource.Id.textView1).Text = UserExperience1[i].company.name;
                        parcaView.FindViewById<TextView>(Resource.Id.textView2).Text = Convert.ToDateTime(UserExperience1[i].start_time).Year.ToString() +
                                                                                         " - " +
                                                                                        Convert.ToDateTime(UserExperience1[i].end_time).Year.ToString();
                        parcaView.FindViewById<TextView>(Resource.Id.textView3).Text = UserExperience1[i].title;
                        KariyerLinear.AddView(parcaView);
                        KariyerGecmisiViews.Add(parcaView);
                    }
                }
            
            }
        }
        void KullaniciBilgileriYansit()
        {
            Biografi.Text = Kullanici.short_biography;
            if (!String.IsNullOrEmpty(Kullanici.date_of_birth))
            {
                try
                {
                    DogumTarihi.Text = Convert.ToDateTime(Kullanici.date_of_birth).ToString();
                }
                catch { }
            }
            if (!String.IsNullOrEmpty(Kullanici.sector_id))
            {
                GetSektor(Kullanici.sector_id);
            }
            if (!String.IsNullOrEmpty(Kullanici.company_id))
            {
                GetCompanyID(Kullanici.company_id);
            }
            KariyerGecmisiniYansit();
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
                        this.Activity.RunOnUiThread(() =>
                        {
                            Sirket.Text = Modell.name;
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
                        this.Activity.RunOnUiThread(() =>
                        {
                            Sektor.Text = Modell.name;
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

        public class Company
        {
            public int id { get; set; }
            public string name { get; set; }
            public string created_at { get; set; }
            public string updated_at { get; set; }
        }
        public class UserExperience
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

        public class KariyerGecmisi_RootObject
        {
            public int status { get; set; }
            public string message { get; set; }
            public List<UserExperience> userExperiences { get; set; }
        }
    }
}