using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using erp.DAL.EF.Repositories;
using erp.Model;

namespace erp.DAL.EF
{
    public class WhitebookCategoryRepository : GenericRepository<Whitebook_category>
    {
        public WhitebookCategoryRepository(Model context) : base(context)
        {
        }


        public Whitebook_category GetRootCat(int? categoryId)
        {
            return
                context.Database.SqlQuery<Whitebook_category>(
                        "SELECT * FROM Whitebook_category WHERE id = fn_GetWhitebookCatRootId(@p0)", categoryId)
                    .FirstOrDefault();
        }

		public List<Whitebook_category> GetAllChildren(int parent_id)
		{
			var result = new List<Whitebook_category>();
			var children = GetChildrenInternal(parent_id, true);
			if (children.Count > 0)
				result.AddRange(children);
			return result;
		}

		public List<Whitebook_category> GetChildren(int parent_id)
		{
			return GetChildrenInternal(parent_id);
		}

		private List<Whitebook_category> GetChildrenInternal(int parent_id, bool deep = false)
		{
			var result = new List<Whitebook_category>();
			var sql = @"SELECT Whitebook_category.* FROM Whitebook_category WHERE parent_id = @p0";
			result = context.Database.SqlQuery<Whitebook_category>(sql,parent_id).ToList();
			
			if (deep)
			{
				var allChildren = new List<Whitebook_category>();
				foreach (var c in result)
				{
					var children = GetChildrenInternal(c.id, true);
					if (children.Count > 0)
						allChildren.AddRange(children);
				}
				result.AddRange(allChildren);
			}

			return result;
		}

		public List<Whitebook_category> GetForSlaveHost(int host_id)
		{
			List<Whitebook_category> result = 
			context.Database.SqlQuery<Whitebook_category>(
					@"SELECT whitebook_category.* FROM whitebook_category INNER JOIN whitebook_category_file_transfer ON whitebook_category.id = whitebook_category_file_transfer.category_id
					  WHERE whitebook_category_file_transfer.host_id = @p0", host_id).ToList();
			
			return result;
		}

		public void DeleteTransferData(int host_id, int cat_id)
		{
			context.Database.ExecuteSqlCommand("DELETE FROM whitebook_category_file_transfer WHERE host_id = @p0 AND category_id = @p1", host_id, cat_id);
		}
	}
}
