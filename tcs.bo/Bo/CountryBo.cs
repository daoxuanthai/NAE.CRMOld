using System;

namespace tcs.bo
{
    [Serializable]
    public class CountryBo
    {
        public int Id { get; set; }
        public string CountryName { get; set; }
        public string CountryCode { get; set; }
        public int OrderNo { get; set; }
        public bool IsDelete { get; set; }
    }

    [Serializable]
    public class CountryQuery : IQuery
    {
        
    }
}
