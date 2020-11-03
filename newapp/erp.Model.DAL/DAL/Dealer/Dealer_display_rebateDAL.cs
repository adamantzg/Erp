
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
    public partial class Dealer_display_rebateDAL
	{
	
		public static List<Dealer_display_rebate> GetAll()
		{
			var result = new List<Dealer_display_rebate>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT * FROM dealer_display_rebate", conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}

        public static List<Dealer_display_rebate> GetByDealer(int dealer_id, IDbConnection conn = null)
        {
            var result = new List<Dealer_display_rebate>();
            var dispose = conn == null;
            if (conn == null)
            {
                conn = new MySqlConnection(Properties.Settings.Default.ConnString);
                conn.Open();
            }
            
            var cmd = new MySqlCommand("SELECT * FROM dealer_display_rebate WHERE dealer_id = @dealer_id", (MySqlConnection) conn);
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
		
		public static Dealer_display_rebate GetById(int id)
		{
			Dealer_display_rebate result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT * FROM dealer_display_rebate WHERE unique_id = @id", conn);
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
		
	
		public static Dealer_display_rebate GetFromDataReader(MySqlDataReader dr)
		{
			Dealer_display_rebate o = new Dealer_display_rebate();
		
			o.unique_id =  (int) dr["unique_id"];
			o.dealer_id =  (int) dr["dealer_id"];
			o.value = Utilities.FromDbValue<double>(Utilities.GetReaderField(dr,"value"));
			o.date_created = Utilities.FromDbValue<DateTime>(Utilities.GetReaderField(dr,"date_created"));
			o.useruserid = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"useruserid"));
			o.type = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"type"));
            o.brand_id = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "brand_id"));
            o.reference = string.Empty + dr["reference"];
			
			return o;

		}
		
		
		public static void Create(Dealer_display_rebate o, IDbTransaction tr = null)
        {
            const string insertsql = @"INSERT INTO dealer_display_rebate (dealer_id,value,date_created,useruserid,type, brand_id, reference) VALUES(@dealer_id,@value,@date_created,@useruserid,@type, @brand_id, @reference)";
		    var dispose = tr == null;
		    var conn = tr == null ? new MySqlConnection(Properties.Settings.Default.ConnString) : tr.Connection;
            if(tr == null)
                conn.Open();
			
			var cmd = new MySqlCommand(insertsql,(MySqlConnection) conn,(MySqlTransaction) tr);
            BuildSqlParameters(cmd,o);
            cmd.ExecuteNonQuery();
			cmd.CommandText = "SELECT unique_id FROM dealer_display_rebate WHERE unique_id = LAST_INSERT_ID()";
            o.unique_id = (int) cmd.ExecuteScalar();
				
            if(dispose)
                conn.Dispose();
		}
		
		private static void BuildSqlParameters(MySqlCommand cmd, Dealer_display_rebate o, bool forInsert = true)
        {
			
			if(!forInsert)
				cmd.Parameters.AddWithValue("@unique_id", o.unique_id);
			cmd.Parameters.AddWithValue("@dealer_id", o.dealer_id);
			cmd.Parameters.AddWithValue("@value", o.value);
			cmd.Parameters.AddWithValue("@date_created", o.date_created);
			cmd.Parameters.AddWithValue("@useruserid", o.useruserid);
			cmd.Parameters.AddWithValue("@type", o.type);
            cmd.Parameters.AddWithValue("@brand_id", o.brand_id);
            cmd.Parameters.AddWithValue("@reference", o.reference);
		}
		
		public static void Update(Dealer_display_rebate o)
		{
			string updatesql = @"UPDATE dealer_display_rebate SET dealer_id = @dealer_id,value = @value,date_created = @date_created,useruserid = @useruserid,type = @type,brand_id = @brand_id,reference = @reference WHERE unique_id = @unique_id";

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
				var cmd = new MySqlCommand("DELETE FROM dealer_display_rebate WHERE unique_id = @id" , conn);
                cmd.Parameters.AddWithValue("@id", unique_id);
                cmd.ExecuteNonQuery();
            }
		}
		
		
	}
}
			
			