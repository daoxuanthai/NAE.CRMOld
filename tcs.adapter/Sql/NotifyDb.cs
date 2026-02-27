
using System.Collections.Generic;
using tcs.bo;
using tcs.dao;

namespace tcs.adapter.Sql
{
    public class NotifyDb
    {
        private static NotifyDb _instance;

        public static NotifyDb Instance => _instance ?? (_instance = new NotifyDb());

        public int Create(NotifyBo obj)
        {
            return NotifySql.Instance.Create(obj);
        }

        public List<NotifyBo> Select(NotifyQuery query = null)
        {
            if (query == null)
                query = new NotifyQuery();

            return NotifySql.Instance.Select(query);
        }

        public bool UpdateRead(string ids)
        {
            return NotifySql.Instance.UpdateRead(ids);
        }

        public bool CreateNotify(int titleId, string title, string message, int objectType = -1, string objectTypeName = "", 
                                    int objectId = -1, int type = -1, int priority = -1, int createUserId = -1, string createUserName = "")
        {
            var notify = new NotifyBo()
            {
                TitleId = titleId,
                Title = title, 
                Message = message,
                ObjectId = objectId,
                ObjectType = objectType,
                ObjectTypeName = objectTypeName,
                Type = type,
                Priority = priority,
                CreateUserId = createUserId,
                CreateUserName = createUserName
            };
            return Create(notify) > 0;
        }

        public List<NotifyBo> GetNew(int titleId, int top, ref int notRead)
        {
            var qr = new NotifyQuery()
            {
                TitleId = titleId,
                ObjectType = -1,
                Type = -1,
                IsRead = false,
                Page = 0,
                PageSize = top
            };

            var lstNotify = Select(qr);
            notRead = qr.NotRead;
            return lstNotify;
        }
    }
}
