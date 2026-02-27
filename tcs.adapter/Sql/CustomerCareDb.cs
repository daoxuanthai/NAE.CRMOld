
using System.Collections.Generic;
using tcs.bo;
using tcs.dao;

namespace tcs.adapter.Sql
{
    public class CustomerCareDb
    {
        private static CustomerCareDb _instance;

        public static CustomerCareDb Instance => _instance ?? (_instance = new CustomerCareDb());

        public int Create(CustomerCareBo obj)
        {
            return CustomerCareSql.Instance.Create(obj);
        }

        public bool Update(CustomerCareBo obj)
        {
            return CustomerCareSql.Instance.Update(obj);
        }

        public bool Delete(string ids, int companyId, int userId, string userName)
        {
            return CustomerCareSql.Instance.Delete(ids, companyId, userId, userName);
        }

        public CustomerCareBo Read(int id)
        {
            return CustomerCareSql.Instance.Read(id);
        }

        public List<CustomerCareBo> Select(CustomerQuery query = null)
        {
            if (query == null)
                query = new CustomerQuery();

            return CustomerCareSql.Instance.Select(query);
        }

        public List<CustomerCareBo> GetByCustomer(int customerId)
        {
            return CustomerCareSql.Instance.GetByCustomer(customerId);
        }
    }
}
