
using System;

namespace tcs.bo
{
    [Serializable]
    public class ContractDocumentBo : BaseBo
    {
        public int ContractId { get; set; }
        public int ProfileTypeId { get; set; }
        public int ProfileStepId { get; set; }
        public int ProfileDocumentId { get; set; }
        public string AttachFile { get; set; }
        public string Note { get; set; }
        public bool IsDone { get; set; }
        public bool IsSkip { get; set; }
    }

    [Serializable]
    public class ContractDocumentQuery : IQuery
    {
        public int ContractId { get; set; }
    }
}
