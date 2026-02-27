using System;

namespace tcs.bo
{
    [Serializable]
    public class BaseBo
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public int CreateUserId { get; set; }
        public string CreateUserName { get; set; }
        public int UpdateUserId { get; set; }
        public string UpdateUserName { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
    }
}
