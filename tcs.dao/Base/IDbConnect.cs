using System;
using System.Collections.Generic;
using System.Data;
using tcs.bo;

namespace tcs.dao
{
    public abstract class IDbConnect<T>
    {
        public IDbConnection Connection { get; }

        protected IDbConnect(IDbConnection connection)
        {
            Connection = connection ?? throw new ArgumentNullException();
        }

        public abstract List<T> Select(IQuery query);

        public abstract int Create(T obj);

        public abstract T Read(int id);

        public abstract bool Update(T obj);

        public abstract bool Delete(string ids, int companyId, int userId, string userName);
    }
}
