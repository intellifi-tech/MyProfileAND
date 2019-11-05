using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android;
using Android.Animation;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Content;
using Android.Views;
using Android.Widget;
using MyProfileAND.GenericClass;
using Android.Gms.Common;

using Android.Locations;
using MyProfileAND.GenericUI;
using Square.Picasso;
using MyProfileAND.DataBasee;
using MyProfileAND.WebServiceHelper;
using Newtonsoft.Json;
using MyProfileAND.AnaMenu;
using Android.Gms.Location;

namespace MyProfileAND.AnaSayfa
{
    public class AnaSayfaBaseFragment : Android.Support.V4.App.Fragment, IOnMapReadyCallback, GoogleMap.IOnMapLongClickListener, GoogleMap.IOnInfoWindowClickListener, GoogleMap.IOnMarkerClickListener
    {
        #region Tanimlamalar
        private GoogleMap _map;
        private MapFragment _mapFragment;
        FrameLayout ListeHaznesi;
        HaritaListeFragment HaritaListeFragment1;
        List<NearbyUserCoordinate> MapDataModel1 = new List<NearbyUserCoordinate>();
        List<NearbyUserCoordinate> MapDataModel1_KLON = new List<NearbyUserCoordinate>();
        public List<TakipEttiklerim_RootObject> TakipEttiklerimListe = new List<TakipEttiklerim_RootObject>();
        string GelenJsonDataModel;
        ImageButton ListeAcKapat, MyLocationButton;
       
        VerticalSeekBar.VerticalSeekBar SeekBarr;
        TextView MetreText;

        CustomLoadingScreen CustomLoadingScreen1;
        string MeId;

        FusedLocationProviderClient FusedLocationProviderClient1;
        LocationCallback LocationCallback1;
        LocationRequest LocationRequest1;

        #endregion
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View RootView = inflater.Inflate(Resource.Layout.AnaSayfaBaseFragment, container, false);
            ListeHaznesi = RootView.FindViewById<FrameLayout>(Resource.Id.frameLayout2);
            ListeAcKapat = RootView.FindViewById<ImageButton>(Resource.Id.ımageButton1);
            MyLocationButton = RootView.FindViewById<ImageButton>(Resource.Id.ımageButton3);
            ListeAcKapat.Click += ListeAcKapat_Click;
            SeekBarr = RootView.FindViewById<VerticalSeekBar.VerticalSeekBar>(Resource.Id.seekbar1);
            SeekBarr.Min = 10;
            SeekBarr.Max = 500;
            SeekBarr.ProgressChanged += SeekBarr_ProgressChanged;
            SeekBarr.Progress = 100;
            MetreText = RootView.FindViewById<TextView>(Resource.Id.textView1);
            MetreText.Text = string.Format("{0}m", SeekBarr.Progress);
            MyLocationButton.Click += MyLocationButton_Click;
            TakipEttiklerimiGetir();
            if (ContextCompat.CheckSelfPermission(this.Activity, Android.Manifest.Permission.AccessFineLocation) == Permission.Granted)
            {
                BuildLocationRequest();
                LocationCallBack();
                FusedLocationProviderClient1 = LocationServices.GetFusedLocationProviderClient(this.Activity);
                KonumKontrol();
                OpenLoading();
            }
            else
            {
                RequestPermissions(new String[] { Android.Manifest.Permission.AccessFineLocation }, 1);
            }
            MeId = DataBase.USER_INFO_GETIR()[0].id;
            return RootView;
        }
        private void MyLocationButton_Click(object sender, EventArgs e)
        {
            if (LastUserLocation != null)
            {
                if (_map != null)
                {
                    CameraUpdate cameraUpdate = CameraUpdateFactory.NewLatLngZoom(new LatLng(LastUserLocation.Latitude, LastUserLocation.Longitude), 10);
                    _map.MoveCamera(cameraUpdate);
                }
            }
            else
            {
                if (ContextCompat.CheckSelfPermission(this.Activity, Android.Manifest.Permission.AccessFineLocation) == Permission.Granted)
                {
                    //locationManager = (LocationManager)this.Activity.GetSystemService("location");
                    //provider = locationManager.GetBestProvider(new Criteria(), false);
                    //locationManager.RequestLocationUpdates(provider, 1, 0, this);
                    KonumKontrol();
                    OpenLoading();
                }
                else
                {
                    RequestPermissions(new String[] { Android.Manifest.Permission.AccessFineLocation }, 1);
                }
            }
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            if (permissions.Length == 1 &&
           permissions[0] == Android.Manifest.Permission.AccessFineLocation && grantResults[0] == Permission.Granted)
            {
                if (ContextCompat.CheckSelfPermission(this.Activity, Android.Manifest.Permission.AccessFineLocation) != Permission.Granted && ContextCompat.CheckSelfPermission(this.Activity, Android.Manifest.Permission.AccessFineLocation) != Permission.Granted)
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
                FusedLocationProviderClient1 = LocationServices.GetFusedLocationProviderClient(this.Activity);
                KonumKontrol();
                OpenLoading();
            }
            else
            {
                // Permission was denied. Display an error message.
            }
        }
        private void SeekBarr_ProgressChanged(object sender, SeekBar.ProgressChangedEventArgs e)
        {
            if (circleOptions != null)
            {
                circleOptions.Radius = e.Progress / 2;
                MetreText.Text = string.Format("{0}m", e.Progress);
                new System.Threading.Thread(new System.Threading.ThreadStart(delegate
                {
                    try
                    {
                        
                    }
                    catch (Exception ex)
                    {
                        var aa = ex.Message;
                    }
                    CemberIcindekileriAyikla();
                })).Start();
            }
        }
        private void ListeAcKapat_Click(object sender, EventArgs e)
        {
            if (MapDataModel1.FindAll(item => item.IsShow == true).Count > 0)
            {
                AcKapat();
            }
        }
        Intent FilterIntent;
        private void FiltreButton_Click(object sender, EventArgs e)
        {
            //FiltreOkeyButtonClick.listener += FiltreOk_Click;
            //FilterIntent = new Intent(this, typeof(FilterBaseActivity));
            //this.StartActivity(FilterIntent);
        }
        public override void OnStart()
        {
            base.OnStart();
            InitMapFragment(); //Map Ayarlarını yap markerleri datamodele yerleştir
            MapsInitializer.Initialize(this.Activity.ApplicationContext);
        }
        public override void OnResume()
        {
            base.OnResume();
            if (FusedLocationProviderClient1 != null)
            {
                FusedLocationProviderClient1.RequestLocationUpdates(LocationRequest1, LocationCallback1, Looper.MyLooper());

            }

        }
        public override void OnStop()
        {
            base.OnStop();
            if (FusedLocationProviderClient1 != null)
            {
                FusedLocationProviderClient1.RemoveLocationUpdates(LocationCallback1);
            }
        }
        public override void OnPause()
        {
            base.OnPause();
            //if (locationManager != null)
            //{
            //    locationManager.RemoveUpdates(this);
            //}

        }
        void OpenLoading()
        {
            CustomLoadingScreen1 = new CustomLoadingScreen("Konumunuz Alıyor...");
            //CustomLoadingScreen1.Cancelable = false;
            CustomLoadingScreen1.Show(this.Activity.SupportFragmentManager, "CustomLoadingScreen1");
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

        #region TakipEttiklerimiCagir
        public void TakipEttiklerimiGetir()
        {
            new System.Threading.Thread(new System.Threading.ThreadStart(delegate
            {
                try
                {
                    WebService webService = new WebService();
                    var Donus = webService.ServisIslem("user/followings", "");
                    if (Donus != "Hata")
                    {
                        TakipEttiklerimListe = Newtonsoft.Json.JsonConvert.DeserializeObject<List<TakipEttiklerim_RootObject>>(Donus);
                        this.Activity.RunOnUiThread(() => {
                            ListeyiFragmentCagir();
                        });
                    }
                }
                catch
                {
                }
            })).Start();
        }

        #endregion

        #region UserLocationn
        Circle circleOptions;
        Marker UserMarker;
        bool ZoomOption = false;
        Location LastUserLocation;
        float bearing;

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
            private AnaSayfaBaseFragment AnaSayfaBaseFragment1;

            public MyLocationCallBack(AnaSayfaBaseFragment AnaSayfaBaseFragment2)
            {
                AnaSayfaBaseFragment1 = AnaSayfaBaseFragment2;
            }

            public override void OnLocationResult(LocationResult result)
            {
                base.OnLocationResult(result);
                AnaSayfaBaseFragment1.OnLocationChangedd(result.LastLocation);
                Console.WriteLine("Güncellendiiiiiiiiiiiiiii");
                Toast.MakeText(AnaSayfaBaseFragment1.Activity, "Güncellendiiiiiiiiiiiiiii", ToastLength.Long).Show();
            }
        }

        //******************************
        public void OnLocationChangedd(Location location)
        {

            if (MapDataModel1 != null)
            {
                MapDataModel1.ForEach(item =>
                {
                    item.IlgiliMarker.SetIcon(BitmapDescriptorFactory.FromBitmap(new MapUtils().createStoreMarker(this.Activity, item.IsShow, item.user.profile_photo)));
                });
            }

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

            if (UserMarker == null)
            {
                //ShowLoading.Hide();
                Map_Ve_IcerikleriOlustur(new LatLng(location.Latitude, location.Longitude));
                UzakDBLokasyonGuncelle(new LatLng(location.Latitude, location.Longitude));
                if (_map != null)
                {
                    MapUtils mapUtils = new MapUtils();
                    Android.Graphics.Bitmap bitmap = mapUtils.createStoreMarker_Dinamik_View(this.Activity, Resource.Layout.current_location_me);
                    BitmapDescriptor image = BitmapDescriptorFactory.FromBitmap(bitmap);
                    var UserMarker2 = new MarkerOptions();
                    UserMarker2.SetPosition(new LatLng(location.Latitude, location.Longitude));
                    UserMarker2.SetIcon(image);
                    UserMarker = _map.AddMarker(UserMarker2);
                    UserMarker.SetAnchor(0.5f, 0.5f);
                    UserMarker.Flat = true;
                }
            }
            else
            {
                if (_map != null)
                {
                    UserMarker.Position = new LatLng(location.Latitude, location.Longitude);
                    bearing = LastUserLocation.BearingTo(location);
                    UserMarker.Rotation = bearing;
                }
            }

            if (circleOptions == null)
            {
                if (_map != null)
                {
                    var circleOptions2 = new CircleOptions();
                    circleOptions2.InvokeCenter(new LatLng(location.Latitude, location.Longitude));
                    circleOptions2.InvokeStrokeColor(Android.Graphics.Color.ParseColor("#901296DB"));
                    circleOptions2.InvokeStrokeWidth(5);
                    circleOptions2.InvokeFillColor(Android.Graphics.Color.ParseColor("#501296DB"));
                    circleOptions2.InvokeRadius(50);
                    circleOptions = _map.AddCircle(circleOptions2);

                    CameraUpdate cameraUpdate = CameraUpdateFactory.NewLatLngZoom(new LatLng(location.Latitude, location.Longitude), 18);
                    _map.AnimateCamera(cameraUpdate);
                }

            }
            else
            {
                if (_map != null)
                {
                    circleOptions.Center = new LatLng(location.Latitude, location.Longitude);

                    if (_map.CameraPosition.Zoom == 18)
                    {
                        ZoomOption = true;
                    }
                    if (ZoomOption)
                    {
                        CameraUpdate cameraUpdate1 = CameraUpdateFactory.NewLatLng(new LatLng(location.Latitude, location.Longitude));
                        _map.AnimateCamera(cameraUpdate1);
                    }
                    else
                    {
                        CameraUpdate cameraUpdate = CameraUpdateFactory.NewLatLngZoom(new LatLng(location.Latitude, location.Longitude), 18);
                        _map.AnimateCamera(cameraUpdate);
                    }
                }
            }

            LastUserLocation = location;
            if (_map != null)
            {
                CemberIcindekileriAyikla();
            }


        }
        void UzakDBLokasyonGuncelle(LatLng latLng)
        {
            new System.Threading.Thread(new System.Threading.ThreadStart(delegate
            {
                try
                {
                    var _timer = new System.Threading.Timer((o) =>
                    {
                        try
                        {
                            WebService webService = new WebService();
                            KordinatGonder_RootObject KordinatGonder_RootObject1 = new KordinatGonder_RootObject()
                            {
                                latitude = latLng.Latitude.ToString(System.Globalization.CultureInfo.InvariantCulture),
                                longitude = latLng.Longitude.ToString(System.Globalization.CultureInfo.InvariantCulture)
                            };
                            string jsonString = JsonConvert.SerializeObject(KordinatGonder_RootObject1);
                            var Donus = webService.ServisIslem("user/coordinate/setCoordinates", jsonString);
                            //FillDataModel(new LatLng(latLng.Latitude, latLng.Longitude));

                        }
                        catch
                        {


                        }

                    }, null, 0, 60000);
                }
                catch
                {
                }

            })).Start();
        }

        void CemberIcindekileriAyikla()
        {
            this.Activity.RunOnUiThread(() =>
            {
                for (int i = 0; i < MapDataModel1.Count; i++)
                {
                    float[] distance = new float[2];
                    this.Activity.RunOnUiThread(() =>
                    {
                        Location.DistanceBetween(MapDataModel1[i].lat, MapDataModel1[i].lon, circleOptions.Center.Latitude, circleOptions.Center.Longitude, distance);
                        if (distance[0] < circleOptions.Radius)
                        {
                        //insite
                        MapDataModel1[i].IsShow = true;
                            MapDataModel1[i].IlgiliMarker.Visible = MapDataModel1[i].IsShow;
                        }
                        else
                        {
                        //outside
                        try
                            {
                                MapDataModel1[i].IsShow = false;
                                MapDataModel1[i].IlgiliMarker.Visible = MapDataModel1[i].IsShow;
                            }
                            catch
                            {
                            }


                        }
                    });
                }

                HaritaListeFragment1.ListeyiYenile(MapDataModel1);
            });

        }
        void Map_Ve_IcerikleriOlustur(LatLng latLng)
        {
            //ShowLoading.Show(this.Activity,"Konum Bekleniyor...");
            FillDataModel(latLng);
            MapDataModel1_KLON = MapDataModel1;

            if (MapDataModel1.Count <= 0)
            {
                ListeHaznesi.Visibility = ViewStates.Gone;
            }
            else
            {
                ListeHaznesi.Visibility = ViewStates.Visible;
            }
        }
        #endregion

        void FillDataModel(LatLng latLng)
        {
            MapDataModel1 = new List<NearbyUserCoordinate>();

            WebService webService = new WebService();
            YakindakiNearbyUserCoordinate_RootObject KordinatGonder_RootObject1 = new YakindakiNearbyUserCoordinate_RootObject()
            {
                latitude = latLng.Latitude.ToString(System.Globalization.CultureInfo.InvariantCulture),
                longitude = latLng.Longitude.ToString(System.Globalization.CultureInfo.InvariantCulture),
                meterLimit = 500
            };
            string jsonString = JsonConvert.SerializeObject(KordinatGonder_RootObject1);
            var Donus = webService.ServisIslem("user/coordinate/nearbyUsers", jsonString);
            if (Donus != "Hata")
            {
                var DonusModel = Newtonsoft.Json.JsonConvert.DeserializeObject<Cevredeki_Kiseler_RootObject>(Donus);
                if (DonusModel.status == 200)
                {
                    var UserOlanlar = DonusModel.nearbyUserCoordinates.FindAll(item => item.user != null);
                    var BenHaric = UserOlanlar.FindAll(item => item.user.id.ToString() != MeId);
                    MapDataModel1 = BenHaric;
                    //Gizlilik Ayarlarını geti
                    MapDataModel1 = getUsersGizlilik(MapDataModel1);

                    //Mapte Görünmek istemeyenleri Ayıkla
                    MapDataModel1 = MapDataModel1.FindAll(item => item.user.userPrivacy.visibility_on_the_map == false);

                }
            }
            SetupMapIfNeeded();

        }

        List<NearbyUserCoordinate> getUsersGizlilik(List<NearbyUserCoordinate> BulunanKullanicilar)
        {
            for (int i = 0; i < BulunanKullanicilar.Count; i++)
            {
                WebService webService = new WebService();
                var Donus = webService.OkuGetir("user/" + BulunanKullanicilar[i].user.id.ToString() + "/getUserPrivacySettings");
                if (Donus != null)
                {
                    var aaa = Donus.ToString();
                    var Ayarlarr = Newtonsoft.Json.JsonConvert.DeserializeObject<UserPrivacy>(Donus.ToString());
                    BulunanKullanicilar[i].user.userPrivacy = Ayarlarr;
                }
            }
            return BulunanKullanicilar;
        }
        

        private void ListeButon_Click(object sender, EventArgs e)
        {
            AcKapat();
        }

        #region Liste Aç Kapat Animation

        bool durum = true;
        int boyut;
        public void AcKapat()
        {
            int sayac1 = ListeHaznesi.Height;
            if (durum == false)
            {
                ListeHaznesi.Visibility = ViewStates.Visible;
                int widthSpec = View.MeasureSpec.MakeMeasureSpec(0, MeasureSpecMode.Unspecified);
                int heightSpec = View.MeasureSpec.MakeMeasureSpec(0, MeasureSpecMode.Unspecified);
                ListeHaznesi.Measure(widthSpec, heightSpec);

                ValueAnimator mAnimator = slideAnimator(0, ListeHaznesi.MeasuredHeight);
                mAnimator.Start();
                durum = true;
            }
            else if (durum == true)
            {
                int finalHeight = ListeHaznesi.Height;

                ValueAnimator mAnimator = slideAnimator(finalHeight, 0);
                mAnimator.Start();
                mAnimator.AnimationEnd += (object IntentSender, EventArgs arg) =>
                {
                    ListeHaznesi.Visibility = ViewStates.Gone;
                };
                durum = false;
            }

        }
        private ValueAnimator slideAnimator(int start, int end)
        {

            ValueAnimator animator = ValueAnimator.OfInt(start, end);
            //ValueAnimator animator2 = ValueAnimator.OfInt(start, end);
            //  animator.AddUpdateListener (new ValueAnimator.IAnimatorUpdateListener{
            animator.Update +=
                (object sender, ValueAnimator.AnimatorUpdateEventArgs e) =>
                {
                    //  int newValue = (int)
                    //e.Animation.AnimatedValue; // Apply this new value to the object being animated.
                    //  myObj.SomeIntegerValue = newValue; 
                    var value = (int)animator.AnimatedValue;
                    ViewGroup.LayoutParams layoutParams = ListeHaznesi.LayoutParameters;
                    layoutParams.Height = value;
                    ListeHaznesi.LayoutParameters = layoutParams;
                };
            //      });
            return animator;
        }

        #endregion

        #region Map Ayarları
        public void OnInfoWindowClick(Marker marker)
        {

        }
        public void OnMapLongClick(LatLng point)
        {

        }
        public bool OnMarkerClick(Marker marker)
        {

            var Item = MapDataModel1[Convert.ToInt32(marker.Title)];
            MapDataModel1.ForEach(item =>
            {
                item.IlgiliMarker.SetIcon(BitmapDescriptorFactory.FromBitmap(new MapUtils().createStoreMarker(this.Activity, false, item.user.profile_photo)));
            });


            #region Bir Öncekini Kapat

            MapUtils mapUtils2 = new MapUtils();
            Android.Graphics.Bitmap bitmap = mapUtils2.createStoreMarker(this.Activity, true, Item.user.profile_photo);
            BitmapDescriptor image = BitmapDescriptorFactory.FromBitmap(bitmap);
            Item.IlgiliMarker.SetIcon(image);

            #endregion

            CameraUpdate cameraUpdate = CameraUpdateFactory.NewLatLngZoom(new LatLng(Item.lat, Item.lon), _map.CameraPosition.Zoom);
            _map.AnimateCamera(cameraUpdate);
            HaritaListeFragment1.SecimYap(Convert.ToInt32(marker.Title));

            if (durum == false)
            {
                AcKapat();
            }

            return true;
        }
        public void MarkerSec(int Position)
        {
            MapDataModel1.ForEach(item =>
            {
                item.IlgiliMarker.SetIcon(BitmapDescriptorFactory.FromBitmap(new MapUtils().createStoreMarker(this.Activity, false, item.user.profile_photo)));
            });

            var Item = MapDataModel1[Position];
            #region Bir Öncekini Kapat

            MapUtils mapUtils2 = new MapUtils();
            Android.Graphics.Bitmap bitmap = mapUtils2.createStoreMarker(this.Activity, true, Item.user.profile_photo);
            BitmapDescriptor image = BitmapDescriptorFactory.FromBitmap(bitmap);
            Item.IlgiliMarker.SetIcon(image);

            #endregion

            CameraUpdate cameraUpdate = CameraUpdateFactory.NewLatLngZoom(new LatLng(Item.lat, Item.lon), _map.CameraPosition.Zoom);
            _map.AnimateCamera(cameraUpdate);

        }
        public void OnMapReady(GoogleMap googleMap)
        {
            _map = googleMap;
            _map.SetOnMarkerClickListener(this);
            SetupMapIfNeeded();
            CameraPosition.Builder builder = CameraPosition.InvokeBuilder();
            builder.Target(new LatLng(41.05401653, 28.98877859));
            builder.Zoom(50);
            builder.Bearing(155);
            builder.Tilt(65);
            CameraPosition cameraPosition = builder.Build();
            //CircleOptions circleOptions = new CircleOptions();
            //circleOptions.InvokeCenter(new LatLng(41.05401653, 28.98877859));
            //circleOptions.InvokeStrokeColor(Android.Graphics.Color.ParseColor("#005A82"));
            //circleOptions.InvokeRadius(10);
            //_map.AddCircle(circleOptions);

        }

        private void InitMapFragment()
        {
            var _mapFragment =(SupportMapFragment)ChildFragmentManager.FindFragmentById(Resource.Id.mapp);
            _mapFragment.GetMapAsync(this);
            _mapFragment = null;
            if (_mapFragment == null)
            {
                GoogleMapOptions mapOptions = new GoogleMapOptions()
                    .InvokeMapType(GoogleMap.MapTypeNormal)
                    // .InvokeZoomControlsEnabled(true)
                    .InvokeCompassEnabled(true);
                
                _mapFragment = SupportMapFragment.NewInstance(mapOptions);
              
                
            }
            _mapFragment.GetMapAsync(this);
        }

        string icerik;
        int position;
        public void Haritaa(string gelenicerik, int gelenposition)
        {
            icerik = gelenicerik;
            position = gelenposition;
        }
        private void SetupMapIfNeeded()
        {
            for (int i = 0; i < MapDataModel1.Count; i++)
            {
                MapUtils mapUtils = new MapUtils();
                Android.Graphics.Bitmap bitmap = mapUtils.createStoreMarker(this.Activity, false, MapDataModel1[i].user.profile_photo);
                BitmapDescriptor image = BitmapDescriptorFactory.FromBitmap(bitmap);

                if (_map != null)
                {

                    MarkerOptions markerOpt1 = new MarkerOptions();
                    markerOpt1.SetPosition(new LatLng(MapDataModel1[i].lat, MapDataModel1[i].lon));
                    markerOpt1.SetTitle(i.ToString());
                    markerOpt1.SetIcon(image);
                    markerOpt1.Visible(MapDataModel1[i].IsShow);
                    var EklenenMarker = _map.AddMarker(markerOpt1);
                    MapDataModel1[i].IlgiliMarker = EklenenMarker;
                }

            }
            ListeyiFragmentCagir();
            if (MapDataModel1.Count > 0)
            {
                CameraUpdate cameraUpdate = CameraUpdateFactory.NewLatLngZoom(new LatLng(MapDataModel1[MapDataModel1.Count - 1].lat, MapDataModel1[MapDataModel1.Count - 1].lon), 10);
                _map.MoveCamera(cameraUpdate);
            }

        }
        Android.Support.V4.App.FragmentTransaction ft;
        void ListeyiFragmentCagir()
        {
            ListeHaznesi.RemoveAllViews();
            HaritaListeFragment1 = new HaritaListeFragment(this, MapDataModel1, TakipEttiklerimListe);
            ft = null;
            ft = this.Activity.SupportFragmentManager.BeginTransaction();
            ft.SetCustomAnimations(Resource.Animation.enter_from_right, Resource.Animation.exit_to_left, Resource.Animation.enter_from_left, Resource.Animation.exit_to_right);
            ft.AddToBackStack(null);
            ft.Replace(Resource.Id.frameLayout2, HaritaListeFragment1);
            ft.CommitAllowingStateLoss();
        }

        #endregion

        #region DataModels

        public class KordinatGonder_RootObject
        {
            public string latitude { get; set; }
            public string longitude { get; set; }
        }


        public class YakindakiNearbyUserCoordinate_RootObject
        {
            public string latitude { get; set; }
            public string longitude { get; set; }
            public int meterLimit { get; set; }
        }


        public class TakipEttiklerim_RootObject
        {
            public int from_user_id { get; set; }
            public int to_user_id { get; set; }
        }

        #endregion
    }
}