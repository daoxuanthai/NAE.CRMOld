
using System.Collections.Generic;
using tcs.bo;
using tcs.dao;

namespace tcs.adapter.Sql
{
    public class RelativesDb
    {
        private static RelativesDb _instance;

        public static RelativesDb Instance => _instance ?? (_instance = new RelativesDb());

        public int Create(RelativesBo obj)
        {
            return RelativesSql.Instance.Create(obj);
        }

        public bool Update(RelativesBo obj)
        {
            return RelativesSql.Instance.Update(obj);
        }

        public bool Delete(string ids, int companyId, int userId, string userName)
        {
            return RelativesSql.Instance.Delete(ids, companyId, userId, userName);
        }

        public RelativesBo Read(int id)
        {
            return RelativesSql.Instance.Read(id);
        }

        public List<RelativesBo> GetByCustomer(int customerId)
        {
            return RelativesSql.Instance.GetByCustomer(customerId);
        }
    }
}
