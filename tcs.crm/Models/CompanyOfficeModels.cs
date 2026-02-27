using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using tcs.bo;

namespace tcs.crm.Models
{
    public class CompanyOfficeModel : BaseModel
    {
        public CompanyOfficeModel()
        {
            ProvinceId = -1;
        }

        public int ProvinceId { get; set; }
        public List<SelectListItem> ListProvince { get; set; }
        public bool IsLock { get; set; }

        public List<CompanyOfficeBo> ListCompanyOffice { get; set; }
    }

    public class CompanyOfficeInsertModel : BaseInsertModel
    {
        public int CompanyId { get; set; }
        public int ProvinceId { get; set; }
        public List<ProvinceBo> ListProvince { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên văn phòng")]
        [MinLength(5, ErrorMessage = "Tên văn phòng tối thiểu 5 ký tự")]
        [MaxLength(128, ErrorMessage = "Tên văn phòng tối đa 128 ký tự")]
        public string OfficeName { get; set; }

        [MaxLength(255, ErrorMessage = "Địa chỉ văn phòng tối đa 255 ký tự")]
        public string OfficeAddress { get; set; }

        [MaxLength(50, ErrorMessage = "Điện thoại văn phòng tối đa 50 ký tự")]
        public string OfficePhone { get; set; }

        [MaxLength(50, ErrorMessage = "Email văn phòng tối đa 50 ký tự")]
        public string OfficeEmail { get; set; }

        [MaxLength(500, ErrorMessage = "Thông tin ghi chú tối đa 500 ký tự")]
        public string Note { get; set; }

        public int DirectorUserId { get; set; }
        public List<SelectListItem> ListCompanyTitle { get; set; }

        public int Status { get; set; }

        public List<ConstantsValue> ListStatus { get; set; }

        public List<CompanyOfficeBo> ListCompanyOffice { get; set; }
    }
}