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
using System.IO;
using SQLite;

namespace MyProfileAND.DataBasee
{
    class DataBase
    {
        public DataBase()
        {
            CreateDataBase();
        }
        public static string documentsFolder()
        {
            string path;
            path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            Directory.CreateDirectory(path);
            return path;
        }
        public static void CreateDataBase()
        {
            var conn = new SQLiteConnection(System.IO.Path.Combine(documentsFolder(), "MyProfile.db"), false);
            conn.CreateTable<USER_INFO>();
            conn.CreateTable<BEGENILEN_FOTOLAR>();
            conn.CreateTable<SON_KONUM_GUNCELLEMESI>();
            conn.CreateTable<GIZLILIK>();
            conn.Close();
        }

        #region USER_INFO

        public static bool USER_INFO_EKLE(USER_INFO GelenDoluTablo)
        {
            try
            {
                var conn = new SQLiteConnection(System.IO.Path.Combine(documentsFolder(), "MyProfile.db"), false);
                conn.Insert(GelenDoluTablo);
                conn.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public static List<USER_INFO> USER_INFO_GETIR()
        {
            var conn = new SQLiteConnection(System.IO.Path.Combine(documentsFolder(), "MyProfile.db"), false);
            var gelenler = conn.Query<USER_INFO>("Select * From USER_INFO");
            conn.Close();
            return gelenler;
        }
        public static bool USER_INFO_TEMIZLE()
        {
            try
            {
                var conn = new SQLiteConnection(System.IO.Path.Combine(documentsFolder(), "MyProfile.db"), false);
                conn.Query<USER_INFO>("Delete From USER_INFO");
                conn.Close();
                return true;
            }
            catch (Exception e)
            {
                string ee = e.ToString();
                return false;
            }

        }
        public static bool USER_INFO_Guncelle(USER_INFO Tablo)
        {
            try
            {
                var conn = new SQLiteConnection(System.IO.Path.Combine(documentsFolder(), "MyProfile.db"), false);
                conn.Update(Tablo);
                conn.Close();
                return true;
            }
            catch (Exception e)
            {
                string ee = e.ToString();
                return false;
            }

        }
        #endregion

        #region BEGENILEN_FOTOLAR

        public static bool BEGENILEN_FOTOLAR_EKLE(BEGENILEN_FOTOLAR GelenDoluTablo)
        {
            try
            {
                var conn = new SQLiteConnection(System.IO.Path.Combine(documentsFolder(), "MyProfile.db"), false);
                conn.Insert(GelenDoluTablo);
                conn.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public static List<BEGENILEN_FOTOLAR> BEGENILEN_FOTOLAR_GETIR()
        {
            var conn = new SQLiteConnection(System.IO.Path.Combine(documentsFolder(), "MyProfile.db"), false);
            var gelenler = conn.Query<BEGENILEN_FOTOLAR>("Select * From BEGENILEN_FOTOLAR");
            conn.Close();
            return gelenler;
        }
 
        public static bool BEGENILEN_FOTOLAR_Guncelle(BEGENILEN_FOTOLAR Tablo)
        {
            try
            {
                var conn = new SQLiteConnection(System.IO.Path.Combine(documentsFolder(), "MyProfile.db"), false);
                conn.Update(Tablo);
                conn.Close();
                return true;
            }
            catch (Exception e)
            {
                string ee = e.ToString();
                return false;
            }

        }
        #endregion

        #region SON_KONUM_GUNCELLEMESI
        public static bool SON_KONUM_GUNCELLEMESI_EKLE(SON_KONUM_GUNCELLEMESI GelenDoluTablo)
        {
            try
            {
                var conn = new SQLiteConnection(System.IO.Path.Combine(documentsFolder(), "MyProfile.db"), false);
                conn.Insert(GelenDoluTablo);
                conn.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public static List<SON_KONUM_GUNCELLEMESI> SON_KONUM_GUNCELLEMESI_GETIR()
        {
            var conn = new SQLiteConnection(System.IO.Path.Combine(documentsFolder(), "MyProfile.db"), false);
            var gelenler = conn.Query<SON_KONUM_GUNCELLEMESI>("Select * From SON_KONUM_GUNCELLEMESI");
            conn.Close();
            return gelenler;
        }

        public static bool SON_KONUM_GUNCELLEMESI_Guncelle(SON_KONUM_GUNCELLEMESI Tablo)
        {
            try
            {
                var conn = new SQLiteConnection(System.IO.Path.Combine(documentsFolder(), "MyProfile.db"), false);
                conn.Update(Tablo);
                conn.Close();
                return true;
            }
            catch (Exception e)
            {
                string ee = e.ToString();
                return false;
            }

        }
        #endregion

        #region GIZLILIK

        public static bool GIZLILIK_EKLE(GIZLILIK GelenDoluTablo)
        {
            try
            {
                var conn = new SQLiteConnection(System.IO.Path.Combine(documentsFolder(), "MyProfile.db"), false);
                conn.Insert(GelenDoluTablo);
                conn.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public static List<GIZLILIK> GIZLILIK_GETIR()
        {
            var conn = new SQLiteConnection(System.IO.Path.Combine(documentsFolder(), "MyProfile.db"), false);
            var gelenler = conn.Query<GIZLILIK>("Select * From GIZLILIK");
            conn.Close();
            return gelenler;
        }
        public static bool GIZLILIK_TEMIZLE()
        {
            try
            {
                var conn = new SQLiteConnection(System.IO.Path.Combine(documentsFolder(), "MyProfile.db"), false);
                conn.Query<GIZLILIK>("Delete From GIZLILIK");
                conn.Close();
                return true;
            }
            catch (Exception e)
            {
                string ee = e.ToString();
                return false;
            }

        }
        public static bool GIZLILIK_Guncelle(GIZLILIK Tablo)
        {
            try
            {
                var conn = new SQLiteConnection(System.IO.Path.Combine(documentsFolder(), "MyProfile.db"), false);
                conn.Update(Tablo);
                conn.Close();
                return true;
            }
            catch (Exception e)
            {
                string ee = e.ToString();
                return false;
            }

        }
        #endregion
    }
}