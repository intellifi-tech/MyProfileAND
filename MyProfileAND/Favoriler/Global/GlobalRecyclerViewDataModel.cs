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

namespace MyProfileAND.Favoriler.Global
{

    public class User
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
        public string status { get; set; }
        public string package { get; set; }
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
        public DateTime created_at { get; set; }
        public string updated_at { get; set; }
        public User user { get; set; }
    }

    public class Event
    {
        public int id { get; set; }
        public string title { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
        public List<UserAttendedEvent> user_attended_event { get; set; }
    }

    public class GlobalRecyclerViewDataModel_For_JSON
    {
        public int status { get; set; }
        public string message { get; set; }
        public List<Event> events { get; set; }
    }


    //CUSTOM MODEL
    public class GlobalRecyclerViewDataModel
    {
        public UserAttendedEvent user_attended_event { get; set; }
        public Event Events { get; set; }
    }










}