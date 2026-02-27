using System;
using System.Collections.Generic;

namespace tcs.bo
{
    [Serializable]
    public class RegisterHistoryBo : BaseBo
    {
        public int CustomerId { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string RegisterLink { get; set; }
        public string RegisterInfo { get; set; }
        public string AdvisoryContent { get; set; }
        public bool IsParent { get; set; }
        public bool IsDelete { get; set; }
        public bool IsContact { get; set; }
        public bool IsCallInfo { get; set; }
    }

    [Serializable]
    public class RegisterHistoryQuery : IQuery
    {
        public List<int> CustomerIds { get; set; }
        public List<int> CompanyIds { get; set; }
    }
}
