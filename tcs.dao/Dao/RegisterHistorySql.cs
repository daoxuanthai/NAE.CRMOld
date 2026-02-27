using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using tcs.bo;
using tcs.lib;

namespace tcs.dao
{
    public class RegisterHistorySql : IDbConnect<RegisterHistoryBo>
    {
        private static RegisterHistorySql _instance;

        public static RegisterHistorySql Instance => _instance ?? (_instance = new RegisterHistorySql(
                                                new SqlConnection(ConfigMgr.ConnectionString)));
        public RegisterHistorySql(SqlConnection connection) : base(connection) { }

        public override List<RegisterHistoryBo> Select(IQuery query)
        {
            var qr = query as RegisterHistoryQuery;
            var ret = new List<RegisterHistoryBo>();
            var cmd = Connection.CreateCommand() as SqlCommand;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "RegisterHistory_Search";
            cmd.Parameters.Add(new SqlParameter("Keyword", qr.Keyword));
            cmd.Parameters.Add(new SqlParameter("FromDate", qr.From));
            cmd.Parameters.Add(new SqlParameter("CompanyId", qr.CompanyIds.ToCommaList()));
            cmd.Parameters.Add(new SqlParameter("CustomerId", qr.CustomerIds.ToCommaList()));
            cmd.Parameters.Add(new SqlParameter("ToDate", qr.To));
            cmd.Parameters.Add(new SqlParameter("CurrentPage", qr.Page));
            cmd.Parameters.Add(new SqlParameter("RecordPerPage", qr.PageSize));

            try
            {
                Connection.Open();

                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var tmp = new RegisterHistoryBo
                    {
                        Id = SqlHelper.Read<int>(reader, "Id"),
                        CustomerId = SqlHelper.Read<int>(reader, "CustomerId"),
                        CompanyId = SqlHelper.Read<int>(reader, "CompanyId"),
                        FullName = SqlHelper.Read<string>(reader, "FullName"),
                        Phone = SqlHelper.Read<string>(reader, "Phone"),
                        Email = SqlHelper.Read<string>(reader, "Email"),
                        RegisterLink = SqlHelper.Read<string>(reader, "RegisterLink"),
                        RegisterInfo = SqlHelper.Read<string>(reader, "RegisterInfo"),
                        AdvisoryContent = SqlHelper.Read<string>(reader, "AdvisoryContent"),
                        IsParent = SqlHelper.Read<bool>(reader, "IsParent"),
                        CreateUserId = SqlHelper.Read<int>(reader, "CreateUserId"),
                        CreateUserName = SqlHelper.Read<string>(reader, "CreateUserName"),
                        UpdateUserId = SqlHelper.Read<int>(reader, "UpdateUserId"),
                        UpdateUserName = SqlHelper.Read<string>(reader, "UpdateUserName"),
                        CreateDate = SqlHelper.Read<DateTime>(reader, "CreateDate"),
                        UpdateDate = SqlHelper.Read<DateTime>(reader, "UpdateDate"),
                        IsCallInfo = SqlHelper.Read<bool>(reader, "IsCallInfo"),
                        IsContact = SqlHelper.Read<bool>(reader, "IsContact")
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
        public override int Create(RegisterHistoryBo obj)
        {
            var id = 0;
            var cmd = Connection.CreateCommand() as SqlCommand;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "RegisterHistory_Insert";
            cmd.Parameters.AddWithValue("CompanyId", obj.CompanyId);
            cmd.Parameters.AddWithValue("CustomerId", obj.CustomerId);
            cmd.Parameters.AddWithValue("FullName", obj.FullName);
            cmd.Parameters.AddWithValue("Phone", obj.Phone);
            cmd.Parameters.AddWithValue("Email", obj.Email);
            cmd.Parameters.AddWithValue("RegisterLink", obj.RegisterLink);
            cmd.Parameters.AddWithValue("RegisterInfo", obj.RegisterInfo);
            cmd.Parameters.AddWithValue("IsParent", obj.IsParent);
            cmd.Parameters.AddWithValue("CreateUserId", obj.CreateUserId);
            cmd.Parameters.AddWithValue("CreateUserName", obj.CreateUserName);
            cmd.Parameters.AddWithValue("IsCallInfo", obj.IsCallInfo);
            cmd.Parameters.AddWithValue("IsContact", obj.IsContact);

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

        public override RegisterHistoryBo Read(int id)
        {
            RegisterHistoryBo tmp = null;
            var cmd = Connection.CreateCommand() as SqlCommand;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "RegisterHistory_Select";
            cmd.Parameters.Add(new SqlParameter("ID", id));

            try
            {
                Connection.Open();

                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    tmp = new RegisterHistoryBo
                    {
                        Id = SqlHelper.Read<int>(reader, "Id"),
                        CustomerId = SqlHelper.Read<int>(reader, "CustomerId"),
                        CompanyId = SqlHelper.Read<int>(reader, "CompanyId"),
                        FullName = SqlHelper.Read<string>(reader, "FullName"),
                        Phone = SqlHelper.Read<string>(reader, "Phone"),
                        Email = SqlHelper.Read<string>(reader, "Email"),
                        RegisterLink = SqlHelper.Read<string>(reader, "RegisterLink"),
                        RegisterInfo = SqlHelper.Read<string>(reader, "RegisterInfo"),
                        AdvisoryContent = SqlHelper.Read<string>(reader, "AdvisoryContent"),
                        IsParent = SqlHelper.Read<bool>(reader, "IsParent"),
                        CreateUserId = SqlHelper.Read<int>(reader, "CreateUserId"),
                        CreateUserName = SqlHelper.Read<string>(reader, "CreateUserName"),
                        UpdateUserId = SqlHelper.Read<int>(reader, "UpdateUserId"),
                        UpdateUserName = SqlHelper.Read<string>(reader, "UpdateUserName"),
                        CreateDate = SqlHelper.Read<DateTime>(reader, "CreateDate"),
                        UpdateDate = SqlHelper.Read<DateTime>(reader, "UpdateDate"),
                        IsCallInfo = SqlHelper.Read<bool>(reader, "IsCallInfo"),
                        IsContact = SqlHelper.Read<bool>(reader, "IsContact")
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

        public override bool Delete(string ids, int companyId, int userId, string userName)
        {
            int num = 0;
            var cmd = Connection.CreateCommand() as SqlCommand;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "RegisterHistory_Delete";
            cmd.Parameters.AddWithValue("Id", ids);

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

        public override bool Update(RegisterHistoryBo obj)
        {
            int num = 0;
            var cmd = Connection.CreateCommand() as SqlCommand;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "RegisterHistory_Update";
            cmd.Parameters.AddWithValue("ID", obj.Id);
            cmd.Parameters.AddWithValue("CompanyId", obj.CompanyId);
            cmd.Parameters.AddWithValue("CustomerId", obj.CustomerId);
            cmd.Parameters.AddWithValue("FullName", obj.FullName);
            cmd.Parameters.AddWithValue("Phone", obj.Phone);
            cmd.Parameters.AddWithValue("Email", obj.Email);
            cmd.Parameters.AddWithValue("RegisterLink", obj.RegisterLink);
            cmd.Parameters.AddWithValue("RegisterInfo", obj.RegisterInfo);
            cmd.Parameters.AddWithValue("AdvisoryContent", obj.AdvisoryContent);
            cmd.Parameters.AddWithValue("IsParent", obj.IsParent);
            cmd.Parameters.AddWithValue("UpdateUserId", obj.UpdateUserId);
            cmd.Parameters.AddWithValue("UpdateUserName", obj.UpdateUserName);
            cmd.Parameters.AddWithValue("IsCallInfo", obj.IsCallInfo);
            cmd.Parameters.AddWithValue("IsContact", obj.IsContact);

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