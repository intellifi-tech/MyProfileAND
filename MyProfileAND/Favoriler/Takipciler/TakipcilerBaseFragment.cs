using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using MyProfileAND.Favoriler.EtkinlikDetay;
using MyProfileAND.Favoriler.TakipEttiklerim;
using MyProfileAND.Favoriler.TakipEttiklerim.KisiBazliListe;
using MyProfileAND.WebServiceHelper;
using Newtonsoft.Json;
using static MyProfileAND.Favoriler.EtkinlikDetay.EtkinlikDetayBaseActivity;

namespace MyProfileAND.Favoriler.Takipciler
{
    public class TakipcilerBaseFragment : Android.Support.V4.App.Fragment
    {
        #region Tanımlamalar
        RecyclerView mRecyclerView;
        RecyclerView.LayoutManager mLayoutManager;
        TakipcilerRecyclerViewAdapter mViewAdapter;
        List<UserAttendedEvent> FavorilerRecyclerViewDataModel1;
        Android.Support.V4.App.FragmentTransaction ft;
        List<Following> TakipEttiklerimDataModel1 = new List<Following>();
        FrameLayout TakipcilerHaznesi;
        #endregion
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            View RootView = inflater.Inflate(Resource.Layout.TakipcilerBaseFragment, container, false);
            mRecyclerView = RootView.FindViewById<RecyclerView>(Resource.Id.recyclerView1);
            TakipcilerHaznesi = RootView.FindViewById<FrameLayout>(Resource.Id.hikayehazne);
            mRecyclerView.HasFixedSize = true;
            mLayoutManager = new LinearLayoutManager(this.Activity);
            mRecyclerView.SetLayoutManager(mLayoutManager);
            BekletveTakipleriGeitr();
            TakipcileriGetir();
            return RootView;
        }
        public void KisiselEventAc(int UserId)
        {
            var KisininEventleri = FavorilerRecyclerViewDataModel1.FindAll(item => item.user_id == UserId);
            if (KisininEventleri.Count>0)
            {
                KisininEventListesiClass.EventListJson = JsonConvert.SerializeObject(KisininEventleri);
                KisininEventListesiClass.UserName = KisininEventleri[0].UserInfo.name + " " + KisininEventleri[0].UserInfo.surname;
                KisininEventListesiClass.UserImage = KisininEventleri[0].UserInfo.profile_photo;
                this.Activity.StartActivity(typeof(KisiBazliListeBaseFragment));
            }
            
        }
        public void TakipEdilenKimseYok()
        {
            TakipcilerHaznesi.Visibility = ViewStates.Gone;
        }
        void BekletveTakipleriGeitr()
        {
            new System.Threading.Thread(new System.Threading.ThreadStart(async delegate
            {
                await Task.Run(() =>
                {
                    //await Task.Delay(500);
                    this.Activity.RunOnUiThread(() =>
                    {
                        try
                        {
                            ft = this.Activity.SupportFragmentManager.BeginTransaction();
                            ft.SetCustomAnimations(Resource.Animation.enter_from_right, Resource.Animation.exit_to_left, Resource.Animation.enter_from_left, Resource.Animation.exit_to_right);
                            ft.AddToBackStack(null);
                            ft.Replace(Resource.Id.hikayehazne, new FavorilerTakipEttiklerimBaseFragment(this));
                            ft.Commit();
                        }
                        catch 
                        {
                        }
                        
                    });
                });
            })).Start();
        }
        void TakipcileriGetir()
        {
            WebService webService = new WebService();
            var Donus = webService.ServisIslem("user/followings", "");
            if (Donus != "Hata")
            {
                try
                {
                    var DonusModel = Newtonsoft.Json.JsonConvert.DeserializeObject<List<TakipEttiklerimDataModel>>(Donus);
                    TakipEttiklerimDataModel1 = DonusModel[0].followings;
                    if (TakipEttiklerimDataModel1.Count > 0)
                    {
                        for (int i = 0; i < TakipEttiklerimDataModel1.Count; i++)
                        {
                            FillDataModel(TakipEttiklerimDataModel1[i]);
                        }

                        if (FavorilerRecyclerViewDataModel1.Count > 0)
                        {
                            mViewAdapter = new TakipcilerRecyclerViewAdapter(FavorilerRecyclerViewDataModel1, (Android.Support.V7.App.AppCompatActivity)this.Activity);
                            mRecyclerView.SetAdapter(mViewAdapter);
                            mViewAdapter.ItemClick += MViewAdapter_ItemClick;
                            var layoutManager = new LinearLayoutManager(Activity);
                            mRecyclerView.SetLayoutManager(layoutManager);
                        }

                    }
                }
                catch 
                {
                }
            }
        }
        void FillDataModel(Following UserInfoo)
        {
            FavorilerRecyclerViewDataModel1 = new List<UserAttendedEvent>();

            WebService webService = new WebService();
            var Donus = webService.OkuGetir("user/"+ UserInfoo.id.ToString()+ "/events");
            if (Donus != null)
            {
                var DonusModel = Newtonsoft.Json.JsonConvert.DeserializeObject<TakipcilerRecyclerViewDataModel>(Donus);
                if (DonusModel.status == 200)
                {
                    var Modell = DonusModel.userAttendedEvents;
                    if (Modell.Count > 0)
                    {
                        Modell.ForEach(x => x.UserInfo = UserInfoo);
                        FavorilerRecyclerViewDataModel1.AddRange(Modell);
                    }
                }
            }
        }
        private void MViewAdapter_ItemClick(object sender, int e)
        {
            SecilenEtkinlik.EtkinlikID = FavorilerRecyclerViewDataModel1[e].event_id.ToString();
            this.Activity.StartActivity(typeof(EtkinlikDetayBaseActivity));
        }
    }
}