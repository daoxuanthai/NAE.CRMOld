using System;
namespace tcs.bo
{
    [Serializable]
    public class CountryCompanyBo : BaseBo
    {
        public int CountryId { get; set; }
        public string CountryName { get; set; }
        public bool Visible { get; set; }
    }

    [Serializable]
    public class CountryCompanyQuery : IQuery
    {

    }
}
