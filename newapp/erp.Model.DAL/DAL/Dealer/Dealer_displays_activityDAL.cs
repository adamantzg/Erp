
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace erp.Model.DAL
{
    public class Dealer_displays_activityDAL
	{
	
		public static List<Dealer_displays_activity> GetAll()
		{
			List<Dealer_displays_activity> result = new List<Dealer_displays_activity>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = Utils.GetCommand("SELECT * FROM dealer_displays_activity", conn);
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
                MySqlCommand cmd = Utils.GetCommand("SELECT * FROM dealer_displays_activity WHERE unique_id = @id", conn);
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

        public static List<Dealer_displays_activity> GetByCriteria(int? dealer_id = null, int? web_unique = null)
        {
            var result = new List<Dealer_displays_activity>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand(@"SELECT * FROM dealer_displays_activity 
                        WHERE (dealer_id = @dealer_id OR @dealer_id IS NULL) AND (web_unique = @web_unique OR @web_unique IS NULL)", conn);
                cmd.Parameters.AddWithValue("@dealer_id", dealer_id);
                cmd.Parameters.AddWithValue("@web_unique", web_unique);   
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
        }

        public static List<Dealer_displays_activity> GetByDealerAndProduct(int dealer_id, int web_unique)
        {
            var result = new List<Dealer_displays_activity>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand(@"SELECT * FROM dealer_displays_activity 
                        WHERE dealer_id = @dealer_id AND web_unique = @web_unique", conn);
                cmd.Parameters.AddWithValue("@dealer_id", dealer_id);
                cmd.Parameters.AddWithValue("@web_unique", web_unique);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
        }

        public static List<Dealer_displays_activity_bydateandbrand> GetGroupedByDateAndBrand(int dealer_id)
        {
            var result = new List<Dealer_displays_activity_bydateandbrand>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand(@"SELECT CONCAT(MONTHNAME(dda.datecreated),'-',YEAR(dda.datecreated)) as date , brands.brand_id,
                                            (dda.new_qty - dda.old_qty) AS qty, dda.web_unique
                                            FROM dealer_displays_activity  AS dda
                                            INNER JOIN web_product_new AS wpn ON wpn.web_unique = dda.web_unique
                                            INNER JOIN brands ON brands.brand_id = wpn.web_site_id
                                            INNER JOIN dealer_image_displays AS did ON did.web_unique = dda.web_unique
                                            INNER JOIN dealer_images AS di ON di.image_unique = did.image_id
                                            WHERE (dda.dealer_id = @dealer_id OR @dealer_id IS NULL) 
                                            AND brand_id IN (1,2,4,11)
                                            GROUP BY date, brands.brand_id", conn);
                cmd.Parameters.AddWithValue("@dealer_id", dealer_id);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    var ddab = new Dealer_displays_activity_bydateandbrand();
                    ddab.date = string.Empty + (string)dr["date"];
                    ddab.brand_id = (int)dr["brand_id"];
                    ddab.qty = dr["qty"] != DBNull.Value ? Utilities.FromDbValue<int>(dr["qty"]).Value : 0;
                    ddab.web_unique = (int)dr["web_unique"];

                    result.Add(ddab);
                }
                dr.Close();
            }
            return result;
        }

        public static List<Dealer_displays_activitiy_bydateandbrand_deatils> GetByDateAndBrand(int dealer_id, int brand_id, DateTime date)
        {
            var result = new List<Dealer_displays_activitiy_bydateandbrand_deatils>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand(@"SELECT web_product_new.web_code, web_product_new.web_name, web_product_new.web_unique,dealer_displays_activity.new_qty FROM dealer_displays_activity 
                                            INNER JOIN web_product_new ON web_product_new.web_unique = dealer_displays_activity.web_unique
                                            INNER JOIN brands ON brands.brand_id = web_product_new.web_site_id
                                            WHERE (dealer_id = @dealer_id OR @dealer_id IS NULL) AND brands.brand_id = @brand_id AND (Month(dealer_displays_activity.datecreated) = Month(@date) AND Year(dealer_displays_activity.datecreated) = Year(@date))", conn);
                cmd.Parameters.AddWithValue("@dealer_id", dealer_id);
                cmd.Parameters.AddWithValue("@brand_id", brand_id);
                cmd.Parameters.AddWithValue("@date", date);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    var ddab2 = new Dealer_displays_activitiy_bydateandbrand_deatils();
                    ddab2.web_code = string.Empty + (string)dr["web_code"];
                    ddab2.web_name = string.Empty + dr["web_name"];
                    ddab2.web_unique = (int)dr["web_unique"];
                    ddab2.qty = dr["new_qty"] != DBNull.Value ? Utilities.FromDbValue<int>(dr["new_qty"]).Value : 0;

                    result.Add(ddab2);
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
		
		public static void Create(Dealer_displays_activity o, IDbTransaction tr = null)
        {
            string insertsql = @"INSERT INTO dealer_displays_activity (web_unique,dealer_id,distributor_id,old_qty,new_qty,useruser_id,datecreated) VALUES(@web_unique,@dealer_id,@distributor_id,@old_qty,@new_qty,@useruser_id,@datecreated)";

            var conn = tr != null ? (MySqlConnection)tr.Connection : new MySqlConnection(Properties.Settings.Default.ConnString);
            if (tr == null)
                conn.Open();

            try
            {
                MySqlCommand cmd = Utils.GetCommand(insertsql, (MySqlConnection) conn, (MySqlTransaction) tr);
                BuildSqlParameters(cmd, o);
                cmd.ExecuteNonQuery();
                cmd.CommandText = "SELECT unique_id FROM dealer_displays_activity WHERE unique_id = LAST_INSERT_ID()";
                o.unique_id = (int) cmd.ExecuteScalar();

            }
            finally
            {
                if(tr == null)
                    conn.Dispose();
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
				MySqlCommand cmd = Utils.GetCommand(updatesql, conn);
                BuildSqlParameters(cmd,o, false);
                cmd.ExecuteNonQuery();
            }
		}
		
		public static void Delete(int unique_id)
		{
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				MySqlCommand cmd = Utils.GetCommand("DELETE FROM dealer_displays_activity WHERE unique_id = @id" , conn);
                cmd.Parameters.AddWithValue("@id", unique_id);
                cmd.ExecuteNonQuery();
            }
		}
	}
}
			
			