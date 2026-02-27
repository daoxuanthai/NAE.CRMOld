using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using tcs.bo;
using tcs.lib;

namespace tcs.crm.Models
{
    public class CustomerModel : BaseModel
    {
        public List<ConstantsValue> ListStatus { get; set; }

        /// <summary>
        /// Tìm theo trạng thái khách hàng
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Danh sách quốc gia theo công ty
        /// </summary>
        public List<SelectListItem> ListCountry { get; set; }

        /// <summary>
        /// Tìm theo quốc gia
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        /// Danh sách vị trí được phân quyền
        /// </summary>
        public List<SelectListItem> ListCompanyTitle { get; set; }

        /// <summary>
        /// Tìm theo vị trí được phân quyền
        /// </summary>
        public string Employee { get; set; }

        /// <summary>
        /// Tìm theo văn phòng
        /// </summary>
        public string Office { get; set; }

        public List<ConstantsValue> ListCustomerSource { get; set; }

        /// <summary>
        /// Tìm theo nguồn: facebook, google,...
        /// </summary>
        public string CustomerSource { get; set; }

        public List<ConstantsValue> ListCustomerSourceType { get; set; }

        /// <summary>
        /// Nguồn on/offline
        /// </summary>
        public string SourceType { get; set; }

        public List<ConstantsValue> ListEducationLevel { get; set; }

        /// <summary>
        /// Tìm theo bậc học
        /// </summary>
        public string EducationLevel { get; set; }

        public List<SelectListItem> ListOffice { get; set; }
        
        public List<CustomerBo> ListCustomer { get; set; }

        public AccountInfo AccountInfo { get; set; }

        public string Company { get; set; }

        public List<SelectListItem> ListCompany { get; set; }

        public bool SortByCustomerCareDate { get; set; }

        public int Sort { get; set; }

        public List<ConstantsValue> ListCustomerSort { get; set; }

        public CustomerModel()
        {
            Country = "-1";
            Status = "-1";
            Company = "-1";
            Office = "-1";
            SourceType = "-1";
            CustomerSource = "-1";
            EducationLevel = "-1";
            FromDate = DateTime.Now.AddYears(-1).ToDateString();
            Sort = CustomerSort.CreateDate.Key;
        }
    }

    public class CustomerInsertModel
    {
        public int Id { get; set; }
        
        public CustomerInfoModel CustomerInfo { get; set; }

        public CustomerParentModel CustomerParent { get; set; }

        public CustomerRelativesModel Relatives { get; set; }
        public CustomerGuaranteeModel Guarantee { get; set; }

        public CustomerCareModel CustomerCare { get; set; }

        public StudyHistoryModel StudyHistory { get; set; }

        public StudyAbroadModel StudyAbroad { get; set; }

        public SeminarRegisterInsertModel SeminarRegister { get; set; }

        public LanguageModel Language { get; set; }

        public AccountInfo AccountInfo { get; set; }

        public List<RegisterHistoryBo> ListRegisterHistory { get; set; }
        public CustomerInsertModel()
        {
            CustomerInfo = new CustomerInfoModel();
            CustomerParent = new CustomerParentModel();
            CustomerCare = new CustomerCareModel();
            StudyHistory = new StudyHistoryModel();
            StudyAbroad = new StudyAbroadModel();
            ListRegisterHistory = new List<RegisterHistoryBo>();
        }
    }

    public class CustomerInfoModel
    {
        public int Id { get; set; }

        public int CompanyId { get; set; }

        public int AreaId { get; set; }

        public int ProvinceId { get; set; }

        public List<SelectListItem> ListProvince { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập họ và tên")]
        [MinLength(2, ErrorMessage = "Họ và tên tối thiểu 2 ký tự")]
        [MaxLength(128, ErrorMessage = "Họ và tên tối đa 128 ký tự")]
        public string Fullname { get; set; }

        public int Gender { get; set; }

        [MinLength(5, ErrorMessage = "Địa chỉ email tối thiểu 5 ký tự")]
        [MaxLength(50, ErrorMessage = "Địa chỉ email tối đa 50 ký tự")]
        [RegularExpression(@"^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$", ErrorMessage = "Địa chỉ email không hợp lệ")]
        public string Email { get; set; }

        [MaxLength(13, ErrorMessage = "Số điện thoại tối đa 13 ký tự")]
        public string Phone { get; set; }

        public DateTime Birthday { get; set; }

        public string BirthdayString { get; set; }

        public string BirthdayYear { get; set; }

        [MaxLength(4, ErrorMessage = "Năm sinh tối đa 4 ký tự")]
        public string BirthdayYearString { get; set; }

        [MaxLength(255, ErrorMessage = "Địa chỉ tối đa 255 ký tự")]
        public string Address { get; set; }

        [MaxLength(1000, ErrorMessage = "Thông tin cần tư vấn tối đa 1000 ký tự")]
        public string CustomerNote { get; set; }

        [MaxLength(1000, ErrorMessage = "Thông tin tư vấn tối đa 1000 ký tự")]
        public string AdvisoryNote { get; set; }

        [MaxLength(1000, ErrorMessage = "Thông tin ghi chú tối đa 1000 ký tự")]
        public string EmployeeNote { get; set; }
        
        public int CustomerType { get; set; }

        public int ProfileType { get; set; }
        
        [MaxLength(500, ErrorMessage = "Link bài tin tối đa 500 ký tự")]
        public string NewsUrlRef { get; set; }
        
        public int Source { get; set; }

        public List<ConstantsValue> ListSource { get; set; }

        public int SourceType { get; set; }

        public int EmployeeId { get; set; }

        public string EmployeeName { get; set; }

        public List<SelectListItem> ListEmployee { get; set; }

        public int AgencyId { get; set; }

        public string AgencyName { get; set; }

        public List<SelectListItem> ListAgency { get; set; }

        public int Status { get; set; }

        public List<ConstantsValue> ListStatus { get; set; }

        public AccountInfo AccountInfo { get; set; }

        public int Desire { get; set; }

        public List<ConstantsValue> ListDesire { get; set; }
    }

    public class CustomerParentModel
    {
        public int CustomerId { get; set; }

        public int Id { get; set; }

        public int ProvinceId { get; set; }

        public List<SelectListItem> ListProvince { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập họ và tên")]
        [MinLength(2, ErrorMessage = "Họ và tên tối thiểu 2 ký tự")]
        [MaxLength(128, ErrorMessage = "Họ và tên tối đa 128 ký tự")]
        public string Name { get; set; }

        public DateTime Birthday { get; set; }

        [MaxLength(128, ErrorMessage = "Năm sinh tối đa 4 ký tự")]
        public string BirthdayString { get; set; }

        public int Gender { get; set; }

        [RegularExpression(@"^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$", ErrorMessage = "Địa chỉ email không hợp lệ")]
        [MaxLength(128, ErrorMessage = "Địa chỉ email tối đa 50 ký tự")]
        public string Email { get; set; }

        [MaxLength(13, ErrorMessage = "Số điện thoại tối đa 13 ký tự")]
        public string Phone { get; set; }

        [MaxLength(500, ErrorMessage = "Thông tin ghi chú tối đa 500 ký tự")]
        public string Note { get; set; }

        public int Desire { get; set; }

        public List<ConstantsValue> ListDesire { get; set; }

        [MaxLength(128, ErrorMessage = "Nghề nghệp tối đa 128 ký tự")]
        public string JobName { get; set; }

        [MaxLength(128, ErrorMessage = "Tên chức vụ tối đa 128 ký tự")]
        public string PositionName { get; set; }

        [MaxLength(128, ErrorMessage = "Tên cơ quan tối đa 128 ký tự")]
        public string CompanyName { get; set; }

        [MaxLength(128, ErrorMessage = "Thu nhập tối đa 128 ký tự")]
        public string Income { get; set; }

        [MaxLength(128, ErrorMessage = "Thu nhập khác tối đa 500 ký tự")]
        public string OtherIncome { get; set; }
        public List<ParentBo> ListParent { get; set; }
    }

    public class CustomerRelativesModel
    {
        public int CustomerId { get; set; }

        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập họ và tên")]
        [MinLength(2, ErrorMessage = "Họ và tên tối thiểu 2 ký tự")]
        [MaxLength(128, ErrorMessage = "Họ và tên tối đa 128 ký tự")]
        public string Name { get; set; }

        [MaxLength(128, ErrorMessage = "Quan hệ với học sinh tối đa 128 ký tự")]
        public string Relationship { get; set; }

        [MaxLength(128, ErrorMessage = "Tên quốc gia tối đa 128 ký tự")]
        public string CountryName { get; set; }

        [MaxLength(255, ErrorMessage = "Nơi ở tối đa 255 ký tự")]
        public string Address { get; set; }

        [MaxLength(128, ErrorMessage = "Nghề nghệp tối đa 128 ký tự")]
        public string JobName { get; set; }

        [MaxLength(128, ErrorMessage = "Nơi làm việc tối đa 128 ký tự")]
        public string CompanyName { get; set; }

        [MaxLength(128, ErrorMessage = "Thu nhập tối đa 128 ký tự")]
        public string Income { get; set; }

        public List<RelativesBo> ListRelatives { get; set; }


        [MaxLength(500, ErrorMessage = "Thông tin ghi chú tối đa 500 ký tự")]
        public string Note { get; set; }
    }

    public class CustomerGuaranteeModel
    {
        public int CustomerId { get; set; }

        public int Id { get; set; }

        public int RelativesId { get; set; }

        public string RelativesName { get; set; }

        public List<RelativesBo> ListRelatives { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên người được bảo lãnh")]
        [MaxLength(128, ErrorMessage = "Tên người được bảo lãnh tối đa 128 ký tự")]
        public string Person { get; set; }

        public string GuaranteeType { get; set; }

        [MaxLength(128, ErrorMessage = "Loại bảo lãnh tối đa 128 ký tự")]
        public string GuaranteeName { get; set; }

        [MaxLength(4, ErrorMessage = "Năm bảo lãnh tối đa 4 ký tự")]
        public string GuaranteeYear { get; set; }

        [MaxLength(500, ErrorMessage = "Thông tin ghi chú tối đa 500 ký tự")]
        public string Note { get; set; }

        public List<GuaranteeBo> ListGuarantee { get; set; }
    }

    public class CustomerCareModel
    {
        public int CustomerId { get; set; }

        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập nội dung chăm sóc")]
        [MinLength(10, ErrorMessage = "Nội dung chăm sóc tối thiểu 10 ký tự")]
        [MaxLength(1000, ErrorMessage = "Nội dung chăm sóc tối đa 1000 ký tự")]
        public string Advisory { get; set; }

        public bool IsAlarm { get; set; }

        public DateTime AlarmTime { get; set; }

        public string AlarmTimeString { get; set; }
        
        public List<CustomerCareBo> ListCustomerCare { get; set; }
    }

    public class StudyHistoryModel
    {
        public int CustomerId { get; set; }

        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên trường")]
        [MinLength(3, ErrorMessage = "Tên trường tối thiểu 3 ký tự")]
        [MaxLength(128, ErrorMessage = "Tên trường tối đa 128 ký tự")]
        public string School { get; set; }

        [MaxLength(128, ErrorMessage = "Tên ngành học tối đa 128 ký tự")]
        public string Major { get; set; }

        [MaxLength(50, ErrorMessage = "Điểm số tối đa 50 ký tự")]
        public string Score { get; set; }

        [MaxLength(128, ErrorMessage = "Tên lớp tối đa 128 ký tự")]
        public string Class { get; set; }

        public DateTime GraduateDate { get; set; }

        [MaxLength(4, ErrorMessage = "Năm tốt nghiệp tối đa 4 ký tự")]
        public string GraduateDateString { get; set; }

        [MaxLength(500, ErrorMessage = "Thông tin ghi chú tối đa 500 ký tự")]
        public string Note { get; set; }

        public List<StudyHistoryBo> ListStudyHistory { get; set; }
    }

    public class StudyAbroadModel
    {
        public int CustomerId { get; set; }

        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên trường")]
        [MinLength(4, ErrorMessage = "Tên trường tối thiểu 4 ký tự")]
        [MaxLength(128, ErrorMessage = "Tên trường tối đa 128 ký tự")]
        public string School { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên ngành học")]
        [MinLength(2, ErrorMessage = "Tên ngành học tối thiểu 2 ký tự")]
        [MaxLength(128, ErrorMessage = "Tên ngành học tối đa 128 ký tự")]
        public string Major { get; set; }

        public int CountryId { get; set; }

        /// <summary>
        /// Dùng cho trang CRM dành cho công ty
        /// </summary>
        public List<CountryCompanyBo> ListCountryCompany { get; set; }

        /// <summary>
        /// Dùng cho inside
        /// </summary>
        public List<CountryBo> ListCountry { get; set; }

        /// <summary>
        /// Bậc học
        /// </summary>
        public int Level { get; set; }

        public List<ConstantsValue> ListStudyLevel { get; set; }

        /// <summary>
        /// Năm dự định du học
        /// </summary>
        public int Year { get; set; }

        [MaxLength(128, ErrorMessage = "Thời gian du học tối đa 128 ký tự")]
        public string Time { get; set; }

        [MaxLength(500, ErrorMessage = "Thông tin ghi chú tối đa 500 ký tự")]
        public string Note { get; set; }

        public List<StudyAbroadBo> ListStudyAbroad { get; set; }
    }

    public class LanguageModel
    {
        public int CustomerId { get; set; }

        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên ngoại ngữ")]
        [MinLength(2, ErrorMessage = "Tên ngoại ngữ tối thiểu 2 ký tự")]
        [MaxLength(128, ErrorMessage = "Tên ngoại ngữ tối đa 128 ký tự")]
        public string Language { get; set; }

        public int Certificate { get; set; }
        public List<ConstantsValue> ListCertificate { get; set; }

        public string Score { get; set; }

        public DateTime RetestDate { get; set; }

        [MaxLength(10, ErrorMessage = "Ngày kiểm tra lại tối đa 10 ký tự")]
        public string RetestDateString { get; set; }

        [MaxLength(500, ErrorMessage = "Thông tin ghi chú tối đa 500 ký tự")]
        public string Note { get; set; }

        public List<LanguageBo> ListLanguage { get; set; }
    }
    public class ImportCustomerModel
    {
        public IList<ImportCustomerData> ListCustomer { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalRecord { get; set; }
        public string PagerString { get; set; }
        public string TotalString { get; set; }
    }

    [Serializable]
    public class ImportCustomerData
    {
        public int ID { get; set; }
        public string Gender { get; set; }
        public string FullName { get; set; }
        public string Birthday { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Province { get; set; }
        public string Content { get; set; }
        public string Source { get; set; }
        public string Note { get; set; }
        public string Country { get; set; }
        public string EducationLevel { get; set; }
        public string EmployeeCode {  get; set; }
    }
}