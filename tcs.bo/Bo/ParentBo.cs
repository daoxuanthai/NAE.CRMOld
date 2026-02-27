
using System;

namespace tcs.bo
{
    /// <summary>
    /// Thông tin người liên hệ (phụ huynh, cộng tác viên)
    /// </summary>
    [Serializable]
    public class ParentBo : BaseBo
    {
        public int CustomerId { get; set; }
        public int ProvinceId { get; set; }
        public string Name { get; set; }
        public DateTime? Birthday { get; set; }
        public int Gender { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Note { get; set; }
        public int Desire { get; set; }
        public string JobName { get; set; }
        public string CompanyName { get; set; }
        public string PositionName { get; set; }
        public string Income { get; set; }
        public string OtherIncome { get; set; }
    }

    [Serializable]
    public class ParentQuery : IQuery
    {

    }
}
