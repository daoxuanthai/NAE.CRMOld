using System;
using System.Collections.Generic;
using System.Linq;
using tcs.bo;
using tcs.dao;
using tcs.lib;

namespace tcs.adapter.Sql
{
    public class CompanyTitleDb
    {
        private static CompanyTitleDb _instance;

        public static CompanyTitleDb Instance => _instance ?? (_instance = new CompanyTitleDb());

        public List<CompanyTitleBo> Select(CompanyTitleQuery query = null)
        {
            if (query == null)
                query = new CompanyTitleQuery();

            return CompanyTitleSql.Instance.Select(query);
        }

        public int Create(CompanyTitleBo obj)
        {
            return CompanyTitleSql.Instance.Create(obj);
        }

        public CompanyTitleBo Read(int id, bool clearCache = false)
        {
            return CompanyTitleSql.Instance.Read(id);
        }

        public bool Update(CompanyTitleBo obj)
        {
            return CompanyTitleSql.Instance.Update(obj);
        }

        public bool Delete(string ids, int companyId, int userId, string userName)
        {
            return CompanyTitleSql.Instance.Delete(ids, companyId, userId, userName);
        }

        /// <summary>
        /// Lấy ra danh sách vị trí theo công ty
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        public List<CompanyTitleBo> GetByCompany(int companyId)
        {
            var cacheKey = Cacher.CreateCacheKey(companyId);
            //var result = Cacher.Get<List<CompanyTitleBo>>(cacheKey);
            //if (result != null)
            //    return result;

            var result = CompanyTitleSql.Instance.GetByCompany(companyId);
            if (result != null)
                Cacher.Add(cacheKey, result, DateTime.Now.AddMinutes(ConfigMgr.DefaultCacheTimeout));
            return result;
        }

        /// <summary>
        /// Lấy ra danh sách vị trí theo user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<CompanyTitleBo> GetByUser(int userId)
        {
            return CompanyTitleSql.Instance.GetByUser(userId);
        }

        /// <summary>
        /// Lấy danh sách tất cả tài khoản đại lý
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="officeId"></param>
        /// <returns></returns>
        public List<CompanyTitleBo> GetListAgency(int companyId, int officeId = 0)
        {
            var cacheKey = Cacher.CreateCacheKey(companyId, officeId);

            var result = CompanyTitleSql.Instance.GetByCompany(companyId);
            if (result != null && result.Any(a => a.UserType == CompanyTitleType.Agency.Key))
            {
                result = result.Where(a => a.UserType == CompanyTitleType.Agency.Key).ToList();
                if(officeId > 0)
                {
                    result = result.Where(a => a.OfficeId == officeId).ToList();
                }
                Cacher.Add(cacheKey, result, DateTime.Now.AddMinutes(ConfigMgr.DefaultCacheTimeout));
                return result;
            }
                
            return null;
        }
    }
}
