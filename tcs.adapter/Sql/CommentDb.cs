
using System.Collections.Generic;
using tcs.bo;
using tcs.dao;

namespace tcs.adapter.Sql
{
    public class CommentDb
    {
        private static CommentDb _instance;

        public static CommentDb Instance => _instance ?? (_instance = new CommentDb());

        public int Create(CommentBo obj)
        {
            return CommentSql.Instance.Create(obj);
        }

        public bool Delete(string id)
        {
            return CommentSql.Instance.Delete(id, 0, 0, "");
        }

        public List<CommentBo> GetComment(int customerId)
        {
            return CommentSql.Instance.GetComment(customerId);
        }
    }
}
