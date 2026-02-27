namespace tcs.bo
{
    public class ConstantsValue
    {
        public int Key
        {
            get;
            set;
        }
        public string Value
        {
            get;
            set;
        }

        public ConstantsValue()
        {
            Key = 0;
            Value = "";
        }

        public ConstantsValue(int key, string value)
        {
            Key = key;
            Value = value;
        }
    }
}
