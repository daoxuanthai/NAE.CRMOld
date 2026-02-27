using System;

namespace tcs.bo
{
    [Serializable]
    public class SeminarPlaceBo : BaseBo
    {
        public int SeminarId { get; set; }
        public int ProvinceId { get; set; }
        public string ProvinceName { get; set; }
        public string Place { get; set; }
        public string Address { get; set; }
        public DateTime SeminarDate { get; set; }
        public string SeminarDateString { get; set; }
        public string Note { get; set; }
        public int Register { get; set; }
        public int Attend { get; set; }
        public decimal Cost { get; set; }
        public int Status { get; set; }
    }

    [Serializable]
    public class SeminarPlaceQuery : IQuery
    {
        
    }

    [Serializable]
    public class SeminarPlaceStatus : Constants
    {
        public static SeminarPlaceStatus Instant()
        {
            return new SeminarPlaceStatus();
        }

        public static ConstantsValue New = new ConstantsValue(0, "Chưa diễn ra");
        public static ConstantsValue End = new ConstantsValue(1, "Đã kết thúc");
    }
}
