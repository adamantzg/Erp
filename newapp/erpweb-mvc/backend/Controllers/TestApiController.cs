using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace backend.Controllers
{
    public class TestApiController : ApiController
    {
        [Route("api/test/apiError")]
        [HttpGet]
        public void ApiError()
        {
            throw new ArgumentException("Argument exception");
        }
    }
}