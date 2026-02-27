
using System.Collections.Generic;
using tcs.bo;
using tcs.dao;

namespace tcs.adapter.Sql
{
    public class ProfileTypeDb
    {
        private static ProfileTypeDb _instance;

        public static ProfileTypeDb Instance => _instance ?? (_instance = new ProfileTypeDb());

        public int Create(ProfileTypeBo obj)
        {
            return ProfileTypeSql.Instance.Create(obj);
        }

        public bool Update(ProfileTypeBo obj)
        {
            return ProfileTypeSql.Instance.Update(obj);
        }

        public bool Delete(string ids, int companyId, int userId, string userName)
        {
            return ProfileTypeSql.Instance.Delete(ids, companyId, userId, userName);
        }

        public ProfileTypeBo Read(int id)
        {
            return ProfileTypeSql.Instance.Read(id);
        }

        public List<ProfileTypeBo> Select(ProfileTypeQuery query = null)
        {
            if (query == null)
                query = new ProfileTypeQuery();

            return ProfileTypeSql.Instance.Select(query);
        }

        public List<ProfileTypeBo> GetByCompany(int companyId)
        {
            var query = new ProfileTypeQuery();
            query.Company = companyId.ToString();
            query.Country = "-1";

            return ProfileTypeSql.Instance.Select(query);
        }
    }
}
