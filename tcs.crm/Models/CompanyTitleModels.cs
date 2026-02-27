
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using tcs.bo;

namespace tcs.crm.Models
{
    public class CompanyTitleModel : BaseModel
    {
        public CompanyTitleModel()
        {
            UserType = -1;
            OfficeId = -1;
        }

        public int OfficeId { get; set; }

        public List<SelectListItem> ListOffice { get; set; }

        public int UserType { get; set; }

        public List<ConstantsValue> ListUserType { get; set; }

        public bool IsLock { get; set; }

        public List<CompanyTitleBo> ListCompanyTitle { get; set; }
        public AccountInfo AccountInfo { get; set; }
    }

    public class CompanyTitleInsertModel : BaseInsertModel
    {
        public int CompanyId { get; set; }
        public int OfficeId { get; set; }

        public List<CompanyOfficeBo> ListOffice { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên vị trí")]
        [MaxLength(128, ErrorMessage = "Tên vị trí tối đa 128 ký tự")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên viết tắt")]
        [MaxLength(50, ErrorMessage = "Tên vị trí tối đa 50 ký tự")]
        public string Code { get; set; }

        public int UserId { get; set; }

        public string UserName { get; set; }

        public List<SelectListItem> ListUser { get; set; }

        [MaxLength(500, ErrorMessage = "Thông tin ghi chú tối đa 500 ký tự")]
        public string Note { get; set; }

        public int UserType { get; set; }

        public List<ConstantsValue> ListUserType { get; set; }

        public bool IsLock { get; set; }

        public bool IsViewAll { get; set; }

        public List<CompanyTitleBo> ListCompanyTitle { get; set; }
    }
}