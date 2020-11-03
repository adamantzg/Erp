using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace backend.ApiServices
{
	public class UploadedFileHandler : IUploadedFileHandler
	{
		public byte[] GetTempFile(string id)
		{
			return WebUtilities.GetTempFile(id);
		}

		public string WriteFile(string name, string folder, byte[] contents, bool overWrite = false)
		{
			return company.Common.Utilities.WriteFile(name, folder, contents, overWrite);
		}
	}
}