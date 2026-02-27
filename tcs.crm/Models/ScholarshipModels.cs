
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using tcs.bo;

namespace tcs.crm.Models
{
    public class ScholarshipModel : BaseModel
    {
        public int ScholarshipType { get; set; }
        public List<ConstantsValue> ListScholarshipType { get; set; }
        public int CountryId { get; set; }
        public List<SelectListItem> ListCountry { get; set; }
        public int SchoolId { get; set; }
        public List<SelectListItem> ListSchool { get; set; }
        public List<ScholarshipBo> ListScholarship { get; set; }
        public AccountInfo AccountInfo { get; set; }
    }

    public class ScholarshipInsertModel : BaseInsertModel
    {
        public int CompanyId { get; set; }
        public List<SelectListItem> ListCountry { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên học bổng")]
        [MaxLength(255, ErrorMessage = "Tên học bổng tối đa 255 ký tự")]
        public string ScholarshipName { get; set; }

        [MaxLength(128, ErrorMessage = "Tên ngắn tối đa 128 ký tự")]
        public string ShortName { get; set; }

        public int Country { get; set; }
        public string CountryName { get; set; }
        public int School { get; set; }
        public string SchoolName { get; set; }
        public List<SelectListItem> ListSchool { get; set; }

        [MaxLength(500, ErrorMessage = "Thông tin yêu cầu học bổng tối đa 500 ký tự")]
        public string Require { get; set; }
        public int ScholarshipType { get; set; }
        public List<ConstantsValue> ListScholarshipType { get; set; }
        [MaxLength(128, ErrorMessage = "Thông tin giá trị học bổng tối đa 128 ký tự")]
        public string Amount { get; set; }
        public int Quantity { get; set; }
        public int TotalRegister { get; set; }
        public int TotalRemain { get; set; }
        public string ExpiredDateString { get; set; }
        [MaxLength(500, ErrorMessage = "Thông tin ghi chú tối đa 500 ký tự")]
        public string Note { get; set; }
    }
}