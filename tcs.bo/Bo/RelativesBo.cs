
using System;

namespace tcs.bo
{
    /// <summary>
    /// Thân nhân
    /// </summary>
    [Serializable]
    public class RelativesBo : BaseBo
    {
        public int CustomerId { get; set; }
        public string Name { get; set; }
        public string CountryName { get; set; }
        public string Relationship { get; set; }
        public string Address { get; set; }
        public string JobName { get; set; }
        public string CompanyName { get; set; }
        public string Income { get; set; }
        public string Note { get; set; }
    }

    [Serializable]
    public class RelativesQuery : IQuery
    {

    }
}
