
using System.Collections.Generic;
using tcs.bo;
using tcs.dao;

namespace tcs.adapter.Sql
{
    public class SeminarPlaceDb
    {
        private static SeminarPlaceDb _instance;

        public static SeminarPlaceDb Instance => _instance ?? (_instance = new SeminarPlaceDb());

        public int Create(SeminarPlaceBo obj)
        {
            return SeminarPlaceSql.Instance.Create(obj);
        }

        public bool Update(SeminarPlaceBo obj)
        {
            return SeminarPlaceSql.Instance.Update(obj);
        }

        public bool Delete(string ids, int companyId, int userId, string userName)
        {
            return SeminarPlaceSql.Instance.Delete(ids, companyId, userId, userName);
        }

        public SeminarPlaceBo Read(int id)
        {
            return SeminarPlaceSql.Instance.Read(id);
        }

        public List<SeminarPlaceBo> GetBySeminar(int id)
        {
            return SeminarPlaceSql.Instance.GetBySeminar(id);
        }
    }
}
