
using System;

namespace tcs.bo
{
    [Serializable]
    public class ProfileDocumentBo : BaseBo
    {
        public int ProfileTypeId { get; set; }
        public int ProfileStepId { get; set; }
        public int DocumentGroupId { get; set; }
        public string DocumentName { get; set; }
        public string Note { get; set; }
        public int DisplayOrder { get; set; }
    }

    [Serializable]
    public class ProfileDocumentQuery : IQuery
    {
        public int ProfileTypeId { get; set; }
    }

    [Serializable]
    public class DocumentGroup : Constants
    {
        public static DocumentGroup Instant()
        {
            return new DocumentGroup();
        }

        public static ConstantsValue StudyHistoryDocument = new ConstantsValue(1, "Giấy tờ học vấn của học sinh");
        public static ConstantsValue PersonalInformationDocument = new ConstantsValue(2, "Giấy tờ thông tin cá nhân của học sinh");
        public static ConstantsValue FinancialDocument = new ConstantsValue(3, "Giấy tờ tài chính của học sinh");
    }
}
