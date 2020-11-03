
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;
using Dapper;

namespace erp.Model.Dal.New
{
    public class Category1Dal : GenericDal<Category1>, ICategory1DAL
	{
		protected override string GetAllSql()
		{
			return "SELECT * FROM category1";
		}

		protected override string GetByIdSql()
		{
			return "SELECT * FROM category1 WHERE category1_id = @id";
		}

		protected override string GetCreateSql()
		{
			return @"INSERT INTO category1 (cat1_name,cat1_duty,cat1_margin) VALUES(@cat1_name,@cat1_duty,@cat1_margin)";
		}

		protected override string GetUpdateSql()
		{
			return
				@"UPDATE category1 SET cat1_name = @cat1_name,cat1_duty = @cat1_duty,cat1_margin = @cat1_margin WHERE category1_id = @category1_id";
		}

		protected override string GetDeleteSql()
		{
			return "DELETE FROM category1 WHERE category1_id = @id";
		}

		public List<Category1> GetByBrand(int brand_id)
		{
			return conn.Query<Category1>(
				@"SELECT * FROM category1 WHERE category1_id IN (SELECT category1 FROM mast_products INNER JOIN 
				cust_products ON mast_products.mast_id = cust_products.cprod_id WHERE cust_products.brand_id = @brand_id 
				AND cust_products.cprod_status <> 'D' )", new { brand_id }).ToList();
		}

		protected override string IdField => "category1_id";


		public Category1Dal(IDbConnection conn) : base(conn)
		{
		}
	}
}
			
			