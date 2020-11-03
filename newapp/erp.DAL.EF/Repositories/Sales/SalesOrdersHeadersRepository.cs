using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using erp.Model;
using RefactorThis.GraphDiff;

namespace erp.DAL.EF.Repositories
{
    public class SalesOrdersHeadersRepository : GenericRepository<Sales_orders_headers>
    {
        public SalesOrdersHeadersRepository(Model context) : base(context)
        {
        }

        public override void Update(Sales_orders_headers entityToUpdate)
        {
            context.UpdateGraph(entityToUpdate, map => map.OwnedCollection(h => h.Shippings));
        }

        public List<sales_orders_headers_summary> GetSummary(string brand = null, bool? outstanding = null,
            int skip = 0, int take = 0)
        {
            var conditions = new List<string>();
            if (brand != null)
                conditions.Add(" COUNT(IF(brand=@p0 AND cprod_name NOT LIKE 'FREIGHT%',1,NULL)) > 0 ");
            if (outstanding != null)
                conditions.Add($" despatched_qty {(outstanding == true ? "<" : "=")} order_qty ");

            var sql = $@"SELECT
                        sales_orders.order_no,
                        sales_orders.date_entered,
                        sales_orders.customer,
                        sales_orders.customer_order_no,
                        sales_orders.pick_list,
                        SUM({GetBrandConditionSum(brand, "order_qty")}) AS order_qty,
                        SUM({GetBrandConditionSum(brand, "despatched_qty")}) AS despatched_qty,
                        SUM({GetBrandConditionSum(brand, "`value`")}) AS `value`
                        FROM
                        sales_orders
                        GROUP BY sales_orders.order_no,
                        sales_orders.date_entered,
                        sales_orders.customer,
                        sales_orders.customer_order_no
                        {(conditions.Count > 0 ? $" HAVING {string.Join(" AND ", conditions)}" : string.Empty)}
                        ORDER BY date_entered DESC, order_no DESC
                        LIMIT {skip},{take}";
            var headers = context.Database.SqlQuery<sales_orders_headers_summary>(sql, brand).ToList();

            var order_nos = headers.Select(h => h.order_no).ToList();
            var sales_orders = context.Set<Sales_orders>().Where(s => order_nos.Contains(s.order_no)).ToList();
            /*sql =
                $@"SELECT Sales_orders_headers_shipping.* FROM Sales_orders_headers_shipping INNER JOIN Sales_orders_headers ON Sales_orders_headers_shipping.header_id = Sales_orders_headers.id 
                    WHERE order_no IN ({string.Join(",", order_nos.Select(o => $"'{o}'"))})";
            var shippings = context.Database.SqlQuery<Sales_orders_headers_shipping>(sql);*/
            var sHeaders =
                context.Set<Sales_orders_headers>()
                    .Include(sh => sh.Shippings)
                    .Where(sh => order_nos.Contains(sh.order_no))
                    .ToList();
            foreach (var h in headers)
            {
                h.Lines = sales_orders.Where(s => s.order_no == h.order_no).ToList();
                var sHeader = sHeaders.FirstOrDefault(s => s.order_no == h.order_no);
                h.header_id = sHeader?.id;
                h.dealer_name = sHeader?.address1;
                h.Shippings = sHeader?.Shippings;
            }

            return headers;
        }

        public List<sales_orders_headers_summary> GetSummarySearch(string warehouse = null, bool? outstanding = null,
            int skip = 0, int take = 0, string text = null)
        {
            var conditions = new List<string>();
            var searchText = "";
            if (warehouse != null)
                conditions.Add(" COUNT(IF(warehouse=@p0 AND cprod_name NOT LIKE 'FREIGHT%',1,NULL)) > 0 ");
            if (outstanding != null)
                conditions.Add($" despatched_qty {(outstanding == true ? "<" : "=")} order_qty ");

            searchText = text != null ? $" WHERE sales_orders.order_no LIKE '%{text}%'" : string.Empty;

            var sql = $@"SELECT
                        sales_orders.order_no,
                        sales_orders.date_entered,
                        sales_orders.customer,
                        sales_orders.customer_order_no,
                        sales_orders.pick_list,
                        SUM({GetWarehouseConditionSum(warehouse, "order_qty")}) AS order_qty,
                        SUM({GetWarehouseConditionSum(warehouse, "despatched_qty")}) AS despatched_qty,
                        SUM({GetWarehouseConditionSum(warehouse, "`value`")}) AS `value`
                        FROM
                        sales_orders
                        {searchText}
                        GROUP BY sales_orders.order_no,
                        sales_orders.date_entered,
                        sales_orders.customer,
                        sales_orders.customer_order_no
                        {(conditions.Count > 0 ? $" HAVING {string.Join(" AND ", conditions)}" : string.Empty)}
                        
                        ORDER BY date_entered DESC, order_no DESC
                        LIMIT {skip},{take}";
            var headers = context.Database.SqlQuery<sales_orders_headers_summary>(sql, warehouse).ToList();

            var order_nos = headers.Select(h => h.order_no).ToList();
            var sales_orders = context.Set<Sales_orders>().Where(s => order_nos.Contains(s.order_no)).ToList();
            /*sql =
                $@"SELECT Sales_orders_headers_shipping.* FROM Sales_orders_headers_shipping INNER JOIN Sales_orders_headers ON Sales_orders_headers_shipping.header_id = Sales_orders_headers.id 
                    WHERE order_no IN ({string.Join(",", order_nos.Select(o => $"'{o}'"))})";
            var shippings = context.Database.SqlQuery<Sales_orders_headers_shipping>(sql);*/
            var sHeaders =
                context.Set<Sales_orders_headers>()
                    .Include(sh => sh.Shippings)
                    .Where(sh => order_nos.Contains(sh.order_no))
                    .ToList();
            foreach (var h in headers)
            {
                h.Lines = sales_orders.Where(s => s.order_no == h.order_no).ToList();
                var sHeader = sHeaders.FirstOrDefault(s => s.order_no == h.order_no);
                h.header_id = sHeader?.id;
                h.dealer_name = sHeader?.address1;
                h.Shippings = sHeader?.Shippings;
            }





            return headers;
        }

        private string GetWarehouseConditionSum(string warehouse, string field)
        {
            return warehouse != null ? $"IF (warehouse = @p0,{field},0)" : field;
        }

        private string GetBrandConditionSum(string brand, string field)
        {
            return brand != null ? $"IF (brand = @p0,{field},0)" : field;
        }

    }
}
