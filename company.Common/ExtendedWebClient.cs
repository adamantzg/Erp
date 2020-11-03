using System;
using System.Net;

namespace company.Common
{
    public class ExtendedWebClient : WebClient
    {
        private Int32 _timeout;
		public CookieContainer CookieContainer { get; private set; }

		public ExtendedWebClient(Int32 timeoutSec = 500)
        {
            this._timeout = timeoutSec*1000;
			
			this.CookieContainer = new CookieContainer();
		}

        protected override WebRequest GetWebRequest(Uri address)
        {
			var request = base.GetWebRequest(address) as HttpWebRequest;
			if (request == null) return base.GetWebRequest(address);
			request.CookieContainer = CookieContainer;
			request.SendChunked = false;			
			request.Timeout = this._timeout;
			return request;			
        }
    }
}
