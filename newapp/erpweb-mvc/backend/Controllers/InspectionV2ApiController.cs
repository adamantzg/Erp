using erp.DAL.EF.New;
using erp.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;

namespace backend.Controllers
{
    [RoutePrefix("api/inspectionv2")]
    public class InspectionV2ApiController : ApiController
    {
        private readonly IUnitOfWork unitOfWork;

        public InspectionV2ApiController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        [Route("getEditModel")]
        [HttpGet]
        public object GetEditModel(int id)
        {
            return new
            {
                insp = unitOfWork.InspectionV2Repository.Get(i => i.id == id, includeProperties: "Factory, InspectionType,Client").FirstOrDefault(),
                rootFolder = Properties.Settings.Default.InspectionImagesFolder
            };
        }

        [Route("updateDrawing")]
        [HttpPost]
        public Inspection_v2 UpdateDrawing(Inspection_v2 insp)
        {
            if (!string.IsNullOrEmpty(insp.drawingFile))
            {
                
                var file = WebUtilities.GetTempFile(insp.file_id);
                if (file != null)
                {
                    var inspectionRootRelativeFolder = InspectionV2Controller.GetInspectionImagesFolder(insp, Properties.Settings.Default.InspectionImagesFolder);
                    var filePath = company.Common.Utilities.WriteFile(insp.drawingFile, InspectionV2Controller.GetInspectionFolderFullPath(inspectionRootRelativeFolder, Properties.Settings.Default.InspectionImagesFolder),
                            file);
                    insp.drawingFile = WebUtilities.CombineUrls(inspectionRootRelativeFolder, Path.GetFileName(filePath));
                }
                var oldInsp = unitOfWork.InspectionV2Repository.GetByID(insp.id);
                oldInsp.drawingFile = insp.drawingFile;
                unitOfWork.Save();
                return oldInsp;
            }
            return null;
        }

        [Route("images")]
        [HttpPost]
        public object UploadImage()
        {
            var request = HttpContext.Current.Request;
            var file = request.Files[0];
            var id = request["id"];
            WebUtilities.SaveTempFile(id, company.Common.Utilities.FileStreamToBytes(file.InputStream));
            return new { success = true };
        }

        [Route("getTempUrl")]
        [HttpGet]
        public HttpResponseMessage getTempUrl(string id, string name)
        {
            var result = new HttpResponseMessage(HttpStatusCode.OK);
            result.Content = new ByteArrayContent(WebUtilities.GetTempFile(id));
            result.Content.Headers.ContentType =
                new MediaTypeHeaderValue("application/octet-stream");
            if(name != null)
                result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment") { FileName = name };
            return result;
        }
    }
}