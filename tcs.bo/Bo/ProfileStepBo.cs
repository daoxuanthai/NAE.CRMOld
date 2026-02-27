
using System;

namespace tcs.bo
{
    [Serializable]
    public class ProfileStepBo : BaseBo
    {
        public int ProfileTypeId { get; set; }
        public int ParentId { get; set; }
        public string StepName { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsStep { get; set; }
    }

    [Serializable]
    public class ProfileStepQuery : IQuery
    {
        public int ProfileTypeId { get; set; }
    }
}
