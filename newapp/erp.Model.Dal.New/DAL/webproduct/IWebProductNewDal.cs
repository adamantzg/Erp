using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace erp.Model.Dal.New
{
	public interface IWebProductNewDal : IGenericDal<Web_product_new>
	{
		List<Web_product_new> GetForCategory(int cat_id,bool deep = true,bool category=false,int? language_id= null,bool files = false,bool related = false,bool complementary=false,bool components = true, bool previewTestProducts=false);
		List<Web_product_new> GetForIds(IList<int> ids,int? web_site_id = null,int? language_id = null,bool files = false,bool searchForCategoryProducts = false);
		List<Web_product_new> Search(string text, out int totalCount, int? site_id = null, int? page = null, int? pageSize = null, bool files = false, List<Search_word> words = null, bool useFullText = true, int? catid = null);
		List<Web_product_new> Search(string text, int? site_id = null, bool files = false, List<Search_word> words = null, bool useFullText = true);
		List<Web_product_new> Search(string text, out int totalCount, out double maxprice, out int maxwidth, out int maxheight, out int maxweight, out List<string> tech_types, int? site_id = null, int? page = null, int? pageSize = null, bool files = false, List<Search_word> words = null, bool useFullText = true, int? catid = null, int? minPrice = 0, int? maxPrice = 15000, int minWidth = 0, int maxWidth = 5000, int minHeight = 0, int maxHeight = 5000, string tech_type = null, int minWeight = 0, int maxWeight = 500);
		int GetProductSearchCount(string text, int? site_id = null, int? catid = null, bool useFullText = false);
		List<Web_product_new> GetForWhitebookTemplate(IList<int?> webUniqueIds, IList<int?> templateIds, int? wras = null);

		/// <summary>
		/// WARNING: After regenerating code in web_product_newDAL, getbyid in that file should be deleted to avoid duplication
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		Web_product_new GetByIdEx(int id, int? language_id = null, IDbConnection conn = null, bool loadSubObjects = true);

		Web_product_new GetByLegacyId(int id,int? language_id = null);
		List<Web_product_new> GetRelated(int webUnique, int? relation_type = null, int? language_id = null, bool files = false, IDbConnection conn = null);
		ICollection<Web_product_new> GetRecomendedProduct(int webUnique, int? language_id = null, bool files = false);

		/// <summary>
		/// Siblings are related by same parent. Used in options - one of the products is main (parent) and others are "children"
		/// </summary>
		/// <param name="webUnique"></param>
		/// <param name="conn"></param>
		/// <param name="language_id"></param>
		/// <returns></returns>
		ICollection<Web_product_new> GetSiblings(int webUnique, MySqlConnection conn, int? language_id = null);

		ICollection<Web_product_new> GetChildren(int webUnique, MySqlConnection conn, int? language_id = null);
		bool HasChildren(int webUnique);
		Web_product_new GetParent(int web_unique,bool loadChildren = false, IDbConnection conn = null);
		void CreateEx(Web_product_new p, bool generateId = true);
		void UpdateEx(Web_product_new p);
		void UpdateProductInfo(Web_product_new p);
		void UpdatePartInfo(Web_product_new p);
		void UpdateProductFlow(Web_product_new p);
		List<Web_product_new> GetForFileType(int web_site_id,int webprod_file_type_id, int? language_id = null);
		List<Web_product_new> GetCategoryForWeb(int cat);
		List<Web_product_new> GetForSite(int siteId,bool related=false, bool components = false);
		List<Web_product_new> GetForSites(List<int> ids,bool loadCats = true, string prefixText = null, bool loadSubObjects = true);
		List<Web_product_new> GetWebProductsByCriteria(string searchText, int? site_id = null);
		Web_product_new Copy(int web_unique, bool setParent = true,bool removeCategories = false, int? web_site_id = null);
		List<Web_product_new> DAMSearch(string text, int? site_id = null, string connstring = null);
		List<WebSiteProductCount> GetWebSiteProductCount();
		List<SpareParts> GetSpareParts(int web_unique, IDbConnection conn = null);
		Web_product_new GetByCode(string web_code);
	}
}
