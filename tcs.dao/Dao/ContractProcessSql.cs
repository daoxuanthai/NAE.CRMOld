using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using tcs.bo;
using tcs.lib;

namespace tcs.dao
{
    public class ContractProcessSql : IDbConnect<ContractProcessBo>
    {
        private static ContractProcessSql _instance;

        public static ContractProcessSql Instance => _instance ??
                                                    (_instance = new ContractProcessSql(
                                                        new SqlConnection(ConfigMgr.ConnectionString)));
        public ContractProcessSql(SqlConnection connection) : base(connection) { }

        public override List<ContractProcessBo> Select(IQuery query)
        {
            throw new NotImplementedException();
        }

        public override int Create(ContractProcessBo obj)
        {
            var id = 0;
            var cmd = Connection.CreateCommand() as SqlCommand;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "ContractProcess_Insert";
            cmd.Parameters.AddWithValue("ContractId", obj.ContractId);
            cmd.Parameters.AddWithValue("ContractData", obj.ContractData);
            cmd.Parameters.AddWithValue("ProcessNote", obj.ProcessNote);
            cmd.Parameters.AddWithValue("CreateUserId", obj.CreateUserId);
            cmd.Parameters.AddWithValue("CreateUserName", obj.CreateUserName);

            SqlParameter outId = new SqlParameter
            {
                ParameterName = "ID",
                DbType = DbType.Int32,
                Direction = ParameterDirection.Output
            };
            cmd.Parameters.Add(outId);

            try
            {
                Connection.Open();
                cmd.ExecuteNonQuery();
                id = (int)outId.Value;
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

            return id;
        }

        public override ContractProcessBo Read(int id)
        {
            throw new NotImplementedException();
        }

        public override bool Update(ContractProcessBo obj)
        {
            throw new NotImplementedException();
        }

        public override bool Delete(string ids, int companyId, int userId, string userName)
        {
            throw new NotImplementedException();
        }

        public List<ContractProcessBo> GetByContract(int contractId)
        {
            var ret = new List<ContractProcessBo>();
            var cmd = Connection.CreateCommand() as SqlCommand;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "ContractProcess_GetByContract";
            cmd.Parameters.Add(new SqlParameter("ContractId", contractId));

            try
            {
                Connection.Open();

                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var tmp = new ContractProcessBo
                    {
                        Id = SqlHelper.Read<int>(reader, "Id"),
                        ContractId = SqlHelper.Read<int>(reader, "ContractId"),
                        ContractData = SqlHelper.Read<string>(reader, "ContractData"),
                        ProcessNote = SqlHelper.Read<string>(reader, "ProcessNote"),
                        CreateUserId = SqlHelper.Read<int>(reader, "CreateUserId"),
                        CreateUserName = SqlHelper.Read<string>(reader, "CreateUserName"),
                        CreateDate = SqlHelper.Read<DateTime>(reader, "CreateDate")
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
    }
}

