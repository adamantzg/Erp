
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using erp.Model;


namespace erp.Model.Dal.New
{
    public partial class ProductInvestigationStatusDAL : GenericDal<Product_investigation_status>, IProductInvestigationStatusDAL
	{
		public ProductInvestigationStatusDAL(IDbConnection conn) : base(conn)
		{
		}
				
		protected override bool IsAutoKey => false;

		protected override string GetAllSql()
		{
			return "SELECT * FROM product_investigation_status";
		}

		protected override string GetByIdSql()
		{
			return "SELECT * FROM product_investigation_status WHERE id = @id";
		}

		protected override string GetCreateSql()
		{
			return @"INSERT INTO product_investigation_status (id,name) VALUES(@id,@name)";
		}

		protected override string GetUpdateSql()
		{
			return @"UPDATE product_investigation_status SET name = @name WHERE id = @id";
		}

		protected override string GetDeleteSql()
		{
			return "DELETE FROM product_investigation_status WHERE id = @id";
		}
	}
}
			
			