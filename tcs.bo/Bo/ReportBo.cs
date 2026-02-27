
using System;
using tcs.lib;

namespace tcs.bo
{
    [Serializable]
    public class DailyReportData
    {
        public string Date { get; set; }
        public int Total { get; set; }
        public int NotCaring { get; set; }
        public int ContinueCare { get; set; }
        public int Potential { get; set; }
        public int NotPotential { get; set; }
        public int MaybeContract { get; set; }
        public int Contracted { get; set; }
    }

    [Serializable]
    public class ContractDailyReportData
    {
        public string Date { get; set; }
        public int Total { get; set; }
        public int New { get; set; }
        public int Process { get; set; }
        public int VisaFail { get; set; }
        public int Complete { get; set; }
        public int Liquidated { get; set; }
    }

    [Serializable]
    public class CustomerReportQuery
    {
        public int[] CompanyId { get; set; }
        public int[] OfficeId { get; set; }
        public int[] ProvinceId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int[] EmployeeId { get; set; }
        public int[] EmployeeProcessId { get; set; }
        public int[] UserIdRef { get; set; }
        public int[] NewsIdRef { get; set; }
        public int[] SeminarIdRef { get; set; }
        public ReportType ReportType { get; set; }

        public CustomerReportQuery()
        {
            CompanyId = new int[] { };
            OfficeId = new int[] { };
            ProvinceId = new int[] { };
            EmployeeId = new int[] { };
            EmployeeProcessId = new int[] { };
            UserIdRef = new int[] { };
            NewsIdRef = new int[] { };
            SeminarIdRef = new int[] { };
            ReportType = ReportType.Daily;
            FromDate = DateTime.Today.AddDays(-7);
            ToDate = DateTime.Today;
        }
    }

    [Serializable]
    public class ContractReportQuery
    {
        public int[] CompanyId { get; set; }
        public int[] OfficeId { get; set; }
        public int[] ProvinceId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int[] EmployeeId { get; set; }
        public int[] EmployeeProcessId { get; set; }
        public int[] UserIdRef { get; set; }
        public int[] NewsIdRef { get; set; }
        public int[] SeminarIdRef { get; set; }
        public ReportType ReportType { get; set; }

        public ContractReportQuery()
        {
            CompanyId = new int[] { };
            OfficeId = new int[] { };
            ProvinceId = new int[] { };
            EmployeeId = new int[] { };
            EmployeeProcessId = new int[] { };
            UserIdRef = new int[] { };
            NewsIdRef = new int[] { };
            SeminarIdRef = new int[] { };
            ReportType = ReportType.Daily;
            FromDate = DateTime.Today.AddDays(-7);
            ToDate = DateTime.Today;
        }
    }
}
