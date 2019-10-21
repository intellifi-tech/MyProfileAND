using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Content;
using Android.Views;
using Android.Widget;
using FFImageLoading;
using FFImageLoading.Transformations;
using FFImageLoading.Views;
using MyProfileAND.GenericClass;
using MyProfileAND.GenericUI;
using MyProfileAND.WebServiceHelper;
using Newtonsoft.Json;

namespace MyProfileAND.Favoriler.YeniEtkinlikOlustur
{
    [Activity(Label = "MyProfile")]
    public class YeniEtkinlikOlusturBaseActivity : Android.Support.V7.App.AppCompatActivity
    {
        #region Tanimlamalar
        EditText EtkinlikAciklamasi;
        ImageView ProfilFotografi;
        ImageButton ProfilFotoGuncelleButton;
        TextView giriSaat, cikiSaat,EtkinlikAdi;
        public static readonly int PickImageId = 1000;
        Android.Support.V7.Widget.Toolbar toolbar;
        Button EtkinlikKaydetButton;
        TextView FotografEkleText;
        LinearLayout FotoEkleHazne;
        #endregion
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.YeniEtkinlikOlusturBaseActivity);
            DinamikStatusBarColor DinamikStatusBarColor1 = new DinamikStatusBarColor();
            DinamikStatusBarColor1.Beyaz(this);
            EtkinlikAdi = FindViewById<TextView>(Resource.Id.textView1);
            EtkinlikAdi.Text = SecilenEventt.EventTitle;
            EtkinlikAdi.Selected = true;
            EtkinlikAciklamasi = FindViewById<EditText>(Resource.Id.textInputEditText2);
            giriSaat = FindViewById<TextView>(Resource.Id.bastarih);
            cikiSaat = FindViewById<TextView>(Resource.Id.bittarihi);
            ProfilFotografi = FindViewById<ImageView>(Resource.Id.imgPortada_item2);
            ProfilFotoGuncelleButton = FindViewById<ImageButton>(Resource.Id.ımageButton1);
            FotografEkleText = FindViewById<TextView>(Resource.Id.textView3);
            FotoEkleHazne = FindViewById<LinearLayout>(Resource.Id.fotoeklelinear);
            FotografEkleText.Click += FotografEkleText_Click;
            giriSaat.Click += GiriSaat_Click;
            cikiSaat.Click += CikiSaat_Click;
            ProfilFotografi.Click += ProfilFotografi_Click;
            EtkinlikKaydetButton = FindViewById<Button>(Resource.Id.button3);
            EtkinlikKaydetButton.Click += EtkinlikKaydetButton_Click;
            toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.app_bar);

            SetSupportActionBar(toolbar);
            if (SupportActionBar != null)
            {
                SupportActionBar.SetDisplayHomeAsUpEnabled(true);
                SupportActionBar.Title = "";
                toolbar.NavigationIcon.SetColorFilter(Android.Graphics.Color.Black, PorterDuff.Mode.SrcAtop);
            }
            
        }

        private void FotografEkleText_Click(object sender, EventArgs e)
        {
            var Intentt = new Intent();
            Intentt.SetType("image/*");
            Intentt.SetAction(Intent.ActionGetContent);
            StartActivityForResult(Intent.CreateChooser(Intentt, "Fotoğraf Seç"), PickImageId);
        }

        private void EtkinlikKaydetButton_Click(object sender, EventArgs e)
        {
            if (BosVarmi())
            {
                WebService webService = new WebService();
                EtkinlikOlusturDataModel EtkinlikOlusturDataModel1;
                if (SecilenEventt.EventID != 0)
                {
                    EtkinlikOlusturDataModel1 = new EtkinlikOlusturDataModel()
                    {
                        event_id = SecilenEventt.EventID,
                        title = SecilenEventt.EventTitle,
                        event_image = base64String,
                        event_description = EtkinlikAciklamasi.Text,
                        date_of_participation = Convert.ToDateTime(giriSaat.Text).ToString("yyyy-MM-dd") + " " + giriSaat.Text,
                        end_date = Convert.ToDateTime(cikiSaat.Text).ToString("yyyy-MM-dd") + " " + cikiSaat.Text,
                        latitude = SecilenEventt.Konum.Latitude.ToString(System.Globalization.CultureInfo.InvariantCulture),
                        longitude = SecilenEventt.Konum.Longitude.ToString(System.Globalization.CultureInfo.InvariantCulture),
                    };
                }
                else
                {
                    EtkinlikOlusturDataModel1 = new EtkinlikOlusturDataModel()
                    {
                        title = SecilenEventt.EventTitle,
                        event_image = base64String,
                        event_description = EtkinlikAciklamasi.Text,
                        date_of_participation = Convert.ToDateTime(giriSaat.Text).ToString("yyyy-MM-dd") + " " + giriSaat.Text,
                        end_date = Convert.ToDateTime(cikiSaat.Text).ToString("yyyy-MM-dd") + " " + cikiSaat.Text,
                        latitude = SecilenEventt.Konum.Latitude.ToString(System.Globalization.CultureInfo.InvariantCulture),
                        longitude = SecilenEventt.Konum.Longitude.ToString(System.Globalization.CultureInfo.InvariantCulture),
                    };
                }
               
                var jsonstring = JsonConvert.SerializeObject(EtkinlikOlusturDataModel1);
                var Donus = webService.ServisIslem("user/userAttendedEvent", jsonstring);
                if (Donus != "Hata")
                {
                    AlertHelper.AlertGoster("Etkinlik katılımı oluşturuldu!", this);
                    this.Finish();
                    return;
                }
                else
                {
                    AlertHelper.AlertGoster("Bu etkinliğe katılamazsınız!",this);
                    return;
                }
            }
        }

        bool BosVarmi()
        {
            if (base64String == "")
            {
                AlertHelper.AlertGoster("Lütfen Bir Etkinlik Fotoğrafı Seçin.", this);
                return false;
            }
            else if (String.IsNullOrEmpty(EtkinlikAciklamasi.Text.Trim()))
            {
                AlertHelper.AlertGoster("Lütfen Etkinlik Detayını Belirtin.", this);
                return false;
            }
            else if (String.IsNullOrEmpty(giriSaat.Text.Trim()))
            {
                AlertHelper.AlertGoster("Lütfen etkinliğe katıldığınız zamanı belirtin.", this);
                return false;
            }
            else if (String.IsNullOrEmpty(cikiSaat.Text.Trim()))
            {
                AlertHelper.AlertGoster("Lütfen etkinlikten çıkış yapacağınız saati belirtin.", this);
                return false;
            }
            else
            {
                return true;
            }
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
        private void ProfilFotografi_Click(object sender, EventArgs e)
        {
            var Intentt = new Intent();
            Intentt.SetType("image/*");
            Intentt.SetAction(Intent.ActionGetContent);
            StartActivityForResult(Intent.CreateChooser(Intentt, "Fotoğraf Seç"), PickImageId);
        }
        private void CikiSaat_Click(object sender, EventArgs e)
        {
            Saat_Secim frag = Saat_Secim.NewInstance(delegate (string time)
            {
                cikiSaat.Text = time;
            });
            frag.Show(this.SupportFragmentManager, Tarih_Cek.TAG);
        }
        private void GiriSaat_Click(object sender, EventArgs e)
        {
            Saat_Secim frag = Saat_Secim.NewInstance(delegate (string time)
            {
                giriSaat.Text = time;

            });
            frag.Show(this.SupportFragmentManager, Tarih_Cek.TAG);
        }

        string base64String = "";
        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            if ((requestCode == PickImageId) && (resultCode == (Result.Ok) && (data != null)))
            {
                try
                {
                    Android.Net.Uri uri = data.Data;

                    using (var inputStream = this.ContentResolver.OpenInputStream(uri))
                    {
                        using (var streamReader = new StreamReader(inputStream))
                        {
                            var bytes = default(byte[]);
                            using (var memstream = new MemoryStream())
                            {
                                streamReader.BaseStream.CopyTo(memstream);
                                bytes = memstream.ToArray();
                                base64String = Convert.ToBase64String(bytes);
                                Stream srm = memstream;
                                var FilePath = System.IO.Path.Combine(documentsFolder(), "PPImagege.jpg");
                                File.WriteAllBytes(FilePath, bytes);
                                if (File.Exists(FilePath))
                                {
                                    ProfilFotografi.SetScaleType(ImageView.ScaleType.CenterCrop);
                                    ProfilFotografi.SetImageURI(uri);
                                    FotoEkleHazne.Visibility = ViewStates.Gone;
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
        public static string documentsFolder()
        {
            string path;
            path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            Directory.CreateDirectory(path);
            return path;
        }
        void FotoGuncelle(string base644, int Durum)
        {

        }

 



  
        #region On Pause
      
        #endregion

        #region  DataModel

        public class EtkinlikOlusturDataModel
        {
            public int event_id { get; set; }
            public string title { get; set; }
            public string event_description { get; set; }
            public string event_image { get; set; }
            public string date_of_participation { get; set; }
            public string end_date { get; set; }
            public string longitude { get; set; }
            public string latitude { get; set; }
        }

        #endregion


     




    }

}