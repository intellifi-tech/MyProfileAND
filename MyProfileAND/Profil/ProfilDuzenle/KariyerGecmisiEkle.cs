using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using MyProfileAND.GenericClass;
using MyProfileAND.WebServiceHelper;
using Newtonsoft.Json;

namespace MyProfileAND.Profil.ProfilDuzenle
{
    public class KariyerGecmisiEkle : Android.Support.V7.App.AppCompatDialogFragment
    {
        #region Tanitm
        ImageButton Kapat;
        TextView BasTarihi, BitTarihi;
        Button Kaydet;
        EditText SirketAdi,Titlee;
        ProfilDuzenleBaseActivity ProfilDuzenleBaseActivity1;
        #endregion
        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            Dialog.Window.RequestFeature(WindowFeatures.NoTitle);
            base.OnActivityCreated(savedInstanceState);
            Dialog.Window.Attributes.WindowAnimations = Resource.Style.action_sheet_animation;
        }
        
        public KariyerGecmisiEkle(ProfilDuzenleBaseActivity ProfilDuzenleBaseActivity2)
        {
            ProfilDuzenleBaseActivity1 = ProfilDuzenleBaseActivity2;
        }


        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View rootView =  inflater.Inflate(Resource.Layout.KariyerGecmisiEkle, container, false);
            Dialog.Window.SetLayout(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
            //Dialog.Window.SetBackgroundDrawable(new ColorDrawable(Color.Transparent));
            Dialog.Window.SetGravity(GravityFlags.FillHorizontal | GravityFlags.CenterHorizontal | GravityFlags.CenterVertical);
            Kapat = rootView.FindViewById<ImageButton>(Resource.Id.ımageButton1);
            BasTarihi = rootView.FindViewById<TextView>(Resource.Id.bastarih);
            BitTarihi = rootView.FindViewById<TextView>(Resource.Id.bittarihi);
            Kaydet = rootView.FindViewById<Button>(Resource.Id.button1);
            SirketAdi = rootView.FindViewById<EditText>(Resource.Id.editText1);
            Titlee = rootView.FindViewById<EditText>(Resource.Id.editText2);
            Kaydet.Click += Kaydet_Click;
            BasTarihi.Click += BasTarihi_Click;
            BitTarihi.Click += BitTarihi_Click;
            Kapat.Click += Kapat_Click;
            return rootView;
        }

        private void Kaydet_Click(object sender, EventArgs e)
        {
            WebService webservices = new WebService();
            DeneyimEkle_RootObject DeneyimEkle_RootObject1 = new DeneyimEkle_RootObject() {
                company_title = SirketAdi.Text,
                description="",
                end_time = Convert.ToDateTime(BitTarihi.Text).ToString("yyyy-MM-dd"),
                start_time = Convert.ToDateTime(BasTarihi.Text).ToString("yyyy-MM-dd"),
                title = Titlee.Text
            };

            string jsonString = JsonConvert.SerializeObject(DeneyimEkle_RootObject1);
            var Donus = webservices.ServisIslem("user/storeExperiences", jsonString);
            if (Donus != "Hata")
            {
                ProfilDuzenleBaseActivity1.KariyerGecmisiniYansit();
                this.Dismiss();
            }
        }

        private void BitTarihi_Click(object sender, EventArgs e)
        {
            Tarih_Cek frag = Tarih_Cek.NewInstance(delegate (DateTime time)
            {
                BitTarihi.Text = time.ToShortDateString();
            });
            frag.Show(this.Activity.FragmentManager, Tarih_Cek.TAG);
        }

        private void BasTarihi_Click(object sender, EventArgs e)
        {
            Tarih_Cek frag = Tarih_Cek.NewInstance(delegate (DateTime time)
            {
                BasTarihi.Text = time.ToShortDateString();
            });
            frag.Show(this.Activity.FragmentManager, Tarih_Cek.TAG);
        }

        private void Kapat_Click(object sender, EventArgs e)
        {
            this.Dismiss();
        }


        public class DeneyimEkle_RootObject
        {
            public string title { get; set; }
            public string company_title { get; set; }
            public string start_time { get; set; }
            public string end_time { get; set; }
            public string description { get; set; }
        }

    }
}

