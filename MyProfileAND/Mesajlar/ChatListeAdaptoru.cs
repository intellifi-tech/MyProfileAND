using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Android.Animation;
using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Text;
using Android.Text.Method;
using Android.Util;
using Android.Views;
using Android.Widget;
using FFImageLoading;
using FFImageLoading.Transformations;
using MyProfileAND.DataBasee;

namespace MyProfileAND.Mesajlar
{

    class ChatListeAdaptoru : BaseAdapter<MyProfileAND.Mesajlarr.Message>, View.IOnClickListener
    {
        private Context mContext;
        private int mRowLayout;
        private List<MyProfileAND.Mesajlarr.Message> mMesajlar;
        private int[] mAlternatingColors;
        Android.App.FragmentManager dondur;
        MediaPlayer _player;
        private Timer _SeekBarTimer;
        string[] YesilRenkTonlari;
        Resources resourcesss;
        string MeId="";


        public ChatListeAdaptoru(Resources reso, Context context, int rowLayout, List<MyProfileAND.Mesajlarr.Message> friends)
        {
            resourcesss = reso;
            mContext = context;
            mRowLayout = rowLayout;
            mMesajlar = friends;
            mAlternatingColors = new int[] { 0xF1F1F1, 0xE7E5E5 };
            dondur = ((Activity)context).FragmentManager;
            MeId = DataBase.USER_INFO_GETIR()[0].id;


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
            get { return mMesajlar.Count; }
        }

        public override MyProfileAND.Mesajlarr.Message this[int position]
        {
            get { return mMesajlar[position]; }
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
                if (mMesajlar[position].from_user_id.ToString() == MeId.ToString())
                {
                    row = LayoutInflater.From(mContext).Inflate(Resource.Layout.ChatCustomCellGidenMesaj, parent, false); //**
                }
                else
                {
                    row = LayoutInflater.From(mContext).Inflate(Resource.Layout.ChatCustomCell, parent, false); //**
                }

                holder.Mesaj = row.FindViewById<TextView>(Resource.Id.textView1);
                holder.SonMesajSaati = row.FindViewById<TextView>(Resource.Id.textView2);
                holder.ProfilFoto = row.FindViewById<ImageView>(Resource.Id.imgPortada_item);
                holder.MesajHaznesi = row.FindViewById<LinearLayout>(Resource.Id.mesajHazneLinear);
                row.Tag = holder;
            }
            if (position != 0)
            {
                if (mMesajlar[position - 1].from_user_id == mMesajlar[position].from_user_id) //İlk Mesajdan sonraki mesaj için bir önceki mesaj ile aynı ise profil fotosunu sakla
                {
                    holder.ProfilFoto.Visibility = ViewStates.Invisible;
                }
            }

            if (mMesajlar[position].from_user_id.ToString() == MeId)
            {
                ImageService.Instance.LoadUrl(((ChatActivity)mContext).UserImagePath)
                                                    .Transform(new CircleTransformation(15, "#FFFFFF"))
                                                    .Into(holder.ProfilFoto);
            }
            else
            {
                ImageService.Instance.LoadUrl(((ChatActivity)mContext).HostImagePath)
                                                    .Transform(new CircleTransformation(15, "#FFFFFF"))
                                                    .Into(holder.ProfilFoto);
            }



            holder.Mesaj.Text = mMesajlar[position].message;
            holder.SonMesajSaati.Text = Convert.ToDateTime(mMesajlar[position].created_at).ToShortTimeString();
            return row;
        }
        public List<string> GetLinks(string message)
        {
            List<string> list = new List<string>();
            Regex urlRx = new Regex(@"((https?|ftp|file|http)\://|www.)[A-Za-z0-9\.\-]+(/[A-Za-z0-9\?\&\=;\+!'\(\)\*\-\._~%]*)*", RegexOptions.IgnoreCase);
            MatchCollection matches = urlRx.Matches(message);
            foreach (Match match in matches)
            {

                list.Add(match.Value);
            }
            return list;
        }
        void TagAtamasiYap(int position, string Tur, View GelenView)
        {
            GelenView.Tag = Tur + "#" + position;
        }
        private Color GetColorFromInteger(int color)
        {
            return Color.Rgb(Color.GetRedComponent(color), Color.GetGreenComponent(color), Color.GetBlueComponent(color));
        }
        public void OnClick(View v)
        {
            var TurNedir = v.Tag.ToString().Split('#')[0].ToString();
            //if (TurNedir == "YouTubeVideoClick")
            //{
            //    Intent appIntent = new Intent(Intent.ActionView, Android.Net.Uri.Parse("vnd.youtube:" + FnGetVideoID(mMesajlar[Convert.ToInt32(v.Tag.ToString().Split('#')[1])].SharedYoutubeLink)));
            //    Intent webIntent = new Intent(Intent.ActionView,
            //               Android.Net.Uri.Parse("http://www.youtube.com/watch?v=" + FnGetVideoID(mMesajlar[Convert.ToInt32(v.Tag.ToString().Split('#')[1])].SharedYoutubeLink)));
            //    try
            //    {
            //        mContext.StartActivity(appIntent);
            //    }
            //    catch (ActivityNotFoundException ex)
            //    {
            //        mContext.StartActivity(webIntent);
            //    }
            //}
            //else if (TurNedir == "SesOynatDurdur")
            //{
            //    var TiklanaViewTag = v.Tag.ToString().Split('#')[1].ToString(); //15
            //                                                                    // var Rowcuk = (LinearLayout)v.Parent;
            //    ((ChatActivity)mContext).SesCal(mMesajlar[Convert.ToInt32(TiklanaViewTag)].SesDosyasiIcerigi);
            //}
            //else if (TurNedir == "PaylasilanFotograf")
            //{
            //    var myIntent = new Intent(((ChatActivity)mContext), typeof(ChatFotografBuyut));
            //    myIntent.PutExtra("fotoPath", mMesajlar[Convert.ToInt32(v.Tag.ToString().Split('#')[1].ToString())].SharedPhoto);
            //    ((ChatActivity)mContext).StartActivity(myIntent);
            //}
        }
        static string FnGetVideoID(string strVideoURL)
        {
            const string regExpPattern = @"youtu(?:\.be|be\.com)/(?:.*v(?:/|=)|(?:.*/)?)([a-zA-Z0-9-_]+)";
            //for Vimeo: vimeo\.com/(?:.*#|.*/videos/)?([0-9]+)
            var regEx = new Regex(regExpPattern);
            var match = regEx.Match(strVideoURL);
            return match.Success ? match.Groups[1].Value : null;
        }
    }
    class EmojiSeti
    {
        public string EmojiName { get; set; }
        public int EmojiImge { get; set; }
    }
    class ListeHolder : Java.Lang.Object
    {
        public TextView Mesaj { get; set; }
        public TextView SonMesajSaati { get; set; }
        public ImageView ProfilFoto { get; set; }
        public LinearLayout MesajHaznesi { get; set; }
    }

}