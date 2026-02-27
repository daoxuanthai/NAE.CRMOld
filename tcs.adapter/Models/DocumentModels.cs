using System;
using System.Collections.Generic;
using tcs.bo;

namespace tcs.adapter.Models
{
    public class DocumentModels
    {
        public int ContractId { get; set; }
        public int ProfileTypeId { get; set; }
        public List<ProfileStepModel> ListStep { get; set; }
    }

    public class ProfileStepModel
    {
        public ProfileStepBo StepInfo { get; set; }
        public List<ProfileDocumentDetail> ListDocumentDetail { get; set; }
    }

    /// <summary>
    /// Thông tin giấy tờ và chi tiết theo hợp đồng
    /// </summary>
    public class ProfileDocumentDetail
    {
        public int Id { get; set; }
        public int StepId { get; set; }
        public int DocumentId { get; set; }
        public string DocumentName { get; set; }
        public string Note { get; set; }
        public bool IsDone { get; set; }
        public bool IsSkip { get; set; }
        public DateTime? UpdateDate { get; set; }
    }
}
