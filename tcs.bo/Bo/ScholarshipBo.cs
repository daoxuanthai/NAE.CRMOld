
using System;

namespace tcs.bo
{
    [Serializable]
    public class ScholarshipBo : BaseBo
    {
        public string ScholarshipName { get; set; }
        public string ShortName { get; set; }
        public int CountryId { get; set; }
        public string CountryName { get; set; }
        public int SchoolId { get; set; }
        public string SchoolName { get; set; }
        public string Require { get; set; }
        public int ScholarshipType { get; set; }
        public string Amount { get; set; }
        public int Quantity { get; set; }
        public int TotalRegister { get; set; }
        public DateTime? ExpiredDate { get; set; }
        public string ExpiredDateString { get; set; }
        public string Note { get; set; }
    }

    [Serializable]
    public class ScholarshipQuery : IQuery
    {
        public int CountryId { get; set; }
        public int SchoolId { get; set; }
        public int ScholarshipType { get; set; }

        public ScholarshipQuery()
        {
            CountryId = -1;
            SchoolId = -1;
            ScholarshipType = -1;
        }
    }

    [Serializable]
    public class ScholarshipType : Constants
    {
        public static ScholarshipType Instant()
        {
            return new ScholarshipType();
        }

        public static ConstantsValue FullScholarship = new ConstantsValue(0, "Học bổng toàn phần");
        public static ConstantsValue HalfScholarship = new ConstantsValue(1, "Học bổng bán phần");
        public static ConstantsValue GovermentScholarship = new ConstantsValue(2, "Học bổng chính phủ");
    }
}
