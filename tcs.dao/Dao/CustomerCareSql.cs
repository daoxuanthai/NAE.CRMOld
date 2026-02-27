using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using tcs.bo;
using tcs.lib;

namespace tcs.dao
{
    public class CustomerCareSql : IDbConnect<CustomerCareBo>
    {
        private static CustomerCareSql _instance;

        public static CustomerCareSql Instance =>
            _instance ?? (_instance = new CustomerCareSql(new SqlConnection(ConfigMgr.ConnectionString)));
        public CustomerCareSql(SqlConnection connection) : base(connection) { }

        public override List<CustomerCareBo> Select(IQuery query)
        {
            var qr = query as CustomerCareQuery;
            var ret = new List<CustomerCareBo>();
            var cmd = Connection.CreateCommand() as SqlCommand;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "CustomerCare_Search";
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
                    var tmp = new CustomerCareBo
                    {
                        Id = SqlHelper.Read<int>(reader, "Id"),
                        CustomerId = SqlHelper.Read<int>(reader, "CustomerId"),
                        Advisory = SqlHelper.Read<string>(reader, "Advisory"),
                        IsAlarm = SqlHelper.Read<bool>(reader, "IsAlarm"),
                        AlarmTime = SqlHelper.Read<DateTime>(reader, "AlarmTime"),
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

        public override int Create(CustomerCareBo obj)
        {
            var id = 0;
            var cmd = Connection.CreateCommand() as SqlCommand;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "CustomerCare_Insert";
            cmd.Parameters.AddWithValue("CustomerId", obj.CustomerId);
            cmd.Parameters.AddWithValue("CompanyId", obj.CompanyId);
            cmd.Parameters.AddWithValue("Advisory", obj.Advisory);
            cmd.Parameters.AddWithValue("IsAlarm", obj.IsAlarm ? 1 : 0);
            cmd.Parameters.AddWithValue("AlarmTime", obj.AlarmTime);
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

        public override CustomerCareBo Read(int id)
        {
            CustomerCareBo tmp = null;
            var cmd = Connection.CreateCommand() as SqlCommand;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "CustomerCare_Select";
            cmd.Parameters.Add(new SqlParameter("ID", id));

            try
            {
                Connection.Open();

                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    tmp = new CustomerCareBo
                    {
                        Id = SqlHelper.Read<int>(reader, "Id"),
                        CustomerId = SqlHelper.Read<int>(reader, "CustomerId"),
                        Advisory = SqlHelper.Read<string>(reader, "Advisory"),
                        IsAlarm = SqlHelper.Read<bool>(reader, "IsAlarm"),
                        AlarmTime = SqlHelper.Read<DateTime>(reader, "AlarmTime"),
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

        public override bool Update(CustomerCareBo obj)
        {
            int num = 0;
            var cmd = Connection.CreateCommand() as SqlCommand;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "CustomerCare_Update";
            cmd.Parameters.AddWithValue("Id", obj.Id);
            cmd.Parameters.AddWithValue("Advisory", obj.Advisory);
            cmd.Parameters.AddWithValue("IsAlarm", obj.IsAlarm ? 1 : 0);
            cmd.Parameters.AddWithValue("AlarmTime", obj.AlarmTime);
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
            cmd.CommandText = "CustomerCare_Delete";
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

        public List<CustomerCareBo> GetByCustomer(int customerId)
        {
            var ret = new List<CustomerCareBo>();
            var cmd = Connection.CreateCommand() as SqlCommand;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "CustomerCare_GetByCustomer";
            cmd.Parameters.Add(new SqlParameter("CustomerID", customerId));

            try
            {
                Connection.Open();

                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var tmp = new CustomerCareBo
                    {
                        Id = SqlHelper.Read<int>(reader, "Id"),
                        CustomerId = SqlHelper.Read<int>(reader, "CustomerId"),
                        Advisory = SqlHelper.Read<string>(reader, "Advisory"),
                        IsAlarm = SqlHelper.Read<bool>(reader, "IsAlarm"),
                        AlarmTime = SqlHelper.Read<DateTime>(reader, "AlarmTime"),
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

