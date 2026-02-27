using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using tcs.bo;
using tcs.lib;

namespace tcs.dao
{
    public class SeminarSql : IDbConnect<SeminarBo>
    {
        private static SeminarSql _instance;

        public static SeminarSql Instance => _instance ?? (_instance = new SeminarSql(new SqlConnection(ConfigMgr.ConnectionString)));
        public SeminarSql(SqlConnection connection) : base(connection) { }

        public override List<SeminarBo> Select(IQuery query)
        {
            var qr = query as SeminarQuery;
            var ret = new List<SeminarBo>();
            var cmd = Connection.CreateCommand() as SqlCommand;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "Seminar_Search";
            cmd.Parameters.Add(new SqlParameter("Keyword", qr.Keyword));
            cmd.Parameters.Add(new SqlParameter("Status", qr.Status));
            cmd.Parameters.Add(new SqlParameter("CompanyId", qr.Company));
            cmd.Parameters.Add(new SqlParameter("CurrentPage", qr.Page));
            cmd.Parameters.Add(new SqlParameter("RecordPerPage", qr.PageSize));

            try
            {
                Connection.Open();

                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var tmp = new SeminarBo
                    {
                        Id = SqlHelper.Read<int>(reader, "Id"),
                        CompanyId = SqlHelper.Read<int>(reader, "CompanyId"),
                        Name = SqlHelper.Read<string>(reader, "Name"),
                        Description = SqlHelper.Read<string>(reader, "Description"),
                        Note = SqlHelper.Read<string>(reader, "Note"),
                        ProvinceId = SqlHelper.Read<string>(reader, "ProvinceId"),
                        ProvinceName = SqlHelper.Read<string>(reader, "ProvinceName"),
                        Time = SqlHelper.Read<string>(reader, "Time"),
                        Link = SqlHelper.Read<string>(reader, "Link"),
                        Register = SqlHelper.Read<int>(reader, "Register"),
                        Attend = SqlHelper.Read<int>(reader, "Attend"),
                        Status = SqlHelper.Read<int>(reader, "Status"),
                        CreateUserId = SqlHelper.Read<int>(reader, "CreateUserId"),
                        CreateUserName = SqlHelper.Read<string>(reader, "CreateUserName"),
                        UpdateUserId = SqlHelper.Read<int>(reader, "UpdateUserId"),
                        UpdateUserName = SqlHelper.Read<string>(reader, "UpdateUserName"),
                        CreateDate = SqlHelper.Read<DateTime>(reader, "CreateDate"),
                        UpdateDate = SqlHelper.Read<DateTime>(reader, "UpdateDate")
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

        public override int Create(SeminarBo obj)
        {
            var id = 0;
            var cmd = Connection.CreateCommand() as SqlCommand;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "Seminar_Insert";
            cmd.Parameters.AddWithValue("CompanyId", obj.CompanyId);
            cmd.Parameters.AddWithValue("Name", obj.Name);
            cmd.Parameters.AddWithValue("Description", obj.Description);
            cmd.Parameters.AddWithValue("Note", obj.Note);
            cmd.Parameters.AddWithValue("ProvinceId", obj.ProvinceId);
            cmd.Parameters.AddWithValue("ProvinceName", obj.ProvinceName);
            cmd.Parameters.AddWithValue("Time", obj.Time);
            cmd.Parameters.AddWithValue("Link", obj.Link);
            cmd.Parameters.AddWithValue("Status", obj.Status);
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

        public override SeminarBo Read(int id)
        {
            SeminarBo tmp = null;
            var cmd = Connection.CreateCommand() as SqlCommand;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "Seminar_Select";
            cmd.Parameters.Add(new SqlParameter("ID", id));

            try
            {
                Connection.Open();

                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    tmp = new SeminarBo
                    {
                        Id = SqlHelper.Read<int>(reader, "Id"),
                        CompanyId = SqlHelper.Read<int>(reader, "CompanyId"),
                        Name = SqlHelper.Read<string>(reader, "Name"),
                        Description = SqlHelper.Read<string>(reader, "Description"),
                        Note = SqlHelper.Read<string>(reader, "Note"),
                        ProvinceId = SqlHelper.Read<string>(reader, "ProvinceId"),
                        ProvinceName = SqlHelper.Read<string>(reader, "ProvinceName"),
                        Time = SqlHelper.Read<string>(reader, "Time"),
                        Link = SqlHelper.Read<string>(reader, "Link"),
                        Register = SqlHelper.Read<int>(reader, "Register"),
                        Attend = SqlHelper.Read<int>(reader, "Attend"),
                        Status = SqlHelper.Read<int>(reader, "Status"),
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

        public override bool Update(SeminarBo obj)
        {
            int num = 0;
            var cmd = Connection.CreateCommand() as SqlCommand;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "Seminar_Update";
            cmd.Parameters.AddWithValue("Id", obj.Id);
            cmd.Parameters.AddWithValue("Name", obj.Name);
            cmd.Parameters.AddWithValue("Description", obj.Description);
            cmd.Parameters.AddWithValue("Note", obj.Note);
            cmd.Parameters.AddWithValue("ProvinceId", obj.ProvinceId);
            cmd.Parameters.AddWithValue("ProvinceName", obj.ProvinceName);
            cmd.Parameters.AddWithValue("Time", obj.Time);
            cmd.Parameters.AddWithValue("Link", obj.Link);
            cmd.Parameters.AddWithValue("Status", obj.Status);
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
            cmd.CommandText = "Seminar_Delete";
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

        public List<SeminarBo> GetByCompany(int companyId)
        {
            var ret = new List<SeminarBo>();
            var cmd = Connection.CreateCommand() as SqlCommand;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "Seminar_GetByCompany";
            cmd.Parameters.Add(new SqlParameter("CompanyId", companyId));

            try
            {
                Connection.Open();

                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var tmp = new SeminarBo
                    {
                        Id = SqlHelper.Read<int>(reader, "Id"),
                        CompanyId = SqlHelper.Read<int>(reader, "CompanyId"),
                        Name = SqlHelper.Read<string>(reader, "Name"),
                        Description = SqlHelper.Read<string>(reader, "Description"),
                        Note = SqlHelper.Read<string>(reader, "Note"),
                        ProvinceId = SqlHelper.Read<string>(reader, "ProvinceId"),
                        ProvinceName = SqlHelper.Read<string>(reader, "ProvinceName"),
                        Time = SqlHelper.Read<string>(reader, "Time"),
                        Link = SqlHelper.Read<string>(reader, "Link"),
                        Register = SqlHelper.Read<int>(reader, "Register"),
                        Attend = SqlHelper.Read<int>(reader, "Attend"),
                        Status = SqlHelper.Read<int>(reader, "Status"),
                        CreateUserId = SqlHelper.Read<int>(reader, "CreateUserId"),
                        CreateUserName = SqlHelper.Read<string>(reader, "CreateUserName"),
                        UpdateUserId = SqlHelper.Read<int>(reader, "UpdateUserId"),
                        UpdateUserName = SqlHelper.Read<string>(reader, "UpdateUserName"),
                        CreateDate = SqlHelper.Read<DateTime>(reader, "CreateDate"),
                        UpdateDate = SqlHelper.Read<DateTime>(reader, "UpdateDate")
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

