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
using MyProfileAND.DataBasee;
using static MyProfileAND.Mesajlar.MesajlarBaseFragment;

namespace MyProfileAND.Mesajlar
{
     class ChatFriendListAdapter : BaseAdapter<MesajlarBaseFragment.Message>
    {
        private Context mContext;
        private int mRowLayout;
        private List<MesajlarBaseFragment.Message> mDepartmanlar;
        private int[] mAlternatingColors;
        string MEID;
        public ChatFriendListAdapter(Context context, int rowLayout, List<MesajlarBaseFragment.Message> friends)
        {
            mContext = context;
            mRowLayout = rowLayout;
            mDepartmanlar = friends;
            mAlternatingColors = new int[] { 0xF1F1F1, 0xE7E5E5 };
            MEID = DataBase.USER_INFO_GETIR()[0].id;
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

        public override MesajlarBaseFragment.Message this[int position]
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


                if (MEID != item.from_user_id) 
                {
                    holder.KisiAdi.Text = item.from_user.name + " " + item.from_user.surname;
                    holder.EnSonMesaj.Text = item.message;
                    holder.SonMesajSaati.Text = Convert.ToDateTime(item.created_at).ToShortTimeString();
                    ImageService.Instance.LoadUrl(item.from_user.profile_photo)
                                                    .Transform(new CircleTransformation(5, "#FFFFFF"))
                                                    .Into(holder.ProfilFoto);
                    if (item.status == 0)
                    {
                        holder.OkunmamisBadge.Visibility = ViewStates.Visible;
                        holder.OkunmamisBadge.Text = "";
                    }
                    else
                    {
                        holder.OkunmamisBadge.Visibility = ViewStates.Gone;
                    }
                }
                else
                {
                    holder.KisiAdi.Text = item.to_user.name + " " + item.to_user.surname;
                    holder.EnSonMesaj.Text = item.message;
                    holder.SonMesajSaati.Text = Convert.ToDateTime(item.created_at).ToShortTimeString();
                    if (item.status == 0)
                    {
                        holder.OkunmamisBadge.Visibility = ViewStates.Visible;
                        holder.OkunmamisBadge.Text = "";
                    }
                    else
                    {
                        holder.OkunmamisBadge.Visibility = ViewStates.Gone;
                    }

                    ImageService.Instance.LoadUrl(item.to_user.profile_photo)
                                                    .Transform(new CircleTransformation(5, "#FFFFFF"))
                                                    .Into(holder.ProfilFoto);
                }

                row.Tag = holder;
            }
            return row;
        }
        public void GetBitmapFromUrl(string url, ImageView GelenImageView)
        {
            new System.Threading.Thread(new System.Threading.ThreadStart(delegate
            {
                using (WebClient webClient = new WebClient())
                {
                    byte[] bytes = webClient.DownloadData(url);

                    if (bytes != null && bytes.Length > 0)
                    {
                        ((Android.Support.V7.App.AppCompatActivity)mContext).RunOnUiThread(() =>
                        {
                            GelenImageView.SetImageBitmap(BitmapFactory.DecodeByteArray(bytes, 0, bytes.Length));
                        });
                    }
                }

            })).Start();
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
