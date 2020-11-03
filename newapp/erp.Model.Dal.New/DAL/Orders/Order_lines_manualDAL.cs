
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using MySql.Data;
using erp.Model;
using MySql.Data.MySqlClient;
using Dapper;

namespace erp.Model.Dal.New
{
    public class OrderLinesManualDAL : IOrderLinesManualDAL
    {
		private MySqlConnection conn;

		public OrderLinesManualDAL(IDbConnection conn)
		{
			this.conn = (MySqlConnection) conn;
		}
	
		public List<Order_lines_manual> GetAll()
		{
			return conn.Query<Order_lines_manual>("SELECT * FROM order_lines_manual").ToList();
		}

        public List<Order_lines_manual> GetForOrder(int orderid)
        {
	        return conn.Query<Order_lines_manual>(
		        "SELECT order_lines_manual.* FROM order_lines_manual WHERE orderid = @orderid",
		        new {orderid}).ToList();
        }

        public List<Order_lines_manual> GetForOrders(IEnumerable<int?> orderids)
        {
			if(orderids.Any())
				return conn.Query<Order_lines_manual>("SELECT order_lines_manual.* FROM order_lines_manual WHERE orderid IN @orderids", new { orderids = orderids }).ToList();
			return new List<Order_lines_manual>();
        }


        public Order_lines_manual GetById(int id)
        {
	        return conn.QueryFirstOrDefault<Order_lines_manual>("SELECT * FROM order_lines_manual WHERE linenum = @id",
		        new {id});
			
		}
		
		public void Create(Order_lines_manual o)
        {
            string insertsql = @"INSERT INTO order_lines_manual (orderid,linedate,cprod_id,description,orderqty,override_cartonqty,unitprice,unitcurrency,linestatus,record_type,net_weight,gross_weight) VALUES(@orderid,@linedate,@cprod_id,@description,@orderqty,@override_cartonqty,@unitprice,@unitcurrency,@linestatus,@record_type,@net_weight,@gross_weight)";

	        conn.Execute(insertsql, o);
	        o.linenum = conn.ExecuteScalar<int>("SELECT LAST_INSERT_ID()");
		}
		
		public void Update(Order_lines_manual o)
		{
			string updatesql = @"UPDATE order_lines_manual SET orderid = @orderid,linedate = @linedate,cprod_id = @cprod_id,description = @description,orderqty = @orderqty,override_cartonqty = @override_cartonqty,unitprice = @unitprice,unitcurrency = @unitcurrency,linestatus = @linestatus,record_type = @record_type,net_weight = @net_weight,gross_weight = @gross_weight WHERE linenum = @linenum";

			conn.Execute(updatesql, o);
		}
		
		public void Delete(int linenum)
		{
			conn.Execute("DELETE FROM order_lines_manual WHERE linenum = @linenum", new {linenum});
		}
		
		
	}
}
			
			