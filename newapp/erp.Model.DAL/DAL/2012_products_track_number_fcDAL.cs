
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using erp.Model;
using MySql.Data.MySqlClient;

namespace erp.Model.DAL
{
    public partial class products_track_number_fcDAL
	{
	
		public static List<products_track_number_fc> GetAll()
		{
			var result = new List<products_track_number_fc>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT * FROM 2012_products_track_number_fc", conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}

        public static List<products_track_number_fc> GetByCriteria(int? orderid = null, int? mastid = null)
        {
            var result = new List<products_track_number_fc>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT * FROM 2012_products_track_number_fc WHERE (orderid = @orderid OR @orderid IS NULL) AND (mastid = @mastid OR @mastid IS NULL)", conn);
                cmd.Parameters.AddWithValue("@orderid", Utilities.ToDBNull(orderid));
                cmd.Parameters.AddWithValue("@mastid", Utilities.ToDBNull(mastid));
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
        }
		
		
		public static products_track_number_fc GetById(int id)
		{
			products_track_number_fc result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT * FROM 2012_products_track_number_fc WHERE producttrack_id = @id", conn);
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
		
	
		public static products_track_number_fc GetFromDataReader(MySqlDataReader dr)
		{
			var o = new products_track_number_fc();
		
			o.producttrack_id =  (int) dr["producttrack_id"];
			o.mastid = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"mastid"));
			o.track_number = string.Empty + Utilities.GetReaderField(dr,"track_number");
			o.orderid = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"orderid"));
			o.producttrack_date = string.Empty + Utilities.GetReaderField(dr,"producttrack_date");
			
			return o;

		}
		
		
		public static void Create(products_track_number_fc o)
        {
            string insertsql = @"INSERT INTO 2012_products_track_number_fc (mastid,track_number,orderid,producttrack_date) VALUES(@mastid,@track_number,@orderid,@producttrack_date)";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				
				var cmd = new MySqlCommand(insertsql, conn);
                BuildSqlParameters(cmd,o);
                cmd.ExecuteNonQuery();
				cmd.CommandText = "SELECT producttrack_id FROM 2012_products_track_number_fc WHERE producttrack_id = LAST_INSERT_ID()";
                o.producttrack_id = (int) cmd.ExecuteScalar();
				
            }
		}
		
		private static void BuildSqlParameters(MySqlCommand cmd, products_track_number_fc o, bool forInsert = true)
        {
			
			if(!forInsert)
				cmd.Parameters.AddWithValue("@producttrack_id", o.producttrack_id);
			cmd.Parameters.AddWithValue("@mastid", o.mastid);
			cmd.Parameters.AddWithValue("@track_number", o.track_number);
			cmd.Parameters.AddWithValue("@orderid", o.orderid);
			cmd.Parameters.AddWithValue("@producttrack_date", o.producttrack_date);
		}
		
		public static void Update(products_track_number_fc o)
		{
			string updatesql = @"UPDATE 2012_products_track_number_fc SET mastid = @mastid,track_number = @track_number,orderid = @orderid,producttrack_date = @producttrack_date WHERE producttrack_id = @producttrack_id";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = new MySqlCommand(updatesql, conn);
                BuildSqlParameters(cmd,o, false);
                cmd.ExecuteNonQuery();
            }
		}
		
		public static void Delete(int producttrack_id)
		{
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = new MySqlCommand("DELETE FROM 2012_products_track_number_fc WHERE producttrack_id = @id" , conn);
                cmd.Parameters.AddWithValue("@id", producttrack_id);
                cmd.ExecuteNonQuery();
            }
		}
		
		
	}
}
			
			