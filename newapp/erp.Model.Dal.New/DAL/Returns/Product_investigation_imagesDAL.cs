using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using erp.Model;
using Dapper;
using System.Data;

namespace erp.Model.Dal.New
{
    public class ProductInvestigationImagesDAL : GenericDal<Product_investigation_images>, IProductInvestigationImagesDAL
	{
		public ProductInvestigationImagesDAL(IDbConnection conn) : base(conn)
		{
		}

		
        public  List<Product_investigation_images> GetForProduct(int cprodId)
        {
            return conn.Query<Product_investigation_images>("SELECT * FROM product_investigation_images WHERE cprod_id= @cprodId", new { cprodId}).ToList();

        }

        public List<Product_investigation_images> GetForInvestigation(int id)
        {
            return conn.Query<Product_investigation_images>(@"SELECT * FROM product_investigation_images WHERE investigation_id=@id", new { id}).ToList();
        }      


        

		protected override string GetAllSql()
		{
			return "SELECT * FROM product_investigation_images";
		}

		protected override string GetByIdSql()
		{
			throw new NotImplementedException();
		}

		protected override string GetCreateSql()
		{
			return @"INSERT INTO product_investigation_images(name,cprod_id,investigation_id)
                                VALUES(@name,@cprod_id,@investigation_id)";
		}

		protected override string GetUpdateSql()
		{
			throw new NotImplementedException();
		}

		protected override string GetDeleteSql()
		{
			return "DELETE FROM product_investigation_images WHERE id = @id";
		}
	}
}
