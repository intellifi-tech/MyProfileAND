using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using MyProfileAND.DataBasee;
using MyProfileAND.GenericClass;
using MyProfileAND.GenericUI;
using MyProfileAND.WebServiceHelper;
using Newtonsoft.Json;

namespace MyProfileAND.Profil.Galeri
{
    public class GaleriBaseFragment : Android.Support.V4.App.Fragment
    {
        #region Tanimlamalar
        RecyclerView mRecyclerView;
        RecyclerView.LayoutManager mLayoutManager;
        GaleriRecyclerViewAdapter mViewAdapter;
        List<UserGallery> UserGallery1 = new List<UserGallery>();
        public static readonly int PickImageId = 1000;
        USER_INFO Kullanici;
        #endregion

        public GaleriBaseFragment(USER_INFO Kullanici2)
        {
            Kullanici = Kullanici2;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View RootView = inflater.Inflate(Resource.Layout.GaleriBaseFragment, container, false);
            mRecyclerView = RootView.FindViewById<RecyclerView>(Resource.Id.recyclerView1);
            mRecyclerView.HasFixedSize = true;
            
            return RootView;
        }


        public override void OnStart()
        {
            base.OnStart();
            GaleriyiGetir();
        }

        private void MViewAdapter_ItemClick1(object sender, object[] e)
        {
            if ((int)e[0] == 0)
            {
                var Intentt = new Intent();
                Intentt.SetType("image/*");
                Intentt.SetAction(Intent.ActionGetContent);
                StartActivityForResult(Intent.CreateChooser(Intentt, "Fotoğraf Seç"), PickImageId);
            }
            else
            {
                if (Build.VERSION.SdkInt < BuildVersionCodes.Lollipop)
                {
                    
                }
                else
                {
                    SecilenFotograf.Link = UserGallery1[(int)e[0]].photo_name;
                    SecilenFotograf.created_at = UserGallery1[(int)e[0]].created_at;
                    SecilenFotograf.id = UserGallery1[(int)e[0]].id;
                    SecilenFotograf.rating = UserGallery1[(int)e[0]].rating;
                    SecilenFotograf.updated_at = UserGallery1[(int)e[0]].updated_at;
                    SecilenFotograf.user_id = UserGallery1[(int)e[0]].user_id;

                    var view = ((Android.Views.View)e[1]);
                    var Imagevieww = view.FindViewById<ImageView>(Resource.Id.imgPortada_item2);
                    SecilenFotograf.bitmap = ((BitmapDrawable)Imagevieww.Drawable).Bitmap;
                    var intent = new Intent(this.Activity, typeof(GaleriDetayBaseActivity));
                    var options = ActivityOptionsCompat.MakeSceneTransitionAnimation(this.Activity, view, "test");
                    StartActivity(intent, options.ToBundle());
                }
            }
        }

        private void MViewAdapter_ItemClick(object sender, int e)
        {
            
        }

        public override void OnActivityResult(int requestCode, int resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if ((requestCode == PickImageId) && (resultCode == (int)Result.Ok) && (data != null))
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
                                string base64String = Convert.ToBase64String(bytes);
                                Stream srm = memstream;
                                var FilePath = System.IO.Path.Combine(documentsFolder(), "NewGaleriImage.jpg");
                                File.WriteAllBytes(FilePath, bytes);
                                if (File.Exists(FilePath))
                                {
                                    FotoGuncelle(base64String);
                                }
                            }
                        }
                    }
                }
                catch
                {
                }

            }
        }

        void FotoGuncelle(string base644)
        {
            WebService webService = new WebService();
            AddnewPhotoClass AddnewPhotoClass1 = new AddnewPhotoClass();
            AddnewPhotoClass1.photo = base644;
            string jsonString = JsonConvert.SerializeObject(AddnewPhotoClass1);
            var Donus = webService.ServisIslem("user/addPhoto", jsonString);
            if (Donus != "Hata")
            {
                var YeniEklenenResim = Newtonsoft.Json.JsonConvert.DeserializeObject<UserGallery>(Donus);
                UserGallery1.Insert(1, YeniEklenenResim);
                mViewAdapter.NotifyDataSetChanged();
                return;
            }
            else
            {
                AlertHelper.AlertGoster("Bir sorun oluştu.", this.Activity);
                return;
            }
        }

        void GaleriyiGetir()
        {


            #region Genislik Alır
            Display display = this.Activity.WindowManager.DefaultDisplay;
            Point size = new Point();
            display.GetSize(size);
            int width = size.X;
            int height = size.Y;
            var Genislik = width / 3;
            #endregion

            WebService webService = new WebService();
            var Donus = webService.OkuGetir("user/" + Kullanici.id.ToString() + "/gallery");
            if (Donus != null)
            {
                var Modell = Newtonsoft.Json.JsonConvert.DeserializeObject<GaleriRecyclerViewDataModel>(Donus);
                UserGallery1 = Modell.userGallery;

                UserGallery1.Reverse();

                if (DataBase.USER_INFO_GETIR()[0].id == BilgileriGosterilecekKullanici.UserID.ToString())
                {
                    UserGallery1.Insert(0, new UserGallery() { AddNewPhoto = true });
                }
                
                mViewAdapter = new GaleriRecyclerViewAdapter(UserGallery1, (Android.Support.V7.App.AppCompatActivity)this.Activity, Genislik);
                mRecyclerView.SetAdapter(mViewAdapter);
                mViewAdapter.ItemClick += MViewAdapter_ItemClick1;
                var layoutManager = new GridLayoutManager(this.Activity, 3);
                mRecyclerView.SetLayoutManager(layoutManager);
            }
            else
            {
                if (DataBase.USER_INFO_GETIR()[0].id == BilgileriGosterilecekKullanici.UserID.ToString())
                {
                    UserGallery1 = new List<UserGallery>();
                    UserGallery1.Insert(0, new UserGallery() { AddNewPhoto = true });
                }

                mViewAdapter = new GaleriRecyclerViewAdapter(UserGallery1, (Android.Support.V7.App.AppCompatActivity)this.Activity, Genislik);
                mRecyclerView.SetAdapter(mViewAdapter);
                mViewAdapter.ItemClick += MViewAdapter_ItemClick1;
                var layoutManager = new GridLayoutManager(this.Activity, 3);
                mRecyclerView.SetLayoutManager(layoutManager);
            }
        }

        public static string documentsFolder()
        {
            string path;
            path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            Directory.CreateDirectory(path);
            return path;
        }

        public class AddnewPhotoClass
        {
            public string photo { get; set; }
        }




    }
}