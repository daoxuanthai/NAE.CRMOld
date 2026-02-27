
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using tcs.adapter.Sql;
using tcs.bo;
using tcs.lib;

namespace tcs.adapter.Elastics
{
    public class ContractSearch
    {
        private string _currentIndexDB = "index_contract";
        private string _currentTypeDB = "contract";
        private readonly ElasticIndexer _indexer;

        public ContractSearch()
        {
            _indexer = new ElasticIndexer();
        }
        private static ContractSearch _instance;
        public static ContractSearch Instance => _instance ?? (_instance = new ContractSearch());

        public bool Index(ContractSE model)
        {
            return _indexer.Index(_currentIndexDB, _currentTypeDB, model, model.Id.ToString());
        }

        public bool Update<T>(string id, object data) where T : class
        {
            var response = _indexer.IndexClient.Update<T, object>(DocumentPath<T>.Id(id), u => u
                .Type(_currentTypeDB)
                .Index(_currentIndexDB)
                .Doc(data)
                .DocAsUpsert());
            if (response != null)
            {
                if (response.IsValid == true)
                {
                    return response.IsValid;
                }
                else
                {
                    throw new Exception("ElasticIndexer::Update::" + response.ServerError.Error);
                }
            }
            return false;
        }

        public bool Delete(int id)
        {
            return _indexer.Delete<ContractSE>(_currentIndexDB, _currentTypeDB, id.ToString());
        }

        public bool DeleteMany(string ids)
        {
            return _indexer.Delete<ContractSE>(_currentIndexDB, _currentTypeDB, ids.SplitTotArray());
        }

        public ContractSE Get(int id)
        {
            return _indexer.Get<ContractSE>(_currentIndexDB, _currentTypeDB, id.ToString());
        }

        public List<ContractSE> Search(string keyword, int[] companyId, int[] officeId, int[] employeeId,
            int[] countryId, int[] status, DateTime fromDate, DateTime toDate, out long total, 
            out Dictionary<int, int> outStatus, out Dictionary<int, int> outCountry, 
            int page, int pageSize, bool isStatistic = false, ReportType reportType = ReportType.Daily)
        {
            total = 0;
            outStatus = new Dictionary<int, int>();
            outCountry = new Dictionary<int, int>();

            QueryContainer filter = null;

            #region Filter theo keyword

            if (!string.IsNullOrEmpty(keyword))
            {
                keyword = keyword.ToLower().Replace("@", "_");
                var keyword_en = keyword.Trim().FilterVietkey().ToKeywordSearch();
                var keyword_vn = keyword.Trim().ToKeywordSearch();

                filter &= Query<ContractSE>.FunctionScore(fc =>
                        fc.Name("search_with_keywords")
                          .Query(qq =>
                                    qq.QueryString(qs => qs.Query("*" + keyword_vn + "*").Fields(fs => fs.Field(f => f.SearchInfo)).Boost(50)) ||
                                    qq.QueryString(qs => qs.Query("*" + keyword_en + "*").Fields(fs => fs.Field(f => f.SearchInfoEn)).Boost(48))
                                )
                          .MinScore(0)
                          .BoostMode(FunctionBoostMode.Multiply)
                          .ScoreMode(FunctionScoreMode.Sum)
                          .Functions(fn => fn.ScriptScore(sc => sc.Script(scr => scr.Source("_score")))));
            }

            #endregion

            if (companyId != null && companyId.Length > 0)
                filter &= Query<ContractSE>.Terms(t => t.Field("companyId").Terms(companyId));

            if (officeId != null && officeId.Length > 0)
                filter &= Query<ContractSE>.Terms(t => t.Field("officeId").Terms(officeId));

            if (status != null && status.Length > 0)
                filter &= Query<ContractSE>.Terms(t => t.Field("status").Terms(status));

            if (countryId != null && countryId.Length > 0)
                filter &= Query<ContractSE>.Terms(t => t.Field("countryId").Terms(countryId));

            if (employeeId != null && employeeId.Length > 0)
                filter &= Query<ContractSE>.Terms(t => t.Field("employeeProcessId").Terms(employeeId));

            if (fromDate != DateTime.MinValue && toDate != DateTime.MinValue)
            {
                filter &= Query<ContractSE>.DateRange(dr =>
                    dr.Field(p => p.CreateDate).GreaterThanOrEquals(fromDate.ToElasticDate()));
                filter &= Query<ContractSE>.DateRange(dr =>
                    dr.Field(p => p.CreateDate).LessThanOrEquals(toDate.ToElasticDate()));
            }
            
            var from = page * pageSize;
            var response = _indexer.IndexClient.Search<ContractSE>(s => s
                    .Index(_currentIndexDB)
                    .Type(_currentTypeDB)
                    .Query(q => filter)
                    .Sort(so =>
                    {
                        return so.Descending(i => i.CreateDate);
                    })
                    .From(from)
                    .Size(pageSize).Aggregations(a =>
                    {
                        if (isStatistic)
                        {
                            a.Terms("status_grp", tad => tad
                                .Field(o => o.Status)
                                .Size(20)
                                .Aggregations(ag => ag.Max("status_max_score", max => max.Script("_score")))
                                .Order(o => o.Descending("status_max_score").CountDescending()));

                            a.Terms("country_grp", tad => tad
                                .Field(o => o.CountryId)
                                .Size(100)
                                .Aggregations(ag => ag.Max("country_max_score", max => max.Script("_score")))
                                .Order(o => o.Descending("country_max_score").CountDescending()));
                        }

                        return a;
                    }));
            total = response.HitsMetadata.Total;

            if (isStatistic)
            {
                var terms = response.Aggregations.Terms("status_grp");
                if (terms != null)
                {
                    foreach (var item in terms.Buckets)
                    {
                        outStatus.Add(Convert.ToInt32(item.Key), Convert.ToInt32(item.DocCount));
                    }
                }
                terms = response.Aggregations.Terms("country_grp");
                if (terms != null)
                {
                    foreach (var item in terms.Buckets)
                    {
                        outCountry.Add(Convert.ToInt32(item.Key), Convert.ToInt32(item.DocCount));
                    }
                }
            }

            var solist = response.Documents;
            var contractList = solist as ContractSE[] ?? solist.ToArray();

            return contractList.ToList();
        }

        public long Statistic(ContractReportQuery query, out Dictionary<int, int> outStatus,
            out Dictionary<int, int> outCountry, out Dictionary<string, int> outDate,
            ReportType reportType = ReportType.Daily)
        {
            outStatus = new Dictionary<int, int>();
            outCountry = new Dictionary<int, int>();
            outDate = new Dictionary<string, int>();

            var cacheKey = Cacher.CreateCacheKey(query, reportType);
            var cacheStatus = cacheKey + "_status";
            var cacheCountry = cacheKey + "_country";
            var cacheDate = cacheKey + "_date";

            var result = Cacher.Get<int>(cacheKey);
            if (result > 0)
            {
                outStatus = Cacher.Get<Dictionary<int, int>>(cacheStatus);
                outCountry = Cacher.Get<Dictionary<int, int>>(cacheCountry);
                return result;
            }

            QueryContainer filter = null;

            if (query.CompanyId != null && query.CompanyId.Length > 0)
                filter &= Query<ContractSE>.Terms(t => t.Field("companyId").Terms(query.CompanyId));

            if (query.OfficeId != null && query.OfficeId.Length > 0)
                filter &= Query<ContractSE>.Terms(t => t.Field("officeId").Terms(query.OfficeId));

            if (query.ProvinceId != null && query.ProvinceId.Length > 0)
                filter &= Query<ContractSE>.Terms(t => t.Field("provinceId").Terms(query.ProvinceId));

            if (query.EmployeeId != null && query.EmployeeId.Length > 0)
                filter &= Query<ContractSE>.Terms(t => t.Field("employeeId").Terms(query.EmployeeId));

            if (query.EmployeeProcessId != null && query.EmployeeProcessId.Length > 0)
                filter &= Query<ContractSE>.Terms(t => t.Field("employeeProcessId").Terms(query.EmployeeProcessId));

            if (query.UserIdRef != null && query.UserIdRef.Length > 0)
                filter &= Query<ContractSE>.Terms(t => t.Field("userIdRef").Terms(query.UserIdRef));

            if (query.NewsIdRef != null && query.NewsIdRef.Length > 0)
                filter &= Query<ContractSE>.Terms(t => t.Field("newsIdRef").Terms(query.NewsIdRef));

            if (query.SeminarIdRef != null && query.SeminarIdRef.Length > 0)
                filter &= Query<ContractSE>.Terms(t => t.Field("seminarIdRef").Terms(query.SeminarIdRef));

            if (query.FromDate != DateTime.MinValue && query.ToDate != DateTime.MinValue)
            {
                filter &= Query<ContractSE>.DateRange(dr =>
                    dr.Field(p => p.CreateDate).GreaterThanOrEquals(query.FromDate.ToElasticDate()));
                filter &= Query<ContractSE>.DateRange(dr =>
                    dr.Field(p => p.CreateDate).LessThanOrEquals(query.ToDate.ToElasticDate()));
            }

            var from = 0;
            var response = _indexer.IndexClient.Search<ContractSE>(s => s
                    .Index(_currentIndexDB)
                    .Type(_currentTypeDB)
                    .Query(q => filter)
                    .From(from)
                    .Size(1).Aggregations(a =>
                    {
                        a.Terms("status_grp", tad => tad
                            .Field(o => o.Status)
                            .Size(20)
                            .Aggregations(ag => ag.Max("status_max_score", max => max.Script("_score"))));

                        a.Terms("country_grp", tad => tad
                            .Field(o => o.CountryId)
                            .Size(100)
                            .Aggregations(ag => ag.Max("country_max_score", max => max.Script("_score"))));

                        a.DateHistogram("date_histogram", tad => tad
                            .Field(o => o.CreateDate)
                            .Interval(reportType == ReportType.Daily ? DateInterval.Day : DateInterval.Month)
                            .MinimumDocumentCount(1));

                        return a;
                    }));

            //var requestJson = System.Text.Encoding.UTF8.GetString(response.ApiCall.RequestBodyInBytes);
            var terms = response.Aggregations.Terms("status_grp");
            if (terms != null)
            {
                foreach (var item in terms.Buckets)
                {
                    outStatus.Add(Convert.ToInt32(item.Key), Convert.ToInt32(item.DocCount));
                }
            }
            terms = response.Aggregations.Terms("country_grp");
            if (terms != null)
            {
                foreach (var item in terms.Buckets)
                {
                    outCountry.Add(Convert.ToInt32(item.Key), Convert.ToInt32(item.DocCount));
                }
            }
            var histogram = response.Aggregations.DateHistogram("date_histogram");
            if (histogram != null)
            {
                foreach (var item in histogram.Buckets)
                {
                    var date = Convert.ToDateTime(item.KeyAsString);
                    outDate.Add(date.ToDateString(), Convert.ToInt32(item.DocCount));
                }
            }

            if (response != null)
            {
                Cacher.Add(cacheKey, response.Total);
                Cacher.Add(cacheStatus, outStatus);
                Cacher.Add(cacheCountry, outCountry);
                Cacher.Add(cacheDate, outDate);
                return response.Total;
            }
            return 0;
        }

        public List<ContractDailyReportData> DailyStatistic(ContractReportQuery query, ReportType reportType = ReportType.Daily)
        {
            var cacheKey = Cacher.CreateCacheKey(query, reportType);

            var result = Cacher.Get<List<ContractDailyReportData>>(cacheKey);
            if (result != null)
            {
                return result;
            }

            result = new List<ContractDailyReportData>();

            var current = query.FromDate;
            while (current <= query.ToDate)
            {
                result.Add(new ContractDailyReportData()
                {
                    Date = current.ToDateString()
                });
                current = current.AddDays(1);
            }

            QueryContainer filter = null;

            if (query.CompanyId != null && query.CompanyId.Length > 0)
                filter &= Query<ContractSE>.Terms(t => t.Field("companyId").Terms(query.CompanyId));

            if (query.OfficeId != null && query.OfficeId.Length > 0)
                filter &= Query<ContractSE>.Terms(t => t.Field("officeId").Terms(query.OfficeId));

            if (query.ProvinceId != null && query.ProvinceId.Length > 0)
                filter &= Query<ContractSE>.Terms(t => t.Field("provinceId").Terms(query.ProvinceId));

            if (query.EmployeeId != null && query.EmployeeId.Length > 0)
                filter &= Query<ContractSE>.Terms(t => t.Field("employeeId").Terms(query.EmployeeId));

            if (query.EmployeeProcessId != null && query.EmployeeProcessId.Length > 0)
                filter &= Query<ContractSE>.Terms(t => t.Field("employeeProcessId").Terms(query.EmployeeProcessId));

            if (query.UserIdRef != null && query.UserIdRef.Length > 0)
                filter &= Query<ContractSE>.Terms(t => t.Field("userIdRef").Terms(query.UserIdRef));

            if (query.NewsIdRef != null && query.NewsIdRef.Length > 0)
                filter &= Query<ContractSE>.Terms(t => t.Field("newsIdRef").Terms(query.NewsIdRef));

            if (query.SeminarIdRef != null && query.SeminarIdRef.Length > 0)
                filter &= Query<ContractSE>.Terms(t => t.Field("seminarIdRef").Terms(query.SeminarIdRef));

            if (query.FromDate != DateTime.MinValue && query.ToDate != DateTime.MinValue)
            {
                filter &= Query<ContractSE>.DateRange(dr =>
                    dr.Field(p => p.CreateDate).GreaterThanOrEquals(query.FromDate.ToElasticDate()));
                filter &= Query<ContractSE>.DateRange(dr =>
                    dr.Field(p => p.CreateDate).LessThanOrEquals(query.ToDate.ToElasticDate()));
            }

            var from = 0;
            var response = _indexer.IndexClient.Search<ContractSE>(s => s
                    .Index(_currentIndexDB)
                    .Type(_currentTypeDB)
                    .Query(q => filter)
                    .From(from)
                    .Size(1).Aggregations(a =>
                    {
                        a.DateHistogram("date_histogram", tad => tad
                            .Field(o => o.CreateDate)
                            .Interval(reportType == ReportType.Daily ? DateInterval.Day : DateInterval.Month)
                            .MinimumDocumentCount(1));

                        return a;
                    }));

            //var requestJson = System.Text.Encoding.UTF8.GetString(response.ApiCall.RequestBodyInBytes);
            var histogram = response.Aggregations.DateHistogram("date_histogram");
            if (histogram != null)
            {
                foreach (var item in histogram.Buckets)
                {
                    var date = Convert.ToDateTime(item.KeyAsString);
                    if (result.Any(i => i.Date == date.ToDateString()))
                    {
                        var tmp = result.FirstOrDefault(i => i.Date == date.ToDateString());
                        tmp.Total = Convert.ToInt32(item.DocCount);
                    }
                }
            }

            var newContract = DailyStatisticByStatus(query, ContractStatus.New.Key, reportType);
            var process = DailyStatisticByStatus(query, ContractStatus.Process.Key, reportType);
            var visaFail = DailyStatisticByStatus(query, ContractStatus.VisaFail.Key, reportType);
            var complete = DailyStatisticByStatus(query, ContractStatus.Complete.Key, reportType);
            var liquidated = DailyStatisticByStatus(query, ContractStatus.Liquidated.Key, reportType);

            if (newContract != null && newContract.Any())
            {
                foreach (var item in newContract)
                {
                    if (result.Any(i => i.Date == item.Key))
                    {
                        var tmp = result.FirstOrDefault(i => i.Date == item.Key);
                        tmp.New = item.Value;
                    }
                }
            }

            if (process != null && process.Any())
            {
                foreach (var item in process)
                {
                    if (result.Any(i => i.Date == item.Key))
                    {
                        var tmp = result.FirstOrDefault(i => i.Date == item.Key);
                        tmp.Process = item.Value;
                    }
                }
            }

            if (visaFail != null && visaFail.Any())
            {
                foreach (var item in visaFail)
                {
                    if (result.Any(i => i.Date == item.Key))
                    {
                        var tmp = result.FirstOrDefault(i => i.Date == item.Key);
                        tmp.VisaFail = item.Value;
                    }
                }
            }

            if (complete != null && complete.Any())
            {
                foreach (var item in complete)
                {
                    if (result.Any(i => i.Date == item.Key))
                    {
                        var tmp = result.FirstOrDefault(i => i.Date == item.Key);
                        tmp.Complete = item.Value;
                    }
                }
            }

            if (liquidated != null && liquidated.Any())
            {
                foreach (var item in liquidated)
                {
                    if (result.Any(i => i.Date == item.Key))
                    {
                        var tmp = result.FirstOrDefault(i => i.Date == item.Key);
                        tmp.Liquidated = item.Value;
                    }
                }
            }

            Cacher.Add(cacheKey, result);
            return result;
        }

        /// <summary>
        /// Thống kê KH theo ngày và theo trạng thái
        /// </summary>
        /// <param name="query"></param>
        /// <param name="status"></param>
        /// <param name="reportType"></param>
        /// <returns></returns>
        private Dictionary<string, int> DailyStatisticByStatus(ContractReportQuery query, int status,
            ReportType reportType = ReportType.Daily)
        {
            var cacheKey = Cacher.CreateCacheKey(query, status, reportType);

            var result = Cacher.Get<Dictionary<string, int>>(cacheKey);
            if (result != null)
            {
                return result;
            }

            result = new Dictionary<string, int>();
            QueryContainer filter = null;

            if (query.CompanyId != null && query.CompanyId.Length > 0)
                filter &= Query<ContractSE>.Terms(t => t.Field("companyId").Terms(query.CompanyId));

            if (query.OfficeId != null && query.OfficeId.Length > 0)
                filter &= Query<ContractSE>.Terms(t => t.Field("officeId").Terms(query.OfficeId));

            if (query.ProvinceId != null && query.ProvinceId.Length > 0)
                filter &= Query<ContractSE>.Terms(t => t.Field("provinceId").Terms(query.ProvinceId));

            if (query.EmployeeId != null && query.EmployeeId.Length > 0)
                filter &= Query<ContractSE>.Terms(t => t.Field("employeeId").Terms(query.EmployeeId));

            if (query.EmployeeProcessId != null && query.EmployeeProcessId.Length > 0)
                filter &= Query<ContractSE>.Terms(t => t.Field("employeeProcessId").Terms(query.EmployeeProcessId));

            if (query.UserIdRef != null && query.UserIdRef.Length > 0)
                filter &= Query<ContractSE>.Terms(t => t.Field("userIdRef").Terms(query.UserIdRef));

            if (query.NewsIdRef != null && query.NewsIdRef.Length > 0)
                filter &= Query<ContractSE>.Terms(t => t.Field("newsIdRef").Terms(query.NewsIdRef));

            if (query.SeminarIdRef != null && query.SeminarIdRef.Length > 0)
                filter &= Query<ContractSE>.Terms(t => t.Field("seminarIdRef").Terms(query.SeminarIdRef));

            if (query.FromDate != DateTime.MinValue && query.ToDate != DateTime.MinValue)
            {
                filter &= Query<ContractSE>.DateRange(dr =>
                    dr.Field(p => p.CreateDate).GreaterThanOrEquals(query.FromDate.ToElasticDate()));
                filter &= Query<ContractSE>.DateRange(dr =>
                    dr.Field(p => p.CreateDate).LessThanOrEquals(query.ToDate.ToElasticDate()));
            }
            filter &= Query<ContractSE>.Term(p => p.Status, status);

            var from = 0;
            var response = _indexer.IndexClient.Search<ContractSE>(s => s
                    .Index(_currentIndexDB)
                    .Type(_currentTypeDB)
                    .Query(q => filter)
                    .From(from)
                    .Size(1).Aggregations(a =>
                    {
                        a.DateHistogram("date_histogram", tad => tad
                            .Field(o => o.CreateDate)
                            .Interval(reportType == ReportType.Daily ? DateInterval.Day : DateInterval.Month)
                            .MinimumDocumentCount(1));

                        return a;
                    }));

            //var requestJson = System.Text.Encoding.UTF8.GetString(response.ApiCall.RequestBodyInBytes);
            var histogram = response.Aggregations.DateHistogram("date_histogram");
            if (histogram != null)
            {
                foreach (var item in histogram.Buckets)
                {
                    var date = Convert.ToDateTime(item.KeyAsString);
                    result.Add(date.ToDateString(), Convert.ToInt32(item.DocCount));
                }
            }

            Cacher.Add(cacheKey, result);
            return result;
        }
    }
}
