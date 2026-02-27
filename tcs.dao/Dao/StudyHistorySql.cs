using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using tcs.bo;
using tcs.lib;

namespace tcs.dao
{
    public class StudyHistorySql : IDbConnect<StudyHistoryBo>
    {
        private static StudyHistorySql _instance;

        public static StudyHistorySql Instance =>
            _instance ?? (_instance = new StudyHistorySql(new SqlConnection(ConfigMgr.ConnectionString)));

        public StudyHistorySql(SqlConnection connection) : base(connection) { }

        public override List<StudyHistoryBo> Select(IQuery query)
        {
            var qr = query as StudyHistoryQuery;
            var ret = new List<StudyHistoryBo>();
            var cmd = Connection.CreateCommand() as SqlCommand;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "StudyHistory_Search";
            cmd.Parameters.Add(new SqlParameter("Keyword", qr.Keyword));
            cmd.Parameters.Add(new SqlParameter("CurrentPage", qr.Page));
            cmd.Parameters.Add(new SqlParameter("RecordPerPage", qr.PageSize));

            try
            {
                Connection.Open();

                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var tmp = new StudyHistoryBo
                    {
                        Id = SqlHelper.Read<int>(reader, "Id"),
                        CustomerId = SqlHelper.Read<int>(reader, "CustomerId"),
                        CompanyId = SqlHelper.Read<int>(reader, "CompanyId"),
                        School = SqlHelper.Read<string>(reader, "School"),
                        Major = SqlHelper.Read<string>(reader, "Major"),
                        Score = SqlHelper.Read<string>(reader, "Score"),
                        Class = SqlHelper.Read<string>(reader, "Class"),
                        GraduateDate = SqlHelper.Read<DateTime>(reader, "GraduateDate"),
                        Note = SqlHelper.Read<string>(reader, "Note"),
                        CreateUserId = SqlHelper.Read<int>(reader, "CreateUserId"),
                        CreateUserName = SqlHelper.Read<string>(reader, "CreateUserName"),
                        UpdateUserId = SqlHelper.Read<int>(reader, "UpdateUserId"),
                        UpdateUserName = SqlHelper.Read<string>(reader, "UpdateUserName"),
                        CreateDate = SqlHelper.Read<DateTime>(reader, "CreateDate"),
                        UpdateDate = SqlHelper.Read<DateTime>(reader, "UpdateDate"),
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

        public override int Create(StudyHistoryBo obj)
        {
            var id = 0;
            var cmd = Connection.CreateCommand() as SqlCommand;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "StudyHistory_Insert";
            cmd.Parameters.AddWithValue("CustomerId", obj.CustomerId);
            cmd.Parameters.AddWithValue("CompanyId", obj.CompanyId);
            cmd.Parameters.AddWithValue("School", obj.School);
            cmd.Parameters.AddWithValue("Major", obj.Major);
            cmd.Parameters.AddWithValue("Score", obj.Score);
            cmd.Parameters.AddWithValue("Class", obj.Class);
            cmd.Parameters.AddWithValue("GraduateDate", obj.GraduateDate.HasValue && obj.GraduateDate != DateTime.MinValue ? obj.GraduateDate : null);
            cmd.Parameters.AddWithValue("Note", obj.Note);
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

        public override StudyHistoryBo Read(int id)
        {
            StudyHistoryBo tmp = null;
            var cmd = Connection.CreateCommand() as SqlCommand;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "StudyHistory_Select";
            cmd.Parameters.Add(new SqlParameter("ID", id));

            try
            {
                Connection.Open();

                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    tmp = new StudyHistoryBo
                    {
                        Id = SqlHelper.Read<int>(reader, "Id"),
                        CustomerId = SqlHelper.Read<int>(reader, "CustomerId"),
                        CompanyId = SqlHelper.Read<int>(reader, "CompanyId"),
                        School = SqlHelper.Read<string>(reader, "School"),
                        Major = SqlHelper.Read<string>(reader, "Major"),
                        Score = SqlHelper.Read<string>(reader, "Score"),
                        Class = SqlHelper.Read<string>(reader, "Class"),
                        GraduateDate = SqlHelper.Read<DateTime>(reader, "GraduateDate"),
                        Note = SqlHelper.Read<string>(reader, "Note"),
                        CreateUserId = SqlHelper.Read<int>(reader, "CreateUserId"),
                        CreateUserName = SqlHelper.Read<string>(reader, "CreateUserName"),
                        UpdateUserId = SqlHelper.Read<int>(reader, "UpdateUserId"),
                        UpdateUserName = SqlHelper.Read<string>(reader, "UpdateUserName"),
                        CreateDate = SqlHelper.Read<DateTime>(reader, "CreateDate"),
                        UpdateDate = SqlHelper.Read<DateTime>(reader, "UpdateDate")
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

        public override bool Update(StudyHistoryBo obj)
        {
            int num = 0;
            var cmd = Connection.CreateCommand() as SqlCommand;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "StudyHistory_Update";
            cmd.Parameters.AddWithValue("Id", obj.Id);
            cmd.Parameters.AddWithValue("School", obj.School);
            cmd.Parameters.AddWithValue("Major", obj.Major);
            cmd.Parameters.AddWithValue("Score", obj.Score);
            cmd.Parameters.AddWithValue("Class", obj.Class);
            cmd.Parameters.AddWithValue("GraduateDate",
                obj.GraduateDate.HasValue && obj.GraduateDate != DateTime.MinValue ? obj.GraduateDate : null);
            cmd.Parameters.AddWithValue("Note", obj.Note);
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
            cmd.CommandText = "StudyHistory_Delete";
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

        public List<StudyHistoryBo> GetByCustomer(int customerId)
        {
            var ret = new List<StudyHistoryBo>();
            var cmd = Connection.CreateCommand() as SqlCommand;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "StudyHistory_GetByCustomer";
            cmd.Parameters.Add(new SqlParameter("CustomerId", customerId));

            try
            {
                Connection.Open();

                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var tmp = new StudyHistoryBo
                    {
                        Id = SqlHelper.Read<int>(reader, "Id"),
                        CustomerId = SqlHelper.Read<int>(reader, "CustomerId"),
                        CompanyId = SqlHelper.Read<int>(reader, "CompanyId"),
                        School = SqlHelper.Read<string>(reader, "School"),
                        Major = SqlHelper.Read<string>(reader, "Major"),
                        Score = SqlHelper.Read<string>(reader, "Score"),
                        Class = SqlHelper.Read<string>(reader, "Class"),
                        GraduateDate = SqlHelper.Read<DateTime>(reader, "GraduateDate"),
                        Note = SqlHelper.Read<string>(reader, "Note"),
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

