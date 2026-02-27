using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using tcs.bo;
using tcs.lib;

namespace tcs.dao
{
    public class SeminarPlaceSql : IDbConnect<SeminarPlaceBo>
    {
        private static SeminarPlaceSql _instance;

        public static SeminarPlaceSql Instance => _instance ?? (_instance = new SeminarPlaceSql(new SqlConnection(ConfigMgr.ConnectionString)));
        public SeminarPlaceSql(SqlConnection connection) : base(connection) { }

        public override List<SeminarPlaceBo> Select(IQuery query)
        {
            throw new NotImplementedException();
        }

        public override int Create(SeminarPlaceBo obj)
        {
            var id = 0;
            var cmd = Connection.CreateCommand() as SqlCommand;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "SeminarPlace_Insert";
            cmd.Parameters.AddWithValue("CompanyId", obj.CompanyId);
            cmd.Parameters.AddWithValue("SeminarId", obj.SeminarId);
            cmd.Parameters.AddWithValue("ProvinceId", obj.ProvinceId);
            cmd.Parameters.AddWithValue("ProvinceName", obj.ProvinceName);
            cmd.Parameters.AddWithValue("Place", obj.Place);
            cmd.Parameters.AddWithValue("Address", obj.Address);
            cmd.Parameters.AddWithValue("SeminarDate", obj.SeminarDate);
            cmd.Parameters.AddWithValue("Note", obj.Note);
            cmd.Parameters.AddWithValue("Cost", obj.Cost);
            cmd.Parameters.AddWithValue("Status", obj.Status);
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

        public override SeminarPlaceBo Read(int id)
        {
            SeminarPlaceBo tmp = null;
            var cmd = Connection.CreateCommand() as SqlCommand;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "SeminarPlace_Select";
            cmd.Parameters.Add(new SqlParameter("ID", id));

            try
            {
                Connection.Open();

                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    tmp = new SeminarPlaceBo
                    {
                        Id = SqlHelper.Read<int>(reader, "Id"),
                        CompanyId = SqlHelper.Read<int>(reader, "CompanyId"),
                        SeminarId = SqlHelper.Read<int>(reader, "SeminarId"),
                        ProvinceId = SqlHelper.Read<int>(reader, "ProvinceId"),
                        ProvinceName = SqlHelper.Read<string>(reader, "ProvinceName"),
                        Place = SqlHelper.Read<string>(reader, "Place"),
                        Address = SqlHelper.Read<string>(reader, "Address"),
                        SeminarDate = SqlHelper.Read<DateTime>(reader, "SeminarDate"),
                        Note = SqlHelper.Read<string>(reader, "Note"),
                        Register = SqlHelper.Read<int>(reader, "Register"),
                        Attend = SqlHelper.Read<int>(reader, "Attend"),
                        Cost = SqlHelper.Read<decimal>(reader, "Cost"),
                        Status = SqlHelper.Read<int>(reader, "Status"),
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

        public override bool Update(SeminarPlaceBo obj)
        {
            int num = 0;
            var cmd = Connection.CreateCommand() as SqlCommand;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "SeminarPlace_Update";
            cmd.Parameters.AddWithValue("Id", obj.Id);
            cmd.Parameters.AddWithValue("ProvinceId", obj.ProvinceId);
            cmd.Parameters.AddWithValue("ProvinceName", obj.ProvinceName);
            cmd.Parameters.AddWithValue("Place", obj.Place);
            cmd.Parameters.AddWithValue("Address", obj.Address);
            cmd.Parameters.AddWithValue("SeminarDate", obj.SeminarDate);
            cmd.Parameters.AddWithValue("Note", obj.Note);
            cmd.Parameters.AddWithValue("Cost", obj.Cost);
            cmd.Parameters.AddWithValue("Status", obj.Status);
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
            cmd.CommandText = "SeminarPlace_Delete";
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

        public List<SeminarPlaceBo> GetBySeminar(int id)
        {
            var ret = new List<SeminarPlaceBo>();
            var cmd = Connection.CreateCommand() as SqlCommand;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "SeminarPlace_GetBySeminar";
            cmd.Parameters.Add(new SqlParameter("ID", id));

            try
            {
                Connection.Open();

                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var tmp = new SeminarPlaceBo
                    {
                        Id = SqlHelper.Read<int>(reader, "Id"),
                        CompanyId = SqlHelper.Read<int>(reader, "CompanyId"),
                        SeminarId = SqlHelper.Read<int>(reader, "SeminarId"),
                        ProvinceId = SqlHelper.Read<int>(reader, "ProvinceId"),
                        ProvinceName = SqlHelper.Read<string>(reader, "ProvinceName"),
                        Place = SqlHelper.Read<string>(reader, "Place"),
                        Address = SqlHelper.Read<string>(reader, "Address"),
                        SeminarDate = SqlHelper.Read<DateTime>(reader, "SeminarDate"),
                        Note = SqlHelper.Read<string>(reader, "Note"),
                        Register = SqlHelper.Read<int>(reader, "Register"),
                        Attend = SqlHelper.Read<int>(reader, "Attend"),
                        Cost = SqlHelper.Read<decimal>(reader, "Cost"),
                        Status = SqlHelper.Read<int>(reader, "Status"),
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

