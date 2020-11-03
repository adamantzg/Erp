
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace asaq2.Model
{
    public class Npd_filetypeDAL
	{
	
		public static List<Npd_filetype> GetAll()
		{
			var result = new List<Npd_filetype>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT * FROM npd_filetype", conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}
		
		
		public static Npd_filetype GetById(int id)
		{
			Npd_filetype result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT * FROM npd_filetype WHERE type_id = @id", conn);
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
		
	
		private static Npd_filetype GetFromDataReader(MySqlDataReader dr)
		{
			Npd_filetype o = new Npd_filetype();
		
			o.type_id =  (int) dr["type_id"];
			o.name = string.Empty + Utilities.GetReaderField(dr,"name");
			o.extensions = string.Empty + Utilities.GetReaderField(dr,"extensions");
			
			return o;

		}
		
		
		public static void Create(Npd_filetype o)
        {
            string insertsql = @"INSERT INTO npd_filetype (name,extensions) VALUES(@name,@extensions)";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				
				var cmd = new MySqlCommand(insertsql, conn);
                BuildSqlParameters(cmd,o);
                cmd.ExecuteNonQuery();
				cmd.CommandText = "SELECT type_id FROM npd_filetype WHERE type_id = LAST_INSERT_ID()";
                o.type_id = (int) cmd.ExecuteScalar();
				
            }
		}
		
		private static void BuildSqlParameters(MySqlCommand cmd, Npd_filetype o, bool forInsert = true)
        {
			
			if(!forInsert)
				cmd.Parameters.AddWithValue("@type_id", o.type_id);
			cmd.Parameters.AddWithValue("@name", o.name);
			cmd.Parameters.AddWithValue("@extensions", o.extensions);
		}
		
		public static void Update(Npd_filetype o)
		{
			string updatesql = @"UPDATE npd_filetype SET name = @name,extensions = @extensions WHERE type_id = @type_id";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = new MySqlCommand(updatesql, conn);
                BuildSqlParameters(cmd,o, false);
                cmd.ExecuteNonQuery();
            }
		}
		
		public static void Delete(int type_id)
		{
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = new MySqlCommand("DELETE FROM npd_filetype WHERE type_id = @id" , conn);
                cmd.Parameters.AddWithValue("@id", type_id);
                cmd.ExecuteNonQuery();
            }
		}
		
		
	}
}
			
			