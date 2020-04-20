using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using FFImageLoading;
using FFImageLoading.Transformations;
using FFImageLoading.Views;
using MyProfileAND.GenericClass;
using MyProfileAND.Profil;
using MyProfileAND.Profil.FarkliKullanici;
using MyProfileAND.WebServiceHelper;
using static MyProfileAND.Favoriler.EtkinlikDetay.EtkinlikDetayBaseActivity;

namespace MyProfileAND.Favoriler.EtkinlikDetay.EtkinlikKatilimcilari
{
    [Activity(Label = "MyProfile")]
    public class EtkinlikKatilimcilariBaseActivty : Android.Support.V7.App.AppCompatActivity
    {
        #region Tanimlamalar
        ListView Liste;
        KatilimcilarListAdapter mAdapter;
        List<User> KatilimciListee;
        Android.Support.V7.Widget.Toolbar toolbar;
        #endregion
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.EtkinlikKatilimcilariBaseActivty);
            DinamikStatusBarColor DinamikStatusBarColor1 = new DinamikStatusBarColor();
            DinamikStatusBarColor1.Beyaz(this);
            Liste = FindViewById<ListView>(Resource.Id.listView1);
            KatilimciListee = EtkinlikKatilimcilariKisiler.Kisiler;
            Liste.ItemClick += Liste_ItemClick;
            toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.app_bar);
            SetSupportActionBar(toolbar);
            if (SupportActionBar != null)
            {
                SupportActionBar.SetDisplayHomeAsUpEnabled(true);
                SupportActionBar.Title = "";
                toolbar.NavigationIcon.SetColorFilter(Android.Graphics.Color.Black, PorterDuff.Mode.SrcAtop);
            }
        }

        private void Liste_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            BilgileriGosterilecekKullanici.UserID = KatilimciListee[e.Position].id;
            this.StartActivity(typeof(FarkliKullaniciBaseActivity));
        }

        protected override void OnResume()
        {
            base.OnResume();
            CreateList();
        }

        void CreateList()
        {
            mAdapter = new KatilimcilarListAdapter(this, Resource.Layout.ChatFriendsCustomView, KatilimciListee);
            Liste.Adapter = mAdapter;
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

        class KatilimcilarListAdapter : BaseAdapter<User>
        {
            private Context mContext;
            private int mRowLayout;
            private List<User> mDepartmanlar;
            
            public KatilimcilarListAdapter(Context context, int rowLayout, List<User> friends)
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

            public override User this[int position]
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
                    holder.KisiAdi = row.FindViewById<TextView>(Resource.Id.textView1);
                    holder.EnSonMesaj = row.FindViewById<TextView>(Resource.Id.textView2);
                    holder.SonMesajSaati = row.FindViewById<TextView>(Resource.Id.textView3);
                    holder.OkunmamisBadge = row.FindViewById<TextView>(Resource.Id.textView4);
                    holder.ProfilFoto = row.FindViewById<ImageViewAsync>(Resource.Id.imgPortada_item);



                    holder.KisiAdi.Text = item.name + " " + item.surname;
                    holder.EnSonMesaj.Text = item.title;
                    holder.SonMesajSaati.Text = "";
                    ImageService.Instance.LoadUrl("http://23.97.222.30"+item.profile_photo)
                                                    .Transform(new CircleTransformation(5, "#FFFFFF"))
                                                    .Into(holder.ProfilFoto);

                    holder.OkunmamisBadge.Visibility = ViewStates.Invisible;
                    holder.OkunmamisBadge.Text = "";

                    row.Tag = holder;
                }
                return row;
            }

            class ListeHolder : Java.Lang.Object
            {
                public TextView KisiAdi { get; set; }
                public TextView EnSonMesaj { get; set; }
                public TextView SonMesajSaati { get; set; }
                public TextView OkunmamisBadge { get; set; }
                public ImageViewAsync ProfilFoto { get; set; }

            }
        }
    }

    public static class EtkinlikKatilimcilariKisiler
    {
        public static List<User> Kisiler { get; set; }
    }
}