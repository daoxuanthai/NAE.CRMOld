
using System;

namespace tcs.bo
{
    [Serializable]
    public class StudyAbroadBo : BaseBo
    {
        public int CustomerId { get; set; }
        public int CountryId { get; set; }
        public string CountryName { get; set; }
        public string School { get; set; }
        public string Major { get; set; }
        public int Level { get; set; }
        public int Year { get; set; }
        public string Time { get; set; }
        public string Note { get; set; }
    }

    [Serializable]
    public class StudyAbroadQuery : IQuery
    {

    }

    [Serializable]
    public class StudyLevel : Constants
    {
        public static StudyLevel Instant()
        {
            return new StudyLevel();
        }

        public static ConstantsValue HighSchool = new ConstantsValue(0, "THPT");
        public static ConstantsValue Vocational = new ConstantsValue(1, "Trung cấp nghề");
        public static ConstantsValue College = new ConstantsValue(2, "Cao đẳng");
        public static ConstantsValue University = new ConstantsValue(3, "Đại học");
        public static ConstantsValue Master = new ConstantsValue(4, "Thạc sĩ");
        public static ConstantsValue Doctor = new ConstantsValue(5, "Tiến sĩ");
        public static ConstantsValue SummerStudyAbroad = new ConstantsValue(6, "Du học hè");
        public static ConstantsValue CommunityCollege = new ConstantsValue(7, "Cao đẳng cộng đồng");
    }
}
