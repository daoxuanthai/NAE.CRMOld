using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using tcs.bo;
using tcs.lib;

namespace tcs.dao
{
    public class ContractDocumentSql : IDbConnect<ContractDocumentBo>
    {
        private static ContractDocumentSql _instance;

        public static ContractDocumentSql Instance => _instance ?? (_instance = new ContractDocumentSql(new SqlConnection(ConfigMgr.ConnectionString)));
        public ContractDocumentSql(SqlConnection connection) : base(connection) { }

        public override List<ContractDocumentBo> Select(IQuery query)
        {
            throw new NotImplementedException();
        }

        public override int Create(ContractDocumentBo obj)
        {
            var id = 0;
            var cmd = Connection.CreateCommand() as SqlCommand;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "ContractDocument_Insert";
            cmd.Parameters.AddWithValue("ContractId", obj.ContractId);
            cmd.Parameters.AddWithValue("ProfileTypeId", obj.ProfileTypeId);
            cmd.Parameters.AddWithValue("ProfileStepId", obj.ProfileStepId);
            cmd.Parameters.AddWithValue("ProfileDocumentId", obj.ProfileDocumentId);
            cmd.Parameters.AddWithValue("AttachFile", obj.AttachFile);
            cmd.Parameters.AddWithValue("Note", obj.Note);
            cmd.Parameters.AddWithValue("IsDone", obj.IsDone);
            cmd.Parameters.AddWithValue("IsSkip", obj.IsSkip);
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

        public override ContractDocumentBo Read(int id)
        {
            ContractDocumentBo tmp = null;
            var cmd = Connection.CreateCommand() as SqlCommand;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "ContractDocument_Select";
            cmd.Parameters.Add(new SqlParameter("ID", id));

            try
            {
                Connection.Open();

                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    tmp = new ContractDocumentBo
                    {
                        Id = SqlHelper.Read<int>(reader, "Id"),
                        ContractId = SqlHelper.Read<int>(reader, "ContractId"),
                        ProfileTypeId = SqlHelper.Read<int>(reader, "ProfileTypeId"),
                        ProfileStepId = SqlHelper.Read<int>(reader, "ProfileStepId"),
                        ProfileDocumentId = SqlHelper.Read<int>(reader, "ProfileDocumentId"),
                        AttachFile = SqlHelper.Read<string>(reader, "AttachFile"),
                        Note = SqlHelper.Read<string>(reader, "Note"),
                        IsDone = SqlHelper.Read<bool>(reader, "IsDone"),
                        IsSkip = SqlHelper.Read<bool>(reader, "IsSkip"),
                        CreateUserId = SqlHelper.Read<int>(reader, "CreateUserId"),
                        CreateUserName = SqlHelper.Read<string>(reader, "CreateUserName"),
                        UpdateUserId = SqlHelper.Read<int>(reader, "UpdateUserId"),
                        UpdateUserName = SqlHelper.Read<string>(reader, "UpdateUserName"),
                        CreateDate = SqlHelper.Read<DateTime>(reader, "CreateDate"),
                        UpdateDate = SqlHelper.Read<DateTime>(reader, "UpdateDate"),
                    };
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

            return tmp;
        }

        public override bool Update(ContractDocumentBo obj)
        {
            int num = 0;
            var cmd = Connection.CreateCommand() as SqlCommand;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "ContractDocument_Update";
            cmd.Parameters.AddWithValue("Id", obj.Id);
            cmd.Parameters.AddWithValue("AttachFile", obj.AttachFile);
            cmd.Parameters.AddWithValue("IsDone", obj.IsDone);
            cmd.Parameters.AddWithValue("IsSkip", obj.IsSkip);
            cmd.Parameters.AddWithValue("UpdateUserId", obj.UpdateUserId);
            cmd.Parameters.AddWithValue("UpdateUserName", obj.UpdateUserName);

            try
            {
                Connection.Open();
                num = cmd.ExecuteNonQuery();
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

            return num > 0;
        }

        public override bool Delete(string ids, int companyId, int userId, string userName)
        {
            int num = 0;
            var cmd = Connection.CreateCommand() as SqlCommand;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "ContractDocument_Delete";
            cmd.Parameters.AddWithValue("Id", ids);
            cmd.Parameters.AddWithValue("CompanyId", companyId);
            cmd.Parameters.AddWithValue("DeleteUserId", userId);
            cmd.Parameters.AddWithValue("DeleteUserName", userName);

            try
            {
                Connection.Open();
                num = cmd.ExecuteNonQuery();
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

            return num > 0;
        }

        public List<ContractDocumentBo> GetByContractId(int id, int profileTypeId)
        {
            var ret = new List<ContractDocumentBo>();
            var cmd = Connection.CreateCommand() as SqlCommand;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "ContractDocument_Search";
            cmd.Parameters.Add(new SqlParameter("ContractId", id));
            cmd.Parameters.Add(new SqlParameter("ProfileTypeId", profileTypeId));

            try
            {
                Connection.Open();

                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var tmp = new ContractDocumentBo
                    {
                        Id = SqlHelper.Read<int>(reader, "Id"),
                        ContractId = SqlHelper.Read<int>(reader, "ContractId"),
                        ProfileTypeId = SqlHelper.Read<int>(reader, "ProfileTypeId"),
                        ProfileStepId = SqlHelper.Read<int>(reader, "ProfileStepId"),
                        ProfileDocumentId = SqlHelper.Read<int>(reader, "ProfileDocumentId"),
                        AttachFile = SqlHelper.Read<string>(reader, "AttachFile"),
                        Note = SqlHelper.Read<string>(reader, "Note"),
                        IsDone = SqlHelper.Read<bool>(reader, "IsDone"),
                        IsSkip = SqlHelper.Read<bool>(reader, "IsSkip"),
                        CreateUserId = SqlHelper.Read<int>(reader, "CreateUserId"),
                        CreateUserName = SqlHelper.Read<string>(reader, "CreateUserName"),
                        UpdateUserId = SqlHelper.Read<int>(reader, "UpdateUserId"),
                        UpdateUserName = SqlHelper.Read<string>(reader, "UpdateUserName"),
                        CreateDate = SqlHelper.Read<DateTime>(reader, "CreateDate"),
                        UpdateDate = SqlHelper.Read<DateTime>(reader, "UpdateDate"),
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

        /// <summary>
        /// Xóa tất cả data cũ và khởi tạo lại data mới theo document được khai báo
        /// </summary>
        /// <param name="id"></param>
        /// <param name="profileTypeId"></param>
        /// <returns></returns>
        public bool InitDocument(int id, int profileTypeId)
        {
            int num = 0;
            var cmd = Connection.CreateCommand() as SqlCommand;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "ContractDocument_Init";
            cmd.Parameters.Add(new SqlParameter("ContractId", id));
            cmd.Parameters.Add(new SqlParameter("ProfileTypeId", profileTypeId));

            try
            {
                Connection.Open();
                num = cmd.ExecuteNonQuery();
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

            return num > 0;
        }
    }
}

