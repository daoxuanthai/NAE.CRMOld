using System;

namespace tcs.bo
{
    [Serializable]
    public class NotifyBo : BaseBo
    {
        public int TitleId { get; set; }
        public int ObjectId { get; set; }
        public int ObjectType { get; set; }
        public string ObjectTypeName { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public bool IsRead { get; set; }
        public int Type { get; set; }
        public int Priority { get; set; }
    }

    [Serializable]
    public class NotifyQuery : IQuery
    {
        public int TitleId { get; set; }
        public int ObjectType { get; set; }
        public int Type { get; set; }
        public bool IsRead { get; set; }
        public int NotRead { get; set; }
    }

    [Serializable]
    public class NotifyType : Constants
    {
        public static NotifyType Instant()
        {
            return new NotifyType();
        }

        public static ConstantsValue Customer = new ConstantsValue(1, "Thông tin khách hàng");
        public static ConstantsValue Contract = new ConstantsValue(2, "Thông tin hợp đồng");
    }

    [Serializable]
    public class NotifyUrl : Constants
    {
        public static NotifyUrl Instant()
        {
            return new NotifyUrl();
        }

        public static ConstantsValue Customer = new ConstantsValue(1, "/Customer");
        public static ConstantsValue Contract = new ConstantsValue(2, "/Contract");
    }
}
