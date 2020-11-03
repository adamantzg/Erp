
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using MySql.Data;
using erp.Model;
using MySql.Data.MySqlClient;
using Dapper;

namespace erp.Model.DAL
{
    public partial class Order_lines_manualDAL
	{
	
		public static List<Order_lines_manual> GetAll()
		{
			var result = new List<Order_lines_manual>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM order_lines_manual", conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}

        public static List<Order_lines_manual> GetForOrder(int orderid, IDbConnection conn = null)
        {
            var result = new List<Order_lines_manual>();
            bool dispose = false;
            if (conn == null)
            {
                conn = new MySqlConnection(Properties.Settings.Default.ConnString);
                conn.Open();
                dispose = true;
            }

            var cmd = Utils.GetCommand("SELECT order_lines_manual.* FROM order_lines_manual WHERE orderid = @orderid", (MySqlConnection)conn);
            cmd.Parameters.AddWithValue("@orderid", orderid);
            var dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                result.Add(GetFromDataReader(dr));
            }
            dr.Close();

            if(dispose)
                conn.Dispose();
            
            return result;
        }

        public static List<Order_lines_manual> GetForOrders(IEnumerable<int?> orderids, IDbConnection conn = null)
        {
            var result = new List<Order_lines_manual>();
            if(orderids.Count() > 0) {
                bool dispose = false;
                if (conn == null) {
                    conn = new MySqlConnection(Properties.Settings.Default.ConnString);
                    conn.Open();
                    dispose = true;
                }

                result = conn.Query<Order_lines_manual>("SELECT order_lines_manual.* FROM order_lines_manual WHERE orderid IN @orderids", new { orderids = orderids }).ToList();

                if (dispose)
                    conn.Dispose();
            }            

            return result;
        }


        public static Order_lines_manual GetById(int id)
		{
			Order_lines_manual result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM order_lines_manual WHERE linenum = @id", conn);
				cmd.Parameters.AddWithValue("@id", id);
                var dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    result = GetFromDataReader(dr);
                }
                dr.Close();
            }
			return result;
		}
		
	
		public static Order_lines_manual GetFromDataReader(MySqlDataReader dr)
		{
			Order_lines_manual o = new Order_lines_manual();
		
			o.linenum =  (int) dr["linenum"];
			o.orderid = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"orderid"));
			o.linedate = Utilities.FromDbValue<DateTime>(Utilities.GetReaderField(dr,"linedate"));
			o.cprod_id = string.Empty + Utilities.GetReaderField(dr,"cprod_id");
			o.description = string.Empty + Utilities.GetReaderField(dr,"description");
			o.orderqty = Utilities.FromDbValue<double>(Utilities.GetReaderField(dr,"orderqty"));
			o.override_cartonqty = Utilities.FromDbValue<double>(Utilities.GetReaderField(dr,"override_cartonqty"));
			o.unitprice = Utilities.FromDbValue<double>(Utilities.GetReaderField(dr,"unitprice"));
			o.unitcurrency = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"unitcurrency"));
			o.linestatus = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"linestatus"));
			o.record_type = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"record_type"));
			o.net_weight = Utilities.FromDbValue<double>(Utilities.GetReaderField(dr,"net_weight"));
			o.gross_weight = Utilities.FromDbValue<double>(Utilities.GetReaderField(dr,"gross_weight"));
			
			return o;

		}
		
		
		public static void Create(Order_lines_manual o)
        {
            string insertsql = @"INSERT INTO order_lines_manual (orderid,linedate,cprod_id,description,orderqty,override_cartonqty,unitprice,unitcurrency,linestatus,record_type,net_weight,gross_weight) VALUES(@orderid,@linedate,@cprod_id,@description,@orderqty,@override_cartonqty,@unitprice,@unitcurrency,@linestatus,@record_type,@net_weight,@gross_weight)";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				
				var cmd = Utils.GetCommand(insertsql, conn);
                BuildSqlParameters(cmd,o);
                cmd.ExecuteNonQuery();
				cmd.CommandText = "SELECT linenum FROM order_lines_manual WHERE linenum = LAST_INSERT_ID()";
                o.linenum = (int) cmd.ExecuteScalar();
				
            }
		}
		
		private static void BuildSqlParameters(MySqlCommand cmd, Order_lines_manual o, bool forInsert = true)
        {
			
			if(!forInsert)
				cmd.Parameters.AddWithValue("@linenum", o.linenum);
			cmd.Parameters.AddWithValue("@orderid", o.orderid);
			cmd.Parameters.AddWithValue("@linedate", o.linedate);
			cmd.Parameters.AddWithValue("@cprod_id", o.cprod_id);
			cmd.Parameters.AddWithValue("@description", o.description);
			cmd.Parameters.AddWithValue("@orderqty", o.orderqty);
			cmd.Parameters.AddWithValue("@override_cartonqty", o.override_cartonqty);
			cmd.Parameters.AddWithValue("@unitprice", o.unitprice);
			cmd.Parameters.AddWithValue("@unitcurrency", o.unitcurrency);
			cmd.Parameters.AddWithValue("@linestatus", o.linestatus);
			cmd.Parameters.AddWithValue("@record_type", o.record_type);
			cmd.Parameters.AddWithValue("@net_weight", o.net_weight);
			cmd.Parameters.AddWithValue("@gross_weight", o.gross_weight);
		}
		
		public static void Update(Order_lines_manual o)
		{
			string updatesql = @"UPDATE order_lines_manual SET orderid = @orderid,linedate = @linedate,cprod_id = @cprod_id,description = @description,orderqty = @orderqty,override_cartonqty = @override_cartonqty,unitprice = @unitprice,unitcurrency = @unitcurrency,linestatus = @linestatus,record_type = @record_type,net_weight = @net_weight,gross_weight = @gross_weight WHERE linenum = @linenum";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = Utils.GetCommand(updatesql, conn);
                BuildSqlParameters(cmd,o, false);
                cmd.ExecuteNonQuery();
            }
		}
		
		public static void Delete(int linenum)
		{
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = Utils.GetCommand("DELETE FROM order_lines_manual WHERE linenum = @id" , conn);
                cmd.Parameters.AddWithValue("@id", linenum);
                cmd.ExecuteNonQuery();
            }
		}
		
		
	}
}
			
			