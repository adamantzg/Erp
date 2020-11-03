using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using erp.Model;

namespace erp.DAL.EF.Repositories
{
    public class SalesOrdersRepository : GenericRepository<Sales_orders>
    {
        public SalesOrdersRepository(Model context) : base(context)
        {
            

        }

        public virtual void Delete(DateTime? from)
        {
            context.Database.ExecuteSqlCommand("DELETE FROM sales_orders WHERE date_report >= @p0", from);
        }

        public List<SalesOrderSummary> GetBestWeekData(IList<string> excludedWarehouses = null, IList<string> excludedDealers = null, List<string> excludedProductsList = null,
			List<string> excludedProductsNewList = null, DateTime? newRulesDate = null)
        {
            var list = context.Database.SqlQuery<SalesOrderSummary>(
                        $@"SELECT brand, YEARWEEK(date_report) AS Period, SUM(value) AS Value,COUNT(DISTINCT order_no) AS nooforders FROM `sales_orders`
                        WHERE value <> 0 {GetWhereClause(excludedWarehouses, excludedDealers, excludedProductsList,excludedProductsNewList: excludedProductsNewList, newRulesDate: newRulesDate)}
                        GROUP BY brand, YEARWEEK(date_report)
                        ORDER BY Value DESC",newRulesDate).ToList();
            var brands = list.GroupBy(l => l.Brand).Select(g => new { brand = g.Key, max = g.Max(l => l.Value) }).ToList();
            var result = list.Where(l => brands.Count(b => b.brand == l.Brand && l.Value == b.max) > 0).ToList();
            //Add total
            var where = " WHERE COALESCE(warehouse,'') <> '' AND value <> 0 " +
                        GetWhereClause(excludedWarehouses, excludedDealers, excludedProductsList, excludedProductsNewList: excludedProductsNewList, newRulesDate: newRulesDate);

            result.AddRange(context.Database.SqlQuery<SalesOrderSummary>(
                $@"SELECT YEARWEEK(date_report) AS Period, SUM(value) AS Value,COUNT(DISTINCT order_no) AS nooforders FROM `sales_orders`
                        {where}      
                        GROUP BY YEARWEEK(date_report)
                        ORDER BY Value DESC LIMIT 1",newRulesDate
                ).ToList());
            return result;
        }

        public List<SalesOrderSummary> GetBestMonthData(IList<string> excludedWarehouses = null, IList<string> excludedDealers = null, List<string> excludedProductsList = null,
			List<string> excludedProductsNewList = null, DateTime? newRulesDate = null)
        {
            var list = context.Database.SqlQuery<SalesOrderSummary>(
                        $@"SELECT brand, fn_Month21(date_report) AS Period, SUM(value) AS Value,COUNT(DISTINCT order_no) AS nooforders FROM `sales_orders`
                        WHERE value <> 0 {GetWhereClause(excludedWarehouses, excludedDealers, excludedProductsList,excludedProductsNewList: excludedProductsNewList, newRulesDate: newRulesDate)}
                        GROUP BY brand, fn_Month21(date_report)
                        ORDER BY Value DESC", newRulesDate).ToList();
            var brands = list.GroupBy(l => l.Brand).Select(g => new { brand = g.Key, max = g.Max(l => l.Value) }).ToList();
            var result = list.Where(l => brands.Count(b => b.brand == l.Brand && l.Value == b.max) > 0).ToList();
            var where = " WHERE COALESCE(warehouse,'') <> '' AND value <> 0  " +
                        GetWhereClause(excludedWarehouses, excludedDealers, excludedProductsList, excludedProductsNewList: excludedProductsNewList, newRulesDate: newRulesDate);

            result.AddRange(context.Database.SqlQuery<SalesOrderSummary>(
                $@"SELECT fn_Month21(date_report) AS Period, SUM(value) AS Value,COUNT(DISTINCT order_no) AS nooforders FROM `sales_orders`
                        {where}
                        GROUP BY fn_Month21(date_report)                        
                        ORDER BY Value DESC LIMIT 1", newRulesDate
                ).ToList());
            return result;
        }

        private string GetWhereClause(IList<string> excludedWarehouses = null, IList<string> excludedDealers = null,
            List<string> excludedProductsList = null, string joinWith = " AND ", string startWith = " AND ",
			List<string> excludedProductsNewList = null, DateTime? newRulesDate = null)
        {
            var result = new List<string>();
			result.Add(" date_report IS NOT NULL ");
            if(excludedWarehouses != null && excludedWarehouses.Count > 0)
                result.Add($" warehouse NOT IN ({string.Join(",", excludedWarehouses.Select(w => $"'{w}'"))})");
            if (excludedDealers != null && excludedDealers.Count > 0)
                result.Add($" customer NOT IN ({string.Join(", ", excludedDealers.Select(d => $"'{d}'"))})");
			result.Add("((@p0 IS NOT NULL AND date_report >= @p0) OR COALESCE(delivery_reason,'') NOT LIKE '%DISPLAY%')");
			var excludedProductsClause = new List<string>();
            if(excludedProductsList != null && excludedProductsList.Count > 0)
                excludedProductsClause.Add($"( (@p0 IS NULL OR date_report < @p0) AND cprod_code1 NOT IN ({string.Join(",", excludedProductsList.Select(d => $"'{d}'"))}))");
			if (excludedProductsNewList != null && excludedProductsNewList.Count > 0)
				excludedProductsClause.Add($"( (@p0 IS NOT NULL AND date_report >= @p0) AND cprod_code1 NOT IN ({string.Join(",", excludedProductsNewList.Select(d => $"'{d}'"))}))");
			if (excludedProductsClause.Count > 0)
				result.Add($"({string.Join(" OR ", excludedProductsClause)})");
			if (result.Count > 0)
                return startWith + String.Join(joinWith, result);
            return string.Empty;
        }

        public List<SalesOrderProductMonthSummary> GetProductMonthSummaries(DateTime? from, DateTime? to)
        {
            return context.Database.SqlQuery<SalesOrderProductMonthSummary>(
						@"SELECT cprod_id, fn_Month21(sales_orders.date_report) AS month21, SUM(`sales_orders`.`order_qty`) AS qty
                         FROM `sales_orders` INNER JOIN us_dealers ON sales_orders.customer = us_dealers.customer
                          WHERE sales_orders.date_report IS NOT NULL AND 
						  ((`sales_orders`.`date_report` >= @p0) AND (`sales_orders`.`date_report` < @p1)) AND (`sales_orders`.`cprod_id` IS NOT NULL) 
                          AND COALESCE(us_dealers.internal,0) <> 1
                          GROUP BY `sales_orders`.`cprod_id`, fn_Month21(sales_orders.date_report)", from,to).ToList();
        }

        public List<SalesOrderBrandMonthSummary> GetBrandMonthSummaries(DateTime? from, DateTime? to, 
            IList<string> excludedWarehouses = null, IList<string> excludedDealers = null, List<string> excludedProductsList = null,
			List<string> excludedProductsNewList = null, DateTime? newRulesDate = null)
        {
            return context.Database.SqlQuery<SalesOrderBrandMonthSummary>(
                        $@"SELECT brand, fn_Month21(sales_orders.date_report) AS month21, SUM(`sales_orders`.`order_qty`) AS qty, SUM(sales_orders.value) AS value
                          FROM `sales_orders` 
                          WHERE sales_orders.date_report IS NOT NULL AND ((`sales_orders`.`date_report` >= @p0) AND (`sales_orders`.`date_report` <= @p1)) 
                          {GetWhereClause(excludedWarehouses,excludedDealers,excludedProductsList,excludedProductsNewList: excludedProductsNewList, newRulesDate: newRulesDate)}
                          GROUP BY `sales_orders`.`brand`, fn_Month21(sales_orders.date_report)", from, to).ToList();
        }

        public void DeleteForIds(IList<int> rowIds )
        {
            context.Database.ExecuteSqlCommand($"DELETE FROM sales_orders WHERE rowid IN ({string.Join(",", rowIds)})");
        }
    }

    public class SalesOrderSummary
    {
		public string Brand { get; set; }
        public string Warehouse { get; set; }
        public int Period { get; set; }
        public DateTime Day { get; set; }
        public int? Qty { get; set; }
        public int? NoOfOrders { get; set; }
        public double? Value { get; set; }
    }

    public class SalesOrderBrandMonthSummary
    {
        public int month21 { get; set; }
        public int? Qty { get; set; }
        public string Brand { get; set; }
        public double? Value { get; set; }
    }

    public class SalesOrderProductMonthSummary
    {
        public int? cprod_id { get; set; }
        public int month21 { get; set; }
        public int? Qty { get; set; }
        public string Warehouse { get; set; }
        public double? Value { get; set; }
    }
}
