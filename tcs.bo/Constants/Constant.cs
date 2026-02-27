using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace tcs.bo
{
    public class Constants
    {
        public string GetValueByKey(int key)
        {
            const string val = "";
            FieldInfo[] p = GetType().GetFields(BindingFlags.Public | BindingFlags.Static);
            foreach (FieldInfo f in p)
            {
                var t = (ConstantsValue)f.GetValue(null);
                if (t != null && t.Key == key)
                    return t.Value;
            }
            return val;
        }

        public List<ConstantsValue> GetAll(bool all = false, bool undefined = false)
        {

            FieldInfo[] p = GetType().GetFields(BindingFlags.Public | BindingFlags.Static);
            var tmp = p.Select(f => (ConstantsValue)f.GetValue(null)).Where(t => t != null).ToList();
            if (all)
            {
                tmp.Insert(0, new ConstantsValue(-1, "Tất cả"));
            }
            if (undefined)
            {
                tmp.Insert(0, new ConstantsValue(-1, "Undefined"));
            }
            return tmp;
        }

        public List<ConstantsValue> GetAllByKey(params int[] keys)
        {
            FieldInfo[] p = GetType().GetFields(BindingFlags.Public | BindingFlags.Static);
            return p.Select(f => (ConstantsValue)f.GetValue(null)).Where(t => t != null && keys.Contains(t.Key)).ToList();
        }
    }
}
