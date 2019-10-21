using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Text;
using Android.Text.Style;
using Android.Views;
using Android.Widget;
using FFImageLoading;
using FFImageLoading.Transformations;
using FFImageLoading.Views;
using MyProfileAND.DataBasee;
using MyProfileAND.GenericUI;
using MyProfileAND.WebServiceHelper;
using Newtonsoft.Json;
using Org.Json;
using Square.OkHttp;
using Square.Picasso;

namespace MyProfileAND.AnaSayfa
{
    class HaritaListeAdapterHolder : RecyclerView.ViewHolder
    {
     
        public TextView UserName,UserTitle;
        public ImageViewAsync UserProfilFoto, UserCover;
        public Button TakipEt;

        public HaritaListeAdapterHolder(View itemView, Action<int> listener) : base(itemView)
        {
            UserName = itemView.FindViewById<TextView>(Resource.Id.textView1);
            UserTitle = itemView.FindViewById<TextView>(Resource.Id.textView2);
            UserCover = itemView.FindViewById<ImageViewAsync>(Resource.Id.ımageView1);
            UserProfilFoto = itemView.FindViewById<ImageViewAsync>(Resource.Id.imgPortada_item2);
            TakipEt = itemView.FindViewById<Button>(Resource.Id.button1);
            itemView.Click += (sender, e) => listener(base.Position);
        }
    }
    class AnaMainRecyclerViewAdapter : RecyclerView.Adapter, View.IOnClickListener
    {
      
        AppCompatActivity BaseActivity;
        public event EventHandler<int> ItemClick;
        HaritaListeFragment GelenBase;
        string MeId;
        public AnaMainRecyclerViewAdapter(HaritaListeFragment Base, AppCompatActivity GelenContex)
        {
            GelenBase = Base;
            BaseActivity = GelenContex;
            MeId = DataBase.USER_INFO_GETIR()[0].id;
        }

        public override int GetItemViewType(int position)
        {
            return position;
        }
        public override int ItemCount
        {
            get
            {
                return GelenBase.MapDataModel1.Count;
            }
        }
        HaritaListeAdapterHolder HolderForAnimation;
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            HaritaListeAdapterHolder viewholder = holder as HaritaListeAdapterHolder;
            HolderForAnimation = holder as HaritaListeAdapterHolder;
            var item = GelenBase.MapDataModel1[position];

            if (item.user != null)
            {

                ImageService.Instance.LoadUrl(item.user.cover_photo)
                                                    .Into(viewholder.UserCover);


                ImageService.Instance.LoadUrl(item.user.profile_photo)
                                                    .Transform(new CircleTransformation(0, "#FFFFFF"))
                                                    .Into(viewholder.UserProfilFoto);

                viewholder.UserName.Text = item.user.name + " " + item.user.surname;
                viewholder.UserTitle.Text = item.user.title;
                viewholder.TakipEt.Tag = item.user.id;
                viewholder.TakipEt.SetOnClickListener(this);

                var TakipEttiklerimArasindaVarmi = GelenBase.TakipEttiklerimList.FindAll(item2 => item2.to_user_id == item.user.id);
                if (TakipEttiklerimArasindaVarmi.Count > 0)
                {
                    viewholder.TakipEt.Text = "Takip";
                }
                else
                {
                    viewholder.TakipEt.Text = "Takip Et";
                }
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
        void TakipEt(int UserId)
        {
            TakipClass TakipClass1 = new TakipClass() {
                to_user_id  = UserId
            };
            var jsonstring = JsonConvert.SerializeObject(TakipClass1);
            WebService webService = new WebService();
            var Donus = webService.ServisIslem("user/follow", jsonstring);
            if (Donus != "Hata")
            {
                AlertHelper.AlertGoster("Takip edildi.", BaseActivity);
                return;
            }

        }
        void TakibiBirak(int UserId)
        {
            TakipClass TakipClass1 = new TakipClass()
            {
                to_user_id = UserId
            };
            var jsonstring = JsonConvert.SerializeObject(TakipClass1);
            WebService webService = new WebService();
            var Donus = webService.ServisIslem("user/stopFollowing", jsonstring);
            if (Donus != "Hata")
            {
                AlertHelper.AlertGoster("Takip durduruldu.", BaseActivity);
                return;
            }
        }
        public class TakipClass
        {
            public int to_user_id { get; set; }
        }
        void ResimUygula(string path, ImageView Imagevieww)
        {
            new System.Threading.Thread(new System.Threading.ThreadStart(delegate
            {
                try
                {
                    if (Imagevieww.Drawable == null)
                    {
                        BaseActivity.RunOnUiThread(() =>
                        {
                            OkHttpClient client = new OkHttpClient();
                            client.SetProtocols(new List<Protocol>() { Protocol.Http11 });
                            Picasso picasso = new Picasso.Builder(BaseActivity)
                            .Downloader(new OkHttpDownloader(client))
                            .Build();
                            picasso.Load(path).Resize(200, 200)
                            .CenterCrop()
                            .Error(Resource.Mipmap.ic_launcher)
                                     .Into(Imagevieww);

                            //ImageService.Instance.LoadUrl(path).WithCache(FFImageLoading.Cache.CacheType.All).Into(Imagevieww);
                        });
                    }
                }
                catch
                {
                }
            })).Start();
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            LayoutInflater inflater = LayoutInflater.From(parent.Context);
            View v = inflater.Inflate(Resource.Layout.HaritaHorizontalCustomView, parent, false);
            return new HaritaListeAdapterHolder(v, OnClickk);
        }

        void OnClickk(int position)
        {
            if (ItemClick != null)
                ItemClick(this, position);
        }

        public void OnClick(View v)
        {
            var UserIdd = (int)((Button)v).Tag;
            var TakipListesi = GelenBase.TakipEttiklerimList;
            var TakipEttiklerimArasindaVarmi = TakipListesi.FindAll(item => item.to_user_id == UserIdd);
            if (TakipEttiklerimArasindaVarmi.Count <= 0)
            {
                Android.App.AlertDialog.Builder cevap = new Android.App.AlertDialog.Builder(BaseActivity);
                cevap.SetIcon(Resource.Mipmap.ic_launcher);
                cevap.SetTitle(Spannla(Color.Black, "MyProfile"));
                cevap.SetMessage(Spannla(Color.DarkGray, "Takip etmek istediğinize emin misiniz?"));
                cevap.SetPositiveButton(Spannla(Color.Black, "Evet"), delegate
                {
                    TakipEt(UserIdd);
                    GelenBase.TakipcileriGuncelle();
                    cevap.Dispose();
                });
                cevap.SetNegativeButton(Spannla(Color.Black, "Hayır"), delegate
                {
                    cevap.Dispose();
                });
                cevap.Show();
            }
            else
            {
                Android.App.AlertDialog.Builder cevap = new Android.App.AlertDialog.Builder(BaseActivity);
                cevap.SetIcon(Resource.Mipmap.ic_launcher);
                cevap.SetTitle(Spannla(Color.Black, "MyProfile"));
                cevap.SetMessage(Spannla(Color.DarkGray, "Takibi bırakmak istediğinizden emin misiniz?"));
                cevap.SetPositiveButton(Spannla(Color.Black, "Evet"), delegate
                {
                    TakibiBirak(UserIdd);
                    GelenBase.TakipcileriGuncelle();
                    cevap.Dispose();
                });
                cevap.SetNegativeButton(Spannla(Color.Black, "Hayır"), delegate
                {
                    cevap.Dispose();
                });
                cevap.Show();
            }
        }
    }
}
