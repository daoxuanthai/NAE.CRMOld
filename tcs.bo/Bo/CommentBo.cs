using System;

namespace tcs.bo
{
    [Serializable]
    public class CommentBo : BaseBo
    {
        public int CustomerId { get; set; }
        public int ObjectId { get; set; }
        public int ObjectType { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
    }

    [Serializable]
    public class CommentType : Constants
    {
        public static CommentType Instant()
        {
            return new CommentType();
        }

        public static ConstantsValue CustomerCare = new ConstantsValue(1, "Thông tin chăm sóc");
    }
}
