
using System.Collections.Generic;
using tcs.bo;
using tcs.dao;

namespace tcs.adapter.Sql
{
    public class ScholarshipDb
    {
        private static ScholarshipDb _instance;

        public static ScholarshipDb Instance => _instance ?? (_instance = new ScholarshipDb());

        public int Create(ScholarshipBo obj)
        {
            return ScholarshipSql.Instance.Create(obj);
        }

        public bool Update(ScholarshipBo obj)
        {
            return ScholarshipSql.Instance.Update(obj);
        }

        public bool Delete(string ids, int companyId, int userId, string userName)
        {
            return ScholarshipSql.Instance.Delete(ids, companyId, userId, userName);
        }

        public ScholarshipBo Read(int id)
        {
            return ScholarshipSql.Instance.Read(id);
        }

        public List<ScholarshipBo> Select(ScholarshipQuery query = null)
        {
            if (query == null)
                query = new ScholarshipQuery();

            return ScholarshipSql.Instance.Select(query);
        }

        public bool UpdateRegister(int id, int updateUserId, string updateUserName)
        {
            return ScholarshipSql.Instance.UpdateRegister(id, updateUserId, updateUserName);
        }

        public List<ScholarshipBo> GetBySchool(int id)
        {
            return ScholarshipSql.Instance.GetBySchool(id);
        }
    }
}
