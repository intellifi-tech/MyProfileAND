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
using Android.Text;
using Android.Text.Style;
using Android.Views;
using Android.Widget;
using ImageViews.Photo;
using Java.IO;
using MyProfileAND.DataBasee;
using MyProfileAND.GenericClass;
using MyProfileAND.GenericUI;
using MyProfileAND.WebServiceHelper;
using Newtonsoft.Json;
using Square.OkHttp;
using Square.Picasso;

namespace MyProfileAND.Profil.Galeri
{
    [Activity(Label = "MyProfile")]
    public class GaleriDetayBaseActivity : Android.Support.V7.App.AppCompatActivity
    {
        #region Tanimlamalar
        PhotoView photoView;
        Android.Support.V7.Widget.Toolbar toolbar;

        ImageButton Begenbutton,ProfilFotosuYap,Sil;
        TextView BegeniSayisi;

        List<BEGENILEN_FOTOLAR> BegenilenFotolar = new List<BEGENILEN_FOTOLAR>();
        bool FotoBegenildimi = false;
        #endregion
        protected override void OnCreate(Bundle savedInstanceState)
        {
            //mipmap/like_icon_img
            //drawable/favorite
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.GaleriDetayBaseActivity);
            DinamikStatusBarColor DinamikStatusBarColor1 = new DinamikStatusBarColor();
            DinamikStatusBarColor1.Trans(this,true);
            photoView = FindViewById<PhotoView>(Resource.Id.iv_photo);
            photoView.SetImageBitmap(SecilenFotograf.bitmap);
            toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar1);
            SetSupportActionBar(toolbar);
            if (SupportActionBar != null)
            {
                SupportActionBar.SetDisplayHomeAsUpEnabled(true);
                SupportActionBar.Title = "";
                toolbar.NavigationIcon.SetColorFilter(Android.Graphics.Color.White, PorterDuff.Mode.SrcAtop);
            }

            Begenbutton = FindViewById<ImageButton>(Resource.Id.begenbutton);
            ProfilFotosuYap = FindViewById<ImageButton>(Resource.Id.ımageButton2);
            Sil = FindViewById<ImageButton>(Resource.Id.ımageButton1);
            BegeniSayisi = FindViewById<TextView>(Resource.Id.textView1);
            BegeniSayisi.Click += BegeniSayisi_Click;

            Begenbutton.Click += Begenbutton_Click;
            ProfilFotosuYap.Click += ProfilFotosuYap_Click;
            Sil.Click += Sil_Click;
            BegeniSayisiniGetir();

            BegenilenFotolar = DataBase.BEGENILEN_FOTOLAR_GETIR();
            if (BegenilenFotolar.Count > 0)
            {
                var Bufoto = BegenilenFotolar.FindAll(item => item.foto_id == SecilenFotograf.id);
                if (Bufoto.Count > 0)
                {
                    Begenbutton.SetImageResource(Resource.Mipmap.like_icon_img);
                    Begenbutton.ClearColorFilter();
                    FotoBegenildimi = true;
                }
            }

        }

        private void BegeniSayisi_Click(object sender, EventArgs e)
        {
            FotografiBegen();
        }

        private void Sil_Click(object sender, EventArgs e)
        {
            AlertDialog.Builder cevap = new AlertDialog.Builder(this);
            cevap.SetIcon(Resource.Mipmap.ic_launcher);
            cevap.SetTitle(Spannla(Color.Black, "MyProfile"));
            cevap.SetMessage(Spannla(Color.DarkGray, "Fotoğrafı silmek istediğinizden emin misniz?"));
            cevap.SetPositiveButton(Spannla(Color.Black, "Evet"), delegate
            {
                FotografSil();
                cevap.Dispose();
            });
            cevap.SetNegativeButton(Spannla(Color.Black, "Hayır"), delegate
            {
                cevap.Dispose();
            });
            cevap.Show();
        }
        private void ProfilFotosuYap_Click(object sender, EventArgs e)
        {
            AlertDialog.Builder cevap = new AlertDialog.Builder(this);
            cevap.SetIcon(Resource.Mipmap.ic_launcher);
            cevap.SetTitle(Spannla(Color.Black, "MyProfile"));
            cevap.SetMessage(Spannla(Color.DarkGray, "Profil fotoğrafı olarak ayarlamak istiyor musunuz?"));
            cevap.SetPositiveButton(Spannla(Color.Black, "Evet"), delegate
            {
                using (var stream = new MemoryStream())
                {
                    SecilenFotograf.bitmap.Compress(Bitmap.CompressFormat.Png, 100, stream);
                    var bytes = stream.ToArray();
                    string base64String = Convert.ToBase64String(bytes);
                    FotoGuncelle(base64String);
                }
                cevap.Dispose();
            });
            cevap.SetNegativeButton(Spannla(Color.Black, "Hayır"), delegate
            {
                cevap.Dispose();
            });
            cevap.Show();
        }
        SpannableStringBuilder Spannla(Color Renk, string textt)
        {
            ForegroundColorSpan foregroundColorSpan = new ForegroundColorSpan(Renk);

            string title = textt;
            SpannableStringBuilder ssBuilder = new SpannableStringBuilder(title);
            ssBuilder.SetSpan(
                    foregroundColorSpan,
                    0,
                    title.Length,
                    SpanTypes.ExclusiveExclusive
            );

            return ssBuilder;
        }
        private void Begenbutton_Click(object sender, EventArgs e)
        {
            FotografiBegen();
        }

        void FotografiBegen()
        {
            var Bufoto = BegenilenFotolar.FindAll(item => item.foto_id == SecilenFotograf.id);
            if (Bufoto.Count <= 0)
            {
                WebService webService = new WebService();
                var Donus = webService.OkuGetir("photo/" + SecilenFotograf.id.ToString() + "/likePhoto");
                if (Donus != null)
                {
                    Begenbutton.SetImageResource(Resource.Mipmap.like_icon_img);
                    Begenbutton.SetColorFilter(null);
                    FotoBegenildimi = true;
                    DataBase.BEGENILEN_FOTOLAR_EKLE(new BEGENILEN_FOTOLAR() {foto_id = SecilenFotograf.id });
                    SecilenFotograf.rating++;
                    BegeniSayisiniGetir();
                    BegenilenFotolar = DataBase.BEGENILEN_FOTOLAR_GETIR();
                }
            }
        }

        void FotografSil()
        {
            WebService webService = new WebService();
            var Donus = webService.OkuGetir("photo/" + SecilenFotograf.id.ToString() + "/deletePhoto");
            if (Donus != null)
            {
                AlertHelper.AlertGoster("Fotoğraf silindi", this);
                this.Finish();
            }
        }

        void FotoGuncelle(string base644)
        {
            WebService webService = new WebService();
            var Kullanici = DataBase.USER_INFO_GETIR()[0];
            
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
                    AlertHelper.AlertGoster("Profil Fotoğrafınız Güncellendi.", this);
                    
                    return;
                }
                else
                {
                    AlertHelper.AlertGoster("Bir sorun oluştu.", this);
                    return;
                }
        }

        void BegeniSayisiniGetir()
        {
            if (SecilenFotograf.rating == 0)
            {
                BegeniSayisi.Text = "İlk beğenen sen ol!";
            }
            else
            {
                BegeniSayisi.Text = SecilenFotograf.rating.ToString() + " kişi beğendi";
            }
        }

        protected override void OnStart()
        {
            base.OnStart();
            
            new System.Threading.Thread(new System.Threading.ThreadStart(delegate
            {
                System.Threading.Thread.Sleep(1000);
                try
                {
                    OkHttpClient client = new OkHttpClient();
                    client.SetProtocols(new List<Protocol>() { Protocol.Http11 });
                    Picasso picasso = new Picasso.Builder(this)
                    .Downloader(new OkHttpDownloader(client))
                    .Build();
                    this.RunOnUiThread(() =>
                    {
                        picasso.Load(SecilenFotograf.Link)
                        .Error(Resource.Mipmap.ic_launcher)
                             .Into(photoView);
                    });

                    

                }
                catch
                {


                }
            })).Start();
            
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    this.Finish();
                    return true;
                default:
                    return base.OnOptionsItemSelected(item);
            }
        }
       
    }
    public static class SecilenFotograf
    {
        public static string Link { get; set; }
        public static Bitmap bitmap { get; set; }
        public static int id { get; set; }
        public static int user_id { get; set; }
        public static int rating { get; set; }
        public static string created_at { get; set; }
        public static string updated_at { get; set; }
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
}