
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using tcs.bo;

namespace tcs.crm.Models
{
    public class SchoolModel : BaseModel
    {
        public int SchoolType { get; set; }
        public List<ConstantsValue> ListSchoolType { get; set; }
        public int CountryId { get; set; }
        public List<SelectListItem> ListCountry { get; set; }
        public List<SchoolBo> ListSchool { get; set; }
        public AccountInfo AccountInfo { get; set; }
    }

    public class SchoolInsertModel : BaseInsertModel
    {
        public int CompanyId { get; set; }
        public List<SelectListItem> ListCountry { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên trường")]
        [MaxLength(255, ErrorMessage = "Tên trường tối đa 255 ký tự")]
        public string SchoolName { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên viết tắt")]
        [MaxLength(128, ErrorMessage = "Tên vị trí tối đa 128 ký tự")]
        public string ShortName { get; set; }

        public int Country { get; set; }
        public string CountryName { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên thành phố")]
        [MaxLength(128, ErrorMessage = "Tên thành phố tối đa 128 ký tự")]
        public string City { get; set; }

        [MaxLength(128, ErrorMessage = "Tên hiệp hội tối đa 128 ký tự")]
        public string Association { get; set; }

        [MaxLength(255, ErrorMessage = "Địa chỉ tối đa 255 ký tự")]
        public string Address { get; set; }

        [MaxLength(255, ErrorMessage = "Địa chỉ tối đa 255 ký tự")]
        public string VnAddress { get; set; }

        [MaxLength(50, ErrorMessage = "Tên người liên hệ tối đa 50 ký tự")]
        public string ContactName { get; set; }

        [MaxLength(50, ErrorMessage = "Email liên hệ tối đa 50 ký tự")]
        public string ContactEmail { get; set; }

        [MaxLength(128, ErrorMessage = "Điện thoại liên hệ tối đa 128 ký tự")]
        public string ContactPhone { get; set; }

        [MaxLength(3000, ErrorMessage = "Chương trình nổi bật tối đa 3000 ký tự")]
        public string HotProgram { get; set; }

        [MaxLength(255, ErrorMessage = "Bậc đào tạo tối đa 255 ký tự")]
        public string EducationLevel { get; set; }

        [MaxLength(255, ErrorMessage = "Thông tin kỳ nhập học tối đa 1000 ký tự")]
        public string CouseOpenTime { get; set; }
        public int SchoolType { get; set; }
        public List<ConstantsValue> ListSchoolType { get; set; }
        public bool IsStrategy { get; set; }

        [MaxLength(500, ErrorMessage = "Thông tin ghi chú tối đa 500 ký tự")]
        public string Note { get; set; }
    }
}