
using System;
using System.Collections.Generic;

namespace tcs.crm.Models
{
    [Serializable]
    public class CustomerStatusRptResult
    {
        public int Total { get; set; }
        public int NotCaring { get; set; }
        public int ContinueCare { get; set; }
        public int Potential { get; set; }
        public int NotPotential { get; set; }
        public int MaybeContract { get; set; }
        public int Contracted { get; set; }
    }

    [Serializable]
    public class ContractStatusRptResult
    {
        public int Total { get; set; }
        public int New { get; set; }
        public int Process { get; set; }
        public int VisaFail { get; set; }
        public int Complete { get; set; }
        public int Liquidated { get; set; }
    }

    [Serializable]
    public class CustomerRptResult
    {
        public int Total { get; set; }
        public List<ReportData> ListData { get; set; }
    }

    [Serializable]
    public class ReportData
    {
        public string Name { get; set; }
        public int Count { get; set; }
    }

    [Serializable]
    public class LineChartModel
    {
        public int Total { get; set; }
        public int TotalContract { get; set; }
        /// <summary>
        /// Danh sách ngày cách nhau bằng dấu ,
        /// </summary>
        public string Categories { get; set; }

        public Series Series { get; set; }
    }

    [Serializable]
    public class Series
    {
        public List<Item> Items { get; set; }
    }

    [Serializable]
    public class Item
    {
        public string Name { get; set; }
        /// <summary>
        /// Các giá trị theo ngày phân cách bằng dấu ,
        /// </summary>
        public string Data { get; set; }
    }
}