using System;
namespace tcs.bo
{
    [Serializable]
    public class UserBo : BaseBo
    {
        public string UserName { get; set; }
        public string Avatar { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Note { get; set; }
        public bool IsLock { get; set; }
        public int UserType { get; set; }
    }

    [Serializable]
    public class UserQuery : IQuery
    {
        public int IsLock { get; set; }
        public UserQuery()
        {
            IsLock = -1;
            From = new DateTime(2000, 1, 1);
            To = new DateTime(2099, 1, 1);
        }
    }
}
