
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace asaq2.Model
{
    public class Cust_products_translateDAL
	{
	
		public static List<Cust_products_translate> GetAll()
		{
			var result = new List<Cust_products_translate>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT * FROM cust_products_translate", conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}
		
		
		public static Cust_products_translate GetById(int id, string lang)
		{
			Cust_products_translate result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT * FROM cust_products_translate WHERE cprod_id = @id AND lang = @lang", conn);
				cmd.Parameters.AddWithValue("@id", id);
                cmd.Parameters.AddWithValue("@lang", lang);
                var dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    result = GetFromDataReader(dr);
                }
                dr.Close();
            }
			return result;
		}
		
	
		private static Cust_products_translate GetFromDataReader(MySqlDataReader dr)
		{
			Cust_products_translate o = new Cust_products_translate();
		
			o.cprod_id =  (int) dr["cprod_id"];
			o.lang = string.Empty + Utilities.GetReaderField(dr,"lang");
			o.cprod_name = string.Empty + Utilities.GetReaderField(dr,"cprod_name");
			
			return o;

		}
		
		
		public static void Create(Cust_products_translate o)
        {
            string insertsql = @"INSERT INTO cust_products_translate (cprod_id,lang,cprod_name) VALUES(@cprod_id,@lang,@cprod_name)";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				
				MySqlCommand cmd = new MySqlCommand(insertsql, conn);
                BuildSqlParameters(cmd,o);
                cmd.ExecuteNonQuery();
            }
		}
		
		private static void BuildSqlParameters(MySqlCommand cmd, Cust_products_translate o, bool forInsert = true)
        {
            cmd.Parameters.AddWithValue("@cprod_id", o.cprod_id);
			cmd.Parameters.AddWithValue("@lang", o.lang);
			cmd.Parameters.AddWithValue("@cprod_name", o.cprod_name);
		}
		
		public static void Update(Cust_products_translate o)
		{
			string updatesql = @"UPDATE cust_products_translate SET cprod_name = @cprod_name WHERE cprod_id = @cprod_id AND lang = @lang";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				MySqlCommand cmd = new MySqlCommand(updatesql, conn);
                BuildSqlParameters(cmd,o, false);
                cmd.ExecuteNonQuery();
            }
		}
		
		public static void Delete(int id, string lang)
		{
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = new MySqlCommand("DELETE FROM cust_products_translate WHERE cprod_id = @id AND lang = @lang" , conn);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.Parameters.AddWithValue("@lang", lang);
                cmd.ExecuteNonQuery();
            }
		}
		
		
	}
}
			
			