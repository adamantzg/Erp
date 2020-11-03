
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using erp.Model;
using MySql.Data.MySqlClient;

namespace erp.Model.DAL
{
    public class Survey_uitypeDAL
	{
	
		public static List<Survey_uitype> GetAll()
		{
			var result = new List<Survey_uitype>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM survey_uitype", conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}
		
		
		public static Survey_uitype GetById(int id)
		{
			Survey_uitype result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM survey_uitype WHERE uitype_id = @id", conn);
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
		
	
		public static Survey_uitype GetFromDataReader(MySqlDataReader dr)
		{
			Survey_uitype o = new Survey_uitype();
		
			o.uitype_id =  (int) dr["uitype_id"];
			o.name = string.Empty + Utilities.GetReaderField(dr,"name");
			
			return o;

		}
		
		
		public static void Create(Survey_uitype o)
        {
            string insertsql = @"INSERT INTO survey_uitype (uitype_id,name) VALUES(@uitype_id,@name)";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				
                var cmd = Utils.GetCommand("SELECT MAX(uitype_id)+1 FROM survey_uitype", conn);
                o.uitype_id = Convert.ToInt32(cmd.ExecuteScalar());
				cmd.CommandText = insertsql;
				
                BuildSqlParameters(cmd,o);
                cmd.ExecuteNonQuery();
				
            }
		}
		
		private static void BuildSqlParameters(MySqlCommand cmd, Survey_uitype o, bool forInsert = true)
        {
			
			cmd.Parameters.AddWithValue("@uitype_id", o.uitype_id);
			cmd.Parameters.AddWithValue("@name", o.name);
		}
		
		public static void Update(Survey_uitype o)
		{
			string updatesql = @"UPDATE survey_uitype SET name = @name WHERE uitype_id = @uitype_id";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = Utils.GetCommand(updatesql, conn);
                BuildSqlParameters(cmd,o, false);
                cmd.ExecuteNonQuery();
            }
		}
		
		public static void Delete(int uitype_id)
		{
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = Utils.GetCommand("DELETE FROM survey_uitype WHERE uitype_id = @id" , conn);
                cmd.Parameters.AddWithValue("@id", uitype_id);
                cmd.ExecuteNonQuery();
            }
		}
		
		
	}
}
			
			