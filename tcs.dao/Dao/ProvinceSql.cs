using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using tcs.bo;
using tcs.lib;

namespace tcs.dao
{
    public class ProvinceSql : IDbConnect<ProvinceBo>
    {
        private static ProvinceSql _instance;

        public static ProvinceSql Instance => _instance ??
                                              (_instance =
                                                  new ProvinceSql(new SqlConnection(ConfigMgr.AccountConnectionString))
                                              );
        public ProvinceSql(SqlConnection connection) : base(connection) { }

        public override List<ProvinceBo> Select(IQuery query)
        {
            var ret = new List<ProvinceBo>();
            var cmd = Connection.CreateCommand() as SqlCommand;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "Province_GetAll";

            try
            {
                Connection.Open();

                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var tmp = new ProvinceBo
                    {
                        Id = SqlHelper.Read<int>(reader, "Id"),
                        ProvinceName = SqlHelper.Read<string>(reader, "ProvinceName"),
                        ProvinceCode = SqlHelper.Read<string>(reader, "ProvinceCode"),
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

        public override int Create(ProvinceBo obj)
        {
            return 0;
        }

        public override ProvinceBo Read(int id)
        {
            throw new NotImplementedException();
        }

        public override bool Update(ProvinceBo obj)
        {
            return false;
        }

        public override bool Delete(string ids, int companyId, int userId, string userName)
        {
            return false;
        }
    }
}

