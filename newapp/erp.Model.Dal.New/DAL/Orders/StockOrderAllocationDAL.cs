
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
    
    public class StockOrderAllocationDAL : IStockOrderAllocationDAL
    {
	    private MySqlConnection conn;

	    public StockOrderAllocationDAL(IDbConnection conn)
	    {
		    this.conn = (MySqlConnection) conn;
	    }
	
		public List<Stock_order_allocation> GetByProducts(IList<int> cprod_ids)
		{
			return conn.Query<Stock_order_allocation>(
				GetSelectSql() + " WHERE so_lines.cprod_id IN @cprod_ids",
				new {cprod_ids}).ToList();
		}

        private string GetSelectSql()
        {
            return
                @"SELECT stock_order_allocation.unique_link_ref,stock_order_allocation.so_line,stock_order_allocation.st_line,stock_order_allocation.alloc_qty,stock_order_allocation.date_allocation,
                        stock_order_allocation.line_status,so_lines.orderid AS so_orderid,co_lines.orderid AS co_orderid, so_lines.cprod_id, porder_header.po_req_etd, co_header.custpo AS co_custpo, so_header.custpo AS so_custpo FROM
                        stock_order_allocation INNER JOIN order_lines AS co_lines ON stock_order_allocation.so_line = co_lines.linenum INNER JOIN order_header AS co_header ON co_lines.orderid = co_header.orderid
                        INNER JOIN order_lines AS so_lines ON so_lines.linenum = stock_order_allocation.st_line INNER JOIN order_header AS so_header ON so_lines.orderid = so_header.orderid LEFT OUTER JOIN porder_lines ON 
                        so_lines.linenum = porder_lines.soline INNER JOIN porder_header on porder_header.porderid = porder_lines.porderid";
        }
		
        public List<soalloc_overallocation> GetOverAllocations()
        {
	        return conn.Query<Cust_products, soalloc_overallocation, Order_header, soalloc_overallocation>(
		        @"SELECT stock_lines.cprod_id, cust_products.cprod_code1, cust_products.cprod_name, 
					stock_lines.linenum as stocklinenum,SUM(alloc_qty) AS allocqty,	stock_lines.orderqty AS stocklineqty,
					order_header.orderid, order_header.custpo
                    FROM  stock_order_allocation
                    INNER JOIN order_lines AS calloff_lines ON calloff_lines.linenum = stock_order_allocation.so_line
                    INNER JOIN order_header AS calloff_header ON calloff_lines.orderid = calloff_header.orderid
                    INNER JOIN order_lines AS stock_lines ON stock_lines.linenum = stock_order_allocation.st_line
                    INNER JOIN order_header AS stock_header ON stock_lines.orderid = stock_header.orderid
                    INNER JOIN cust_products ON stock_lines.cprod_id = cust_products.cprod_id
                    WHERE calloff_header.`status` NOT IN ('X','Y') AND stock_header.status NOT IN ('X','Y')
                    GROUP BY stock_order_allocation.st_line HAVING SUM(alloc_qty) > stock_lines.orderqty",
		        (cp, so, oh) =>
		        {
			        so.Header = oh;
			        so.Product = cp;
			        return so;
		        }, splitOn: "linenum, orderid").ToList();

        }

        public List<Order_lines> GetAllocationCalloffLines(int stocklineNum)
        {
	        return conn.Query<Order_header, Order_lines, Order_lines>(
		        @"SELECT order_header.custpo,porder_header.po_req_etd, order_lines.*,stock_order_allocation.alloc_qty as allocqty,
					stock_order_allocation.unique_link_ref as allocation_id
                    FROM order_lines INNER JOIN stock_order_allocation ON order_lines.linenum = stock_order_allocation.so_line 
                    INNER JOIN order_header ON order_lines.orderid = order_header.orderid
                     INNER JOIN porder_lines ON order_lines.linenum = porder_lines.soline 
                     INNER JOIN porder_header ON porder_lines.porderid = porder_header.porderid
                    WHERE st_line = @stocklineNum ORDER BY order_lines.linedate",
		        (oh, ol) =>
		        {
			        ol.Header = oh;
			        return ol;
		        }, new {stocklineNum}, splitOn: "linenum").ToList();
			
        }

        public List<Order_lines> GetAllocationSOLines(int colineNum)
        {
			return conn.Query<Order_header, Order_lines, Order_lines>(
				@"SELECT order_header.custpo,porder_header.po_req_etd,order_lines.*,stock_order_allocation.alloc_qty as allocqty,
				stock_order_allocation.unique_link_ref as allocation_id
                FROM order_lines INNER JOIN stock_order_allocation ON order_lines.linenum = stock_order_allocation.st_line 
                INNER JOIN order_header ON order_lines.orderid = order_header.orderid
                 INNER JOIN porder_lines ON order_lines.linenum = porder_lines.soline 
                 INNER JOIN porder_header ON porder_lines.porderid = porder_header.porderid
                WHERE so_line = @colineNum ORDER BY order_lines.linedate",
				(oh, ol) =>
				{
					ol.Header = oh;
					return ol;
				}, new {colineNum}, splitOn: "linenum").ToList();

            
        }

        public List<Order_lines> GetAvailableStockLines(int cprod_id)
        {
			var lines = conn.Query<Order_header, Order_lines, Order_lines>(
				@"SELECT stock_header.custpo,porder_header.po_req_etd, stock_lines.linenum,stock_lines.orderqty
	            FROM  
	             order_lines AS stock_lines
	            INNER JOIN order_header AS stock_header ON stock_lines.orderid = stock_header.orderid
	            INNER JOIN porder_lines ON stock_lines.linenum = porder_lines.soline 
	            INNER JOIN porder_header ON porder_lines.porderid = porder_header.porderid
	            INNER JOIN cust_products ON stock_lines.cprod_id = cust_products.cprod_id
	            WHERE stock_header.status NOT IN ('X','Y') AND stock_header.stock_order = 1 AND stock_header.req_eta > CURDATE() 
				AND stock_lines.orderqty > 0 AND stock_lines.cprod_id = @cprod_id",
					(oh, ol) =>
					{
						ol.Header = oh;
						return ol;
					}, new {cprod_id}, splitOn: "linenum").ToList();
	        foreach (var line in lines)
	        {
		        line.AllocatedLines = GetAllocationCalloffLines(line.linenum);
		        line.AllocQty = line.AllocatedLines.Sum(l => l.orderqty != null ? Convert.ToInt32(l.orderqty.Value) : 0);
	        }

	        return lines;
        }

        public List<OrderLinesLight> GetOrderLineLightForReport(int? cprod_id)
        {
	        return conn.Query<OrderLinesLight>(
		        @"
                SELECT 			
                    stock_lines.cprod_id
                    ,stock_header.orderid
                    ,stock_header.custpo
                    ,stock_lines.orderqty
                    ,calloff_header.custpo as calloff_custpo
                    ,alloc_qty
                FROM  stock_order_allocation
                    INNER JOIN order_lines AS calloff_lines ON calloff_lines.linenum = stock_order_allocation.so_line
                    INNER JOIN order_header AS calloff_header ON calloff_lines.orderid = calloff_header.orderid
                    INNER JOIN order_lines AS stock_lines ON stock_lines.linenum = stock_order_allocation.st_line
                    INNER JOIN order_header AS stock_header ON stock_lines.orderid = stock_header.orderid
                    INNER JOIN cust_products ON stock_lines.cprod_id = cust_products.cprod_id
                WHERE alloc_qty > 0  AND
                    stock_lines.cprod_id = @cprod_id",
		        new {cprod_id}).ToList();

        }
        
    }
}
			
			