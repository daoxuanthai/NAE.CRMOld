
using System;
using System.Collections.Generic;
using tcs.adapter.Sql;
using tcs.bo;
using tcs.lib;

namespace Notify
{
    class Program
    {
        static void Main(string[] args)
        {
            UpdateNotify();
        }

        /// <summary>
        /// Lấy DSKH đến hẹn chăm sóc hôm nay
        /// </summary>
        /// <returns></returns>
        public static void UpdateNotify()
        {
            var query = new CustomerQuery()
            {
                Keyword = "",
                From = DateTime.Today,
                To = DateTime.Now.AddDays(1),
                Status = "0,1,2,4",
                Country = "-1",
                Employee = "ADMIN",
                Company = "1005",
                Office = "-1",
                Page = 0,
                PageSize = 1000,
                Sort = CustomerSort.AlarmDate.Key
            };
            var result = CustomerDb.Instance.GetCustomerNotify(query);
            if (result != null)
            {
                var index = 1;
                foreach (var cus in result)
                {
                    
                    //if((cus.Status == CustomerStatus.NotCaring.Key || cus.Status == CustomerStatus.ContinueCare.Key ||
                    //    cus.Status == CustomerStatus.Potential.Key || cus.Status == CustomerStatus.MaybeContract.Key) 
                    //    && cus.AlarmTime != DateTime.MinValue && cus.IsAlarm)
                    //{
                    //    Console.WriteLine((index++).ToString() + ".Notify.ID: " + cus.Id);
                    //    var title = $"KH: {cus.Fullname} đã đến hạn chăm sóc hôm nay {DateTime.Now.ToString("dd/MM")}";
                    //    var content = $"KH: <b>{cus.Fullname}</b> đã đến hạn chăm sóc hôm nay <b>{DateTime.Now.ToString("dd/MM")}</b>";
                    //    NotifyDb.Instance.CreateNotify(cus.EmployeeId, title, content, NotifyType.Customer.Key, NotifyType.Customer.Value,
                    //                                        cus.Id, createUserId: -99, createUserName: "Administrator");
                    //}
                }
            }
            Console.WriteLine("DONE");
        }
    }
}
