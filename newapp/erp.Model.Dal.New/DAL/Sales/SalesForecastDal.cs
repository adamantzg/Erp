using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace erp.Model.Dal.New
{
	public class SalesForecastDal : GenericDal<Sales_forecast>, ISalesForecastDal
	{
		public SalesForecastDal(IDbConnection conn) : base(conn)
		{
		}

		public void DeleteByIdAndMonth(int cprod_id, int? monthFrom = null, int? monthTo = null)
		{
			conn.Execute(@"DELETE FROM sales_forecast WHERE cprod_id = @cprod_id 
					AND (month21>= @monthFrom OR @monthFrom IS NULL) AND (month21<= @monthTo OR @monthTo IS NULL)",
					new {cprod_id, monthFrom, monthTo });
		}

		public List<Sales_forecast> GetForecastForPeriod(int monthFrom, int monthTo, List<int> numCprodUser)
		{
			return conn.Query<Sales_forecast>($@"SELECT sales_forecast.* FROM sales_forecast 
                                               INNER JOIN cust_products ON sales_forecast.cprod_id=cust_products.cprod_id
                                               WHERE month21 BETWEEN @monthFrom AND @monthTo
											{(numCprodUser != null ? " AND cprod_user IN @numCprodUser " : "")}", 
											new { monthFrom, monthTo, numCprodUser}).ToList();
		}

		public List<Sales_forecast> GetForMastProdAndPeriod(int id, int monthFrom, int monthTo)
		{
			return GetForMastProdAndPeriod(new List<int>{id},monthFrom,monthTo );
		}

		public List<Sales_forecast> GetForMastProdAndPeriod(IList<int> ids, int monthFrom, int monthTo)
		{
			return conn.Query<Sales_forecast>(@"SELECT * FROM sales_forecast_all 
						INNER JOIN cust_products ON sales_forecast_all.cprod_id = cust_products.cprod_id 
                        WHERE cust_products.cprod_mast IN @ids AND month21 BETWEEN @monthFrom AND @monthTo", 
						new {ids, monthFrom, monthTo }).ToList();
		}

		public List<Sales_forecast> GetForPeriod(int cprod_id, int monthFrom, int monthTo)
		{
			return GetForPeriod(new[] {cprod_id}, monthFrom, monthTo);
		}

		public List<Sales_forecast> GetForPeriod(IList<int> cprod_ids, int monthFrom, int monthTo)
		{
			return conn.Query<Sales_forecast>(@"SELECT * FROM sales_forecast_all 
					WHERE cprod_id IN @cprod_ids AND month21 BETWEEN @monthFrom AND @monthTo", new { cprod_ids, monthFrom, monthTo }).ToList();
		}

		protected override string GetAllSql()
		{
			return "SELECT * FROM sales_forecast";
		}

		protected override string GetByIdSql()
		{
			return "SELECT * FROM sales_forecast WHERE sales_unique = @id";
		}

		protected override string GetCreateSql()
		{
			return @"INSERT INTO sales_forecast (cprod_id,sales_qty,month21) VALUES(@cprod_id,@sales_qty,@month21)";
		}

		protected override string GetDeleteSql()
		{
			return "DELETE FROM sales_forecast WHERE sales_unique = @id";
		}

		
		protected override string GetUpdateSql()
		{
			return @"UPDATE sales_forecast SET cprod_id = @cprod_id,sales_qty = @sales_qty,month21 = @month21 WHERE sales_unique = @sales_unique";
		}

		protected override bool IsAutoKey => true;
		protected override string IdField => "sales_unique";
	}
}
