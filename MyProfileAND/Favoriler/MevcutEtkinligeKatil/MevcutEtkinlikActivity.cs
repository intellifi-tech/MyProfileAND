using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Location;
using Android.Graphics;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Content;
using Android.Text;
using Android.Text.Style;
using Android.Views;
using Android.Widget;
using FFImageLoading;
using FFImageLoading.Transformations;
using FFImageLoading.Views;
using MyProfileAND.DataBasee;
using MyProfileAND.Favoriler.YeniEtkinlikOlustur;
using MyProfileAND.GenericClass;
using MyProfileAND.GenericUI;
using MyProfileAND.WebServiceHelper;
using Newtonsoft.Json;

namespace MyProfileAND.Favoriler.MevcutEtkinligeKatil
{
    [Activity(Label = "MyProfile")]
    public class MevcutEtkinlikActivity : Android.Support.V7.App.AppCompatActivity
    {

        #region Tanimlşamalr
        ImageButton YeniEkle;
        Android.Support.V7.Widget.Toolbar toolbar;





        ListView BulunanEtkinliklerList;
        EventListAdapter mAdapter;
        //BulunanEventlerRootModel BulunanEventlerRootModel1;
        List<NearbyEvent> searchedMenus = new List<NearbyEvent>();




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
            BulunanEtkinliklerList = FindViewById<ListView>(Resource.Id.listView1);
            BulunanEtkinliklerList.ItemClick += BulunanEtkinliklerList_ItemClick;
            BulunanEtkinliklerList.Visibility = ViewStates.Visible;
            YeniEkle = FindViewById<ImageButton>(Resource.Id.ımageButton2);
            YeniEkle.Click += YeniEkle_Click;
            toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.app_bar);

            SetSupportActionBar(toolbar);
            if (SupportActionBar != null)
            {
                SupportActionBar.SetDisplayHomeAsUpEnabled(true);
                SupportActionBar.Title = "";
                toolbar.NavigationIcon.SetColorFilter(Android.Graphics.Color.Black, PorterDuff.Mode.SrcAtop);
            }
        }
        private void NextButton_Click(object sender, EventArgs e)
        {
        }
        private void YeniEkle_Click(object sender, EventArgs e)
        {
            var YeniEtkinlikDialogFragment1 = new YeniEtkinlikDialogFragment(this);
            YeniEtkinlikDialogFragment1.Show(this.SupportFragmentManager, "YeniEtkinlikDialogFragment1");
        }
        private void BulunanEtkinliklerList_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            searchedMenus.ForEach(x => x.isSelect = false);
            searchedMenus[e.Position].isSelect = true;
            DevamEt();
        }
        private void AraEditText_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
        }
        protected override void OnStart()
        {
            base.OnStart();
            KonumSor();
        }

        #region Konum Islemleri
        void KonumSor()
        {
            if (ContextCompat.CheckSelfPermission(this, Android.Manifest.Permission.AccessFineLocation) == Permission.Granted)
            {
                BuildLocationRequest();
                LocationCallBack();
                FusedLocationProviderClient1 = LocationServices.GetFusedLocationProviderClient(this);
                if (KonumKontrol())
                {
                    BekletVeUygula();
                }
            }
            else
            {
                RequestPermissions(new String[] { Android.Manifest.Permission.AccessFineLocation }, 1);
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
        protected override void OnResume()
        {
            base.OnResume();
            if (FusedLocationProviderClient1 != null)
            {
                FusedLocationProviderClient1.RequestLocationUpdates(LocationRequest1, LocationCallback1, Looper.MyLooper());

            }
        }
        internal class MyLocationCallBack : LocationCallback
        {
            private MevcutEtkinlikActivity AnaSayfaBaseFragment1;

            public MyLocationCallBack(MevcutEtkinlikActivity AnaSayfaBaseFragment2)
            {
                AnaSayfaBaseFragment1 = AnaSayfaBaseFragment2;
            }

            public override void OnLocationResult(LocationResult result)
            {
                base.OnLocationResult(result);
                StartLocationCall.UserLastLocation = result.LastLocation;
            }
        }
        bool KonumKontrol()
        {
            bool gps_enabled = false;
            bool network_enabled = false;
            Android.Locations.LocationManager locationManager = (Android.Locations.LocationManager)this.GetSystemService(Context.LocationService);

            try
            {
                gps_enabled = locationManager.IsProviderEnabled(Android.Locations.LocationManager.GpsProvider);
            }
            catch { }
            try
            {
                network_enabled = locationManager.IsProviderEnabled(Android.Locations.LocationManager.NetworkProvider);
            }
            catch { }

            if (!gps_enabled && !network_enabled)
            {


                AlertDialog.Builder cevap = new AlertDialog.Builder(this);
                cevap.SetCancelable(false);
                cevap.SetIcon(Resource.Mipmap.ic_launcher_round);
                cevap.SetTitle(Spannla(Color.Black, "MyProfil"));
                cevap.SetMessage(Spannla(Color.DarkGray, "Yeni etkinlik oluşturabilmek veya etkinlik katılımı yapabilmek için cihaz konumunuz aktif olmalıdır. Konumu aktif etmek ister misiniz?"));
                cevap.SetPositiveButton("Evet", delegate
                {

                    //LocationSettingsRequest.Builder builder = new LocationSettingsRequest.Builder().AddLocationRequest(LocationRequest1);
                    StartActivity(new Intent(Android.Provider.Settings.ActionLocationSourceSettings));
                    cevap.Dispose();
                });
                //cevap.SetNegativeButton("Hayır", delegate
                //{

                //    cevap.Dispose();
                //});
                cevap.Show();

                return false;
            }
            else
            {
                return true;
            }
        }
        SpannableStringBuilder Spannla(Color Renk, string textt)
        {
            ForegroundColorSpan foregroundColorSpan = new ForegroundColorSpan(Renk);

            string title = textt;
            SpannableStringBuilder ssBuilder = new SpannableStringBuilder(title);
            ssBuilder.SetSpan(
                    foregroundColorSpan,
                    0,
                    title.Length,
                    SpanTypes.ExclusiveExclusive
            );

            return ssBuilder;
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            if (permissions.Length == 1 &&
           permissions[0] == Android.Manifest.Permission.AccessFineLocation && grantResults[0] == Permission.Granted)
            {
                if (ContextCompat.CheckSelfPermission(this, Android.Manifest.Permission.AccessFineLocation) != Permission.Granted && ContextCompat.CheckSelfPermission(this, Android.Manifest.Permission.AccessFineLocation) != Permission.Granted)
                {
                    return;
                }
                BuildLocationRequest();
                LocationCallBack();
                FusedLocationProviderClient1 = LocationServices.GetFusedLocationProviderClient(this);
                if (KonumKontrol())
                {
                    BekletVeUygula();
                }
            }
            else
            {
                if (KonumKontrol())
                {
                    BekletVeUygula();
                }
            }
        }
        async void BekletVeUygula()
        {
            await Task.Run(async delegate () {
            Atla:
                await Task.Delay(500);
                if (StartLocationCall.UserLastLocation == null)
                {
                    goto Atla;
                }
                else
                {
                    var aa = StartLocationCall.UserLastLocation;
                    GetAllEvents();
                }
            });
        }

        #endregion



        #region Event Islemleri
        void GetAllEvents()
        {
            WebService webService = new WebService();
            YakinimdakilerSearchDTO yakinimdakilerSearchDTO = new YakinimdakilerSearchDTO()
            {
                latitude = StartLocationCall.UserLastLocation.Latitude.ToString(System.Globalization.CultureInfo.InvariantCulture),
                longitude = StartLocationCall.UserLastLocation.Longitude.ToString(System.Globalization.CultureInfo.InvariantCulture),
                meterLimit = 100
            };
            var jsonstring = JsonConvert.SerializeObject(yakinimdakilerSearchDTO);
            var Donus = webService.ServisIslem("event/nearbyEvents", jsonstring);
            if (Donus != "Hata")
            {
                var durumm = Donus.ToString();
                var GelenEtkinler = Newtonsoft.Json.JsonConvert.DeserializeObject<RootObject_Events>(Donus.ToString().Replace(",0", ""));
                if (GelenEtkinler.status == 200)
                {
                    searchedMenus = GelenEtkinler.nearbyEvent;
                    mAdapter = new EventListAdapter(this, Resource.Layout.BulunanMekanlarCustomListView, searchedMenus);
                    try
                    {
                        RunOnUiThread(delegate ()
                        {
                            BulunanEtkinliklerList.Adapter = mAdapter;
                        });
                    }
                    catch (Exception exx)
                    {
                        Console.WriteLine("==========>>>>>>>  " + exx.Message);
                    }
                }
            }
        }
        #endregion


        public void DevamEt(string Titlee = "")
        {
            var SecimVasrmi = searchedMenus.FindAll(item => item.isSelect == true);
            if (SecimVasrmi.Count > 0)
            {
                SecilenEventt.EventID = SecimVasrmi[0].eventt.id;
                SecilenEventt.EventTitle = SecimVasrmi[0].eventt.title;
                SecilenEventt.Konum = StartLocationCall.UserLastLocation;
                StartActivity(typeof(YeniEtkinlikOlusturBaseActivity));
                this.Finish();
            }
            else
            {
                if (Titlee != "")
                {
                    if (StartLocationCall.UserLastLocation != null)
                    {
                        SecilenEventt.EventID = 0;
                        SecilenEventt.EventTitle = Titlee;
                        SecilenEventt.Konum = StartLocationCall.UserLastLocation;
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

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    //_timer.Dispose();
                    this.Finish();
                    return true;
                default:
                    return base.OnOptionsItemSelected(item);
            }
        }

        #region DataModels

        public class YakinimdakilerSearchDTO
        {
            public string latitude { get; set; }
            public string longitude { get; set; }
            public int meterLimit { get; set; }
        }



        public class Event
        {
            public int id { get; set; }
            public string title { get; set; }
            public double latitude { get; set; }
            public double longitude { get; set; }
            public string created_at { get; set; }
            public string updated_at { get; set; }
        }

        public class NearbyEvent
        {
            [JsonProperty("event")]
            public Event eventt { get; set; }
            public double lat { get; set; }
            public double lon { get; set; }
            public int eventDistance { get; set; }
            //CustomClass
            public bool isSelect { get; set; }
        }

        public class RootObject_Events
        {
            public int status { get; set; }
            public string message { get; set; }
            public List<NearbyEvent> nearbyEvent { get; set; }
        }

        #endregion



        #region ListView Adaper
        class EventListAdapter : BaseAdapter<NearbyEvent>
        {
            private Activity mContext;
            private int mRowLayout;
            public List<NearbyEvent> mDepartmanlar;

            public EventListAdapter(Activity context, int rowLayout, List<NearbyEvent> friends)
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
                    holder.EventTitle.Text = item.eventt.title;
                    // holder.EventUserCount.Text = item.eventt.user_attended_events_count.ToString() + " Kişi bu etkinliğe katıldı.";
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
                    GetEventInfoo(item.eventt.id.ToString(), holder.EventTitle, holder.EventUserCount, holder.EventImage);

                    row.Tag = holder;
                }
                return row;
            }

            void GetEventInfoo(string EventID, TextView EtkinlikAdii, TextView EtkinlikKatilimci, ImageViewAsync EtkinlikImg)
            {

                new System.Threading.Thread(new System.Threading.ThreadStart(delegate
                {
                    WebService webService = new WebService();
                    var Donuss = webService.OkuGetir("event/" + EventID + "/show");
                    if (Donuss != null)
                    {
                        try
                        {
                            var aaaa = Donuss.ToString();
                            var Detayy = Newtonsoft.Json.JsonConvert.DeserializeObject<RootObject2>(aaaa);
                            if (Detayy.user_attended_event.Count > 0)
                            {
                                mContext.RunOnUiThread(delegate ()
                                {
                                    EtkinlikAdii.Text = Detayy.title;
                                    ImageService.Instance.LoadUrl("http://23.97.222.30"+Detayy.user_attended_event[0].event_image)
                                                        .Transform(new CircleTransformation(15, "#FFFFFF"))
                                                        .Into(EtkinlikImg);
                                    EtkinlikKatilimci.Text = Detayy.user_attended_event.Count + " Kişi bu etkinliğe katıldı.";
                                });
                            }
                        }
                        catch
                        {
                            mContext.RunOnUiThread(delegate ()
                            {
                                EtkinlikAdii.Text = "-";
                                EtkinlikKatilimci.Text = "-";
                            });
                        }
                    }
                })).Start();
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
            public class RootObject2
            {
                public int id { get; set; }
                public string title { get; set; }
                public double latitude { get; set; }
                public double longitude { get; set; }
                public string created_at { get; set; }
                public string updated_at { get; set; }
                public List<UserAttendedEvent> user_attended_event { get; set; }
            }
        }
        #endregion

    }

    public static class StartLocationCall
    {
        public static Android.Locations.Location UserLastLocation { get; set; }
    }

    public static class SecilenEventt
    {
        public static int EventID { get; set; }
        public static string EventTitle { get; set; }
        public static Location Konum { get; set; }
    }
}