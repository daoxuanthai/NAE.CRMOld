using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using tcs.bo;
using tcs.lib;

namespace tcs.dao
{
    public class CompanySql : IDbConnect<CompanyBo>
    {
        private static CompanySql _instance;

        public static CompanySql Instance =>
            _instance ?? (_instance = new CompanySql(new SqlConnection(ConfigMgr.AccountConnectionString)));
        public CompanySql(SqlConnection connection) : base(connection) { }

        public override List<CompanyBo> Select(IQuery query)
        {
            var qr = query as CompanyQuery;
            var ret = new List<CompanyBo>();
            var cmd = Connection.CreateCommand() as SqlCommand;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "Company_Search";
            cmd.Parameters.Add(new SqlParameter("Keyword", qr.Keyword));
            cmd.Parameters.Add(new SqlParameter("Status", qr.Status));
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
                    var tmp = new CompanyBo
                    {
                        Id = SqlHelper.Read<int>(reader, "Id"),
                        CompanyName = SqlHelper.Read<string>(reader, "CompanyName"),
                        FullName = SqlHelper.Read<string>(reader, "FullName"),
                        Address = SqlHelper.Read<string>(reader, "Address"),
                        ContactName = SqlHelper.Read<string>(reader, "ContactName"),
                        ContactPhone = SqlHelper.Read<string>(reader, "ContactPhone"),
                        ContactEmail = SqlHelper.Read<string>(reader, "ContactEmail"),
                        CompanyType = SqlHelper.Read<int>(reader, "CompanyType"),
                        Status = SqlHelper.Read<int>(reader, "Status"),
                        IsLock = SqlHelper.Read<bool>(reader, "IsLock"),

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

        public override int Create(CompanyBo obj)
        {
            var id = 0;
            var cmd = Connection.CreateCommand() as SqlCommand;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "Company_Insert";
            cmd.Parameters.AddWithValue("CompanyName", obj.CompanyName);
            cmd.Parameters.AddWithValue("FullName", obj.FullName);
            cmd.Parameters.AddWithValue("Address", obj.Address);
            cmd.Parameters.AddWithValue("ContactName", obj.ContactName);
            cmd.Parameters.AddWithValue("ContactPhone", obj.ContactPhone);
            cmd.Parameters.AddWithValue("ContactEmail", obj.ContactEmail);
            cmd.Parameters.AddWithValue("CompanyType", obj.CompanyType);
            cmd.Parameters.AddWithValue("Status", obj.Status);
            cmd.Parameters.AddWithValue("IsLock", obj.IsLock);

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

        public override CompanyBo Read(int id)
        {
            CompanyBo tmp = null;
            var cmd = Connection.CreateCommand() as SqlCommand;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "Company_Select";
            cmd.Parameters.Add(new SqlParameter("ID", id));

            try
            {
                Connection.Open();

                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    tmp = new CompanyBo
                    {
                        Id = SqlHelper.Read<int>(reader, "Id"),
                        CompanyName = SqlHelper.Read<string>(reader, "CompanyName"),
                        FullName = SqlHelper.Read<string>(reader, "FullName"),
                        Address = SqlHelper.Read<string>(reader, "Address"),
                        ContactName = SqlHelper.Read<string>(reader, "ContactName"),
                        ContactPhone = SqlHelper.Read<string>(reader, "ContactPhone"),
                        ContactEmail = SqlHelper.Read<string>(reader, "ContactEmail"),
                        CompanyType = SqlHelper.Read<int>(reader, "CompanyType"),
                        Status = SqlHelper.Read<int>(reader, "Status"),
                        IsLock = SqlHelper.Read<bool>(reader, "IsLock"),
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

        public override bool Update(CompanyBo obj)
        {
            int num = 0;
            var cmd = Connection.CreateCommand() as SqlCommand;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "Company_Update";
            cmd.Parameters.AddWithValue("Id", obj.Id);
            cmd.Parameters.AddWithValue("CompanyName", obj.CompanyName);
            cmd.Parameters.AddWithValue("FullName", obj.FullName);
            cmd.Parameters.AddWithValue("Address", obj.Address);
            cmd.Parameters.AddWithValue("ContactName", obj.ContactName);
            cmd.Parameters.AddWithValue("ContactPhone", obj.ContactPhone);
            cmd.Parameters.AddWithValue("ContactEmail", obj.ContactEmail);
            cmd.Parameters.AddWithValue("CompanyType", obj.CompanyType);
            cmd.Parameters.AddWithValue("Status", obj.Status);
            cmd.Parameters.AddWithValue("IsLock", obj.IsLock);
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
            cmd.CommandText = "Company_Delete";
            cmd.Parameters.AddWithValue("Id", ids);
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

        public List<CompanyBo> GetByUser(int userId)
        {
            var ret = new List<CompanyBo>();
            var cmd = Connection.CreateCommand() as SqlCommand;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "Company_GetByUser";
            cmd.Parameters.Add(new SqlParameter("UserId", userId));

            try
            {
                Connection.Open();

                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var tmp = new CompanyBo
                    {
                        Id = SqlHelper.Read<int>(reader, "Id"),
                        CompanyName = SqlHelper.Read<string>(reader, "CompanyName"),
                        FullName = SqlHelper.Read<string>(reader, "FullName"),
                        Address = SqlHelper.Read<string>(reader, "Address"),
                        ContactName = SqlHelper.Read<string>(reader, "ContactName"),
                        ContactPhone = SqlHelper.Read<string>(reader, "ContactPhone"),
                        ContactEmail = SqlHelper.Read<string>(reader, "ContactEmail"),
                        CompanyType = SqlHelper.Read<int>(reader, "CompanyType"),
                        Status = SqlHelper.Read<int>(reader, "Status"),
                        IsLock = SqlHelper.Read<bool>(reader, "IsLock"),
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

