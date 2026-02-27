
using System;

namespace tcs.bo
{
    /// <summary>
    /// Thân nhân
    /// </summary>
    [Serializable]
    public class GuaranteeBo : BaseBo
    {
        public int CustomerId { get; set; }
        public int RelativesId { get; set; }
        public string RelativesName { get; set; }
        public string Person { get; set; }
        public string GuaranteeType { get; set; }
        public string GuaranteeName { get; set; }
        public string GuaranteeYear { get; set; }
        public string Note { get; set; }
    }

    [Serializable]
    public class GuaranteeQuery : IQuery
    {

    }
}
