using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Remoting.Messaging;
using tcs.bo;
using tcs.lib;

namespace tcs.dao
{
    public class CompanyTitleSql : IDbConnect<CompanyTitleBo>
    {
        private static CompanyTitleSql _instance;

        public static CompanyTitleSql Instance => _instance ??
                                                  (_instance =
                                                      new CompanyTitleSql(
                                                          new SqlConnection(ConfigMgr.AccountConnectionString)));
        public CompanyTitleSql(SqlConnection connection) : base(connection) { }

        public override List<CompanyTitleBo> Select(IQuery query)
        {
            var qr = query as CompanyTitleQuery;
            var ret = new List<CompanyTitleBo>();
            var cmd = Connection.CreateCommand() as SqlCommand;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "CompanyTitle_Search";
            cmd.Parameters.Add(new SqlParameter("Keyword", qr.Keyword));
            cmd.Parameters.Add(new SqlParameter("CompanyId", qr.Company));
            cmd.Parameters.Add(new SqlParameter("OfficeId", qr.Office));
            cmd.Parameters.Add(new SqlParameter("IsLock", qr.IsLock));
            cmd.Parameters.Add(new SqlParameter("CurrentPage", qr.Page));
            cmd.Parameters.Add(new SqlParameter("RecordPerPage", qr.PageSize));

            try
            {
                Connection.Open();

                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var tmp = new CompanyTitleBo
                    {
                        Id = SqlHelper.Read<int>(reader, "Id"),
                        UserName = SqlHelper.Read<string>(reader, "UserName"),
                        UserFullName = SqlHelper.Read<string>(reader, "UserFullName"),
                        CompanyId = SqlHelper.Read<int>(reader, "CompanyId"),
                        OfficeId = SqlHelper.Read<int>(reader, "OfficeId"),
                        Title = SqlHelper.Read<string>(reader, "Title"),
                        Code = SqlHelper.Read<string>(reader, "Code"),
                        Note = SqlHelper.Read<string>(reader, "Note"),
                        UserType = SqlHelper.Read<int>(reader, "UserType"),
                        IsLock = SqlHelper.Read<bool>(reader, "IsLock"),
                        IsViewAll = SqlHelper.Read<bool>(reader, "IsViewAll"),
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

        public override int Create(CompanyTitleBo obj)
        {
            var id = 0;
            var cmd = Connection.CreateCommand() as SqlCommand;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "CompanyTitle_Insert";
            cmd.Parameters.AddWithValue("UserId", obj.UserId);
            cmd.Parameters.AddWithValue("UserName", obj.UserName);
            cmd.Parameters.AddWithValue("UserFullName", obj.UserFullName);
            cmd.Parameters.AddWithValue("CompanyId", obj.CompanyId);
            cmd.Parameters.AddWithValue("OfficeId", obj.OfficeId);
            cmd.Parameters.AddWithValue("Title", obj.Title);
            cmd.Parameters.AddWithValue("Code", obj.Code);
            cmd.Parameters.AddWithValue("Note", obj.Note);
            cmd.Parameters.AddWithValue("UserType", obj.UserType);
            cmd.Parameters.AddWithValue("IsLock", obj.IsLock);
            cmd.Parameters.AddWithValue("IsViewAll", obj.IsViewAll);
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

        public override CompanyTitleBo Read(int id)
        {
            CompanyTitleBo tmp = null;
            var cmd = Connection.CreateCommand() as SqlCommand;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "CompanyTitle_Select";
            cmd.Parameters.Add(new SqlParameter("ID", id));

            try
            {
                Connection.Open();

                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    tmp = new CompanyTitleBo
                    {
                        Id = SqlHelper.Read<int>(reader, "Id"),
                        UserId = SqlHelper.Read<int>(reader, "UserId"),
                        UserName = SqlHelper.Read<string>(reader, "UserName"),
                        UserFullName = SqlHelper.Read<string>(reader, "UserFullName"),
                        CompanyId = SqlHelper.Read<int>(reader, "CompanyId"),
                        OfficeId = SqlHelper.Read<int>(reader, "OfficeId"),
                        Title = SqlHelper.Read<string>(reader, "Title"),
                        Code = SqlHelper.Read<string>(reader, "Code"),
                        Note = SqlHelper.Read<string>(reader, "Note"),
                        UserType = SqlHelper.Read<int>(reader, "UserType"),
                        IsLock = SqlHelper.Read<bool>(reader, "IsLock"),
                        IsViewAll = SqlHelper.Read<bool>(reader, "IsViewAll"),
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

        public override bool Update(CompanyTitleBo obj)
        {
            int num = 0;
            var cmd = Connection.CreateCommand() as SqlCommand;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "CompanyTitle_Update";
            cmd.Parameters.AddWithValue("Id", obj.Id);
            cmd.Parameters.AddWithValue("UserId", obj.UserId);
            cmd.Parameters.AddWithValue("UserName", obj.UserName);
            cmd.Parameters.AddWithValue("UserFullName", obj.UserFullName);
            cmd.Parameters.AddWithValue("OfficeId", obj.OfficeId);
            cmd.Parameters.AddWithValue("Title", obj.Title);
            cmd.Parameters.AddWithValue("Code", obj.Code);
            cmd.Parameters.AddWithValue("Note", obj.Note);
            cmd.Parameters.AddWithValue("IsLock", obj.IsLock);
            cmd.Parameters.AddWithValue("IsViewAll", obj.IsViewAll);
            cmd.Parameters.AddWithValue("UserType", obj.UserType);
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
            cmd.CommandText = "CompanyTitle_Delete";
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

        public List<CompanyTitleBo> GetByCompany(int companyId)
        {
            var ret = new List<CompanyTitleBo>();
            var cmd = Connection.CreateCommand() as SqlCommand;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "CompanyTitle_GetByCompany";
            cmd.Parameters.Add(new SqlParameter("CompanyID", companyId));

            try
            {
                Connection.Open();

                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var tmp = new CompanyTitleBo
                    {
                        Id = SqlHelper.Read<int>(reader, "Id"),
                        UserId = SqlHelper.Read<int>(reader, "UserId"),
                        UserName = SqlHelper.Read<string>(reader, "UserName"),
                        UserFullName = SqlHelper.Read<string>(reader, "UserFullName"),
                        CompanyId = SqlHelper.Read<int>(reader, "CompanyId"),
                        OfficeId = SqlHelper.Read<int>(reader, "OfficeId"),
                        Title = SqlHelper.Read<string>(reader, "Title"),
                        Code = SqlHelper.Read<string>(reader, "Code"),
                        Note = SqlHelper.Read<string>(reader, "Note"),
                        UserType = SqlHelper.Read<int>(reader, "UserType"),
                        IsLock = SqlHelper.Read<bool>(reader, "IsLock"),
                        IsViewAll = SqlHelper.Read<bool>(reader, "IsViewAll"),
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

        public List<CompanyTitleBo> GetByUser(int userId)
        {
            var ret = new List<CompanyTitleBo>();
            var cmd = Connection.CreateCommand() as SqlCommand;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "CompanyTitle_GetByUser";
            cmd.Parameters.Add(new SqlParameter("UserId", userId));

            try
            {
                Connection.Open();

                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var tmp = new CompanyTitleBo
                    {
                        Id = SqlHelper.Read<int>(reader, "Id"),
                        UserId = SqlHelper.Read<int>(reader, "UserId"),
                        UserName = SqlHelper.Read<string>(reader, "UserName"),
                        UserFullName = SqlHelper.Read<string>(reader, "UserFullName"),
                        CompanyId = SqlHelper.Read<int>(reader, "CompanyId"),
                        OfficeId = SqlHelper.Read<int>(reader, "OfficeId"),
                        Title = SqlHelper.Read<string>(reader, "Title"),
                        Code = SqlHelper.Read<string>(reader, "Code"),
                        Note = SqlHelper.Read<string>(reader, "Note"),
                        UserType = SqlHelper.Read<int>(reader, "UserType"),
                        IsLock = SqlHelper.Read<bool>(reader, "IsLock"),
                        IsViewAll = SqlHelper.Read<bool>(reader, "IsViewAll"),
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

