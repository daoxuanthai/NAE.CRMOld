using System;
using System.Runtime.Caching;

namespace tcs.lib
{
    public static class CacheHelper
    {
        private static ObjectCache cache = MemoryCache.Default;

        public static bool Add(string strCacheKeyName, Object objCacheItem)
        {
            CacheItemPolicy policy = new CacheItemPolicy();
            policy.Priority = CacheItemPriority.Default;
            policy.AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(ConfigMgr.DefaultCacheTimeout);
            cache.Set(strCacheKeyName, objCacheItem, policy);
            return true;
        }

        public static bool Add(string strCacheKeyName, Object objCacheItem, DateTime dayExpiration)
        {
            CacheItemPolicy policy = new CacheItemPolicy();
            policy.Priority = CacheItemPriority.Default;
            policy.AbsoluteExpiration = dayExpiration;
            cache.Set(strCacheKeyName, objCacheItem, policy);
            return true;
        }

        public static Object Get(String strCacheKeyName) 
        {
            return cache[strCacheKeyName] as Object; 
        }

        public static void Remove(String strCacheKeyName) 
        {
            if (cache.Contains(strCacheKeyName)) 
            { 
                cache.Remove(strCacheKeyName); 
            } 
        }
    }
}
