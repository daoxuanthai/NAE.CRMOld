
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using tcs.bo;

namespace tcs.crm.Models
{
    public class ProfileTypeModel : BaseModel
    {
        public string CountryId { get; set; }
        public List<SelectListItem> ListCountry { get; set; }
        public AccountInfo AccountInfo { get; set; }
        public List<ProfileTypeBo> ListProfileType { get; set; }
    }

    public class ProfileTypeInsertModel : BaseInsertModel
    {
        public int CompanyId { get; set; }

        public int Country { get; set; }

        public List<SelectListItem> ListCountry { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên loại hồ sơ")]
        [MaxLength(128, ErrorMessage = "Tên loại hồ sơ tối đa 128 ký tự")]
        public string TypeName { get; set; }

        [MaxLength(500, ErrorMessage = "Thông tin ghi chú tối đa 500 ký tự")]
        public string Note { get; set; }

        public List<ProfileStepBo> ListProfileStep { get; set; }
        public List<ProfileDocumentBo> ListProfileDocument { get; set; }
    }
}