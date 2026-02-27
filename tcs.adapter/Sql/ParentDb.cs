
using System.Collections.Generic;
using tcs.bo;
using tcs.dao;

namespace tcs.adapter.Sql
{
    public class ParentDb
    {
        private static ParentDb _instance;

        public static ParentDb Instance => _instance ?? (_instance = new ParentDb());

        public int Create(ParentBo obj)
        {
            return ParentSql.Instance.Create(obj);
        }

        public bool Update(ParentBo obj)
        {
            return ParentSql.Instance.Update(obj);
        }

        public bool Delete(string ids, int companyId, int userId, string userName)
        {
            return ParentSql.Instance.Delete(ids, companyId, userId, userName);
        }

        public ParentBo Read(int id)
        {
            return ParentSql.Instance.Read(id);
        }

        public List<ParentBo> Select(CustomerQuery query = null)
        {
            if (query == null)
                query = new CustomerQuery();

            return ParentSql.Instance.Select(query);
        }

        public List<ParentBo> GetByCustomer(int customerId)
        {
            return ParentSql.Instance.GetByCustomer(customerId);
        }
    }
}
