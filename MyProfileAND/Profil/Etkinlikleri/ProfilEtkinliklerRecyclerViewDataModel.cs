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
using Newtonsoft.Json;

namespace MyProfileAND.Profil.Etkinlikleri
{
    public class Event
    {
        public int id { get; set; }
        public string title { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
    }

    public class UserAttendedEvent
    {
        public int id { get; set; }
        public int event_id { get; set; }
        public int user_id { get; set; }
        public string event_description { get; set; }
        public string event_image { get; set; }
        public string date_of_participation { get; set; }
        public string end_date { get; set; }
        public int rating { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
        [JsonProperty("event")]
        public Event Event { get; set; }
    }

    public class ProfilEtkinliklerRecyclerViewDataModel
    {
        public int status { get; set; }
        public string message { get; set; }
        public List<UserAttendedEvent> userAttendedEvents { get; set; }
    }



}