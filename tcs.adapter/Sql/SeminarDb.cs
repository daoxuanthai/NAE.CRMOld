
using System.Collections.Generic;
using tcs.bo;
using tcs.dao;

namespace tcs.adapter.Sql
{
    public class SeminarDb
    {
        private static SeminarDb _instance;

        public static SeminarDb Instance => _instance ?? (_instance = new SeminarDb());

        public int Create(SeminarBo obj)
        {
            return SeminarSql.Instance.Create(obj);
        }

        public bool Update(SeminarBo obj)
        {
            return SeminarSql.Instance.Update(obj);
        }

        public bool Delete(string ids, int companyId, int userId, string userName)
        {
            return SeminarSql.Instance.Delete(ids, companyId, userId, userName);
        }

        public SeminarBo Read(int id)
        {
            return SeminarSql.Instance.Read(id);
        }

        public List<SeminarBo> Search(SeminarQuery query = null)
        {
            if (query == null)
                query = new SeminarQuery();

            return SeminarSql.Instance.Select(query);
        }

        public List<SeminarBo> GetByCompany(int companyId)
        {
            return SeminarSql.Instance.GetByCompany(companyId);
        }
    }
}
