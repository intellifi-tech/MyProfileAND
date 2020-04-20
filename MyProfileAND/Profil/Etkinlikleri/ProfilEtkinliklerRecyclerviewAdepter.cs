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
using MyProfileAND.WebServiceHelper;

namespace MyProfileAND.Profil.Etkinlikleri
{
    class ProfilEtkinliklerRecyclerViewHolder : RecyclerView.ViewHolder
    {
        public TextView  EventTitle, EventAciklama,GirisSaati,CikisSaati, KatilimSayisi;
        public ImageViewAsync EventFoto;

        public ProfilEtkinliklerRecyclerViewHolder(View itemView, Action<int> listener) : base(itemView)
        {
            
            EventTitle = itemView.FindViewById<TextView>(Resource.Id.textView1);
            EventAciklama = itemView.FindViewById<TextView>(Resource.Id.textView2);
            EventFoto = itemView.FindViewById<ImageViewAsync>(Resource.Id.ımageView1);
            GirisSaati = itemView.FindViewById<TextView>(Resource.Id.textView6);
            CikisSaati = itemView.FindViewById<TextView>(Resource.Id.textView7);
            KatilimSayisi = itemView.FindViewById<TextView>(Resource.Id.textView5);
            itemView.Click += (sender, e) => listener(base.Position);
        }
    }
    class ProfilEtkinliklerRecyclerViewAdapter : RecyclerView.Adapter/*, ValueAnimator.IAnimatorUpdateListener*/
    {
        private List<UserAttendedEvent> mData = new List<UserAttendedEvent>();
        AppCompatActivity BaseActivity;
        public event EventHandler<int> ItemClick;

        public ProfilEtkinliklerRecyclerViewAdapter(List<UserAttendedEvent> GelenData, AppCompatActivity GelenContex)
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
        ProfilEtkinliklerRecyclerViewHolder HolderForAnimation;
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            ProfilEtkinliklerRecyclerViewHolder viewholder = holder as ProfilEtkinliklerRecyclerViewHolder;
            HolderForAnimation = holder as ProfilEtkinliklerRecyclerViewHolder;
            var item = mData[position];
          
            ImageService.Instance.LoadUrl("http://23.97.222.30"+item.event_image)
                                                  .Into(viewholder.EventFoto);
           
            viewholder.EventTitle.Text = item.Event.title;
            viewholder.EventAciklama.Text = item.event_description;
            viewholder.GirisSaati.Text = "";
            viewholder.CikisSaati.Text = "";

            if (item.date_of_participation != "")
            {
                viewholder.GirisSaati.Text = Convert.ToDateTime(item.date_of_participation).ToShortTimeString();
            }

            if (item.end_date != "")
            {
                viewholder.CikisSaati.Text = Convert.ToDateTime(item.end_date).ToShortTimeString();
            }

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
            View v = inflater.Inflate(Resource.Layout.ProfilEtkinliklerCustomCardView, parent, false);

            return new ProfilEtkinliklerRecyclerViewHolder(v, OnClick);
        }

        void OnClick(int position)
        {
            if (ItemClick != null)
                ItemClick(this, position);
        }
    }
}