
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using erp.Model;
using MySql.Data.MySqlClient;

namespace erp.Model.DAL
{
    public partial class Web_category_translateDAL
	{
	
		public static List<Web_category_translate> GetAll()
		{
			var result = new List<Web_category_translate>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM web_category_translate", conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}
		
		
		public static Web_category_translate GetById(int id)
		{
			Web_category_translate result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM web_category_translate WHERE language_id = @id", conn);
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
		
	
		public static Web_category_translate GetFromDataReader(MySqlDataReader dr)
		{
			Web_category_translate o = new Web_category_translate();
		
			o.category_id =  (int) dr["category_id"];
			o.language_id =  (int) dr["language_id"];
			o.name = string.Empty + Utilities.GetReaderField(dr,"name");
			o.alternate_name = string.Empty + Utilities.GetReaderField(dr,"alternate_name");
			o.path = string.Empty + Utilities.GetReaderField(dr,"path");
			o.title = string.Empty + Utilities.GetReaderField(dr,"title");
			o.description = string.Empty + Utilities.GetReaderField(dr,"description");
			o.pricing_note = string.Empty + Utilities.GetReaderField(dr,"pricing_note");
			o.group = string.Empty + Utilities.GetReaderField(dr,"group");
			
			return o;

		}
		
		
		public static void Create(Web_category_translate o)
        {
            string insertsql = @"INSERT INTO web_category_translate (category_id,language_id,name,alternate_name,path,title,description,pricing_note,group) VALUES(@category_id,@language_id,@name,@alternate_name,@path,@title,@description,@pricing_note,@group)";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				
                var cmd = Utils.GetCommand(insertsql, conn);
                BuildSqlParameters(cmd,o);
                cmd.ExecuteNonQuery();
				
            }
		}
		
		private static void BuildSqlParameters(MySqlCommand cmd, Web_category_translate o, bool forInsert = true)
        {
			
			cmd.Parameters.AddWithValue("@category_id", o.category_id);
			cmd.Parameters.AddWithValue("@language_id", o.language_id);
			cmd.Parameters.AddWithValue("@name", o.name);
			cmd.Parameters.AddWithValue("@alternate_name", o.alternate_name);
			cmd.Parameters.AddWithValue("@path", o.path);
			cmd.Parameters.AddWithValue("@title", o.title);
			cmd.Parameters.AddWithValue("@description", o.description);
			cmd.Parameters.AddWithValue("@pricing_note", o.pricing_note);
			cmd.Parameters.AddWithValue("@group", o.group);
		}
		
		public static void Update(Web_category_translate o)
		{
			string updatesql = @"UPDATE web_category_translate SET name = @name,alternate_name = @alternate_name,path = @path,title = @title,description = @description,pricing_note = @pricing_note,group = @group WHERE language_id = @language_id";

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
				var cmd = Utils.GetCommand("DELETE FROM web_category_translate WHERE language_id = @id" , conn);
                cmd.Parameters.AddWithValue("@id", language_id);
                cmd.ExecuteNonQuery();
            }
		}
		
		
	}
}
			
			