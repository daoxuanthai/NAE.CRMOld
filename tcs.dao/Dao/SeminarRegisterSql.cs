using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using tcs.bo;
using tcs.lib;

namespace tcs.dao
{
    public class SeminarRegisterSql : IDbConnect<SeminarRegisterBo>
    {
        private static SeminarRegisterSql _instance;

        public static SeminarRegisterSql Instance => _instance ?? (_instance = new SeminarRegisterSql(new SqlConnection(ConfigMgr.ConnectionString)));
        public SeminarRegisterSql(SqlConnection connection) : base(connection) { }

        public override List<SeminarRegisterBo> Select(IQuery query)
        {
            var qr = query as SeminarRegisterQuery;
            var ret = new List<SeminarRegisterBo>();
            var cmd = Connection.CreateCommand() as SqlCommand;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "SeminarRegister_Search";
            cmd.Parameters.Add(new SqlParameter("Keyword", qr.Keyword));
            cmd.Parameters.Add(new SqlParameter("CompanyId", qr.Company));
            cmd.Parameters.Add(new SqlParameter("SeminarId", qr.SeminarId));
            cmd.Parameters.Add(new SqlParameter("SeminarPlaceId", qr.SeminarPlaceId));
            cmd.Parameters.Add(new SqlParameter("IsAttend", qr.IsAttend));
            cmd.Parameters.Add(new SqlParameter("TitleId", qr.TitleId));
            cmd.Parameters.Add(new SqlParameter("CurrentPage", qr.Page));
            cmd.Parameters.Add(new SqlParameter("RecordPerPage", qr.PageSize));

            try
            {
                Connection.Open();

                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var tmp = new SeminarRegisterBo
                    {
                        Id = SqlHelper.Read<int>(reader, "Id"),
                        CompanyId = SqlHelper.Read<int>(reader, "CompanyId"),
                        SeminarId = SqlHelper.Read<int>(reader, "SeminarId"),
                        SeminarName = SqlHelper.Read<string>(reader, "SeminarName"),
                        SeminarPlaceId = SqlHelper.Read<int>(reader, "SeminarPlaceId"),
                        CustomerId = SqlHelper.Read<int>(reader, "CustomerId"),
                        FullName = SqlHelper.Read<string>(reader, "FullName"),
                        Phone = SqlHelper.Read<string>(reader, "Phone"),
                        Email = SqlHelper.Read<string>(reader, "Email"),
                        TicketId = SqlHelper.Read<string>(reader, "TicketId"),
                        School1 = SqlHelper.Read<string>(reader, "School1"),
                        School1Time = SqlHelper.Read<string>(reader, "School1Time"),
                        School2 = SqlHelper.Read<string>(reader, "School2"),
                        School2Time = SqlHelper.Read<string>(reader, "School2Time"),
                        School3 = SqlHelper.Read<string>(reader, "School3"),
                        School3Time = SqlHelper.Read<string>(reader, "School3Time"),
                        EmployeeName = SqlHelper.Read<string>(reader, "EmployeeName"),
                        CustomerNote = SqlHelper.Read<string>(reader, "CustomerNote"),
                        Note = SqlHelper.Read<string>(reader, "Note"),
                        IsAttend = SqlHelper.Read<bool>(reader, "IsAttend"),
                        Source = SqlHelper.Read<int>(reader, "Source"),
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

        public override int Create(SeminarRegisterBo obj)
        {
            var id = 0;
            var cmd = Connection.CreateCommand() as SqlCommand;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "SeminarRegister_Insert";
            cmd.Parameters.AddWithValue("CompanyId", obj.CompanyId);
            cmd.Parameters.AddWithValue("SeminarId", obj.SeminarId);
            cmd.Parameters.AddWithValue("SeminarName", obj.SeminarName);
            cmd.Parameters.AddWithValue("SeminarPlaceId", obj.SeminarPlaceId);
            cmd.Parameters.AddWithValue("CustomerId", obj.CustomerId);
            cmd.Parameters.AddWithValue("TicketId", obj.TicketId);
            cmd.Parameters.AddWithValue("School1", obj.School1);
            cmd.Parameters.AddWithValue("School1Time", obj.School1Time);
            cmd.Parameters.AddWithValue("School2", obj.School2);
            cmd.Parameters.AddWithValue("School2Time", obj.School2Time);
            cmd.Parameters.AddWithValue("School3", obj.School3);
            cmd.Parameters.AddWithValue("School3Time", obj.School3Time);
            cmd.Parameters.AddWithValue("CustomerNote", obj.CustomerNote);
            cmd.Parameters.AddWithValue("Note", obj.Note);
            cmd.Parameters.AddWithValue("IsAttend", obj.IsAttend);
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

        public override SeminarRegisterBo Read(int id)
        {
            SeminarRegisterBo tmp = null;
            var cmd = Connection.CreateCommand() as SqlCommand;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "SeminarRegister_Select";
            cmd.Parameters.Add(new SqlParameter("ID", id));

            try
            {
                Connection.Open();

                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    tmp = new SeminarRegisterBo
                    {
                        Id = SqlHelper.Read<int>(reader, "Id"),
                        CompanyId = SqlHelper.Read<int>(reader, "CompanyId"),
                        SeminarId = SqlHelper.Read<int>(reader, "SeminarId"),
                        SeminarName = SqlHelper.Read<string>(reader, "SeminarName"),
                        SeminarPlaceId = SqlHelper.Read<int>(reader, "SeminarPlaceId"),
                        CustomerId = SqlHelper.Read<int>(reader, "CustomerId"),
                        FullName = SqlHelper.Read<string>(reader, "FullName"),
                        Phone = SqlHelper.Read<string>(reader, "Phone"),
                        Email = SqlHelper.Read<string>(reader, "Email"),
                        TicketId = SqlHelper.Read<string>(reader, "TicketId"),
                        School1 = SqlHelper.Read<string>(reader, "School1"),
                        School1Time = SqlHelper.Read<string>(reader, "School1Time"),
                        School2 = SqlHelper.Read<string>(reader, "School2"),
                        School2Time = SqlHelper.Read<string>(reader, "School2Time"),
                        School3 = SqlHelper.Read<string>(reader, "School3"),
                        School3Time = SqlHelper.Read<string>(reader, "School3Time"),
                        EmployeeName = SqlHelper.Read<string>(reader, "EmployeeName"),
                        CustomerNote = SqlHelper.Read<string>(reader, "CustomerNote"),
                        Note = SqlHelper.Read<string>(reader, "Note"),
                        IsAttend = SqlHelper.Read<bool>(reader, "IsAttend"),
                        Source = SqlHelper.Read<int>(reader, "Source"),
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

        public override bool Update(SeminarRegisterBo obj)
        {
            int num = 0;
            var cmd = Connection.CreateCommand() as SqlCommand;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "SeminarRegister_Update";
            cmd.Parameters.AddWithValue("Id", obj.Id);
            cmd.Parameters.AddWithValue("SeminarId", obj.SeminarId);
            cmd.Parameters.AddWithValue("SeminarName", obj.SeminarName);
            cmd.Parameters.AddWithValue("SeminarPlaceId", obj.SeminarPlaceId);
            cmd.Parameters.AddWithValue("CustomerId", obj.CustomerId);
            cmd.Parameters.AddWithValue("TicketId", obj.TicketId);
            cmd.Parameters.AddWithValue("School1", obj.School1);
            cmd.Parameters.AddWithValue("School1Time", obj.School1Time);
            cmd.Parameters.AddWithValue("School2", obj.School2);
            cmd.Parameters.AddWithValue("School2Time", obj.School2Time);
            cmd.Parameters.AddWithValue("School3", obj.School3);
            cmd.Parameters.AddWithValue("School3Time", obj.School3Time);
            cmd.Parameters.AddWithValue("CustomerNote", obj.CustomerNote);
            cmd.Parameters.AddWithValue("Note", obj.Note);
            cmd.Parameters.AddWithValue("IsAttend", obj.IsAttend);
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
            cmd.CommandText = "SeminarRegister_Delete";
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

        public List<SeminarRegisterBo> GetByCustomer(int customerId, int seminarId)
        {
            var result = new List<SeminarRegisterBo>();
            var cmd = Connection.CreateCommand() as SqlCommand;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "SeminarRegister_GetByCustomer";
            cmd.Parameters.Add(new SqlParameter("CustomerId", customerId));
            cmd.Parameters.Add(new SqlParameter("SeminarId", seminarId));

            try
            {
                Connection.Open();

                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    var tmp = new SeminarRegisterBo
                    {
                        Id = SqlHelper.Read<int>(reader, "Id"),
                        CompanyId = SqlHelper.Read<int>(reader, "CompanyId"),
                        SeminarId = SqlHelper.Read<int>(reader, "SeminarId"),
                        SeminarName = SqlHelper.Read<string>(reader, "SeminarName"),
                        SeminarPlaceId = SqlHelper.Read<int>(reader, "SeminarPlaceId"),
                        CustomerId = SqlHelper.Read<int>(reader, "CustomerId"),
                        FullName = SqlHelper.Read<string>(reader, "FullName"),
                        Phone = SqlHelper.Read<string>(reader, "Phone"),
                        Email = SqlHelper.Read<string>(reader, "Email"),
                        TicketId = SqlHelper.Read<string>(reader, "TicketId"),
                        School1 = SqlHelper.Read<string>(reader, "School1"),
                        School1Time = SqlHelper.Read<string>(reader, "School1Time"),
                        School2 = SqlHelper.Read<string>(reader, "School2"),
                        School2Time = SqlHelper.Read<string>(reader, "School2Time"),
                        School3 = SqlHelper.Read<string>(reader, "School3"),
                        School3Time = SqlHelper.Read<string>(reader, "School3Time"),
                        EmployeeName = SqlHelper.Read<string>(reader, "EmployeeName"),
                        CustomerNote = SqlHelper.Read<string>(reader, "CustomerNote"),
                        Note = SqlHelper.Read<string>(reader, "Note"),
                        IsAttend = SqlHelper.Read<bool>(reader, "IsAttend"),
                        Source = SqlHelper.Read<int>(reader, "Source"),
                        CreateUserId = SqlHelper.Read<int>(reader, "CreateUserId"),
                        CreateUserName = SqlHelper.Read<string>(reader, "CreateUserName"),
                        UpdateUserId = SqlHelper.Read<int>(reader, "UpdateUserId"),
                        UpdateUserName = SqlHelper.Read<string>(reader, "UpdateUserName"),
                        CreateDate = SqlHelper.Read<DateTime>(reader, "CreateDate"),
                        UpdateDate = SqlHelper.Read<DateTime>(reader, "UpdateDate")
                    };
                    result.Add(tmp);
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

            return result;
        }
    }
}

