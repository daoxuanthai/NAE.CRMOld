using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using tcs.bo;

namespace tcs.crm.Models
{
    public class SeminarRegisterModel : BaseModel
    {
        public int CustomerId { get; set; }
        public int SeminarId { get; set; }
        public string SeminarName { get; set; }
        public int SeminarPlaceId { get; set; }
        public List<SelectListItem> ListSeminarPlace { get; set; }

        public List<SeminarRegisterBo> ListSeminarRegister { get; set; }

        public int IsAttend { get; set; }

        public AccountInfo AccountInfo { get; set; }

        public int TitleId { get; set; }
        public List<SelectListItem> ListTitle { get; set; }

        public SeminarRegisterModel()
        {

        }
    }

    public class SeminarRegisterInsertModel : BaseInsertModel
    {
        public int SeminarId { get; set; }
        public string SeminarName { get; set; }
        public List<SelectListItem> ListSeminar { get; set; }
        public int SeminarPlaceId { get; set; }
        public List<SelectListItem> ListSeminarPlace { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }
        public string CustomerEmail { get; set; }

        [MaxLength(500, ErrorMessage = "Thông tin ghi chú khách hàng không được vượt quá 500 ký tự")]
        public string CustomerNote { get; set; }

        [MaxLength(500, ErrorMessage = "Thông tin ghi chú không được vượt quá 500 ký tự")]
        public string Note { get; set; }

        public bool IsAttend { get; set; }
        public CustomerBo CustomerInfo { get; set; }
        public SeminarBo SeminarInfo { get; set; }
        public SeminarPlaceBo SeminarPlaceInfo { get; set; }
        public string TicketId { get; set; }
        public string School1 { get; set; }
        public string School1Time { get; set; }
        public string School2 { get; set; }
        public string School2Time { get; set; }
        public string School3 { get; set; }
        public string School3Time { get; set; }

        /// <summary>
        /// Danh sách đăng ký hội thảo của khách hàng (sử dụng trong trang quản lý khách hàng detail)
        /// </summary>
        public List<SeminarRegisterBo> ListSeminarRegister { get; set; }
    }
}
