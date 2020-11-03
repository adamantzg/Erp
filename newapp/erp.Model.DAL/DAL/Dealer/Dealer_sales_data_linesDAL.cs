using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using MySql.Data;
using erp.Model;
using MySql.Data.MySqlClient;

namespace erp.Model.DAL
{
    public partial class Dealer_sales_data_linesDAL
	{
	
		public static List<Dealer_sales_data_lines> GetAll()
		{
			var result = new List<Dealer_sales_data_lines>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT * FROM dealer_sales_data_lines", conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}

		public static Dealer_sales_data_lines GetById(int id)
		{
			Dealer_sales_data_lines result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT * FROM dealer_sales_data_lines WHERE lineid = @id", conn);
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

        public static List<Dealer_sales_data_lines> GetByOrderId(int id)
        {
            var results = new List<Dealer_sales_data_lines>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT * FROM dealer_sales_data_lines WHERE orderid = @id", conn);
                cmd.Parameters.AddWithValue("@id", id);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    var dsdl = GetFromDataReader(dr);
                    results.Add(dsdl);
                }
                dr.Close();
            }
            return results;
        }
		
	
		public static Dealer_sales_data_lines GetFromDataReader(MySqlDataReader dr)
		{
			Dealer_sales_data_lines o = new Dealer_sales_data_lines();

            o.lineid = (int)dr["lineid"];
            o.orderid = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "orderid"));
            o.cprod_id = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "cprod_id"));
            o.qty = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "qty"));
            o.unit_price = Utilities.FromDbValue<double>(Utilities.GetReaderField(dr, "unit_price"));
			
			return o;

		}
		
		
		public static void Create(Dealer_sales_data_lines o, IDbTransaction tr = null)
        {
            const string insertsql = @"INSERT INTO dealer_sales_data_lines (orderid,cprod_id,qty, unit_price) VALUES(@orderid,@cprod_id,@qty, @unit_price)";
		    var dispose = tr == null;
		    var conn = tr == null ? new MySqlConnection(Properties.Settings.Default.ConnString) : tr.Connection;
            if(tr == null)
                conn.Open();
			
			var cmd = new MySqlCommand(insertsql,(MySqlConnection) conn,(MySqlTransaction) tr);
            BuildSqlParameters(cmd,o);
            cmd.ExecuteNonQuery();
            cmd.CommandText = "SELECT lineid FROM dealer_sales_data_lines WHERE lineid = LAST_INSERT_ID()";
            o.orderid = (int) cmd.ExecuteScalar();
				
            if(dispose)
                conn.Dispose();
		}
		
		private static void BuildSqlParameters(MySqlCommand cmd, Dealer_sales_data_lines o, bool forInsert = true)
        {
			
			if(!forInsert)
                cmd.Parameters.AddWithValue("@lineid", o.lineid);
            cmd.Parameters.AddWithValue("@orderid", o.orderid);
            cmd.Parameters.AddWithValue("@cprod_id", o.cprod_id);
            cmd.Parameters.AddWithValue("@qty", o.qty);
            cmd.Parameters.AddWithValue("@unit_price", o.unit_price);
		}
		
		public static void Update(Dealer_sales_data_lines o)
		{
            string updatesql = @"UPDATE dealer_sales_data_lines SET orderid = @orderid,cprod_id = @cprod_id,qty = @qty,unit_price = @unit_price WHERE lineid = @lineid";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = new MySqlCommand(updatesql, conn);
                BuildSqlParameters(cmd,o, false);
                cmd.ExecuteNonQuery();
            }
		}
		
		public static void Delete(int unique_id)
		{
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand("DELETE FROM dealer_sales_data_lines WHERE lineid = @id", conn);
                cmd.Parameters.AddWithValue("@id", unique_id);
                cmd.ExecuteNonQuery();
            }
		}

        public static void DeleteByOrderId(int unique_id)
        {
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand("DELETE FROM dealer_sales_data_lines WHERE orderid = @id", conn);
                cmd.Parameters.AddWithValue("@id", unique_id);
                cmd.ExecuteNonQuery();
            }
        }
		
		
	}
}
			
			