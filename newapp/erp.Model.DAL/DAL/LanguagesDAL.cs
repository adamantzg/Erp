
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using erp.Model;
using MySql.Data.MySqlClient;

namespace erp.Model.DAL
{
    public partial class LanguagesDAL
	{
	
		public static List<Language> GetAll()
		{
			var result = new List<Language>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM languages", conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}
		
		
		public static Language GetById(int id)
		{
			Language result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM languages WHERE language_id = @id", conn);
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

        public static Language GetByCode(string code)
        {
            Language result = null;
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM languages WHERE code = @code", conn);
                cmd.Parameters.AddWithValue("@code", code);
                var dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    result = GetFromDataReader(dr);
                }
                dr.Close();
            }
            return result;
        }
		
	
		public static Language GetFromDataReader(MySqlDataReader dr)
		{
			Language o = new Language();
		
			o.language_id =  (int) dr["language_id"];
			o.code = string.Empty + Utilities.GetReaderField(dr,"code");
			o.name = string.Empty + Utilities.GetReaderField(dr,"name");
			
			return o;

		}
		
		
		public static void Create(Language o)
        {
            string insertsql = @"INSERT INTO languages (language_id,code,name) VALUES(@language_id,@code,@name)";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				
                var cmd = Utils.GetCommand("SELECT MAX(language_id)+1 FROM languages", conn);
                o.language_id = Convert.ToInt32(cmd.ExecuteScalar());
				cmd.CommandText = insertsql;
				
                BuildSqlParameters(cmd,o);
                cmd.ExecuteNonQuery();
				
            }
		}
		
		private static void BuildSqlParameters(MySqlCommand cmd, Language o, bool forInsert = true)
        {
			
			cmd.Parameters.AddWithValue("@language_id", o.language_id);
			cmd.Parameters.AddWithValue("@code", o.code);
			cmd.Parameters.AddWithValue("@name", o.name);
		}
		
		public static void Update(Language o)
		{
			string updatesql = @"UPDATE languages SET code = @code,name = @name WHERE language_id = @language_id";

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
				var cmd = Utils.GetCommand("DELETE FROM languages WHERE language_id = @id" , conn);
                cmd.Parameters.AddWithValue("@id", language_id);
                cmd.ExecuteNonQuery();
            }
		}
		
		
	}
}
			
			