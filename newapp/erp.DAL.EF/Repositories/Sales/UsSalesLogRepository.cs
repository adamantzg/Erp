using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using erp.Model;

namespace erp.DAL.EF.Repositories
{
    public class UsSalesLogRepository : GenericRepository<Ussales_log>
    {
        public UsSalesLogRepository(Model context) : base(context)
        {
        }

        public List<ussales_log_report> GetReport(DateTime? from,DateTime? to, int? type = null)
        {
            var sql = GetSelect(type);
            return context.Database.SqlQuery<ussales_log_report>(sql,from,to).ToList();
        }

        private string GetSelect(int? type)
        {
            var sql = new StringBuilder();
            if (type != Ussales_log.type_SalesOut)
                sql.Append($@"SELECT userusers.userwelcome, 'Goods In' AS Type, CONCAT('9',order_header.orderid) AS custpo, ussales_log.logdate,
                          ussales_log.user_id, cust_products.cprod_code1, cust_products.cprod_name, ussales_log.old_qty,
                          ussales_log.new_qty, ussales_log.lineid, {Ussales_log.type_GoodsIn} AS type_id, order_lines.orderqty, order_lines.received_qty, NULL AS despatched_qty, 
                            NULL AS Dealer, NULL AS State, NULL AS orderDate
                   FROM `ussales_log` INNER JOIN userusers ON ussales_log.user_id = userusers.useruserid
                   INNER JOIN order_lines ON ussales_log.lineid = order_lines.linenum INNER JOIN cust_products ON order_lines.cprod_id = cust_products.cprod_id
                   INNER JOIN order_header ON order_lines.orderid = order_header.orderid
                   WHERE ussales_log.logdate >= @p0 AND ussales_log.logdate <= @p1  AND Type = {Ussales_log.type_GoodsIn}");
            if (type == null)
                sql.Append(" UNION ");
            if(type != Ussales_log.type_GoodsIn)
                sql.Append($@"SELECT userusers.userwelcome, 'Sales Out' AS Type, sales_orders.order_no AS custpo, ussales_log.logdate,
                          ussales_log.user_id, sales_orders.cprod_code1, sales_orders.cprod_name, ussales_log.old_qty,
                          ussales_log.new_qty, ussales_log.lineid, {Ussales_log.type_SalesOut} AS type_id, sales_orders.order_qty AS orderqty, NULL AS received_qty, sales_orders.despatched_qty,
                          sales_orders.alpha AS Dealer, us_dealers.state_region AS State, COALESCE(sales_orders.pick_list_date,date_entered) AS orderDate
                   FROM `ussales_log` INNER JOIN userusers ON ussales_log.user_id = userusers.useruserid
                   INNER JOIN sales_orders ON ussales_log.lineid = sales_orders.rowid                    
                    INNER JOIN us_dealers ON sales_orders.customer = us_dealers.customer
                   WHERE ussales_log.logdate >= @p0 AND ussales_log.logdate <= @p1 AND Type = {Ussales_log.type_SalesOut}");
            sql.Append(" ORDER BY userwelcome, Type,custpo,lineid");
            return sql.ToString();

            
        }
    }
}
