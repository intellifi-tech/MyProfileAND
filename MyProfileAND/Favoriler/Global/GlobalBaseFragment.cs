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
using MyProfileAND.WebServiceHelper;
using static MyProfileAND.Favoriler.EtkinlikDetay.EtkinlikDetayBaseActivity;

namespace MyProfileAND.Favoriler.Global
{
    public class GlobalBaseFragment : Android.Support.V4.App.Fragment
    {
        #region Tanımlamalar
        RecyclerView mRecyclerView;
        RecyclerView.LayoutManager mLayoutManager;
        GlobalRecyclerViewAdapter mViewAdapter;
        // List<Event> FavorilerRecyclerViewDataModel1;

        List<GlobalRecyclerViewDataModel> globalRecyclerViewDataModels = new List<GlobalRecyclerViewDataModel>();
        #endregion
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            View RootView = inflater.Inflate(Resource.Layout.GlobalBaseFragment, container, false);
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
            globalRecyclerViewDataModels = new List<GlobalRecyclerViewDataModel>();
            WebService webService = new WebService();
            var Donus = webService.OkuGetir("event/index");
            if (Donus != null)
            {
                var a = Donus.ToString();
                var DonusModel = Newtonsoft.Json.JsonConvert.DeserializeObject<GlobalRecyclerViewDataModel_For_JSON>(Donus.ToString());
                if (DonusModel.status == 200)
                {
                    var FavorilerRecyclerViewDataModel1 = DonusModel.events;
                    if (FavorilerRecyclerViewDataModel1.Count > 0)
                    {
                       
                        for (int i = 0; i < DonusModel.events.Count; i++)
                        {
                            for (int i2 = 0; i2 < DonusModel.events[i].user_attended_event.Count; i2++)
                            {
                                globalRecyclerViewDataModels.Add(new GlobalRecyclerViewDataModel() {
                                    Events = DonusModel.events[i],
                                    user_attended_event = DonusModel.events[i].user_attended_event[i2]
                                });
                            }
                        }

                        var aa = globalRecyclerViewDataModels;
                        globalRecyclerViewDataModels = globalRecyclerViewDataModels.OrderBy(x => x.user_attended_event.created_at).ToList();
                        globalRecyclerViewDataModels.Reverse();

                        mViewAdapter = new GlobalRecyclerViewAdapter(globalRecyclerViewDataModels, (Android.Support.V7.App.AppCompatActivity)this.Activity);
                        mRecyclerView.SetAdapter(mViewAdapter);
                        mViewAdapter.ItemClick += MViewAdapter_ItemClick;
                        var layoutManager = new LinearLayoutManager(Activity);
                        mRecyclerView.SetLayoutManager(layoutManager);
                    }
                }
            }

        }

        private void MViewAdapter_ItemClick(object sender, int e)
        {
            SecilenEtkinlik.EtkinlikID = globalRecyclerViewDataModels[e].user_attended_event.event_id.ToString();
            this.Activity.StartActivity(typeof(EtkinlikDetayBaseActivity));
        }
    }
}