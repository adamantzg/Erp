using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace erp.Model
{
    public partial class Permission
    {
        public const int ITF_ViewAllInternalComments = 1;
        public const int ITF_Authorize = 2;
        public const int ITF_CloseFeedback = 3;
        public const int ITF_OpenFeedback = 4;
        public const int ITF_DeleteSubscriptions = 5;

        public const int CA_ViewAllInternalComments = 10;
        public const int CA_Authorize = 11;
        public const int CA_CloseFeedback = 12;
        public const int CA_OpenFeedback = 13;
        public const int CA_DeleteSubscriptions = 14;
    }
}
