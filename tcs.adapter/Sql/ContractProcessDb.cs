using System.Collections.Generic;
using tcs.bo;
using tcs.dao;

namespace tcs.adapter.Sql
{
    public class ContractProcessDb
    {
        private static ContractProcessDb _instance;

        public static ContractProcessDb Instance => _instance ?? (_instance = new ContractProcessDb());

        public int Create(ContractProcessBo obj)
        {
            var id = ContractProcessSql.Instance.Create(obj);
            return id;
        }

        public List<ContractProcessBo> GetByContract(int contractid)
        {
            return ContractProcessSql.Instance.GetByContract(contractid);
        }
    }
}
