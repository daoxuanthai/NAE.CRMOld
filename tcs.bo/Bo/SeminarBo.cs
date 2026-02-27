using System;

namespace tcs.bo
{
    [Serializable]
    public class SeminarBo : BaseBo
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Note { get; set; }
        public string ProvinceId { get; set; }
        public string ProvinceName { get; set; }
        public string Time { get; set; }
        public string Link { get; set; }
        public int Register { get; set; }
        public int Attend { get; set; }
        public int Status { get; set; }
    }

    [Serializable]
    public class SeminarQuery : IQuery
    {
        
    }

    [Serializable]
    public class SeminarStatus : Constants
    {
        public static SeminarStatus Instant()
        {
            return new SeminarStatus();
        }

        public static ConstantsValue New = new ConstantsValue(0, "Chưa diễn ra");
        public static ConstantsValue Begin = new ConstantsValue(1, "Đang diễn ra");
        public static ConstantsValue End = new ConstantsValue(2, "Đã kết thúc");
    }
}
