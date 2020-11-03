
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace erp.Model.DAL
{
    public class Return_categoryDAL
	{
	
		public static List<Return_category> GetAll()
		{
			var result = new List<Return_category>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM return_category", conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}
		
		
		public static Return_category GetById(int id)
		{
			Return_category result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM return_category WHERE returncategory_id = @id", conn);
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
		
	
		private static Return_category GetFromDataReader(MySqlDataReader dr)
		{
			Return_category o = new Return_category();
		
			o.returncategory_id =  (int) dr["returncategory_id"];
			o.category_name = string.Empty + Utilities.GetReaderField(dr,"category_name");
			o.category_code = string.Empty + Utilities.GetReaderField(dr,"category_code");
			o.inspection_full_check = Utilities.BoolFromLong(dr["inspection_full_check"]);
			return o;

		}
		
		
		public static void Create(Return_category o)
        {
            string insertsql = @"INSERT INTO return_category (returncategory_id,category_name,category_code) VALUES(@returncategory_id,@category_name,@category_code)";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				
                MySqlCommand cmd = Utils.GetCommand("SELECT MAX(returncategory_id)+1 FROM return_category", conn);
                o.returncategory_id = Convert.ToInt32(cmd.ExecuteScalar());
				cmd.CommandText = insertsql;
				
                BuildSqlParameters(cmd,o);
                cmd.ExecuteNonQuery();
				
            }
		}
		
		private static void BuildSqlParameters(MySqlCommand cmd, Return_category o, bool forInsert = true)
        {
			
			cmd.Parameters.AddWithValue("@returncategory_id", o.returncategory_id);
			cmd.Parameters.AddWithValue("@category_name", o.category_name);
			cmd.Parameters.AddWithValue("@category_code", o.category_code);
		}
		
		public static void Update(Return_category o)
		{
			string updatesql = @"UPDATE return_category SET category_name = @category_name,category_code = @category_code WHERE returncategory_id = @returncategory_id";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				MySqlCommand cmd = Utils.GetCommand(updatesql, conn);
                BuildSqlParameters(cmd,o, false);
                cmd.ExecuteNonQuery();
            }
		}
		
		public static void Delete(int returncategory_id)
		{
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = Utils.GetCommand("DELETE FROM return_category WHERE returncategory_id = @id" , conn);
                cmd.Parameters.AddWithValue("@id", returncategory_id);
                cmd.ExecuteNonQuery();
            }
		}
		
		
	}
}
			
			