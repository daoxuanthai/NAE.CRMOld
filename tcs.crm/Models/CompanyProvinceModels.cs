
using System.Collections.Generic;
using System.Web.Mvc;
using tcs.bo;

namespace tcs.crm.Models
{
    public class CompanyProvinceModel : BaseModel
    {
        public int OfficeId { get; set; }
        public List<SelectListItem> OfficeSelectList { get; set; }
        public List<CompanyOfficeBo> ListOffice { get; set; }
        public List<ProvinceCompanyBo> ListCompanyProvince { get; set; }
        public AccountInfo AccountInfo { get; set; }
        public CompanyProvinceModel()
        {
            OfficeId = -1;
        }
    }
}