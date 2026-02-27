using System.Collections.Generic;
using tcs.bo;
using tcs.dao;

namespace tcs.adapter.Sql
{
    public class CompanyOfficeDb
    {
        private static CompanyOfficeDb _instance;

        public static CompanyOfficeDb Instance => _instance ?? (_instance = new CompanyOfficeDb());

        public List<CompanyOfficeBo> GetAll(int companyId)
        {
            var query = new CompanyOfficeQuery()
            {
                Company = companyId.ToString(),
                ProvinceId = -1,
                Keyword = string.Empty,
                //From = model.From,
                //To = model.To,
                Page = 0,
                PageSize = 100
            };
            return Select(query);
        }

        public List<CompanyOfficeBo> Select(CompanyOfficeQuery query)
        {
            if (query == null)
                query = new CompanyOfficeQuery();

            return CompanyOfficeSql.Instance.Select(query);
        }

        public int Create(CompanyOfficeBo obj)
        {
            return CompanyOfficeSql.Instance.Create(obj);
        }

        public CompanyOfficeBo Read(int id, bool clearCache = false)
        {
            return CompanyOfficeSql.Instance.Read(id);
        }

        public bool Update(CompanyOfficeBo obj)
        {
            return CompanyOfficeSql.Instance.Update(obj);
        }

        public bool Delete(string ids, int companyId, int userId, string userName)
        {
            return CompanyOfficeSql.Instance.Delete(ids, companyId, userId, userName);
        }
        public List<CompanyOfficeBo> GetByCompany(int companyId, bool clearCache = false)
        {
            return CompanyOfficeSql.Instance.GetByCompany(companyId);
        }
    }
}
