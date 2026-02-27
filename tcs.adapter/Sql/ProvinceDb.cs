using System;
using System.Collections.Generic;
using System.Linq;
using tcs.bo;
using tcs.dao;
using tcs.lib;

namespace tcs.adapter.Sql
{
    public class ProvinceDb
    {
        private static ProvinceDb _instance;

        public static ProvinceDb Instance => _instance ?? (_instance = new ProvinceDb());

        public List<ProvinceBo> Select(ProvinceQuery query = null)
        {
            var cacheKey = Cacher.CreateCacheKey(query);
            var result = Cacher.Get<List<ProvinceBo>>(cacheKey);
            if (result != null)
                return result;

            if (query == null)
                query = new ProvinceQuery();

            result = ProvinceSql.Instance.Select(query);
            if (result != null)
                Cacher.Add(cacheKey, result, DateTime.Now.AddDays(ConfigMgr.DefaultCacheTimeout));
            return result;
        }

        public int Create(ProvinceBo obj)
        {
            return ProvinceSql.Instance.Create(obj);
        }

        public ProvinceBo Read(int id)
        {
            return ProvinceSql.Instance.Read(id);
        }

        public bool Update(ProvinceBo obj)
        {
            return ProvinceSql.Instance.Update(obj);
        }

        public bool Delete(string ids, int companyId, int userId, string userName)
        {
            return ProvinceSql.Instance.Delete(ids, companyId, userId, userName);
        }

        public string GetProvinceName(int id)
        {
            var all = Select();
            if (all != null)
                return all.FirstOrDefault(p => p.Id == id)?.ProvinceName;
            return string.Empty;
        }
    }
}
