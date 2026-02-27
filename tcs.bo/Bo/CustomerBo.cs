using System;
using System.Collections.Generic;
using tcs.lib;

namespace tcs.bo
{
    [Serializable]
    public class CustomerBo : BaseBo
    {
        public int AreaId { get; set; }
        public int ProvinceId { get; set; }
        public int OfficeId { get; set; }
        public string Fullname { get; set; }
        public int Gender { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public DateTime? Birthday { get; set; }
        public string Address { get; set; }
        public string CustomerNote { get; set; }
        public string AdvisoryNote { get; set; }
        public string EmployeeNote { get; set; }
        public string CountryId { get; set; }
        public string CountryName { get; set; }
        public string AbroadTime { get; set; }
        public string EducationLevelId { get; set; }
        public string SearchInfo { get; set; }
        public int CustomerType { get; set; }
        public int ProfileType { get; set; }
        public int NewsIdRef { get; set; }
        public string NewsUrlRef { get; set; }
        public int UserIdRef { get; set; }
        public int SeminarIdRef { get; set; }
        public string SeminarNameRef { get; set; }
        public int Source { get; set; }
        public int SourceType { get; set; }
        /// <summary>
        /// ID title nhân viên sale theo công ty
        /// </summary>
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        /// <summary>
        /// Id title nhân viên admission theo công ty
        /// </summary>
        public int EmployeeProcessId { get; set; }
        public string EmployeeProcessName { get; set; }
        /// <summary>
        /// Id title của đại lý theo công ty
        /// </summary>
        public int AgencyId { get; set; }
        public string AgencyName { get; set; }
        public string ProcessNote { get; set; }
        public int Status { get; set; }
        public bool IsFly { get; set; }
        public bool IsCommission { get; set; }
        public bool IsAlarm { get; set; }
        public DateTime? AlarmTime { get; set; }
        public DateTime? AlarmTimeOrder { get; set; }
        public DateTime? LastCare { get; set; }
        public int Desire { get; set; }
    }


    [Serializable]
    public class CustomerSE
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public int OfficeId { get; set; }

        public string SearchInfo { get; set; }
        public string SearchInfoEn { get; set; }
        public DateTime Birthday { get; set; }

        public int ProvinceId { get; set; }
        public string Address { get; set; }
        public int[] CountryId { get; set; }
        public int[] EducationLevelId { get; set; }

        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public int EmployeeProcessId { get; set; }
        public string EmployeeProcessName { get; set; }

        public int AgencyId { get; set; }
        public string AgencyName { get; set; }

        public int Source { get; set; }
        public int SourceType { get; set; }

        public int NewsId { get; set; }
        public string NewsUrl { get; set; }
        public int SeminarId { get; set; }
        public string SerminarName { get; set; }

        public int CustomerType { get; set; }
        public int ProfileType { get; set; }

        /// <summary>
        /// ID cộng tác viên
        /// </summary>
        public int UserIdRef { get; set; }

        public int Status { get; set; }

        public DateTime AlarmTime { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime UpdateDate { get; set; }

        public DateTime LastCare { get; set; }

        public int CreateUserId { get; set; }
        public string CreateUserName { get; set; }
        public string[] PhoneList { get; set; }
    }

    [Serializable]
    public class CustomerQuery : IQuery
    {
        public string Area { get; set; }
        public string Country { get; set; }
        public string Employee { get; set; }
        public string Source { get; set; }
        public string SourceType { get; set; }
        public string EducationLevel { get; set; }
        public string Agency { get; set; }
        public int Sort { get; set; }
        public bool IsAgency { get; set; }

        public CustomerQuery()
        {
            Keyword = string.Empty;
            Area = "-1";
            Country = "-1";
            Employee = "-1";
            Source = "-1";
            SourceType = "-1";
            EducationLevel = "-1";
            Company = "-1";
            Agency = "-1";
        }
    }

    [Serializable]
    public class CustomerStatus : Constants
    {
        public static CustomerStatus Instant()
        {
            return new CustomerStatus();
        }

        public static ConstantsValue NotCaring = new ConstantsValue(0, "Chưa chăm sóc");
        public static ConstantsValue ContinueCare = new ConstantsValue(1, "Tiếp tục chăm sóc");
        public static ConstantsValue Potential = new ConstantsValue(2, "Tiềm năng");
        public static ConstantsValue NotPotential = new ConstantsValue(3, "Không tiềm năng");
        public static ConstantsValue MaybeContract = new ConstantsValue(4, "Có thể ký hợp đồng");
        public static ConstantsValue Contracted = new ConstantsValue(5, "Đã ký hợp đồng");
    }

    [Serializable]
    public class CustomerSource : Constants
    {
        public static CustomerSource Instant()
        {
            return new CustomerSource();
        }

        public static ConstantsValue Undefine = new ConstantsValue(0, "Chưa phân loại");
        public static ConstantsValue Website = new ConstantsValue(1, "Website");
        public static ConstantsValue Seminar = new ConstantsValue(2, "Hội thảo");
        public static ConstantsValue EmailMarketing = new ConstantsValue(3, "Email marketing");
        public static ConstantsValue SelfMining = new ConstantsValue(4, "Tự khai thác");
        public static ConstantsValue Collaborator = new ConstantsValue(5, "Cộng tác viên");
        public static ConstantsValue Facebook = new ConstantsValue(6, "Facebook");
        public static ConstantsValue Google = new ConstantsValue(7, "Google");
        public static ConstantsValue CustomerParent = new ConstantsValue(8, "Phụ huynh giới thiệu");
        public static ConstantsValue CustomerParentMeeting = new ConstantsValue(9, "Họp phụ huynh");
        public static ConstantsValue Agency = new ConstantsValue(10, "Đại lý");
        public static ConstantsValue Leaflet = new ConstantsValue(11, "MKT Offline");
        public static ConstantsValue Other = new ConstantsValue(12, "Nguồn khác");
        public static ConstantsValue MrNam = new ConstantsValue(13, "Thầy Nam");
        public static ConstantsValue HiddenData = new ConstantsValue(14, "Data ẩn");
        public static ConstantsValue OldData = new ConstantsValue(15, "Data cũ");
    }

    [Serializable]
    public class CustomerSourceType : Constants
    {
        public static CustomerSourceType Instant()
        {
            return new CustomerSourceType();
        }

        public static ConstantsValue Undefine = new ConstantsValue(0, "Chưa phân loại");
        public static ConstantsValue Website = new ConstantsValue(1, "Online");
        public static ConstantsValue Seminar = new ConstantsValue(2, "Offline");
    }

    [Serializable]
    public class EducationLevel : Constants
    {
        public static EducationLevel Instant()
        {
            return new EducationLevel();
        }

        public static ConstantsValue HighSchool = new ConstantsValue(0, "THPT");
        public static ConstantsValue Apprentice = new ConstantsValue(1, "Học nghề");
        public static ConstantsValue Vocational = new ConstantsValue(2, "Trung cấp");
        public static ConstantsValue College = new ConstantsValue(3, "Cao đẳng");
        public static ConstantsValue University = new ConstantsValue(4, "Đại học");
        public static ConstantsValue Master = new ConstantsValue(5, "Thạc sỹ");
        public static ConstantsValue Certificate = new ConstantsValue(6, "Chứng chỉ sau đại học");
    }

    [Serializable]
    public class CustomerExcel
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Employee { get; set; }
        public string Source { get; set; }
        public string Status { get; set; }
        public string CreateDate { get; set; }
    }

    [Serializable]
    public class CustomerDesire : Constants
    {
        public static CustomerDesire Instant()
        {
            return new CustomerDesire();
        }

        public static ConstantsValue Desire1 = new ConstantsValue(1, "Không muốn đi");
        public static ConstantsValue Desire2 = new ConstantsValue(2, "Đi cũng được và không đi cũng được");
        public static ConstantsValue Desire3 = new ConstantsValue(3, "Muốn");
        public static ConstantsValue Desire4 = new ConstantsValue(4, "Rất muốn");
    }

    [Serializable]
    public class CustomerSort : Constants
    {
        public static CustomerSort Instant()
        {
            return new CustomerSort();
        }

        public static ConstantsValue CreateDate = new ConstantsValue(0, "Ngày tạo mới nhất");
        public static ConstantsValue UpdateDate = new ConstantsValue(1, "Ngày cập nhật mới nhất");
        public static ConstantsValue LastCare = new ConstantsValue(2, "Ngày chăm sóc mới nhất");
        public static ConstantsValue AlarmDate = new ConstantsValue(3, "Ngày đến hẹn chăm sóc");
    }

    [Serializable]
    public class CustomerStatusReportData
    {
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public DateTime Date { get; set; }
        public int Total { get; set; }
        public int TotalNotCare { get; set; }
        public int TotalContinueCare { get; set; }
        public int TotalPotential { get; set; }
        public int TotalNotPotential { get; set; }
        public int TotalMaybeContract { get; set; }
        public int TotalContract { get; set; }
    }

    [Serializable]
    public class StatusReport
    {
        public string Date { get; set; }
        public int Status { get; set; }
        public int Total { get; set; }
    }

    [Serializable]
    public class CountryReport
    {
        public string Date { get; set; }
        public string CountryId { get; set; }
        public int Total { get; set; }
    }
}
