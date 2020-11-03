using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace erp.Model
{
    public class Lookup
    {
        public int Id { get; set; }
        public string Title { get; set; }
    }

    [Serializable]
    public class LookupItem
    {
        public string value { get; set; }
        public int id { get; set; }

        public LookupItem()
        {
        }

        public LookupItem(int id, string value)
        {
            this.id = id;
            this.value = value;
        }
    }

    public class LookupItemComparer : IEqualityComparer<LookupItem>
    {
        
        public bool Equals(LookupItem x, LookupItem y)
        {
            return x.id == y.id;
        }

        public int GetHashCode(LookupItem obj)
        {
            return obj.id;
        }
    }

    public class OLookupItem
    {
        public object id { get; set; }
        public string value { get; set; }
    }
}
