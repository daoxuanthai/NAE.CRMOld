using System;

namespace tcs.bo
{
    [Serializable]
    public class CommissionBo : BaseBo
    {
        public int OfficeId { get; set; }
        public int ContractId { get; set; }
        public int CustomerId { get; set; }
        public string Name { get; set; }
        public decimal Commission { get; set; }
        public DateTime? CommissionDate { get; set; }
        public string CommissionDateString { get; set; }
        public bool IsCollect { get; set; }
        public string Note { get; set; }
    }

    [Serializable]
    public class CommissionQuery : IQuery
    {
        public string ContractId { get; set; }
        public string CustomerId { get; set; }
    }
}
