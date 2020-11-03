using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace backend.ApiServices
{
	public interface IUploadedFileHandler
	{
		byte[] GetTempFile(string id);
		string WriteFile(string name, string folder, byte[] contents, bool overWrite = false);
	}
}
