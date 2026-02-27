using System;

namespace tcs.bo
{
    [Serializable]
    public class ProvinceBo
    {
        public int Id { get; set; }
        public string ProvinceName { get; set; }
        public string ProvinceCode { get; set; }
        public int OrderNo { get; set; }
        public bool IsDelete { get; set; }
    }

    [Serializable]
    public class ProvinceQuery : IQuery
    {

    }
}
