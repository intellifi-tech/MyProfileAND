using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Util;
using Android.Views;
using Android.Widget;
using FFImageLoading;
using FFImageLoading.Transformations;
using FFImageLoading.Views;
using MyProfileAND.DataBasee;
using MyProfileAND.Favoriler.TakipEttiklerim;
using MyProfileAND.GenericClass;
using MyProfileAND.GenericUI;
using MyProfileAND.Mesajlar;
using MyProfileAND.Profil.Etkinlikleri;
using MyProfileAND.Profil.Galeri;
using MyProfileAND.Profil.ProfilDetaylari;
using MyProfileAND.Profil.ProfilDuzenle;
using MyProfileAND.WebServiceHelper;
using Newtonsoft.Json;
using Square.Picasso;
using static MyProfileAND.Mesajlar.ChatActivity;

namespace MyProfileAND.Profil
{
    public class ProfilBaseFragment : Android.Support.V4.App.Fragment
    {
        #region Tanimalamalar
        TextView AdiSoyadi, Titlee;
        ImageViewAsync ProfilFotografi, KapakFotografi;
        ImageButton ProfilFotoGuncelleButton;
        Button KapakDegistir,ProfilDuzenleAc;
        LinearLayout iceriklinearhazne;
        Android.Support.V4.App.FragmentTransaction ft;
        ImageButton ProfilButton, EventlerButton, GaleriButton;
        public static readonly int PickImageId = 1000;
        public static readonly int CoverImageId = 1001;

        USER_INFO Kullanici;
        RelativeLayout KapakDegisHazne, ProfilDuzenleHazne;
        TextView TakipciSayisiText, MesajAtText;
        LinearLayout MesatAtHazne;

        public object Mesaj_Listesi_RootObject1 { get; private set; }
        #endregion

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View RootView = inflater.Inflate(Resource.Layout.ProfilBaseFragment, container, false);
            AdiSoyadi = RootView.FindViewById<TextView>(Resource.Id.adsoyadtxt);
            Titlee = RootView.FindViewById<TextView>(Resource.Id.titletxt);
            ProfilFotografi = RootView.FindViewById<ImageViewAsync>(Resource.Id.imgPortada_item2);
            KapakFotografi = RootView.FindViewById<ImageViewAsync>(Resource.Id.ımageView1);
            ProfilFotoGuncelleButton = RootView.FindViewById<ImageButton>(Resource.Id.ımageButton1);
            KapakDegistir = RootView.FindViewById<Button>(Resource.Id.kapakguncelle);
            ProfilDuzenleAc = RootView.FindViewById<Button>(Resource.Id.profilduzenle);
            KapakDegistir.Click += KapakDegistir_Click;
            ProfilFotoGuncelleButton.Click += ProfilFotoGuncelleButton_Click;
            ProfilDuzenleAc.Click += ProfilDuzenleAc_Click;
            iceriklinearhazne = RootView.FindViewById<LinearLayout>(Resource.Id.iceriklinearhazne);
            ProfilButton = RootView.FindViewById<ImageButton>(Resource.Id.ımageButton2);
            EventlerButton = RootView.FindViewById<ImageButton>(Resource.Id.ımageButton3);
            GaleriButton = RootView.FindViewById<ImageButton>(Resource.Id.ımageButton4);
            KapakDegisHazne = RootView.FindViewById<RelativeLayout>(Resource.Id.kapakdegistirhazne);
            ProfilDuzenleHazne = RootView.FindViewById<RelativeLayout>(Resource.Id.profileduzenlehazne);
            TakipciSayisiText = RootView.FindViewById<TextView>(Resource.Id.takipsayisi);
            MesajAtText = RootView.FindViewById<TextView>(Resource.Id.mesajat);
            MesajAtText.Click += MesajAtText_Click;
            MesatAtHazne = RootView.FindViewById<LinearLayout>(Resource.Id.mesatathazne);
            ProfilButton.Tag = 0;
            EventlerButton.Tag = 1;
            GaleriButton.Tag = 2;

            ProfilButton.Click += TabButtonClick;
            EventlerButton.Click += TabButtonClick;
            GaleriButton.Click += TabButtonClick;

            if (DataBase.USER_INFO_GETIR()[0].id != BilgileriGosterilecekKullanici.UserID.ToString())
            {
                KapakDegisHazne.Visibility = ViewStates.Gone;
                ProfilDuzenleHazne.Visibility = ViewStates.Gone;
                ProfilFotoGuncelleButton.Visibility = ViewStates.Gone;
                UserGizlilik();
                if (userPrivacy.no_message==true)
                {
                    MesatAtHazne.Visibility = ViewStates.Gone;
                }
            }
            else
            {
                MesatAtHazne.Visibility = ViewStates.Gone;
            }
            Kullanici = KullaniciBilgileriniGetir(BilgileriGosterilecekKullanici.UserID.ToString());
            TakiSayisiniGetir();
            ParcaYerlestir(0);
            KullaniciBilgileriniYansit();
            return RootView;
        }

        UserPrivacy userPrivacy = new UserPrivacy();
        void UserGizlilik()
        {
            WebService webService = new WebService();
            var Donus = webService.OkuGetir("user/" + BilgileriGosterilecekKullanici.UserID.ToString() + "/getUserPrivacySettings");
            if (Donus != null)
            {
                var aaa = Donus.ToString();
                userPrivacy = Newtonsoft.Json.JsonConvert.DeserializeObject<UserPrivacy>(Donus.ToString());
            }
        }


        private void MesajAtText_Click(object sender, EventArgs e)
        {
            ChatUserId.UserID = Kullanici.id;
            this.Activity.StartActivity(typeof(ChatActivity));
        }

        private void TabButtonClick(object sender, EventArgs e)
        {
            ParcaYerlestir((int)((ImageButton)sender).Tag);
        }
        
        private void ProfilDuzenleAc_Click(object sender, EventArgs e)
        {
            this.Activity.StartActivity(typeof(ProfilDuzenleBaseActivity));
        }
        private void KapakDegistir_Click(object sender, EventArgs e)
        {
            var Intentt = new Intent();
            Intentt.SetType("image/*");
            Intentt.SetAction(Intent.ActionGetContent);
            StartActivityForResult(Intent.CreateChooser(Intentt, "Fotoğraf Seç"), CoverImageId);
        }
        private void ProfilFotoGuncelleButton_Click(object sender, EventArgs e)
        {
            var Intentt = new Intent();
            Intentt.SetType("image/*");
            Intentt.SetAction(Intent.ActionGetContent);
            StartActivityForResult(Intent.CreateChooser(Intentt, "Fotoğraf Seç"), PickImageId);
        }
        public override void OnActivityResult(int requestCode, int resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if ((requestCode == PickImageId) || (requestCode == CoverImageId) && (resultCode == (int)Result.Ok) && (data != null))
            {
                try
                {
                    Android.Net.Uri uri = data.Data;

                    using (var inputStream = this.Activity.ContentResolver.OpenInputStream(uri))
                    {
                        using (var streamReader = new StreamReader(inputStream))
                        {
                            var bytes = default(byte[]);
                            using (var memstream = new MemoryStream())
                            {
                                streamReader.BaseStream.CopyTo(memstream);
                                bytes = memstream.ToArray();

                                var Guidee = Guid.NewGuid().ToString();
                                var FilePath = System.IO.Path.Combine(documentsFolder(), Guidee + ".jpg");
                                System.IO.File.WriteAllBytes(FilePath, memstream.ToArray());
                                if (System.IO.File.Exists(FilePath))
                                {
                                    var newbytess = ResizeImageAndroid(FilePath, bytes, 600, 600);
                                    if (newbytess != null)
                                    {
                                        var base64String = Convert.ToBase64String(newbytess);
                                        if (requestCode == PickImageId)
                                        {
                                            FotoGuncelle(base64String, 0);
                                        }
                                        else
                                        {
                                            FotoGuncelle(base64String, 1);
                                        }
                                    }
                                }

                                //string base64String = Convert.ToBase64String(bytes);
                                //Stream srm = memstream;
                                //var FilePath = System.IO.Path.Combine(documentsFolder(), "PPImagege.jpg");
                                //File.WriteAllBytes(FilePath, bytes);
                                //if (File.Exists(FilePath))
                                //{
                                //    if (requestCode == PickImageId)
                                //    {
                                //        FotoGuncelle(base64String, 0);
                                //    }
                                //    else
                                //    {
                                //        FotoGuncelle(base64String, 1);
                                //    }

                                //}
                            }
                        }
                    }
                }
                catch 
                {
                }
                
            }
        }

        public byte[] ResizeImageAndroid(string FileDesc, byte[] imageData, float width, float height)
        {

            Android.Media.ExifInterface oldExif = new Android.Media.ExifInterface(FileDesc);
            String exifOrientation = oldExif.GetAttribute(Android.Media.ExifInterface.TagOrientation);


            // Load the bitmap 
            Bitmap originalImage = BitmapFactory.DecodeByteArray(imageData, 0, imageData.Length);
            //
            float ZielHoehe = 0;
            float ZielBreite = 0;
            //
            var Hoehe = originalImage.Height;
            var Breite = originalImage.Width;

            //
            float NereyeRotate = 0;
            if (Hoehe > Breite) // Höhe (71 für Avatar) ist Master
            {
                ZielHoehe = height;
                float teiler = Hoehe / height;
                ZielBreite = Breite / teiler;
                NereyeRotate = 0;
            }
            else if (Hoehe < Breite) // Breite (61 für Avatar) ist Master
            {
                ZielBreite = width;
                float teiler = Breite / width;
                ZielHoehe = Hoehe / teiler;
                NereyeRotate = -90;
            }
            else //EsitOlmaDurumu
            {
                ZielBreite = width;
                ZielHoehe = height;
                NereyeRotate = 0;
            }
            //
            Bitmap resizedImage = Bitmap.CreateScaledBitmap(originalImage, (int)ZielBreite, (int)ZielHoehe, true);
            //return rotateBitmap(resizedImage, 0);

            using (MemoryStream ms = new MemoryStream())
            {
                resizedImage.Compress(Bitmap.CompressFormat.Jpeg, 100, ms);

                var Guidee = Guid.NewGuid().ToString();
                var FilePath = System.IO.Path.Combine(documentsFolder(), Guidee + ".jpg");
                System.IO.File.WriteAllBytes(FilePath, ms.ToArray());
                if (System.IO.File.Exists(FilePath))
                {
                    if (exifOrientation != null)
                    {
                        Android.Media.ExifInterface newExif = new Android.Media.ExifInterface(FilePath);
                        newExif.SetAttribute(Android.Media.ExifInterface.TagOrientation, exifOrientation);
                        newExif.SaveAttributes();
                        var bytess = System.IO.File.ReadAllBytes(FilePath);

                        System.IO.File.Delete(FileDesc);
                        System.IO.File.Delete(FilePath);

                        return bytess;
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }


            }
        }

        void FotoGuncelle(string base644,int Durum)
        {
            WebService webService = new WebService();
            var Kullanici = DataBase.USER_INFO_GETIR()[0];
            if (Durum == 0)
            {
                Kullanici.profile_photo = base644;
                string jsonString = JsonConvert.SerializeObject(Kullanici);
                var Donus = webService.ServisIslem("user/update", jsonString, Method: "PUT");
                if (Donus != "Hata")
                {
                    var GuncelKullanici = Newtonsoft.Json.JsonConvert.DeserializeObject<FotografGuncelleModel>(Donus);
                    USER_INFO GuncellenecekDTO = new USER_INFO()
                    {
                        cover_photo = GuncelKullanici.user.cover_photo,
                        api_token = Kullanici.api_token,
                        career_history = GuncelKullanici.user.career_history,
                        company_id = GuncelKullanici.user.company_id,
                        credentials= GuncelKullanici.user.credentials,
                        date_of_birth = GuncelKullanici.user.date_of_birth,
                        email = GuncelKullanici.user.email,
                        id = GuncelKullanici.user.id,
                        localid = Kullanici.localid,
                        name = GuncelKullanici.user.name,
                        profile_photo = GuncelKullanici.user.profile_photo,
                        sector_id = GuncelKullanici.user.sector_id,
                        short_biography = GuncelKullanici.user.short_biography,
                        status = GuncelKullanici.user.status,
                        surname = GuncelKullanici.user.surname,
                        title = GuncelKullanici.user.title,
                        
                    };

                    DataBase.USER_INFO_Guncelle(GuncellenecekDTO);
                    ImageService.Instance.LoadUrl("http://23.97.222.30"+GuncellenecekDTO.profile_photo)
                                                    .Transform(new CircleTransformation(15, "#FFFFFF"))
                                                    .Into(ProfilFotografi);


                    AlertHelper.AlertGoster("Profil Fotoğrafınız Güncellendi.", this.Activity);
                    KullaniciBilgileriniYansit(false);
                    return;
                }
                else
                {
                    AlertHelper.AlertGoster("Bir sorun oluştu.", this.Activity);
                    return;
                }
            }
            else
            {
                Kullanici.cover_photo = base644;
                string jsonString = JsonConvert.SerializeObject(Kullanici);
                var Donus = webService.ServisIslem("user/update", jsonString, Method: "PUT");
                if (Donus != "Hata")
                {
                    var GuncelKullanici = Newtonsoft.Json.JsonConvert.DeserializeObject<FotografGuncelleModel>(Donus);
                    USER_INFO GuncellenecekDTO = new USER_INFO()
                    {
                        cover_photo = GuncelKullanici.user.cover_photo,
                        api_token = Kullanici.api_token,
                        career_history = GuncelKullanici.user.career_history,
                        company_id = GuncelKullanici.user.company_id,
                        credentials = GuncelKullanici.user.credentials,
                        date_of_birth = GuncelKullanici.user.date_of_birth,
                        email = GuncelKullanici.user.email,
                        id = GuncelKullanici.user.id,
                        localid = Kullanici.localid,
                        name = GuncelKullanici.user.name,
                        profile_photo = GuncelKullanici.user.profile_photo,
                        sector_id = GuncelKullanici.user.sector_id,
                        short_biography = GuncelKullanici.user.short_biography,
                        status = GuncelKullanici.user.status,
                        surname = GuncelKullanici.user.surname,
                        title = GuncelKullanici.user.title,
                    };

                    DataBase.USER_INFO_Guncelle(GuncellenecekDTO);
                    ImageService.Instance.LoadUrl("http://23.97.222.30"+GuncellenecekDTO.cover_photo).Into(KapakFotografi);
                    AlertHelper.AlertGoster("Kapak Fotoğrafınız Güncellendi.", this.Activity);
                    KullaniciBilgileriniYansit(false);
                    return;
                }
                else
                {
                    AlertHelper.AlertGoster("Bir sorun oluştu.", this.Activity);
                    return;
                }
            }
        }
        public static string documentsFolder()
        {
            string path;
            path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            Directory.CreateDirectory(path);
            return path;
        }
        void KullaniciBilgileriniYansit(bool GorselleriGuncelle=true)
        {
            if (Kullanici != null)
            {
                AdiSoyadi.Text = Kullanici.name + " " + Kullanici.surname;
                Titlee.Text = Kullanici.title;

                if (GorselleriGuncelle)//Fotografları güncellediğinde bu bölüm eski fotoğrafların tekrar yüklenmesini sağlıyor o yüzden kontrol koyuyoruz
                {
                    if (!string.IsNullOrEmpty(Kullanici.profile_photo))
                    {
                        ImageService.Instance.LoadUrl("http://23.97.222.30"+Kullanici.profile_photo)
                                                        .Transform(new CircleTransformation(15, "#FFFFFF"))
                                                        .Into(ProfilFotografi);
                    }
                    if (!string.IsNullOrEmpty(Kullanici.cover_photo))
                    {
                        KapakFotografi.ClearColorFilter();
                        ImageService.Instance.LoadUrl("http://23.97.222.30"+Kullanici.cover_photo).Into(KapakFotografi);
                    }
                }
                
            }
        }

        USER_INFO KullaniciBilgileriniGetir(string UserID)
        {
            WebService webService = new WebService();
            var Donus = webService.OkuGetir("user/"+UserID+"/show");
            if (Donus != null)
            {
                return Newtonsoft.Json.JsonConvert.DeserializeObject<USER_INFO>(Donus);
            }
            else
            {
                this.Activity.Finish();
                return null;
            }
        }

        private int[] getScreenDimension()
        {
            DisplayMetrics dm = new DisplayMetrics();
            this.Activity.WindowManager.DefaultDisplay.GetMetrics(dm);
            int width = dm.WidthPixels;
            int height = dm.HeightPixels;
            var dens = dm.DensityDpi;
            double wi = (double)width / (double)dens;
            double hi = (double)height / (double)dens;
            double x = Math.Pow(wi, 2);
            double y = Math.Pow(hi, 2);
            double screenInches = Math.Sqrt(x + y);

            int[] screenInformation = new int[2];
            screenInformation[0] = width;
            screenInformation[1] = height;
            return screenInformation;
        }
        void ParcaYerlestir(int durum)
        {
            ProfilButton.SetColorFilter(Color.ParseColor("#686868"));
            EventlerButton.SetColorFilter(Color.ParseColor("#686868"));
            GaleriButton.SetColorFilter(Color.ParseColor("#686868"));

            switch (durum)
            {
                case 0:
                    ProfilButton.SetColorFilter(Color.ParseColor("#1a237e"));
                    ProfilDetaylariBaseFragment ProfilDetaylariBaseFragment1 = new ProfilDetaylariBaseFragment(Kullanici);
                    iceriklinearhazne.RemoveAllViews();
                    ft = this.Activity.SupportFragmentManager.BeginTransaction();
                    ft.AddToBackStack(null);
                    ft.Replace(Resource.Id.iceriklinearhazne, ProfilDetaylariBaseFragment1);//
                    ft.Commit();
                    break;
                case 1:
                    EventlerButton.SetColorFilter(Color.ParseColor("#1a237e"));
                    ProfilEtkinliklerBaseFragment ProfilEtkinliklerBaseFragment1 = new ProfilEtkinliklerBaseFragment(Kullanici);
                    iceriklinearhazne.RemoveAllViews();
                    ft = this.Activity.SupportFragmentManager.BeginTransaction();
                    ft.AddToBackStack(null);
                    ft.Replace(Resource.Id.iceriklinearhazne, ProfilEtkinliklerBaseFragment1);//
                    ft.Commit();
                    break;
                case 2:
                    GaleriButton.SetColorFilter(Color.ParseColor("#1a237e"));
                    GaleriBaseFragment GaleriBaseFragment1 = new GaleriBaseFragment(Kullanici);
                    iceriklinearhazne.RemoveAllViews();
                    ft = this.Activity.SupportFragmentManager.BeginTransaction();
                    ft.AddToBackStack(null);
                    ft.Replace(Resource.Id.iceriklinearhazne, GaleriBaseFragment1);//
                    ft.Commit();
                    break;
                default:
                    break;
            }

        }

        void TakiSayisiniGetir()
        {
            WebService webService = new WebService();
            var Donus = webService.OkuGetir("user/"+Kullanici.id.ToString()+"/myFollowers");
            if (Donus != "Hata")
            {
                try
                {
                    var Modell = Newtonsoft.Json.JsonConvert.DeserializeObject<List<TakipcilerimRootView>>(Donus);
                    TakipciSayisiText.Text = Modell[0].my_followers.Count().ToString() + " Takipçi";
                }
                catch 
                {
                }
            }
            else
            {
                
            }
        }

        public class User
        {
            public string id { get; set; }
            public int type { get; set; }
            public string profile_photo { get; set; }
            public string cover_photo { get; set; }
            public string title { get; set; }
            public string name { get; set; }
            public string surname { get; set; }
            public string career_history { get; set; }
            public string short_biography { get; set; }
            public string credentials { get; set; }
            public string date_of_birth { get; set; }
            public string company_id { get; set; }
            public string company_title { get; set; }
            public string sector_id { get; set; }
            public string email { get; set; }
            public string email_verified_at { get; set; }
            public string status { get; set; }
            public string package { get; set; }
            public string created_at { get; set; }
            public string updated_at { get; set; }
        }
        public class FotografGuncelleModel
        {
            public int status { get; set; }
            public string message { get; set; }
            public User user { get; set; }
        }

        public class MyFollower
        {
            public int id { get; set; }
            public int type { get; set; }
            public string profile_photo { get; set; }
            public string cover_photo { get; set; }
            public string title { get; set; }
            public string name { get; set; }
            public string surname { get; set; }
            public string career_history { get; set; }
            public string short_biography { get; set; }
            public string credentials { get; set; }
            public string date_of_birth { get; set; }
            public int company_id { get; set; }
            public int sector_id { get; set; }
            public string email { get; set; }
            public int status { get; set; }
            public string package { get; set; }
            public string created_at { get; set; }
            public string updated_at { get; set; }
        }

        public class TakipcilerimRootView
        {
            public int id { get; set; }
            public int from_user_id { get; set; }
            public int to_user_id { get; set; }
            public string created_at { get; set; }
            public string updated_at { get; set; }
            public List<MyFollower> my_followers { get; set; }
        }

        public class UserPrivacy
        {
            public int id { get; set; }
            public int user_id { get; set; }
            public bool visibility_on_the_map { get; set; }
            public bool no_message { get; set; }
            public bool no_follow_up_request { get; set; }
            public string created_at { get; set; }
            public string updated_at { get; set; }
        }

    }

    public static class BilgileriGosterilecekKullanici
    {
        public static int UserID { get; set; } 

    }
}
