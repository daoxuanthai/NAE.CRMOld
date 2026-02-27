
using System.Collections.Generic;
using tcs.bo;
using tcs.dao;

namespace tcs.adapter.Sql
{
    public class LanguageDb
    {
        private static LanguageDb _instance;

        public static LanguageDb Instance => _instance ?? (_instance = new LanguageDb());

        public int Create(LanguageBo obj)
        {
            return LanguageSql.Instance.Create(obj);
        }

        public bool Update(LanguageBo obj)
        {
            return LanguageSql.Instance.Update(obj);
        }

        public bool Delete(string ids, int companyId, int userId, string userName)
        {
            return LanguageSql.Instance.Delete(ids, companyId, userId, userName);
        }

        public LanguageBo Read(int id)
        {
            return LanguageSql.Instance.Read(id);
        }

        public List<LanguageBo> Select(LanguageQuery query = null)
        {
            if (query == null)
                query = new LanguageQuery();

            return LanguageSql.Instance.Select(query);
        }

        public List<LanguageBo> GetByCustomer(int customerId)
        {
            return LanguageSql.Instance.GetByCustomer(customerId);
        }
    }
}
