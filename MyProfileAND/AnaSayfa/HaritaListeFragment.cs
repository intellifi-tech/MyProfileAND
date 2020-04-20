using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Gms.Maps.Model;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using MyProfileAND.GenericClass;
using MyProfileAND.Profil;
using MyProfileAND.Profil.FarkliKullanici;
using Newtonsoft.Json;
using static MyProfileAND.AnaSayfa.AnaSayfaBaseFragment;

namespace MyProfileAND.AnaSayfa
{
    public class HaritaListeFragment : Android.Support.V4.App.Fragment
    {
        #region Tanımlamalar
        RecyclerView mRecyclerView;
        Android.Support.V7.Widget.LinearLayoutManager mLayoutManager;
        AnaMainRecyclerViewAdapter mViewAdapter;
        public List<NearbyUserCoordinate> MapDataModel1;
        public List<NearbyUserCoordinate> MapDataModel1_KLON;
        public List<TakipEttiklerim_RootObject> TakipEttiklerimList;
        AnaSayfaBaseFragment GelenBase;
        HaritaListeRecyclerViewOnScrollListener ScrollDinleyici;
        public int LastScrollPosition;
        #endregion
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }
        public HaritaListeFragment(AnaSayfaBaseFragment Base, List<NearbyUserCoordinate> MapDataModel11, List<TakipEttiklerim_RootObject> TakipEttiklerimList2)
        {
            GelenBase = Base;
            MapDataModel1_KLON = MapDataModel11;
            MapDataModel1 = MapDataModel11.FindAll(item => item.IsShow == true);
            //MapDataModel1 = MapDataModel11;
            TakipEttiklerimList = TakipEttiklerimList2;
        }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View rootview = inflater.Inflate(Resource.Layout.HaritaListeFragment, container, false);
            mRecyclerView = rootview.FindViewById<RecyclerView>(Resource.Id.recyclerView1);
            var a = MapDataModel1;
            mRecyclerView.HasFixedSize = true;
            mLayoutManager = new LinearLayoutManager(this.Activity);
            mRecyclerView.SetLayoutManager(mLayoutManager);
            mViewAdapter = new AnaMainRecyclerViewAdapter(this, (Android.Support.V7.App.AppCompatActivity)this.Activity);
            mRecyclerView.SetAdapter(mViewAdapter);
            mViewAdapter.ItemClick += MViewAdapter_ItemClick;
            mLayoutManager = new LinearLayoutManager(Activity, LinearLayoutManager.Horizontal, false);

            // mLayoutManager = new CenterZoomLayoutManager(this.Activity, LinearLayoutManager.Horizontal, false);
            mRecyclerView.SetLayoutManager(mLayoutManager);
            //ScrollDinleyici = new HaritaListeRecyclerViewOnScrollListener(mLayoutManager, this);
            //mRecyclerView.AddOnScrollListener(ScrollDinleyici);
            try
            {
                //SnapHelper snapHelper = new LinearSnapHelper();
                //snapHelper.AttachToRecyclerView(mRecyclerView);
            }
            catch
            {
            }
            return rootview;
        }

        private void MViewAdapter_ItemClick(object sender, int e)
        {
            var Indexx = MapDataModel1_KLON.FindIndex(item => item.user.id == MapDataModel1[e].user.id);
            GelenBase.MarkerSec(Indexx);
            mViewAdapter.NotifyItemChanged(e);
            BilgileriGosterilecekKullanici.UserID = MapDataModel1[e].user.id;
            this.Activity.StartActivity(typeof(FarkliKullaniciBaseActivity));
        }

        public void TakipcileriGuncelle()
        {
            GelenBase.TakipEttiklerimiGetir();
        }

        public void SecimYap(int position)
        {
            var Indexx = MapDataModel1.FindIndex(item => item.user.id == MapDataModel1_KLON[position].user.id);
            mRecyclerView.SmoothScrollToPosition(Indexx);
            MapDataModel1[Indexx].Secim = true;
            mViewAdapter.NotifyItemChanged(Indexx);
        }
        public void SecimYapScrollsuz(int position)
        {
            //GelenBase.MarkerSec(position);
        }

        public void ListeyiYenile(List<NearbyUserCoordinate> YeniModel)
        {
            
            MapDataModel1_KLON = YeniModel;
            MapDataModel1 = YeniModel.FindAll(item => item.IsShow == true);
            //mRecyclerView.HasFixedSize = true;
            //mLayoutManager = new LinearLayoutManager(this.Activity);
            //mRecyclerView.SetLayoutManager(mLayoutManager);
            //mViewAdapter = new AnaMainRecyclerViewAdapter(this, (Android.Support.V7.App.AppCompatActivity)this.Activity);
            //mRecyclerView.SetAdapter(mViewAdapter);
            //mViewAdapter.ItemClick += MViewAdapter_ItemClick;
            //mLayoutManager = new LinearLayoutManager(Activity, LinearLayoutManager.Horizontal, false);
            //mRecyclerView.SetLayoutManager(mLayoutManager);
            //if (ScrollDinleyici != null)
            //{
            //    mRecyclerView.RemoveOnScrollListener(ScrollDinleyici);
            //    ScrollDinleyici = new HaritaListeRecyclerViewOnScrollListener(mLayoutManager, this);
            //    mRecyclerView.AddOnScrollListener(ScrollDinleyici);
            //}
            try
            {
                mViewAdapter.NotifyDataSetChanged();
            }
            catch
            {
            }

        }
    }

    class HaritaListeRecyclerViewOnScrollListener : RecyclerView.OnScrollListener
    {
        int mLastFirstVisibleItem = 0;
        private LinearLayoutManager mLinearLayoutManager;
        HaritaListeFragment GelenBase;
        public HaritaListeRecyclerViewOnScrollListener(LinearLayoutManager layoutManager, HaritaListeFragment Base)
        {
            mLinearLayoutManager = layoutManager;
            GelenBase = Base;
        }

        public override void OnScrollStateChanged(RecyclerView recyclerView, int newState)
        {
            base.OnScrollStateChanged(recyclerView, newState);
        }

        public override void OnScrolled(RecyclerView recyclerView, int dx, int dy)
        {
            base.OnScrolled(recyclerView, dx, dy);
            int currentFirstVisibleItem = mLinearLayoutManager.FindFirstVisibleItemPosition();
            this.mLastFirstVisibleItem = currentFirstVisibleItem;
            Console.WriteLine("-------------------------- " + dx.ToString());
        }
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
        public string email_verified_at { get; set; }
        public int status { get; set; }
        public string package { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
        public UserPrivacy userPrivacy { get; set; }
    }

    public class NearbyUserCoordinate
    {
        public User user { get; set; }
        public double lat { get; set; }
        public double lon { get; set; }
        public int userDistance { get; set; }

        //Custom Property
        public bool Secim { get; set; }
        public bool IsShow { get; set; }
        public Marker IlgiliMarker { get; set; }

    }

    public class Cevredeki_Kiseler_RootObject
    {
        public int status { get; set; }
        public string message { get; set; }
        public List<NearbyUserCoordinate> nearbyUserCoordinates { get; set; }
    }





}