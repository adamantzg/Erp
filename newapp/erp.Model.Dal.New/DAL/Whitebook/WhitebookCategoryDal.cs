using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace erp.Model.Dal.New
{
	public class WhitebookCategoryDal : GenericDal<Whitebook_category>, IWhitebookCategoryDal
	{
		public WhitebookCategoryDal(IDbConnection conn) : base(conn)
		{
		}

		protected override string GetAllSql()
		{
			throw new NotImplementedException();
		}

		protected override string GetByIdSql()
		{
			throw new NotImplementedException();
		}

		protected override string GetCreateSql()
		{
			throw new NotImplementedException();
		}

		protected override string GetUpdateSql()
		{
			throw new NotImplementedException();
		}

		protected override string GetDeleteSql()
		{
			throw new NotImplementedException();
		}

		public Whitebook_category GetRootCat(int? categoryId)
		{
			return
				conn.QueryFirstOrDefault<Whitebook_category>(
					"SELECT * FROM Whitebook_category WHERE id = fn_GetWhitebookCatRootId(@categoryId)", new {categoryId});
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

		public List<Whitebook_category> GetForSlaveHost(int host_id)
		{
			return conn.Query<Whitebook_category>(
				@"SELECT whitebook_category.* FROM whitebook_category INNER JOIN whitebook_category_file_transfer ON whitebook_category.id = whitebook_category_file_transfer.category_id
					  WHERE whitebook_category_file_transfer.host_id = @host_id",  new {host_id}).ToList();
		}

		public void DeleteTransferData(int host_id, int cat_id)
		{
			conn.Execute("DELETE FROM whitebook_category_file_transfer WHERE host_id = @host_id AND category_id = @cat_id", new { host_id, cat_id});
		}

		private List<Whitebook_category> GetChildrenInternal(int parent_id, bool deep = false)
		{
			var result = new List<Whitebook_category>();
			var sql = @"SELECT Whitebook_category.* FROM Whitebook_category WHERE parent_id = @parent_id";
			result = conn.Query<Whitebook_category>(sql, new {parent_id}).ToList();
			
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

		public List<Whitebook_category> GetByCriteria(int? wras = null, bool includeParents = true)
		{
			var result = conn.Query<Whitebook_category>(
				@"SELECT * FROM whitebook_category WHERE id IN (SELECT category_id FROM whitebook_template_category 
				INNER JOIN whitebook_template ON whitebook_template_category.template_id = whitebook_template.id 
				INNER JOIN web_product_new 
				ON (whitebook_template.id = web_product_new.whitebook_template_id 
						OR whitebook_template.default_web_unique = web_product_new.web_unique)
				INNER JOIN web_product_component ON web_product_new.web_unique = web_product_component.web_unique 
				INNER JOIN cust_products_extradata ON web_product_component.cprod_id = cust_products_extradata.cprod_id 
				WHERE (@wras IS NULL OR cust_products_extradata.wras_approval BETWEEN 1 and 3))", new { wras }
				).ToList();

			if(includeParents)
			{
				var parents = new List<Whitebook_category>();
				foreach(var c in result)
				{
					parents.AddRange(GetParents(c).Where(p=>parents.Count(x=>x.id == p.id)==0));
				}
				result.AddRange(parents);
			}

			return result;
		}

		public List<Whitebook_category> GetParents(Whitebook_category category)
		{
			var result = new List<Whitebook_category>();
			var cat = category;
			while(cat.parent_id != null && cat.parent_id != cat.id)
			{
				cat = conn.QueryFirstOrDefault<Whitebook_category>("SELECT * FROM whitebook_category WHERE id = @id", new { id = cat.parent_id });
				result.Add(cat);
			}
			return result;
		}
	}
}
