using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using tcs.bo;
using tcs.lib;

namespace tcs.dao
{
    public class GuaranteeSql : IDbConnect<GuaranteeBo>
    {
        private static GuaranteeSql _instance;

        public static GuaranteeSql Instance => _instance ?? (_instance = new GuaranteeSql(new SqlConnection(ConfigMgr.ConnectionString)));
        public GuaranteeSql(SqlConnection connection) : base(connection) { }

        public override List<GuaranteeBo> Select(IQuery query)
        {
            return null;
        }

        public override int Create(GuaranteeBo obj)
        {
            var id = 0;
            var cmd = Connection.CreateCommand() as SqlCommand;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "Guarantee_Insert";
            cmd.Parameters.AddWithValue("CustomerId", obj.CustomerId);
            cmd.Parameters.AddWithValue("CompanyId", obj.CompanyId);
            cmd.Parameters.AddWithValue("RelativesId", obj.RelativesId);
            cmd.Parameters.AddWithValue("RelativesName", obj.RelativesName);
            cmd.Parameters.AddWithValue("Person", obj.Person);
            cmd.Parameters.AddWithValue("GuaranteeType", obj.GuaranteeType);
            cmd.Parameters.AddWithValue("GuaranteeName", obj.GuaranteeName);
            cmd.Parameters.AddWithValue("GuaranteeYear", obj.GuaranteeYear);
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

        public override GuaranteeBo Read(int id)
        {
            GuaranteeBo tmp = null;
            var cmd = Connection.CreateCommand() as SqlCommand;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "Guarantee_Select";
            cmd.Parameters.Add(new SqlParameter("ID", id));

            try
            {
                Connection.Open();

                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    tmp = new GuaranteeBo
                    {
                        Id = SqlHelper.Read<int>(reader, "Id"),
                        CustomerId = SqlHelper.Read<int>(reader, "CustomerId"),
                        CompanyId = SqlHelper.Read<int>(reader, "CompanyId"),
                        RelativesId = SqlHelper.Read<int>(reader, "RelativesId"),
                        RelativesName = SqlHelper.Read<string>(reader, "RelativesName"),
                        Person = SqlHelper.Read<string>(reader, "Person"),
                        GuaranteeType = SqlHelper.Read<string>(reader, "GuaranteeType"),
                        GuaranteeName = SqlHelper.Read<string>(reader, "GuaranteeName"),
                        GuaranteeYear = SqlHelper.Read<string>(reader, "GuaranteeYear"),
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

        public override bool Update(GuaranteeBo obj)
        {
            int num = 0;
            var cmd = Connection.CreateCommand() as SqlCommand;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "Guarantee_Update";
            cmd.Parameters.AddWithValue("Id", obj.Id);
            cmd.Parameters.AddWithValue("RelativesId", obj.RelativesId);
            cmd.Parameters.AddWithValue("RelativesName", obj.RelativesName);
            cmd.Parameters.AddWithValue("Person", obj.Person);
            cmd.Parameters.AddWithValue("GuaranteeType", obj.GuaranteeType);
            cmd.Parameters.AddWithValue("GuaranteeName", obj.GuaranteeName);
            cmd.Parameters.AddWithValue("GuaranteeYear", obj.GuaranteeYear);
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
            cmd.CommandText = "Guarantee_Delete";
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

        public List<GuaranteeBo> GetByCustomer(int customerId)
        {
            var ret = new List<GuaranteeBo>();
            var cmd = Connection.CreateCommand() as SqlCommand;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "Guarantee_GetByCustomer";
            cmd.Parameters.Add(new SqlParameter("CustomerId", customerId));

            try
            {
                Connection.Open();

                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var tmp = new GuaranteeBo
                    {
                        Id = SqlHelper.Read<int>(reader, "Id"),
                        CustomerId = SqlHelper.Read<int>(reader, "CustomerId"),
                        CompanyId = SqlHelper.Read<int>(reader, "CompanyId"),
                        RelativesId = SqlHelper.Read<int>(reader, "RelativesId"),
                        RelativesName = SqlHelper.Read<string>(reader, "RelativesName"),
                        Person = SqlHelper.Read<string>(reader, "Person"),
                        GuaranteeType = SqlHelper.Read<string>(reader, "GuaranteeType"),
                        GuaranteeName = SqlHelper.Read<string>(reader, "GuaranteeName"),
                        GuaranteeYear = SqlHelper.Read<string>(reader, "GuaranteeYear"),
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

