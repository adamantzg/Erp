using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using erp.Model;

namespace backend.Controllers
{
    public class TestController : Controller
    {
        //
        // GET: /Test/

        public ActionResult Index()
        {
            //var data = Product_app.GetAll_TechnicalDataType();
            return View();
        }

        [AllowAnonymous]
        public ActionResult ToPdf(string url, string filename, string options = "debug=true,scale=0.78, leftmargin=22,rightmargin=22,media=1,timeout=300")
        {
            var pdf = new ASPPDFLib.PdfManager();
            var doc = pdf.CreateDocument();
            var log = doc.ImportFromUrl(url, options);
            var s = doc.SaveToMemory();
            return File((byte[])s, "application/pdf",filename);
        }

    }
}
