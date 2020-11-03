
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using erp.Model;
using MySql.Data.MySqlClient;

namespace erp.Model.DAL
{
    public class Survey_optionsetDAL
	{
	
		public static List<Survey_optionset> GetAll()
		{
			var result = new List<Survey_optionset>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM survey_optionset", conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}
		
		
		public static Survey_optionset GetById(int id)
		{
			Survey_optionset result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM survey_optionset WHERE option_set_id = @id", conn);
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
		
	
		public static Survey_optionset GetFromDataReader(MySqlDataReader dr)
		{
			Survey_optionset o = new Survey_optionset();
		
			o.option_set_id =  (int) dr["option_set_id"];
			o.name = string.Empty + Utilities.GetReaderField(dr,"name");
			
			return o;

		}
		
		
		public static void Create(Survey_optionset o)
        {
            string insertsql = @"INSERT INTO survey_optionset (option_set_id,name) VALUES(@option_set_id,@name)";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				
                var cmd = Utils.GetCommand("SELECT MAX(option_set_id)+1 FROM survey_optionset", conn);
                o.option_set_id = Convert.ToInt32(cmd.ExecuteScalar());
				cmd.CommandText = insertsql;
				
                BuildSqlParameters(cmd,o);
                cmd.ExecuteNonQuery();
				
            }
		}
		
		private static void BuildSqlParameters(MySqlCommand cmd, Survey_optionset o, bool forInsert = true)
        {
			
			cmd.Parameters.AddWithValue("@option_set_id", o.option_set_id);
			cmd.Parameters.AddWithValue("@name", o.name);
		}
		
		public static void Update(Survey_optionset o)
		{
			string updatesql = @"UPDATE survey_optionset SET name = @name WHERE option_set_id = @option_set_id";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = Utils.GetCommand(updatesql, conn);
                BuildSqlParameters(cmd,o, false);
                cmd.ExecuteNonQuery();
            }
		}
		
		public static void Delete(int option_set_id)
		{
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = Utils.GetCommand("DELETE FROM survey_optionset WHERE option_set_id = @id" , conn);
                cmd.Parameters.AddWithValue("@id", option_set_id);
                cmd.ExecuteNonQuery();
            }
		}
		
		
	}
}
			
			