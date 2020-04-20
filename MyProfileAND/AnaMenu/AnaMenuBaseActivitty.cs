using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Internal;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Widget;
using FFImageLoading;
using FFImageLoading.Transformations;
using MyProfileAND.AnaSayfa;
using MyProfileAND.DataBasee;
using MyProfileAND.Favoriler;
using MyProfileAND.GenericClass;
using MyProfileAND.Mesajlar;
using MyProfileAND.Profil;
using Square.Picasso;

namespace MyProfileAND.AnaMenu
{
    [Activity(Label = "MyProfile", Theme = "@style/AppTheme")]
    public class AnaMenuBaseActivitty : Android.Support.V7.App.AppCompatActivity
    {
        #region Tanımlamlar
        BottomNavigationView bottomNavigation;
        int sonacilanyer = Resource.Id.menu_home;
        Android.Support.V4.App.FragmentTransaction ft;
        FrameLayout ContentFrame;
        ImageView UserImage;

        TextView Baslik;
        #endregion
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.AnaMenu);
            DinamikStatusBarColor DinamikStatusBarColor1 = new DinamikStatusBarColor();
            DinamikStatusBarColor1.Beyaz(this);
            bottomNavigation = FindViewById<BottomNavigationView>(Resource.Id.bottom_navigation);
            ContentFrame = FindViewById<FrameLayout>(Resource.Id.content_frame);
            bottomNavigation.NavigationItemSelected += BottomNavigation_NavigationItemSelected;
            UserImage = FindViewById<ImageView>(Resource.Id.imgPortada_item2);
            Baslik = FindViewById<TextView>(Resource.Id.basliktext);
            MenuTextleriniKucult(Resource.Id.menu_home);
            MenuTextleriniKucult(Resource.Id.menu_takip);
            MenuTextleriniKucult(Resource.Id.menu_mesaj);
            MenuTextleriniKucult(Resource.Id.menu_profil);
            SetShiftMode(bottomNavigation, true, false);
            var Userr = DataBase.USER_INFO_GETIR();
            if (Userr.Count > 0)
            {
                if (Userr[0].profile_photo != "")
                {
                    ImageService.Instance.LoadUrl("http://23.97.222.30"+Userr[0].profile_photo)
                                                    .Transform(new CircleTransformation(15, "#FFFFFF"))
                                                    .Into(UserImage);
                }
            }
             AnaSayfaYerlestir();
        }

        private void BottomNavigation_NavigationItemSelected(object sender, BottomNavigationView.NavigationItemSelectedEventArgs e)
        {
            LoadFragment(e.Item.ItemId);
        }
        void LoadFragment(int id)
        {
            if (sonacilanyer != id)
            {
                switch (id)
                {
                    case (Resource.Id.menu_home):
                        ClearFragment();
                        AnaSayfaYerlestir();
                        break;
                    case (Resource.Id.menu_takip):
                        ClearFragment();
                        FavorilerYerlestir();
                        break;
                    case (Resource.Id.menu_mesaj):
                        ClearFragment();
                        MesajlarYerlestir();
                        break;
                    case (Resource.Id.menu_profil):
                        ClearFragment();
                        ProfilfaYerlestir();
                        break;
                }
            }
            sonacilanyer = id;
        }
        void MenuTextleriniKucult(int MenuId)
        {
            //return;
            //TextView textView = (TextView)bottomNavigation.FindViewById(MenuId).FindViewById(Resource.Id.largeLabel);
            //textView.SetTextSize(Android.Util.ComplexUnitType.Dip, 0);
        }
        void MesajlarYerlestir()
        {
            Baslik.Text = "Mesajlar";
            MesajlarBaseFragment MesajlarBaseFragment1 = new MesajlarBaseFragment();
            ContentFrame.RemoveAllViews();
            ft = SupportFragmentManager.BeginTransaction();
            ft.AddToBackStack(null);
            ft.Replace(Resource.Id.content_frame, MesajlarBaseFragment1);//
            ft.Commit();
        }
        void FavorilerYerlestir()
        {
            Baslik.Text = "Etkinlikler";
            FavorilerBaseFragment FavorilerBaseFragment1 = new FavorilerBaseFragment();
            ContentFrame.RemoveAllViews();
            ft = SupportFragmentManager.BeginTransaction();
            ft.AddToBackStack(null);
            ft.Replace(Resource.Id.content_frame, FavorilerBaseFragment1);//
            ft.Commit();
        }
        void AnaSayfaYerlestir()
        {
            Baslik.Text = "Keşfet";
            AnaSayfaBaseFragment AnaSayfaBaseFragment1 = new AnaSayfaBaseFragment();
            ContentFrame.RemoveAllViews();
            ft = SupportFragmentManager.BeginTransaction();
            ft.AddToBackStack(null);
            ft.Replace(Resource.Id.content_frame, AnaSayfaBaseFragment1);//
            ft.Commit();
        }

        void ProfilfaYerlestir()
        {
            BilgileriGosterilecekKullanici.UserID =Convert.ToInt32(DataBase.USER_INFO_GETIR()[0].id);
            Baslik.Text = "Profil";
            ProfilBaseFragment ProfilBaseFragment1 = new ProfilBaseFragment();
            ContentFrame.RemoveAllViews();
            ft = SupportFragmentManager.BeginTransaction();
            ft.AddToBackStack(null);
            ft.Replace(Resource.Id.content_frame, ProfilBaseFragment1);//
            ft.Commit();
        }

        void ClearFragment()
        {
            foreach (var item in SupportFragmentManager.Fragments)
            {
                SupportFragmentManager.BeginTransaction().Remove(item).Commit();
            }
        }
        public override void OnBackPressed()
        {

        }
        void SetShiftMode(BottomNavigationView bottomNavigationView, bool enableShiftMode, bool enableItemShiftMode)
        {
            return;
            try
            {
                var menuView = bottomNavigationView.GetChildAt(0) as BottomNavigationMenuView;
                if (menuView == null)
                {
                    System.Diagnostics.Debug.WriteLine("Unable to find BottomNavigationMenuView");
                    return;
                }


                var shiftMode = menuView.Class.GetDeclaredField("mShiftingMode");

                shiftMode.Accessible = true;
                shiftMode.SetBoolean(menuView, enableShiftMode);
                shiftMode.Accessible = false;
                shiftMode.Dispose();


                for (int i = 0; i < menuView.ChildCount; i++)
                {
                    var item = menuView.GetChildAt(i) as BottomNavigationItemView;
                    if (item == null)
                        continue;

                    item.SetChecked(item.ItemData.IsChecked);
                }

                menuView.UpdateMenuView();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Unable to set shift mode: {ex}");
            }
        }
    }
}