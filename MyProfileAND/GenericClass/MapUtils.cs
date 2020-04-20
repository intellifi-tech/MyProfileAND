using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using FFImageLoading;
using FFImageLoading.Transformations;
using FFImageLoading.Views;
using Square.Picasso;

namespace MyProfileAND.GenericClass
{
    class MapUtils
    {
        public Bitmap createStoreMarker(Context BaseContex,bool IsSelect,string ProfilImage = "")
        {
            LayoutInflater inflater = LayoutInflater.From(BaseContex);
            View markerLayout = inflater.Inflate(Resource.Layout.custom_map_marker_layout, null);
            ImageView Markerr = markerLayout.FindViewById<ImageView>(Resource.Id.ımageView1);
            ImageViewAsync ProfilFotoo = markerLayout.FindViewById<ImageViewAsync>(Resource.Id.imgPortada_item2);
            //if (!IsSelect)
            //{
            //    Markerr.SetColorFilter(Color.ParseColor("#ffffff"));
            //}
            //else
            //{
            //    Markerr.SetColorFilter(Color.ParseColor("#304ffe"));
            //}

            if (!String.IsNullOrEmpty(ProfilImage))
            {
                ImageService.Instance.LoadUrl("http://23.97.222.30"+ProfilImage)
                                                    .Transform(new CircleTransformation(0, "#FFFFFF"))
                                                    .Into(ProfilFotoo);
            }

            markerLayout.Measure(View.MeasureSpec.MakeMeasureSpec(0, MeasureSpecMode.Unspecified), View.MeasureSpec.MakeMeasureSpec(0, MeasureSpecMode.Unspecified));
            markerLayout.Layout(0, 0, markerLayout.MeasuredWidth, markerLayout.MeasuredHeight);

            Bitmap bitmap = Bitmap.CreateBitmap(markerLayout.MeasuredWidth, markerLayout.MeasuredHeight, Bitmap.Config.Argb8888);
            Canvas canvas = new Canvas(bitmap);
            markerLayout.Draw(canvas);
            return bitmap;
        }

        public Bitmap createStoreMarker_Dinamik_View(Context BaseContex,int Layout)
        {
            LayoutInflater inflater = LayoutInflater.From(BaseContex);
            View markerLayout = inflater.Inflate(Layout, null);

            markerLayout.Measure(View.MeasureSpec.MakeMeasureSpec(0, MeasureSpecMode.Unspecified), View.MeasureSpec.MakeMeasureSpec(0, MeasureSpecMode.Unspecified));
            markerLayout.Layout(0, 0, markerLayout.MeasuredWidth, markerLayout.MeasuredHeight);

            Bitmap bitmap = Bitmap.CreateBitmap(markerLayout.MeasuredWidth, markerLayout.MeasuredHeight, Bitmap.Config.Argb8888);
            Canvas canvas = new Canvas(bitmap);
            markerLayout.Draw(canvas);
            return bitmap;
        }
    }
}