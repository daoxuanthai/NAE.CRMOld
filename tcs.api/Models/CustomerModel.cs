
namespace tcs.api.Models
{
    public class Customer
    {
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Question { get; set; }
        public int Province { get; set; }
        public int Country { get; set; }
        public int PartnerId { get; set; }
        public string Url { get; set; }
        public bool IsParent { get; set; }
        public bool IsContact { get; set; }
        public bool IsCallInfo { get; set; }
    }
}