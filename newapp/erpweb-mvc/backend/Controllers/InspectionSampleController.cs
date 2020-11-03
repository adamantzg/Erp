using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace asaq2back.Controllers
{
    [Authorize]
    public class InspectionSampleController : BaseController
    {
        // GET: InspectionSample
        public ActionResult Index()
        {
            return View();
        }
    }
}