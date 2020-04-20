using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using Android.Animation;
using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using FFImageLoading;
using FFImageLoading.Transformations;
using FFImageLoading.Views;
using MyProfileAND.DataBasee;
using MyProfileAND.GenericClass;
using MyProfileAND.GenericUI;
using MyProfileAND.Profil;
using MyProfileAND.Profil.FarkliKullanici;
using MyProfileAND.WebServiceHelper;
using Newtonsoft.Json;
using Org.Json;

namespace MyProfileAND.Mesajlar
{
    [Activity(Label = "MyProfile")]
    public class ChatActivity : Android.Support.V7.App.AppCompatActivity
    {

        #region Tanimlamaalar
        ImageButton Gonder;
        EditText Mesaj;
        DinamikStatusBarColor DinamikStatusBarColor1 = new DinamikStatusBarColor();
        Android.Support.V7.Widget.Toolbar toolbar;
        ListView Liste;
        List<MyProfileAND.Mesajlarr.Message> mItems;
        ChatListeAdaptoru mAdapter;
        InputMethodManager imm;
        Resources genelResources;

        ImageViewAsync HostImageView;
        public Bitmap HostImage, MeImage;
        public string HostImagePath, UserImagePath;
        MyProfileAND.Mesajlarr.ChatItems ChatItemsss;
        TextView HostNameText;

        #endregion  
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.ChatActivity);
            DinamikStatusBarColor1.Beyaz(this);
            Bundle extras = Intent.Extras;
            genelResources = Resources;
            Gonder = FindViewById<ImageButton>(Resource.Id.ımageButton5);
            Liste = FindViewById<ListView>(Resource.Id.listView1);
            Mesaj = FindViewById<EditText>(Resource.Id.editText1);
            HostImageView = FindViewById<ImageViewAsync>(Resource.Id.imgPortada_item);
            HostNameText = FindViewById<TextView>(Resource.Id.usernametxt);
            Gonder.Click += Gonder_Click;
            Mesaj.SetTextColor(Color.Black);
            Mesaj.RequestFocus();
            Mesaj.ClipToOutline = true;
            toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.app_bar);
            SetSupportActionBar(toolbar);
            if (SupportActionBar != null)
            {
                SupportActionBar.SetDisplayHomeAsUpEnabled(true);
                toolbar.NavigationIcon.SetColorFilter(Android.Graphics.Color.Black, PorterDuff.Mode.SrcAtop);
                SupportActionBar.Title = "";
            }
            mItems = new List<MyProfileAND.Mesajlarr.Message>();
            imm = (InputMethodManager)this.GetSystemService(Context.InputMethodService);
            imm.HideSoftInputFromInputMethod(Mesaj.WindowToken, 0);
            this.Window.SetSoftInputMode(SoftInput.StateHidden);
            ChatItemsss = new MyProfileAND.Mesajlarr.ChatItems();
            ChatItemsss.messages = new List<MyProfileAND.Mesajlarr.Message>();
            SetUserInformation();
            MesajlariCek();
            MesajlariOkunduYap();
            if (mItems.Count > 0)
            {
                mAdapter = new ChatListeAdaptoru(Resources, this, Resource.Layout.ChatCustomCell, mItems);
                Liste.Adapter = mAdapter;
            }
            MessageListenerr();
            HostImageView.Click += HostImageView_Click;
        }

        private void HostImageView_Click(object sender, EventArgs e)
        {
            BilgileriGosterilecekKullanici.UserID = HostModelll.id;
            this.StartActivity(typeof(FarkliKullaniciBaseActivity));
        }
        User_RootObject HostModelll;
        void SetUserInformation()
        {
            WebService webService = new WebService();
            var Donus = webService.OkuGetir("user/"+ ChatUserId.UserID+ "/show");
            if (Donus != null)
            {
                HostModelll =   Newtonsoft.Json.JsonConvert.DeserializeObject<User_RootObject>(Donus.ToString());
                HostNameText.Text = HostModelll.name + " " + HostModelll.surname;
                ImageService.Instance.LoadUrl("http://23.97.222.30"+HostModelll.profile_photo)
                                                    .Transform(new CircleTransformation(5, "#FFFFFF"))
                                                    .Into(HostImageView);
                HostImagePath = HostModelll.profile_photo;
                UserImagePath = DataBase.USER_INFO_GETIR()[0].profile_photo;

            }
        }
        public void GetBitmapFromUrl(string url, ImageView GelenImageView = null,bool HostOrMe = true)
        {
            //new System.Threading.Thread(new System.Threading.ThreadStart(delegate
            //{
                using (WebClient webClient = new WebClient())
                {
                    byte[] bytes = webClient.DownloadData(url);

                    if (bytes != null && bytes.Length > 0)
                    {
                        this.RunOnUiThread(() =>
                        {
                            var Imagee = BitmapFactory.DecodeByteArray(bytes, 0, bytes.Length);
                            if (HostOrMe)
                            {
                                if (GelenImageView != null)
                                {
                                    GelenImageView.SetImageBitmap(Imagee);
                                }
                                HostImage = Imagee;
                            }
                            else
                            {
                                MeImage = Imagee;
                            }
                            
                        });
                    }
                }

            //})).Start();
        }
        public static void hideSoftKeyboard(Activity activity)
        {
            InputMethodManager inputMethodManager =
                (InputMethodManager)activity.GetSystemService(
                    Activity.InputMethodService);
            inputMethodManager.HideSoftInputFromWindow(
                activity.CurrentFocus.WindowToken, 0);
        }
        public void AcKapat(View Gelenview, bool durum, int boyut)
        {
            int sayac1 = Gelenview.Height;
            if (durum == false)
            {
                Gelenview.Visibility = ViewStates.Visible;
                int widthSpec = View.MeasureSpec.MakeMeasureSpec(0, MeasureSpecMode.Unspecified);
                int heightSpec = View.MeasureSpec.MakeMeasureSpec(0, MeasureSpecMode.Unspecified);
                Gelenview.Measure(widthSpec, heightSpec);

                ValueAnimator mAnimator = slideAnimator(Gelenview, 0, Gelenview.MeasuredHeight);
                mAnimator.Start();
                durum = true;
            }
            else if (durum == true)
            {
                int finalHeight = Gelenview.Height;

                ValueAnimator mAnimator = slideAnimator(Gelenview, finalHeight, 0);
                mAnimator.Start();
                mAnimator.AnimationEnd += (object IntentSender, EventArgs arg) =>
                {
                    Gelenview.Visibility = ViewStates.Gone;
                };
                durum = false;
            }

        }
        private ValueAnimator slideAnimator(View Gelenvieww, int start, int end)
        {

            ValueAnimator animator = ValueAnimator.OfInt(start, end);
            //ValueAnimator animator2 = ValueAnimator.OfInt(start, end);
            //  animator.AddUpdateListener (new ValueAnimator.IAnimatorUpdateListener{
            animator.Update +=
                (object sender, ValueAnimator.AnimatorUpdateEventArgs e) => {
                    //  int newValue = (int)
                    //e.Animation.AnimatedValue; // Apply this new value to the object being animated.
                    //  myObj.SomeIntegerValue = newValue; 
                    var value = (int)animator.AnimatedValue;
                    LinearLayout.LayoutParams layoutParams = (LinearLayout.LayoutParams)Gelenvieww.LayoutParameters;
                    layoutParams.Height = value;
                    Gelenvieww.LayoutParameters = layoutParams;

                };


            //      });
            return animator;
        }
        private void Gonder_Click(object sender, EventArgs e)
        {
            if (Mesaj.Text.Trim() != "")
            {
                if (MesajAt())
                {
                    MesajlariCek();
                    if (mItems.Count>0)
                    {
                        mAdapter = new ChatListeAdaptoru(Resources, this, Resource.Layout.ChatCustomCell, mItems);
                        Liste.Adapter = mAdapter;
                        Liste.SmoothScrollToPosition(0);
                        Mesaj.Text = "";
                    }
                }
            }
            else
            {
                Mesaj.Text = "";
            }
        }
        void MesajlariOkunduYap()
        {
            WebService webService = new WebService();
            var Donus = webService.OkuGetir("message/"+ChatUserId.UserID+"/readMessage");
            if (Donus == null)
            {
                Toast.MakeText(this, "asd asd ada", ToastLength.Long).Show();
            }
        }
        bool MesajlariCek()
        {
            WebService webService = new WebService();
            var Donus = webService.OkuGetir("message/"+ ChatUserId.UserID+ "/userIndexMessages");
            if (Donus != null)
            {
                var Modell = Newtonsoft.Json.JsonConvert.DeserializeObject<MyProfileAND.Mesajlarr.ChatItems>(Donus.ToString());
                mItems = Modell.messages;
                return true;
            }
            else
            {
                return false;
            }
        }
        bool MesajAt()
        {
            WebService webService = new WebService();
            Mesaj_Gonder_RootObject Mesaj_Gonder_RootObject1 = new Mesaj_Gonder_RootObject()
            {
                message = Mesaj.Text,
                to_user_id = Convert.ToInt32(ChatUserId.UserID)
            };
            var jsonstring = JsonConvert.SerializeObject(Mesaj_Gonder_RootObject1);
            var Donus = webService.ServisIslem("message/sendMessage", jsonstring);
            if (Donus != "Hata")
            {
                return true;
            }
            else
            {
                AlertHelper.AlertGoster("Mesaj Gönderilemedi", this);
                return true;
            }

            
        }
        public class MesajRoot
        {
            public int to { get; set; }
            public int from { get; set; }
            public string message { get; set; }
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
        static string FnGetVideoID(string strVideoURL)
        {
            const string regExpPattern = @"youtu(?:\.be|be\.com)/(?:.*v(?:/|=)|(?:.*/)?)([a-zA-Z0-9-_]+)";
            //for Vimeo: vimeo\.com/(?:.*#|.*/videos/)?([0-9]+)
            var regEx = new Regex(regExpPattern);
            var match = regEx.Match(strVideoURL);
            return match.Success ? match.Groups[1].Value : null;
        }
        System.Threading.Timer _timer;
        void MessageListenerr()
        {
            new System.Threading.Thread(new System.Threading.ThreadStart(delegate
            {

                _timer = new System.Threading.Timer((o) =>
                {
                    try
                    {
                        var Durum = MesajlariCek();
                        this.RunOnUiThread(() =>
                        {
                            if (Durum) //İçerik  Değişmişse Uygula
                            {
                                mAdapter = new ChatListeAdaptoru(Resources, this, Resource.Layout.ChatCustomCell, mItems);
                                Liste.Adapter = mAdapter;
                                Liste.SmoothScrollToPosition(0);
                            }
                        });
                    }
                    catch
                    {
                    }

                }, null, 0, 3000);
            })).Start();
        }
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    //_timer.Dispose();
                    this.Finish();
                    return true;
                default:
                    return base.OnOptionsItemSelected(item);
            }
        }
        public override void OnBackPressed()
        {

        }

        public static class ChatUserId
        {
            public static string UserID { get; set; }
        }

        public class User_RootObject
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
            public string company_id { get; set; }
            public string sector_id { get; set; }
            public string email { get; set; }
            public int status { get; set; }
            public string package { get; set; }
            public string created_at { get; set; }
            public string updated_at { get; set; }
        }

        public class Mesaj_Gonder_RootObject
        {
            public int to_user_id { get; set; }
            public string message { get; set; }
        }
    }
}
