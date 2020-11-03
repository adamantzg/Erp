
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using erp.Model;
using MySql.Data.MySqlClient;

namespace erp.Model.DAL
{
    public class Survey_optionset_optionDAL
	{
	
		public static List<Survey_optionset_option> GetAll()
		{
			var result = new List<Survey_optionset_option>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM survey_optionset_option", conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}

        public static List<Survey_optionset_option> GetForSet(int optionset_id)
        {
            var result = new List<Survey_optionset_option>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM survey_optionset_option WHERE optionset_id = @optionset_id", conn);
                cmd.Parameters.AddWithValue("@optionset_id", optionset_id);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
        }
		
		
		public static Survey_optionset_option GetById(int id)
		{
			Survey_optionset_option result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM survey_optionset_option WHERE option_id = @id", conn);
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
		
	
		public static Survey_optionset_option GetFromDataReader(MySqlDataReader dr)
		{
			Survey_optionset_option o = new Survey_optionset_option();
		
			o.option_id =  (int) dr["option_id"];
			o.optionset_id =  (int) dr["optionset_id"];
			o.text = string.Empty + Utilities.GetReaderField(dr,"text");
			o.value =  (int) dr["value"];
			
			return o;

		}
		
		
		public static void Create(Survey_optionset_option o)
        {
            string insertsql = @"INSERT INTO survey_optionset_option (optionset_id,text,value) VALUES(@optionset_id,@text,@value)";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				
				var cmd = Utils.GetCommand(insertsql, conn);
                BuildSqlParameters(cmd,o);
                cmd.ExecuteNonQuery();
				cmd.CommandText = "SELECT option_id FROM survey_optionset_option WHERE option_id = LAST_INSERT_ID()";
                o.option_id = (int) cmd.ExecuteScalar();
				
            }
		}
		
		private static void BuildSqlParameters(MySqlCommand cmd, Survey_optionset_option o, bool forInsert = true)
        {
			
			if(!forInsert)
				cmd.Parameters.AddWithValue("@option_id", o.option_id);
			cmd.Parameters.AddWithValue("@optionset_id", o.optionset_id);
			cmd.Parameters.AddWithValue("@text", o.text);
			cmd.Parameters.AddWithValue("@value", o.value);
		}
		
		public static void Update(Survey_optionset_option o)
		{
			string updatesql = @"UPDATE survey_optionset_option SET optionset_id = @optionset_id,text = @text,value = @value WHERE option_id = @option_id";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = Utils.GetCommand(updatesql, conn);
                BuildSqlParameters(cmd,o, false);
                cmd.ExecuteNonQuery();
            }
		}
		
		public static void Delete(int option_id)
		{
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = Utils.GetCommand("DELETE FROM survey_optionset_option WHERE option_id = @id" , conn);
                cmd.Parameters.AddWithValue("@id", option_id);
                cmd.ExecuteNonQuery();
            }
		}
		
		
	}
}
			
			