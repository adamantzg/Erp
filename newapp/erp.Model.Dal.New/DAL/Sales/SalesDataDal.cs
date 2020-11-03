using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace erp.Model.Dal.New
{
	public class SalesDataDal : GenericDal<Sales_data>, ISalesDataDal
	{
		public SalesDataDal(IDbConnection conn) : base(conn)
		{
		}

		public void DeleteByIdAndMonth(int cprod_id, int? monthFrom = null, int? monthTo = null)
		{
			throw new NotImplementedException();
		}

		public Sales_data GetByProdAndMonth(int cprod_id, int month)
		{
			return conn.QueryFirstOrDefault<Sales_data>("SELECT * FROM sales_data WHERE cprod_id = @cprod_id AND month21 = @month",
				new {cprod_id, month});
		}

		public List<Sales_data> GetForCompanyAndPeriod(IList<int> company_ids, int monthFrom, int monthTo)
		{
			return conn.Query<Sales_data>(@"SELECT sales_data.* FROM sales_data INNER JOIN cust_products ON sales_data.cprod_id = cust_products.cprod_id 
                                    WHERE cust_products.cprod_user IN @company_ids AND month21 BETWEEN @monthFrom AND @monthTo",
									new { company_ids, monthFrom, monthTo }).ToList();
		}

		public List<Sales_data> GetForMastProdAndPeriod(int mast_id, int monthFrom, int monthTo)
		{
			return GetForMastProdAndPeriod(new List<int>{mast_id},monthFrom,monthTo);
		}

		public List<Sales_data> GetForMastProdAndPeriod(IList<int> mast_ids, int monthFrom, int monthTo)
		{
			return conn.Query<Cust_products, Sales_data, Sales_data>(
				@"SELECT cust_products.*,sales_data.* FROM sales_data INNER JOIN cust_products ON sales_data.cprod_id = cust_products.cprod_id 
                WHERE cust_products.cprod_mast IN @mast_ids AND month21 BETWEEN @monthFrom AND @monthTo",
				(cp, sd) =>
				{
					sd.Product = cp;
					return sd;
				}, new {mast_ids, monthFrom, monthTo}, splitOn: "sales_unique").ToList();
		}

		public List<Sales_data> GetForPeriod(int cprod_id, int monthFrom, int monthTo)
		{
			return GetForPeriod(new[] {cprod_id}, monthFrom, monthTo);
		}

		public List<Sales_data> GetForPeriod(IList<int> cprod_ids, int monthFrom, int monthTo)
		{
			return conn.Query<Sales_data>("SELECT * FROM sales_data WHERE cprod_id IN @cprod_ids AND month21 BETWEEN @monthFrom AND @monthTo",
					new {cprod_ids, monthFrom, monthTo}).ToList();
		}

		public List<Cust_products> GetForProdUserAndPeriod(int monthFrom, int monthTo, IList<int> cprod_user = null)
		{
			return conn.Query<Cust_products, Sales_data, Cust_products>(
				$@"SELECT cust_products.*, sales_data.* FROM cust_products
                INNER JOIN sales_data ON cust_products.cprod_id = sales_data.cprod_id
                WHERE month21 BETWEEN @monthFrom AND @monthTo {(cprod_user != null ? " AND cprod_user IN @cprod_user" : "")} ORDER BY cust_products.cprod_id", 
				(cp, sd) =>
				{
					if(cp.SalesProducts == null)
						cp.SalesProducts = new List<Sales_data>();
					cp.SalesProducts.Add(sd);
					return cp;
				},new {monthFrom, monthTo, cprod_user }, splitOn: "sales_unique" ).ToList();
		}

		protected override string GetAllSql()
		{
			return "SELECT * FROM sales_data";
		}

		protected override string GetByIdSql()
		{
			return "SELECT * FROM sales_data WHERE sales_unique = @id";
		}

		protected override string GetCreateSql()
		{
			return @"INSERT INTO sales_data (cprod_id,sales_qty,month21) VALUES(@cprod_id,@sales_qty,@month21)";
		}

		protected override string GetDeleteSql()
		{
			return "DELETE FROM sales_data WHERE sales_unique = @id";
		}

		protected override string GetUpdateSql()
		{
			return @"UPDATE sales_data SET cprod_id = @cprod_id,sales_qty = @sales_qty,month21 = @month21 WHERE sales_unique = @sales_unique";
		}

		protected override bool IsAutoKey => true;
		protected override string IdField => "sales_unique";

	}
}
