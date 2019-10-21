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
using SQLite;

namespace MyProfileAND.DataBasee
{
    class DataModel
    {
        public DataModel()
        {

        }
    }
    
    public class USER_INFO
    {
         [PrimaryKey, AutoIncrement]
        public int localid { get; set; }
        public string api_token { get; set; }
        public string id { get; set; }
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
        public string status { get; set; }
    }

    public class BEGENILEN_FOTOLAR
    {
        [PrimaryKey, AutoIncrement]
        public int id { get; set; }
        public int foto_id { get; set; }

    }

    public class SON_KONUM_GUNCELLEMESI
    {
        [PrimaryKey, AutoIncrement]
        public int id { get; set; }
        public double lat { get; set; }
        public double lon { get; set; }
        public DateTime GuncellemeZamani { get; set; }

    }

}