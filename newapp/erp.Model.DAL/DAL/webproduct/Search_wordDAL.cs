
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using erp.Model;
using MySql.Data.MySqlClient;

namespace erp.Model.DAL
{
    public partial class Search_wordDAL
	{
	
		public static List<Search_word> GetAll()
		{
			var result = new List<Search_word>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM search_word", conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}
		
		
		public static Search_word GetById(int id)
		{
			Search_word result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM search_word WHERE id = @id", conn);
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
		
	
		public static Search_word GetFromDataReader(MySqlDataReader dr)
		{
			Search_word o = new Search_word();
		
			o.id =  (int) dr["id"];
			o.site_id = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"site_id"));
			o.word = string.Empty + Utilities.GetReaderField(dr,"word");
			o.group_id = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"group_id"));
			
			return o;

		}
		
		
		public static void Create(Search_word o)
        {
            string insertsql = @"INSERT INTO search_word (id,site_id,word,group_id) VALUES(@id,@site_id,@word,@group_id)";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				
                var cmd = Utils.GetCommand("SELECT MAX(id)+1 FROM search_word", conn);
                o.id = Convert.ToInt32(cmd.ExecuteScalar());
				cmd.CommandText = insertsql;
				
                BuildSqlParameters(cmd,o);
                cmd.ExecuteNonQuery();
				
            }
		}
		
		private static void BuildSqlParameters(MySqlCommand cmd, Search_word o, bool forInsert = true)
        {
			
			cmd.Parameters.AddWithValue("@id", o.id);
			cmd.Parameters.AddWithValue("@site_id", o.site_id);
			cmd.Parameters.AddWithValue("@word", o.word);
			cmd.Parameters.AddWithValue("@group_id", o.group_id);
		}
		
		public static void Update(Search_word o)
		{
			string updatesql = @"UPDATE search_word SET site_id = @site_id,word = @word,group_id = @group_id WHERE id = @id";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = Utils.GetCommand(updatesql, conn);
                BuildSqlParameters(cmd,o, false);
                cmd.ExecuteNonQuery();
            }
		}
		
		public static void Delete(int id)
		{
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = Utils.GetCommand("DELETE FROM search_word WHERE id = @id" , conn);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }
		}
		
		
	}
}
			
			