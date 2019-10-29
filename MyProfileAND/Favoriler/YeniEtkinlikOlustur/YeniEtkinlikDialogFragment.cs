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
using MyProfileAND.Favoriler.MevcutEtkinligeKatil;
using MyProfileAND.GenericClass;
using MyProfileAND.WebServiceHelper;
using Newtonsoft.Json;

namespace MyProfileAND.Favoriler.YeniEtkinlikOlustur
{
    public class YeniEtkinlikDialogFragment : Android.Support.V7.App.AppCompatDialogFragment
    {
        #region Tanitm
        ImageButton Kapat;
        Button Kaydet;
        EditText Titlee;
        MevcutEtkinlikActivity YeniEtkinlikOlusturBaseActivity02;
        #endregion
        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            Dialog.Window.RequestFeature(WindowFeatures.NoTitle);
            base.OnActivityCreated(savedInstanceState);
            Dialog.Window.Attributes.WindowAnimations = Resource.Style.action_sheet_animation;
        }
        
        public YeniEtkinlikDialogFragment(MevcutEtkinlikActivity YeniEtkinlikOlusturBaseActivity01)
        {
            YeniEtkinlikOlusturBaseActivity02 = YeniEtkinlikOlusturBaseActivity01;
        }


        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View rootView =  inflater.Inflate(Resource.Layout.YeniEtkinlikDialogFragment, container, false);
            Dialog.Window.SetLayout(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
            Dialog.Window.SetGravity(GravityFlags.FillHorizontal | GravityFlags.CenterHorizontal | GravityFlags.CenterVertical);
            Kapat = rootView.FindViewById<ImageButton>(Resource.Id.ımageButton1);
            Kaydet = rootView.FindViewById<Button>(Resource.Id.button1);
            
            Titlee = rootView.FindViewById<EditText>(Resource.Id.editText2);
            Kaydet.Click += Kaydet_Click;
            Kapat.Click += Kapat_Click;
            return rootView;
        }

        private void Kaydet_Click(object sender, EventArgs e)
        {
            YeniEtkinlikOlusturBaseActivity02.DevamEt(Titlee.Text);
            this.Dismiss();
        }

     
        private void Kapat_Click(object sender, EventArgs e)
        {
            this.Dismiss();
        }
    }
}

