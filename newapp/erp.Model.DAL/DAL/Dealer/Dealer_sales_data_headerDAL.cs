
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
    public partial class Dealer_sales_data_headerDAL
	{
	
		public static List<Dealer_sales_data_header> GetAll()
		{
			var result = new List<Dealer_sales_data_header>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT * FROM dealer_sales_data_header", conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}

        public static List<Dealer_sales_data_header> GetByDealer(int dealer_id, IDbConnection conn = null)
        {
            var result = new List<Dealer_sales_data_header>();
            var dispose = conn == null;
            if (conn == null)
            {
                conn = new MySqlConnection(Properties.Settings.Default.ConnString);
                conn.Open();
            }
            
            var cmd = new MySqlCommand("SELECT * FROM dealer_sales_data_header WHERE dealer_id = @dealer_id", (MySqlConnection) conn);
            cmd.Parameters.AddWithValue("@dealer_id", dealer_id);
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
		
		public static Dealer_sales_data_header GetById(int id)
		{
			Dealer_sales_data_header result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT * FROM dealer_sales_data_header WHERE orderid = @id", conn);
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
		
	
		public static Dealer_sales_data_header GetFromDataReader(MySqlDataReader dr)
		{
			Dealer_sales_data_header o = new Dealer_sales_data_header();

            o.orderid = (int)dr["orderid"];
			o.dealer_id =  (int) dr["dealer_id"];
            o.order_date = Utilities.FromDbValue<DateTime>(Utilities.GetReaderField(dr, "order_date"));
            o.entered_by = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "entered_by"));
            o.reference = string.Empty + dr["reference"];
            o.for_arcade = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "for_arcade"));
			
			return o;

		}
		
		
		public static void Create(Dealer_sales_data_header o, IDbTransaction tr = null)
        {
            const string insertsql = @"INSERT INTO dealer_sales_data_header (dealer_id,order_date,entered_by, reference, for_arcade) VALUES(@dealer_id,@order_date,@entered_by, @reference, @for_arcade)";
		    var dispose = tr == null;
		    var conn = tr == null ? new MySqlConnection(Properties.Settings.Default.ConnString) : tr.Connection;
            if(tr == null)
                conn.Open();
			
			var cmd = new MySqlCommand(insertsql,(MySqlConnection) conn,(MySqlTransaction) tr);
            BuildSqlParameters(cmd,o);
            cmd.ExecuteNonQuery();
            cmd.CommandText = "SELECT orderid FROM dealer_sales_data_header WHERE orderid = LAST_INSERT_ID()";
            o.orderid = (int) cmd.ExecuteScalar();
				
            if(dispose)
                conn.Dispose();
		}
		
		private static void BuildSqlParameters(MySqlCommand cmd, Dealer_sales_data_header o, bool forInsert = true)
        {
			
			if(!forInsert)
                cmd.Parameters.AddWithValue("@orderid", o.orderid);
			cmd.Parameters.AddWithValue("@dealer_id", o.dealer_id);
            cmd.Parameters.AddWithValue("@order_date", o.order_date);
            cmd.Parameters.AddWithValue("@entered_by", o.entered_by);
            cmd.Parameters.AddWithValue("@reference", o.reference);
            cmd.Parameters.AddWithValue("@for_arcade", o.for_arcade);
		}
		
		public static void Update(Dealer_sales_data_header o)
		{
            string updatesql = @"UPDATE dealer_sales_data_header SET dealer_id = @dealer_id,order_date = @order_date,entered_by = @entered_by,reference = @reference, for_arcade=@for_arcade WHERE orderid = @orderid";

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
                var cmd = new MySqlCommand("DELETE FROM dealer_sales_data_header WHERE orderid = @id", conn);
                cmd.Parameters.AddWithValue("@id", unique_id);
                cmd.ExecuteNonQuery();
            }
		}
		
		
	}
}
			
			