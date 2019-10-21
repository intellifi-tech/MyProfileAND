using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Location;
using Android.Graphics;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Content;
using Android.Views;
using Android.Widget;
using FFImageLoading;
using FFImageLoading.Transformations;
using FFImageLoading.Views;
using MyProfileAND.GenericClass;
using MyProfileAND.GenericUI;
using MyProfileAND.WebServiceHelper;
using Newtonsoft.Json;

namespace MyProfileAND.Favoriler.YeniEtkinlikOlustur
{
    [Activity(Label = "MyProfile")]
    public class YeniEtkinlikOlusturBaseActivity0 : Android.Support.V7.App.AppCompatActivity
    {

        #region Tanimlşamalr
        EditText AraEditText;
        ImageButton YeniEkle;
        Android.Support.V7.Widget.Toolbar toolbar;


        


        ListView BulunanEtkinliklerList;
        EventListAdapter mAdapter;
        BulunanEventlerRootModel BulunanEventlerRootModel1;
        List<NearbyEvent> searchedMenus = new List<NearbyEvent>();


        ImageButton NextButton;
        TextView InformationText;


        CustomLoadingScreen CustomLoadingScreen1;
        FusedLocationProviderClient FusedLocationProviderClient1;
        LocationCallback LocationCallback1;
        LocationRequest LocationRequest1;
        #endregion
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.YeniEtkinlikOlusturBaseActivity0);
            DinamikStatusBarColor DinamikStatusBarColor1 = new DinamikStatusBarColor();
            DinamikStatusBarColor1.Beyaz(this);
            AraEditText = FindViewById<EditText>(Resource.Id.aratext);
            InformationText = FindViewById<TextView>(Resource.Id.textView2);
            AraEditText.TextChanged += AraEditText_TextChanged;
            BulunanEtkinliklerList = FindViewById<ListView>(Resource.Id.listView1);
            BulunanEtkinliklerList.ItemClick += BulunanEtkinliklerList_ItemClick;
            NextButton = FindViewById<ImageButton>(Resource.Id.ımageButton1);
            YeniEkle = FindViewById<ImageButton>(Resource.Id.ımageButton2);
            YeniEkle.Click += YeniEkle_Click;
            NextButton.Click += NextButton_Click;
            toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.app_bar);

            SetSupportActionBar(toolbar);
            if (SupportActionBar != null)
            {
                SupportActionBar.SetDisplayHomeAsUpEnabled(true);
                SupportActionBar.Title = "";
                toolbar.NavigationIcon.SetColorFilter(Android.Graphics.Color.Black, PorterDuff.Mode.SrcAtop);
            }
        }

      

        private void YeniEkle_Click(object sender, EventArgs e)
        {
            var YeniEtkinlikDialogFragment1 = new YeniEtkinlikDialogFragment(this);
            YeniEtkinlikDialogFragment1.Show(this.SupportFragmentManager, "YeniEtkinlikDialogFragment1");
        }


        private void NextButton_Click(object sender, EventArgs e)
        {
            DevamEt();
        }

        public void DevamEt(string Titlee="")
        {
            var SecimVasrmi = searchedMenus.FindAll(item => item.isSelect == true);
            if (SecimVasrmi.Count > 0)
            {
                SecilenEventt.EventID = SecimVasrmi[0].Event.id;
                SecilenEventt.EventTitle = SecimVasrmi[0].Event.title;
                SecilenEventt.Konum = sonLokasyon;
                StartActivity(typeof(YeniEtkinlikOlusturBaseActivity));
                this.Finish();
            }
            else
            {
                if (Titlee != "")
                {
                    if (sonLokasyon != null)
                    {
                        SecilenEventt.EventID = 0;
                        SecilenEventt.EventTitle = Titlee;
                        SecilenEventt.Konum = sonLokasyon;
                        StartActivity(typeof(YeniEtkinlikOlusturBaseActivity));
                        this.Finish();
                    }
                    else
                    {
                        AlertHelper.AlertGoster("Konumuza ulaşamıyoruz. Konum izni verdiğinize ve konumunuzun açık olduğuna emin olun.", this);
                        return;
                    }
                   
                }
                else
                {
                    AlertHelper.AlertGoster("Lütfen bir etkinlik adı yazın.", this);
                    return;
                }
            }
        }

        private void BulunanEtkinliklerList_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var pos = BulunanEtkinliklerList.ScrollY;
            searchedMenus.ForEach(x => x.isSelect = false);
            searchedMenus[e.Position].isSelect = true;

            mAdapter = new EventListAdapter(this, Resource.Layout.BulunanMekanlarCustomListView, searchedMenus);
            BulunanEtkinliklerList.Adapter = mAdapter;
            BulunanEtkinliklerList.ScrollY = pos;
        }

        private void AraEditText_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            #region Search
            if (AraEditText.Text.Trim() == "")
            {
                BulunanEtkinliklerList.Adapter = null;
                InformationText.Visibility = ViewStates.Visible;
                BulunanEtkinliklerList.Visibility = ViewStates.Gone;
                return;
            }

            if (BulunanEventlerRootModel1 != null)
            {
                 searchedMenus = (from menuu in BulunanEventlerRootModel1.nearbyEvent
                                                   where
                                                    menuu.Event.title.Contains(AraEditText.Text, StringComparison.OrdinalIgnoreCase)
                                                   select
                                                 menuu).ToList();
                this.RunOnUiThread(() =>
                {

                    mAdapter = new EventListAdapter(this, Resource.Layout.BulunanMekanlarCustomListView, searchedMenus);
                    try
                    {
                        BulunanEtkinliklerList.Adapter = mAdapter;
                    }
                    catch (Exception exx)
                    {
                        Console.WriteLine("==========>>>>>>>  " + exx.Message);
                    }
                    
                });
            }
            #endregion

            new System.Threading.Thread(new System.Threading.ThreadStart(delegate
            {
                try
                {
                    if (sonLokasyon != null)
                    {
                        WebService webService = new WebService();
                        EventAraDataModel eventAraDataModel = new EventAraDataModel()
                        {
                            latitude = sonLokasyon.Latitude.ToString(System.Globalization.CultureInfo.InvariantCulture),
                            longitude = sonLokasyon.Longitude.ToString(System.Globalization.CultureInfo.InvariantCulture),
                            meterLimit = "100",
                            title = AraEditText.Text
                        };
                        var jsonstring = JsonConvert.SerializeObject(eventAraDataModel);
                        var Donus = webService.ServisIslem("event/search", jsonstring);
                        if (Donus != "Hata")
                        {
                            var NewModel = Newtonsoft.Json.JsonConvert.DeserializeObject<BulunanEventlerRootModel>(Donus);
                            if (NewModel.status == 200)
                            {
                                if (BulunanEventlerRootModel1 != NewModel)
                                {
                                    BulunanEventlerRootModel1 = NewModel;

                                    this.RunOnUiThread(() => {
                                        InformationText.Visibility = ViewStates.Gone;
                                        BulunanEtkinliklerList.Visibility = ViewStates.Visible;
                                    });
                                    
                                }
                            }
                            else
                            {
                                this.RunOnUiThread(() => {

                                    InformationText.Visibility = ViewStates.Visible;
                                    BulunanEtkinliklerList.Visibility = ViewStates.Gone;
                                });
                            }
                        }
                        else
                        {
                            this.RunOnUiThread(() => {

                                InformationText.Visibility = ViewStates.Visible;
                                BulunanEtkinliklerList.Visibility = ViewStates.Gone;
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    string aa = ex.Message;
                   // BulunanEtkinliklerList.Adapter = null;
                }
            })).Start();
        }

        void OpenLoading()
        {
            CustomLoadingScreen1 = new CustomLoadingScreen("Konumunuz Alıyor...");
            CustomLoadingScreen1.Cancelable = false; 
            CustomLoadingScreen1.Show(this.SupportFragmentManager, "CustomLoadingScreen1");
        }

        #region  Life
        protected override void OnResume()
        {
            base.OnResume();
            KonumGetir();
        }

        protected override void OnPause()
        {
            base.OnPause();
         
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

        #endregion

        #region LocationManager
        void KonumGetir()
        {

            if (ContextCompat.CheckSelfPermission(this, Android.Manifest.Permission.AccessFineLocation) == Permission.Granted)
            {

                BuildLocationRequest();
                LocationCallBack();
                FusedLocationProviderClient1 = LocationServices.GetFusedLocationProviderClient(this);
                KonumKontrol();
                OpenLoading();

                if (FusedLocationProviderClient1 != null)
                {
                    FusedLocationProviderClient1.RequestLocationUpdates(LocationRequest1, LocationCallback1, Looper.MyLooper());
                }
            }
            else
            {
                RequestPermissions(new String[] { Android.Manifest.Permission.AccessFineLocation }, 1);
            }
        }
        Android.Locations.Location sonLokasyon = null;
        public void GelenKonumuYansit(Android.Locations.Location gelenloc)
        {
            try
            {
                if (CustomLoadingScreen1 != null)
                {
                    CustomLoadingScreen1.Dismiss();
                    CustomLoadingScreen1 = null;
                }
            }
            catch
            {
            }
            if (sonLokasyon == null)
            {
                sonLokasyon = gelenloc;
            }

        }

        void LocationCallBack()
        {
            LocationCallback1 = new MyLocationCallBack(this);
        }
        void BuildLocationRequest()
        {
            LocationRequest1 = new LocationRequest();
            LocationRequest1.SetPriority(LocationRequest.PriorityHighAccuracy);
            LocationRequest1.SetInterval(5000);
            LocationRequest1.SetFastestInterval(3000);
            LocationRequest1.SetSmallestDisplacement(10f);
        }

        internal class MyLocationCallBack : LocationCallback
        {
            private YeniEtkinlikOlusturBaseActivity0 YeniEtkinlikOlusturBaseActivity01;

            public MyLocationCallBack(YeniEtkinlikOlusturBaseActivity0 YeniEtkinlikOlusturBaseActivity02)
            {
                YeniEtkinlikOlusturBaseActivity01 = YeniEtkinlikOlusturBaseActivity02;
            }

            public override void OnLocationResult(LocationResult result)
            {
                base.OnLocationResult(result);
                YeniEtkinlikOlusturBaseActivity01.GelenKonumuYansit(result.LastLocation);
            }
        }
        bool KonumKontrol()
        {
            bool gps_enabled = false;
            bool network_enabled = false;
            return true;
            /*
            try
            {
                gps_enabled = locationManager.IsProviderEnabled(LocationManager.GpsProvider);
            }
            catch { }
            try
            {
                network_enabled = locationManager.IsProviderEnabled(LocationManager.NetworkProvider);
            }
            catch { }

            if (!gps_enabled && !network_enabled)
            {

                AlertHelper.AlertGoster("Lütfen konum ayarlarınızı açın.", this.Activity);
                return false;
            }
            else
            {
                return true;
            }*/
        }

        #endregion

        #region Permission
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            if (permissions.Length == 1 &&
               permissions[0] == Android.Manifest.Permission.AccessFineLocation &&
               grantResults[0] == Permission.Granted)
            {
                if (ContextCompat.CheckSelfPermission(this, Android.Manifest.Permission.AccessFineLocation) != Permission.Granted && ContextCompat.CheckSelfPermission(this, Android.Manifest.Permission.AccessFineLocation) != Permission.Granted)
                {
                    // TODO: Consider calling
                    //    ActivityCompat#requestPermissions
                    // here to request the missing permissions, and then overriding
                    //   public void onRequestPermissionsResult(int requestCode, String[] permissions,
                    //                                          int[] grantResults)
                    // to handle the case where the user grants the permission. See the documentation
                    // for ActivityCompat#requestPermissions for more details.
                    return;
                }
                BuildLocationRequest();
                LocationCallBack();
                FusedLocationProviderClient1 = LocationServices.GetFusedLocationProviderClient(this);
                KonumKontrol();
                OpenLoading();

                if (FusedLocationProviderClient1 != null)
                {
                    FusedLocationProviderClient1.RequestLocationUpdates(LocationRequest1, LocationCallback1, Looper.MyLooper());
                }
            }
            else
            {
                // Permission was denied. Display an error message.
            }
        }
        #endregion

        #region DataModels
        public class EventAraDataModel
        {
            public string title { get; set; }
            public string meterLimit { get; set; }
            public string latitude { get; set; }
            public string longitude { get; set; }
        }


        public class UserAttendedEvent
        {
            public int id { get; set; }
            public int event_id { get; set; }
            public int user_id { get; set; }
            public string event_description { get; set; }
            public string event_image { get; set; }
            public string date_of_participation { get; set; }
            public string end_date { get; set; }
            public int rating { get; set; }
            public string created_at { get; set; }
            public string updated_at { get; set; }
        }

        public class Event
        {
            public int id { get; set; }
            public string title { get; set; }
            public double latitude { get; set; }
            public double longitude { get; set; }
            public string created_at { get; set; }
            public string updated_at { get; set; }
            public int user_attended_events_count { get; set; }
            public List<UserAttendedEvent> user_attended_event { get; set; }
        }

        public class NearbyEvent
        {
            [JsonProperty("event")]
            public Event Event { get; set; }
            public double lat { get; set; }
            public double lon { get; set; }
            public int eventDistance { get; set; }

            //Custom Propery
            public bool isSelect { get; set; }
        }

        public class BulunanEventlerRootModel
        {
            public int status { get; set; }
            public string message { get; set; }
            public List<NearbyEvent> nearbyEvent { get; set; }
        }







        #endregion

        #region ListView Adaper
        class EventListAdapter : BaseAdapter<NearbyEvent>
        {
            private Context mContext;
            private int mRowLayout;
            public List<NearbyEvent> mDepartmanlar;

            public EventListAdapter(Context context, int rowLayout, List<NearbyEvent> friends)
            {
                mContext = context;
                mRowLayout = rowLayout;
                mDepartmanlar = friends;

            }

            public override int ViewTypeCount
            {
                get
                {
                    return Count;
                }
            }
            public override int GetItemViewType(int position)
            {
                return position;
            }

            public override int Count
            {
                get { return mDepartmanlar.Count; }
            }

            public override NearbyEvent this[int position]
            {
                get { return mDepartmanlar[position]; }
            }

            public override long GetItemId(int position)
            {
                return position;
            }
            protected override void Dispose(bool disposing)
            {
                base.Dispose(disposing);

            }
            public override View GetView(int position, View convertView, ViewGroup parent)
            {
                ListeHolder holder;

                View row = convertView;


                if (row != null)
                {
                    holder = row.Tag as ListeHolder;
                }
                else //(row2 == null) **
                {
                    holder = new ListeHolder();
                    row = LayoutInflater.From(mContext).Inflate(mRowLayout, parent, false);
                    var item = mDepartmanlar[position];
                    holder.EventTitle = row.FindViewById<TextView>(Resource.Id.textView1);
                    holder.EventUserCount = row.FindViewById<TextView>(Resource.Id.textView2);
                    holder.EventImage = row.FindViewById<ImageViewAsync>(Resource.Id.imgPortada_item);
                    holder.Tick = row.FindViewById<ImageView>(Resource.Id.imageView1);
                    holder.EventTitle.Text = item.Event.title;
                    holder.EventUserCount.Text = item.Event.user_attended_events_count.ToString() + " Kişi bu etkinliğe katıldı.";

                    ImageService.Instance.LoadUrl(item.Event.user_attended_event[0].event_image)
                                                    .Transform(new CircleTransformation(15, "#FFFFFF"))
                                                    .Into(holder.EventImage);

                    if (item.isSelect)
                    {
                        row.SetBackgroundColor(Color.ParseColor("#10304ffe"));
                        holder.Tick.Visibility = ViewStates.Visible;
                    }
                    else
                    {
                        row.SetBackgroundResource(Resource.Drawable.chatFriendListBackgroundSelector);
                        holder.Tick.Visibility = ViewStates.Invisible;
                    }


                    row.Tag = holder;
                }
                return row;
            }
            public void Reflesh()
            {
                //this.NotifyAll();
                this.NotifyDataSetChanged();
            }
            class ListeHolder : Java.Lang.Object
            {
                public TextView EventTitle { get; set; }
                public TextView EventUserCount { get; set; }
                public ImageViewAsync EventImage { get; set; }
                public ImageView Tick { get; set; }

            }
        }
        #endregion
    }

    public static class SecilenEventt
    {
        public static int EventID { get; set; }
        public static string EventTitle { get; set; }
        public static Location Konum { get; set; }
    }
}