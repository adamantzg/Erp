
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace asaq2.Model
{
    public class Dealer_brandstatusDAL
	{
	
		public static List<Dealer_brandstatus> GetAll()
		{
			var result = new List<Dealer_brandstatus>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT * FROM dealer_brandstatus", conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}
		
		
		public static Dealer_brandstatus GetById(int brand_id, int dealer_id)
		{
			Dealer_brandstatus result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT * FROM dealer_brandstatus WHERE brand_id = @brand_id AND dealer_id = @dealer_id", conn);
				cmd.Parameters.AddWithValue("@brand_id", brand_id);
                cmd.Parameters.AddWithValue("@dealer_id", dealer_id);
                var dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    result = GetFromDataReader(dr);
                }
                dr.Close();
            }
			return result;
		}
		
	
		private static Dealer_brandstatus GetFromDataReader(MySqlDataReader dr)
		{
			Dealer_brandstatus o = new Dealer_brandstatus();
		
			o.dealer_id =  (int) dr["dealer_id"];
			o.brand_id =  (int) dr["brand_id"];
			o.brand_status = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"brand_status"));
			
			return o;

		}
		
		
		public static void Create(Dealer_brandstatus o)
        {
            string insertsql = @"INSERT INTO dealer_brandstatus (dealer_id,brand_id,brand_status) VALUES(@dealer_id,@brand_id,@brand_status)";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				
                var cmd = new MySqlCommand(insertsql, conn);
                
                BuildSqlParameters(cmd,o);
                cmd.ExecuteNonQuery();
				
            }
		}
		
		private static void BuildSqlParameters(MySqlCommand cmd, Dealer_brandstatus o, bool forInsert = true)
        {
			
			cmd.Parameters.AddWithValue("@dealer_id", o.dealer_id);
			cmd.Parameters.AddWithValue("@brand_id", o.brand_id);
			cmd.Parameters.AddWithValue("@brand_status", o.brand_status);
		}
		
		public static void Update(Dealer_brandstatus o)
		{
			string updatesql = @"UPDATE dealer_brandstatus SET brand_status = @brand_status WHERE brand_id = @brand_id AND dealer_id = @dealer_id";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = new MySqlCommand(updatesql, conn);
                BuildSqlParameters(cmd,o, false);
                cmd.ExecuteNonQuery();
            }
		}
		
		public static void Delete(int brand_id, int dealer_id)
		{
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = new MySqlCommand("DELETE FROM dealer_brandstatus WHERE brand_id = @brand_id AND dealer_id = @dealer_id" , conn);
                cmd.Parameters.AddWithValue("@brand_id", brand_id);
                cmd.Parameters.AddWithValue("@dealer_id", dealer_id);
                cmd.ExecuteNonQuery();
            }
		}
		
		
	}
}
			
			