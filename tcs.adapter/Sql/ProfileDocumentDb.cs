
using System.Collections.Generic;
using tcs.bo;
using tcs.dao;

namespace tcs.adapter.Sql
{
    public class ProfileDocumentDb
    {
        private static ProfileDocumentDb _instance;

        public static ProfileDocumentDb Instance => _instance ?? (_instance = new ProfileDocumentDb());

        public int Create(ProfileDocumentBo obj)
        {
            return ProfileDocumentSql.Instance.Create(obj);
        }

        public bool Update(ProfileDocumentBo obj)
        {
            return ProfileDocumentSql.Instance.Update(obj);
        }

        public bool Delete(string ids, int companyId, int userId, string userName)
        {
            return ProfileDocumentSql.Instance.Delete(ids, companyId, userId, userName);
        }

        public ProfileDocumentBo Read(int id)
        {
            return ProfileDocumentSql.Instance.Read(id);
        }

        public List<ProfileDocumentBo> Select(ProfileDocumentQuery query = null)
        {
            if (query == null)
                query = new ProfileDocumentQuery();

            return ProfileDocumentSql.Instance.Select(query);
        }

        public List<ProfileDocumentBo> GetByProfileType(int typeId)
        {
            return ProfileDocumentSql.Instance.GetByProfileType(typeId);
        }
    }
}
