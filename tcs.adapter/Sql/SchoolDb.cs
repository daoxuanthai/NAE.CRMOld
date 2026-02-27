
using System.Collections.Generic;
using tcs.bo;
using tcs.dao;

namespace tcs.adapter.Sql
{
    public class SchoolDb
    {
        private static SchoolDb _instance;

        public static SchoolDb Instance => _instance ?? (_instance = new SchoolDb());

        public int Create(SchoolBo obj)
        {
            return SchoolSql.Instance.Create(obj);
        }

        public bool Update(SchoolBo obj)
        {
            return SchoolSql.Instance.Update(obj);
        }

        public bool Delete(string ids, int companyId, int userId, string userName)
        {
            return SchoolSql.Instance.Delete(ids, companyId, userId, userName);
        }

        public SchoolBo Read(int id)
        {
            return SchoolSql.Instance.Read(id);
        }

        public List<SchoolBo> Select(SchoolQuery query = null)
        {
            if (query == null)
                query = new SchoolQuery();

            return SchoolSql.Instance.Select(query);
        }

        public List<SchoolBo> GetAll(int companyId)
        {
            return SchoolSql.Instance.GetAll(companyId);
        }
    }
}
