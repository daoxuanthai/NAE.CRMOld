
using System;

namespace tcs.bo
{
    [Serializable]
    public class ContractProcessBo : BaseBo
    {
        public int ContractId { get; set; }

        /// <summary>
        /// Lưu những thông tin cần thiết về hợp đồng để ghi log lại
        /// </summary>
        public string ContractData { get; set; }

        public string ProcessNote { get; set; }

    }

    [Serializable]
    public class ContractLogInfo
    {
        public DateTime ContractDate { get; set; }
        public DateTime VisaDate { get; set; }
        public int Status { get; set; }
        public bool IsVisa { get; set; }
        public decimal Deposit { get; set; }
        public decimal ServiceFee { get; set; }
        public decimal CollectOne { get; set; }
        public decimal CollectTwo { get; set; }
        public bool IsRefund { get; set; }
        public DateTime RefundDate { get; set; }
        public string Promotion { get; set; }
        public int EmployeeId { get; set; }
        public decimal TotalCommission { get; set; }

        public static ContractLogInfo FromContractBo(ContractBo contract)
        {
            if (contract == null)
                return null;

            return new ContractLogInfo()
            {
                CollectOne = contract.CollectOne,
                CollectTwo = contract.CollectTwo,
                ContractDate = contract.ContractDate.Value,
                Deposit = contract.Deposit,
                EmployeeId = contract.EmployeeId,
                IsRefund= contract.IsRefund,
                IsVisa=contract.IsVisa,
                Promotion=contract.Promotion,
                RefundDate=contract.RefundDate.Value,
                ServiceFee=contract.ServiceFee,
                Status=contract.Status,
                TotalCommission=contract.TotalCommission,
                VisaDate=contract.VisaDate.Value
            };
        }
    }
}
