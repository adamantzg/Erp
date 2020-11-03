using System.Collections.Generic;
using System.Data;

namespace erp.Model.Dal.New
{
	public interface IWebProductFileDal
	{
		List<Web_product_file> GetAll();
		List<Web_product_file> GetForProduct(int web_unique, IDbConnection conn = null);
		void CreateEx(Web_product_file o, IDbTransaction tr = null);
		void UpdateEx(Web_product_file o, IDbTransaction tr = null);
		void Delete(int id, IDbTransaction tr = null);

		/// <summary>
		/// Deletes images for web product that are not in the list
		/// </summary>
		/// <param name="web_unique"></param>
		/// <param name="ids"></param>
		/// <param name="tr"></param>
		void DeleteMissing(int web_unique, IList<int> ids, IDbTransaction tr = null);

		List<Web_product_file> GetForTypes(int[] ids);
		List<Web_product_file> GetForSearch(int id,int fileType, int? date_period = null);
		List<Web_product_file> GetForWebsites(int id, int? date_period=null);
		List<Web_product_file> GetForWebsitesList(List<int> ids, int? date_period = null);
		List<Web_product_file> GetForWebsitesWithWidthAndHeightEmpty(int id);
		List<Web_product_file> GetFiles(int catid, string searchQuery = "");
		List<Web_product_file> GetDAMFiles(int catid, string searchQuery = "");
		List<Web_product_file> GetFilesForDAMSearch(string term, int site_id);
		List<Web_product_file> GetListByWebUniqueAndFileType(int web_unique, int file_type);
		int GetCountByFilename(string name);
		List<Web_product_file> GetForSlaveHost(int host_id);
		void UpdateFileForSlaveHost(int host_id, int file_id);
	}
}