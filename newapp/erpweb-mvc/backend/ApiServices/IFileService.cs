using System.Collections.Generic;
using erp.Model;

namespace backend.ApiServices
{
	public interface IFileService
	{
		string GetFileUrl(IList<File_type> fileTypes, File f);
		void HandleFiles(IList<File> files);
	}
}