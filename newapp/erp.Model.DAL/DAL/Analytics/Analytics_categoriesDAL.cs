
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;


namespace erp.Model.DAL
{
    public class Analytics_categoriesDAL
	{
	
		public static List<Analytics_categories> GetAll()
		{
			var result = new List<Analytics_categories>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM analytics_categories", conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}

        public static List<Analytics_categories> GetForBrand(int? brand_user_id = null)
        {
            var result = new List<Analytics_categories>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand(string.Format("SELECT * FROM analytics_categories WHERE category_type {0}", brand_user_id != null ? "= @user_id" : " IS NULL"), conn);
                if(brand_user_id != null)
                    cmd.Parameters.AddWithValue("@user_id", brand_user_id);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
        }
		
		public static Analytics_categories GetById(int id)
		{
			Analytics_categories result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM analytics_categories WHERE category_id = @id", conn);
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
		
	
		public static Analytics_categories GetFromDataReader(MySqlDataReader dr)
		{
			Analytics_categories o = new Analytics_categories();
		
			o.category_id =  (int) dr["category_id"];
			o.category_name = string.Empty + Utilities.GetReaderField(dr,"category_name");
			o.category_type = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"category_type"));
		    o.category_seq = Utilities.FromDbValue<int>(dr["category_seq"]);
			return o;

		}
		
		
		public static void Create(Analytics_categories o)
        {
            string insertsql = @"INSERT INTO analytics_categories (category_name,category_type) VALUES(@category_name,@category_type)";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				
				var cmd = Utils.GetCommand(insertsql, conn);
                BuildSqlParameters(cmd,o);
                cmd.ExecuteNonQuery();
				cmd.CommandText = "SELECT category_id FROM analytics_categories WHERE category_id = LAST_INSERT_ID()";
                o.category_id = (int) cmd.ExecuteScalar();
				
            }
		}
		
		private static void BuildSqlParameters(MySqlCommand cmd, Analytics_categories o, bool forInsert = true)
        {
			
			if(!forInsert)
				cmd.Parameters.AddWithValue("@category_id", o.category_id);
			cmd.Parameters.AddWithValue("@category_name", o.category_name);
			cmd.Parameters.AddWithValue("@category_type", o.category_type);
		}
		
		public static void Update(Analytics_categories o)
		{
			string updatesql = @"UPDATE analytics_categories SET category_name = @category_name,category_type = @category_type WHERE category_id = @category_id";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = Utils.GetCommand(updatesql, conn);
                BuildSqlParameters(cmd,o, false);
                cmd.ExecuteNonQuery();
            }
		}
		
		public static void Delete(int category_id)
		{
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = Utils.GetCommand("DELETE FROM analytics_categories WHERE category_id = @id" , conn);
                cmd.Parameters.AddWithValue("@id", category_id);
                cmd.ExecuteNonQuery();
            }
		}
		
		
	}
}
			
			