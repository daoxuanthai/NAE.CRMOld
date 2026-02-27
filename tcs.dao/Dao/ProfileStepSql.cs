using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using tcs.bo;
using tcs.lib;

namespace tcs.dao
{
    public class ProfileStepSql : IDbConnect<ProfileStepBo>
    {
        private static ProfileStepSql _instance;

        public static ProfileStepSql Instance => _instance ?? (_instance = new ProfileStepSql(new SqlConnection(ConfigMgr.ConnectionString)));
        public ProfileStepSql(SqlConnection connection) : base(connection) { }

        public override List<ProfileStepBo> Select(IQuery query)
        {
            var qr = query as ProfileStepQuery;
            var ret = new List<ProfileStepBo>();
            var cmd = Connection.CreateCommand() as SqlCommand;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "ProfileStep_Search";
            cmd.Parameters.Add(new SqlParameter("ProfileTypeId", qr.ProfileTypeId));

            try
            {
                Connection.Open();

                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var tmp = new ProfileStepBo
                    {
                        Id = SqlHelper.Read<int>(reader, "Id"),
                        ProfileTypeId = SqlHelper.Read<int>(reader, "ProfileTypeId"),
                        ParentId = SqlHelper.Read<int>(reader, "ParentId"),
                        StepName = SqlHelper.Read<string>(reader, "StepName"),
                        DisplayOrder = SqlHelper.Read<int>(reader, "DisplayOrder"),
                        IsStep = SqlHelper.Read<bool>(reader, "IsStep"),
                        CreateUserId = SqlHelper.Read<int>(reader, "CreateUserId"),
                        CreateUserName = SqlHelper.Read<string>(reader, "CreateUserName"),
                        CreateDate = SqlHelper.Read<DateTime>(reader, "CreateDate")
                    };
                    if (qr.TotalRecord <= 0)
                        qr.TotalRecord = SqlHelper.Read<int>(reader, "TotalRecord");

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

        public override int Create(ProfileStepBo obj)
        {
            var id = 0;
            var cmd = Connection.CreateCommand() as SqlCommand;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "ProfileStep_Insert";
            cmd.Parameters.AddWithValue("ProfileTypeId", obj.ProfileTypeId);
            cmd.Parameters.AddWithValue("ParentId", obj.ParentId);
            cmd.Parameters.AddWithValue("StepName", obj.StepName);
            cmd.Parameters.AddWithValue("DisplayOrder", obj.DisplayOrder);
            cmd.Parameters.AddWithValue("IsStep", obj.IsStep);
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

        public override ProfileStepBo Read(int id)
        {
            ProfileStepBo tmp = null;
            var cmd = Connection.CreateCommand() as SqlCommand;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "ProfileStep_Select";
            cmd.Parameters.Add(new SqlParameter("ID", id));

            try
            {
                Connection.Open();

                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    tmp = new ProfileStepBo
                    {
                        Id = SqlHelper.Read<int>(reader, "Id"),
                        ProfileTypeId = SqlHelper.Read<int>(reader, "ProfileTypeId"),
                        ParentId = SqlHelper.Read<int>(reader, "ParentId"),
                        StepName = SqlHelper.Read<string>(reader, "StepName"),
                        DisplayOrder = SqlHelper.Read<int>(reader, "DisplayOrder"),
                        IsStep = SqlHelper.Read<bool>(reader, "IsStep"),
                        CreateUserId = SqlHelper.Read<int>(reader, "CreateUserId"),
                        CreateUserName = SqlHelper.Read<string>(reader, "CreateUserName"),
                        CreateDate = SqlHelper.Read<DateTime>(reader, "CreateDate")
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

        public override bool Update(ProfileStepBo obj)
        {
            int num = 0;
            var cmd = Connection.CreateCommand() as SqlCommand;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "ProfileStep_Update";
            cmd.Parameters.AddWithValue("Id", obj.Id);
            cmd.Parameters.AddWithValue("ProfileTypeId", obj.ProfileTypeId);
            cmd.Parameters.AddWithValue("ParentId", obj.ParentId);
            cmd.Parameters.AddWithValue("StepName", obj.StepName);
            cmd.Parameters.AddWithValue("DisplayOrder", obj.DisplayOrder);
            cmd.Parameters.AddWithValue("IsStep", obj.IsStep);

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
            cmd.CommandText = "ProfileStep_Delete";
            cmd.Parameters.AddWithValue("Id", ids);
            cmd.Parameters.AddWithValue("CompanyId", companyId);

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

        public List<ProfileStepBo> GetByProfileType(int typeId)
        {
            var qr = new ProfileStepQuery() {
                ProfileTypeId = typeId
            };
            var ret = new List<ProfileStepBo>();
            var cmd = Connection.CreateCommand() as SqlCommand;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "ProfileStep_Search";
            cmd.Parameters.Add(new SqlParameter("ProfileTypeId", qr.ProfileTypeId));

            try
            {
                Connection.Open();

                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var tmp = new ProfileStepBo
                    {
                        Id = SqlHelper.Read<int>(reader, "Id"),
                        ProfileTypeId = SqlHelper.Read<int>(reader, "ProfileTypeId"),
                        ParentId = SqlHelper.Read<int>(reader, "ParentId"),
                        StepName = SqlHelper.Read<string>(reader, "StepName"),
                        DisplayOrder = SqlHelper.Read<int>(reader, "DisplayOrder"),
                        IsStep = SqlHelper.Read<bool>(reader, "IsStep"),
                        CreateUserId = SqlHelper.Read<int>(reader, "CreateUserId"),
                        CreateUserName = SqlHelper.Read<string>(reader, "CreateUserName"),
                        CreateDate = SqlHelper.Read<DateTime>(reader, "CreateDate")
                    };
                    if (qr.TotalRecord <= 0)
                        qr.TotalRecord = SqlHelper.Read<int>(reader, "TotalRecord");

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

