
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using erp.Model;
using MySql.Data.MySqlClient;

namespace erp.Model.DAL
{
    public partial class Web_product_info_translateDAL
	{
	
		public static List<Web_product_info_translate> GetAll()
		{
			var result = new List<Web_product_info_translate>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM web_product_info_translate", conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}
		
		
		public static Web_product_info_translate GetById(int id)
		{
			Web_product_info_translate result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM web_product_info_translate WHERE language_id = @id", conn);
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
		
	
		public static Web_product_info_translate GetFromDataReader(MySqlDataReader dr)
		{
			Web_product_info_translate o = new Web_product_info_translate();
		
			o.info_id =  (int) dr["info_id"];
			o.language_id =  (int) dr["language_id"];
			o.value = string.Empty + Utilities.GetReaderField(dr,"value");
			
			return o;

		}
		
		
		public static void Create(Web_product_info_translate o)
        {
            string insertsql = @"INSERT INTO web_product_info_translate (info_id,language_id,value) VALUES(@info_id,@language_id,@value)";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				
                var cmd = Utils.GetCommand(insertsql, conn);
                
                BuildSqlParameters(cmd,o);
                cmd.ExecuteNonQuery();
				
            }
		}
		
		private static void BuildSqlParameters(MySqlCommand cmd, Web_product_info_translate o, bool forInsert = true)
        {
			
			cmd.Parameters.AddWithValue("@info_id", o.info_id);
			cmd.Parameters.AddWithValue("@language_id", o.language_id);
			cmd.Parameters.AddWithValue("@value", o.value);
		}
		
		public static void Update(Web_product_info_translate o)
		{
			string updatesql = @"UPDATE web_product_info_translate SET value = @value WHERE language_id = @language_id";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = Utils.GetCommand(updatesql, conn);
                BuildSqlParameters(cmd,o, false);
                cmd.ExecuteNonQuery();
            }
		}
		
		public static void Delete(int language_id)
		{
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = Utils.GetCommand("DELETE FROM web_product_info_translate WHERE language_id = @id" , conn);
                cmd.Parameters.AddWithValue("@id", language_id);
                cmd.ExecuteNonQuery();
            }
		}


        public static List<Web_product_info_translate> GetByProduct(int web_unique)
        {
            var result = new List<Web_product_info_translate>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand(@"SELECT * FROM web_product_info_translate INNER JOIN web_product_info ON web_product_info_translate.info_id = web_product_info.id 
                                            WHERE web_unique = @id", conn);
                cmd.Parameters.AddWithValue("@id", web_unique);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
        }
	}
}
			
			