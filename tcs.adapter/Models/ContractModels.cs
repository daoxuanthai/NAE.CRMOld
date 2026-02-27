
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;
using tcs.adapter.Models;
using tcs.bo;
using tcs.lib;

namespace tcs.crm.Models
{
    public class ContractModel : BaseModel
    {
        public string Status { get; set; }
        public List<ConstantsValue> ListStatus { get; set; }

        public string CountryId { get; set; }
        public List<SelectListItem> ListCountryCompany { get; set; }

        /// <summary>
        /// ID title thuộc công ty
        /// </summary>
        public string EmployeeProcessId { get; set; }

        public List<SelectListItem> ListCompanyTitle { get; set; }

        public string OfficeId { get; set; }

        public List<SelectListItem> ListOffice { get; set; }

        /// <summary>
        /// Lấy dữ liệu được lưu trong elastic hiển thị lên
        /// </summary>
        public List<ContractSE> ListContract { get; set; }

        public AccountInfo AccountInfo { get; set; }

        public ContractModel()
        {
            Status = "-1";
            OfficeId = "-1";
            EmployeeProcessId = "-1";
            FromDate = DateTime.Now.AddYears(-1).ToDateString();
        }
    }

    public class ContractInsertModel : BaseInsertModel
    {
        public int CustomerId { get; set; }
        public DateTime ContractDate { get; set; }

        [MaxLength(10, ErrorMessage = "Ngày ký hợp đồng không hợp lệ")]
        public string ContractDateString { get; set; }

        public DateTime VisaDate { get; set; }

        [MaxLength(10, ErrorMessage = "Ngày làm visa không hợp lệ")]
        public string VisaDateString { get; set; }

        public int Status { get; set; }

        public List<ConstantsValue> ListStatus { get; set; }

        public int ProfileTypeId { get; set; }

        public List<ProfileTypeBo> ListProfileType { get; set; }

        public bool IsVisa { get; set; }

        public decimal Deposit { get; set; }

        public decimal ServiceFee { get; set; }

        public decimal CollectOne { get; set; }

        public decimal CollectTwo { get; set; }

        public string Currency { get; set; }

        public decimal TotalCommission { get; set; }

        public bool IsRefund { get; set; }

        public DateTime RefundDate { get; set; }

        [MaxLength(10, ErrorMessage = "Ngày hoàn tiền cọc không hợp lệ")]
        public string RefundDateString { get; set; }

        [MaxLength(255, ErrorMessage = "Thông tin khuyến mãi tối đa 255 ký tự")]
        public string Promotion { get; set; }

        [MaxLength(128, ErrorMessage = "File đính kèm tối đa 128 ký tự")]
        public string AttachFilePath { get; set; }

        public HttpPostedFileBase AttachFile { get; set; }

        [MaxLength(500, ErrorMessage = "Thông tin ghi chú tối đa 500 ký tự")]
        public string Note { get; set; }

        /// <summary>
        /// Tỉ lệ hoàn thành hồ sơ theo checklist
        /// </summary>
        public int Percent { get; set; }

        public string ProcessNote { get; set; }

        /// <summary>
        /// ID nhân viên xử lý hợp đồng
        /// </summary>
        public int EmployeeId { get; set; }

        /// <summary>
        /// Tên nhân viên xử lý hợp đồng
        /// </summary>
        public string EmployeeName { get; set; }

        /// <summary>
        /// Danh sách nhân viên theo công ty, văn phòng
        /// </summary>
        public List<CompanyTitleBo> ListEmployee { get; set; }

        public List<SelectListItem> ListEmployeeWithName { get; set; }

        public int SchoolId { get; set; }

        public List<SelectListItem> ListSchool { get; set; }

        public int ScholarshipId { get; set; }

        public List<SelectListItem> ListScholarship { get; set; }
    }

    public class ContractDetailModel
    {
        public int Id { get; set; }
        public ContractInsertModel ContractInfo { get; set; }
        public CommissionInsertModel CommissionInfo { get; set; }
        public List<ContractProcessBo> ContractProcessInfo { get; set; }
        /// <summary>
        /// Thông tin các giai đoạn và thông tin giấy tờ của học sinh
        /// </summary>
        public DocumentModels DocumentInfo { get; set; }
    }

}