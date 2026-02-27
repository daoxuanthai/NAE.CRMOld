using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using tcs.bo;
using tcs.lib;

namespace tcs.dao
{
    public class ParentSql : IDbConnect<ParentBo>
    {
        private static ParentSql _instance;

        public static ParentSql Instance => _instance ?? (_instance = new ParentSql(
                                                new SqlConnection(ConfigMgr.ConnectionString)));
        public ParentSql(SqlConnection connection) : base(connection) { }

        public override List<ParentBo> Select(IQuery query)
        {
            var qr = query as ParentQuery;
            var ret = new List<ParentBo>();
            var cmd = Connection.CreateCommand() as SqlCommand;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "Parent_Search";
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
                    var tmp = new ParentBo
                    {
                        Id = SqlHelper.Read<int>(reader, "Id"),
                        CustomerId = SqlHelper.Read<int>(reader, "CustomerId"),
                        Name = SqlHelper.Read<string>(reader, "Name"),
                        Birthday = SqlHelper.Read<DateTime>(reader, "Birthday"),
                        Gender = SqlHelper.Read<int>(reader, "Gender"),
                        Email = SqlHelper.Read<string>(reader, "Email"),
                        Phone = SqlHelper.Read<string>(reader, "Phone"),
                        Note = SqlHelper.Read<string>(reader, "Note"),
                        Desire = SqlHelper.Read<int>(reader, "Desire"),
                        JobName = SqlHelper.Read<string>(reader, "JobName"),
                        PositionName = SqlHelper.Read<string>(reader, "PositionName"),
                        CompanyName = SqlHelper.Read<string>(reader, "CompanyName"),
                        Income = SqlHelper.Read<string>(reader, "Income"),
                        OtherIncome = SqlHelper.Read<string>(reader, "OtherIncome"),
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

        public override int Create(ParentBo obj)
        {
            var id = 0;
            var cmd = Connection.CreateCommand() as SqlCommand;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "Parent_Insert";
            cmd.Parameters.AddWithValue("CustomerId", obj.CustomerId);
            cmd.Parameters.AddWithValue("CompanyId", obj.CompanyId);
            cmd.Parameters.AddWithValue("Name", obj.Name);
            cmd.Parameters.AddWithValue("Birthday", obj.Birthday);
            cmd.Parameters.AddWithValue("Gender", obj.Gender);
            cmd.Parameters.AddWithValue("Email", obj.Email);
            cmd.Parameters.AddWithValue("Phone", obj.Phone);
            cmd.Parameters.AddWithValue("Note", obj.Note);
            cmd.Parameters.AddWithValue("Desire", obj.Desire);
            cmd.Parameters.AddWithValue("JobName", obj.JobName);
            cmd.Parameters.AddWithValue("PositionName", obj.PositionName);
            cmd.Parameters.AddWithValue("CompanyName", obj.CompanyName);
            cmd.Parameters.AddWithValue("Income", obj.Income);
            cmd.Parameters.AddWithValue("OtherIncome", obj.OtherIncome);
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

        public override ParentBo Read(int id)
        {
            ParentBo tmp = null;
            var cmd = Connection.CreateCommand() as SqlCommand;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "Parent_Select";
            cmd.Parameters.Add(new SqlParameter("ID", id));

            try
            {
                Connection.Open();

                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    tmp = new ParentBo
                    {
                        Id = SqlHelper.Read<int>(reader, "Id"),
                        CustomerId = SqlHelper.Read<int>(reader, "CustomerId"),
                        Name = SqlHelper.Read<string>(reader, "Name"),
                        Birthday = SqlHelper.Read<DateTime>(reader, "Birthday"),
                        Gender = SqlHelper.Read<int>(reader, "Gender"),
                        Email = SqlHelper.Read<string>(reader, "Email"),
                        Phone = SqlHelper.Read<string>(reader, "Phone"),
                        Note = SqlHelper.Read<string>(reader, "Note"),
                        Desire = SqlHelper.Read<int>(reader, "Desire"),
                        JobName = SqlHelper.Read<string>(reader, "JobName"),
                        PositionName = SqlHelper.Read<string>(reader, "PositionName"),
                        CompanyName = SqlHelper.Read<string>(reader, "CompanyName"),
                        Income = SqlHelper.Read<string>(reader, "Income"),
                        OtherIncome = SqlHelper.Read<string>(reader, "OtherIncome"),
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

        public override bool Update(ParentBo obj)
        {
            int num = 0;
            var cmd = Connection.CreateCommand() as SqlCommand;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "Parent_Update";
            cmd.Parameters.AddWithValue("Id", obj.Id);
            cmd.Parameters.AddWithValue("Name", obj.Name);
            cmd.Parameters.AddWithValue("Birthday", obj.Birthday != DateTime.MinValue ? obj.Birthday : null);
            cmd.Parameters.AddWithValue("Gender", obj.Gender);
            cmd.Parameters.AddWithValue("Email", obj.Email);
            cmd.Parameters.AddWithValue("Phone", obj.Phone);
            cmd.Parameters.AddWithValue("Note", obj.Note);
            cmd.Parameters.AddWithValue("Desire", obj.Desire);
            cmd.Parameters.AddWithValue("JobName", obj.JobName);
            cmd.Parameters.AddWithValue("PositionName", obj.PositionName);
            cmd.Parameters.AddWithValue("CompanyName", obj.CompanyName);
            cmd.Parameters.AddWithValue("Income", obj.Income);
            cmd.Parameters.AddWithValue("OtherIncome", obj.OtherIncome);
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
            cmd.CommandText = "Parent_Delete";
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

        public List<ParentBo> GetByCustomer(int customerId)
        {
            var ret = new List<ParentBo>();
            var cmd = Connection.CreateCommand() as SqlCommand;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "Parent_GetByCustomer";
            cmd.Parameters.Add(new SqlParameter("CustomerId", customerId));

            try
            {
                Connection.Open();

                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var tmp = new ParentBo
                    {
                        Id = SqlHelper.Read<int>(reader, "Id"),
                        CustomerId = SqlHelper.Read<int>(reader, "CustomerId"),
                        Name = SqlHelper.Read<string>(reader, "Name"),
                        Birthday = SqlHelper.Read<DateTime>(reader, "Birthday"),
                        Gender = SqlHelper.Read<int>(reader, "Gender"),
                        Email = SqlHelper.Read<string>(reader, "Email"),
                        Phone = SqlHelper.Read<string>(reader, "Phone"),
                        Note = SqlHelper.Read<string>(reader, "Note"),
                        Desire = SqlHelper.Read<int>(reader, "Desire"),
                        JobName = SqlHelper.Read<string>(reader, "JobName"),
                        PositionName = SqlHelper.Read<string>(reader, "PositionName"),
                        CompanyName = SqlHelper.Read<string>(reader, "CompanyName"),
                        Income = SqlHelper.Read<string>(reader, "Income"),
                        OtherIncome = SqlHelper.Read<string>(reader, "OtherIncome"),
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