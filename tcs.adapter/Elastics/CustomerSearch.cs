
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using tcs.adapter.Sql;
using tcs.bo;
using tcs.lib;

namespace tcs.adapter.Elastics
{
    public class CustomerSearch
    {
        private string _currentIndexDB = "customer_nae";
        private string _currentTypeDB = "customer";
        private readonly ElasticIndexer _indexer;

        public CustomerSearch()
        {
            _indexer = new ElasticIndexer();
        }
        private static CustomerSearch _instance;
        public static CustomerSearch Instance => _instance ?? (_instance = new CustomerSearch());

        public bool Index(CustomerSE model)
        {
            return _indexer.Index(_currentIndexDB, _currentTypeDB, model, model.Id.ToString());
        }

        public bool Update(string id, object data)
        {
            return _indexer.Update<CustomerSE>(_currentIndexDB, _currentTypeDB, id, data);
        }

        public bool Delete(int id)
        {
            return _indexer.Delete<CustomerSE>(_currentIndexDB, _currentTypeDB, id.ToString());
        }

        public bool DeleteMany(string ids)
        {
            return _indexer.Delete<CustomerSE>(_currentIndexDB, _currentTypeDB, ids.SplitTotArray());
        }

        public CustomerSE Get(int id)
        {
            return _indexer.Get<CustomerSE>(_currentIndexDB, _currentTypeDB, id.ToString());
        }

        /// <summary>
        /// Hàm search thông tin khách hàng
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="companyId"></param>
        /// <param name="officeId"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="status"></param>
        /// <param name="countries"></param>
        /// <param name="employeeId"></param>
        /// <param name="employeeProcessId"></param>
        /// <param name="source"></param>
        /// <param name="sourceType"></param>
        /// <param name="educationLevel"></param>
        /// <param name="agency"></param>
        /// <param name="total"></param>
        /// <param name="outStatus">Thống kê theo trạng thái</param>
        /// <param name="outCountry">Thống kê theo quốc gia</param>
        /// <param name="isStatistic">true: lấy thông tin thống kê</param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="isAgency">Filter theo đại lý</param>
        /// <param name="sort"></param>
        /// <returns></returns>
        public List<int> Search(string keyword, int[] companyId, int[] officeId, DateTime fromDate, DateTime toDate,
            int[] status, int[] countries, int[] employeeId, int[] employeeProcessId, int[] source, int[] sourceType,
            int[] educationLevel, int[] agency, out long total, out Dictionary<int, int> outStatus, out Dictionary<int, int> outCountry,
            int page = 0, int pageSize = 20, bool isStatistic = false, int sort = 0, bool isAgency = false)
        {
            total = 0;
            outStatus = new Dictionary<int, int>();
            outCountry = new Dictionary<int, int>();

            QueryContainer filter = null;

            #region Filter theo keyword

            if (!string.IsNullOrEmpty(keyword))
            {
                keyword = keyword.ToLower().Replace("@","_");
                var keyword_en = keyword.Trim().FilterVietkey().ToKeywordSearch();
                var keyword_vn = keyword.Trim().ToKeywordSearch();
                
                filter &= Query<CustomerSE>.FunctionScore(fc =>
                        fc.Name("search_with_keywords")
                          .Query(qq =>
                                    qq.QueryString(qs => qs.Query("*" + keyword_vn + "*").Fields(fs => fs.Field(f => f.SearchInfo)).Boost(50)) ||
                                    qq.QueryString(qs => qs.Query("*" + keyword_en + "*").Fields(fs => fs.Field(f => f.SearchInfoEn)).Boost(48)) ||
                                    qq.QueryString(qs => qs.Query(keyword_en).Fields(fs => fs.Field(f => f.PhoneList)).Boost(40))
                                )
                          .MinScore(0)
                          .BoostMode(FunctionBoostMode.Multiply)
                          .ScoreMode(FunctionScoreMode.Sum)
                          .Functions(fn => fn.ScriptScore(sc => sc.Script(scr => scr.Source("_score")))));
            }

            #endregion

            if (companyId != null && companyId.Length > 0)
                filter &= Query<CustomerSE>.Terms(t => t.Field("companyId").Terms(companyId));

            if (officeId != null && officeId.Length > 0)
                filter &= Query<CustomerSE>.Terms(t => t.Field("officeId").Terms(officeId));

            if (status != null && status.Length > 0)
                filter &= Query<CustomerSE>.Terms(t => t.Field("status").Terms(status));

            if (countries != null && countries.Length > 0)
                filter &= Query<CustomerSE>.Terms(t => t.Field("countryId").Terms(countries));

            // nếu là tài khoản đại lý thì ưu tiên tìm theo đại lý
            if(isAgency)
            {
                if (agency != null && agency.Length == 1 && agency[0] != -1)
                {
                    filter &= Query<CustomerSE>.Terms(t => t.Field("agencyId").Terms(agency));
                }
            }
            else
            {
                if (employeeId != null && employeeId.Length == 1 && employeeId[0] == 0)
                {
                    // tìm KH ko được phân quyền thì chỉ tìm theo employeeId
                    filter &= Query<CustomerSE>.Terms(t => t.Field("employeeId").Terms(employeeId));
                }
                else if (employeeId != null && employeeId.Length > 0)
                {
                    // nhân viên xử lý hoặc nhân viên tư vấn đều có quyền xem thông tin khách hàng
                    filter &= Query<CustomerSE>.Bool(b => b
                        .Should(
                            bs => bs.Terms(t => t.Field("employeeId").Terms(employeeId)),
                            bs => bs.Terms(t => t.Field("employeeProcessId").Terms(employeeProcessId))
                        )
                    );
                }
                if (agency != null && (agency.Length > 1 || (agency.Length == 1 && agency[0] != -1)))
                {
                    filter &= Query<CustomerSE>.Terms(t => t.Field("agencyId").Terms(agency));
                }
            }
            

            if (source != null && source.Length > 0)
                filter &= Query<CustomerSE>.Terms(t => t.Field("source").Terms(source));

            if (sourceType != null && sourceType.Length > 0)
                filter &= Query<CustomerSE>.Terms(t => t.Field("sourceType").Terms(sourceType));

            if (educationLevel != null && educationLevel.Length > 0)
                filter &= Query<CustomerSE>.Terms(t => t.Field("educationLevelId").Terms(educationLevel));

            if (fromDate != DateTime.MinValue && toDate != DateTime.MinValue)
            {
                filter &= Query<CustomerSE>.DateRange(dr =>
                    dr.Field(p => p.CreateDate).GreaterThanOrEquals(fromDate.ToElasticDate()));
                filter &= Query<CustomerSE>.DateRange(dr =>
                    dr.Field(p => p.CreateDate).LessThanOrEquals(toDate.ToElasticDate()));
            }

            if (sort == CustomerSort.AlarmDate.Key)
                filter &= Query<CustomerSE>.DateRange(dr =>
                    dr.Field(p => p.AlarmTime).LessThanOrEquals(DateTime.Now.ToElasticDate()));

            var from = page * pageSize;
            var response = _indexer.IndexClient.Search<CustomerSE>(s => s
                    .Index(_currentIndexDB)
                    .Type(_currentTypeDB)
                    .Query(q => filter)
                    .Sort(so =>
                    {
                        if (sort == CustomerSort.UpdateDate.Key)
                            return so.Descending(i => i.UpdateDate);
                        if (sort == CustomerSort.LastCare.Key)
                            return so.Descending(i => i.LastCare);
                        if (sort == CustomerSort.AlarmDate.Key)
                            return so.Ascending(i => i.AlarmTime);
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

            //var requestJson = System.Text.Encoding.UTF8.GetString(response.ApiCall.RequestBodyInBytes);

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

            var result = new List<int>();
            var solist = response.Documents;
            var customerList = solist as CustomerSE[] ?? solist.ToArray();
            result.AddRange(customerList.Select(i => i.Id));

            return result;
        }

        /// <summary>
        /// Lấy DSKH đến hẹn chăm sóc
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="officeId"></param>
        /// <param name="employee"></param>
        /// <param name="status"></param>
        /// <param name="total"></param>
        /// <param name="isCurrent">true: lấy ngày hiện tại, false: lấy ngày hiện tại trở về trước</param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        public List<int> GetCustomerNotify(int[] companyId, int[] officeId, int[] employee, int[] status, out long total,
            DateTime fromDate, DateTime toDate, int page = 0, int pageSize = 10, int sort = 0)
        {
            QueryContainer filter = null;

            if (companyId != null && companyId.Length > 0)
                filter &= Query<CustomerSE>.Terms(t => t.Field("companyId").Terms(companyId));

            if (officeId != null && officeId.Length > 0)
                filter &= Query<CustomerSE>.Terms(t => t.Field("officeId").Terms(officeId));

            if (status != null && status.Length > 0)
                filter &= Query<CustomerSE>.Terms(t => t.Field("status").Terms(status));

            if (fromDate != DateTime.MinValue && toDate != DateTime.MinValue)
            {
                filter &= Query<CustomerSE>.DateRange(dr =>
                    dr.Field(p => p.AlarmTime).GreaterThanOrEquals(fromDate.ToElasticDate()));
                filter &= Query<CustomerSE>.DateRange(dr =>
                    dr.Field(p => p.AlarmTime).LessThanOrEquals(toDate.ToElasticDate()));
            }

            var from = page * pageSize;
            var response = _indexer.IndexClient.Search<CustomerSE>(s => s
                    .Index(_currentIndexDB)
                    .Type(_currentTypeDB)
                    .Query(q => filter)
                    .Sort(so =>
                    {
                        return so.Ascending(i => i.AlarmTime);
                    })
                    .From(from)
                    .Size(pageSize));

            total = response.HitsMetadata.Total;

            var result = new List<int>();
            var solist = response.Documents;
            var customerList = solist as CustomerSE[] ?? solist.ToArray();
            result.AddRange(customerList.Select(i => i.Id));

            return result;
        }

        /// <summary>
        /// Thống kê khách hàng
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="officeId"></param>
        /// <param name="provinceId"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="total"></param>
        /// <param name="outStatus"></param>
        /// <param name="outCountry"></param>
        /// <param name="outDate"></param>
        /// <param name="outSource"></param>
        /// <param name="outSourceType"></param>
        /// <param name="outNews"></param>
        /// <param name="outSeminar"></param>
        /// <param name="employeeId"></param>
        /// <param name="employeeProcessId"></param>
        /// <param name="userIdRef"></param>
        /// <param name="newsIdRef"></param>
        /// <param name="seminarIdRef"></param>
        /// <param name="isSource"></param>
        /// <param name="isSourceType"></param>
        /// <param name="isNews"></param>
        /// <param name="isSeminar"></param>
        /// <param name="reportType"></param>
        /// <returns></returns>
        public long Statistic(CustomerReportQuery query, out Dictionary<int, int> outStatus,
            out Dictionary<int, int> outCountry, out Dictionary<string, int> outDate,
            out Dictionary<int, int> outSource, out Dictionary<int, int> outSourceType,
            out Dictionary<int, int> outNews, out Dictionary<int, int> outSeminar,
            bool isSource = false, bool isSourceType = false, bool isNews = false,
            bool isSeminar = false, ReportType reportType = ReportType.Daily)
        {
            outStatus = new Dictionary<int, int>();
            outCountry = new Dictionary<int, int>();
            outDate = new Dictionary<string, int>();
            outSource = new Dictionary<int, int>();
            outSourceType = new Dictionary<int, int>();
            outNews = new Dictionary<int, int>();
            outSeminar = new Dictionary<int, int>();

            var cacheKey = Cacher.CreateCacheKey(query, isSource, isSourceType, isNews, isSeminar, reportType);
            var cacheStatus = cacheKey + "_status";
            var cacheCountry = cacheKey + "_country";
            var cacheDate = cacheKey + "_date";
            var cacheSource = cacheKey + "_source";
            var cacheSourceType = cacheKey + "_sourcetype";
            var cacheNews = cacheKey + "_news";
            var cacheSeminar = cacheKey + "_seminar";

            var result = Cacher.Get<int>(cacheKey);
            if (result > 0)
            {
                outStatus = Cacher.Get<Dictionary<int, int>>(cacheStatus);
                outCountry = Cacher.Get<Dictionary<int, int>>(cacheCountry);
                outDate = Cacher.Get<Dictionary<string, int>>(cacheDate);
                outSource = Cacher.Get<Dictionary<int, int>>(cacheSource);
                outSourceType = Cacher.Get<Dictionary<int, int>>(cacheSourceType);
                outNews = Cacher.Get<Dictionary<int, int>>(cacheNews);
                outSeminar = Cacher.Get<Dictionary<int, int>>(cacheSeminar);
                return result;
            }

            QueryContainer filter = null;

            if (query.CompanyId != null && query.CompanyId.Length > 0)
                filter &= Query<CustomerSE>.Terms(t => t.Field("companyId").Terms(query.CompanyId));

            if (query.OfficeId != null && query.OfficeId.Length > 0)
                filter &= Query<CustomerSE>.Terms(t => t.Field("officeId").Terms(query.OfficeId));

            if (query.ProvinceId != null && query.ProvinceId.Length > 0)
                filter &= Query<CustomerSE>.Terms(t => t.Field("provinceId").Terms(query.ProvinceId));

            if (query.EmployeeId != null && query.EmployeeId.Length > 0)
                filter &= Query<CustomerSE>.Terms(t => t.Field("employeeId").Terms(query.EmployeeId));

            if (query.EmployeeProcessId != null && query.EmployeeProcessId.Length > 0)
                filter &= Query<CustomerSE>.Terms(t => t.Field("employeeProcessId").Terms(query.EmployeeProcessId));

            if (query.UserIdRef != null && query.UserIdRef.Length > 0)
                filter &= Query<CustomerSE>.Terms(t => t.Field("userIdRef").Terms(query.UserIdRef));

            if (query.NewsIdRef != null && query.NewsIdRef.Length > 0)
                filter &= Query<CustomerSE>.Terms(t => t.Field("newsIdRef").Terms(query.NewsIdRef));

            if (query.SeminarIdRef != null && query.SeminarIdRef.Length > 0)
                filter &= Query<CustomerSE>.Terms(t => t.Field("seminarIdRef").Terms(query.SeminarIdRef));

            if (query.FromDate != DateTime.MinValue && query.ToDate != DateTime.MinValue)
            {
                filter &= Query<CustomerSE>.DateRange(dr =>
                    dr.Field(p => p.CreateDate).GreaterThanOrEquals(query.FromDate.ToElasticDate()));
                filter &= Query<CustomerSE>.DateRange(dr =>
                    dr.Field(p => p.CreateDate).LessThanOrEquals(query.ToDate.ToElasticDate()));
            }

            var from = 0;
            var response = _indexer.IndexClient.Search<CustomerSE>(s => s
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

                        if (isSource)
                            a.Terms("source_grp", tad => tad
                                .Field(o => o.Source)
                                .Size(20)
                                .Aggregations(ag => ag.Max("source_max_score", max => max.Script("_score"))));

                        if (isSourceType)
                            a.Terms("sourcetype_grp", tad => tad
                                .Field(o => o.SourceType)
                                .Size(20)
                                .Aggregations(ag => ag.Max("sourcetype_max_score", max => max.Script("_score"))));

                        if (isNews)
                            a.Terms("news_grp", tad => tad
                                .Field(o => o.NewsId)
                                .Size(100)
                                .Aggregations(ag => ag.Max("news_max_score", max => max.Script("_score"))));

                        if (isSeminar)
                            a.Terms("seminar_grp", tad => tad
                                .Field(o => o.SeminarId)
                                .Size(100)
                                .Aggregations(ag => ag.Max("seminar_max_score", max => max.Script("_score"))));

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
            if (isSource)
            {
                terms = response.Aggregations.Terms("source_grp");
                if (terms != null)
                {
                    foreach (var item in terms.Buckets)
                    {
                        outSource.Add(Convert.ToInt32(item.Key), Convert.ToInt32(item.DocCount));
                    }
                }
            }

            if (isSourceType)
            {
                terms = response.Aggregations.Terms("sourcetype_grp");
                if (terms != null)
                {
                    foreach (var item in terms.Buckets)
                    {
                        outSourceType.Add(Convert.ToInt32(item.Key), Convert.ToInt32(item.DocCount));
                    }
                }
            }

            if (isNews)
            {
                terms = response.Aggregations.Terms("news_grp");
                if (terms != null)
                {
                    foreach (var item in terms.Buckets)
                    {
                        outNews.Add(Convert.ToInt32(item.Key), Convert.ToInt32(item.DocCount));
                    }
                }
            }

            if (isSeminar)
            {
                terms = response.Aggregations.Terms("seminar_grp");
                if (terms != null)
                {
                    foreach (var item in terms.Buckets)
                    {
                        outSeminar.Add(Convert.ToInt32(item.Key), Convert.ToInt32(item.DocCount));
                    }
                }
            }

            if (response != null)
            {
                Cacher.Add(cacheKey, response.Total);
                Cacher.Add(cacheStatus, outStatus);
                Cacher.Add(cacheCountry, outCountry);
                Cacher.Add(cacheDate, outDate);
                Cacher.Add(cacheSource, outSource);
                Cacher.Add(cacheSourceType, outSourceType);
                Cacher.Add(cacheNews, outNews);
                Cacher.Add(cacheSeminar, outSeminar);
                return response.Total;
            }
            return 0;
        }

        public List<DailyReportData> DailyStatistic(CustomerReportQuery query, ReportType reportType = ReportType.Daily)
        {
            var cacheKey = Cacher.CreateCacheKey(query, reportType);

            var result = Cacher.Get<List<DailyReportData>>(cacheKey);
            if (result != null)
            {
                return result;
            }

            result = new List<DailyReportData>();

            var current = query.FromDate;
            while (current <= query.ToDate)
            {
                result.Add(new DailyReportData()
                {
                    Date = current.ToDateString()
                });
                current = current.AddDays(1);
            }

            QueryContainer filter = null;

            if (query.CompanyId != null && query.CompanyId.Length > 0)
                filter &= Query<CustomerSE>.Terms(t => t.Field("companyId").Terms(query.CompanyId));

            if (query.OfficeId != null && query.OfficeId.Length > 0)
                filter &= Query<CustomerSE>.Terms(t => t.Field("officeId").Terms(query.OfficeId));

            if (query.ProvinceId != null && query.ProvinceId.Length > 0)
                filter &= Query<CustomerSE>.Terms(t => t.Field("provinceId").Terms(query.ProvinceId));

            if (query.EmployeeId != null && query.EmployeeId.Length > 0)
                filter &= Query<CustomerSE>.Terms(t => t.Field("employeeId").Terms(query.EmployeeId));

            if (query.EmployeeProcessId != null && query.EmployeeProcessId.Length > 0)
                filter &= Query<CustomerSE>.Terms(t => t.Field("employeeProcessId").Terms(query.EmployeeProcessId));

            if (query.UserIdRef != null && query.UserIdRef.Length > 0)
                filter &= Query<CustomerSE>.Terms(t => t.Field("userIdRef").Terms(query.UserIdRef));

            if (query.NewsIdRef != null && query.NewsIdRef.Length > 0)
                filter &= Query<CustomerSE>.Terms(t => t.Field("newsIdRef").Terms(query.NewsIdRef));

            if (query.SeminarIdRef != null && query.SeminarIdRef.Length > 0)
                filter &= Query<CustomerSE>.Terms(t => t.Field("seminarIdRef").Terms(query.SeminarIdRef));

            if (query.FromDate != DateTime.MinValue && query.ToDate != DateTime.MinValue)
            {
                filter &= Query<CustomerSE>.DateRange(dr =>
                    dr.Field(p => p.CreateDate).GreaterThanOrEquals(query.FromDate.ToElasticDate()));
                filter &= Query<CustomerSE>.DateRange(dr =>
                    dr.Field(p => p.CreateDate).LessThanOrEquals(query.ToDate.ToElasticDate()));
            }

            var from = 0;
            var response = _indexer.IndexClient.Search<CustomerSE>(s => s
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

            var continueCare = DailyStatisticByStatus(query, CustomerStatus.ContinueCare.Key, reportType);
            var contracted = DailyStatisticByStatus(query, CustomerStatus.Contracted.Key, reportType);
            var maybeContract = DailyStatisticByStatus(query, CustomerStatus.MaybeContract.Key, reportType);
            var notCare = DailyStatisticByStatus(query, CustomerStatus.NotCaring.Key, reportType);
            var notPotential = DailyStatisticByStatus(query, CustomerStatus.NotPotential.Key, reportType);
            var potential = DailyStatisticByStatus(query, CustomerStatus.Potential.Key, reportType);

            if (continueCare != null && continueCare.Any())
            {
                foreach (var item in continueCare)
                {
                    if (result.Any(i => i.Date == item.Key))
                    {
                        var tmp = result.FirstOrDefault(i => i.Date == item.Key);
                        tmp.ContinueCare = item.Value;
                    }
                }
            }

            if (contracted != null && contracted.Any())
            {
                foreach (var item in contracted)
                {
                    if (result.Any(i => i.Date == item.Key))
                    {
                        var tmp = result.FirstOrDefault(i => i.Date == item.Key);
                        tmp.Contracted = item.Value;
                    }
                }
            }

            if (maybeContract != null && maybeContract.Any())
            {
                foreach (var item in maybeContract)
                {
                    if (result.Any(i => i.Date == item.Key))
                    {
                        var tmp = result.FirstOrDefault(i => i.Date == item.Key);
                        tmp.MaybeContract = item.Value;
                    }
                }
            }

            if (notCare != null && notCare.Any())
            {
                foreach (var item in notCare)
                {
                    if (result.Any(i => i.Date == item.Key))
                    {
                        var tmp = result.FirstOrDefault(i => i.Date == item.Key);
                        tmp.NotCaring = item.Value;
                    }
                }
            }

            if (notPotential != null && notPotential.Any())
            {
                foreach (var item in notPotential)
                {
                    if (result.Any(i => i.Date == item.Key))
                    {
                        var tmp = result.FirstOrDefault(i => i.Date == item.Key);
                        tmp.NotPotential = item.Value;
                    }
                }
            }

            if (potential != null && potential.Any())
            {
                foreach (var item in potential)
                {
                    if (result.Any(i => i.Date == item.Key))
                    {
                        var tmp = result.FirstOrDefault(i => i.Date == item.Key);
                        tmp.Potential = item.Value;
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
        private Dictionary<string, int> DailyStatisticByStatus(CustomerReportQuery query, int status,
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
                filter &= Query<CustomerSE>.Terms(t => t.Field("companyId").Terms(query.CompanyId));

            if (query.OfficeId != null && query.OfficeId.Length > 0)
                filter &= Query<CustomerSE>.Terms(t => t.Field("officeId").Terms(query.OfficeId));

            if (query.ProvinceId != null && query.ProvinceId.Length > 0)
                filter &= Query<CustomerSE>.Terms(t => t.Field("provinceId").Terms(query.ProvinceId));

            if (query.EmployeeId != null && query.EmployeeId.Length > 0)
                filter &= Query<CustomerSE>.Terms(t => t.Field("employeeId").Terms(query.EmployeeId));

            if (query.EmployeeProcessId != null && query.EmployeeProcessId.Length > 0)
                filter &= Query<CustomerSE>.Terms(t => t.Field("employeeProcessId").Terms(query.EmployeeProcessId));

            if (query.UserIdRef != null && query.UserIdRef.Length > 0)
                filter &= Query<CustomerSE>.Terms(t => t.Field("userIdRef").Terms(query.UserIdRef));

            if (query.NewsIdRef != null && query.NewsIdRef.Length > 0)
                filter &= Query<CustomerSE>.Terms(t => t.Field("newsIdRef").Terms(query.NewsIdRef));

            if (query.SeminarIdRef != null && query.SeminarIdRef.Length > 0)
                filter &= Query<CustomerSE>.Terms(t => t.Field("seminarIdRef").Terms(query.SeminarIdRef));

            if (query.FromDate != DateTime.MinValue && query.ToDate != DateTime.MinValue)
            {
                filter &= Query<CustomerSE>.DateRange(dr =>
                    dr.Field(p => p.CreateDate).GreaterThanOrEquals(query.FromDate.ToElasticDate()));
                filter &= Query<CustomerSE>.DateRange(dr =>
                    dr.Field(p => p.CreateDate).LessThanOrEquals(query.ToDate.ToElasticDate()));
            }
            filter &= Query<CustomerSE>.Term(p => p.Status, status);

            var from = 0;
            var response = _indexer.IndexClient.Search<CustomerSE>(s => s
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
