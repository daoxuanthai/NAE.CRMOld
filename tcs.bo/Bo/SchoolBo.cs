
using System;

namespace tcs.bo
{
    [Serializable]
    public class SchoolBo : BaseBo
    {
        public string SchoolName { get; set; }
        public string ShortName { get; set; }
        public int Country { get; set; }
        public string CountryName { get; set; }
        public string City { get; set; }
        public string Association { get; set; }
        public string Address { get; set; }
        public string VnAddress { get; set; }
        public string ContactName { get; set; }
        public string ContactEmail { get; set; }
        public string ContactPhone { get; set; }
        public string HotProgram { get; set; }
        public string EducationLevel { get; set; }
        public string CouseOpenTime { get; set; }
        public int SchoolType { get; set; }
        public bool IsStrategy { get; set; }
        public string Note { get; set; }
    }

    [Serializable]
    public class SchoolQuery : IQuery
    {
        public int Country { get; set; }
        public int SchoolType { get; set; }
    }

    [Serializable]
    public class SchoolType : Constants
    {
        public static SchoolType Instant()
        {
            return new SchoolType();
        }

        public static ConstantsValue HighSchool = new ConstantsValue(0, "THPT");
        public static ConstantsValue Vocational = new ConstantsValue(1, "Trung cấp nghề");
        public static ConstantsValue College = new ConstantsValue(2, "Cao đẳng");
        public static ConstantsValue University = new ConstantsValue(3, "Đại học");
    }
}
