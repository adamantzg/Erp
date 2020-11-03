using erp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using erp.Model.Dal.New;
using erp.DAL.EF.New;
using Utilities = company.Common.Utilities;

namespace backend.ApiServices
{
    public class AccountService : IAccountService
    {
        public const string CurrentUserSessionVar = "CurrentUser";

        private readonly ILoginhistoryDAL loginhistoryDAL;
        private readonly IUnitOfWork unitOfWork;
        private readonly IRoleDAL roleDAL;
        private readonly IUserDAL userDAL;
        private readonly IPermissionDAL permissionDAL;

        public AccountService(ILoginhistoryDAL loginhistoryDAL, IUnitOfWork unitOfWork, IRoleDAL roleDAL, IUserDAL userDAL,
            IPermissionDAL permissionDAL)
        {
            this.loginhistoryDAL = loginhistoryDAL;
            this.unitOfWork = unitOfWork;
            this.roleDAL = roleDAL;
            this.userDAL = userDAL;
            this.permissionDAL = permissionDAL;
        }

        public void LoginUser(User user)
        {
            var ctx = System.Web.HttpContext.Current;
            Login_history hist = new Login_history { login_username = user.username, user_id = user.company_id, login_date = DateTime.Now, ip_address = ctx.Request.UserHostAddress };
            loginhistoryDAL.Create(hist);
            ctx.Session["session_id"] = hist.session_id;
            ctx.Session["history_id"] = hist.history_id;
            ctx.Session["datenow"] = hist.login_date.Value.ToString("yyyy-MM-dd HH':'mm':'ss");
            ctx.Session["CurrentUser"] = user;
        }

        public User GetCurrentUser()
        {
            HttpContext ctx = HttpContext.Current;
            User user = (User)ctx.Session?[CurrentUserSessionVar];
            if (user != null && user.username == ctx.User.Identity.Name)
            {
                if (user.Roles == null)
                    user.Roles = roleDAL.GetRolesForUser(user.userid);
                if (user.Permissions == null)
                    user.Permissions = permissionDAL.GetForUser(user.userid);
                return user;
            }
            else
            {
                if (!string.IsNullOrEmpty(ctx.User.Identity.Name))
                {
                    user = userDAL.GetByLogin(ctx.User.Identity.Name);
                    ctx.Session[CurrentUserSessionVar] = user;
                    string message;
                    if (!ctx.Request.IsLocal)
                        return CheckIpAndTimeRestrictions(user, ctx, out message);
                    return user;

                }
                return null;
            }
        }

        public User CheckIpAndTimeRestrictions(User user, HttpContext ctx, out string message)
        {
            message = string.Empty;
            if (user == null || ctx == null)
                return null;
            long? number = null;
            string countryCode = string.Empty;

            var ipAddressStr = ctx.Request.UserHostAddress;
            var restrict = user.restrict_ip != 0 && user.ip_address != ipAddressStr && !ctx.Request.IsLocal;
            if (restrict)
            {
                number = Utilities.IpToLong(ipAddressStr);
                if (number != null)
                {
                    var ip2 = unitOfWork.Ip2Repository.Get(i => i.ip_from <= number && i.ip_to >= number).FirstOrDefault();
                    if (ip2 != null && ip2.country_code == "HR")
                    {
                        restrict = false;
                        countryCode = "HR";
                    }

                }

            }

            if (restrict)
                message = "IP address not allowed.";
            else if (countryCode != "HR")
            {
                //Check allowed days
                if (!string.IsNullOrEmpty(user.login_restriction_days))
                {
                    var days = user.login_restriction_days.Split(',').Select(int.Parse).ToList();
                    if (!days.Contains(Convert.ToInt32(DateTime.Today.DayOfWeek)))
                    {
                        message = "You are not allowed to log-in today";
                        return null;
                    }
                }

                if (user.login_restriction_from == null ||
                    DateTime.Now > (DateTime.Today + user.login_restriction_from.Value) &&
                    (user.login_restriction_to == null ||
                     DateTime.Now < (DateTime.Today + user.login_restriction_to.Value)))
                {
                    ctx.Session[CurrentUserSessionVar] = user;
                    return user;
                }
                message = string.Format("You are only allowed to log-in between {0} and {1}",
                    user.login_restriction_from, user.login_restriction_to);
            }
            return null;
        }
    }
}