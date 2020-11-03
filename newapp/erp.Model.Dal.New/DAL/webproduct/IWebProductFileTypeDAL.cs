using System.Collections.Generic;
using System.Data;

namespace erp.Model.Dal.New
{
	public interface IWebProductFileTypeDAL : IGenericDal<Web_product_file_type>
	{
		List<Web_product_file_type> GetForSite(int site_id, bool overrideUniversalTypes = true);
		List<Web_product_file_type> GetForSite(int site_id, List<Web_product_file_type> all, List<File_type> fileTypes, bool overrideUniversalTypes = true );
	}
}