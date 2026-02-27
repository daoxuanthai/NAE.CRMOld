using NPOI.HSSF.Record.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using tcs.adapter.Elastics;
using tcs.adapter.Sql;
using tcs.bo;
using tcs.crm.Models;
using tcs.lib;

namespace tcs.crm.Controllers
{
    [Authorize]
    public class ReportController : BaseController
    {
        public JsonResult StatusSummary(int day = ConfigMgr.DashboardDayLeft)
        {
            var from = DateTime.Today.AddDays(-day);
            var to = DateTime.Today.ToEndDay();
            var status = new Dictionary<int, int>();
            var country = new Dictionary<int, int>();
            var date = new Dictionary<string, int>();
            var source = new Dictionary<int, int>();
            var sourceType = new Dictionary<int, int>();
            var news = new Dictionary<int, int>();
            var seminar = new Dictionary<int, int>();

            // chỉ lấy những tài khoản thuộc cùng văn phòng để thống kê
            var lstEmployee = AccountInfo.ListTitle.Where(t => (t.OfficeId == AccountInfo.OfficeByTitle) || AccountInfo.OfficeByTitle == -1);

            var query = new CustomerReportQuery();
            query.CompanyId = new int[] { AccountInfo.CompanyId };

            if (AccountInfo.TitleType == CompanyTitleType.Leader.Key || AccountInfo.TitleType == CompanyTitleType.Counselor.Key)
                query.OfficeId = new int[] { AccountInfo.OfficeByTitle };

            query.EmployeeId = lstEmployee.Select(t => t.Id).ToArray();
            query.FromDate = from;
            query.ToDate = to;

            //var total = CustomerSearch.Instance.Statistic(query, out status, out country, out date, 
            //    out source, out sourceType, out news, out seminar);

            var data = CustomerDb.Instance.GetStatusSummary(from, to, AccountInfo.CompanyId, AccountInfo.OfficeByTitle, string.Join(",", query.EmployeeId));
            if (data == null || data.Count <= 0)
                return new Extension.JsonResult(HttpStatusCode.OK, new CustomerStatusRptResult());

            var result = new CustomerStatusRptResult()
            {
                Total = (int)data.Sum(t => t.Total)
            };

            if(data != null && data.Any())
            {
                result.NotCaring = data.Where(s => s.Status == CustomerStatus.NotCaring.Key).Sum(s => s.Total);
                result.ContinueCare = data.Where(s => s.Status == CustomerStatus.ContinueCare.Key).Sum(s => s.Total);
                result.Potential = data.Where(s => s.Status == CustomerStatus.Potential.Key).Sum(s => s.Total);
                result.NotPotential = data.Where(s => s.Status == CustomerStatus.NotPotential.Key).Sum(s => s.Total);
                result.MaybeContract = data.Where(s => s.Status == CustomerStatus.MaybeContract.Key).Sum(s => s.Total);
                result.Contracted = data.Where(s => s.Status == CustomerStatus.Contracted.Key).Sum(s => s.Total);
            }

            return new Extension.JsonResult(HttpStatusCode.OK, result);
        }

        public JsonResult CountrySummary(int day = ConfigMgr.DashboardDayLeft)
        {
            var from = DateTime.Today.AddDays(-day);
            var to = DateTime.Today.ToEndDay();
            var status = new Dictionary<int, int>();
            var country = new Dictionary<int, int>();
            var date = new Dictionary<string, int>();
            var source = new Dictionary<int, int>();
            var sourceType = new Dictionary<int, int>();
            var news = new Dictionary<int, int>();
            var seminar = new Dictionary<int, int>();

            // chỉ lấy những tài khoản thuộc cùng văn phòng để thống kê
            var lstEmployee = AccountInfo.GetListTitleByOfficeId(AccountInfo.OfficeByTitle);

            var query = new CustomerReportQuery();
            query.CompanyId = new int[] { AccountInfo.CompanyId };

            if(AccountInfo.TitleType == CompanyTitleType.Leader.Key || AccountInfo.TitleType == CompanyTitleType.Counselor.Key)
                query.OfficeId = new int[] { AccountInfo.OfficeByTitle };

            query.EmployeeId = lstEmployee.Select(t => t.Id).ToArray();
            query.FromDate = from;
            query.ToDate = to;

            //var total = CustomerSearch.Instance.Statistic(query, out status, out country, out date, 
            //    out source, out sourceType, out news, out seminar);

            var data = CustomerDb.Instance.GetCountrySummary(from, to, AccountInfo.CompanyId, AccountInfo.OfficeByTitle, string.Join(",", query.EmployeeId));
           
            var lstCountry = CountryCompanyDb.Instance.GetByCompany(AccountInfo.CompanyId, true);

            var result = new CustomerRptResult()
            {
                Total = data.Sum(s => s.Total),
                ListData = new List<ReportData>()
            };

            foreach (var item in lstCountry)
            {
                var count = 0;
                if(data != null || data.Count > 0)
                {
                    count = data.Count(i => !string.IsNullOrEmpty(i.CountryId) && i.CountryId.Contains("," + item.CountryId + ","));
                }    
                result.ListData.Add(new ReportData()
                {
                    Name = item.CountryName,
                    Count = count
                });
            }
            
            return new Extension.JsonResult(HttpStatusCode.OK, result);
        }

        public JsonResult CustomerDaily(int day = ConfigMgr.DashboardDayLeft)
        {
            var from = DateTime.Today.AddDays(-day);
            var to = DateTime.Today.ToEndDay();
            var status = new Dictionary<int, int>();
            var country = new Dictionary<int, int>();
            var date = new Dictionary<string, int>();
            var source = new Dictionary<int, int>();
            var sourceType = new Dictionary<int, int>();
            var news = new Dictionary<int, int>();
            var seminar = new Dictionary<int, int>();

            // chỉ lấy những tài khoản thuộc cùng văn phòng để thống kê
            var lstEmployee = AccountInfo.GetListTitleByOfficeId(AccountInfo.OfficeByTitle);

            var query = new CustomerReportQuery();
            query.CompanyId = new int[] { AccountInfo.CompanyId };

            if (AccountInfo.TitleType == CompanyTitleType.Leader.Key || AccountInfo.TitleType == CompanyTitleType.Counselor.Key)
                query.OfficeId = new int[] { AccountInfo.OfficeByTitle };

            query.EmployeeId = lstEmployee.Select(t => t.Id).ToArray();
            query.FromDate = from;
            query.ToDate = to;

            //var result = CustomerSearch.Instance.DailyStatistic(query);

            var data = CustomerDb.Instance.GetStatusDaily(from, to, AccountInfo.CompanyId, AccountInfo.OfficeByTitle, string.Join(",", query.EmployeeId));
            if (data == null || data.Count <= 0)
                return null;

            var dateDistinct = data.Select(i => i.Date).Distinct();
            var output = dateDistinct
                .Select(d => DateTime.ParseExact(d, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture)
                                     .ToString("dd-MM-yyyy"))
                .OrderBy(d => d)
                .ToList();

            var model = new LineChartModel();
            model.Categories = string.Join(",", output.ToArray());
            model.Series = new Series()
            {
                Items = new List<Item>()
            };

            var totalByDate = data
                .GroupBy(r => DateTime.ParseExact(
                    r.Date,
                    "dd/MM/yyyy HH:mm:ss",
                    CultureInfo.InvariantCulture))
                .OrderBy(g => g.Key)
                .Select(g => new
                {
                    Date = g.Key.ToString("dd-MM-yyyy"),
                    Total = g.Sum(x => x.Total)
                })
                .ToList();
            
            model.Series.Items.Add(new Item()
            {
                Name = "Tổng số hồ sơ",
                Data = string.Join(",", totalByDate.Select(i => i.Total).ToArray())
            });

            var continueCare = data
                .Where(r => r.Status == CustomerStatus.ContinueCare.Key)
                .GroupBy(r => DateTime.ParseExact(r.Date, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture))
                .OrderBy(g => g.Key)
                .Select(g => new StatusReport
                {
                    Date = g.Key.ToString("dd-MM-yyyy"),
                    Status = 1,
                    Total = g.Sum(x => x.Total)
                })
                .ToList();
            
            if (continueCare.Count > 0)
            {
                model.Series.Items.Add(new Item()
                {
                    Name = "Tiếp tục chăm sóc",
                    Data = string.Join(",", continueCare.Select(i => i.Total).ToArray())
                });
            }
            else
            {
                model.Series.Items.Add(new Item()
                {
                    Name = "Tiếp tục chăm sóc",
                    Data = string.Join(",", totalByDate.Select(i => 0).ToArray())
                });
            }

            var contracted = data
                .Where(r => r.Status == CustomerStatus.Contracted.Key)
                .GroupBy(r => DateTime.ParseExact(r.Date, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture))
                .OrderBy(g => g.Key)
                .Select(g => new StatusReport
                {
                    Date = g.Key.ToString("dd-MM-yyyy"),
                    Status = 1,
                    Total = g.Sum(x => x.Total)
                })
                .ToList();
            
            if (contracted.Count > 0)
            {
                model.Series.Items.Add(new Item()
                {
                    Name = "Đã ký hợp đồng",
                    Data = string.Join(",", contracted.Select(i => i.Total).ToArray())
                });
            }
            else
            {
                model.Series.Items.Add(new Item()
                {
                    Name = "Đã ký hợp đồng",
                    Data = string.Join(",", totalByDate.Select(i => 0).ToArray())
                });
            }

            var maybeContract = data
                .Where(r => r.Status == CustomerStatus.MaybeContract.Key)
                .GroupBy(r => DateTime.ParseExact(r.Date, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture))
                .OrderBy(g => g.Key)
                .Select(g => new StatusReport
                {
                    Date = g.Key.ToString("dd-MM-yyyy"),
                    Status = 1,
                    Total = g.Sum(x => x.Total)
                })
                .ToList();
           
            if (maybeContract.Count > 0)
            {
                model.Series.Items.Add(new Item()
                {
                    Name = "Có thể ký hợp đồng",
                    Data = string.Join(",", maybeContract.Select(i => i.Total).ToArray())
                });
            }
            else
            {
                model.Series.Items.Add(new Item()
                {
                    Name = "Có thể ký hợp đồng",
                    Data = string.Join(",", totalByDate.Select(i => 0).ToArray())
                });
            }

            var notCare = data
               .Where(r => r.Status == CustomerStatus.NotCaring.Key)
               .GroupBy(r => DateTime.ParseExact(r.Date, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture))
               .OrderBy(g => g.Key)
               .Select(g => new StatusReport
               {
                   Date = g.Key.ToString("dd-MM-yyyy"),
                   Status = 1,
                   Total = g.Sum(x => x.Total)
               })
               .ToList();
            
            if (notCare.Count > 0)
            {
                model.Series.Items.Add(new Item()
                {
                    Name = "Chưa chăm sóc",
                    Data = string.Join(",", notCare.Select(i => i.Total).ToArray())
                });
            }
            else
            {
                model.Series.Items.Add(new Item()
                {
                    Name = "Chưa chăm sóc",
                    Data = string.Join(",", totalByDate.Select(i => 0).ToArray())
                });
            }

            var notPotential = data
               .Where(r => r.Status == CustomerStatus.NotPotential.Key)
               .GroupBy(r => DateTime.ParseExact(r.Date, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture))
               .OrderBy(g => g.Key)
               .Select(g => new StatusReport
               {
                   Date = g.Key.ToString("dd-MM-yyyy"),
                   Status = 1,
                   Total = g.Sum(x => x.Total)
               })
               .ToList();
            
            if (notPotential.Count > 0)
            {
                model.Series.Items.Add(new Item()
                {
                    Name = "Không tiềm năng",
                    Data = string.Join(",", notPotential.Select(i => i.Total).ToArray())
                });
            }
            else
            {
                model.Series.Items.Add(new Item()
                {
                    Name = "Không tiềm năng",
                    Data = string.Join(",", totalByDate.Select(i => 0).ToArray())
                });
            }
            var potential = data
               .Where(r => r.Status == CustomerStatus.Potential.Key)
               .GroupBy(r => DateTime.ParseExact(r.Date, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture))
               .OrderBy(g => g.Key)
               .Select(g => new StatusReport
               {
                   Date = g.Key.ToString("dd-MM-yyyy"),
                   Status = 1,
                   Total = g.Sum(x => x.Total)
               })
               .ToList();
            
            if (notPotential.Count > 0)
            {
                model.Series.Items.Add(new Item()
                {
                    Name = "Tiềm năng",
                    Data = string.Join(",", potential.Select(i => i.Total).ToArray())
                });
            }
            else
            {
                model.Series.Items.Add(new Item()
                {
                    Name = "Tiềm năng",
                    Data = string.Join(",", totalByDate.Select(i => 0).ToArray())
                });
            }
            model.Total = data.Sum(s => s.Total);
            model.TotalContract = contracted.Sum(s => s.Total);

            return new Extension.JsonResult(HttpStatusCode.OK, model);
        }

        public JsonResult ContractStatusSummary(int day = ConfigMgr.DashboardDayLeft)
        {
            var from = DateTime.Today.AddDays(-day);
            var to = DateTime.Today.ToEndDay();
            var status = new Dictionary<int, int>();
            var country = new Dictionary<int, int>();
            var date = new Dictionary<string, int>();

            // chỉ lấy những tài khoản thuộc cùng văn phòng để thống kê
            var lstEmployee = AccountInfo.GetListTitleByOfficeId(AccountInfo.OfficeByTitle);

            var query = new ContractReportQuery();
            query.CompanyId = new int[] { AccountInfo.CompanyId };

            if (AccountInfo.TitleType == CompanyTitleType.Leader.Key || AccountInfo.TitleType == CompanyTitleType.Counselor.Key)
                query.OfficeId = new int[] { AccountInfo.OfficeByTitle };

            query.EmployeeId = lstEmployee.Select(t => t.Id).ToArray();
            query.FromDate = from;
            query.ToDate = to;

            //var total = ContractSearch.Instance.Statistic(query, out status, out country, out date);

            var data = ContractDb.Instance.GetContractSummary(from, to, AccountInfo.CompanyId, AccountInfo.OfficeByTitle, string.Join(",", query.EmployeeId));
            if(data == null || data.Count <= 0)
                return new Extension.JsonResult(HttpStatusCode.OK, new ContractStatusRptResult());

            var result = new ContractStatusRptResult()
            {
                Total = data.Sum(i => i.Total)
            };

            if (status != null && status.Any())
            {
                result.New = data.Where(s => s.Status == ContractStatus.New.Key).Sum(s => s.Total);
                result.Complete = data.Where(s => s.Status == ContractStatus.Complete.Key).Sum(s => s.Total);
                result.Liquidated = data.Where(s => s.Status == ContractStatus.Liquidated.Key).Sum(s => s.Total);
                result.Process = data.Where(s => s.Status == ContractStatus.Process.Key).Sum(s => s.Total);
                result.VisaFail = data.Where(s => s.Status == ContractStatus.VisaFail.Key).Sum(s => s.Total);
            }

            return new Extension.JsonResult(HttpStatusCode.OK, result);
        }

        public JsonResult ContractCountrySummary(int day = ConfigMgr.DashboardDayLeft)
        {
            var from = DateTime.Today.AddDays(-day);
            var to = DateTime.Today.ToEndDay();
            var status = new Dictionary<int, int>();
            var country = new Dictionary<int, int>();
            var date = new Dictionary<string, int>();

            // chỉ lấy những tài khoản thuộc cùng văn phòng để thống kê
            var lstEmployee = AccountInfo.GetListTitleByOfficeId(AccountInfo.OfficeByTitle);

            var query = new ContractReportQuery();
            query.CompanyId = new int[] { AccountInfo.CompanyId };

            if (AccountInfo.TitleType == CompanyTitleType.Leader.Key || AccountInfo.TitleType == CompanyTitleType.Counselor.Key)
                query.OfficeId = new int[] { AccountInfo.OfficeByTitle };

            query.EmployeeId = lstEmployee.Select(t => t.Id).ToArray();
            query.FromDate = from;
            query.ToDate = to;

            //var total = ContractSearch.Instance.Statistic(query, out status, out country, out date);
            var data = ContractDb.Instance.GetCountrySummary(from, to, AccountInfo.CompanyId, AccountInfo.OfficeByTitle, string.Join(",", query.EmployeeId));

            var lstCountry = CountryCompanyDb.Instance.GetByCompany(AccountInfo.CompanyId, true);

            var result = new CustomerRptResult()
            {
                Total = data.Sum(s => s.Total),
                ListData = new List<ReportData>()
            };

            foreach (var item in lstCountry)
            {
                var count = 0;
                if (data != null || data.Count > 0)
                {
                    count = data.Count(i => !string.IsNullOrEmpty(i.CountryId) && i.CountryId.Contains("," + item.CountryId + ","));
                }
                result.ListData.Add(new ReportData()
                {
                    Name = item.CountryName,
                    Count = count
                });
            }

            return new Extension.JsonResult(HttpStatusCode.OK, result);
        }

        public JsonResult ContractDaily(int day = ConfigMgr.DashboardDayLeft)
        {
            var from = DateTime.Today.AddDays(-day);
            var to = DateTime.Today.ToEndDay();
            var status = new Dictionary<int, int>();
            var country = new Dictionary<int, int>();
            var date = new Dictionary<string, int>();

            // chỉ lấy những tài khoản thuộc cùng văn phòng để thống kê
            var lstEmployee = AccountInfo.GetListTitleByOfficeId(AccountInfo.OfficeByTitle);

            var query = new ContractReportQuery();
            query.CompanyId = new int[] { AccountInfo.CompanyId };

            if (AccountInfo.TitleType == CompanyTitleType.Leader.Key || AccountInfo.TitleType == CompanyTitleType.Counselor.Key)
                query.OfficeId = new int[] { AccountInfo.OfficeByTitle };

            query.EmployeeId = lstEmployee.Select(t => t.Id).ToArray();
            query.FromDate = from;
            query.ToDate = to;

            //var result = ContractSearch.Instance.DailyStatistic(query);
            var data = ContractDb.Instance.GetStatusDaily(from, to, AccountInfo.CompanyId, AccountInfo.OfficeByTitle, string.Join(",", query.EmployeeId));

            if (data == null || data.Count <= 0)
                return null;

            var dateDistinct = data.Select(i => i.Date).Distinct();
            var output = dateDistinct
                .Select(d => DateTime.ParseExact(d, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture)
                                     .ToString("dd-MM-yyyy"))
                .OrderBy(d => d)
                .ToList();

            var model = new LineChartModel();
            model.Categories = string.Join(",", output.ToArray());
            model.Series = new Series()
            {
                Items = new List<Item>()
            };

            var totalByDate = data
                .GroupBy(r => DateTime.ParseExact(
                    r.Date,
                    "dd/MM/yyyy HH:mm:ss",
                    CultureInfo.InvariantCulture))
                .OrderBy(g => g.Key)
                .Select(g => new
                {
                    Date = g.Key.ToString("dd-MM-yyyy"),
                    Total = g.Sum(x => x.Total)
                })
                .ToList();
            model.Series.Items.Add(new Item()
            {
                Name = "Tổng số hồ sơ",
                Data = string.Join(",", totalByDate.Select(i => i.Total).ToArray())
            });

            var contracted = data
                .Where(r => r.Status == ContractStatus.New.Key)
                .GroupBy(r => DateTime.ParseExact(r.Date, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture))
                .OrderBy(g => g.Key)
                .Select(g => new StatusReport
                {
                    Date = g.Key.ToString("dd-MM-yyyy"),
                    Status = 1,
                    Total = g.Sum(x => x.Total)
                })
                .ToList();
            
            if (contracted.Count > 0)
            {
                model.Series.Items.Add(new Item()
                {
                    Name = "Mới ký hợp đồng",
                    Data = string.Join(",", contracted.Select(i => i.Total).ToArray())
                });
            }
            else
            {
                model.Series.Items.Add(new Item()
                {
                    Name = "Mới ký hợp đồng",
                    Data = string.Join(",", totalByDate.Select(i => 0).ToArray())
                });
            }

            var process = data
                .Where(r => r.Status == ContractStatus.Process.Key)
                .GroupBy(r => DateTime.ParseExact(r.Date, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture))
                .OrderBy(g => g.Key)
                .Select(g => new StatusReport
                {
                    Date = g.Key.ToString("dd-MM-yyyy"),
                    Status = 1,
                    Total = g.Sum(x => x.Total)
                })
                .ToList();
            
            if (process.Count > 0)
            {
                model.Series.Items.Add(new Item()
                {
                    Name = "Đang xử lý",
                    Data = string.Join(",", process.Select(i => i.Total).ToArray())
                });
            }
            else
            {
                model.Series.Items.Add(new Item()
                {
                    Name = "Đang xử lý",
                    Data = string.Join(",", totalByDate.Select(i => 0).ToArray())
                });
            }

            var fail = data
                .Where(r => r.Status == ContractStatus.VisaFail.Key)
                .GroupBy(r => DateTime.ParseExact(r.Date, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture))
                .OrderBy(g => g.Key)
                .Select(g => new StatusReport
                {
                    Date = g.Key.ToString("dd-MM-yyyy"),
                    Status = 1,
                    Total = g.Sum(x => x.Total)
                })
                .ToList();
            
            if (fail.Count > 0)
            {
                model.Series.Items.Add(new Item()
                {
                    Name = "Rớt visa",
                    Data = string.Join(",", fail.Select(i => i.Total).ToArray())
                });
            }
            else
            {
                model.Series.Items.Add(new Item()
                {
                    Name = "Rớt visa",
                    Data = string.Join(",", totalByDate.Select(i => 0).ToArray())
                });
            }

            var complete = data
                .Where(r => r.Status == ContractStatus.Complete.Key)
                .GroupBy(r => DateTime.ParseExact(r.Date, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture))
                .OrderBy(g => g.Key)
                .Select(g => new StatusReport
                {
                    Date = g.Key.ToString("dd-MM-yyyy"),
                    Status = 1,
                    Total = g.Sum(x => x.Total)
                })
                .ToList();
            
            if (complete.Count > 0)
            {
                model.Series.Items.Add(new Item()
                {
                    Name = "Hoàn tất",
                    Data = string.Join(",", complete.Select(i => i.Total).ToArray())
                });
            }
            else
            {
                model.Series.Items.Add(new Item()
                {
                    Name = "Hoàn tất",
                    Data = string.Join(",", totalByDate.Select(i => 0).ToArray())
                });
            }

            var liquidated = data
                .Where(r => r.Status == ContractStatus.Liquidated.Key)
                .GroupBy(r => DateTime.ParseExact(r.Date, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture))
                .OrderBy(g => g.Key)
                .Select(g => new StatusReport
                {
                    Date = g.Key.ToString("dd-MM-yyyy"),
                    Status = 1,
                    Total = g.Sum(x => x.Total)
                })
                .ToList();
            
            if (liquidated.Count > 0)
            {
                model.Series.Items.Add(new Item()
                {
                    Name = "Thanh lý",
                    Data = string.Join(",", liquidated.Select(i => i.Total).ToArray())
                });
            }
            else
            {
                model.Series.Items.Add(new Item()
                {
                    Name = "Thanh lý",
                    Data = string.Join(",", totalByDate.Select(i => 0).ToArray())
                });
            }
            model.Total = data.Sum(s => s.Total);

            return new Extension.JsonResult(HttpStatusCode.OK, model);
        }
    }
}