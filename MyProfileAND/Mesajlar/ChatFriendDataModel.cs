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

namespace MyProfileAND.Mesajlar
{
    public class To
    {
        public int member_id { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string job { get; set; }
        public string avatar { get; set; }
        public bool status { get; set; }
        public bool approved { get; set; }
    }

    public class From
    {
        public int member_id { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string job { get; set; }
        public string avatar { get; set; }
        public bool status { get; set; }
        public bool approved { get; set; }
    }

    public class LastFrom
    {
        public int member_id { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string job { get; set; }
        public string avatar { get; set; }
        public bool status { get; set; }
        public bool approved { get; set; }
    }

    public class Message
    {
        public string message_id { get; set; }
        public string to_id { get; set; }
        public string from_id { get; set; }
        public To to { get; set; }
        public From from { get; set; }
        public string last_message { get; set; }
        public LastFrom last_from { get; set; }
        public string last_date { get; set; }
        public bool reading { get; set; }
        public string current { get; set; }
    }

    public class ChatFriendDataModel
    {
        public int total_messages { get; set; }
        public List<Message> messages { get; set; }
    }
}