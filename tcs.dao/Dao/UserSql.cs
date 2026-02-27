
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using tcs.bo;
using tcs.lib;

namespace tcs.dao
{
    public class UserSql : IDbConnect<UserBo>
    {
        private static UserSql _instance;

        public static UserSql Instance => _instance ?? (_instance =
                                              new UserSql(new SqlConnection(ConfigMgr.AccountConnectionString)));
        public UserSql(SqlConnection connection) : base(connection) { }

        public override List<UserBo> Select(IQuery query)
        {
            var qr = query as UserQuery;
            var ret = new List<UserBo>();
            var cmd = Connection.CreateCommand() as SqlCommand;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "User_Search";
            cmd.Parameters.Add(new SqlParameter("Keyword", qr.Keyword));
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
                    var tmp = new UserBo
                    {
                        Id = SqlHelper.Read<int>(reader, "Id"),
                        UserName = SqlHelper.Read<string>(reader, "UserName"),
                        Avatar = SqlHelper.Read<string>(reader, "Avatar"),
                        FullName = SqlHelper.Read<string>(reader, "FullName"),
                        Address = SqlHelper.Read<string>(reader, "Address"),
                        Email = SqlHelper.Read<string>(reader, "Email"),
                        Phone = SqlHelper.Read<string>(reader, "Phone"),
                        Note = SqlHelper.Read<string>(reader, "Note"),
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

        public override int Create(UserBo obj)
        {
            throw new NotImplementedException();
        }

        public override UserBo Read(int id)
        {
            UserBo tmp = null;
            var cmd = Connection.CreateCommand() as SqlCommand;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "User_Select";
            cmd.Parameters.Add(new SqlParameter("ID", id));

            try
            {
                Connection.Open();

                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    tmp = new UserBo
                    {
                        Id = SqlHelper.Read<int>(reader, "Id"),
                        UserName = SqlHelper.Read<string>(reader, "UserName"),
                        Avatar = SqlHelper.Read<string>(reader, "Avatar"),
                        FullName = SqlHelper.Read<string>(reader, "FullName"),
                        Address = SqlHelper.Read<string>(reader, "Address"),
                        Email = SqlHelper.Read<string>(reader, "Email"),
                        Phone = SqlHelper.Read<string>(reader, "Phone"),
                        Note = SqlHelper.Read<string>(reader, "Note"),
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

        public override bool Update(UserBo obj)
        {
            int num = 0;
            var cmd = Connection.CreateCommand() as SqlCommand;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "User_Update";
            cmd.Parameters.AddWithValue("Id", obj.Id);
            cmd.Parameters.AddWithValue("Avatar", obj.Avatar);
            cmd.Parameters.AddWithValue("FullName", obj.FullName);
            cmd.Parameters.AddWithValue("Address", obj.Address);
            cmd.Parameters.AddWithValue("Email", obj.Email);
            cmd.Parameters.AddWithValue("Phone", obj.Phone);
            cmd.Parameters.AddWithValue("Note", obj.Note);
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
            cmd.CommandText = "User_Delete";
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

        public UserBo GetByUserName(string userName)
        {
            UserBo tmp = null;
            var cmd = Connection.CreateCommand() as SqlCommand;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "User_GetByUserName";
            cmd.Parameters.Add(new SqlParameter("UserName", userName));

            try
            {
                Connection.Open();

                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    tmp = new UserBo
                    {
                        Id = SqlHelper.Read<int>(reader, "Id"),
                        UserName = SqlHelper.Read<string>(reader, "UserName"),
                        Avatar = SqlHelper.Read<string>(reader, "Avatar"),
                        FullName = SqlHelper.Read<string>(reader, "FullName"),
                        Address = SqlHelper.Read<string>(reader, "Address"),
                        Email = SqlHelper.Read<string>(reader, "Email"),
                        Phone = SqlHelper.Read<string>(reader, "Phone"),
                        Note = SqlHelper.Read<string>(reader, "Note"),
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
    }
}
