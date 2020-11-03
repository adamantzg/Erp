
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace asaq2.Model
{
    
    public class Stock_order_allocationDAL
	{
	
		public static List<Stock_order_allocation> GetByProduct(int cprod_id)
		{
			var result = new List<Stock_order_allocation>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand(GetSelectSql() + " WHERE so_lines.cprod_id = @cprod_id", conn);
                cmd.Parameters.AddWithValue("@cprod_id", cprod_id);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}

        private static string GetSelectSql()
        {
            return
                @"SELECT stock_order_allocation.unique_link_ref,stock_order_allocation.so_line,stock_order_allocation.st_line,stock_order_allocation.alloc_qty,stock_order_allocation.date_allocation,
                        stock_order_allocation.line_status,so_lines.orderid AS so_orderid,co_lines.orderid AS co_orderid, so_lines.cprod_id, porder_header.po_req_etd, co_header.custpo AS co_custpo, so_header.custpo AS so_custpo FROM
                        stock_order_allocation INNER JOIN order_lines AS co_lines ON stock_order_allocation.so_line = co_lines.linenum INNER JOIN order_header AS co_header ON co_lines.orderid = co_header.orderid
                        INNER JOIN order_lines AS so_lines ON so_lines.linenum = stock_order_allocation.st_line INNER JOIN order_header AS so_header ON so_lines.orderid = so_header.orderid LEFT OUTER JOIN porder_lines ON 
                        so_lines.linenum = porder_lines.soline INNER JOIN porder_header on porder_header.porderid = porder_lines.porderid";
        }


        //public static Stock_order_allocation GetById(int id)
        //{
        //    Stock_order_allocation result = null;
        //    using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
        //    {
        //        conn.Open();
        //        MySqlCommand cmd = new MySqlCommand("SELECT * FROM stock_order_allocation WHERE unique_link_ref = @id", conn);
        //        cmd.Parameters.AddWithValue("@id", id);
        //        MySqlDataReader dr = cmd.ExecuteReader();
        //        if (dr.Read())
        //        {
        //            result = GetFromDataReader(dr);
        //        }
        //        dr.Close();
        //    }
        //    return result;
        //}
		
	
		private static Stock_order_allocation GetFromDataReader(MySqlDataReader dr)
		{
			var o = new Stock_order_allocation();
		
			o.unique_link_ref =  (int) dr["unique_link_ref"];
			o.so_line = Utilities.FromDbValue<int>(dr["so_line"]);
			o.st_line = Utilities.FromDbValue<int>(dr["st_line"]);
			o.alloc_qty = Utilities.FromDbValue<int>(dr["alloc_qty"]);
			o.date_allocation = Utilities.FromDbValue<DateTime>(dr["date_allocation"]);
			o.line_status = string.Empty + dr["line_status"];
		    o.so_orderid = (int) dr["so_orderid"];
		    o.co_orderid = (int) dr["co_orderid"];
		    o.cprod_id = (int) dr["cprod_id"];
		    o.co_custpo = string.Empty + dr["co_custpo"];
		    o.so_custpo = string.Empty + dr["so_custpo"];
            if (Utilities.ColumnExists(dr, "po_req_etd"))
		        o.po_req_etd = Utilities.FromDbValue<DateTime>(dr["po_req_etd"]);
			return o;

		}

        public static List<soalloc_overallocation> GetOverAllocations()
        {
            var result = new List<soalloc_overallocation>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand(@"SELECT stock_lines.linenum,stock_lines.cprod_id,cust_products.cprod_code1, cust_products.cprod_name,stock_header.custpo,SUM(alloc_qty) AS allocated, stock_lines.orderqty
                                FROM  stock_order_allocation
                                INNER JOIN order_lines AS calloff_lines ON calloff_lines.linenum = stock_order_allocation.so_line
                                INNER JOIN order_header AS calloff_header ON calloff_lines.orderid = calloff_header.orderid
                                INNER JOIN order_lines AS stock_lines ON stock_lines.linenum = stock_order_allocation.st_line
                                INNER JOIN order_header AS stock_header ON stock_lines.orderid = stock_header.orderid
                                INNER JOIN cust_products ON stock_lines.cprod_id = cust_products.cprod_id
                                WHERE calloff_header.`status` NOT IN ('X','Y') AND stock_header.status NOT IN ('X','Y')
                                GROUP BY stock_order_allocation.st_line HAVING SUM(alloc_qty) > stock_lines.orderqty", conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    var obj = new soalloc_overallocation
                        {
                            stockLineNum = (int) dr["linenum"],
                            Product =
                                new Cust_products
                                    {
                                        cprod_id = (int) dr["cprod_id"],
                                        cprod_code1 = string.Empty + dr["cprod_code1"],
                                        cprod_name = string.Empty + dr["cprod_name"]
                                    },
                            Header = new Order_header{custpo = string.Empty + dr["custpo"]},
                            AllocQty = Convert.ToInt32(dr["allocated"]),
                            StockLineQty = Convert.ToInt32(dr["orderqty"])
                        };
                    result.Add(obj);
                }
            }
            return result;
        }

        public static List<Order_lines> GetAllocationCalloffLines(int stocklineNum)
        {
            var result = new List<Order_lines>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand(@"SELECT order_lines.*,order_header.custpo,porder_header.po_req_etd, stock_order_allocation.alloc_qty, stock_order_allocation.unique_link_ref
                                        FROM order_lines INNER JOIN stock_order_allocation ON order_lines.linenum = stock_order_allocation.so_line 
                                        INNER JOIN order_header ON order_lines.orderid = order_header.orderid
                                         INNER JOIN porder_lines ON order_lines.linenum = porder_lines.soline 
	                                     INNER JOIN porder_header ON porder_lines.porderid = porder_header.porderid
                                        WHERE st_line = @stockline ORDER BY order_lines.linedate", conn);
                cmd.Parameters.AddWithValue("@stockline", stocklineNum);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    var line = Order_linesDAL.GetFromDataReader(dr);
                    line.Header = new Order_header {custpo = string.Empty + dr["custpo"],po_req_etd = Utilities.FromDbValue<DateTime>(dr["po_req_etd"])};
                    line.AllocQty = Utilities.FromDbValue<int>(dr["alloc_qty"]);
                    line.allocation_id = (int) dr["unique_link_ref"];
                    result.Add(line);
                }
                dr.Close();
            }
            return result;
        }

        public static List<Order_lines> GetAvailableStockLines(int cprod_id)
        {
            var result = new List<Order_lines>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand(@"SELECT stock_lines.linenum,stock_header.custpo,porder_header.po_req_etd,stock_lines.orderqty
	                        FROM  
	                         order_lines AS stock_lines
	                        INNER JOIN order_header AS stock_header ON stock_lines.orderid = stock_header.orderid
	                        INNER JOIN porder_lines ON stock_lines.linenum = porder_lines.soline 
	                        INNER JOIN porder_header ON porder_lines.porderid = porder_header.porderid
	                        INNER JOIN cust_products ON stock_lines.cprod_id = cust_products.cprod_id
	                        WHERE stock_header.status NOT IN ('X','Y') AND stock_header.stock_order = 1 AND stock_header.req_eta > CURDATE() AND stock_lines.orderqty > 0 AND stock_lines.cprod_id = @cprod_id", conn);
                cmd.Parameters.AddWithValue("@cprod_id", cprod_id);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    var line = new Order_lines
                        {
                            linenum = (int) dr["linenum"],
                            Header = new Order_header
                                {
                                    custpo = string.Empty + dr["custpo"],
                                    po_req_etd = (DateTime) dr["po_req_etd"]
                                },
                            orderqty = Convert.ToInt32(dr["orderqty"])
                        };
                    result.Add(line);
                    
                }
                dr.Close();
                foreach (var line in result)
                {
                    line.AllocatedLines = GetAllocationCalloffLines(line.linenum);
                    line.AllocQty = line.AllocatedLines.Sum(l => l.orderqty != null ? Convert.ToInt32(l.orderqty.Value) : 0);
                }
            }
            return result;
        }


        //public static void Create(Stock_order_allocation o)
        //{
        //    string insertsql = @"INSERT INTO stock_order_allocation (so_line,st_line,alloc_qty,date_allocation,line_status) VALUES(@so_line,@st_line,@alloc_qty,@date_allocation,@line_status)";

        //    using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
        //    {
        //        conn.Open();
				
        //        MySqlCommand cmd = new MySqlCommand(insertsql, conn);
        //        BuildSqlParameters(cmd,o);
        //        cmd.ExecuteNonQuery();
        //        cmd.CommandText = "SELECT unique_link_ref FROM stock_order_allocation WHERE unique_link_ref = LAST_INSERT_ID()";
        //        o.unique_link_ref = (int) cmd.ExecuteScalar();
				
        //    }
        //}
		
        //private static void BuildSqlParameters(MySqlCommand cmd, Stock_order_allocation o, bool forInsert = true)
        //{
			
        //    if(!forInsert)
        //        cmd.Parameters.AddWithValue("@unique_link_ref", o.unique_link_ref);
        //    cmd.Parameters.AddWithValue("@so_line", o.so_line);
        //    cmd.Parameters.AddWithValue("@st_line", o.st_line);
        //    cmd.Parameters.AddWithValue("@alloc_qty", o.alloc_qty);
        //    cmd.Parameters.AddWithValue("@date_allocation", o.date_allocation);
        //    cmd.Parameters.AddWithValue("@line_status", o.line_status);
        //}
		
        //public static void Update(Stock_order_allocation o)
        //{
        //    string updatesql = @"UPDATE stock_order_allocation SET so_line = @so_line,st_line = @st_line,alloc_qty = @alloc_qty,date_allocation = @date_allocation,line_status = @line_status WHERE unique_link_ref = @unique_link_ref";

        //    using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
        //    {
        //        conn.Open();
        //        MySqlCommand cmd = new MySqlCommand(updatesql, conn);
        //        BuildSqlParameters(cmd,o, false);
        //        cmd.ExecuteNonQuery();
        //    }
        //}
		
        //public static void Delete(int unique_link_ref)
        //{
        //    using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
        //    {
        //        conn.Open();
        //        MySqlCommand cmd = new MySqlCommand("DELETE FROM stock_order_allocation WHERE unique_link_ref = @id" , conn);
        //        cmd.Parameters.AddWithValue("@id", unique_link_ref);
        //        cmd.ExecuteNonQuery();
        //    }
        //}
		
		
	}
}
			
			