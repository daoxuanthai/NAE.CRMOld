
using System.Collections.Generic;
using tcs.bo;
using tcs.dao;

namespace tcs.adapter.Sql
{
    public class RegisterHistoryDb
    {
        private static RegisterHistoryDb _instance;

        public static RegisterHistoryDb Instance => _instance ?? (_instance = new RegisterHistoryDb());

        public int Create(RegisterHistoryBo obj)
        {
            return RegisterHistorySql.Instance.Create(obj);
        }

        public bool Update(RegisterHistoryBo obj)
        {
            return RegisterHistorySql.Instance.Update(obj);
        }

        public bool Delete(string ids, int companyId, int userId, string userName)
        {
            return RegisterHistorySql.Instance.Delete(ids, companyId, userId, userName);
        }

        public RegisterHistoryBo Read(int id)
        {
            return RegisterHistorySql.Instance.Read(id);
        }
        public List<RegisterHistoryBo> GetList(RegisterHistoryQuery query)
        {
            if (query == null)
                query = new RegisterHistoryQuery();
            return RegisterHistorySql.Instance.Select(query);
        }
    }
}
