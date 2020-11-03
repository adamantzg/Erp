
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace asaq2.Model
{
    public class Stock_order_linesDAL
	{
	
		public static List<Stock_order_lines> GetAll()
		{
			var result = new List<Stock_order_lines>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT * FROM stock_order_lines", conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}
		
		
		public static Stock_order_lines GetById(int id)
		{
			Stock_order_lines result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT * FROM stock_order_lines WHERE linenum = @id", conn);
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
		
	
		private static Stock_order_lines GetFromDataReader(MySqlDataReader dr)
		{
			Stock_order_lines o = new Stock_order_lines();
		
			o.linenum =  (int) dr["linenum"];
			o.porderid = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"porderid"));
			o.linedate = Utilities.FromDbValue<DateTime>(Utilities.GetReaderField(dr,"linedate"));
			o.cprod_id = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"cprod_id"));
			o.desc1 = string.Empty + Utilities.GetReaderField(dr,"desc1");
			o.orderqty = Utilities.FromDbValue<double>(Utilities.GetReaderField(dr,"orderqty"));
			o.shipped = Utilities.FromDbValue<double>(Utilities.GetReaderField(dr,"shipped"));
			o.unitprice = Utilities.FromDbValue<double>(Utilities.GetReaderField(dr,"unitprice"));
			o.unitcurrency = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"unitcurrency"));
			o.linestatus = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"linestatus"));
            //o.mast_id = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"mast_id"));
            //o.mfg_code = string.Empty + Utilities.GetReaderField(dr,"mfg_code");
			o.lme = Utilities.FromDbValue<double>(Utilities.GetReaderField(dr,"lme"));
			
			return o;

		}


        public static void Create(Stock_order_lines o, MySqlTransaction tr = null)
        {
            string insertsql = @"INSERT INTO stock_order_lines (porderid,linedate,cprod_id,desc1,orderqty,shipped,unitprice,unitcurrency,linestatus,lme) VALUES(@porderid,@linedate,@cprod_id,@desc1,@orderqty,@shipped,@unitprice,@unitcurrency,@linestatus,@lme)";

			var conn = (tr != null ? tr.Connection : new MySqlConnection(Properties.Settings.Default.ConnString));
            if(tr == null)
                conn.Open();
				
			MySqlCommand cmd = new MySqlCommand(insertsql, conn,tr);
            BuildSqlParameters(cmd,o);
            cmd.ExecuteNonQuery();
			cmd.CommandText = "SELECT linenum FROM stock_order_lines WHERE linenum = LAST_INSERT_ID()";
            o.linenum = Convert.ToInt32(cmd.ExecuteScalar());
            if (tr == null)
                conn.Close();
		}
		
		private static void BuildSqlParameters(MySqlCommand cmd, Stock_order_lines o, bool forInsert = true)
        {
			
			if(!forInsert)
				cmd.Parameters.AddWithValue("@linenum", o.linenum);
			cmd.Parameters.AddWithValue("@porderid", o.porderid);
			cmd.Parameters.AddWithValue("@linedate", o.linedate);
			cmd.Parameters.AddWithValue("@cprod_id", o.cprod_id);
			cmd.Parameters.AddWithValue("@desc1", o.desc1);
			cmd.Parameters.AddWithValue("@orderqty", o.orderqty);
			cmd.Parameters.AddWithValue("@shipped", o.shipped);
			cmd.Parameters.AddWithValue("@unitprice", o.unitprice);
			cmd.Parameters.AddWithValue("@unitcurrency", o.unitcurrency);
			cmd.Parameters.AddWithValue("@linestatus", o.linestatus);
            //cmd.Parameters.AddWithValue("@mast_id", o.mast_id);
            //cmd.Parameters.AddWithValue("@mfg_code", o.mfg_code);
			cmd.Parameters.AddWithValue("@lme", o.lme);
		}

        public static void Update(Stock_order_lines o, MySqlTransaction tr = null)
		{
			string updatesql = @"UPDATE stock_order_lines SET porderid = @porderid,linedate = @linedate,cprod_id = @cprod_id,desc1 = @desc1,orderqty = @orderqty,shipped = @shipped,
        unitprice = @unitprice,unitcurrency = @unitcurrency,linestatus = @linestatus,lme = @lme WHERE linenum = @linenum";

            var conn = tr != null ? tr.Connection : new MySqlConnection(Properties.Settings.Default.ConnString);
            if (tr == null)
                conn.Open();
            var cmd = new MySqlCommand(updatesql, conn, tr);
            BuildSqlParameters(cmd, o, false);
            cmd.ExecuteNonQuery();

            if (tr == null)
                conn.Close();
		}

        public static void Delete(int linenum, MySqlTransaction tr = null)
		{
			var conn = tr != null ? tr.Connection : new MySqlConnection(Properties.Settings.Default.ConnString);
            if(tr == null)
                conn.Open();
			var cmd = new MySqlCommand("DELETE FROM stock_order_lines WHERE linenum = @id" , conn);
            cmd.Parameters.AddWithValue("@id", linenum);
            cmd.ExecuteNonQuery();
            if (tr == null)
                conn.Close();
		}


        public static List<Stock_order_lines> GetForOrder(int id)
        {
            
            var result = new List<Stock_order_lines>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand(@"SELECT stock_order_lines.*, cust_products.*,mast_products.* FROM stock_order_lines 
                                            INNER JOIN cust_products ON stock_order_lines.cprod_id = cust_products.cprod_id 
                                            INNER JOIN mast_products ON cust_products.cprod_mast = mast_products.mast_id
                                            WHERE porderid = @id", conn);
                cmd.Parameters.AddWithValue("@id", id);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    var line = GetFromDataReader(dr);
                    line.Product = Cust_productsDAL.GetFromDataReader(dr);
                    line.Product.MastProduct = Mast_productsDAL.GetFromDataReader(dr);
                    result.Add(line);

                }
                dr.Close();
            }
            return result;
        }
	}
}
			
			