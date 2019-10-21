using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.View;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using FFImageLoading;
using FFImageLoading.Views;
using Java.Util;
using MyProfileAND.GenericClass;
using Square.OkHttp;
using Square.Picasso;

namespace MyProfileAND.Profil.Galeri
{
    class GaleriRecyclerViewHolder : RecyclerView.ViewHolder
    {
        
        public ImageViewAsync Imagee;
        
        public GaleriRecyclerViewHolder(View itemView, Action<object[]> listener) : base(itemView)
        {

            Imagee = itemView.FindViewById<ImageViewAsync>(Resource.Id.imgPortada_item2);
            
            itemView.Click += (sender, e) => listener(new object[] { base.Position,itemView });
        }
    }
    class GaleriRecyclerViewAdapter : RecyclerView.Adapter/*, ValueAnimator.IAnimatorUpdateListener*/
    {
        private List<UserGallery> mData = new List<UserGallery>();
        AppCompatActivity BaseActivity;
        public event EventHandler<object[]> ItemClick;
        int Genislikk;
        public GaleriRecyclerViewAdapter(List<UserGallery> GelenData, AppCompatActivity GelenContex,int GelenGenislik)
        {
            mData = GelenData;
            BaseActivity = GelenContex;
            Genislikk = GelenGenislik;
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
        GaleriRecyclerViewHolder HolderForAnimation;
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            GaleriRecyclerViewHolder viewholder = holder as GaleriRecyclerViewHolder;
            HolderForAnimation = holder as GaleriRecyclerViewHolder;
            var item = mData[position];
            if (item.AddNewPhoto)
            {
                viewholder.Imagee.SetImageResource(Resource.Mipmap.add_photo);
                viewholder.Imagee.SetScaleType(ImageView.ScaleType.CenterInside);
                var PaddingDegeri = DPX.dpToPx(BaseActivity,30);
                viewholder.Imagee.SetPadding(PaddingDegeri, PaddingDegeri, PaddingDegeri, PaddingDegeri);
                viewholder.Imagee.Alpha = 0.5f;
            }
            else
            {
                ResimUygula(item.photo_name, viewholder.Imagee);
            }
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
                            client.SetProtocols(new List<Protocol>() { Protocol.Http11 } );
                            Picasso picasso = new Picasso.Builder(BaseActivity)
                            .Downloader(new OkHttpDownloader(client))
                            .Build();
                            picasso.Load(path).Resize(200,200)
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
            View v = inflater.Inflate(Resource.Layout.GaleriCustomCardView, parent, false);
            var paramss = v.LayoutParameters;
            paramss.Height = Genislikk;
            paramss.Width = Genislikk;
            v.LayoutParameters = paramss;
            return new GaleriRecyclerViewHolder(v, OnClick);
        }

        void OnClick(object[] Icerik)
        {
            if (ItemClick != null)
                ItemClick(this, Icerik);
        }
    }
}