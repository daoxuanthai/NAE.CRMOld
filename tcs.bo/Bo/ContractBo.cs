
using System;

namespace tcs.bo
{
    [Serializable]
    public class ContractBo : BaseBo
    {
        public int CustomerId { get; set; }

        public int ProfileTypeId { get; set; }

        public DateTime? ContractDate { get; set; }

        public DateTime? VisaDate { get; set; }

        public int Status { get; set; }

        public bool IsVisa { get; set; }

        public decimal Deposit { get; set; }

        public decimal ServiceFee { get; set; }

        public decimal CollectOne { get; set; }

        public decimal CollectTwo { get; set; }

        public string Currency { get; set; }

        public bool IsRefund { get; set; }

        public DateTime? RefundDate { get; set; }

        public string Promotion { get; set; }

        public string AttachFile { get; set; }

        public string Note { get; set; }

        public string ProcessNote { get; set; }

        public int EmployeeId { get; set; }

        public string EmployeeName { get; set; }

        public decimal TotalCommission { get; set; }

        public string ContractDateString { get; set; }

        public string VisaDateString { get; set; }

        public string RefundDateString { get; set; }

        public int PercentProcessing { get; set; }

        public int SchoolId { get; set; }

        public string SchoolName { get; set; }

        public int ScholarshipId { get; set; }

        public string ScholarshipName { get; set; }

    }

    [Serializable]
    public class ContractQuery : IQuery
    {
        public string EmployeeProcessId { get; set; }
        public string CountryId { get; set; }
    }

    [Serializable]
    public class ContractStatus : Constants
    {
        public static ContractStatus Instant()
        {
            return new ContractStatus();
        }

        public static ConstantsValue New = new ConstantsValue(0, "Mới ký hợp đồng");
        public static ConstantsValue Process = new ConstantsValue(1, "Đang xử lý");
        public static ConstantsValue VisaFail = new ConstantsValue(2, "Rớt visa");
        public static ConstantsValue Complete = new ConstantsValue(3, "Hoàn tất");
        public static ConstantsValue Liquidated = new ConstantsValue(4, "Thanh lý");
    }

    [Serializable]
    public class ContractSE
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public int OfficeId { get; set; }
        public int CustomerId { get; set; }
        public int ProvinceId { get; set; }
        public int[] CountryId { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string SearchInfo { get; set; }
        public string SearchInfoEn { get; set; }
        public int EmployeeId { get; set; }
        public int EmployeeProcessId { get; set; }
        public string EmployeeProcessName { get; set; }
        public int Status { get; set; }
        public decimal Deposit { get; set; }
        public decimal ServiceFee { get; set; }
        public decimal CollectOne { get; set; }
        public decimal CollectTwo { get; set; }
        public bool IsRefund { get; set; }
        public DateTime RefundDate { get; set; }
        public DateTime ContractDate { get; set; }
        public bool IsVisa { get; set; }
        public DateTime VisaDate { get; set; }
        public decimal TotalCommission { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public int NewsIdRef { get; set; }
        public string NewsUrlRef { get; set; }
        public int SeminarIdRef { get; set; }
        public int UserIdRef { get; set; }
        public int ProfileTypeId { get; set; }
        public int PercentProcessing { get; set; }
    }

    [Serializable]
    public class ContractSummaryReportData
    {
        public DateTime Date { get; set; }
        public int Total { get; set; }
        public int TotalContract { get; set; }
        public int TotalNotContract { get; set; }
    }

    [Serializable]
    public class ContractStatusReportData
    {
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public DateTime Date { get; set; }
        public int Total { get; set; }
        public int TotalNew { get; set; }
        public int TotalProcess { get; set; }
        public int TotalFail { get; set; }
        public int TotalComplete { get; set; }
        public int TotalRefund { get; set; }
    }

    [Serializable]
    public class ContractCountryReportData
    {
        public int CountryId { get; set; }
        public string CountryName { get; set; }
        public int Total { get; set; }
    }
}
