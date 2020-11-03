
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace asaq2.Model
{
    public class Category1DAL
	{
	
		public static List<Category1> GetAll()
		{
			var result = new List<Category1>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT * FROM category1", conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}
		
		
		public static Category1 GetById(int id)
		{
			Category1 result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT * FROM category1 WHERE category1_id = @id", conn);
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
		
	
		public static Category1 GetFromDataReader(MySqlDataReader dr)
		{
			Category1 o = new Category1();
		
			o.category1_id =  (int) dr["category1_id"];
			o.cat1_name = string.Empty + Utilities.GetReaderField(dr,"cat1_name");
			o.cat1_duty = Utilities.FromDbValue<double>(Utilities.GetReaderField(dr,"cat1_duty"));
			o.cat1_margin = Utilities.FromDbValue<double>(Utilities.GetReaderField(dr,"cat1_margin"));
            
			
			return o;

		}
		
		
		public static void Create(Category1 o)
        {
            string insertsql = @"INSERT INTO category1 (cat1_name,cat1_duty,cat1_margin) VALUES(@cat1_name,@cat1_duty,@cat1_margin)";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				
				var cmd = new MySqlCommand(insertsql, conn);
                BuildSqlParameters(cmd,o);
                cmd.ExecuteNonQuery();
				cmd.CommandText = "SELECT category1_id FROM category1 WHERE category1_id = LAST_INSERT_ID()";
                o.category1_id = (int) cmd.ExecuteScalar();
				
            }
		}
		
		private static void BuildSqlParameters(MySqlCommand cmd, Category1 o, bool forInsert = true)
        {
			
			if(!forInsert)
				cmd.Parameters.AddWithValue("@category1_id", o.category1_id);
			cmd.Parameters.AddWithValue("@cat1_name", o.cat1_name);
			cmd.Parameters.AddWithValue("@cat1_duty", o.cat1_duty);
			cmd.Parameters.AddWithValue("@cat1_margin", o.cat1_margin);
		}
		
		public static void Update(Category1 o)
		{
			string updatesql = @"UPDATE category1 SET cat1_name = @cat1_name,cat1_duty = @cat1_duty,cat1_margin = @cat1_margin WHERE category1_id = @category1_id";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = new MySqlCommand(updatesql, conn);
                BuildSqlParameters(cmd,o, false);
                cmd.ExecuteNonQuery();
            }
		}
		
		public static void Delete(int category1_id)
		{
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = new MySqlCommand("DELETE FROM category1 WHERE category1_id = @id" , conn);
                cmd.Parameters.AddWithValue("@id", category1_id);
                cmd.ExecuteNonQuery();
            }
		}
		
		
	}
}
			
			