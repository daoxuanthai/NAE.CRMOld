
using System;
using System.Collections.Generic;
using System.Linq;
using tcs.adapter.Elastics;
using tcs.bo;
using tcs.dao;
using tcs.lib;

namespace tcs.adapter.Sql
{
    public class CommissionDb
    {
        private static CommissionDb _instance;

        public static CommissionDb Instance => _instance ?? (_instance = new CommissionDb());

        public int Create(CommissionBo obj)
        {
            var id = CommissionSql.Instance.Create(obj);
            return id;
        }

        public bool Update(CommissionBo obj)
        {
            var ret = CommissionSql.Instance.Update(obj);
            return ret;
        }

        public bool Delete(string ids, int companyId, int userId, string userName)
        {
            var ret = CommissionSql.Instance.Delete(ids, companyId, userId, userName);
            return ret;
        }

        public CommissionBo Read(int id)
        {
            return CommissionSql.Instance.Read(id);
        }

        public List<CommissionBo> Select(CommissionQuery query = null)
        {
            if (query == null)
                query = new CommissionQuery();

            return CommissionSql.Instance.Select(query);
        }

        public List<CommissionBo> GetByContract(int contractId)
        {
            return CommissionSql.Instance.GetByContract(contractId);
        }

        public List<CommissionBo> Search(CommissionQuery query = null)
        {
            if (query == null)
                query = new CommissionQuery();

            var cacheKey = Cacher.CreateCacheKey(query);
            var cacheTotal = cacheKey + "_total";
            var result = Cacher.Get<List<CommissionBo>>(cacheKey);
            if (result != null)
            {
                query.TotalRecord = Cacher.Get<int>(cacheTotal);
                return result;
            }

            var lstCommission = CommissionSql.Instance.Select(query);
            if (lstCommission != null && lstCommission.Any())
            {
                Cacher.Add(cacheTotal, (int)query.TotalRecord);
                Cacher.Add(cacheKey, lstCommission);
                return lstCommission;
            }
            return null;
        }
    }
}
