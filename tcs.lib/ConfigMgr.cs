
using System;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;

namespace tcs.lib
{
    public static class ConfigMgr
    {
        public const int DashboardDayLeft = 7;

        #region Connection

        public static string ConnectionString
        {
            get
            {
                if (ConfigurationManager.ConnectionStrings["SqlConnectionString"] != null)
                    return ConfigurationManager.ConnectionStrings["SqlConnectionString"].ConnectionString;
                return string.Empty;
            }
        }

        public static string AccountConnectionString
        {
            get
            {
                if (ConfigurationManager.ConnectionStrings["AccountConnectionString"] != null)
                    return ConfigurationManager.ConnectionStrings["AccountConnectionString"].ConnectionString;
                return string.Empty;
            }
        }

        #endregion
        
        /// <summary>
        /// Nhận biết môi trường thật hay local (LIVE = config Env="live")
        /// </summary>
        public static bool IsLiveEnv => Get<string>("Env") == "live";
        public static string Cdn => Get<string>("Cdn");
        public static string ContentCdn => Get<string>("ContentCdn");
        public static string ScriptCdnPath => Get<string>("ScriptCdnPath");
        public static string StaticVersionFeed => Get<string>("StaticVersionFeed");
        /// <summary>
        /// Tìm kiếm trên elastic sau đó mới search dưới DB
        /// </summary>
        public static bool IsElastic => Get<bool>("IsElastic");

        public static int DefaultCacheTimeout => Get<int>("DefaultCacheTimeout");
        public static int DefaultPageSize => Get<int>("DefaultPageSize");

        public static string UploadFolder => Get<string>("UploadFolder");
        public static string ValidFileUpload => Get<string>("ValidFileUpload");

        public static DateTime DefaultDate => new DateTime(2000, 1, 1);

        public static DateTime DefaultAlarmDate => new DateTime(2299, 1, 1);

        public static string DefaultDateFormat = "dd/MM/yyyy";

        public static string DefaultDateTimeFormat = "dd/MM/yyyy HH:mm";

        #region Discord bot token

        public static string DiscordWebhooks = "1469370291263115470/4EPt28QnAvji_5YgFO_naK-vTWf99nbCQa-C_EOqp_vlhrKrcGKOrV45pSR5dwY8PsTY";

        #endregion

        /// <summary>
        /// Id mặc định khi thêm dữ liệu bằng tài khoản inside của TCS mà không phải là tài khoản công ty
        /// </summary>
        public static int DefaultCompany = -99;

        public static string GetStaticVersion(string filePath = "")
        {
            if (!Cdn.Contains("cdn"))
                return string.Empty;

            var cacheKey = Cacher.CreateCacheKey(StaticVersionFeed, filePath);
            string version = Cacher.Get<string>(cacheKey, neverClear: true);
            if (version != null)
                return version;

            version = ".v" + DateTime.Now.ToString("yyyyMMddHHmm");
            if (string.IsNullOrEmpty(filePath))
                goto Cache;

            try
            {
                if (filePath.StartsWith("/"))
                    filePath = "~" + filePath;
                filePath = HttpContext.Current.Server.MapPath(filePath);
                if (!File.Exists(filePath))
                    goto Cache;

                var fileInfo = new FileInfo(filePath);
                version = ".v" + fileInfo.LastWriteTime.ToString("yyyyMMddHHmm");
            }
            catch (Exception)
            {
#if DEBUG
                throw;
#endif
            }

            Cache:
            Cacher.Add(cacheKey, version, DateTime.Now.AddDays(1),
                new Cacher.CacheInfo(Cacher.CacheGroup.StaticVersion));
            return version;
        }

        public static CultureInfo DefaultCultureInfo
        {
            get
            {
                var defaultCulture = Get<string>("DefaultCultureInfo");
                if (string.IsNullOrEmpty(defaultCulture))
                    defaultCulture = "vi-VN";
                var cultureInfo = (CultureInfo)CultureInfo.GetCultureInfo(defaultCulture).Clone();
                cultureInfo.NumberFormat.CurrencyDecimalDigits = 0;
                cultureInfo.NumberFormat.CurrencyPositivePattern = 1;
                return cultureInfo;
            }
        }

        public static T Get<T>(string key, char separator = ',')
        {
            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(ConfigurationManager.AppSettings[key]))
            {
                if (typeof(T) == typeof(int[]))
                    return (T)Convert.ChangeType(new int[0], typeof(T));
                if (typeof(T) == typeof(string[]))
                    return (T)Convert.ChangeType(new string[0], typeof(T));
                if (typeof(T) == typeof(string))
                    return (T)Convert.ChangeType(string.Empty, typeof(T));
                return default(T);
            }

            object result;

            if (typeof(T) == typeof(int[]))
            {
                result = ConfigurationManager.AppSettings[key].Split(new[] { separator }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(t => Convert.ToInt32(t)).ToArray();
            }
            else if (typeof(T) == typeof(string[]))
            {
                result = ConfigurationManager.AppSettings[key].Split(new[] { separator }, StringSplitOptions.RemoveEmptyEntries);
            }
            else if (typeof(T) == typeof(DateTime))
            {
                var val = ConfigurationManager.AppSettings[key];
                if (val.Length <= 10)
                    val += " 00:00:00";
                if (val.Length <= 16)
                    val += ":00";
                result = DateTime.ParseExact(val, "dd/MM/yyyy HH:mm:ss", DefaultCultureInfo);
            }
            else
            {
                result = ConfigurationManager.AppSettings[key];
            }

            return (T)Convert.ChangeType(result, typeof(T));
        }
    }
}
