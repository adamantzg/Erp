using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using erp.DAL.EF.New;
using erp.Model;
using backend.Controllers;
using Elmah;

//using FluentValidation.Mvc;
using Newtonsoft.Json;
using System.Web.SessionState;
using Elmah.Contrib.WebApi;

namespace backend
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            
            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new RazorViewEngine());
            AuthConfig.RegisterAuth();
            Configuration.Initialize(DependencyResolver.Current.GetService<IUnitOfWork>(),Properties.Settings.Default.ApplicationId);
            //FluentValidationModelValidatorProvider.Configure();
            GlobalConfiguration.Configuration.EnsureInitialized();
            GlobalConfiguration.Configuration.Filters.Add(new ElmahHandleErrorApiAttribute());

#if DEBUG
            BundleTable.EnableOptimizations = true;
#else
            BundleTable.EnableOptimizations = true;
#endif

        }

        protected void Application_PostAuthorizeRequest()
        {
            if (IsWebApiRequest()) {
                HttpContext.Current.SetSessionStateBehavior(SessionStateBehavior.Required);
            }
        }

        private bool IsWebApiRequest()
        {
            return HttpContext.Current.Request.AppRelativeCurrentExecutionFilePath.StartsWith(WebApiConfig.UrlPrefixRelative);
        }

        

        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        void ErrorMail_Filtering(object sender, ExceptionFilterEventArgs e)
        {
            if(e.Exception.GetBaseException() is HttpException || (e.Context as HttpContext).Request.IsLocal)
                e.Dismiss();
        }
    }
}