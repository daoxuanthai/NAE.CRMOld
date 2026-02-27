
using System;

namespace tcs.bo
{
    [Serializable]
    public class LanguageBo : BaseBo
    {
        public int CustomerId { get; set; }
        public string Language { get; set; }
        public int Certificate { get; set; }
        public string CertificateName { get; set; }
        public string Score { get; set; }
        public DateTime? RetestDate { get; set; }
        public string RetestDateString { get; set; }
        public string Note { get; set; }
    }

    [Serializable]
    public class LanguageQuery : IQuery
    {

    }

    [Serializable]
    public class LanguageCertificate : Constants
    {
        public static LanguageCertificate Instant()
        {
            return new LanguageCertificate();
        }

        public static ConstantsValue TOEFL = new ConstantsValue(0, "TOEFL");
        public static ConstantsValue IELTS = new ConstantsValue(1, "IELTS");
        public static ConstantsValue SAT = new ConstantsValue(2, "SAT");
        public static ConstantsValue ACT = new ConstantsValue(3, "ACT");
        public static ConstantsValue ITEPS = new ConstantsValue(4, "ITEPS");
        public static ConstantsValue ELTIS = new ConstantsValue(5, "ELTIS");
        public static ConstantsValue MICHIGAN = new ConstantsValue(6, "MICHIGAN");
        public static ConstantsValue TOEFLConcord = new ConstantsValue(7, "TOEFL Concord");
        public static ConstantsValue GMAT = new ConstantsValue(8, "GMAT");
        public static ConstantsValue GRE = new ConstantsValue(9, "GRE");
        public static ConstantsValue DET = new ConstantsValue(10, "DET");
        public static ConstantsValue Other = new ConstantsValue(11, "Khác");
    }
}
