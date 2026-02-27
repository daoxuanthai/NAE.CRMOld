
using System.Collections.Generic;
using tcs.bo;
using tcs.dao;

namespace tcs.adapter.Sql
{
    public class StudyHistoryDb
    {
        private static StudyHistoryDb _instance;

        public static StudyHistoryDb Instance => _instance ?? (_instance = new StudyHistoryDb());

        public int Create(StudyHistoryBo obj)
        {
            return StudyHistorySql.Instance.Create(obj);
        }

        public bool Update(StudyHistoryBo obj)
        {
            return StudyHistorySql.Instance.Update(obj);
        }

        public bool Delete(string ids, int companyId, int userId, string userName)
        {
            return StudyHistorySql.Instance.Delete(ids, companyId, userId, userName);
        }

        public StudyHistoryBo Read(int id)
        {
            return StudyHistorySql.Instance.Read(id);
        }

        public List<StudyHistoryBo> Select(StudyHistoryQuery query = null)
        {
            if (query == null)
                query = new StudyHistoryQuery();

            return StudyHistorySql.Instance.Select(query);
        }

        public List<StudyHistoryBo> GetByCustomer(int customerId)
        {
            return StudyHistorySql.Instance.GetByCustomer(customerId);
        }
    }
}
