using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using MyProfileAND.DataBasee;
using MyProfileAND.Favoriler.EtkinlikDetay;
using MyProfileAND.WebServiceHelper;
using static MyProfileAND.Favoriler.EtkinlikDetay.EtkinlikDetayBaseActivity;

namespace MyProfileAND.Profil.Etkinlikleri
{
    public class ProfilEtkinliklerBaseFragment : Android.Support.V4.App.Fragment
    {
        #region Tanımlamalar
        RecyclerView mRecyclerView;
        RecyclerView.LayoutManager mLayoutManager;
        ProfilEtkinliklerRecyclerViewAdapter mViewAdapter;
        List<UserAttendedEvent> FavorilerRecyclerViewDataModel1;
        USER_INFO Kullanici;
        #endregion

        public ProfilEtkinliklerBaseFragment(USER_INFO Kullanici2)
        {
            Kullanici = Kullanici2;
        }


        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            View RootView = inflater.Inflate(Resource.Layout.ProfilEtkinliklerBaseFragment, container, false);
            mRecyclerView = RootView.FindViewById<RecyclerView>(Resource.Id.recyclerView1);
            mRecyclerView.HasFixedSize = true;
            mLayoutManager = new LinearLayoutManager(this.Activity);
            mRecyclerView.SetLayoutManager(mLayoutManager);
            return RootView;
        }
        public override void OnStart()
        {
            base.OnStart();
            FillDataModel();
        }
        void FillDataModel()
        {
            FavorilerRecyclerViewDataModel1 = new List<UserAttendedEvent>();
            WebService webService = new WebService();
            var Donus = webService.OkuGetir("user/"+ Kullanici .id+ "/events");
            if (Donus != null)
            {
                var Modell = Newtonsoft.Json.JsonConvert.DeserializeObject<ProfilEtkinliklerRecyclerViewDataModel>(Donus.ToString());
                FavorilerRecyclerViewDataModel1 = Modell.userAttendedEvents;
                FavorilerRecyclerViewDataModel1.Reverse();
                mViewAdapter = new ProfilEtkinliklerRecyclerViewAdapter(FavorilerRecyclerViewDataModel1, (Android.Support.V7.App.AppCompatActivity)this.Activity);
                mRecyclerView.SetAdapter(mViewAdapter);
                mViewAdapter.ItemClick += MViewAdapter_ItemClick;
                var layoutManager = new LinearLayoutManager(Activity);
                mRecyclerView.SetLayoutManager(layoutManager);
            }
        }

        private void MViewAdapter_ItemClick(object sender, int e)
        {
            SecilenEtkinlik.EtkinlikID = FavorilerRecyclerViewDataModel1[e].event_id.ToString();
            this.Activity.StartActivity(typeof(EtkinlikDetayBaseActivity));

        }
    }
}