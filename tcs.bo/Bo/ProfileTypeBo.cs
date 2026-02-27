
using System;

namespace tcs.bo
{
    [Serializable]
    public class ProfileTypeBo : BaseBo
    {
        public int CountryId { get; set; }
        public string CountryName { get; set; }
        public string TypeName { get; set; }
        public string Note { get; set; }
    }

    [Serializable]
    public class ProfileTypeQuery : IQuery
    {
        public string Country { get; set; }

        public ProfileTypeQuery()
        {
            Country = "-1";
        }
    }
}
