using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using tcs.bo;

namespace tcs.crm.Models
{
    public class SeminarModel : BaseModel
    {
        public string Status { get; set; }
        public List<ConstantsValue> ListStatus { get; set; }

        public string ProvinceId { get; set; }
        public List<SelectListItem> ListProvince { get; set; }
        public List<SeminarBo> ListSeminar { get; set; }

        public AccountInfo AccountInfo { get; set; }

        public SeminarModel()
        {
            Status = "-1";
        }
    }

    public class SeminarInsertModel : BaseInsertModel
    {
        [Required(ErrorMessage = "Vui lòng nhập tên hội thảo")]
        [MinLength(2, ErrorMessage = "Tên hội thảo tối thiểu 2 ký tự")]
        [MaxLength(128, ErrorMessage = "Tên hội thảo không được vượt quá 128 ký tự")]
        public string Name { get; set; }

        [MaxLength(500, ErrorMessage = "Thông tin mô tả không được vượt quá 500 ký tự")]
        public string Description { get; set; }

        [MaxLength(500, ErrorMessage = "Thông tin ghi chú không được vượt quá 500 ký tự")]
        public string Note { get; set; }

        [MaxLength(255, ErrorMessage = "Link bài tin hội thảo không được vượt quá 255 ký tự")]
        public string Link { get; set; }

        public int Status { get; set; }

        public List<ConstantsValue> ListStatus { get; set; }

        public SeminarBo SeminarInfo { get; set; }

        public SeminarPlaceInsertModel SeminarPlaceModel { get; set; }
    }

    public class SeminarPlaceInsertModel : BaseInsertModel
    {
        public int SeminarId { get; set; }
        public int ProvinceId { get; set; }
        public List<SelectListItem> ListProvince { get; set; }
        public string Place { get; set; }
        public string Address { get; set; }
        public DateTime SeminarDate { get; set; }
        public string SeminarDateString { get; set; }
        public string Note { get; set; }
        public int Status { get; set; }
        public List<ConstantsValue> ListStatus { get; set; }
        public List<SeminarPlaceBo> ListSeminarPlace { get; set; }
    }
}
