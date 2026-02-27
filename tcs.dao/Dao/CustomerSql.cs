using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using tcs.bo;
using tcs.lib;

namespace tcs.dao
{
    public class CustomerSql : IDbConnect<CustomerBo>
    {
        private static CustomerSql _instance;

        public static CustomerSql Instance => _instance ??
                                              (_instance =
                                                  new CustomerSql(new SqlConnection(ConfigMgr.ConnectionString))
                                              );
        public CustomerSql(SqlConnection connection) : base(connection) { }

        public override List<CustomerBo> Select(IQuery query)
        {
            var qr = query as CustomerQuery;
            var ret = new List<CustomerBo>();
            var cmd = Connection.CreateCommand() as SqlCommand;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "Customer_Search";
            cmd.Parameters.Add(new SqlParameter("keyword", qr.Keyword));
            cmd.Parameters.Add(new SqlParameter("Status", qr.Status));
            cmd.Parameters.Add(new SqlParameter("EmployeeId", qr.Employee));
            cmd.Parameters.Add(new SqlParameter("AreaId", qr.Area));
            cmd.Parameters.Add(new SqlParameter("CountryId", qr.Country));
            cmd.Parameters.Add(new SqlParameter("CompanyId", qr.Company));
            cmd.Parameters.Add(new SqlParameter("OfficeId", qr.Office));
            cmd.Parameters.Add(new SqlParameter("SourceType", qr.SourceType));
            cmd.Parameters.Add(new SqlParameter("Source", qr.Source));
            cmd.Parameters.Add(new SqlParameter("EducationLevel", qr.EducationLevel));
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
                    var tmp = new CustomerBo
                    {
                        Id = SqlHelper.Read<int>(reader, "Id"),
                        AreaId = SqlHelper.Read<int>(reader, "AreaId"),
                        ProvinceId = SqlHelper.Read<int>(reader, "ProvinceId"),
                        CompanyId = SqlHelper.Read<int>(reader, "CompanyId"),
                        OfficeId = SqlHelper.Read<int>(reader, "OfficeId"),
                        Fullname = SqlHelper.Read<string>(reader, "Fullname"),
                        Gender = SqlHelper.Read<int>(reader, "Gender"),
                        Email = SqlHelper.Read<string>(reader, "Email"),
                        Phone = SqlHelper.Read<string>(reader, "Phone"),
                        Birthday = SqlHelper.Read<DateTime>(reader, "Birthday"),
                        Address = SqlHelper.Read<string>(reader, "Address"),
                        CustomerNote = SqlHelper.Read<string>(reader, "CustomerNote"),
                        AdvisoryNote = SqlHelper.Read<string>(reader, "AdvisoryNote"),
                        EmployeeNote = SqlHelper.Read<string>(reader, "EmployeeNote"),
                        CountryId = SqlHelper.Read<string>(reader, "CountryId"),
                        EducationLevelId = SqlHelper.Read<string>(reader, "EducationLevelId"),
                        SearchInfo = SqlHelper.Read<string>(reader, "SearchInfo"),
                        CustomerType = SqlHelper.Read<int>(reader, "CustomerType"),
                        ProfileType = SqlHelper.Read<int>(reader, "ProfileType"),
                        NewsIdRef = SqlHelper.Read<int>(reader, "NewsIdRef"),
                        NewsUrlRef = SqlHelper.Read<string>(reader, "NewsUrlRef"),
                        UserIdRef = SqlHelper.Read<int>(reader, "UserIdRef"),
                        SeminarIdRef = SqlHelper.Read<int>(reader, "SerminarIdRef"),
                        SeminarNameRef = SqlHelper.Read<string>(reader, "SeminarNameRef"),
                        Source = SqlHelper.Read<int>(reader, "Source"),
                        SourceType = SqlHelper.Read<int>(reader, "SourceType"),
                        EmployeeId = SqlHelper.Read<int>(reader, "EmployeeId"),
                        EmployeeName = SqlHelper.Read<string>(reader, "EmployeeName"),
                        EmployeeProcessId = SqlHelper.Read<int>(reader, "EmployeeProcessId"),
                        EmployeeProcessName = SqlHelper.Read<string>(reader, "EmployeeProcessName"),
                        ProcessNote = SqlHelper.Read<string>(reader, "ProcessNote"),
                        Status = SqlHelper.Read<int>(reader, "Status"),
                        IsFly = SqlHelper.Read<bool>(reader, "IsFly"),
                        IsCommission = SqlHelper.Read<bool>(reader, "IsCommission"),
                        IsAlarm = SqlHelper.Read<bool>(reader, "IsAlarm"),
                        AlarmTime = SqlHelper.Read<DateTime>(reader, "AlarmTime"),
                        AlarmTimeOrder = SqlHelper.Read<DateTime>(reader, "AlarmTimeOrder"),
                        Desire = SqlHelper.Read<int>(reader, "Desire"),
                        CreateUserId = SqlHelper.Read<int>(reader, "CreateUserId"),
                        CreateUserName = SqlHelper.Read<string>(reader, "CreateUserName"),
                        UpdateUserId = SqlHelper.Read<int>(reader, "UpdateUserId"),
                        UpdateUserName = SqlHelper.Read<string>(reader, "UpdateUserName"),
                        CreateDate = SqlHelper.Read<DateTime>(reader, "CreateDate"),
                        UpdateDate = SqlHelper.Read<DateTime>(reader, "UpdateDate"),
                        LastCare = SqlHelper.Read<DateTime>(reader, "LastCare")
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

        public override int Create(CustomerBo obj)
        {
            var id = 0;
            var cmd = Connection.CreateCommand() as SqlCommand;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "Customer_Insert";
            cmd.Parameters.AddWithValue("AreaId", obj.AreaId);
            cmd.Parameters.AddWithValue("ProvinceId", obj.ProvinceId);
            cmd.Parameters.AddWithValue("CompanyId", obj.CompanyId);
            cmd.Parameters.AddWithValue("OfficeId", obj.OfficeId);
            cmd.Parameters.AddWithValue("Fullname", obj.Fullname);
            cmd.Parameters.AddWithValue("Gender", obj.Gender);
            cmd.Parameters.AddWithValue("Email", obj.Email);
            cmd.Parameters.AddWithValue("Phone", obj.Phone);
            cmd.Parameters.AddWithValue("Birthday", obj.Birthday != DateTime.MinValue ? obj.Birthday : null);
            cmd.Parameters.AddWithValue("Address", obj.Address);
            cmd.Parameters.AddWithValue("CustomerNote", obj.CustomerNote);
            cmd.Parameters.AddWithValue("AdvisoryNote ", obj.AdvisoryNote);
            cmd.Parameters.AddWithValue("EmployeeNote", obj.EmployeeNote);
            cmd.Parameters.AddWithValue("CountryId", obj.CountryId);
            cmd.Parameters.AddWithValue("EducationLevelId", obj.EducationLevelId);
            cmd.Parameters.AddWithValue("SearchInfo", obj.SearchInfo);
            cmd.Parameters.AddWithValue("CustomerType", obj.CustomerType);
            cmd.Parameters.AddWithValue("ProfileType", obj.ProfileType);
            cmd.Parameters.AddWithValue("NewsIdRef", obj.NewsIdRef);
            cmd.Parameters.AddWithValue("NewsUrlRef", obj.NewsUrlRef);
            cmd.Parameters.AddWithValue("UserIdRef", obj.UserIdRef);
            cmd.Parameters.AddWithValue("SerminarIdRef", obj.SeminarIdRef);
            cmd.Parameters.AddWithValue("SeminarNameRef", obj.SeminarNameRef);
            cmd.Parameters.AddWithValue("Source", obj.Source);
            cmd.Parameters.AddWithValue("SourceType", obj.SourceType);
            cmd.Parameters.AddWithValue("EmployeeId", obj.EmployeeId);
            cmd.Parameters.AddWithValue("EmployeeName", obj.EmployeeName);
            cmd.Parameters.AddWithValue("EmployeeProcessId", obj.EmployeeProcessId);
            cmd.Parameters.AddWithValue("EmployeeProcessName", obj.EmployeeProcessName);
            cmd.Parameters.AddWithValue("AgencyId", obj.AgencyId);
            cmd.Parameters.AddWithValue("AgencyName", obj.AgencyName);
            cmd.Parameters.AddWithValue("ProcessNote", obj.ProcessNote);
            cmd.Parameters.AddWithValue("Status", obj.Status);
            cmd.Parameters.AddWithValue("IsFly", obj.IsFly);
            cmd.Parameters.AddWithValue("IsCommission", obj.IsCommission);
            cmd.Parameters.AddWithValue("Desire", obj.Desire);
            cmd.Parameters.AddWithValue("CreateUserId", obj.CreateUserId);
            cmd.Parameters.AddWithValue("CreateUserName", obj.CreateUserName);

            SqlParameter cusId = new SqlParameter
            {
                ParameterName = "ID",
                DbType = DbType.Int32,
                Direction = ParameterDirection.Output
            };
            cmd.Parameters.Add(cusId);

            try
            {
                Connection.Open();
                cmd.ExecuteNonQuery();
                id = (int)cusId.Value;
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

        public override CustomerBo Read(int id)
        {
            CustomerBo tmp = null;
            var cmd = Connection.CreateCommand() as SqlCommand;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "Customer_Select";
            cmd.Parameters.Add(new SqlParameter("ID", id));

            try
            {
                Connection.Open();

                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    tmp = new CustomerBo
                    {
                        Id = SqlHelper.Read<int>(reader, "Id"),
                        AreaId = SqlHelper.Read<int>(reader, "AreaId"),
                        ProvinceId = SqlHelper.Read<int>(reader, "ProvinceId"),
                        CompanyId = SqlHelper.Read<int>(reader, "CompanyId"),
                        OfficeId = SqlHelper.Read<int>(reader, "OfficeId"),
                        Fullname = SqlHelper.Read<string>(reader, "Fullname"),
                        Gender = SqlHelper.Read<int>(reader, "Gender"),
                        Email = SqlHelper.Read<string>(reader, "Email"),
                        Phone = SqlHelper.Read<string>(reader, "Phone"),
                        Birthday = SqlHelper.Read<DateTime>(reader, "Birthday"),
                        Address = SqlHelper.Read<string>(reader, "Address"),
                        CustomerNote = SqlHelper.Read<string>(reader, "CustomerNote"),
                        AdvisoryNote = SqlHelper.Read<string>(reader, "AdvisoryNote"),
                        EmployeeNote = SqlHelper.Read<string>(reader, "EmployeeNote"),
                        CountryId = SqlHelper.Read<string>(reader, "CountryId"),
                        CountryName = SqlHelper.Read<string>(reader, "CountryName"),
                        AbroadTime = SqlHelper.Read<string>(reader, "AbroadTime"),
                        EducationLevelId = SqlHelper.Read<string>(reader, "EducationLevelId"),
                        SearchInfo = SqlHelper.Read<string>(reader, "SearchInfo"),
                        CustomerType = SqlHelper.Read<int>(reader, "CustomerType"),
                        ProfileType = SqlHelper.Read<int>(reader, "ProfileType"),
                        NewsIdRef = SqlHelper.Read<int>(reader, "NewsIdRef"),
                        NewsUrlRef = SqlHelper.Read<string>(reader, "NewsUrlRef"),
                        UserIdRef = SqlHelper.Read<int>(reader, "UserIdRef"),
                        SeminarIdRef = SqlHelper.Read<int>(reader, "SerminarIdRef"),
                        SeminarNameRef = SqlHelper.Read<string>(reader, "SeminarNameRef"),
                        Source = SqlHelper.Read<int>(reader, "Source"),
                        SourceType = SqlHelper.Read<int>(reader, "SourceType"),
                        EmployeeId = SqlHelper.Read<int>(reader, "EmployeeId"),
                        EmployeeName = SqlHelper.Read<string>(reader, "EmployeeName"),
                        EmployeeProcessId = SqlHelper.Read<int>(reader, "EmployeeProcessId"),
                        EmployeeProcessName = SqlHelper.Read<string>(reader, "EmployeeProcessName"),
                        AgencyId = SqlHelper.Read<int>(reader, "AgencyId"),
                        AgencyName = SqlHelper.Read<string>(reader, "AgencyName"),
                        ProcessNote = SqlHelper.Read<string>(reader, "ProcessNote"),
                        Status = SqlHelper.Read<int>(reader, "Status"),
                        IsFly = SqlHelper.Read<bool>(reader, "IsFly"),
                        IsCommission = SqlHelper.Read<bool>(reader, "IsCommission"),
                        IsAlarm = SqlHelper.Read<bool>(reader, "IsAlarm"),
                        AlarmTime = SqlHelper.Read<DateTime>(reader, "AlarmTime"),
                        AlarmTimeOrder = SqlHelper.Read<DateTime>(reader, "AlarmTimeOrder"),
                        Desire = SqlHelper.Read<int>(reader, "Desire"),
                        CreateUserId = SqlHelper.Read<int>(reader, "CreateUserId"),
                        CreateUserName = SqlHelper.Read<string>(reader, "CreateUserName"),
                        UpdateUserId = SqlHelper.Read<int>(reader, "UpdateUserId"),
                        UpdateUserName = SqlHelper.Read<string>(reader, "UpdateUserName"),
                        CreateDate = SqlHelper.Read<DateTime>(reader, "CreateDate"),
                        UpdateDate = SqlHelper.Read<DateTime>(reader, "UpdateDate"),
                        LastCare = SqlHelper.Read<DateTime>(reader, "LastCare")
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

        public override bool Update(CustomerBo obj)
        {
            int num = 0;
            var cmd = Connection.CreateCommand() as SqlCommand;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "Customer_Update";
            cmd.Parameters.AddWithValue("Id", obj.Id);
            cmd.Parameters.AddWithValue("AreaId", obj.AreaId);
            cmd.Parameters.AddWithValue("ProvinceId", obj.ProvinceId);
            cmd.Parameters.AddWithValue("CompanyId", obj.CompanyId);
            cmd.Parameters.AddWithValue("OfficeId", obj.OfficeId);
            cmd.Parameters.AddWithValue("Fullname", obj.Fullname);
            cmd.Parameters.AddWithValue("Gender", obj.Gender);
            cmd.Parameters.AddWithValue("Email", obj.Email);
            cmd.Parameters.AddWithValue("Phone", obj.Phone);
            cmd.Parameters.AddWithValue("Birthday", obj.Birthday != DateTime.MinValue ? obj.Birthday : null);
            cmd.Parameters.AddWithValue("Address", obj.Address);
            cmd.Parameters.AddWithValue("CustomerNote", obj.CustomerNote);
            cmd.Parameters.AddWithValue("AdvisoryNote ", obj.AdvisoryNote);
            cmd.Parameters.AddWithValue("EmployeeNote", obj.EmployeeNote);
            cmd.Parameters.AddWithValue("CountryId", obj.CountryId);
            cmd.Parameters.AddWithValue("EducationLevelId", obj.EducationLevelId);
            cmd.Parameters.AddWithValue("SearchInfo", obj.SearchInfo);
            cmd.Parameters.AddWithValue("CustomerType", obj.CustomerType);
            cmd.Parameters.AddWithValue("ProfileType", obj.ProfileType);
            cmd.Parameters.AddWithValue("NewsIdRef", obj.NewsIdRef);
            cmd.Parameters.AddWithValue("NewsUrlRef", obj.NewsUrlRef);
            cmd.Parameters.AddWithValue("UserIdRef", obj.UserIdRef);
            cmd.Parameters.AddWithValue("SerminarIdRef", obj.SeminarIdRef);
            cmd.Parameters.AddWithValue("SeminarNameRef", obj.SeminarNameRef);
            cmd.Parameters.AddWithValue("Source", obj.Source);
            cmd.Parameters.AddWithValue("SourceType", obj.SourceType);
            cmd.Parameters.AddWithValue("EmployeeId", obj.EmployeeId);
            cmd.Parameters.AddWithValue("EmployeeName", obj.EmployeeName);
            cmd.Parameters.AddWithValue("EmployeeProcessId", obj.EmployeeProcessId);
            cmd.Parameters.AddWithValue("EmployeeProcessName", obj.EmployeeProcessName);
            cmd.Parameters.AddWithValue("AgencyId", obj.AgencyId);
            cmd.Parameters.AddWithValue("AgencyName", obj.AgencyName);
            cmd.Parameters.AddWithValue("ProcessNote", obj.ProcessNote);
            cmd.Parameters.AddWithValue("Status", obj.Status);
            cmd.Parameters.AddWithValue("IsFly", obj.IsFly);
            cmd.Parameters.AddWithValue("IsCommission", obj.IsCommission);
            cmd.Parameters.AddWithValue("Desire", obj.Desire);
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
            cmd.CommandText = "Customer_Delete";
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

        public CustomerBo GetByEmail(string email, int companyId)
        {
            CustomerBo tmp = null;

            var cmd = Connection.CreateCommand() as SqlCommand;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "Customer_SelectByEmail";
            cmd.Parameters.Add(new SqlParameter("Email", email));
            cmd.Parameters.Add(new SqlParameter("CompanyId", companyId));

            try
            {
                Connection.Open();

                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    tmp = new CustomerBo
                    {
                        Id = SqlHelper.Read<int>(reader, "Id"),
                        AreaId = SqlHelper.Read<int>(reader, "AreaId"),
                        ProvinceId = SqlHelper.Read<int>(reader, "ProvinceId"),
                        CompanyId = SqlHelper.Read<int>(reader, "CompanyId"),
                        OfficeId = SqlHelper.Read<int>(reader, "OfficeId"),
                        Fullname = SqlHelper.Read<string>(reader, "Fullname"),
                        Gender = SqlHelper.Read<int>(reader, "Gender"),
                        Email = SqlHelper.Read<string>(reader, "Email"),
                        Phone = SqlHelper.Read<string>(reader, "Phone"),
                        Birthday = SqlHelper.Read<DateTime>(reader, "Birthday"),
                        Address = SqlHelper.Read<string>(reader, "Address"),
                        CustomerNote = SqlHelper.Read<string>(reader, "CustomerNote"),
                        AdvisoryNote = SqlHelper.Read<string>(reader, "AdvisoryNote"),
                        EmployeeNote = SqlHelper.Read<string>(reader, "EmployeeNote"),
                        CountryId = SqlHelper.Read<string>(reader, "CountryId"),
                        CountryName = SqlHelper.Read<string>(reader, "CountryName"),
                        AbroadTime = SqlHelper.Read<string>(reader, "AbroadTime"),
                        EducationLevelId = SqlHelper.Read<string>(reader, "EducationLevelId"),
                        SearchInfo = SqlHelper.Read<string>(reader, "SearchInfo"),
                        CustomerType = SqlHelper.Read<int>(reader, "CustomerType"),
                        ProfileType = SqlHelper.Read<int>(reader, "ProfileType"),
                        NewsIdRef = SqlHelper.Read<int>(reader, "NewsIdRef"),
                        NewsUrlRef = SqlHelper.Read<string>(reader, "NewsUrlRef"),
                        UserIdRef = SqlHelper.Read<int>(reader, "UserIdRef"),
                        SeminarIdRef = SqlHelper.Read<int>(reader, "SerminarIdRef"),
                        SeminarNameRef = SqlHelper.Read<string>(reader, "SeminarNameRef"),
                        Source = SqlHelper.Read<int>(reader, "Source"),
                        SourceType = SqlHelper.Read<int>(reader, "SourceType"),
                        EmployeeId = SqlHelper.Read<int>(reader, "EmployeeId"),
                        EmployeeName = SqlHelper.Read<string>(reader, "EmployeeName"),
                        EmployeeProcessId = SqlHelper.Read<int>(reader, "EmployeeProcessId"),
                        EmployeeProcessName = SqlHelper.Read<string>(reader, "EmployeeProcessName"),
                        AgencyId = SqlHelper.Read<int>(reader, "AgencyId"),
                        AgencyName = SqlHelper.Read<string>(reader, "AgencyName"),
                        ProcessNote = SqlHelper.Read<string>(reader, "ProcessNote"),
                        Status = SqlHelper.Read<int>(reader, "Status"),
                        IsFly = SqlHelper.Read<bool>(reader, "IsFly"),
                        IsCommission = SqlHelper.Read<bool>(reader, "IsCommission"),
                        IsAlarm = SqlHelper.Read<bool>(reader, "IsAlarm"),
                        AlarmTime = SqlHelper.Read<DateTime>(reader, "AlarmTime"),
                        AlarmTimeOrder = SqlHelper.Read<DateTime>(reader, "AlarmTimeOrder"),
                        Desire = SqlHelper.Read<int>(reader, "Desire"),
                        CreateUserId = SqlHelper.Read<int>(reader, "CreateUserId"),
                        CreateUserName = SqlHelper.Read<string>(reader, "CreateUserName"),
                        UpdateUserId = SqlHelper.Read<int>(reader, "UpdateUserId"),
                        UpdateUserName = SqlHelper.Read<string>(reader, "UpdateUserName"),
                        CreateDate = SqlHelper.Read<DateTime>(reader, "CreateDate"),
                        UpdateDate = SqlHelper.Read<DateTime>(reader, "UpdateDate"),
                        LastCare = SqlHelper.Read<DateTime>(reader, "LastCare")
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

        public CustomerBo GetByPhone(string phone, int companyId)
        {
            CustomerBo tmp = null;
            var cmd = Connection.CreateCommand() as SqlCommand;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "Customer_SelectByPhone";
            cmd.Parameters.Add(new SqlParameter("PhoneNumber", phone));
            cmd.Parameters.Add(new SqlParameter("CompanyId", companyId));

            try
            {
                Connection.Open();

                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    tmp = new CustomerBo
                    {
                        Id = SqlHelper.Read<int>(reader, "Id"),
                        AreaId = SqlHelper.Read<int>(reader, "AreaId"),
                        ProvinceId = SqlHelper.Read<int>(reader, "ProvinceId"),
                        CompanyId = SqlHelper.Read<int>(reader, "CompanyId"),
                        OfficeId = SqlHelper.Read<int>(reader, "OfficeId"),
                        Fullname = SqlHelper.Read<string>(reader, "Fullname"),
                        Gender = SqlHelper.Read<int>(reader, "Gender"),
                        Email = SqlHelper.Read<string>(reader, "Email"),
                        Phone = SqlHelper.Read<string>(reader, "Phone"),
                        Birthday = SqlHelper.Read<DateTime>(reader, "Birthday"),
                        Address = SqlHelper.Read<string>(reader, "Address"),
                        CustomerNote = SqlHelper.Read<string>(reader, "CustomerNote"),
                        AdvisoryNote = SqlHelper.Read<string>(reader, "AdvisoryNote"),
                        EmployeeNote = SqlHelper.Read<string>(reader, "EmployeeNote"),
                        CountryId = SqlHelper.Read<string>(reader, "CountryId"),
                        CountryName = SqlHelper.Read<string>(reader, "CountryName"),
                        AbroadTime = SqlHelper.Read<string>(reader, "AbroadTime"),
                        EducationLevelId = SqlHelper.Read<string>(reader, "EducationLevelId"),
                        SearchInfo = SqlHelper.Read<string>(reader, "SearchInfo"),
                        CustomerType = SqlHelper.Read<int>(reader, "CustomerType"),
                        ProfileType = SqlHelper.Read<int>(reader, "ProfileType"),
                        NewsIdRef = SqlHelper.Read<int>(reader, "NewsIdRef"),
                        NewsUrlRef = SqlHelper.Read<string>(reader, "NewsUrlRef"),
                        UserIdRef = SqlHelper.Read<int>(reader, "UserIdRef"),
                        SeminarIdRef = SqlHelper.Read<int>(reader, "SerminarIdRef"),
                        SeminarNameRef = SqlHelper.Read<string>(reader, "SeminarNameRef"),
                        Source = SqlHelper.Read<int>(reader, "Source"),
                        SourceType = SqlHelper.Read<int>(reader, "SourceType"),
                        EmployeeId = SqlHelper.Read<int>(reader, "EmployeeId"),
                        EmployeeName = SqlHelper.Read<string>(reader, "EmployeeName"),
                        EmployeeProcessId = SqlHelper.Read<int>(reader, "EmployeeProcessId"),
                        EmployeeProcessName = SqlHelper.Read<string>(reader, "EmployeeProcessName"),
                        ProcessNote = SqlHelper.Read<string>(reader, "ProcessNote"),
                        Status = SqlHelper.Read<int>(reader, "Status"),
                        IsFly = SqlHelper.Read<bool>(reader, "IsFly"),
                        IsCommission = SqlHelper.Read<bool>(reader, "IsCommission"),
                        IsAlarm = SqlHelper.Read<bool>(reader, "IsAlarm"),
                        AlarmTime = SqlHelper.Read<DateTime>(reader, "AlarmTime"),
                        AlarmTimeOrder = SqlHelper.Read<DateTime>(reader, "AlarmTimeOrder"),
                        Desire = SqlHelper.Read<int>(reader, "Desire"),
                        CreateUserId = SqlHelper.Read<int>(reader, "CreateUserId"),
                        CreateUserName = SqlHelper.Read<string>(reader, "CreateUserName"),
                        UpdateUserId = SqlHelper.Read<int>(reader, "UpdateUserId"),
                        UpdateUserName = SqlHelper.Read<string>(reader, "UpdateUserName"),
                        CreateDate = SqlHelper.Read<DateTime>(reader, "CreateDate"),
                        UpdateDate = SqlHelper.Read<DateTime>(reader, "UpdateDate"),
                        LastCare = SqlHelper.Read<DateTime>(reader, "LastCare")
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

        public bool UpdateAlarmTime(int id, bool isAlarm, DateTime? alarmTime, int userId, string userName)
        {
            int num = 0;
            var cmd = Connection.CreateCommand() as SqlCommand;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "Customer_UpdateAlarmTime";
            cmd.Parameters.AddWithValue("Id", id);
            cmd.Parameters.AddWithValue("IsAlarm", isAlarm ? 1 : 0);
            cmd.Parameters.AddWithValue("AlarmTime", alarmTime != null && alarmTime.HasValue ? alarmTime.Value : ConfigMgr.DefaultDate);
            cmd.Parameters.AddWithValue("UpdateUserId", userId);
            cmd.Parameters.AddWithValue("UpdateUserName", userName);

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

        public bool UpdateLastCare(int id, DateTime? lastCare)
        {
            int num = 0;
            var cmd = Connection.CreateCommand() as SqlCommand;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "Customer_UpdateLastCare";
            cmd.Parameters.AddWithValue("Id", id);
            cmd.Parameters.AddWithValue("LastCare", lastCare);

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

        public bool UpdateCompany(string ids, int companyId, int userId, string userName)
        {
            int num = 0;
            var cmd = Connection.CreateCommand() as SqlCommand;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "Customer_UpdateCompany";
            cmd.Parameters.AddWithValue("Ids", ids);
            cmd.Parameters.AddWithValue("CompanyId", companyId);
            cmd.Parameters.AddWithValue("UpdateUserId", userId);
            cmd.Parameters.AddWithValue("UpdateUserName", userName);

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

        public bool UpdateEmployee(string ids, int employeeId, string employeeName, int userId, string userName)
        {
            int num = 0;
            var cmd = Connection.CreateCommand() as SqlCommand;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "Customer_UpdateEmployee";
            cmd.Parameters.AddWithValue("Ids", ids);
            cmd.Parameters.AddWithValue("EmployeeId", employeeId);
            cmd.Parameters.AddWithValue("EmployeeName", employeeName);
            cmd.Parameters.AddWithValue("UpdateUserId", userId);
            cmd.Parameters.AddWithValue("UpdateUserName", userName);

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

        public bool UpdateCountryLevel(int id, string countries, string level, string countryNames, string abroadTimes)
        {
            int num = 0;
            var cmd = Connection.CreateCommand() as SqlCommand;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "Customer_UpdateCountryLevel";
            cmd.Parameters.AddWithValue("Id", id);
            cmd.Parameters.AddWithValue("CountryId", countries);
            cmd.Parameters.AddWithValue("LevelId", level);
            cmd.Parameters.AddWithValue("CountryName", countryNames);
            cmd.Parameters.AddWithValue("AbroadTime", abroadTimes);

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

        public List<CustomerBo> GetByListId(string ids)
        {
            var ret = new List<CustomerBo>();
            var cmd = Connection.CreateCommand() as SqlCommand;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "Customer_GetByListId";
            cmd.Parameters.Add(new SqlParameter("Ids", ids));

            try
            {
                Connection.Open();

                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var tmp = new CustomerBo
                    {
                        Id = SqlHelper.Read<int>(reader, "Id"),
                        AreaId = SqlHelper.Read<int>(reader, "AreaId"),
                        ProvinceId = SqlHelper.Read<int>(reader, "ProvinceId"),
                        CompanyId = SqlHelper.Read<int>(reader, "CompanyId"),
                        OfficeId = SqlHelper.Read<int>(reader, "OfficeId"),
                        Fullname = SqlHelper.Read<string>(reader, "Fullname"),
                        Gender = SqlHelper.Read<int>(reader, "Gender"),
                        Email = SqlHelper.Read<string>(reader, "Email"),
                        Phone = SqlHelper.Read<string>(reader, "Phone"),
                        Birthday = SqlHelper.Read<DateTime>(reader, "Birthday"),
                        Address = SqlHelper.Read<string>(reader, "Address"),
                        CustomerNote = SqlHelper.Read<string>(reader, "CustomerNote"),
                        AdvisoryNote = SqlHelper.Read<string>(reader, "AdvisoryNote"),
                        EmployeeNote = SqlHelper.Read<string>(reader, "EmployeeNote"),
                        CountryId = SqlHelper.Read<string>(reader, "CountryId"),
                        CountryName = SqlHelper.Read<string>(reader, "CountryName"),
                        AbroadTime = SqlHelper.Read<string>(reader, "AbroadTime"),
                        EducationLevelId = SqlHelper.Read<string>(reader, "EducationLevelId"),
                        SearchInfo = SqlHelper.Read<string>(reader, "SearchInfo"),
                        CustomerType = SqlHelper.Read<int>(reader, "CustomerType"),
                        ProfileType = SqlHelper.Read<int>(reader, "ProfileType"),
                        NewsIdRef = SqlHelper.Read<int>(reader, "NewsIdRef"),
                        NewsUrlRef = SqlHelper.Read<string>(reader, "NewsUrlRef"),
                        UserIdRef = SqlHelper.Read<int>(reader, "UserIdRef"),
                        SeminarIdRef = SqlHelper.Read<int>(reader, "SerminarIdRef"),
                        SeminarNameRef = SqlHelper.Read<string>(reader, "SeminarNameRef"),
                        Source = SqlHelper.Read<int>(reader, "Source"),
                        SourceType = SqlHelper.Read<int>(reader, "SourceType"),
                        EmployeeId = SqlHelper.Read<int>(reader, "EmployeeId"),
                        EmployeeName = SqlHelper.Read<string>(reader, "EmployeeName"),
                        EmployeeProcessId = SqlHelper.Read<int>(reader, "EmployeeProcessId"),
                        EmployeeProcessName = SqlHelper.Read<string>(reader, "EmployeeProcessName"),
                        ProcessNote = SqlHelper.Read<string>(reader, "ProcessNote"),
                        Status = SqlHelper.Read<int>(reader, "Status"),
                        IsFly = SqlHelper.Read<bool>(reader, "IsFly"),
                        IsCommission = SqlHelper.Read<bool>(reader, "IsCommission"),
                        IsAlarm = SqlHelper.Read<bool>(reader, "IsAlarm"),
                        AlarmTime = SqlHelper.Read<DateTime>(reader, "AlarmTime"),
                        AlarmTimeOrder = SqlHelper.Read<DateTime>(reader, "AlarmTimeOrder"),
                        Desire = SqlHelper.Read<int>(reader, "Desire"),
                        CreateUserId = SqlHelper.Read<int>(reader, "CreateUserId"),
                        CreateUserName = SqlHelper.Read<string>(reader, "CreateUserName"),
                        UpdateUserId = SqlHelper.Read<int>(reader, "UpdateUserId"),
                        UpdateUserName = SqlHelper.Read<string>(reader, "UpdateUserName"),
                        CreateDate = SqlHelper.Read<DateTime>(reader, "CreateDate"),
                        UpdateDate = SqlHelper.Read<DateTime>(reader, "UpdateDate"),
                        LastCare = SqlHelper.Read<DateTime>(reader, "LastCare")
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

        public bool UpdateCreateDate(int id, DateTime createDate, DateTime updateDate)
        {
            int num = 0;
            var cmd = Connection.CreateCommand() as SqlCommand;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "Customer_UpdateCreateDate";
            cmd.Parameters.AddWithValue("Id", id);
            cmd.Parameters.AddWithValue("CreateDate", createDate);
            cmd.Parameters.AddWithValue("UpdateDate", updateDate);

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

        public List<CustomerStatusReportData> GetStatusReportMonthly(DateTime from, DateTime to, int companyId)
        {
            var ret = new List<CustomerStatusReportData>();
            var cmd = Connection.CreateCommand() as SqlCommand;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "Customer_StatusReportMonthly";
            cmd.Parameters.Add(new SqlParameter("FromDate", from));
            cmd.Parameters.Add(new SqlParameter("ToDate", to));
            cmd.Parameters.Add(new SqlParameter("CompanyId", companyId));

            try
            {
                Connection.Open();

                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    ret.Add(new CustomerStatusReportData()
                    {
                        Date = SqlHelper.Read<DateTime>(reader, "Date"),
                        Total = SqlHelper.Read<int>(reader, "Total"),
                        TotalContinueCare = SqlHelper.Read<int>(reader, "TotalContinueCare"),
                        TotalPotential = SqlHelper.Read<int>(reader, "TotalPotential"),
                        TotalNotPotential = SqlHelper.Read<int>(reader, "TotalNotPotential"),
                        TotalMaybeContract = SqlHelper.Read<int>(reader, "TotalMaybeContract"),
                        TotalContract = SqlHelper.Read<int>(reader, "TotalContract")
                    });
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

        public List<StatusReport> GetStatusSummary(DateTime from, DateTime to, int companyId, int officeId, string employeeIds)
        {
            var ret = new List<StatusReport>();
            var cmd = Connection.CreateCommand() as SqlCommand;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "Customer_StatusSummary";
            cmd.Parameters.Add(new SqlParameter("FromDate", from));
            cmd.Parameters.Add(new SqlParameter("ToDate", to));
            cmd.Parameters.Add(new SqlParameter("CompanyId", companyId));
            cmd.Parameters.Add(new SqlParameter("OfficeId", officeId));
            cmd.Parameters.Add(new SqlParameter("EmployeeIds", employeeIds));

            try
            {
                Connection.Open();

                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    ret.Add(new StatusReport()
                    {
                        Total = SqlHelper.Read<int>(reader, "Total"),
                        Status = SqlHelper.Read<int>(reader, "Status")
                    });
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

        public List<CountryReport> GetCountrySummary(DateTime from, DateTime to, int companyId, int officeId, string employeeIds)
        {
            var ret = new List<CountryReport>();
            var cmd = Connection.CreateCommand() as SqlCommand;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "Customer_CountrySummary";
            cmd.Parameters.Add(new SqlParameter("FromDate", from));
            cmd.Parameters.Add(new SqlParameter("ToDate", to));
            cmd.Parameters.Add(new SqlParameter("CompanyId", companyId));
            cmd.Parameters.Add(new SqlParameter("OfficeId", officeId));
            cmd.Parameters.Add(new SqlParameter("EmployeeIds", employeeIds));

            try
            {
                Connection.Open();

                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    ret.Add(new CountryReport()
                    {
                        Total = SqlHelper.Read<int>(reader, "Total"),
                        CountryId = SqlHelper.Read<string>(reader, "CountryId")
                    });
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

        public List<StatusReport> GetStatusDaily(DateTime from, DateTime to, int companyId, int officeId, string employeeIds)
        {
            var ret = new List<StatusReport>();
            var cmd = Connection.CreateCommand() as SqlCommand;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "Customer_StatusDaily";
            cmd.Parameters.Add(new SqlParameter("FromDate", from));
            cmd.Parameters.Add(new SqlParameter("ToDate", to));
            cmd.Parameters.Add(new SqlParameter("CompanyId", companyId));
            cmd.Parameters.Add(new SqlParameter("OfficeId", officeId));
            cmd.Parameters.Add(new SqlParameter("EmployeeIds", employeeIds));

            try
            {
                Connection.Open();

                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    ret.Add(new StatusReport()
                    {
                        Total = SqlHelper.Read<int>(reader, "Total"),
                        Status = SqlHelper.Read<int>(reader, "Status"),
                        Date = SqlHelper.Read<string>(reader, "Date")
                    });
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
