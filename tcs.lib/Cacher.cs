using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;

namespace tcs.lib
{
    public static class Cacher
    {
        public const string CacheStore = "VV_CACHE_KEYS";
        public const string CacheDicKey = "VV_CACHE_DICTIONARY";

        public const string CacheActionAll = "@all";
        public const string CacheActionByPass = "@bypass";
        public const string ByPassUtmSource = "by_pass_utm_source";

        public static string ClearCacheAction
            => HttpContext.Current != null ? HttpContext.Current.Request.QueryString["clearcache"] : "";

        private static ConcurrentDictionary<string, DateTime> _cacheDic;
        private static ConcurrentDictionary<string, DateTime> _htmlCssCacheDic;

        public static void InitCache()
        {
            _cacheDic = new ConcurrentDictionary<string, DateTime>();
            _htmlCssCacheDic = new ConcurrentDictionary<string, DateTime>();
        }

        [Serializable]
        public enum ClearCacheActions
        {
            /// <summary>
            /// Không xác định
            /// </summary>
            Undefined,

            /// <summary>
            /// Xóa cache
            /// </summary>
            [Description("1")]
            Normal = 1,


            /// <summary>
            /// Xóa tất cả cache CSS
            /// </summary>
            [Description("@css")]
            Css = 2,

            /// <summary>
            /// Xóa các cache HTML
            /// </summary>
            [Description("@html")]
            Html = 3,

            /// <summary>
            /// Xóa các cache static version
            /// </summary>
            [Description("@static")]
            StaticContent = 4,

            /// <summary>
            /// Xóa tất cả cache
            /// </summary>
            [Description("@all")]
            All = 999,

            /// <summary>
            /// Bỏ qua việc lấy cache (không xóa cache)
            /// </summary>
            [Description("@bypass")]
            ByPass = 9999
        }

        /// <summary>
        /// Nhóm của cache key
        /// </summary>
        [Serializable]
        public enum CacheGroup
        {
            General,
            StaticVersion,
            Js
        }

        /// <summary>
        /// Thông tin liên quan của một cache
        /// </summary>
        [Serializable]
        public class CacheInfo
        {
            public string CacheKey { get; set; }
            public object DataId { get; set; }
            public CacheGroup CacheGroup { get; set; }
            public DateTime CreatedTime { get; set; }

            public CacheInfo()
            {

            }

            public CacheInfo(CacheGroup cacheGroup)
            {
                CacheGroup = cacheGroup;
            }

            public CacheInfo(CacheGroup cacheGroup, object dataId)
            {
                CacheGroup = cacheGroup;
                DataId = dataId;
            }

            public override string ToString()
            {
                return CacheGroup + "_" + DataId;
            }
        }

        /// <summary>
        /// Tạo cache key với prefix và tham số
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static string CreateCacheKeyWithPrefix(string prefix, params object[] parameters)
        {
            prefix = prefix.Replace("Async", "");

            string utmSourcePrefix = string.Empty;

            if (parameters == null || parameters.Length == 0)
                return prefix;

            if (!prefix.EndsWith("__"))
                prefix += "__";

            bool byPassUtmSource = false;
            var paramString = string.Empty;
            foreach (var param in parameters)
            {
                if (param == null)
                {
                    paramString += "null,";
                }
                else
                {
                    var paramType = param.GetType();
                    if (paramType.IsValueType || paramType == typeof(string))
                    {
                        if (paramType == typeof(DateTime))
                        {
                            paramString += ((DateTime)param).ToString("yyyyMMddHHmmss");
                        }
                        else if (paramType == typeof(string))
                        {
                            if ((string)param == "by_pass_utm_source")
                            {
                                byPassUtmSource = true;
                            }
                            else
                            {
                                paramString += ((string)param).ToUrl();
                            }
                        }
                        else
                        {
                            paramString += param;
                        }
                    }
                    else
                    {
                        paramString += param.GetMD5Hash();
                    }

                    paramString += ",";
                }
            }

            return (byPassUtmSource && !string.IsNullOrEmpty(utmSourcePrefix)
                       ? prefix.Replace(utmSourcePrefix, "")
                       : prefix) + (paramString.Length > 150 ? paramString.GetMD5Hash() : paramString.Trim(','));
        }

        /// <summary>
        /// Tạo cache key với tham số. Prefix sẽ được tự động tạo ra với dạng namespace.class.method của hàm đang được gọi
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static string CreateCacheKey(params object[] parameters)
        {
            var stackTrace = new StackTrace();
            MethodBase method = stackTrace.GetFrame(1).GetMethod();
            var callingMethod = method.Name;
            if (method.ReflectedType != null)
            {
                if (callingMethod == "MoveNext")
                {
                    callingMethod = method.ReflectedType.FullName ?? string.Empty;
                    MatchCollection matchCollection = Regex.Matches(callingMethod, @"\+<[^>]*>");
                    if (matchCollection.Count > 0)
                    {
                        var methodName = matchCollection[0].ToString();
                        var i = callingMethod.IndexOf("+", StringComparison.Ordinal);

                        callingMethod = callingMethod.Substring(0, i) + "." + methodName.Substring(2, methodName.Length - 3);
                    }
                }
                else
                {
                    callingMethod = method.ReflectedType.FullName + "." + callingMethod;
                }
            }

            return CreateCacheKeyWithPrefix(ConfigMgr.StaticVersionFeed, callingMethod, parameters);
        }

        /// <summary>
        /// Add một object vào cache với thời điểm xác định
        /// </summary>
        /// <param name="key">Cache key</param>
        /// <param name="value">Object cần cache</param>
        /// <param name="absoluteExpiration">Thời điểm hết hạn cache</param>
        /// <param name="cacheInfo">Nhóm của cache key tương ứng</param>
        /// <returns></returns>
        public static bool Add(string key, object value, DateTime absoluteExpiration, CacheInfo cacheInfo = null)
        {
            if (value == null)
                return false;

            var result = CacheHelper.Add(key, value, absoluteExpiration);

            if (!result)
                return false;

            if (cacheInfo == null)
            {
                _cacheDic?.AddOrUpdate(key, DateTime.Now, (k, v) => DateTime.Now);
            }
            else
            {
                switch (cacheInfo.CacheGroup)
                {
                    case CacheGroup.Js:
                        _htmlCssCacheDic?.AddOrUpdate(key, DateTime.Now, (k, v) => DateTime.Now);
                        break;
                    default:
                        _cacheDic?.AddOrUpdate(key, DateTime.Now, (k, v) => DateTime.Now);
                        break;
                }
            }

            return true;
        }

        /// <summary>
        /// Add một object vào cache
        /// </summary>
        /// <param name="key">Cache key</param>
        /// <param name="value">Object cần cache</param>
        /// <param name="cacheInfo">Nhóm của cache key tương ứng</param>
        /// <returns></returns>
        public static bool Add(string key, object value, CacheInfo cacheInfo = null)
        {
            return Add(key, value, DateTime.Now.AddMinutes(ConfigMgr.DefaultCacheTimeout), cacheInfo);
        }

        /// <summary>
        /// Remove a caching value by special key
        /// </summary>
        /// <param name="key"></param>
        public static bool Remove(string key)
        {
            DateTime d;
            if (_htmlCssCacheDic != null)
                _htmlCssCacheDic.TryRemove(key, out d);
            if (_cacheDic != null)
                _cacheDic.TryRemove(key, out d);
            var prefixKey = ConfigMgr.Get<string>("CacheKeyPrefix") + key;
            if (HttpContext.Current != null)
            {
                HttpContext.Current.Cache.Remove(key);
                HttpContext.Current.Cache.Remove(prefixKey);
            }
            HttpRuntime.Cache.Remove(key);
            HttpRuntime.Cache.Remove(prefixKey);
            CacheHelper.Remove(key);
            CacheHelper.Remove(prefixKey);
            return true;
        }

        public static bool Remove(string[] keys)
        {
            var result = false;
            foreach (var key in keys)
            {
                result = Remove(key);
            }
            return result;
        }

        /// <summary>
        /// Lấy dữ liệu từ cache. 
        /// Nếu request hiện tại có yêu cầu xóa cache, thì gọi xóa cache và trả về giá trị mặc định của kiểu dữ liệu đó (object: null, number: 0...)
        /// </summary>
        /// <typeparam name="T">Kiểu của dữ liệu được cache</typeparam>
        /// <param name="key">Key của cache</param>
        /// <param name="isRootKey">TRUE: Nghĩa là key thuộc loại RootKey, loại này chỉ bị xóa khi yêu cầu clear cache ALL</param>
        /// <param name="neverClear">TRUE: Nghĩa là không bao giờ clear cache này</param>
        /// <returns></returns>
        public static T Get<T>(string key, bool isRootKey = false, bool neverClear = false)
        {
            if (HttpContext.Current == null)
                return default(T);

            var clearCacheAction = Extension.GetValueFromDescription<ClearCacheActions>(ClearCacheAction);

            // Xóa static version
            if (clearCacheAction == ClearCacheActions.StaticContent)
            {
                if (key.Contains("GetStaticVersion") ||
                    key.Contains("GetStaticMinVersion") ||
                    key.Contains("RenderCss"))
                {
                    if (Remove(key))
                        return default(T);
                }
            }

            if (neverClear || clearCacheAction == ClearCacheActions.Undefined)
            {
                goto GetCache;
            }

            if (clearCacheAction == ClearCacheActions.ByPass)
                return default(T);

            if (clearCacheAction == ClearCacheActions.Css ||
                clearCacheAction == ClearCacheActions.Html)
            {

                if (_htmlCssCacheDic.ContainsKey(key))
                    if (Remove(key))
                        return default(T);
                goto GetCache;
            }

            if (clearCacheAction < ClearCacheActions.All && !isRootKey ||
                clearCacheAction == ClearCacheActions.All)
            {
                if (Remove(key))
                    return default(T);
            }

            GetCache:
            var obj = CacheHelper.Get(key);
            if (obj is T)
            {
                Type t = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);
                return (T)Convert.ChangeType(obj, t);
            }
            return default(T);

        }
    }
}
