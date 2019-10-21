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
using System.Net;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Json;
using System.Threading.Tasks;
using MyProfileAND.DataBasee;

namespace MyProfileAND.WebServiceHelper
{
    class WebService
    {
        string kokurl = "https://demo.intellifi.tech/demo/MyProfile/web/public/api/";
        public string ServisIslem(string url, string istekler, bool isLogin = false, string Method = "POST")
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(kokurl + url);
                request.Method = Method;
                request.ContentType = "application/json";
                request.Accept = "*/*";

                if (!isLogin)
                {
                    request.Headers["api-token"] = TokenGetir();
                }

                byte[] _byteVersion = Encoding.UTF8.GetBytes(string.Concat(istekler));

                request.ContentLength = _byteVersion.Length;

                Stream stream = request.GetRequestStream();
                stream.Write(_byteVersion, 0, _byteVersion.Length);
                stream.Close();

                // JsonValue jsonDoc = await Task.Run(() => JsonObject.Load(stream));

                //string  aa =  Ayristir(jsonDoc);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {

                    string aas = reader.ReadToEnd();
                    //var bob = Newtonsoft.Json.Linq.JObject.Parse(aas);
                    var json_string = aas.ToString().Replace(@"\","");
                    return json_string;
                }
            }
            catch (Exception exx)
            {
                string exmes = exx.Message.ToString();
                return "Hata";
            }
           
        }

        public string OkuGetir(string url, bool isLogin = false)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(new Uri(kokurl + url));
                request.ContentType = "application/json";
                request.Method = "GET";

                if (!isLogin)
                {
                    request.Headers["api-token"] = TokenGetir();
                }
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                        Console.Out.WriteLine("Error fetching data. Server returned status code: {0}", response.StatusCode);
                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        var aas = reader.ReadToEnd();
                        return aas.ToString().Replace(@"\", "");
                        // Assert.NotNull(content);
                    }
                }
            }
            catch (Exception Ex)
            {
                var durum = Ex.Message.ToString();
                return null;
            }

        }


        string TokenGetir()
        {
            if (OkunanTokenClass.Tokenn == "")
            {
                var UserToken = DataBase.USER_INFO_GETIR();

                if (UserToken.Count > 0)
                {
                    var tooo = UserToken[0].api_token;
                    OkunanTokenClass.Tokenn = UserToken[0].api_token;
                    return OkunanTokenClass.Tokenn;
                }
                else
                {
                    return "";
                }
            }
            else
            {
                return OkunanTokenClass.Tokenn;
            }
            
        }
    }
    public static class OkunanTokenClass
    {
        public static string Tokenn { get; set; } = "";
    }
}