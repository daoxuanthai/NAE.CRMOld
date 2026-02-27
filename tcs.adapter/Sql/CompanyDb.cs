using System;
using System.Collections.Generic;
using System.Linq;
using tcs.bo;
using tcs.dao;
using tcs.lib;

namespace tcs.adapter.Sql
{
    public class CompanyDb
    {
        private static CompanyDb _instance;

        public static CompanyDb Instance => _instance ?? (_instance = new CompanyDb());

        public List<CompanyBo> Select(CompanyQuery query = null)
        {
            if (query == null)
                query = new CompanyQuery();

            return CompanySql.Instance.Select(query);
        }

        public int Create(CompanyBo obj)
        {
            return CompanySql.Instance.Create(obj);
        }

        public CompanyBo Read(int id)
        {
            return CompanySql.Instance.Read(id);
        }

        public bool Update(CompanyBo obj)
        {
            return CompanySql.Instance.Update(obj);
        }

        public bool Delete(string ids, int companyId, int userId, string userName)
        {
            return CompanySql.Instance.Delete(ids, companyId, userId, userName);
        }

        /// <summary>
        /// Lấy ra danh sách công ty theo user
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="clearCache"></param>
        /// <returns></returns>
        public List<CompanyBo> GetByUser(int userId, bool clearCache = false)
        {
            return CompanySql.Instance.GetByUser(userId);
        }

        public List<CompanyBo> GetAllCompany()
        {
            var cacheKey = Cacher.CreateCacheKey();
            var result = Cacher.Get<List<CompanyBo>>(cacheKey);
            if (result != null)
                return result;
            
            var query = new CompanyQuery() {
                From = new DateTime(2000, 1, 1),
                To = DateTime.Now.AddDays(1)
            };

            result = CompanySql.Instance.Select(query);

            if (result != null)
            {
                result = result.OrderBy(c => c.CompanyName).ToList();
                Cacher.Add(cacheKey, result, DateTime.Now.AddDays(ConfigMgr.DefaultCacheTimeout));
            }
            return result;
        }

        public string GetCompanyName(int id)
        {
            var allCompany = GetAllCompany();
            if (allCompany == null || !allCompany.Any(c => c.Id == id))
                return string.Empty;

            return allCompany.FirstOrDefault(c => c.Id == id)?.CompanyName;
        }
    }
}
