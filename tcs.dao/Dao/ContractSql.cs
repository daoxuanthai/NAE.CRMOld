using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using tcs.bo;
using tcs.lib;

namespace tcs.dao
{
    public class ContractSql : IDbConnect<ContractBo>
    {
        private static ContractSql _instance;

        public static ContractSql Instance =>
            _instance ?? (_instance = new ContractSql(new SqlConnection(ConfigMgr.ConnectionString)));

        public ContractSql(SqlConnection connection) : base(connection) { }

        public override List<ContractBo> Select(IQuery query)
        {
            var qr = query as ContractQuery;
            var ret = new List<ContractBo>();
            var cmd = Connection.CreateCommand() as SqlCommand;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "Contract_Search";
            cmd.Parameters.Add(new SqlParameter("Keyword", qr.Keyword));
            cmd.Parameters.Add(new SqlParameter("CompanyID", qr.Company));
            cmd.Parameters.Add(new SqlParameter("Status", qr.Status));
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
                    var tmp = new ContractBo
                    {
                        Id = SqlHelper.Read<int>(reader, "Id"),
                        CustomerId = SqlHelper.Read<int>(reader, "CustomerId"),
                        CompanyId = SqlHelper.Read<int>(reader, "CompanyId"),
                        SchoolId = SqlHelper.Read<int>(reader, "SchoolId"),
                        ScholarshipId = SqlHelper.Read<int>(reader, "ScholarshipId"),
                        ContractDate = SqlHelper.Read<DateTime>(reader, "ContractDate"),
                        VisaDate = SqlHelper.Read<DateTime>(reader, "VisaDate"),
                        Status = SqlHelper.Read<int>(reader, "Status"),
                        IsVisa = SqlHelper.Read<bool>(reader, "IsVisa"),
                        Currency = SqlHelper.Read<string>(reader, "Currency"),
                        IsRefund = SqlHelper.Read<bool>(reader, "IsRefund"),
                        RefundDate = SqlHelper.Read<DateTime>(reader, "RefundDate"),
                        Promotion = SqlHelper.Read<string>(reader, "Promotion"),
                        AttachFile = SqlHelper.Read<string>(reader, "AttachFile"),
                        Note = SqlHelper.Read<string>(reader, "Note"),
                        ProcessNote = SqlHelper.Read<string>(reader, "ProcessNote"),
                        EmployeeId = SqlHelper.Read<int>(reader, "EmployeeId"),
                        EmployeeName = SqlHelper.Read<string>(reader, "EmployeeName"),
                        PercentProcessing = SqlHelper.Read<int>(reader, "PercentProcessing"),
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

        public override int Create(ContractBo obj)
        {
            var id = 0;
            var cmd = Connection.CreateCommand() as SqlCommand;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "Contract_Insert";
            cmd.Parameters.AddWithValue("CustomerId", obj.CustomerId);
            cmd.Parameters.AddWithValue("CompanyId", obj.CompanyId);
            cmd.Parameters.AddWithValue("ProfileTypeId", obj.ProfileTypeId);
            cmd.Parameters.AddWithValue("SchoolId", obj.SchoolId);
            cmd.Parameters.AddWithValue("SchoolName", obj.SchoolName);
            cmd.Parameters.AddWithValue("ScholarshipId", obj.ScholarshipId);
            cmd.Parameters.AddWithValue("ScholarshipName", obj.ScholarshipName);
            cmd.Parameters.AddWithValue("ContractDate", obj.ContractDate != DateTime.MinValue ? obj.ContractDate : null);
            cmd.Parameters.AddWithValue("VisaDate", obj.VisaDate != DateTime.MinValue ? obj.VisaDate : null);
            cmd.Parameters.AddWithValue("Status", obj.Status);
            cmd.Parameters.AddWithValue("IsVisa", obj.IsVisa);
            cmd.Parameters.AddWithValue("Deposit", obj.Deposit);
            cmd.Parameters.AddWithValue("ServiceFee", obj.ServiceFee);
            cmd.Parameters.AddWithValue("CollectOne", obj.CollectOne);
            cmd.Parameters.AddWithValue("CollectTwo", obj.CollectTwo);
            cmd.Parameters.AddWithValue("Currency", obj.Currency);
            cmd.Parameters.AddWithValue("IsRefund", obj.IsRefund);
            cmd.Parameters.AddWithValue("RefundDate", obj.RefundDate != DateTime.MinValue ? obj.RefundDate : null);
            cmd.Parameters.AddWithValue("Promotion", obj.Promotion);
            cmd.Parameters.AddWithValue("AttachFile", obj.AttachFile);
            cmd.Parameters.AddWithValue("Note", obj.Note);
            cmd.Parameters.AddWithValue("ProcessNote", obj.ProcessNote);
            cmd.Parameters.AddWithValue("EmployeeId", obj.EmployeeId);
            cmd.Parameters.AddWithValue("EmployeeName", obj.EmployeeName);
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

        public override ContractBo Read(int id)
        {
            ContractBo tmp = null;
            var cmd = Connection.CreateCommand() as SqlCommand;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "Contract_Select";
            cmd.Parameters.Add(new SqlParameter("ID", id));

            try
            {
                Connection.Open();

                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    tmp = new ContractBo
                    {
                        Id = SqlHelper.Read<int>(reader, "Id"),
                        CustomerId = SqlHelper.Read<int>(reader, "CustomerId"),
                        CompanyId = SqlHelper.Read<int>(reader, "CompanyId"),
                        ProfileTypeId = SqlHelper.Read<int>(reader, "ProfileTypeId"),
                        SchoolId = SqlHelper.Read<int>(reader, "SchoolId"),
                        ScholarshipId = SqlHelper.Read<int>(reader, "ScholarshipId"),
                        ContractDate = SqlHelper.Read<DateTime>(reader, "ContractDate"),
                        VisaDate = SqlHelper.Read<DateTime>(reader, "VisaDate"),
                        Status = SqlHelper.Read<int>(reader, "Status"),
                        IsVisa = SqlHelper.Read<bool>(reader, "IsVisa"),
                        Deposit = SqlHelper.Read<decimal>(reader, "Deposit"),
                        ServiceFee = SqlHelper.Read<decimal>(reader, "ServiceFee"),
                        CollectOne = SqlHelper.Read<decimal>(reader, "CollectOne"),
                        CollectTwo = SqlHelper.Read<decimal>(reader, "CollectTwo"),
                        Currency = SqlHelper.Read<string>(reader, "Currency"),
                        IsRefund = SqlHelper.Read<bool>(reader, "IsRefund"),
                        RefundDate = SqlHelper.Read<DateTime>(reader, "RefundDate"),
                        Promotion = SqlHelper.Read<string>(reader, "Promotion"),
                        AttachFile = SqlHelper.Read<string>(reader, "AttachFile"),
                        Note = SqlHelper.Read<string>(reader, "Note"),
                        ProcessNote = SqlHelper.Read<string>(reader, "ProcessNote"),
                        EmployeeId = SqlHelper.Read<int>(reader, "EmployeeId"),
                        EmployeeName = SqlHelper.Read<string>(reader, "EmployeeName"),
                        PercentProcessing = SqlHelper.Read<int>(reader, "PercentProcessing"),
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

        public override bool Update(ContractBo obj)
        {
            int num = 0;
            var cmd = Connection.CreateCommand() as SqlCommand;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "Contract_Update";
            cmd.Parameters.AddWithValue("Id", obj.Id);
            cmd.Parameters.AddWithValue("CustomerId", obj.CustomerId);
            cmd.Parameters.AddWithValue("ProfileTypeId", obj.ProfileTypeId);
            cmd.Parameters.AddWithValue("SchoolId", obj.SchoolId);
            cmd.Parameters.AddWithValue("SchoolName", obj.SchoolName);
            cmd.Parameters.AddWithValue("ScholarshipId", obj.ScholarshipId);
            cmd.Parameters.AddWithValue("ScholarshipName", obj.ScholarshipName);
            cmd.Parameters.AddWithValue("ContractDate", obj.ContractDate != DateTime.MinValue ? obj.ContractDate : null);
            cmd.Parameters.AddWithValue("VisaDate", obj.VisaDate != DateTime.MinValue ? obj.VisaDate : null);
            cmd.Parameters.AddWithValue("Status", obj.Status);
            cmd.Parameters.AddWithValue("IsVisa", obj.IsVisa);
            cmd.Parameters.AddWithValue("Deposit", obj.Deposit);
            cmd.Parameters.AddWithValue("ServiceFee", obj.ServiceFee);
            cmd.Parameters.AddWithValue("CollectOne", obj.CollectOne);
            cmd.Parameters.AddWithValue("CollectTwo", obj.CollectTwo);
            cmd.Parameters.AddWithValue("Currency", obj.Currency);
            cmd.Parameters.AddWithValue("IsRefund", obj.IsRefund);
            cmd.Parameters.AddWithValue("RefundDate", obj.RefundDate != DateTime.MinValue ? obj.RefundDate : null);
            cmd.Parameters.AddWithValue("Promotion", obj.Promotion);
            cmd.Parameters.AddWithValue("AttachFile", obj.AttachFile);
            cmd.Parameters.AddWithValue("Note", obj.Note);
            cmd.Parameters.AddWithValue("ProcessNote", obj.ProcessNote);
            cmd.Parameters.AddWithValue("EmployeeId", obj.EmployeeId);
            cmd.Parameters.AddWithValue("EmployeeName", obj.EmployeeName);

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

        public bool UpdatePercentProcessing(ContractBo obj)
        {
            int num = 0;
            var cmd = Connection.CreateCommand() as SqlCommand;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "Contract_UpdatePercentProcessing";
            cmd.Parameters.AddWithValue("Id", obj.Id);
            cmd.Parameters.AddWithValue("PercentProcessing", obj.PercentProcessing);

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
            cmd.CommandText = "Contract_Delete";
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

        public ContractBo GetByCustomer(int customerId)
        {
            ContractBo tmp = null;
            var cmd = Connection.CreateCommand() as SqlCommand;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "Contract_SelectByCustomer";
            cmd.Parameters.Add(new SqlParameter("CustomerID", customerId));

            try
            {
                Connection.Open();

                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    tmp = new ContractBo
                    {
                        Id = SqlHelper.Read<int>(reader, "Id"),
                        CustomerId = SqlHelper.Read<int>(reader, "CustomerId"),
                        CompanyId = SqlHelper.Read<int>(reader, "CompanyId"),
                        ProfileTypeId = SqlHelper.Read<int>(reader, "ProfileTypeId"),
                        SchoolId = SqlHelper.Read<int>(reader, "SchoolId"),
                        ScholarshipId = SqlHelper.Read<int>(reader, "ScholarshipId"),
                        ContractDate = SqlHelper.Read<DateTime>(reader, "ContractDate"),
                        VisaDate = SqlHelper.Read<DateTime>(reader, "VisaDate"),
                        Status = SqlHelper.Read<int>(reader, "Status"),
                        IsVisa = SqlHelper.Read<bool>(reader, "IsVisa"),
                        Deposit = SqlHelper.Read<decimal>(reader, "Deposit"),
                        ServiceFee = SqlHelper.Read<decimal>(reader, "ServiceFee"),
                        CollectOne = SqlHelper.Read<decimal>(reader, "CollectOne"),
                        CollectTwo = SqlHelper.Read<decimal>(reader, "CollectTwo"),
                        Currency = SqlHelper.Read<string>(reader, "Currency"),
                        IsRefund = SqlHelper.Read<bool>(reader, "IsRefund"),
                        RefundDate = SqlHelper.Read<DateTime>(reader, "RefundDate"),
                        Promotion = SqlHelper.Read<string>(reader, "Promotion"),
                        AttachFile = SqlHelper.Read<string>(reader, "AttachFile"),
                        Note = SqlHelper.Read<string>(reader, "Note"),
                        ProcessNote = SqlHelper.Read<string>(reader, "ProcessNote"),
                        EmployeeId = SqlHelper.Read<int>(reader, "EmployeeId"),
                        EmployeeName = SqlHelper.Read<string>(reader, "EmployeeName"),
                        PercentProcessing = SqlHelper.Read<int>(reader, "PercentProcessing"),
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

        public List<ContractSummaryReportData> GetSummaryReportMonthly(DateTime from, DateTime to, int companyId)
        {
            var ret = new List<ContractSummaryReportData>();
            var cmd = Connection.CreateCommand() as SqlCommand;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "Contract_SummaryReportMonthly";
            cmd.Parameters.Add(new SqlParameter("FromDate", from));
            cmd.Parameters.Add(new SqlParameter("ToDate", to));
            cmd.Parameters.Add(new SqlParameter("CompanyId", companyId));

            try
            {
                Connection.Open();

                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    ret.Add(new ContractSummaryReportData()
                    {
                        Date = SqlHelper.Read<DateTime>(reader, "Date"),
                        Total = SqlHelper.Read<int>(reader, "TotalCustomer"),
                        TotalContract = SqlHelper.Read<int>(reader, "TotalContract"),
                        TotalNotContract = SqlHelper.Read<int>(reader, "TotalNotContract")
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
            cmd.CommandText = "Contract_StatusSummary";
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
            cmd.CommandText = "Contract_CountrySummary";
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
            cmd.CommandText = "Contract_StatusDaily";
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