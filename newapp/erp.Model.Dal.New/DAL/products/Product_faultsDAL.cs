
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Dapper;

namespace erp.Model.Dal.New
{
    public class ProductFaultsDAL : GenericDal<Product_faults>, IProductFaultsDAL
	{
		public ProductFaultsDAL(IDbConnection conn) : base(conn)
		{
		}

		
        public List<Product_faults> GetProductFaults(int cprod_id, DateTime? from, DateTime? to)
        {
			return conn.Query<Product_faults>(
				@"SELECT * FROM product_faults WHERE fault_cprod = @cprod_id AND (fault_date >= @from OR @from IS NULL) 
					AND (fault_date <= @to OR @to IS NULL)",
					new {cprod_id, from, to}).ToList();            
        }

        public List<Product_faults> GetProductFaultsForCompanies(IList<int> company_ids, DateTime? from, DateTime? to)
        {
			return conn.Query<Product_faults, Cust_products, Mast_products, Company, Product_faults>(
				@"SELECT product_faults.*, cust_products.*,mast_products.*,factory.* FROM product_faults INNER JOIN cust_products ON product_faults.fault_cprod = cust_products.cprod_id 
                    INNER JOIN mast_products ON cust_products.cprod_mast = mast_products.mast_id INNER JOIN users factory ON mast_products.factory_id = factory.user_id
                    WHERE cust_products.cprod_user IN @company_ids AND (fault_date >= @from OR @from IS NULL) AND (fault_date <= @to OR @to IS NULL)",
				(pf, cp, mp, f) =>
				{
					pf.Product = cp;
					pf.Product.MastProduct = mp;
					pf.Product.MastProduct.Factory = f;
					return pf;
				},	new {from, to, company_ids}, splitOn: "cprod_id,mast_id, user_id").ToList();
            
        }

		protected override string IdField => "fault_id";

		protected override string GetAllSql()
		{
			return "SELECT * FROM product_faults";
		}

		protected override string GetByIdSql()
		{
			return "SELECT * FROM product_faults WHERE fault_id = @id";
		}

		protected override string GetCreateSql()
		{
			return @"INSERT INTO product_faults (fault_cprod,fault_category,fault_reason,fault_qty,fault_order_number,fault_date,fault_cost,fault_comments,
					fault_po,fault_original,fault_TMS,fault_store,fault_summary) VALUES(@fault_cprod,@fault_category,@fault_reason,@fault_qty,
					@fault_order_number,@fault_date,@fault_cost,@fault_comments,@fault_po,@fault_original,@fault_TMS,@fault_store,@fault_summary)";
		}

		protected override string GetUpdateSql()
		{
			return @"UPDATE product_faults SET fault_cprod = @fault_cprod,fault_category = @fault_category,fault_reason = @fault_reason,fault_qty = @fault_qty,
					fault_order_number = @fault_order_number,fault_date = @fault_date,fault_cost = @fault_cost,fault_comments = @fault_comments,
					fault_po = @fault_po,fault_original = @fault_original,fault_TMS = @fault_TMS,fault_store = @fault_store,fault_summary = @fault_summary 
					WHERE fault_id = @fault_id";
		}

		protected override string GetDeleteSql()
		{
			return "DELETE FROM product_faults WHERE fault_id = @id";
		}
	}
}
			
			