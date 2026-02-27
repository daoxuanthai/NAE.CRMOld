
using System.Collections.Generic;
using tcs.bo;
using tcs.dao;

namespace tcs.adapter.Sql
{
    public class ProfileStepDb
    {
        private static ProfileStepDb _instance;

        public static ProfileStepDb Instance => _instance ?? (_instance = new ProfileStepDb());

        public int Create(ProfileStepBo obj)
        {
            return ProfileStepSql.Instance.Create(obj);
        }

        public bool Update(ProfileStepBo obj)
        {
            return ProfileStepSql.Instance.Update(obj);
        }

        public bool Delete(string ids, int companyId, int userId, string userName)
        {
            return ProfileStepSql.Instance.Delete(ids, companyId, userId, userName);
        }

        public ProfileStepBo Read(int id)
        {
            return ProfileStepSql.Instance.Read(id);
        }

        public List<ProfileStepBo> Select(ProfileStepQuery query = null)
        {
            if (query == null)
                query = new ProfileStepQuery();

            return ProfileStepSql.Instance.Select(query);
        }
        public List<ProfileStepBo> GetByProfileType(int typeId)
        {
            return ProfileStepSql.Instance.GetByProfileType(typeId);
        }
    }
}
