using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace erp.Model
{
    public class UserGroup
    {
        public int id { get; set; }
        public string name { get; set; }
        public bool? returns_default { get; set; }

        public List<User> Users { get; set; }
    }
}
