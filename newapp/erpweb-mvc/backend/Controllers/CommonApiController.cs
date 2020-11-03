using company.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace backend.Controllers
{
	public class CommonApiController : ApiController
	{
		[EnableCors(origins: "*", headers: "*", methods: "*")]
		[Route("api/uploadimage")]
		[HttpPost]
		public object UploadImage()
		{
			var request = HttpContext.Current.Request;
			var file = request.Files[0];
			var id = request["id"];
			WebUtilities.SaveTempFile(id, Utilities.FileStreamToBytes(file.InputStream));
			//[{"fieldname":"file","originalname":"7.jpg","encoding":"7bit","mimetype":"image/jpeg","destination":"/app/dist/api/uploads","filename":"lp4dmp.jpg","path":"/app/dist/api/uploads/lp4dmp.jpg","size":120220}]
			return new[] {
				new { 
					fieldname =  "file",
					originalname = file.FileName,
					encoding = "7bit",
					mimetype = "image/jpeg",
					destination = "/api/uploadimage",
					filename = file.FileName,
					path = "",
					size = file.InputStream.Length
				}
			};
		}

		[Route("api/getTempUrl")]
		[HttpGet]
		public HttpResponseMessage getTempUrl(string id)
		{
			var result = new HttpResponseMessage(HttpStatusCode.OK);
			var file = WebUtilities.GetTempFile(id);
			if(file != null)
			{
				result.Content = new ByteArrayContent(WebUtilities.GetTempFile(id));
				result.Content.Headers.ContentType =
					new MediaTypeHeaderValue("application/octet-stream");
				return result;
			}
			return new HttpResponseMessage(HttpStatusCode.NoContent);
		}
	}
}