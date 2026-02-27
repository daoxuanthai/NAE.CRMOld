
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using tcs.bo;

namespace tcs.crm.Models
{
    public class CommissionInsertModel : BaseModel
    {
        public int Id { get; set; }
        public int ContractId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập nội dung thu")]
        [MaxLength(128, ErrorMessage = "Nội dung thu tối đa 128 ký tự")]
        [MinLength(10, ErrorMessage = "Nội dung thu tối thiểu 10 ký tự")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số tiền thu")]
        public decimal Commission { get; set; }
        public DateTime CommissionDate { get; set; }
        public string CommissionDateString { get; set; }
        public bool IsCollect { get; set; }
        public string Note { get; set; }
        public List<CommissionBo> ListCommission { get; set; }

        public CommissionInsertModel()
        {

        }
    }
}