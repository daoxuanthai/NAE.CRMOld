
using System;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;
using tcs.adapter.Elastics;
using tcs.adapter.Sql;
using tcs.bo;
using tcs.lib;
using VietFuture.CRM.BO;
using VietFuture.CRM.BO.BO;

namespace taiminh
{
    class Program
    {
        public static string oldConn = string.Empty;
        public static int CompanyId = 1000;
        public static DateTime DefaultDate = DateTime.Now;
        public static int CustomerSize = 2000;
        static void Main(string[] args)
        {
            oldConn = ConfigurationManager.ConnectionStrings["TMConnectionString"].ConnectionString;
            DefaultDate = new DateTime(1999, 1, 1);

            // tao moi thong tin khach hang
            // trước khi đồng bộ cập nhật lại CreateUserId vào field CreateUserName
            //CustomerSync();

            //UpdateCountry();

            IndexCustomer();

            //IndexContract();

            Console.Write("Success");
            Console.Read();
        }

        public static string GetCreateUserName(int id, ref int newId)
        {
            switch(id)
            {
                //1   admin
                case 1:
                    newId = 0;
                    return "admin";
                //16  mar1
                case 16:
                    newId = 0;
                    return "admin";
                //33  phie
                case 33:
                    newId = 0;
                    return "admin";
                //4   quantri
                case 4:
                    newId = 0;
                    return "admin";
                //11  tv1dn
                case 11:
                    newId = 1005;
                    return "ĐN-TV1";
                //9   tv1hcm
                case 9:
                    newId = 1002;
                    return "HCM-TV1";
                //22  tv1hp
                case 22:
                    newId = 1025;
                    return "HP-TV1";
                //12  tv2dn
                case 12:
                    newId = 1006;
                    return "ĐN-TV2";
                //10  tv2hcm
                case 10:
                    newId = 1003;
                    return "HCM-TV2";
                //13  tv3dn
                case 13:
                    newId = 1020;
                    return "ĐN-TV3";
                //29  tv3hcm
                case 29:
                    newId = 1012;
                    return "HCM-TV3";
                //14  tv4dn
                case 14:
                    newId = 1021;
                    return "ĐN-TV4";
                //25  tv6dn
                case 25:
                    newId = 1023;
                    return "Đn-TV6";
                //7   tvp_dn
                case 7:
                    newId = 1004;
                    return "ĐN-TVP";
                //19  tvp_hcm
                case 19:
                    newId = 1001;
                    return "HCM-TVP";
                //15  tvp_hp
                case 15:
                    newId = 1017;
                    return "HP-TVP";
                //18  tvp_qn
                case 18:
                    newId = 1018;
                    return "QNG-TVP";
            }
            newId = 0;
            return "Undefined";
        }

        /// <summary>
        /// Cập nhật lại thông tin quốc gia theo studyabroad
        /// </summary>
        public static void UpdateCountry()
        {
            var query = new CustomerQuery()
            {
                Keyword = "",
                From = new DateTime(2018, 1, 1),
                To = DateTime.Now.AddDays(1),
                Status = "-1",
                Country = "-1",
                Employee = "ADMIN",
                Company = "-1",
                Office = "-1",
                Source = "-1",
                SourceType = "-1",
                EducationLevel = "-1",
                Page = 0,
                PageSize = 2000,
                Sort = CustomerSort.CreateDate
            };
            var result = CustomerDb.Instance.Select(query);
            if (result != null)
            {
                var lstCountry = CountryCompanyDb.Instance.GetByCompany(1000);
                foreach (var cus in result)
                {
                    var lstStudyAbroad = StudyAbroadDb.Instance.GetByCustomer(cus.Id);
                    if(lstStudyAbroad != null && lstStudyAbroad.Any(s => s.CountryId > 0))
                    {
                        Console.WriteLine("Country.ID: " + cus.Id);
                        foreach (var item in lstStudyAbroad)
                        {
                            if(item.CountryId > 0)
                            {
                                var country = lstCountry.FirstOrDefault(c => c.Id == item.CountryId);
                                if (country != null)
                                    item.CountryName = country.CountryName;
                                item.UpdateUserId = 0;
                                item.UpdateUserName = "thaidx";
                                StudyAbroadDb.Instance.Update(item);
                            }
                        }
                        var arrCountry = lstStudyAbroad.Where(s => s.CountryId > 0).Select(s => s.CountryId).ToArray();
                        if(arrCountry.Any())
                            cus.CountryId = string.Join(",", arrCountry);
                    }

                    if (lstStudyAbroad != null && lstStudyAbroad.Any(s => s.Level > 0))
                    {
                        Console.WriteLine("Level.ID: " + cus.Id);
                        var arrLevel = lstStudyAbroad.Where(s => s.Level > 0).Select(s => s.Level).ToArray();
                        if (arrLevel.Any())
                            cus.EducationLevelId = string.Join(",", arrLevel);
                    }
                    cus.UpdateUserId = 0;
                    cus.UpdateUserName = "thaidx";
                    CustomerDb.Instance.Update(cus);
                }
            }
        }

        public static void IndexCustomer()
        {
            var query = new CustomerQuery()
            {
                Keyword = "",
                From = new DateTime(2018,1,1),
                To = DateTime.Now.AddDays(1),
                Status = "-1",
                Country = "-1",
                Employee = "ADMIN",
                Company = "-1",
                Office = "-1",
                Source = "-1",
                SourceType = "-1",
                EducationLevel = "-1",
                Page = 0,
                PageSize = 2000,
                Sort = CustomerSort.CreateDate
            };
            var result = CustomerDb.Instance.Select(query);
            if(result != null)
            {
                foreach (var cus in result)
                {
                    Console.WriteLine("ProcessIndex.ID: " + cus.Id);
                    //if(cus.Id == 1634)
                    //{
                        var lstCustomerCare = CustomerCareDb.Instance.GetByCustomer(cus.Id);
                        if (lstCustomerCare != null && lstCustomerCare.Any())
                        {
                            var newCare = lstCustomerCare.OrderByDescending(c => c.CreateDate).FirstOrDefault();
                            cus.AdvisoryNote = newCare.Advisory;
                            LogHelper.Error("UpdateCustomer: " + cus.Id + ", CareId: " + newCare.Id, "");
                            CustomerDb.Instance.Update(cus, false);
                        }
                    //}

                    //if (cus.Phone.Length == 11)
                    //{
                    //    var output = cus.Phone;
                    //    if (cus.Phone.StartsWith("84"))
                    //    {
                    //        output = Regex.Replace(cus.Phone, "^84", "0");
                    //    }
                    //    else if (cus.Phone.StartsWith("009"))
                    //    {
                    //        output = Regex.Replace(cus.Phone, "^009", "09");
                    //    }
                    //    else if (cus.Phone.StartsWith("008"))
                    //    {
                    //        output = Regex.Replace(cus.Phone, "^008", "08");
                    //    }
                    //    else
                    //    {
                    //        output = Regex.Replace(output, "^016", "03");
                    //        output = Regex.Replace(output, "^0120", "070");
                    //        output = Regex.Replace(output, "^0121", "079");
                    //        output = Regex.Replace(output, "^0122", "077");
                    //        output = Regex.Replace(output, "^0126", "076");
                    //        output = Regex.Replace(output, "^0128", "078");
                    //        output = Regex.Replace(output, "^0124", "084");
                    //        output = Regex.Replace(output, "^0127", "081");
                    //        output = Regex.Replace(output, "^0129", "082");
                    //        output = Regex.Replace(output, "^0123", "083");
                    //        output = Regex.Replace(output, "^0125", "085");
                    //        output = Regex.Replace(output, "^018", "05");
                    //        output = Regex.Replace(output, "^019", "05");
                    //    }

                    //    LogHelper.WriteLog("Id:" + cus.Id, "Old: " + cus.Phone + " - New: " + output);
                    //    cus.Phone = output;
                    //    cus.EmployeeId = cus.EmployeeProcessId;
                    //    CustomerDb.Instance.Update(cus);
                    //}

                    //CustomerSearch.Instance.Update(cus.Id.ToString(), new
                    //{
                    //    EmployeeId = 1017,
                    //    EmployeeName = "TVP-HP"
                    //});
                    //CustomerDb.Instance.SEIndex(cus.Id);
                    //var searchInfo = cus.Fullname.ToSearchInfo();
                    //var searchInfoEn = cus.Fullname.ToUnsignedVietnamese().ToSearchInfo();
                    //if (!string.IsNullOrEmpty(cus.Phone))
                    //    searchInfo += " " + cus.Phone;
                    //if (!string.IsNullOrEmpty(cus.Email))
                    //    searchInfo += " " + cus.Email.ToSearchInfo();

                    //CustomerSearch.Instance.Update(cus.Id.ToString(), new
                    //{
                    //    SearchInfo = searchInfo,
                    //    SearchInfoEn = searchInfoEn
                    //});
                }
                //var cus = result.FirstOrDefault(c => c.Id == 2335);
                //if (cus != null)
                //{
                //    var searchInfo = cus.Fullname;
                //    var searchInfoEn = cus.Fullname.ToUnsignedVietnamese();
                //    if (!string.IsNullOrEmpty(cus.Phone))
                //        searchInfo += " " + cus.Phone;
                //    if (!string.IsNullOrEmpty(cus.Email))
                //        searchInfo += " " + cus.Email;

                //    CustomerSearch.Instance.Update<CustomerSE>("2335", new
                //    {
                //        SearchInfo = searchInfo,
                //        SearchInfoEn = searchInfoEn
                //    });
                //}
            }
        }

        public static void IndexContract()
        {
            var query = new ContractQuery()
            {
                Keyword = "",
                From = DateTime.Now.AddYears(-1),
                To = DateTime.Now,
                Status = "-1",
                Page = 0,
                PageSize = 100,
                Company = "1000",
                Office = "-1",
                CountryId = "-1",
                EmployeeProcessId = "-1"
            };
            var lstContract = ContractDb.Instance.Search(query);
            if(lstContract != null)
            {
                foreach (var item in lstContract)
                {
                    Console.WriteLine("ProcessIndex.ID: " + item.Id);
                    ContractDb.Instance.SEIndex(item.Id);
                }
            }
        }

        public static void CustomerSync()
        {
            var bo = new CustomerBO(oldConn);
            var lstCustomer = bo.Search("", -1, DateTime.Now.AddYears(-1), DateTime.Now, 
                -1, 1, CustomerSize, "999", -1,"");
            if(lstCustomer != null && lstCustomer.Any())
            {
                foreach (var item in lstCustomer)
                {
                    if (item.IsDelete.Value || item.IsTemp.Value)
                        continue;

                    Console.WriteLine("ProcessCustomer.ID: " + item.ID);
                    var cus = ToNewCustomer(item);

                    // 1872: parent, history, abroad
                    // 1873: customerCare
                    if(cus != null)
                    {
                        var newBo = new CustomerDb();
                        var newId = newBo.Create(cus);
                        ProcessInfo(newId, item.ID, item.CreateDate, item.UpdateDate);
                    }
                }
            }
        }
        
        /// <summary>
        /// Xử lý thông tin khách hàng cũ qua hệ thống mới
        /// </summary>
        /// <param name="newId"></param>
        /// <param name="oldId"></param>
        /// <param name="createDate"></param>
        /// <param name="updateDate"></param>
        public static void ProcessInfo(int newId, int oldId, DateTime? createDate, DateTime? updateDate)
        {
            // cập nhật ngày tạo, ngày cập nhật
            CustomerDb.Instance.UpdateCreateDate(newId, createDate.Value, updateDate.HasValue ? updateDate.Value : createDate.Value);

            // thông tin hợp đồng
            Contract(newId, oldId);

            CustomerCare(newId, oldId);

            CustomerParent(newId, oldId);

            StudyHistory(newId, oldId);

            StudyAbroad(newId, oldId);

            Language(newId, oldId);

            Experience(newId, oldId);

            Financial(newId, oldId);
        }

        public static void Contract(int newId, int oldId)
        {
            try
            {
                var contractBo = new CRMContractBO(oldConn);
                var contract = contractBo.Select(oldId);
                if(contract != null)
                {
                    var newContract = new ContractBo()
                    {
                        CustomerId = newId,
                        CompanyId = CompanyId,
                        CollectOne = contract.CommissionTotal1 != null && contract.CommissionTotal1.HasValue ? contract.CommissionTotal1.Value : 0,
                        CollectTwo = contract.CommissionTotal2 != null && contract.CommissionTotal2.HasValue ? contract.CommissionTotal2.Value : 0,
                        ContractDate = contract.ContractDate != null && contract.ContractDate.HasValue ? contract.ContractDate : DefaultDate,
                        Deposit  = contract.DepositTotal != null && contract.DepositTotal.HasValue ? contract.DepositTotal.Value : 0,
                        IsRefund = contract.IsRefundDeposit != null && contract.IsRefundDeposit.HasValue ? contract.IsRefundDeposit.Value : false,
                        IsVisa = contract.IsVisa != null && contract.IsVisa.HasValue ? contract.IsVisa.Value : false,
                        VisaDate = contract.UpdateDate != null && contract.UpdateDate.HasValue ? contract.UpdateDate : DefaultDate,
                        ServiceFee = contract.ContractTotal != null && contract.ContractTotal.HasValue ? contract.ContractTotal.Value : 0,
                        RefundDate = contract.CreateDate.Value,
                        Status = contract.Status.Value,
                        Promotion = contract.Note,
                        CreateUserId = 0,
                        CreateUserName = contract.CreateUserName
                    };
                    ContractDb.Instance.Create(newContract);
                }
            }
            catch (Exception ex)
            {
                tcs.lib.LogHelper.WriteLog("Contract.Error: newId: " + newId + " - oldId: " + oldId, ex);
            }
        }

        public static void Financial(int newId, int oldId)
        {
            try
            {
                var financialBo = new FinancialBO(oldConn);
                var finans = financialBo.GetListFinancial(oldId);
                if (finans != null && finans.Any())
                {
                    foreach (var item in finans)
                    {
                        var info = new tcs.bo.FinancialBo()
                        {
                            CompanyId = CompanyId,
                            CustomerId = newId,
                            Income = item.Income,
                            Name = item.DisplayName,
                            Note = item.CreateDate.Value.ToString("yyyy-MM-dd HH:mm:ss"),
                            CreateUserId = 0,
                            CreateUserName = item.CreateUserName
                        };
                        FinancialDb.Instance.Create(info);
                    }
                }
            }
            catch (Exception ex)
            {
                tcs.lib.LogHelper.WriteLog("Financial.Error: newId: " + newId + " - oldId: " + oldId, ex);
            }
        }

        public static void Experience(int newId, int oldId)
        {
            try
            {
                var expBo = new ExperienceBO(oldConn);
                var exps = expBo.GetListExperiences(oldId);
                if (exps != null && exps.Any())
                {
                    foreach (var item in exps)
                    {
                        var info = new tcs.bo.ExperienceBo()
                        {
                            CompanyId = CompanyId,
                            CustomerId = newId,
                            Company = item.Company,
                            JobName = item.Position,
                            Year = item.YearCount != null && item.YearCount.HasValue ? item.YearCount.Value : 0,
                            OffDate = item.EndDate != null && item.EndDate.HasValue ? item.EndDate.Value : DefaultDate,
                            CreateUserId = 0,
                            CreateUserName = item.CreateUserName
                        };
                        ExperienceDb.Instance.Create(info);
                    }
                }
            }
            catch (Exception ex)
            {
                tcs.lib.LogHelper.WriteLog("Experience.Error: newId: " + newId + " - oldId: " + oldId, ex);
            }
        }

        /// <summary>
        /// StudyHistory: 0: history, 1: quan tâm, 2: ngoại ngữ, Note = Ngày tạo
        /// </summary>
        /// <param name="newId"></param>
        /// <param name="oldId"></param>
        public static void Language(int newId, int oldId)
        {
            try
            {
                var historyBo = new StudyHistoryBO(oldConn);
                var historys = historyBo.GetListStudyHistories(oldId, 2);
                if (historys != null && historys.Any())
                {
                    foreach (var item in historys)
                    {
                        var info = new tcs.bo.LanguageBo()
                        {
                            CompanyId = CompanyId,
                            CustomerId = newId,
                            Language = item.StudyType != null && item.StudyType.HasValue ? item.StudyType.Value.ToString() : "",
                            Score = item.Score,
                            RetestDate = item.RetestDate != null && item.RetestDate.HasValue ? item.RetestDate : DefaultDate,
                            Note = item.CreateDate.Value.ToString("yyyy-MM-dd HH:mm:ss"),
                            CreateUserId = 0,
                            CreateUserName = item.CreateUserName
                        };
                        LanguageDb.Instance.Create(info);
                    }
                }
            }
            catch (Exception ex)
            {
                tcs.lib.LogHelper.WriteLog("Language.Error: newId: " + newId + " - oldId: " + oldId, ex);
            }
        }

        /// <summary>
        /// StudyHistory: 0: history, 1: quan tâm, 2: ngoại ngữ, Note = Ngày tạo
        /// </summary>
        /// <param name="newId"></param>
        /// <param name="oldId"></param>
        public static void StudyAbroad(int newId, int oldId)
        {
            try
            {
                var historyBo = new StudyHistoryBO(oldConn);
                var historys = historyBo.GetListStudyHistories(oldId, 1);
                if (historys != null && historys.Any())
                {
                    foreach (var item in historys)
                    {
                        var info = new tcs.bo.StudyAbroadBo()
                        {
                            CompanyId = CompanyId,
                            CustomerId = newId,
                            CountryId = item.CountryID != null && item.CountryID.HasValue ? item.CountryID.Value : 0,
                            Major = item.MajorsName,
                            School = item.SchoolName,
                            Level = item.StudyLevel != null && item.StudyLevel.HasValue ? item.StudyLevel.Value : 0,
                            Year = item.StudyAbroadTime != null && item.StudyAbroadTime.HasValue ? item.StudyAbroadTime.Value : -1,
                            Note = item.CreateDate.Value.ToString("yyyy-MM-dd HH:mm:ss"),
                            CreateUserId = 0,
                            CreateUserName = item.CreateUserName
                        };
                        StudyAbroadDb.Instance.Create(info);
                    }
                }
            }
            catch (Exception ex)
            {
                tcs.lib.LogHelper.WriteLog("StudyAbroad.Error: newId: " + newId + " - oldId: " + oldId, ex);
            }
        }

        /// <summary>
        /// StudyHistory: 0: history, 1: quan tâm, 2: ngoại ngữ, Note = Ngày tạo
        /// </summary>
        /// <param name="newId"></param>
        /// <param name="oldId"></param>
        public static void StudyHistory(int newId, int oldId)
        {
            try
            {
                var historyBo = new StudyHistoryBO(oldConn);
                var historys = historyBo.GetListStudyHistories(oldId, 0);
                if (historys != null && historys.Any())
                {
                    foreach (var item in historys)
                    {
                        var info = new tcs.bo.StudyHistoryBo()
                        {
                            CompanyId = CompanyId,
                            CustomerId = newId,
                            GraduateDate = item.GraduationYear !=null && item.GraduationYear.HasValue ? new DateTime(item.GraduationYear.Value,1,1) : DefaultDate,
                            Major = item.MajorsName,
                            School = item.SchoolName,
                            Score = item.Score,
                            Note = item.CreateDate.Value.ToString("yyyy-MM-dd HH:mm:ss"),
                            CreateUserId = 0,
                            CreateUserName = item.CreateUserName
                        };
                        StudyHistoryDb.Instance.Create(info);
                    }
                }
            }
            catch (Exception ex)
            {
                tcs.lib.LogHelper.WriteLog("StudyHistory.Error: newId: " + newId + " - oldId: " + oldId, ex);
            }
        }

        public static void CustomerParent(int newId, int oldId)
        {
            try
            {
                var parentBo = new ParentBO(oldConn);
                var parents = parentBo.GetListParents(oldId);
                if (parents != null && parents.Any())
                {
                    foreach (var item in parents)
                    {
                        var info = new tcs.bo.ParentBo()
                        {
                            CompanyId = CompanyId,
                            CustomerId = newId,
                            Name = item.DisplayName,
                            Phone = item.Phone,
                            Email = item.Email,
                            Birthday = item.CreateDate.Value,
                            CreateUserId = 0,
                            CreateUserName = item.CreateUserName
                        };
                        ParentDb.Instance.Create(info);
                    }
                }
            }
            catch (Exception ex)
            {
                tcs.lib.LogHelper.WriteLog("CustomerParent.Error: newId: " + newId + " - oldId: " + oldId, ex);
            }
        }

        public static void CustomerCare(int newId, int oldId)
        {
            try
            {
                var careBo = new CRMCustomerCareBO(oldConn);
                var customerCare = careBo.Search(oldId, true);
                if (customerCare != null && customerCare.Any())
                {
                    foreach (var item in customerCare)
                    {
                        var info = new tcs.bo.CustomerCareBo()
                        {
                            CompanyId = CompanyId,
                            CustomerId = newId,
                            Advisory = item.CustomerCareNote,
                            AlarmTime = item.CreateDate.Value,
                            CreateUserId = item.CreateUserID != null && item.CreateUserID.HasValue ? item.CreateUserID.Value : 0,
                            CreateUserName = item.CreateUserName
                        };
                        CustomerCareDb.Instance.Create(info);
                    }
                }
            }
            catch (Exception ex)
            {
                tcs.lib.LogHelper.WriteLog("CustomerCare.Error: newId: " + newId + " - oldId: " + oldId, ex);
            }
        }

        public static tcs.bo.CustomerBo ToNewCustomer(Customer_SearchResult cus)
        {
            if (cus == null)
                return null;

            var status = 0;
            switch(cus.Status.Value)
            {
                case 1:
                    status = 3;
                    break;
                case 2:
                    status = 1;
                    break;
                case 3:
                    status = 2;
                    break;
                case 5:
                    status = 4;
                    break;
                case 6:
                    status = 5;
                    break;
                default:
                    status = 0;
                    break;
            }

            var createUser = Convert.ToInt32(cus.CreateUserName);
            var newCreateUser = 0;
            var newCreateUserName = GetCreateUserName(createUser, ref newCreateUser);

            return new tcs.bo.CustomerBo() {
                Fullname = cus.DisplayName,
                Address = cus.Address,
                AdvisoryNote = cus.AdvisoryNote,
                AlarmTime = cus.AlarmTime,
                Birthday = cus.Birthday,
                CompanyId = CompanyId,
                OfficeId = 0,
                CustomerNote = cus.CustomerNote,
                //CreateUserName = cus.CreateUserName,
                Email = cus.Email,
                EmployeeId = Convert.ToInt32(cus.EmployeeID),
                EmployeeName = cus.EmployeeName,
                EmployeeNote = cus.EmployeeNote,
                Gender = cus.Gender != null && cus.Gender.HasValue ? cus.Gender.Value : -1,
                IsAlarm = cus.IsAlarm != null && cus.IsAlarm.HasValue ? cus.IsAlarm.Value : false,
                IsCommission = cus.IsCommission != null && cus.IsCommission.HasValue ? cus.IsCommission.Value : false,
                IsFly = cus.IsFly != null && cus.IsFly.HasValue ? cus.IsFly.Value : false,
                NewsUrlRef = cus.NewsReferer,
                Phone = cus.Phone,
                ProvinceId = cus.ProvinceID != null && cus.ProvinceID.HasValue ? cus.ProvinceID.Value : -1,
                Source = Convert.ToInt32(cus.Source),
                SourceType = 0,
                Status = status,
                // luu thong tin ID cu vao day
                EmployeeProcessId = cus.ID,
                CreateUserId = newCreateUser,
                CreateUserName = newCreateUserName
            };
        }

    }
}
