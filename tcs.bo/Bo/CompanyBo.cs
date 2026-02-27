using System;

namespace tcs.bo
{
    [Serializable]
    public class CompanyBo : BaseBo
    {
        public string CompanyName { get; set; }
        public string FullName { get; set; }
        public string Address { get; set; }
        public string ContactName { get; set; }
        public string ContactPhone { get; set; }
        public string ContactEmail { get; set; }
        public int CompanyType { get; set; }
        public int Status { get; set; }
        public bool IsLock { get; set; }
    }

    [Serializable]
    public class CompanyQuery : IQuery
    {

    }

    [Serializable]
    public class CompanyStatus : Constants
    {
        public static CompanyStatus Instant()
        {
            return new CompanyStatus();
        }

        public static ConstantsValue Active = new ConstantsValue(0, "Active");
        public static ConstantsValue InActive = new ConstantsValue(1, "InActive");
    }

    [Serializable]
    public class CompanyType : Constants
    {
        public static CompanyType Instant()
        {
            return new CompanyType();
        }

        public static ConstantsValue Normal = new ConstantsValue(0, "Normal");
        public static ConstantsValue Vip = new ConstantsValue(1, "Vip");
    }
}
