
using System;
using tcs.lib;
using tcs.bo;

namespace tcs.crm.Models
{
    public class BaseModel
    {
        public string Keyword { get; set; }
        public DateTime From { get; set; }
        public string FromDate { get; set; }
        public DateTime To { get; set; }
        public string ToDate { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalRecord { get; set; }
        public string PagerString { get; set; }
        public string TotalString { get; set; }

        public BaseModel()
        {
            Keyword = string.Empty;
            var now = DateTime.Now;
            DateTime from = now.AddMonths(-3);
            DateTime to = new DateTime(now.Year, now.Month, now.Day, 23, 59, 59);
            From = from;
            FromDate = from.ToString(ConfigMgr.DefaultDateFormat);
            To = to;
            ToDate = to.ToString(ConfigMgr.DefaultDateFormat);
            PageIndex = 0;
            PageSize = ConfigMgr.DefaultPageSize;
        }
    }

    public class BaseInsertModel
    {
        public int Id { get; set; }
        public AccountInfo AccountInfo { get; set; }
    }
}