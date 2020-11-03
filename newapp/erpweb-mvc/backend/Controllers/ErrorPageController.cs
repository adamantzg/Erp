using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace backend.Controllers
{
    public class ErrorPageController : Controller
    {
        //
        // GET: /ErrorPage/
        [AllowAnonymous]
        public ActionResult Error(int statusCode, Exception exception)
        {
            ViewBag.exception = exception;
            return View();
        }

    }
}
