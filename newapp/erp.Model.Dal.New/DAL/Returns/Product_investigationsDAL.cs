
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using erp.Model;
using Dapper;

namespace erp.Model.Dal.New
{
	public partial class ProductInvestigationsDAL : GenericDal<Product_investigations>, IProductInvestigationsDAL
	{
		public ProductInvestigationsDAL(IDbConnection conn) : base(conn)
		{
		}

		public override List<Product_investigations> GetAll()
		{
			return conn.Query<Cust_products, Product_investigations, Product_investigations>(
				@"SELECT cust_products.*,Product_investigations.* FROM product_investigations INNER JOIN cust_products ON
					product_investigations.cprod_id = cust_products.cprod_id",
				(cp, pi) =>
				{
					pi.Product = cp;
					return pi;
				}, splitOn: "id").ToList();
		}			
		
		

		public List<Product_investigations> GetClaimInvestigationForProduct(int cprodId)
		{
			return conn.Query<Cust_products, Product_investigations, Product_investigations>(
				@"SELECT cust_products.*,Product_investigations.* FROM product_investigations INNER JOIN cust_products ON
					product_investigations.cprod_id = cust_products.cprod_id WHERE cust_products.cprod_id = @cprodId",
				(cp, pi) =>
				{
					pi.Product = cp;
					return pi;
				}, new { cprodId }, splitOn: "id").ToList();

		}
				
		protected override string GetAllSql()
		{
			return "SELECT * FROM product_investigations";
		}

		protected override string GetByIdSql()
		{
			return "SELECT * FROM product_investigations WHERE id = @id";
		}

		protected override string GetCreateSql()
		{
			return @"INSERT INTO product_investigations (cprod_id,mast_id,date,monitored_by,status,comments) 
					VALUES(@cprod_id,@mast_id,@date,@monitored_by,@status,@comments)";
		}

		protected override string GetUpdateSql()
		{
			return @"UPDATE product_investigations SET cprod_id = @cprod_id,mast_id = @mast_id,date = @date,monitored_by = @monitored_by,status = @status,
					comments = @comments WHERE id = @id";
		}

		protected override string GetDeleteSql()
		{
			return "DELETE FROM product_investigations WHERE id = @id";
		}
	}
}
			
			