using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace erp.Model
{
    public partial class Role
    {
        public const int FCOfficeUser = 1;
        public const int FCOfficeAdmin = 2;
        public const int ITAdmin = 3;
        public const int ITUser = 4;
        public const int CAAdmin = 5;
        public const int CAUser = 6;
        public const int FC_SI_Approver = 9;

        public ICollection<Permission> Permissions { get; set; }
        public ICollection<User> Users { get; set; }
    }
}
