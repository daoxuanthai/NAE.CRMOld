
using System.Collections.Generic;
using tcs.bo;
using tcs.dao;

namespace tcs.adapter.Sql
{
    public class GuaranteeDb
    {
        private static GuaranteeDb _instance;

        public static GuaranteeDb Instance => _instance ?? (_instance = new GuaranteeDb());

        public int Create(GuaranteeBo obj)
        {
            return GuaranteeSql.Instance.Create(obj);
        }

        public bool Update(GuaranteeBo obj)
        {
            return GuaranteeSql.Instance.Update(obj);
        }

        public bool Delete(string ids, int companyId, int userId, string userName)
        {
            return GuaranteeSql.Instance.Delete(ids, companyId, userId, userName);
        }

        public GuaranteeBo Read(int id)
        {
            return GuaranteeSql.Instance.Read(id);
        }

        public List<GuaranteeBo> GetByCustomer(int customerId)
        {
            return GuaranteeSql.Instance.GetByCustomer(customerId);
        }
    }
}
