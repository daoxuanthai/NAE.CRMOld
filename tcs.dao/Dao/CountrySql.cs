using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using tcs.bo;
using tcs.lib;

namespace tcs.dao
{
    public class CountrySql : IDbConnect<CountryBo>
    {
        private static CountrySql _instance;

        public static CountrySql Instance =>
            _instance ?? (_instance = new CountrySql(new SqlConnection(ConfigMgr.AccountConnectionString)));
        public CountrySql(SqlConnection connection) : base(connection) { }

        public override List<CountryBo> Select(IQuery query)
        {
            var ret = new List<CountryBo>();
            var cmd = Connection.CreateCommand() as SqlCommand;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "Country_GetAll";

            try
            {
                Connection.Open();

                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var tmp = new CountryBo
                    {
                        Id = SqlHelper.Read<int>(reader, "Id"),
                        CountryName = SqlHelper.Read<string>(reader, "CountryName"),
                        CountryCode = SqlHelper.Read<string>(reader, "CountryCode"),
                        OrderNo = SqlHelper.Read<int>(reader, "OrderNo")
                    };

                    ret.Add(tmp);
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                cmd.Dispose();
                Connection.Close();
            }

            return ret;
        }

        public override int Create(CountryBo obj)
        {
            return 0;
        }

        public override CountryBo Read(int id)
        {
            return null;
        }

        public override bool Update(CountryBo obj)
        {
            return false;
        }

        public override bool Delete(string ids, int companyId, int userId, string userName)
        {
            return false;
        }
    }
}

