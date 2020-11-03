using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


using erp.Model;

using backend.Models;
using backend.Properties;
using Utilities = company.Common.Utilities;

namespace backend.Controllers
{
    [Authorize]
    public class ManualV2Controller : BaseController
    {
        

        public ActionResult Index()
        {
            return View();
        }
    }
}