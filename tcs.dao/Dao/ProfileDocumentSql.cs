using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using tcs.bo;
using tcs.lib;

namespace tcs.dao
{
    public class ProfileDocumentSql : IDbConnect<ProfileDocumentBo>
    {
        private static ProfileDocumentSql _instance;

        public static ProfileDocumentSql Instance => _instance ?? (_instance = new ProfileDocumentSql(new SqlConnection(ConfigMgr.ConnectionString)));
        public ProfileDocumentSql(SqlConnection connection) : base(connection) { }

        public override List<ProfileDocumentBo> Select(IQuery query)
        {
            throw new NotImplementedException();
        }

        public override int Create(ProfileDocumentBo obj)
        {
            var id = 0;
            var cmd = Connection.CreateCommand() as SqlCommand;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "ProfileDocument_Insert";
            cmd.Parameters.AddWithValue("ProfileTypeId", obj.ProfileTypeId);
            cmd.Parameters.AddWithValue("ProfileStepId", obj.ProfileStepId);
            cmd.Parameters.AddWithValue("DocumentGroupId", obj.DocumentGroupId);
            cmd.Parameters.AddWithValue("DocumentName", obj.DocumentName);
            cmd.Parameters.AddWithValue("Note", obj.Note);
            cmd.Parameters.AddWithValue("DisplayOrder", obj.DisplayOrder);
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

        public override ProfileDocumentBo Read(int id)
        {
            ProfileDocumentBo tmp = null;
            var cmd = Connection.CreateCommand() as SqlCommand;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "ProfileDocument_Select";
            cmd.Parameters.Add(new SqlParameter("ID", id));

            try
            {
                Connection.Open();

                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    tmp = new ProfileDocumentBo
                    {
                        Id = SqlHelper.Read<int>(reader, "Id"),
                        ProfileTypeId = SqlHelper.Read<int>(reader, "ProfileTypeId"),
                        ProfileStepId = SqlHelper.Read<int>(reader, "ProfileStepId"),
                        DocumentGroupId = SqlHelper.Read<int>(reader, "DocumentGroupId"),
                        DocumentName = SqlHelper.Read<string>(reader, "DocumentName"),
                        Note = SqlHelper.Read<string>(reader, "Note"),
                        DisplayOrder = SqlHelper.Read<int>(reader, "DisplayOrder"),
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

        public override bool Update(ProfileDocumentBo obj)
        {
            int num = 0;
            var cmd = Connection.CreateCommand() as SqlCommand;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "ProfileDocument_Update";
            cmd.Parameters.AddWithValue("Id", obj.Id);
            cmd.Parameters.AddWithValue("ProfileTypeId", obj.ProfileTypeId);
            cmd.Parameters.AddWithValue("ProfileStepId", obj.ProfileStepId);
            cmd.Parameters.AddWithValue("DocumentGroupId", obj.DocumentGroupId);
            cmd.Parameters.AddWithValue("DocumentName", obj.DocumentName);
            cmd.Parameters.AddWithValue("Note", obj.Note);
            cmd.Parameters.AddWithValue("DisplayOrder", obj.DisplayOrder);
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
            cmd.CommandText = "ProfileDocument_Delete";
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

        public List<ProfileDocumentBo> GetByProfileType(int typeId)
        {
            var ret = new List<ProfileDocumentBo>();
            var cmd = Connection.CreateCommand() as SqlCommand;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "ProfileDocument_Search";
            cmd.Parameters.Add(new SqlParameter("ProfileTypeId", typeId));

            try
            {
                Connection.Open();

                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var tmp = new ProfileDocumentBo
                    {
                        Id = SqlHelper.Read<int>(reader, "Id"),
                        ProfileTypeId = SqlHelper.Read<int>(reader, "ProfileTypeId"),
                        ProfileStepId = SqlHelper.Read<int>(reader, "ProfileStepId"),
                        DocumentGroupId = SqlHelper.Read<int>(reader, "DocumentGroupId"),
                        DocumentName = SqlHelper.Read<string>(reader, "DocumentName"),
                        Note = SqlHelper.Read<string>(reader, "Note"),
                        DisplayOrder = SqlHelper.Read<int>(reader, "DisplayOrder"),
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
    }
}

