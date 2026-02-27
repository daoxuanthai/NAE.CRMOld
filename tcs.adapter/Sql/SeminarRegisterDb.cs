
using System.Collections.Generic;
using tcs.bo;
using tcs.dao;

namespace tcs.adapter.Sql
{
    public class SeminarRegisterDb
    {
        private static SeminarRegisterDb _instance;

        public static SeminarRegisterDb Instance => _instance ?? (_instance = new SeminarRegisterDb());

        public int Create(SeminarRegisterBo obj)
        {
            return SeminarRegisterSql.Instance.Create(obj);
        }

        public bool Update(SeminarRegisterBo obj)
        {
            return SeminarRegisterSql.Instance.Update(obj);
        }

        public bool Delete(string ids, int companyId, int userId, string userName)
        {
            return SeminarRegisterSql.Instance.Delete(ids, companyId, userId, userName);
        }

        public SeminarRegisterBo Read(int id)
        {
            return SeminarRegisterSql.Instance.Read(id);
        }

        public List<SeminarRegisterBo> Search(SeminarRegisterQuery query = null)
        {
            if (query == null)
                query = new SeminarRegisterQuery();

            return SeminarRegisterSql.Instance.Select(query);
        }

        public List<SeminarRegisterBo> GetByCustomer(int customerId, int seminarId)
        {
            return SeminarRegisterSql.Instance.GetByCustomer(customerId, seminarId);
        }
    }
}
