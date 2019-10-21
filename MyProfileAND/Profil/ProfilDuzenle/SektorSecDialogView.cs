using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using MyProfileAND.WebServiceHelper;

namespace MyProfileAND.Profil.ProfilDuzenle
{
    public class SektorSecDialogView : Android.Support.V7.App.AppCompatDialogFragment
    {
        #region Tanitm
        ImageButton Kapat;
        ListView SektorListesi;
        SektorlerListAdepter mAdapter;
        List<Sector> mData = new List<Sector>();
        List<Sector> mData_Kopya = new List<Sector>();
        ProfilDuzenleBaseActivity GelenBase1;
        EditText Ara;
        #endregion
        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            Dialog.Window.RequestFeature(WindowFeatures.NoTitle);
            base.OnActivityCreated(savedInstanceState);
            Dialog.Window.Attributes.WindowAnimations = Resource.Style.action_sheet_animation;
        }

        public SektorSecDialogView(ProfilDuzenleBaseActivity GelenBase2)
        {
            GelenBase1 = GelenBase2;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View rootView =  inflater.Inflate(Resource.Layout.SektorSecDialogView, container, false);
            Dialog.Window.SetLayout(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
            //Dialog.Window.SetBackgroundDrawable(new ColorDrawable(Color.Transparent));
            Dialog.Window.SetGravity(GravityFlags.FillHorizontal | GravityFlags.CenterHorizontal | GravityFlags.CenterVertical);
            Kapat = rootView.FindViewById<ImageButton>(Resource.Id.ımageButton1);
            SektorListesi = rootView.FindViewById<ListView>(Resource.Id.listView1);
            Ara = rootView.FindViewById<EditText>(Resource.Id.adrestext);
            Ara.TextChanged += Ara_TextChanged;
            Kapat.Click += Kapat_Click;
            GetAllSektor();
            SektorListesi.ItemClick += SektorListesi_ItemClick;
            return rootView;
        }

        private void Ara_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            BUL(Ara.Text);
        }
        void BUL(string ifade)
        {
            new System.Threading.Thread(new System.Threading.ThreadStart(delegate
            {
                try
                {
                    mData = (from friend in mData_Kopya
                             where friend.name.Contains(ifade, StringComparison.OrdinalIgnoreCase)
                             select friend).ToList<Sector>();

                    this.Activity.RunOnUiThread(() =>
                    {
                        mAdapter = new SektorlerListAdepter(this, Resource.Layout.SektorCustomListView, mData);
                        SektorListesi.Adapter = mAdapter;
                    });
                }
                catch
                {
                }

            })).Start();
        }
        private void SektorListesi_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var TiklananSektor = mData[e.Position];
            GelenBase1.SecilenSektorYansit(TiklananSektor);
            this.Dismiss();
        }

        private void Kapat_Click(object sender, EventArgs e)
        {
            this.Dismiss();
        }

        void GetAllSektor()
        {
            new System.Threading.Thread(new System.Threading.ThreadStart(delegate
            {
                try
                {
                    WebService webService = new WebService();
                    var Donus = webService.OkuGetir("sector/index");
                    if (Donus != null)
                    {
                        var aaa = Donus.ToString();
                        mData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Sector>>(Donus.ToString());
                        mData_Kopya = mData;
                        this.Activity.RunOnUiThread(() =>
                        {
                            mAdapter = new SektorlerListAdepter(this, Resource.Layout.SektorCustomListView, mData);
                            SektorListesi.Adapter = mAdapter;
                        });
                    }
                }
                catch
                {
                }

            })).Start();
        }

    }

    public class Sector
    {
        public int id { get; set; }
        public string name { get; set; }
    }

    class SektorlerListAdepter : BaseAdapter<Sector>
    {
        private SektorSecDialogView mContext;
        private int mRowLayout;
        List<Sector> mData;
        private int[] mAlternatingColors;

        public SektorlerListAdepter(SektorSecDialogView context, int rowLayout, List<Sector> mData2)
        {
            mContext = context;
            mRowLayout = rowLayout;
            mData = mData2;
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
            get { return mData.Count; }
        }

        public override Sector this[int position]
        {
            get { return mData[position]; }
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
            else
            {
                holder = new ListeHolder();
                row = LayoutInflater.From(mContext.Activity).Inflate(mRowLayout, parent, false);
                var item = mData[position];
                holder.Title1 = row.FindViewById<TextView>(Resource.Id.textView1);
                holder.Title1.Text = item.name;
                row.Tag = holder;
            }
            return row;
        }
        class ListeHolder : Java.Lang.Object
        {
            public TextView Title1 { get; set; }
        }
    }
}

