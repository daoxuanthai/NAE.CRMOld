
using System.Collections.Generic;
using tcs.bo;
using tcs.dao;

namespace tcs.adapter.Sql
{
    public class StudyAbroadDb
    {
        private static StudyAbroadDb _instance;

        public static StudyAbroadDb Instance => _instance ?? (_instance = new StudyAbroadDb());

        public int Create(StudyAbroadBo obj)
        {
            return StudyAbroadSql.Instance.Create(obj);
        }

        public bool Update(StudyAbroadBo obj)
        {
            return StudyAbroadSql.Instance.Update(obj);
        }

        public bool Delete(string ids, int companyId, int userId, string userName)
        {
            return StudyAbroadSql.Instance.Delete(ids, companyId, userId, userName);
        }

        public StudyAbroadBo Read(int id)
        {
            return StudyAbroadSql.Instance.Read(id);
        }

        public List<StudyAbroadBo> Select(StudyAbroadQuery query = null)
        {
            if (query == null)
                query = new StudyAbroadQuery();

            return StudyAbroadSql.Instance.Select(query);
        }

        public List<StudyAbroadBo> GetByCustomer(int customerId)
        {
            return StudyAbroadSql.Instance.GetByCustomer(customerId);
        }
    }
}
