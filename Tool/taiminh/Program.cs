
using System;
using System.Configuration;
using System.Linq;
using tcs.adapter.Elastics;
using tcs.adapter.Sql;
using tcs.bo;
using tcs.dao;
using tcs.lib;

namespace taiminh
{
    class Program
    {
        public static string oldConn = string.Empty;
        public static int CompanyId = 1005;
        public static DateTime DefaultDate = DateTime.Now;
        public static int CustomerSize = 2000;
        static void Main(string[] args)
        {
            oldConn = ConfigurationManager.ConnectionStrings["SqlConnectionString"].ConnectionString;
            DefaultDate = new DateTime(1999, 1, 1);

            //CustomerSync();

            //UpdateCountry();

            IndexCustomer();

            //DeleteCustomer();

            //IndexContract();

            //CloneDocument();

            //IndexSeminarRegister(1002);

            Console.Write("Success");
            Console.Read();
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
                Company = "1005",
                Office = "-1",
                Source = "-1",
                SourceType = "-1",
                EducationLevel = "-1",
                Page = 0,
                PageSize = 5000,
                Sort = CustomerSort.CreateDate.Key
            };
            var result = CustomerDb.Instance.Select(query);
            if (result != null)
            {
                var lstCountry = CountryCompanyDb.Instance.GetByCompany(1000);
                foreach (var cus in result)
                {
                    var lstStudyAbroad = StudyAbroadDb.Instance.GetByCustomer(cus.Id);
                    if (lstStudyAbroad != null && lstStudyAbroad.Any(s => s.CountryId > 0))
                    {
                        Console.WriteLine("Country.ID: " + cus.Id);
                        foreach (var item in lstStudyAbroad)
                        {
                            if (item.CountryId > 0)
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
                        if (arrCountry.Any())
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
                From = new DateTime(2018, 1, 1),
                To = DateTime.Now.AddDays(1),
                Status = "-1",
                Country = "-1",
                Employee = "ADMIN",
                Company = "1005",
                Office = "-1",
                Source = "-1",
                SourceType = "-1",
                EducationLevel = "-1",
                Page = 0,
                PageSize = 7000,
                Sort = CustomerSort.CreateDate.Key
            };
            var result = CustomerDb.Instance.Select(query);
            //var result = CustomerSql.Instance.GetByListId("13601");
            if (result != null)
            {
                var index = 1;
                foreach (var cus in result)
                {
                    Console.WriteLine((index++).ToString() + ".ProcessIndex.ID: " + cus.Id);
                    CustomerDb.Instance.SEIndex(cus.Id);

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
            }
        }

        public static void DeleteCustomer()
        {
            var query = new CustomerQuery()
            {
                Keyword = "",
                From = new DateTime(2018, 1, 1),
                To = DateTime.Now.AddDays(1),
                Status = "-1",
                Country = "-1",
                Employee = "1056",
                Company = "1005",
                Office = "-1",
                Source = "-1",
                SourceType = "-1",
                EducationLevel = "-1",
                Page = 0,
                PageSize = 2000,
                Sort = CustomerSort.CreateDate.Key
            };
            var result = CustomerDb.Instance.Select(query);
            //var result = CustomerSql.Instance.GetByListId("5444");
            if (result != null)
            {
                var index = 1;
                foreach (var cus in result)
                {
                    Console.WriteLine((index++).ToString() + ".ProcessIndex.ID: " + cus.Id);
                    CustomerDb.Instance.Delete(cus.Id.ToString(), 1005, 99, "thaidx");
                }
            }
        }

        public static void UpdateCountryAbroadTime()
        {
            var query = new CustomerQuery()
            {
                Keyword = "",
                From = new DateTime(2018, 1, 1),
                To = DateTime.Now.AddDays(1),
                Status = "-1",
                Country = "-1",
                Employee = "ADMIN",
                Company = "1000",
                Office = "-1",
                Source = "-1",
                SourceType = "-1",
                EducationLevel = "-1",
                Page = 0,
                PageSize = 3000,
                Sort = CustomerSort.CreateDate.Key
            };
            var result = CustomerDb.Instance.Select(query);
            //var result = CustomerSql.Instance.GetByListId("5444");
            if (result != null)
            {
                foreach (var cus in result)
                {
                    Console.WriteLine("ProcessIndex.ID: " + cus.Id);

                    //var lstCare = CustomerCareDb.Instance.GetByCustomer(cus.Id);
                    //if(lstCare != null && lstCare.Any())
                    //{
                    //    lstCare = lstCare.OrderByDescending(c => c.CreateDate).ToList();
                    //    CustomerDb.Instance.UpdateLastCare(cus.Id, lstCare.FirstOrDefault().CreateDate);
                    //}

                    var lstStudyAbroad = StudyAbroadDb.Instance.GetByCustomer(cus.Id);

                    // cập nhật lại thông tin quốc gia và bậc học trong customer
                    if (lstStudyAbroad != null && lstStudyAbroad.Any())
                    {
                        var countries = lstStudyAbroad.Select(s => s.CountryId).ToArray();
                        var levels = lstStudyAbroad.Select(s => s.Level).ToArray();
                        var times = lstStudyAbroad.Select(s => s.Year).ToArray();
                        var countryNames = lstStudyAbroad.Select(s => s.CountryName).ToArray();
                        CustomerDb.Instance.UpdateCountryLevel(cus.Id, string.Join(",", countries), string.Join(",", levels),
                            string.Join(",", countryNames), string.Join(",", times));
                    }
                }
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
                PageSize = 300,
                Company = "1000",
                Office = "-1",
                CountryId = "-1",
                EmployeeProcessId = "-1"
            };
            var lstContract = ContractDb.Instance.Search(query);
            if (lstContract != null)
            {
                foreach (var item in lstContract)
                {
                    if (item == null)
                        continue;

                    Console.WriteLine("ProcessIndex.ID: " + item.Id);
                    try
                    {
                        ContractDb.Instance.SEIndex(item.Id);
                    }
                    catch (Exception)
                    {

                    }

                }
            }
        }

        public static void CloneDocument()
        {
            var types = ProfileTypeDb.Instance.GetByCompany(1000);
            if (types != null && types.Any())
            {
                foreach (var t in types)
                {
                    // tạo type mới
                    t.CompanyId = 1005;
                    t.CreateUserName = "thaidx";
                    t.CreateUserId = 1;
                    var typeId = ProfileTypeDb.Instance.Create(t);

                    var steps = ProfileStepDb.Instance.GetByProfileType(t.Id);
                    if (steps != null && steps.Any())
                    {
                        foreach (var s in steps)
                        {
                            // tạo step mới
                            s.ProfileTypeId = typeId;
                            s.CreateUserName = "thaidx";
                            s.CreateUserId = 1;
                            var stepId = ProfileStepDb.Instance.Create(s);

                            var docs = ProfileDocumentDb.Instance.GetByProfileType(t.Id);
                            if (docs != null && docs.Any())
                            {
                                foreach (var d in docs)
                                {
                                    // profileTypeId
                                    d.ProfileTypeId = typeId;
                                    // profileStepId
                                    d.ProfileStepId = stepId;
                                    d.CreateUserName = "thaidx";
                                    d.CreateUserId = 1;
                                    ProfileDocumentDb.Instance.Create(d);
                                }
                            }
                        }
                    }
                }
            }
        }

        public static void IndexSeminarRegister(int seminarId)
        {
            var query = new SeminarRegisterQuery()
            {
                Keyword = string.Empty,
                SeminarId = seminarId,
                SeminarPlaceId = -1,
                IsAttend = -1,
                From = DateTime.Now,
                To = DateTime.Now,
                TitleId = -1,
                Page = 0,
                PageSize = 2000,
                Company = "1005"
            };
            var lstSeminarRegister = SeminarRegisterDb.Instance.Search(query);
            if (lstSeminarRegister != null && lstSeminarRegister.Any())
            {
                foreach (var info in lstSeminarRegister)
                {
                    var customer = CustomerDb.Instance.Read(info.CustomerId);
                    var parents = ParentDb.Instance.GetByCustomer(info.CustomerId);
                    var histories = StudyHistoryDb.Instance.GetByCustomer(info.CustomerId);
                    var abroads = StudyAbroadDb.Instance.GetByCustomer(info.CustomerId);
                    var languages = LanguageDb.Instance.GetByCustomer(info.CustomerId);
                    var obj = SeminarRegisterSE.ToSeminarRegisterSE(info, customer, parents, histories, abroads, languages);
                    SeminarRegisterSearch.Instance.Index(obj);
                }
            }

            var lstIndex = SeminarRegisterSearch.Instance.Search(1002);
            
        }

    }
}
