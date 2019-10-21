using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using MyProfileAND.DataBasee;
using MyProfileAND.WebServiceHelper;
using static MyProfileAND.Mesajlar.ChatActivity;

namespace MyProfileAND.Mesajlar
{
    public class MesajlarBaseFragment : Android.Support.V4.App.Fragment
    {
        #region Tanimlamalar
        ListView Liste;
        List<Message> mFriends;
        ChatFriendListAdapter mAdapter;
        Mesaj_Listesi_RootObject Mesaj_Listesi_RootObject1;
        #endregion
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
           View RootView= inflater.Inflate(Resource.Layout.MesajlarBaseFragment, container, false);
            Liste = RootView.FindViewById<ListView>(Resource.Id.listView1);
            Liste.ItemClick += Liste_ItemClick;
            return RootView;
        }

        private void Liste_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var MeId = DataBase.USER_INFO_GETIR()[0].id;
            if (Mesaj_Listesi_RootObject1.messages[e.Position].from_user_id != MeId)
            {
                ChatUserId.UserID = Mesaj_Listesi_RootObject1.messages[e.Position].from_user_id;
            }
            else
            {
                ChatUserId.UserID = Mesaj_Listesi_RootObject1.messages[e.Position].to_user_id;
            }
            this.Activity.StartActivity(typeof(ChatActivity));
        }
        public override void OnStart()
        {
            base.OnStart();
            SonMesajlariGetir();
        }

        void SonMesajlariGetir()
        {   
            mFriends = new List<Message>();
            WebService webService = new WebService();
            var Donus = webService.OkuGetir("message/getMessages");
            if (Donus != null)
            {
                var A = Donus.ToString();
                if (Donus != null)
                {
                    var ddddd = Donus.ToString();
                    Mesaj_Listesi_RootObject1 = Newtonsoft.Json.JsonConvert.DeserializeObject<Mesaj_Listesi_RootObject>(Donus.ToString());
                    mFriends = Mesaj_Listesi_RootObject1.messages;
                }
                if (mFriends.Count > 0)
                {
                    mAdapter = new ChatFriendListAdapter(this.Activity, Resource.Layout.ChatFriendsCustomView, mFriends);
                    mFriends.Reverse();
                    Liste.Adapter = mAdapter;
                }
            }
        }

        public class FromUser
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
            public string email_verified_at { get; set; }
            public int status { get; set; }
            public string package { get; set; }
            public string created_at { get; set; }
            public string updated_at { get; set; }
        }

        public class ToUser
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
            public string email_verified_at { get; set; }
            public int status { get; set; }
            public string package { get; set; }
            public string created_at { get; set; }
            public string updated_at { get; set; }
        }

        public class Message
        {
            public int id { get; set; }
            public int type { get; set; }
            public string parent_id { get; set; }
            public string from_user_id { get; set; }
            public string to_user_id { get; set; }
            public string message { get; set; }
            public int status { get; set; }
            public int end_message { get; set; }
            public string created_at { get; set; }
            public string updated_at { get; set; }
            public FromUser from_user { get; set; }
            public ToUser to_user { get; set; }
        }

        public class Mesaj_Listesi_RootObject
        {
            public int status { get; set; }
            public string message { get; set; }
            public List<Message> messages { get; set; }
        }
    }
}