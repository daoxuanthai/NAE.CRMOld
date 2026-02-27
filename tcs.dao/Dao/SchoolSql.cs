using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using tcs.bo;
using tcs.lib;

namespace tcs.dao
{
    public class SchoolSql : IDbConnect<SchoolBo>
    {
        private static SchoolSql _instance;

        public static SchoolSql Instance => _instance ?? (_instance = new SchoolSql(new SqlConnection(ConfigMgr.ConnectionString)));

        public SchoolSql(SqlConnection connection) : base(connection) { }

        public override List<SchoolBo> Select(IQuery query)
        {
            var qr = query as SchoolQuery;
            var ret = new List<SchoolBo>();
            var cmd = Connection.CreateCommand() as SqlCommand;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "School_Search";
            cmd.Parameters.Add(new SqlParameter("Keyword", qr.Keyword));
            cmd.Parameters.Add(new SqlParameter("CompanyId", qr.Company));
            cmd.Parameters.Add(new SqlParameter("CountryId", qr.Country));
            cmd.Parameters.Add(new SqlParameter("SchoolType", qr.SchoolType));
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
                    var tmp = new SchoolBo
                    {
                        Id = SqlHelper.Read<int>(reader, "Id"),
                        CompanyId = SqlHelper.Read<int>(reader, "CompanyId"),
                        SchoolName = SqlHelper.Read<string>(reader, "SchoolName"),
                        ShortName = SqlHelper.Read<string>(reader, "ShortName"),
                        Country = SqlHelper.Read<int>(reader, "Country"),
                        CountryName = SqlHelper.Read<string>(reader, "CountryName"),
                        City = SqlHelper.Read<string>(reader, "City"),
                        Association = SqlHelper.Read<string>(reader, "Association"),
                        Address = SqlHelper.Read<string>(reader, "Address"),
                        VnAddress = SqlHelper.Read<string>(reader, "VnAddress"),
                        ContactName = SqlHelper.Read<string>(reader, "ContactName"),
                        ContactEmail = SqlHelper.Read<string>(reader, "ContactEmail"),
                        ContactPhone = SqlHelper.Read<string>(reader, "ContactPhone"),
                        HotProgram = SqlHelper.Read<string>(reader, "HotProgram"),
                        EducationLevel = SqlHelper.Read<string>(reader, "EducationLevel"),
                        CouseOpenTime = SqlHelper.Read<string>(reader, "CouseOpenTime"),
                        SchoolType = SqlHelper.Read<int>(reader, "SchoolType"),
                        IsStrategy = SqlHelper.Read<bool>(reader, "IsStrategy"),
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

        public override int Create(SchoolBo obj)
        {
            var id = 0;
            var cmd = Connection.CreateCommand() as SqlCommand;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "School_Insert";
            cmd.Parameters.AddWithValue("CompanyId", obj.CompanyId);
            cmd.Parameters.AddWithValue("SchoolName", obj.SchoolName);
            cmd.Parameters.AddWithValue("ShortName", obj.ShortName);
            cmd.Parameters.AddWithValue("Country", obj.Country);
            cmd.Parameters.AddWithValue("CountryName", obj.CountryName);
            cmd.Parameters.AddWithValue("City", obj.City);
            cmd.Parameters.AddWithValue("Association", obj.Association);
            cmd.Parameters.AddWithValue("Address", obj.Address);
            cmd.Parameters.AddWithValue("VnAddress", obj.VnAddress);
            cmd.Parameters.AddWithValue("ContactName", obj.ContactName);
            cmd.Parameters.AddWithValue("ContactEmail", obj.ContactEmail);
            cmd.Parameters.AddWithValue("ContactPhone", obj.ContactPhone);
            cmd.Parameters.AddWithValue("HotProgram", obj.HotProgram);
            cmd.Parameters.AddWithValue("EducationLevel", obj.EducationLevel);
            cmd.Parameters.AddWithValue("CouseOpenTime", obj.CouseOpenTime);
            cmd.Parameters.AddWithValue("SchoolType", obj.SchoolType);
            cmd.Parameters.AddWithValue("IsStrategy", obj.IsStrategy);
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

        public override SchoolBo Read(int id)
        {
            SchoolBo tmp = null;
            var cmd = Connection.CreateCommand() as SqlCommand;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "School_Select";
            cmd.Parameters.Add(new SqlParameter("ID", id));

            try
            {
                Connection.Open();

                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    tmp = new SchoolBo
                    {
                        Id = SqlHelper.Read<int>(reader, "Id"),
                        CompanyId = SqlHelper.Read<int>(reader, "CompanyId"),
                        SchoolName = SqlHelper.Read<string>(reader, "SchoolName"),
                        ShortName = SqlHelper.Read<string>(reader, "ShortName"),
                        Country = SqlHelper.Read<int>(reader, "Country"),
                        CountryName = SqlHelper.Read<string>(reader, "CountryName"),
                        City = SqlHelper.Read<string>(reader, "City"),
                        Association = SqlHelper.Read<string>(reader, "Association"),
                        Address = SqlHelper.Read<string>(reader, "Address"),
                        VnAddress = SqlHelper.Read<string>(reader, "VnAddress"),
                        ContactName = SqlHelper.Read<string>(reader, "ContactName"),
                        ContactEmail = SqlHelper.Read<string>(reader, "ContactEmail"),
                        ContactPhone = SqlHelper.Read<string>(reader, "ContactPhone"),
                        HotProgram = SqlHelper.Read<string>(reader, "HotProgram"),
                        EducationLevel = SqlHelper.Read<string>(reader, "EducationLevel"),
                        CouseOpenTime = SqlHelper.Read<string>(reader, "CouseOpenTime"),
                        SchoolType = SqlHelper.Read<int>(reader, "SchoolType"),
                        IsStrategy = SqlHelper.Read<bool>(reader, "IsStrategy"),
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

        public override bool Update(SchoolBo obj)
        {
            int num = 0;
            var cmd = Connection.CreateCommand() as SqlCommand;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "School_Update";
            cmd.Parameters.AddWithValue("Id", obj.Id);
            cmd.Parameters.AddWithValue("SchoolName", obj.SchoolName);
            cmd.Parameters.AddWithValue("ShortName", obj.ShortName);
            cmd.Parameters.AddWithValue("Country", obj.Country);
            cmd.Parameters.AddWithValue("CountryName", obj.CountryName);
            cmd.Parameters.AddWithValue("City", obj.City);
            cmd.Parameters.AddWithValue("Association", obj.Association);
            cmd.Parameters.AddWithValue("Address", obj.Address);
            cmd.Parameters.AddWithValue("VnAddress", obj.VnAddress);
            cmd.Parameters.AddWithValue("ContactName", obj.ContactName);
            cmd.Parameters.AddWithValue("ContactEmail", obj.ContactEmail);
            cmd.Parameters.AddWithValue("ContactPhone", obj.ContactPhone);
            cmd.Parameters.AddWithValue("HotProgram", obj.HotProgram);
            cmd.Parameters.AddWithValue("EducationLevel", obj.EducationLevel);
            cmd.Parameters.AddWithValue("CouseOpenTime", obj.CouseOpenTime);
            cmd.Parameters.AddWithValue("SchoolType", obj.SchoolType);
            cmd.Parameters.AddWithValue("IsStrategy", obj.IsStrategy);
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
            cmd.CommandText = "School_Delete";
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

        public List<SchoolBo> GetAll(int companyId)
        {
            var ret = new List<SchoolBo>();
            var cmd = Connection.CreateCommand() as SqlCommand;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "School_GetAll";
            cmd.Parameters.Add(new SqlParameter("companyId", companyId));

            try
            {
                Connection.Open();

                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var tmp = new SchoolBo
                    {
                        Id = SqlHelper.Read<int>(reader, "Id"),
                        SchoolName = SqlHelper.Read<string>(reader, "SchoolName")
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

