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
using MyProfileAND.Favoriler.Takipciler;
using MyProfileAND.WebServiceHelper;

namespace MyProfileAND.Favoriler.TakipEttiklerim
{
    public class FavorilerTakipEttiklerimBaseFragment : Android.Support.V4.App.Fragment
    {
        #region Tanimlamalar
        RecyclerView mRecyclerView;
        RecyclerView.LayoutManager mLayoutManager;
        FavorilerTakipEttiklerimRecyclerViewAdapter mViewAdapter;
        List<Following> TakipEttiklerimDataModel1 = new List<Following>();
        LinearLayout MasterLayot;
        TakipcilerBaseFragment GelenBase;
        #endregion
        public FavorilerTakipEttiklerimBaseFragment(TakipcilerBaseFragment Basee)
        {
            GelenBase = Basee;
        }
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }
       
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            View rootview = inflater.Inflate(Resource.Layout.FavorilerTakipEttiklerimBaseFragment, container, false);
            mRecyclerView = rootview.FindViewById<RecyclerView>(Resource.Id.recyclerView1);
            MasterLayot = rootview.FindViewById<LinearLayout>(Resource.Id.rootView);
            TakipcileriGetir();
            return rootview;
        }

        List<Menu> ListelemeIcinMenuler = new List<Menu>();
        void TakipcileriGetir()
        {
            TakipEttiklerimDataModel1 = new List<Following>();

            WebService webService = new WebService();
            var Donus = webService.ServisIslem("user/followings","");
            if (Donus != "Hata")
            {
                try
                {
                    var DonusModel = Newtonsoft.Json.JsonConvert.DeserializeObject<List<TakipEttiklerimDataModel>>(Donus);
                    TakipEttiklerimDataModel1 = DonusModel[0].followings;
                    if (TakipEttiklerimDataModel1.Count > 0)
                    {
                        mRecyclerView.HasFixedSize = true;
                        mLayoutManager = new LinearLayoutManager(this.Activity);
                        mRecyclerView.SetLayoutManager(mLayoutManager);

                        mViewAdapter = new FavorilerTakipEttiklerimRecyclerViewAdapter(this, (Android.Support.V7.App.AppCompatActivity)this.Activity, TakipEttiklerimDataModel1);
                        mRecyclerView.SetAdapter(mViewAdapter);
                        mViewAdapter.ItemClick += MViewAdapter_ItemClick;
                        mLayoutManager = new LinearLayoutManager(Activity, LinearLayoutManager.Horizontal, false);
                        mRecyclerView.SetLayoutManager(mLayoutManager);
                    }
                    else
                    {
                        GelenBase.TakipEdilenKimseYok();
                    }
                }
                catch 
                {
                }
                
            }
            else
            {
                GelenBase.TakipEdilenKimseYok();
            }
        }

        private void MViewAdapter_ItemClick(object sender, int e)
        {
            GelenBase.KisiselEventAc(TakipEttiklerimDataModel1[e].id);

            //this.Activity.StartActivity(typeof(KisiBazliListeBaseActivity));
            //TiklananMember.TiklananMemberId= TakipEttiklerimDataModel1[e].member_id.ToString();
            //TiklananMember.TiklananMemberName = TakipEttiklerimDataModel1[e].name.ToString();
            //this.Activity.StartActivity(typeof(KisiBazliListeBaseActivity));
        }
    }


    public class Following
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
        public int status { get; set; }
        public string package { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
    }

    public class TakipEttiklerimDataModel
    {
        public int id { get; set; }
        public int from_user_id { get; set; }
        public int to_user_id { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
        public List<Following> followings { get; set; }
    }


}