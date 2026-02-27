using System;

namespace tcs.bo
{
    [Serializable]
    public class CompanyOfficeBo : BaseBo
    {
        public int ProvinceId { get; set; }
        public string ProvinceName { get; set; }
        public string OfficeName { get; set; }
        public string OfficeAddress { get; set; }
        public string OfficePhone { get; set; }
        public string OfficeEmail { get; set; }
        public string Note { get; set; }
        public int DirectorUserId { get; set; }
        public int Status { get; set; }
        public bool Visible { get; set; }
    }

    [Serializable]
    public class CompanyOfficeQuery : IQuery
    {
        public int ProvinceId { get; set; }
    }

    [Serializable]
    public class CompanyOfficeStatus : Constants
    {
        public static CompanyOfficeStatus Instant()
        {
            return new CompanyOfficeStatus();
        }

        public static ConstantsValue Active = new ConstantsValue(0, "Hoạt động");
        public static ConstantsValue InActive = new ConstantsValue(1, "Tạm ngừng");
    }
}
