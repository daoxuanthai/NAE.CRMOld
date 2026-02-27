using System;
using System.Data.Common;

namespace tcs.dao
{
    public static class SqlHelper
    {
        public static T Read<T>(DbDataReader dataReader, string fieldName)
        {
            int fieldIndex;
            try { fieldIndex = dataReader.GetOrdinal(fieldName); }
            catch { return default(T); }

            if (dataReader.IsDBNull(fieldIndex))
            {
                return default(T);
            }
            else
            {
                object readData = dataReader.GetValue(fieldIndex);
                if (readData is T)
                {
                    return (T)readData;
                }
                else
                {
                    try
                    {
                        return (T)Convert.ChangeType(readData, typeof(T));
                    }
                    catch (InvalidCastException)
                    {
                        return default(T);
                    }
                }
            }
        }
    }
}
