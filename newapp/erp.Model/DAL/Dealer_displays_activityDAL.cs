
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace asaq2.Model
{
    public class Dealer_displays_activityDAL
	{
	
		public static List<Dealer_displays_activity> GetAll()
		{
			List<Dealer_displays_activity> result = new List<Dealer_displays_activity>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM dealer_displays_activity", conn);
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}
		
		
		public static Dealer_displays_activity GetById(int id)
		{
			Dealer_displays_activity result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM dealer_displays_activity WHERE unique_id = @id", conn);
				cmd.Parameters.AddWithValue("@id", id);
                MySqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    result = GetFromDataReader(dr);
                }
                dr.Close();
            }
			return result;
		}
	
		private static Dealer_displays_activity GetFromDataReader(MySqlDataReader dr)
		{
			Dealer_displays_activity o = new Dealer_displays_activity();
		
			o.unique_id =  (int) dr["unique_id"];
			o.web_unique = Utilities.FromDbValue<int>(dr["web_unique"]);
			o.dealer_id = Utilities.FromDbValue<int>(dr["dealer_id"]);
			o.distributor_id = Utilities.FromDbValue<int>(dr["distributor_id"]);
			o.old_qty = Utilities.FromDbValue<int>(dr["old_qty"]);
			o.new_qty = Utilities.FromDbValue<int>(dr["new_qty"]);
			o.useruser_id = Utilities.FromDbValue<int>(dr["useruser_id"]);
			o.datecreated = Utilities.FromDbValue<DateTime>(dr["datecreated"]);
			
			return o;

		}
		
		public static void Create(Dealer_displays_activity o)
        {
            string insertsql = @"INSERT INTO dealer_displays_activity (web_unique,dealer_id,distributor_id,old_qty,new_qty,useruser_id,datecreated) VALUES(@web_unique,@dealer_id,@distributor_id,@old_qty,@new_qty,@useruser_id,@datecreated)";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				
				MySqlCommand cmd = new MySqlCommand(insertsql, conn);
                BuildSqlParameters(cmd,o);
                cmd.ExecuteNonQuery();
				cmd.CommandText = "SELECT unique_id FROM dealer_displays_activity WHERE unique_id = LAST_INSERT_ID()";
                o.unique_id = (int) cmd.ExecuteScalar();
				
            }
		}
		
		private static void BuildSqlParameters(MySqlCommand cmd, Dealer_displays_activity o, bool forInsert = true)
        {
			
			if(!forInsert)
				cmd.Parameters.AddWithValue("@unique_id", o.unique_id);
			cmd.Parameters.AddWithValue("@web_unique", o.web_unique);
			cmd.Parameters.AddWithValue("@dealer_id", o.dealer_id);
			cmd.Parameters.AddWithValue("@distributor_id", o.distributor_id);
			cmd.Parameters.AddWithValue("@old_qty", o.old_qty);
			cmd.Parameters.AddWithValue("@new_qty", o.new_qty);
			cmd.Parameters.AddWithValue("@useruser_id", o.useruser_id);
			cmd.Parameters.AddWithValue("@datecreated", o.datecreated);
		}
		
		public static void Update(Dealer_displays_activity o)
		{
			string updatesql = @"UPDATE dealer_displays_activity SET web_unique = @web_unique,dealer_id = @dealer_id,distributor_id = @distributor_id,old_qty = @old_qty,new_qty = @new_qty,useruser_id = @useruser_id,datecreated = @datecreated WHERE unique_id = @unique_id";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				MySqlCommand cmd = new MySqlCommand(updatesql, conn);
                BuildSqlParameters(cmd,o, false);
                cmd.ExecuteNonQuery();
            }
		}
		
		public static void Delete(int unique_id)
		{
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				MySqlCommand cmd = new MySqlCommand("DELETE FROM dealer_displays_activity WHERE unique_id = @id" , conn);
                cmd.Parameters.AddWithValue("@id", unique_id);
                cmd.ExecuteNonQuery();
            }
		}
	}
}
			
			