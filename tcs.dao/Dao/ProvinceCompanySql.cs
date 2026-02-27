using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using tcs.bo;
using tcs.lib;

namespace tcs.dao
{
    public class ProvinceCompanySql : IDbConnect<ProvinceCompanyBo>
    {
        private static ProvinceCompanySql _instance;

        public static ProvinceCompanySql Instance => _instance ??
                                                     (_instance = new ProvinceCompanySql(
                                                         new SqlConnection(ConfigMgr.AccountConnectionString)));
        public ProvinceCompanySql(SqlConnection connection) : base(connection) { }

        public override List<ProvinceCompanyBo> Select(IQuery query)
        {
            var qr = query as ProvinceCompanyQuery;
            var ret = new List<ProvinceCompanyBo>();
            var cmd = Connection.CreateCommand() as SqlCommand;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "ProvinceCompany_Search";
            cmd.Parameters.Add(new SqlParameter("CompanyId", qr.Company));
            cmd.Parameters.Add(new SqlParameter("OfficeId", qr.Office));
            cmd.Parameters.Add(new SqlParameter("CurrentPage", qr.Page));
            cmd.Parameters.Add(new SqlParameter("RecordPerPage", qr.PageSize));

            try
            {
                Connection.Open();

                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var tmp = new ProvinceCompanyBo
                    {
                        Id = SqlHelper.Read<int>(reader, "Id"),
                        ProvinceId = SqlHelper.Read<int>(reader, "ProvinceId"),
                        ProvinceName = SqlHelper.Read<string>(reader, "ProvinceName"),
                        CompanyId = SqlHelper.Read<int>(reader, "CompanyId"),
                        OfficeId = SqlHelper.Read<int>(reader, "OfficeId"),
                        OfficeName = SqlHelper.Read<string>(reader, "OfficeName"),
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

        public override int Create(ProvinceCompanyBo obj)
        {
            var id = 0;
            var cmd = Connection.CreateCommand() as SqlCommand;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "ProvinceCompany_Insert";
            cmd.Parameters.AddWithValue("ProvinceId", obj.ProvinceId);
            cmd.Parameters.AddWithValue("ProvinceName", obj.ProvinceName);
            cmd.Parameters.AddWithValue("CompanyId", obj.CompanyId);
            cmd.Parameters.AddWithValue("OfficeId", obj.OfficeId);
            cmd.Parameters.AddWithValue("OfficeName", obj.OfficeName);
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

        public override ProvinceCompanyBo Read(int id)
        {
            ProvinceCompanyBo tmp = null;
            var cmd = Connection.CreateCommand() as SqlCommand;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "ProvinceCompany_Select";
            cmd.Parameters.Add(new SqlParameter("ID", id));

            try
            {
                Connection.Open();

                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    tmp = new ProvinceCompanyBo
                    {
                        Id = SqlHelper.Read<int>(reader, "Id"),
                        ProvinceId = SqlHelper.Read<int>(reader, "ProvinceId"),
                        ProvinceName = SqlHelper.Read<string>(reader, "ProvinceName"),
                        CompanyId = SqlHelper.Read<int>(reader, "CompanyId"),
                        OfficeId = SqlHelper.Read<int>(reader, "OfficeId"),
                        OfficeName = SqlHelper.Read<string>(reader, "OfficeName"),
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

        public override bool Update(ProvinceCompanyBo obj)
        {
            int num = 0;
            var cmd = Connection.CreateCommand() as SqlCommand;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "ProvinceCompany_Update";
            cmd.Parameters.AddWithValue("Id", obj.Id);
            cmd.Parameters.AddWithValue("CompanyId", obj.CompanyId);
            cmd.Parameters.AddWithValue("OfficeId", obj.OfficeId);
            cmd.Parameters.AddWithValue("OfficeName", obj.OfficeName);
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
            cmd.CommandText = "ProvinceCompany_Delete";
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

        public List<ProvinceCompanyBo> GetByCompany(int companyId)
        {
            var ret = new List<ProvinceCompanyBo>();
            var cmd = Connection.CreateCommand() as SqlCommand;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "ProvinceCompany_GetByCompany";
            cmd.Parameters.Add(new SqlParameter("CompanyId", companyId));

            try
            {
                Connection.Open();

                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var tmp = new ProvinceCompanyBo
                    {
                        Id = SqlHelper.Read<int>(reader, "Id"),
                        ProvinceId = SqlHelper.Read<int>(reader, "ProvinceId"),
                        ProvinceName = SqlHelper.Read<string>(reader, "ProvinceName"),
                        CompanyId = SqlHelper.Read<int>(reader, "CompanyId"),
                        OfficeId = SqlHelper.Read<int>(reader, "OfficeId"),
                        OfficeName = SqlHelper.Read<string>(reader, "OfficeName"),
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

        public bool DeleteByCompany(int companyId)
        {
            int num = 0;
            var cmd = Connection.CreateCommand() as SqlCommand;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "ProvinceCompany_DeleteByCompany";
            cmd.Parameters.AddWithValue("CompanyId", companyId);

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

