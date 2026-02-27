
using System;
using System.Collections.Generic;
using System.Linq;
using tcs.adapter.Elastics;
using tcs.adapter.Helper;
using tcs.bo;
using tcs.dao;
using tcs.lib;

namespace tcs.adapter.Sql
{
    public class CustomerDb
    {
        private static CustomerDb _instance;

        public static CustomerDb Instance => _instance ?? (_instance = new CustomerDb());

        public int Create(CustomerBo obj, bool isIndex = false)
        {
            var ret = CustomerSql.Instance.Create(obj);
            if(ret > 0 && isIndex)
                SEIndex(ret);
            return ret;
        }

        public bool Update(CustomerBo obj, bool isIndex = false)
        {
            var ret = CustomerSql.Instance.Update(obj);

            if(isIndex)
                SEIndex(obj.Id);

            return ret;
        }

        public bool UpdateLastCare(int id, DateTime? lastCare, bool isIndex = false)
        {
            var ret = CustomerSql.Instance.UpdateLastCare(id, lastCare);

            if (isIndex)
                SEIndex(id);

            return ret;
        }

        public bool Delete(string ids, int companyId, int userId, string userName, bool isIndex = false)
        {
            var ret = CustomerSql.Instance.Delete(ids, companyId, userId, userName);
            if (isIndex)
                SEIndex(ids, ElasticAction.Delete);
            return ret;
        }

        public CustomerBo Read(int id)
        {
            return CustomerSql.Instance.Read(id);
        }

        public CustomerBo GetByEmail(string email, int companyId)
        {
            return CustomerSql.Instance.GetByEmail(email, companyId);
        }

        public CustomerBo GetByPhone(string phone, int companyId)
        {
            phone = phone.Replace("-", "");
            return CustomerSql.Instance.GetByPhone(phone, companyId);
        }

        public List<CustomerBo> Select(CustomerQuery query = null)
        {
            if (query == null)
                query = new CustomerQuery();

            //var cacheKey = Cacher.CreateCacheKey(query);
            //var cacheTotal = cacheKey + "_total";
            //var result = Cacher.Get<List<CustomerBo>>(cacheKey);
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
                var status = !string.IsNullOrEmpty(query.Status) ? query.Status.SplitToIntArr() : new int[] { };
                var country = !string.IsNullOrEmpty(query.Country) ? query.Country.SplitToIntArr() : new int[] { };
                var source = !string.IsNullOrEmpty(query.Source) ? query.Source.SplitToIntArr() : new int[] { };
                var sourceType = !string.IsNullOrEmpty(query.SourceType) ? query.SourceType.SplitToIntArr() : new int[] { };
                var educationLevel = !string.IsNullOrEmpty(query.EducationLevel) ? query.EducationLevel.SplitToIntArr() : new int[] { };
                var employee = !query.Employee.ToLower().Equals("admin") ? query.Employee.SplitToIntArr() : new int[] { };
                var agency = !string.IsNullOrEmpty(query.Agency) ? query.Agency.SplitToIntArr() : new int[] { };

                var lstId = CustomerSearch.Instance.Search(query.Keyword, company, office,
                    query.From, query.To, status, country, employee, employee, source,
                    sourceType, educationLevel, agency, out total, out outStatus, out outCountry,
                    query.Page, query.PageSize, false, query.Sort, query.IsAgency);
                if (lstId != null && lstId.Any())
                {
                    query.TotalRecord = (int)total;
                    var tmp = string.Join(",", lstId.ToArray());
                    var result = CustomerSql.Instance.GetByListId(tmp);
                    if (result != null)
                    {
                        result = result.OrderBy(i => lstId.IndexOf(i.Id)).ToList();
                        //Cacher.Add(cacheTotal, (int)total);
                        //Cacher.Add(cacheKey, result);
                        return result;
                    }
                }
                return null;
            }

            var lstCustomer = CustomerSql.Instance.Select(query);
            if(lstCustomer != null)
            {
                //Cacher.Add(cacheTotal, query.TotalRecord);
                //Cacher.Add(cacheKey, lstCustomer);
                return lstCustomer;
            }
            return null;
        }

        public bool UpdateAlarmTime(int id, bool isAlarm, DateTime? alarmTime, int userId, string userName)
        {
            var ret = CustomerSql.Instance.UpdateAlarmTime(id, isAlarm, alarmTime, userId, userName);
            CustomerSearch.Instance.Update(id.ToString(), new
            {
                AlarmTime = alarmTime.Value,
                UpdateDate = DateTime.Now
            });
            return ret;
        }

        public bool UpdateCompany(string ids, int companyId, int userId, string userName)
        {
            var ret = CustomerSql.Instance.UpdateCompany(ids, companyId, userId, userName);
            if (ret)
            {
                var lstId = ids.SplitToIntArr();
                foreach (var id in lstId)
                {
                    CustomerSearch.Instance.Update(id.ToString(), new
                    {
                        CompanyId = companyId
                    });
                }
            }
            return ret;
        }

        public bool UpdateEmployee(string ids, int employeeId, string employeeName, int userId, string userName)
        {
            var ret = CustomerSql.Instance.UpdateEmployee(ids, employeeId, employeeName, userId, userName);
            if(ret)
            {
                var lstId = ids.SplitToIntArr();
                foreach (var id in lstId)
                {
                    CustomerSearch.Instance.Update(id.ToString(), new
                    {
                        EmployeeId = employeeId,
                        EmployeeName = employeeName
                    });
                }
            }
            return ret;
        }

        /// <summary>
        /// Cập nhật lại thông tin quốc gia và bậc học KH đang quan tâm
        /// </summary>
        /// <param name="id"></param>
        /// <param name="countries"></param>
        /// <param name="levels"></param>
        /// <returns></returns>
        public bool UpdateCountryLevel(int id, string countries, string levels, string countryNames, string abroadTimes)
        {
            try
            {
                var ret = CustomerSql.Instance.UpdateCountryLevel(id, countries, levels, countryNames, abroadTimes);
                var countryIds = countries.SplitToIntArr();
                var levelIds = levels.SplitToIntArr();
                CustomerSearch.Instance.Update(id.ToString(), new
                {
                    CountryId = countryIds,
                    EducationLevelId = levelIds
                });
                return ret;
            }
            catch (Exception ex)
            {
                LogHelper.Error("CustomerDb.UpdateCountryLevel", ex);
            }
            return false;
        }

        /// <summary>
        /// Hàm xử lý cập nhật thông tin khách hàng lên ElasticSearch
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public bool SEIndex(string ids, ElasticAction action = ElasticAction.Update)
        {
            // TODO: đẩy danh sách KH cần cập nhật vào DB để cập nhật lại trên elastics
            // có 2 loại: 1 là cập nhật toàn bộ thông tin KH, 2 là chỉ cập nhật 1 vài thông tin nào đó
            // có thể dùng dynamic object để truyền dữ liệu dạng json

            if(action == ElasticAction.Delete)
            {
                return CustomerSearch.Instance.DeleteMany(ids);
            }

            return true;
        }

        public bool SEIndex(int id, ElasticAction action = ElasticAction.Update)
        {
            return false;

            try
            {
                if (action == ElasticAction.Delete)
                    return CustomerSearch.Instance.Delete(id);

                var cus = Read(id);
                if (cus == null)
                    return false;

                return CustomerSearch.Instance.Index(ToCustomerSE(cus));
            }
            catch (Exception ex)
            {
                LogHelper.Error("CustomerDb.SEIndex" + ex.Message, ex);
            }

            return false;
        }

        public CustomerSE ToCustomerSE(CustomerBo cus)
        {
            var countries = new int[] { };
            var educationLevel = new int[] { };
            var searchInfo = "";
            var searchInfoEn = "";
            var parentInfo = "";
            var parentInfoEn = "";
            var phoneList = new List<string>();
            
            try
            {
                if (!string.IsNullOrEmpty(cus.CountryId))
                {
                    countries = cus.CountryId.SplitToIntArr();
                }

                if (!string.IsNullOrEmpty(cus.EducationLevelId))
                {
                    educationLevel = cus.EducationLevelId.SplitToIntArr();
                }
            }
            catch
            {
            }
            
            var lstParent = ParentDb.Instance.GetByCustomer(cus.Id);
            if(lstParent != null && lstParent.Any())
            {
                foreach (var parent in lstParent)
                {
                    parentInfo += " " + parent.Name.ToSearchInfo();
                    if (!string.IsNullOrEmpty(parent.Phone))
                    {
                        parentInfo += " " + parent.Phone;
                        if(!phoneList.Contains(parent.Phone))
                            phoneList.Add(parent.Phone);
                    }
                    if (!string.IsNullOrEmpty(parent.Email))
                        parentInfo += " " + parent.Email.ToSearchInfo();

                    parentInfoEn += " " + parent.Name.ToUnsignedVietnamese().ToSearchInfo();
                }
            }

            // lấy thông tin văn phòng theo thông tin vị trí được phân quyền
            var officeId = 0;
            if(cus.EmployeeId > 0)
            {
                var title = CompanyTitleDb.Instance.GetByCompany(cus.CompanyId);
                if(title != null && title.Any(t=>t.Id==cus.EmployeeId))
                {
                    officeId = title.FirstOrDefault(t => t.Id == cus.EmployeeId).OfficeId;
                }
            }

            searchInfo = cus.Fullname.ToSearchInfo();
            searchInfoEn = cus.Fullname.ToUnsignedVietnamese().ToSearchInfo();
            if (!string.IsNullOrEmpty(cus.Phone))
            {
                searchInfo += " " + cus.Phone;
                if (!phoneList.Contains(cus.Phone))
                    phoneList.Add(cus.Phone);
            }
            if (!string.IsNullOrEmpty(cus.Email))
                searchInfo += " " + cus.Email.ToSearchInfo();

            searchInfo += " " + parentInfo;
            searchInfoEn += " " + parentInfoEn;
            var setAlarmTime = false;
            if (ViewHelper.IsContinueCare(cus.Status))
            {
                if (cus.AlarmTime != null && cus.AlarmTime.HasValue && cus.AlarmTime.Value != DateTime.MinValue && cus.AlarmTime.Value != ConfigMgr.DefaultDate)
                    setAlarmTime = true;
            }

            //var relatives = RelativesDb.Instance.GetByCustomer(cus.Id);

            var model = new CustomerSE()
            {
                Id = cus.Id,
                CompanyId = cus.CompanyId,
                OfficeId = officeId,
                SearchInfo = searchInfo,
                SearchInfoEn = searchInfoEn,
                Birthday = cus.Birthday.HasValue ? cus.Birthday.Value : DateTime.MinValue,
                ProvinceId = cus.ProvinceId,
                Address = cus.Address,
                CountryId = countries != null && countries.Any() ? countries : null,
                EducationLevelId = educationLevel != null && educationLevel.Any() ? educationLevel : null,
                EmployeeId = cus.EmployeeId,
                EmployeeName = cus.EmployeeName,
                EmployeeProcessId = cus.EmployeeProcessId,
                EmployeeProcessName = cus.EmployeeProcessName,
                AgencyId = cus.AgencyId,
                AgencyName = cus.AgencyName,
                Source = cus.Source,
                SourceType = cus.SourceType,
                NewsId = cus.NewsIdRef,
                NewsUrl = cus.NewsUrlRef,
                SeminarId = cus.SeminarIdRef,
                SerminarName = cus.SeminarNameRef,
                CustomerType = cus.CustomerType,
                ProfileType = cus.ProfileType,
                UserIdRef = cus.UserIdRef,
                Status = cus.Status,
                AlarmTime = setAlarmTime ? cus.AlarmTime.Value : ConfigMgr.DefaultAlarmDate,
                CreateDate = cus.CreateDate.Value,
                UpdateDate = cus.UpdateDate.HasValue ? cus.UpdateDate.Value : DateTime.MinValue,
                CreateUserId = cus.CreateUserId,
                CreateUserName = cus.CreateUserName,
                LastCare = cus.LastCare.HasValue ? cus.LastCare.Value : DateTime.MinValue,
                PhoneList = phoneList != null && phoneList.Any() ? phoneList.ToArray() : null
            };

            return model;
        }

        /// <summary>
        /// Chỉ dùng khi move dữ liệu
        /// </summary>
        /// <param name="id"></param>
        /// <param name="createDate"></param>
        /// <param name="updateDate"></param>
        /// <returns></returns>
        public bool UpdateCreateDate(int id, DateTime createDate, DateTime updateDate)
        {
            return CustomerSql.Instance.UpdateCreateDate(id, createDate, updateDate);
        }

        public CustomerExcel ToCustomerExcel(CustomerBo cus)
        {
            var model = new CustomerExcel()
            {
                Id = cus.Id,
                FullName = cus.Fullname,
                Phone = cus.Phone,
                Email = cus.Email,
                Employee = cus.EmployeeName,
                Status = CustomerStatus.Instant().GetValueByKey(cus.Status),
                Source = CustomerSource.Instant().GetValueByKey(cus.Source),
                CreateDate = cus.CreateDate.Value.ToDateString()
            };

            return model;
        }

        /// <summary>
        /// Lấy DSKH đến hẹn chăm sóc hôm nay
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public List<CustomerBo> GetCustomerNotify(CustomerQuery query = null)
        {
            if (query == null)
                query = new CustomerQuery();

            var total = 0L;
            var outStatus = new Dictionary<int, int>();
            var outCountry = new Dictionary<int, int>();
            var company = !string.IsNullOrEmpty(query.Company) ? query.Company.SplitToIntArr() : new int[] { };
            var office = !string.IsNullOrEmpty(query.Office) ? query.Office.SplitToIntArr() : new int[] { };
            var status = !string.IsNullOrEmpty(query.Status) ? query.Status.SplitToIntArr() : new int[] { };
            var employee = !query.Employee.ToLower().Equals("admin") ? query.Employee.SplitToIntArr() : new int[] { };

            var lstId = CustomerSearch.Instance.GetCustomerNotify(company, office, employee, status, out total, query.From, query.To,
                            query.Page, query.PageSize, query.Sort);
            if (lstId != null && lstId.Any())
            {
                query.TotalRecord = (int)total;
                var tmp = string.Join(",", lstId.ToArray());
                var result = CustomerSql.Instance.GetByListId(tmp);
                if (result != null)
                {
                    result = result.OrderBy(i => lstId.IndexOf(i.Id)).ToList();
                    return result;
                }
            }
            return null;
        }

        public List<CustomerStatusReportData> GetStatusReportMonthly(DateTime from, DateTime to, int companyId)
        {
            return CustomerSql.Instance.GetStatusReportMonthly(from, to, companyId);
        }

        public List<StatusReport> GetStatusSummary(DateTime from, DateTime to, int companyId, int officeId, string employeeIds)
        {
            return CustomerSql.Instance.GetStatusSummary(from, to, companyId, officeId, employeeIds);
        }

        public List<CountryReport> GetCountrySummary(DateTime from, DateTime to, int companyId, int officeId, string employeeIds)
        {
            return CustomerSql.Instance.GetCountrySummary(from, to, companyId, officeId, employeeIds);
        }

        public List<StatusReport> GetStatusDaily(DateTime from, DateTime to, int companyId, int officeId, string employeeIds)
        {
            return CustomerSql.Instance.GetStatusDaily(from, to, companyId, officeId, employeeIds);
        }
    }
}
