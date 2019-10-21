using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using FFImageLoading;
using FFImageLoading.Transformations;
using FFImageLoading.Views;

namespace MyProfileAND.Favoriler.TakipEttiklerim
{
    class FavorilerTakipEttiklerimListeAdapterHolder : RecyclerView.ViewHolder
    {
        public TextView AdSoyad;
        //public TextView TitleText, DescriptionText;
        //public LinearLayout SecimSellector;
        public ImageViewAsync KisiImage;
        //public TextView AdSoyadText, EtkinlikText;
        public FavorilerTakipEttiklerimListeAdapterHolder(View itemView, Action<int> listener) : base(itemView)
        {
            //item_icon = itemView.FindViewById<ImageView>(Resource.Id.ımageView1);
            AdSoyad = itemView.FindViewById<TextView>(Resource.Id.textView1);
            //SecimSellector = itemView.FindViewById<LinearLayout>(Resource.Id.linearLayout8);
            KisiImage = itemView.FindViewById<ImageViewAsync>(Resource.Id.imgPortada_item2);
            //AdSoyadText = itemView.FindViewById<TextView>(Resource.Id.textView1);
            //EtkinlikText = itemView.FindViewById<TextView>(Resource.Id.textView2);
            itemView.Click += (sender, e) => listener(base.Position);
        }
    }
    class FavorilerTakipEttiklerimRecyclerViewAdapter : RecyclerView.Adapter/*, ValueAnimator.IAnimatorUpdateListener*/
    {
        AppCompatActivity BaseActivity;
        public event EventHandler<int> ItemClick;
        FavorilerTakipEttiklerimBaseFragment GelenBase;
        List<Following> TakipEttiklerimDataModel1;
        public FavorilerTakipEttiklerimRecyclerViewAdapter(FavorilerTakipEttiklerimBaseFragment Base, AppCompatActivity GelenContex, List<Following> TakipEttiklerimDataModel11)
        {
            GelenBase = Base;
            BaseActivity = GelenContex;
            TakipEttiklerimDataModel1 = TakipEttiklerimDataModel11;
        }

        public override int GetItemViewType(int position)
        {
            return position;
        }
        public override int ItemCount
        {
            get
            {
                return TakipEttiklerimDataModel1.Count;
            }
        }
        FavorilerTakipEttiklerimListeAdapterHolder HolderForAnimation;
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            FavorilerTakipEttiklerimListeAdapterHolder viewholder = holder as FavorilerTakipEttiklerimListeAdapterHolder;
            HolderForAnimation = holder as FavorilerTakipEttiklerimListeAdapterHolder;
            var item = TakipEttiklerimDataModel1[position];
            viewholder.AdSoyad.Text = item.name;
            viewholder.AdSoyad.Selected = true;
            ImageService.Instance.LoadUrl(item.profile_photo)
              .Transform(new CircleTransformation(15, "#FFFFFF"))
              .Into(viewholder.KisiImage);

        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            LayoutInflater inflater = LayoutInflater.From(parent.Context);
            View v = inflater.Inflate(Resource.Layout.HikayeKisilerCustomCardView, parent, false);
            return new FavorilerTakipEttiklerimListeAdapterHolder(v, OnClick);
        }

        void OnClick(int position)
        {
            if (ItemClick != null)
                ItemClick(this, position);
        }
    }
}
 