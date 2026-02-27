using System;
using System.Collections.Generic;
using System.Linq;
using tcs.bo;
using tcs.dao;
using tcs.lib;

namespace tcs.adapter.Sql
{
    public class UserDb
    {
        private static UserDb _instance;

        public static UserDb Instance => _instance ?? (_instance = new UserDb());

        public List<UserBo> Select(UserQuery query = null, bool useCache = true)
        {
            //var cacheKey = Cacher.CreateCacheKey(query);
            //var result = Cacher.Get<List<UserBo>>(cacheKey);
            //if (result != null && useCache)
            //    return result;

            if (query == null)
                query = new UserQuery();

            var result = UserSql.Instance.Select(query);
            //if (result != null)
            //    Cacher.Add(cacheKey, result, DateTime.Now.AddDays(ConfigMgr.DefaultCacheTimeout));
            return result;
        }

        public int Create(UserBo obj)
        {
            return UserSql.Instance.Create(obj);
        }

        public UserBo Read(int id)
        {
            return UserSql.Instance.Read(id);
        }

        public bool Update(UserBo obj)
        {
            return UserSql.Instance.Update(obj);
        }

        public bool Delete(string ids, int userId, string userName)
        {
            return UserSql.Instance.Delete(ids, 0, userId, userName);
        }

        public string GetUserName(int id)
        {
            var all = Select(new UserQuery());
            if (all != null)
                return all.FirstOrDefault(p => p.Id == id)?.UserName;
            return string.Empty;
        }

        public UserBo GetByUserName(string userName)
        {
            return UserSql.Instance.GetByUserName(userName);
        }
    }
}
