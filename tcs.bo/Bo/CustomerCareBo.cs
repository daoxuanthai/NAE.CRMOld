using System;

namespace tcs.bo
{
    [Serializable]
    public class CustomerCareBo : BaseBo
    {
        public int CustomerId { get; set; }
        public string Advisory { get; set; }
        public bool IsAlarm { get; set; }
        public DateTime AlarmTime { get; set; }
        public string AlarmTimeString { get; set; }
    }

    [Serializable]
    public class CustomerCareQuery : IQuery
    {

    }
}
