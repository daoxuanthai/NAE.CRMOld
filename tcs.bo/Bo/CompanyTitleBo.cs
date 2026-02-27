using System;
namespace tcs.bo
{
    [Serializable]
    public class CompanyTitleBo : BaseBo
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string UserFullName { get; set; }
        public int OfficeId { get; set; }

        /// <summary>
        /// Chức danh của nhân viên này trong công ty
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Tên ngắn của chức danh (TVP-HCM, TV-HCM1)
        /// </summary>
        public string Code { get; set; }
        public string Note { get; set; }

        /// <summary>
        /// Chia ra thành nhiều nhóm, VD tư vấn, kế toán, marketing
        /// </summary>
        public int UserType { get; set; }
        public bool IsLock { get; set; }

        /// <summary>
        /// TVP có thể xem tất cả mọi data
        /// </summary>
        public bool IsViewAll { get; set; }
    }

    [Serializable]
    public class CompanyTitleQuery : IQuery
    {
        public int IsLock { get; set; }
    }

    [Serializable]
    public class CompanyTitleType : Constants
    {
        public static CompanyTitleType Instant()
        {
            return new CompanyTitleType();
        }

        public static ConstantsValue Director = new ConstantsValue(0, "Giám đốc");
        public static ConstantsValue Leader = new ConstantsValue(1, "Trưởng văn phòng");
        public static ConstantsValue Accountant = new ConstantsValue(2, "Kế toán");
        public static ConstantsValue Marketing = new ConstantsValue(3, "Marketing");
        public static ConstantsValue Counselor = new ConstantsValue(4, "Tư vấn viên");
        public static ConstantsValue Admission = new ConstantsValue(5, "Admission");
        public static ConstantsValue Admin = new ConstantsValue(6, "Admin");
        public static ConstantsValue Agency = new ConstantsValue(7, "Tài khoản đại lý");
    }


    [Serializable]
    public class CompanyRoles : Constants
    {
        public static CompanyTitleType Instant()
        {
            return new CompanyTitleType();
        }

        public static ConstantsValue Director = new ConstantsValue(0, "Director");
        public static ConstantsValue Leader = new ConstantsValue(1, "Leader");
        public static ConstantsValue Accountant = new ConstantsValue(2, "Accountant");
        public static ConstantsValue Marketing = new ConstantsValue(3, "Marketing");
        public static ConstantsValue Counselor = new ConstantsValue(4, "Counselor");
        public static ConstantsValue Admission = new ConstantsValue(5, "Admission");
        public static ConstantsValue Admin = new ConstantsValue(6, "Admin");
        public static ConstantsValue Agency = new ConstantsValue(7, "Agency");
    }
}
