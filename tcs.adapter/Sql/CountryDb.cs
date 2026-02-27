using System;
using System.Collections.Generic;
using System.Linq;
using tcs.bo;
using tcs.dao;
using tcs.lib;

namespace tcs.adapter.Sql
{
    public class CountryDb
    {
        private static CountryDb _instance;

        public static CountryDb Instance => _instance ?? (_instance = new CountryDb());

        /// <summary>
        /// Lấy ra danh sách quốc gia
        /// </summary>
        /// <returns></returns>
        public List<CountryBo> GetListCountry()
        {
            var query = new CountryQuery();
            var cacheKey = Cacher.CreateCacheKey(query);
            var result = Cacher.Get<List<CountryBo>>(cacheKey);
            if (result != null)
                return result;

            result = CountrySql.Instance.Select(query);
            if (result != null)
                Cacher.Add(cacheKey, result, DateTime.Now.AddDays(ConfigMgr.DefaultCacheTimeout));
            return result;
        }

        public string GetCountryName(int id)
        {
            var all = GetListCountry();
            if (all != null)
                return all.FirstOrDefault(p => p.Id == id)?.CountryName;
            return string.Empty;
        }
    }
}
