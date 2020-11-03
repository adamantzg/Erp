using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace backend.Controllers
{
    [Authorize]
    public class FormsController : BaseController
    {
        // GET: Forms
        public ActionResult Index()
        {
            return View();
        }
    }
}