using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Gms.Location;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace MyProfileAND
{
    [Activity(Label = "KobayActivity")]
    public class KobayActivity : Android.Support.V7.App.AppCompatActivity
    {
        TextView textLastLocation;
        TextView textLocationUpdates;

        FusedLocationProviderClient FusedLocationProviderClient1;
        LocationCallback LocationCallback1;
        LocationRequest LocationRequest1;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.KobayActivity);
            textLastLocation = FindViewById<TextView>(Resource.Id.textView1);
            textLocationUpdates = FindViewById<TextView>(Resource.Id.textView2);
            BuildLocationRequest();
            LocationCallBack();

            FusedLocationProviderClient1 = LocationServices.GetFusedLocationProviderClient(this);
            
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

        protected override  void OnResume()
        {
            base.OnResume();
            if (FusedLocationProviderClient1 != null)
            {
                FusedLocationProviderClient1.RequestLocationUpdates(LocationRequest1, LocationCallback1, Looper.MyLooper());
                
            }

        }
        protected override void OnStop()
        {
            base.OnStop();
            if (FusedLocationProviderClient1 != null)
            {
                FusedLocationProviderClient1.RemoveLocationUpdates(LocationCallback1);
            }

        }
        protected override  void OnPause()
        {
            base.OnPause();
            

        }
        public void LokasyonBas(Location locationn)
        {
            textLocationUpdates.Text = locationn.Latitude + " / " + locationn.Longitude;
        }
        internal class MyLocationCallBack : LocationCallback
        {
            private KobayActivity KobayActivity1;

            public MyLocationCallBack(KobayActivity KobayActivity2)
            {
                KobayActivity1 = KobayActivity2;
            }

            public override void OnLocationResult(LocationResult result)
            {
                base.OnLocationResult(result);
                KobayActivity1.LokasyonBas(result.LastLocation);
            }
        }
        
    }
}