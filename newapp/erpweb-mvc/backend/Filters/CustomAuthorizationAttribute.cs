using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace backend
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class CustomAuthorizationAttribute : AuthorizeAttribute
    {
        public string LoginPage { get; set; }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            base.HandleUnauthorizedRequest(filterContext);
        }
               
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);
            if (filterContext.Result is HttpUnauthorizedResult)
            {
                filterContext.Result = new RedirectToRouteResult(
                    new System.Web.Routing.RouteValueDictionary 
                    { 
                            { "language", filterContext.RouteData.Values[ "language" ] }, 
                            { "controller", "Account" }, 
                            { "action", "LogOn" }, 
                            { "ReturnUrl", filterContext.HttpContext.Request.RawUrl } 
                    });
            }
        }
    }
}