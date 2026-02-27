using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using tcs.bo;
using tcs.lib;

namespace tcs.dao
{
    public class ScholarshipSql : IDbConnect<ScholarshipBo>
    {
        private static ScholarshipSql _instance;

        public static ScholarshipSql Instance => _instance ?? (_instance = new ScholarshipSql(new SqlConnection(ConfigMgr.ConnectionString)));

        public ScholarshipSql(SqlConnection connection) : base(connection) { }

        public override List<ScholarshipBo> Select(IQuery query)
        {
            var qr = query as ScholarshipQuery;
            var ret = new List<ScholarshipBo>();
            var cmd = Connection.CreateCommand() as SqlCommand;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "Scholarship_Search";
            cmd.Parameters.Add(new SqlParameter("Keyword", qr.Keyword));
            cmd.Parameters.Add(new SqlParameter("CompanyId", qr.Company));
            cmd.Parameters.Add(new SqlParameter("CountryId", qr.CountryId));
            cmd.Parameters.Add(new SqlParameter("SchoolId", qr.SchoolId));
            cmd.Parameters.Add(new SqlParameter("ScholarshipType", qr.ScholarshipType));
            cmd.Parameters.Add(new SqlParameter("FromDate", qr.From));
            cmd.Parameters.Add(new SqlParameter("ToDate", qr.To));
            cmd.Parameters.Add(new SqlParameter("CurrentPage", qr.Page));
            cmd.Parameters.Add(new SqlParameter("RecordPerPage", qr.PageSize));

            try
            {
                Connection.Open();

                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var tmp = new ScholarshipBo
                    {
                        Id = SqlHelper.Read<int>(reader, "Id"),
                        CompanyId = SqlHelper.Read<int>(reader, "CompanyId"),
                        ScholarshipName = SqlHelper.Read<string>(reader, "ScholarshipName"),
                        ShortName = SqlHelper.Read<string>(reader, "ShortName"),
                        CountryId = SqlHelper.Read<int>(reader, "CountryId"),
                        CountryName = SqlHelper.Read<string>(reader, "CountryName"),
                        SchoolId = SqlHelper.Read<int>(reader, "SchoolId"),
                        SchoolName = SqlHelper.Read<string>(reader, "SchoolName"),
                        Require = SqlHelper.Read<string>(reader, "Require"),
                        ScholarshipType = SqlHelper.Read<int>(reader, "ScholarshipType"),
                        Amount = SqlHelper.Read<string>(reader, "Amount"),
                        Quantity = SqlHelper.Read<int>(reader, "Quantity"),
                        TotalRegister = SqlHelper.Read<int>(reader, "TotalRegister"),
                        ExpiredDate = SqlHelper.Read<DateTime>(reader, "ExpiredDate"),
                        Note = SqlHelper.Read<string>(reader, "Note"),
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

        public override int Create(ScholarshipBo obj)
        {
            var id = 0;
            var cmd = Connection.CreateCommand() as SqlCommand;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "Scholarship_Insert";
            cmd.Parameters.AddWithValue("CompanyId", obj.CompanyId);
            cmd.Parameters.AddWithValue("ScholarshipName", obj.ScholarshipName);
            cmd.Parameters.AddWithValue("ShortName", obj.ShortName);
            cmd.Parameters.AddWithValue("CountryId", obj.CountryId);
            cmd.Parameters.AddWithValue("CountryName", obj.CountryName);
            cmd.Parameters.AddWithValue("SchoolId", obj.SchoolId);
            cmd.Parameters.AddWithValue("SchoolName", obj.SchoolName);
            cmd.Parameters.AddWithValue("Require", obj.Require);
            cmd.Parameters.AddWithValue("ScholarshipType", obj.ScholarshipType);
            cmd.Parameters.AddWithValue("Amount", obj.Amount);
            cmd.Parameters.AddWithValue("Quantity", obj.Quantity);
            cmd.Parameters.AddWithValue("TotalRegister", obj.TotalRegister);
            cmd.Parameters.AddWithValue("ExpiredDate", obj.ExpiredDate != DateTime.MinValue ? obj.ExpiredDate : null);
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

        public override ScholarshipBo Read(int id)
        {
            ScholarshipBo tmp = null;
            var cmd = Connection.CreateCommand() as SqlCommand;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "Scholarship_Select";
            cmd.Parameters.Add(new SqlParameter("ID", id));

            try
            {
                Connection.Open();

                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    tmp = new ScholarshipBo
                    {
                        Id = SqlHelper.Read<int>(reader, "Id"),
                        CompanyId = SqlHelper.Read<int>(reader, "CompanyId"),
                        ScholarshipName = SqlHelper.Read<string>(reader, "ScholarshipName"),
                        ShortName = SqlHelper.Read<string>(reader, "ShortName"),
                        CountryId = SqlHelper.Read<int>(reader, "CountryId"),
                        CountryName = SqlHelper.Read<string>(reader, "CountryName"),
                        SchoolId = SqlHelper.Read<int>(reader, "SchoolId"),
                        SchoolName = SqlHelper.Read<string>(reader, "SchoolName"),
                        Require = SqlHelper.Read<string>(reader, "Require"),
                        ScholarshipType = SqlHelper.Read<int>(reader, "ScholarshipType"),
                        Amount = SqlHelper.Read<string>(reader, "Amount"),
                        Quantity = SqlHelper.Read<int>(reader, "Quantity"),
                        TotalRegister = SqlHelper.Read<int>(reader, "TotalRegister"),
                        ExpiredDate = SqlHelper.Read<DateTime>(reader, "ExpiredDate"),
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

        public override bool Update(ScholarshipBo obj)
        {
            int num = 0;
            var cmd = Connection.CreateCommand() as SqlCommand;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "Scholarship_Update";
            cmd.Parameters.AddWithValue("Id", obj.Id);
            cmd.Parameters.AddWithValue("ScholarshipName", obj.ScholarshipName);
            cmd.Parameters.AddWithValue("ShortName", obj.ShortName);
            cmd.Parameters.AddWithValue("CountryId", obj.CountryId);
            cmd.Parameters.AddWithValue("CountryName", obj.CountryName);
            cmd.Parameters.AddWithValue("SchoolId", obj.SchoolId);
            cmd.Parameters.AddWithValue("SchoolName", obj.SchoolName);
            cmd.Parameters.AddWithValue("Require", obj.Require);
            cmd.Parameters.AddWithValue("ScholarshipType", obj.ScholarshipType);
            cmd.Parameters.AddWithValue("Amount", obj.Amount);
            cmd.Parameters.AddWithValue("Quantity", obj.Quantity);
            cmd.Parameters.AddWithValue("TotalRegister", obj.TotalRegister);
            cmd.Parameters.AddWithValue("ExpiredDate", obj.ExpiredDate != DateTime.MinValue ? obj.ExpiredDate : null);
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
            cmd.CommandText = "Scholarship_Delete";
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

        public bool UpdateRegister(int id, int updateUserId, string updateUserName)
        {
            int num = 0;
            var cmd = Connection.CreateCommand() as SqlCommand;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "Scholarship_UpdateRegister";
            cmd.Parameters.AddWithValue("Id", id);
            cmd.Parameters.AddWithValue("UpdateUserId", updateUserId);
            cmd.Parameters.AddWithValue("UpdateUserName", updateUserName);

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

        public List<ScholarshipBo> GetBySchool(int id)
        {
            var ret = new List<ScholarshipBo>();
            var cmd = Connection.CreateCommand() as SqlCommand;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "Scholarship_GetBySchool";
            cmd.Parameters.Add(new SqlParameter("SchoolId", id));

            try
            {
                Connection.Open();

                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var tmp = new ScholarshipBo
                    {
                        Id = SqlHelper.Read<int>(reader, "Id"),
                        CompanyId = SqlHelper.Read<int>(reader, "CompanyId"),
                        ScholarshipName = SqlHelper.Read<string>(reader, "ScholarshipName"),
                        ShortName = SqlHelper.Read<string>(reader, "ShortName"),
                        CountryId = SqlHelper.Read<int>(reader, "CountryId"),
                        CountryName = SqlHelper.Read<string>(reader, "CountryName"),
                        SchoolId = SqlHelper.Read<int>(reader, "SchoolId"),
                        SchoolName = SqlHelper.Read<string>(reader, "SchoolName"),
                        Require = SqlHelper.Read<string>(reader, "Require"),
                        ScholarshipType = SqlHelper.Read<int>(reader, "ScholarshipType"),
                        Amount = SqlHelper.Read<string>(reader, "Amount"),
                        Quantity = SqlHelper.Read<int>(reader, "Quantity"),
                        TotalRegister = SqlHelper.Read<int>(reader, "TotalRegister"),
                        ExpiredDate = SqlHelper.Read<DateTime>(reader, "ExpiredDate"),
                        Note = SqlHelper.Read<string>(reader, "Note"),
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

