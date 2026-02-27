
using System;
using System.Collections.Generic;
using System.Linq;
using tcs.adapter.Elastics;
using tcs.bo;
using tcs.dao;
using tcs.lib;

namespace tcs.adapter.Sql
{
    public class ContractDb
    {
        private static ContractDb _instance;

        public static ContractDb Instance => _instance ?? (_instance = new ContractDb());

        public int Create(ContractBo obj)
        {
            var id = ContractSql.Instance.Create(obj);
            //if (id > 0)
            //    SEIndex(id);
            return id;
        }

        public bool Update(ContractBo obj)
        {
            var ret = ContractSql.Instance.Update(obj);
            //if (ret)
            //    SEIndex(obj.Id);
            return ret;
        }

        /// <summary>
        /// Cập nhật lại phần trăm xử lý hồ sơ
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool UpdatePercentProcessing(ContractBo obj)
        {
            var ret = ContractSql.Instance.UpdatePercentProcessing(obj);
            //if (ret)
            //    SEIndex(obj.Id);
            return ret;
        }

        public bool Delete(string ids, int companyId, int userId, string userName)
        {
            var ret = ContractSql.Instance.Delete(ids, companyId, userId, userName);
            //if (ret)
            //    SEIndex(ids, ElasticAction.Delete);
            return ret;
        }

        public ContractBo Read(int id)
        {
            return ContractSql.Instance.Read(id);
        }

        public List<ContractBo> Select(ContractQuery query = null)
        {
            if (query == null)
                query = new ContractQuery();

            return ContractSql.Instance.Select(query);
        }

        public ContractBo GetByCustomer(int customerId)
        {
            return ContractSql.Instance.GetByCustomer(customerId);
        }

        public List<ContractSE> Search(ContractQuery query = null)
        {
            if (query == null)
                query = new ContractQuery();

            var cacheKey = Cacher.CreateCacheKey(query);
            var cacheTotal = cacheKey + "_total";
            var result = Cacher.Get<List<ContractSE>>(cacheKey);
            //if (result != null)
            //{
            //    query.TotalRecord = Cacher.Get<int>(cacheTotal);
            //    return result;
            //}

            if (ConfigMgr.IsElastic)
            {
                var total = 0L;
                var outStatus = new Dictionary<int, int>();
                var outCountry = new Dictionary<int, int>();
                var company = !string.IsNullOrEmpty(query.Company) ? query.Company.SplitToIntArr() : new int[] { };
                var office = !string.IsNullOrEmpty(query.Office) ? query.Office.SplitToIntArr() : new int[] { };
                var employee = !string.IsNullOrEmpty(query.EmployeeProcessId) ? query.EmployeeProcessId.SplitToIntArr() : new int[] { };
                var status = !string.IsNullOrEmpty(query.Status) ? query.Status.SplitToIntArr() : new int[] { };
                var country = !string.IsNullOrEmpty(query.CountryId) ? query.CountryId.SplitToIntArr() : new int[] { };

                var contracts = ContractSearch.Instance.Search(query.Keyword, company, office,
                    employee, country, status, query.From, query.To, out total, out outStatus,
                    out outCountry, query.Page, query.PageSize);

                if (contracts != null && contracts.Any())
                {
                    query.TotalRecord = (int)total;
                    //Cacher.Add(cacheTotal, (int)total);
                    //Cacher.Add(cacheKey, contracts);
                }

                return contracts;
            }

            var lstContract = ContractSql.Instance.Select(query);
            if (lstContract != null && lstContract.Any())
            {
                var tmp = lstContract.Select(c => ToContractSE(c)).ToList();
                //Cacher.Add(cacheTotal, (int)query.TotalRecord);
                //Cacher.Add(cacheKey, tmp);
                return tmp;
            }
            return null;
        }

        public bool SEIndex(string ids, ElasticAction action = ElasticAction.Update)
        {
            if(action == ElasticAction.Delete)
            {
                return ContractSearch.Instance.DeleteMany(ids);
            }
            
            return true;
        }

        public bool SEIndex(int id, ElasticAction action = ElasticAction.Update)
        {


            if (action == ElasticAction.Delete)
                return CustomerSearch.Instance.Delete(id);

            var con = Read(id);
            if (con == null)
                return false;

            return ContractSearch.Instance.Index(ToContractSE(con));
        }

        private ContractSE ToContractSE(ContractBo con)
        {
            if (con == null)
                return null;

            var cus = CustomerDb.Instance.Read(con.CustomerId);
            if (cus == null)
                return null;

            var searchInfo = cus.Fullname.ToSearchInfo();
            var searchInfoEn = cus.Fullname.ToUnsignedVietnamese().ToSearchInfo();
            if (!string.IsNullOrEmpty(cus.Phone))
                searchInfo += " " + cus.Phone;
            if (!string.IsNullOrEmpty(cus.Email))
                searchInfo += " " + cus.Email.ToSearchInfo();

            var contract = new ContractSE()
            {
                Id = con.Id,
                CustomerId = con.CustomerId,
                CompanyId = cus.CompanyId,
                OfficeId = cus.OfficeId,
                CountryId = string.IsNullOrWhiteSpace(cus.CountryId) ? null : cus.CountryId.SplitToIntArr(),
                ProvinceId = cus.ProvinceId,
                FullName = cus.Fullname,
                Phone = cus.Phone,
                Email = cus.Email,
                SearchInfo = searchInfo,
                SearchInfoEn = searchInfoEn,
                EmployeeId = cus.EmployeeId,
                EmployeeProcessId = con.EmployeeId,
                EmployeeProcessName = con.EmployeeName,
                Status = con.Status,
                Deposit = con.Deposit,
                ServiceFee = con.ServiceFee,
                CollectOne = con.CollectOne,
                CollectTwo = con.CollectTwo,
                IsRefund = con.IsRefund,
                RefundDate = con.RefundDate.HasValue ? con.RefundDate.Value : DateTime.MinValue,
                ContractDate = con.ContractDate.HasValue ? con.ContractDate.Value : DateTime.MinValue,
                IsVisa = con.IsVisa,
                VisaDate = con.VisaDate.HasValue ? con.VisaDate.Value : DateTime.MinValue,
                TotalCommission = con.TotalCommission,
                CreateDate = con.CreateDate.Value,
                UpdateDate = con.UpdateDate.Value,
                NewsIdRef = cus.NewsIdRef,
                NewsUrlRef = cus.NewsUrlRef,
                SeminarIdRef = cus.SeminarIdRef,
                UserIdRef = cus.UserIdRef,
                ProfileTypeId = con.ProfileTypeId,
                PercentProcessing = con.PercentProcessing
            };
            return contract;
        }

        public List<ContractSummaryReportData> GetSummaryReportMonthly(DateTime from, DateTime to, int companyId)
        {
            return ContractSql.Instance.GetSummaryReportMonthly(from, to, companyId);
        }

        public List<StatusReport> GetContractSummary(DateTime from, DateTime to, int companyId, int officeId, string employeeIds)
        {
            return ContractSql.Instance.GetStatusSummary(from, to, companyId, officeId, employeeIds);
        }

        public List<CountryReport> GetCountrySummary(DateTime from, DateTime to, int companyId, int officeId, string employeeIds)
        {
            return ContractSql.Instance.GetCountrySummary(from, to, companyId, officeId, employeeIds);
        }

        public List<StatusReport> GetStatusDaily(DateTime from, DateTime to, int companyId, int officeId, string employeeIds)
        {
            return ContractSql.Instance.GetStatusDaily(from, to, companyId, officeId, employeeIds);
        }
    }
}
