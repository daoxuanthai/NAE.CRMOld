using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using tcs.bo;
using tcs.lib;

namespace tcs.dao
{
    public class CommissionSql : IDbConnect<CommissionBo>
    {
        private static CommissionSql _instance;

        public static CommissionSql Instance =>
            _instance ?? (_instance = new CommissionSql(new SqlConnection(ConfigMgr.ConnectionString)));

        public CommissionSql(SqlConnection connection) : base(connection) { }

        public override List<CommissionBo> Select(IQuery query)
        {
            var qr = query as CommissionQuery;
            var ret = new List<CommissionBo>();
            var cmd = Connection.CreateCommand() as SqlCommand;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "Commission_Search";
            cmd.Parameters.Add(new SqlParameter("Keyword", qr.Keyword));
            cmd.Parameters.Add(new SqlParameter("CompanyId", qr.Company));
            cmd.Parameters.Add(new SqlParameter("OfficeId", qr.Office));
            cmd.Parameters.Add(new SqlParameter("ContractId", qr.ContractId));
            cmd.Parameters.Add(new SqlParameter("CustomerId", qr.CustomerId));
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
                    var tmp = new CommissionBo
                    {
                        Id = SqlHelper.Read<int>(reader, "Id"),
                        CustomerId = SqlHelper.Read<int>(reader, "CustomerId"),
                        ContractId = SqlHelper.Read<int>(reader, "ContractId"),
                        CompanyId = SqlHelper.Read<int>(reader, "CompanyId"),
                        OfficeId = SqlHelper.Read<int>(reader, "OfficeId"),
                        Name = SqlHelper.Read<string>(reader, "Name"),
                        Commission = SqlHelper.Read<decimal>(reader, "Commission"),
                        CommissionDate = SqlHelper.Read<DateTime>(reader, "CommissionDate"),
                        Note = SqlHelper.Read<string>(reader, "Note"),
                        IsCollect = SqlHelper.Read<bool>(reader, "IsCollect"),
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

        public override int Create(CommissionBo obj)
        {
            var id = 0;
            var cmd = Connection.CreateCommand() as SqlCommand;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "Commission_Insert";
            cmd.Parameters.AddWithValue("CustomerId", obj.CustomerId);
            cmd.Parameters.AddWithValue("ContractId", obj.ContractId);
            cmd.Parameters.AddWithValue("CompanyId", obj.CompanyId);
            cmd.Parameters.AddWithValue("OfficeId", obj.OfficeId);
            cmd.Parameters.AddWithValue("Name", obj.Name);
            cmd.Parameters.AddWithValue("Commission", obj.Commission);
            cmd.Parameters.AddWithValue("CommissionDate", obj.CommissionDate != DateTime.MinValue ? obj.CommissionDate : null);
            cmd.Parameters.AddWithValue("IsCollect", obj.IsCollect);
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

        public override CommissionBo Read(int id)
        {
            CommissionBo tmp = null;
            var cmd = Connection.CreateCommand() as SqlCommand;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "Commission_Select";
            cmd.Parameters.Add(new SqlParameter("ID", id));

            try
            {
                Connection.Open();

                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    tmp = new CommissionBo
                    {
                        Id = SqlHelper.Read<int>(reader, "Id"),
                        CustomerId = SqlHelper.Read<int>(reader, "CustomerId"),
                        ContractId = SqlHelper.Read<int>(reader, "ContractId"),
                        CompanyId = SqlHelper.Read<int>(reader, "CompanyId"),
                        OfficeId = SqlHelper.Read<int>(reader, "OfficeId"),
                        Commission = SqlHelper.Read<decimal>(reader, "Commission"),
                        CommissionDate = SqlHelper.Read<DateTime>(reader, "CommissionDate"),
                        Name = SqlHelper.Read<string>(reader, "Name"),
                        Note = SqlHelper.Read<string>(reader, "Note"),
                        IsCollect = SqlHelper.Read<bool>(reader, "IsCollect"),
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

        public override bool Update(CommissionBo obj)
        {
            int num = 0;
            var cmd = Connection.CreateCommand() as SqlCommand;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "Commission_Update";
            cmd.Parameters.AddWithValue("Id", obj.Id);
            cmd.Parameters.AddWithValue("OfficeId", obj.OfficeId);
            cmd.Parameters.AddWithValue("Name", obj.Name);
            cmd.Parameters.AddWithValue("Commission", obj.Commission);
            cmd.Parameters.AddWithValue("CommissionDate", obj.CommissionDate != DateTime.MinValue ? obj.CommissionDate : null);
            cmd.Parameters.AddWithValue("IsCollect", obj.IsCollect);
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
            cmd.CommandText = "Commission_Delete";
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

        public List<CommissionBo> GetByContract(int contractId)
        {
            var ret = new List<CommissionBo>();
            var cmd = Connection.CreateCommand() as SqlCommand;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "Commission_SelectByContract";
            cmd.Parameters.Add(new SqlParameter("ContractID", contractId));

            try
            {
                Connection.Open();

                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var tmp = new CommissionBo
                    {
                        Id = SqlHelper.Read<int>(reader, "Id"),
                        CustomerId = SqlHelper.Read<int>(reader, "CustomerId"),
                        ContractId = SqlHelper.Read<int>(reader, "ContractId"),
                        CompanyId = SqlHelper.Read<int>(reader, "CompanyId"),
                        OfficeId = SqlHelper.Read<int>(reader, "OfficeId"),
                        Name = SqlHelper.Read<string>(reader, "Name"),
                        Commission = SqlHelper.Read<decimal>(reader, "Commission"),
                        CommissionDate = SqlHelper.Read<DateTime>(reader, "CommissionDate"),
                        Note = SqlHelper.Read<string>(reader, "Note"),
                        IsCollect = SqlHelper.Read<bool>(reader, "IsCollect"),
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

