using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.View;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using FFImageLoading;
using FFImageLoading.Transformations;
using FFImageLoading.Views;
using MyProfileAND.Profil;
using MyProfileAND.Profil.FarkliKullanici;
using MyProfileAND.WebServiceHelper;

namespace MyProfileAND.Favoriler.Takipciler
{
    class TakipcilerRecyclerViewHolder : RecyclerView.ViewHolder
    {
        public TextView KullaniciAdSoyad, KullaniciTitle, EventTitle, EventAciklama, GirisSaati, CikisSaati,KatilimSayisi;
        public ImageViewAsync KullaniciProfilFoto, EventFoto;
        public TakipcilerRecyclerViewHolder(View itemView, Action<int> listener) : base(itemView)
        {
            KullaniciAdSoyad = itemView.FindViewById<TextView>(Resource.Id.textView3);
            KullaniciTitle = itemView.FindViewById<TextView>(Resource.Id.textView4);
            EventTitle = itemView.FindViewById<TextView>(Resource.Id.textView1);
            EventAciklama = itemView.FindViewById<TextView>(Resource.Id.textView2);
            KullaniciProfilFoto = itemView.FindViewById<ImageViewAsync>(Resource.Id.imgPortada_item2);
            EventFoto = itemView.FindViewById<ImageViewAsync>(Resource.Id.ımageView1);
            GirisSaati = itemView.FindViewById<TextView>(Resource.Id.textView6);
            CikisSaati = itemView.FindViewById<TextView>(Resource.Id.textView7);
            KatilimSayisi = itemView.FindViewById<TextView>(Resource.Id.textView5);
            itemView.Click += (sender, e) => listener(base.Position);
        }
    }
    class TakipcilerRecyclerViewAdapter : RecyclerView.Adapter,View.IOnClickListener
    {
        private List<UserAttendedEvent> mData = new List<UserAttendedEvent>();
        AppCompatActivity BaseActivity;
        public event EventHandler<int> ItemClick;

        public TakipcilerRecyclerViewAdapter(List<UserAttendedEvent> GelenData, AppCompatActivity GelenContex)
        {
            mData = GelenData;
            BaseActivity = GelenContex;
        }

        public override int GetItemViewType(int position)
        {
            return position;
        }
        public override int ItemCount
        {
            get
            {
                return mData.Count;
            }
        }
        TakipcilerRecyclerViewHolder HolderForAnimation;
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            TakipcilerRecyclerViewHolder viewholder = holder as TakipcilerRecyclerViewHolder;
            HolderForAnimation = holder as TakipcilerRecyclerViewHolder;
            var item = mData[position];

            ImageService.Instance.LoadUrl("http://23.97.222.30"+item.UserInfo.profile_photo)
                                                  .Transform(new CircleTransformation(15, "#FFFFFF"))
                                                  .Into(viewholder.KullaniciProfilFoto);


            ImageService.Instance.LoadUrl("http://23.97.222.30"+item.event_image)
                                                  .Into(viewholder.EventFoto);


            viewholder.KullaniciAdSoyad.Text = item.UserInfo.name + " " + item.UserInfo.surname;
            viewholder.KullaniciTitle.Text = item.UserInfo.title;
            viewholder.EventTitle.Text = item.Event.title;
            viewholder.EventAciklama.Text =item.event_description;
            viewholder.GirisSaati.Text = "";
            viewholder.CikisSaati.Text = "";


            if (item.date_of_participation != "")
            {
                viewholder.GirisSaati.Text = Convert.ToDateTime(item.date_of_participation).ToShortTimeString();
            }

            if (item.end_date != "")
            {
                viewholder.GirisSaati.Text = Convert.ToDateTime(item.end_date).ToShortTimeString();
            }
            viewholder.KullaniciAdSoyad.Tag = position;
            viewholder.KullaniciProfilFoto.Tag = position;
            viewholder.KullaniciAdSoyad.SetOnClickListener(this);
            viewholder.KullaniciProfilFoto.SetOnClickListener(this);


            if (DateTime.Now > Convert.ToDateTime(item.end_date))
            {
                //viewholder.GirisSaati.SetTextColor(Color.Red);
                //viewholder.CikisSaati.SetTextColor(Color.Red);
                viewholder.GirisSaati.PaintFlags = PaintFlags.StrikeThruText;
                viewholder.CikisSaati.PaintFlags = PaintFlags.StrikeThruText;
            }
            else
            {
                //viewholder.GirisSaati.SetTextColor(Color.White);
                //viewholder.CikisSaati.SetTextColor(Color.White);
            }
          KatilimciSayisiniGetir(item.event_id.ToString(), viewholder.KatilimSayisi);
        }
        void KatilimciSayisiniGetir(string EventID, TextView KatilimciSayiText)
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
                        var Countt = Newtonsoft.Json.JsonConvert.DeserializeObject<RootObject>(aaaa).user_attended_event.Count;
                        BaseActivity.RunOnUiThread(delegate ()
                        {
                            KatilimciSayiText.Text = Countt.ToString();
                        });
                    }
                    catch
                    {
                        BaseActivity.RunOnUiThread(delegate ()
                        {
                            KatilimciSayiText.Text = "-";
                        });
                    }
                }
            })).Start();
        }
        public class RootObject
        {
            public List<object> user_attended_event { get; set; }
        }
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            LayoutInflater inflater = LayoutInflater.From(parent.Context);
            View v = inflater.Inflate(Resource.Layout.FavorilerCardView, parent, false);

            return new TakipcilerRecyclerViewHolder(v, OnClickk);
        }

        void OnClickk(int position)
        {
            if (ItemClick != null)
                ItemClick(this, position);
        }

        public void OnClick(View v)
        {
            var Posi = (int)v.Tag;
            var kisi = mData[Posi];

            BilgileriGosterilecekKullanici.UserID = kisi.user_id;
            BaseActivity.StartActivity(typeof(FarkliKullaniciBaseActivity));

        }
    }
}