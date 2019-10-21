using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace MyProfileAND.Mesajlarr
{
    public class Message
    {
        public int id { get; set; }
        public int type { get; set; }
        public object parent_id { get; set; }
        public int from_user_id { get; set; }
        public int to_user_id { get; set; }
        public string message { get; set; }
        public int status { get; set; }
        public int end_message { get; set; }
        public object created_at { get; set; }
        public object updated_at { get; set; }
    }

    public class ChatItems
    {
        public int status { get; set; }
        public string message { get; set; }
        public List<Message> messages { get; set; }
    }




}