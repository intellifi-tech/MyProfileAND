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
using MyProfileAND.DataBasee;
using MyProfileAND.GenericUI;
using MyProfileAND.WebServiceHelper;
using Newtonsoft.Json;

namespace MyProfileAND.Profil.ProfilDuzenle.GizlilikAyarlari
{
    public class GizlilikDuzenleDialogFragment : Android.Support.V7.App.AppCompatDialogFragment
    {
        #region Tanitm
        ImageButton Kapat;
        ProfilDuzenleBaseActivity GelenBase1;
        ToggleButton Harita_Tog, Mesaj_Tog, Takip_Tog;
        Button Kaydet;
        #endregion
        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            Dialog.Window.RequestFeature(WindowFeatures.NoTitle);
            base.OnActivityCreated(savedInstanceState);
            Dialog.Window.Attributes.WindowAnimations = Resource.Style.action_sheet_animation;
        }

        public GizlilikDuzenleDialogFragment(ProfilDuzenleBaseActivity GelenBase2)
        {
            GelenBase1 = GelenBase2;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View rootView =  inflater.Inflate(Resource.Layout.GizlilikDuzenleDialogFragment, container, false);

            Kapat = rootView.FindViewById<ImageButton>(Resource.Id.ımageButton1);
            Kapat.Click += Kapat_Click;
            Harita_Tog = rootView.FindViewById<ToggleButton>(Resource.Id.toggleButton1);
            Mesaj_Tog = rootView.FindViewById<ToggleButton>(Resource.Id.toggleButton2);
            Takip_Tog = rootView.FindViewById<ToggleButton>(Resource.Id.toggleButton3);
            Kaydet = rootView.FindViewById<Button>(Resource.Id.button1);
            Kaydet.Click += Kaydet_Click;
            return rootView;
        }

        private void Kaydet_Click(object sender, EventArgs e)
        {
            SetGizlilik();
        }
        public override void OnStart()
        {
            base.OnStart();
            Dialog.Window.SetLayout(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
            Dialog.Window.SetGravity(GravityFlags.FillHorizontal | GravityFlags.CenterHorizontal | GravityFlags.CenterVertical);
            GetGizlilik();
        }
        private void Kapat_Click(object sender, EventArgs e)
        {
            this.Dismiss();
        }

        void SetGizlilik()
        {
            var MEID = DataBase.USER_INFO_GETIR()[0];
            WebService webService = new WebService();
            GIZLILIK set_Ayarlar_DTO = new GIZLILIK()
            {
                 no_follow_up_request = Takip_Tog.Checked,
                 no_message = Mesaj_Tog.Checked,
                 visibility_on_the_map = Harita_Tog.Checked
            };
            string jsonString = JsonConvert.SerializeObject(set_Ayarlar_DTO);
            var Donus = webService.ServisIslem("user/userPrivacySettings", jsonString);
            if (Donus != "Hata")
            {
                var AyarlarDonus = Newtonsoft.Json.JsonConvert.DeserializeObject<Get_Ayarlar_DTO>(Donus);
                DataBase.GIZLILIK_TEMIZLE();
                DataBase.GIZLILIK_EKLE(set_Ayarlar_DTO);
                AlertHelper.AlertGoster("Gizlilik ayarlarınız güncellendi", this.Activity);
                this.Dismiss();
            }
        }
        
        void GetGizlilik()
        {
            new System.Threading.Thread(new System.Threading.ThreadStart(delegate
            {
                var MeId = DataBase.USER_INFO_GETIR()[0];
                WebService webService = new WebService();
                var Donus = webService.OkuGetir("user/" + MeId.id.ToString() + "/getUserPrivacySettings");
                if (Donus != null)
                {
                    var aaa = Donus.ToString();
                    var Ayarlarr = Newtonsoft.Json.JsonConvert.DeserializeObject<UserPrivacy>(Donus.ToString());
                    this.Activity.RunOnUiThread(delegate ()
                    {
                        Harita_Tog.Checked = Ayarlarr.visibility_on_the_map;
                        Mesaj_Tog.Checked = Ayarlarr.no_message;
                        Takip_Tog.Checked = Ayarlarr.no_follow_up_request;
                    });
                }
            })).Start();
        }

        public class UserPrivacy
        {
            public int id { get; set; }
            public int user_id { get; set; }
            public bool visibility_on_the_map { get; set; }
            public bool no_message { get; set; }
            public bool no_follow_up_request { get; set; }
            public string created_at { get; set; }
            public string updated_at { get; set; }
        }

        public class Get_Ayarlar_DTO
        {
            public int status { get; set; }
            public string message { get; set; }
            public UserPrivacy userPrivacy { get; set; }
        }
    }
}

