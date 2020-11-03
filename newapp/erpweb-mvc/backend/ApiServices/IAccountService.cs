using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using erp.Model;

namespace backend.ApiServices
{
    public interface IAccountService
    {
        void LoginUser(User user);
        User GetCurrentUser();
        User CheckIpAndTimeRestrictions(User user, HttpContext ctx, out string message);
    }
}
