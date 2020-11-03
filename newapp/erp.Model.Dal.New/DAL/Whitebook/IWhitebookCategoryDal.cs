using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace erp.Model.Dal.New
{
	public interface IWhitebookCategoryDal : IGenericDal<Whitebook_category>
	{
		Whitebook_category GetRootCat(int? categoryId);
		List<Whitebook_category> GetAllChildren(int parent_id);
		List<Whitebook_category> GetChildren(int parent_id);
		List<Whitebook_category> GetForSlaveHost(int host_id);
		void DeleteTransferData(int host_id, int cat_id);
		List<Whitebook_category> GetByCriteria(int? wras = null, bool includeParents = true);
		List<Whitebook_category> GetParents(Whitebook_category category);
	}

	
}
