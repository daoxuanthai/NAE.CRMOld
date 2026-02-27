using System;

namespace tcs.bo
{
    [Serializable]
    public abstract class IQuery
    {
        public string Keyword { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public string Status { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalRecord { get; set; }
        public string Company { get; set; }
        public string Office { get; set; }

        protected IQuery()
        {
            var now = DateTime.Now;
            Keyword = string.Empty;
            Status = "-1";
            PageSize = 30;
            Company = "-1";
            Office = "-1";
            DateTime from = now.AddMonths(-3);
            DateTime to = new DateTime(now.Year, now.Month, now.Day, 23, 59, 59);
            From = from;
            To = to;
        }
    }
}
