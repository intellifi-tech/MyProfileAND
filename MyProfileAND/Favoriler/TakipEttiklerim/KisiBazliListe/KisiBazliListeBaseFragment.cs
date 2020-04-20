using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using FFImageLoading;
using FFImageLoading.Transformations;
using FFImageLoading.Views;
using MyProfileAND.Favoriler.EtkinlikDetay;
using MyProfileAND.Favoriler.TakipEttiklerim;
using MyProfileAND.GenericClass;
using MyProfileAND.Profil;
using MyProfileAND.Profil.FarkliKullanici;
using MyProfileAND.WebServiceHelper;
using static MyProfileAND.Favoriler.EtkinlikDetay.EtkinlikDetayBaseActivity;

namespace MyProfileAND.Favoriler.TakipEttiklerim.KisiBazliListe
{
    [Activity(Label = "MyProfile")]
    public class KisiBazliListeBaseFragment : Android.Support.V7.App.AppCompatActivity
    {
        #region Tanımlamalar
        RecyclerView mRecyclerView;
        RecyclerView.LayoutManager mLayoutManager;
        KisiBazliListeRecyclerviewAdepter mViewAdapter;
        List<UserAttendedEvent> FavorilerRecyclerViewDataModel1;
        Android.Support.V7.Widget.Toolbar toolbar;
        TextView KisiAdi;
        ImageViewAsync KullaniciProfilFoto;
        #endregion
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.KisiBazliListeBaseFragment);
            DinamikStatusBarColor DinamikStatusBarColor1 = new DinamikStatusBarColor();
            DinamikStatusBarColor1.Beyaz(this);
            mRecyclerView = FindViewById<RecyclerView>(Resource.Id.recyclerView1);
            mRecyclerView.HasFixedSize = true;
            mLayoutManager = new LinearLayoutManager(this);
            mRecyclerView.SetLayoutManager(mLayoutManager);
            FavorilerRecyclerViewDataModel1 = Newtonsoft.Json.JsonConvert.DeserializeObject<List<UserAttendedEvent>>(KisininEventListesiClass.EventListJson);
            KisiAdi = FindViewById<TextView>(Resource.Id.textView1);
            KullaniciProfilFoto = FindViewById<ImageViewAsync>(Resource.Id.imgPortada_item2);
            ImageService.Instance.LoadUrl("http://23.97.222.30"+KisininEventListesiClass.UserImage)
                                                  .Transform(new CircleTransformation(15, "#FFFFFF"))
                                                  .Into(KullaniciProfilFoto);
            KisiAdi.Text = KisininEventListesiClass.UserName;

            toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.app_bar);
            SetSupportActionBar(toolbar);
            if (SupportActionBar != null)
            {
                SupportActionBar.SetDisplayHomeAsUpEnabled(true);
                SupportActionBar.Title = "";
                toolbar.NavigationIcon.SetColorFilter(Android.Graphics.Color.Black, PorterDuff.Mode.SrcAtop);
            }
            KisiAdi.Click += KisiAdi_Click;
            KullaniciProfilFoto.Click += KisiAdi_Click;
        }

        private void KisiAdi_Click(object sender, EventArgs e)
        {
            BilgileriGosterilecekKullanici.UserID = FavorilerRecyclerViewDataModel1[0].user_id;
            this.StartActivity(typeof(FarkliKullaniciBaseActivity));
        }

        protected override void OnStart()
        {
            base.OnStart();
            if (FavorilerRecyclerViewDataModel1.Count > 0)
            {
                mViewAdapter = new KisiBazliListeRecyclerviewAdepter(FavorilerRecyclerViewDataModel1, this);
                mRecyclerView.SetAdapter(mViewAdapter);
                mViewAdapter.ItemClick += MViewAdapter_ItemClick;
                var layoutManager = new LinearLayoutManager(this);
                mRecyclerView.SetLayoutManager(layoutManager);
            }
        }
        private void MViewAdapter_ItemClick(object sender, int e)
        {
            SecilenEtkinlik.EtkinlikID = FavorilerRecyclerViewDataModel1[e].event_id.ToString();
            this.StartActivity(typeof(EtkinlikDetayBaseActivity));
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
    }

    public static class KisininEventListesiClass
    {
        public static string EventListJson { get; set; }
        public static string UserName { get; set; }
        public static string UserImage { get; set; }
    }
}