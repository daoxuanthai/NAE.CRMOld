
using System.Collections.Generic;
using tcs.bo;
using tcs.dao;

namespace tcs.adapter.Sql
{
    public class ContractDocumentDb
    {
        private static ContractDocumentDb _instance;

        public static ContractDocumentDb Instance => _instance ?? (_instance = new ContractDocumentDb());

        public int Create(ContractDocumentBo obj)
        {
            return ContractDocumentSql.Instance.Create(obj);
        }

        public bool Update(ContractDocumentBo obj)
        {
            return ContractDocumentSql.Instance.Update(obj);
        }

        public bool Delete(string ids, int companyId, int userId, string userName)
        {
            return ContractDocumentSql.Instance.Delete(ids, companyId, userId, userName);
        }

        public ContractDocumentBo Read(int id)
        {
            return ContractDocumentSql.Instance.Read(id);
        }

        public List<ContractDocumentBo> Select(ProfileDocumentQuery query = null)
        {
            if (query == null)
                query = new ProfileDocumentQuery();

            return ContractDocumentSql.Instance.Select(query);
        }

        public List<ContractDocumentBo> GetByContractId(int id, int profileTypeId)
        {
            return ContractDocumentSql.Instance.GetByContractId(id, profileTypeId);
        }

        public bool InitDocument(int id, int profileTypeId)
        {
            return ContractDocumentSql.Instance.InitDocument(id, profileTypeId);
        }
    }
}
