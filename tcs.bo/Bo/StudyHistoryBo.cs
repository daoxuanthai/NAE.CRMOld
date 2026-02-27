
using System;

namespace tcs.bo
{
    [Serializable]
    public class StudyHistoryBo : BaseBo
    {
        public int CustomerId { get; set; }
        public string School { get; set; }
        public string Major { get; set; }
        public string Class { get; set; }
        public string Score { get; set; }
        public DateTime? GraduateDate { get; set; }
        public string GraduateDateString { get; set; }
        public string Note { get; set; }
    }

    [Serializable]
    public class StudyHistoryQuery : IQuery
    {

    }
}
