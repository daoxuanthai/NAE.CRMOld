using System;
namespace tcs.bo
{
    [Serializable]
    public class ProvinceCompanyBo : BaseBo
    {
        public int ProvinceId { get; set; }
        public string ProvinceName { get; set; }
        public int OfficeId { get; set; }
        public string OfficeName { get; set; }
    }

    [Serializable]
    public class ProvinceCompanyQuery : IQuery
    {

    }
}
