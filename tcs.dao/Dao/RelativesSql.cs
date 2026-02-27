using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using tcs.bo;
using tcs.lib;

namespace tcs.dao
{
    public class RelativesSql : IDbConnect<RelativesBo>
    {
        private static RelativesSql _instance;

        public static RelativesSql Instance => _instance ?? (_instance = new RelativesSql(new SqlConnection(ConfigMgr.ConnectionString)));
        public RelativesSql(SqlConnection connection) : base(connection) { }

        public override List<RelativesBo> Select(IQuery query)
        {
            var qr = query as RelativesQuery;
            var ret = new List<RelativesBo>();
            var cmd = Connection.CreateCommand() as SqlCommand;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "Relatives_Search";
            cmd.Parameters.Add(new SqlParameter("Keyword", qr.Keyword));
            cmd.Parameters.Add(new SqlParameter("CurrentPage", qr.Page));
            cmd.Parameters.Add(new SqlParameter("RecordPerPage", qr.PageSize));

            try
            {
                Connection.Open();

                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var tmp = new RelativesBo
                    {
                        Id = SqlHelper.Read<int>(reader, "Id"),
                        CustomerId = SqlHelper.Read<int>(reader, "CustomerId"),
                        CompanyId = SqlHelper.Read<int>(reader, "CompanyId"),
                        Name = SqlHelper.Read<string>(reader, "Name"),
                        Relationship = SqlHelper.Read<string>(reader, "Relationship"),
                        CountryName = SqlHelper.Read<string>(reader, "CountryName"),
                        Address = SqlHelper.Read<string>(reader, "Address"),
                        JobName = SqlHelper.Read<string>(reader, "JobName"),
                        CompanyName = SqlHelper.Read<string>(reader, "CompanyName"),
                        Income = SqlHelper.Read<string>(reader, "Income"),
                        Note = SqlHelper.Read<string>(reader, "Note"),
                        CreateUserId = SqlHelper.Read<int>(reader, "CreateUserId"),
                        CreateUserName = SqlHelper.Read<string>(reader, "CreateUserName"),
                        UpdateUserId = SqlHelper.Read<int>(reader, "UpdateUserId"),
                        UpdateUserName = SqlHelper.Read<string>(reader, "UpdateUserName"),
                        CreateDate = SqlHelper.Read<DateTime>(reader, "CreateDate"),
                        UpdateDate = SqlHelper.Read<DateTime>(reader, "UpdateDate"),
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

        public override int Create(RelativesBo obj)
        {
            var id = 0;
            var cmd = Connection.CreateCommand() as SqlCommand;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "Relatives_Insert";
            cmd.Parameters.AddWithValue("CustomerId", obj.CustomerId);
            cmd.Parameters.AddWithValue("CompanyId", obj.CompanyId);
            cmd.Parameters.AddWithValue("Name", obj.Name);
            cmd.Parameters.AddWithValue("Relationship", obj.Relationship);
            cmd.Parameters.AddWithValue("CountryName", obj.CountryName);
            cmd.Parameters.AddWithValue("Address", obj.Address);
            cmd.Parameters.AddWithValue("JobName", obj.JobName);
            cmd.Parameters.AddWithValue("CompanyName", obj.CompanyName);
            cmd.Parameters.AddWithValue("Income", obj.Income);
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

        public override RelativesBo Read(int id)
        {
            RelativesBo tmp = null;
            var cmd = Connection.CreateCommand() as SqlCommand;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "Relatives_Select";
            cmd.Parameters.Add(new SqlParameter("ID", id));

            try
            {
                Connection.Open();

                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    tmp = new RelativesBo
                    {
                        Id = SqlHelper.Read<int>(reader, "Id"),
                        CustomerId = SqlHelper.Read<int>(reader, "CustomerId"),
                        CompanyId = SqlHelper.Read<int>(reader, "CompanyId"),
                        Name = SqlHelper.Read<string>(reader, "Name"),
                        Relationship = SqlHelper.Read<string>(reader, "Relationship"),
                        CountryName = SqlHelper.Read<string>(reader, "CountryName"),
                        Address = SqlHelper.Read<string>(reader, "Address"),
                        JobName = SqlHelper.Read<string>(reader, "JobName"),
                        CompanyName = SqlHelper.Read<string>(reader, "CompanyName"),
                        Income = SqlHelper.Read<string>(reader, "Income"),
                        Note = SqlHelper.Read<string>(reader, "Note"),
                        CreateUserId = SqlHelper.Read<int>(reader, "CreateUserId"),
                        CreateUserName = SqlHelper.Read<string>(reader, "CreateUserName"),
                        UpdateUserId = SqlHelper.Read<int>(reader, "UpdateUserId"),
                        UpdateUserName = SqlHelper.Read<string>(reader, "UpdateUserName"),
                        CreateDate = SqlHelper.Read<DateTime>(reader, "CreateDate"),
                        UpdateDate = SqlHelper.Read<DateTime>(reader, "UpdateDate"),
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

        public override bool Update(RelativesBo obj)
        {
            int num = 0;
            var cmd = Connection.CreateCommand() as SqlCommand;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "Relatives_Update";
            cmd.Parameters.AddWithValue("Id", obj.Id);
            cmd.Parameters.AddWithValue("Name", obj.Name);
            cmd.Parameters.AddWithValue("Relationship", obj.Relationship);
            cmd.Parameters.AddWithValue("CountryName", obj.CountryName);
            cmd.Parameters.AddWithValue("Address", obj.Address);
            cmd.Parameters.AddWithValue("JobName", obj.JobName);
            cmd.Parameters.AddWithValue("CompanyName", obj.CompanyName);
            cmd.Parameters.AddWithValue("Income", obj.Income);
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
            cmd.CommandText = "Relatives_Delete";
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

        public List<RelativesBo> GetByCustomer(int customerId)
        {
            var ret = new List<RelativesBo>();
            var cmd = Connection.CreateCommand() as SqlCommand;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "Relatives_GetByCustomer";
            cmd.Parameters.Add(new SqlParameter("CustomerId", customerId));

            try
            {
                Connection.Open();

                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var tmp = new RelativesBo
                    {
                        Id = SqlHelper.Read<int>(reader, "Id"),
                        CustomerId = SqlHelper.Read<int>(reader, "CustomerId"),
                        CompanyId = SqlHelper.Read<int>(reader, "CompanyId"),
                        Name = SqlHelper.Read<string>(reader, "Name"),
                        Relationship = SqlHelper.Read<string>(reader, "Relationship"),
                        CountryName = SqlHelper.Read<string>(reader, "CountryName"),
                        Address = SqlHelper.Read<string>(reader, "Address"),
                        JobName = SqlHelper.Read<string>(reader, "JobName"),
                        CompanyName = SqlHelper.Read<string>(reader, "CompanyName"),
                        Income = SqlHelper.Read<string>(reader, "Income"),
                        Note = SqlHelper.Read<string>(reader, "Note"),
                        CreateUserId = SqlHelper.Read<int>(reader, "CreateUserId"),
                        CreateUserName = SqlHelper.Read<string>(reader, "CreateUserName"),
                        UpdateUserId = SqlHelper.Read<int>(reader, "UpdateUserId"),
                        UpdateUserName = SqlHelper.Read<string>(reader, "UpdateUserName"),
                        CreateDate = SqlHelper.Read<DateTime>(reader, "CreateDate"),
                        UpdateDate = SqlHelper.Read<DateTime>(reader, "UpdateDate"),
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

