using System.Collections.Generic;
using System.Data;

namespace erp.Model.Dal.New
{
	public interface IWebCategoryDal : IGenericDal<Web_category>
	{
		List<Web_category> GetForBrand(int brand_id, int? language_id = null, bool searchForProducts = false);
		List<Web_category> GetForSite(int site_id, int? language_id = null);
		List<Web_category> GetForIds(IList<int> ids);
		List<Web_category> GetChildren(int parent_id, int? language_id = null, bool searchForProducts = false);
		List<Web_category> GetForProduct(int web_unique, IDbConnection conn = null, bool searchForProducts = false);
		List<Web_category> BuildTreeForSelected(IList<int> catIds,int brand_id);
		List<Web_category> GetAllChildren(int parent_id);
		bool CanCategoryBeDeleted(int id);
		void Rename(int category_id, string name);
		Web_category GetById(int id, IDbConnection conn = null,int? language_id = null);
		List<Web_category> GetParents(int cat_id, IDbConnection conn = null);
		bool IsInBrand(Web_category cat, int brand_id, IDbConnection conn = null);
		List<Web_category> GetProductCategoriesForSearch(string text, int? site_id = null, bool files = false, List<Search_word> words = null, bool useFullText = true,int? catid = null, int minPrice = 0, int maxPrice = 15000, int minWidth = 0, int maxWidth = 5000, int minHeight = 0, int maxHeight = 5000, string tech_type = null, int minWeight = 0, int maxWeight = 500);
		List<Web_category> DAMSearch(string text, int? site_id = null, string connstring = null);
		List<Web_category> GetForSlaveHost(int host_id);
		void DeleteTransferData(int host_id, int cat_id);
	}
}