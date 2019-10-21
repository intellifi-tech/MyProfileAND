using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Gms.Maps;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using MyProfileAND.Favoriler.Global;
using MyProfileAND.Favoriler.Takipciler;
using MyProfileAND.Favoriler.TakipEttiklerim;
using MyProfileAND.Favoriler.YeniEtkinlikOlustur;
using MyProfileAND.GenericClass;

namespace MyProfileAND.Favoriler
{
    public class FavorilerBaseFragment : Android.Support.V4.App.Fragment
    {
        #region Tanimlamalar
        RelativeLayout AramaHaznesi;
        FrameLayout TakipEttiklerimHaznesi;
        Android.Support.V4.App.FragmentTransaction ft;
        TabLayout tabLayout;
        ViewPager viewPager;
        ImageButton YeniEtkinlikOlustur;
        #endregion

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View RootView = inflater.Inflate(Resource.Layout.FavorilerBaseFragment, container, false);
            TakipEttiklerimHaznesi = RootView.FindViewById<FrameLayout>(Resource.Id.takipettiklerimHazne);
            tabLayout = RootView.FindViewById<TabLayout>(Resource.Id.sliding_tabsIcon);
            viewPager = RootView.FindViewById<ViewPager>(Resource.Id.viewpagerIcon);
            YeniEtkinlikOlustur = RootView.FindViewById<ImageButton>(Resource.Id.ımageButton1);
            YeniEtkinlikOlustur.Click += YeniEtkinlikOlustur_Click;
            FnInitTabLayout();
            return RootView;
        }

        private void YeniEtkinlikOlustur_Click(object sender, EventArgs e)
        {
            this.Activity.StartActivity(typeof(YeniEtkinlikOlusturBaseActivity0));
        }

        void FnInitTabLayout()
        {
            tabLayout.SetTabTextColors(Android.Graphics.Color.ParseColor("#909090"), Android.Graphics.Color.ParseColor("#1a237e"));
            Android.Support.V4.App.Fragment ss1 = null;
            Android.Support.V4.App.Fragment ss2 = null;

            ss1 = new TakipcilerBaseFragment();
            ss2 = new GlobalBaseFragment();

            //Fragment array
            var fragments = new Android.Support.V4.App.Fragment[]
            {
                ss1,
                ss2,

            };

            var titles = CharSequence.ArrayFromStringArray(new[] {
               "Takip Ettkilerim",
               "Global",
            });

            viewPager.Adapter = new TabPagerAdaptor(this.Activity.SupportFragmentManager, fragments, titles, true);

            tabLayout.SetupWithViewPager(viewPager);
        }
    }
}