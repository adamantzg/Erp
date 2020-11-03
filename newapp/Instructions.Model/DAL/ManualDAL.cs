
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace Instructions.Model
{
    public class ManualDAL
	{
	
		public static List<Manual> GetAll()
		{
			var result = new List<Manual>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utilities.GetCommand("SELECT * FROM manual", conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}
		
		
		public static Manual GetById(int id)
		{
			Manual result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utilities.GetCommand("SELECT * FROM manual WHERE manual_id = @id", conn);
				cmd.Parameters.AddWithValue("@id", id);
                var dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    result = GetFromDataReader(dr);
                    result.Chapters = ChapterDAL.GetForManual(result.manual_id);
                }
                dr.Close();
            }
			return result;
		}
		
	
		private static Manual GetFromDataReader(MySqlDataReader dr)
		{
			Manual o = new Manual();
		
			o.manual_id =  (int) dr["manual_id"];
			o.title = string.Empty + Utilities.GetReaderField(dr,"title");
			o.logo = string.Empty + Utilities.GetReaderField(dr,"logo");
			o.create_date = Utilities.FromDbValue<DateTime>(Utilities.GetReaderField(dr,"create_date"));
			
			return o;

		}
		
		
		public static void Create(Manual o)
        {
            string insertsql = @"INSERT INTO manual (title,logo,create_date) VALUES(@title,@logo,@create_date)";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				
				MySqlCommand cmd = Utilities.GetCommand(insertsql, conn);
                BuildSqlParameters(cmd,o);
                cmd.ExecuteNonQuery();
				cmd.CommandText = "SELECT manual_id FROM manual WHERE manual_id = LAST_INSERT_ID()";
                o.manual_id = (int) cmd.ExecuteScalar();
				
            }
		}
		
		private static void BuildSqlParameters(MySqlCommand cmd, Manual o, bool forInsert = true)
        {
			
			if(!forInsert)
				cmd.Parameters.AddWithValue("@manual_id", o.manual_id);
			cmd.Parameters.AddWithValue("@title", o.title);
			cmd.Parameters.AddWithValue("@logo", o.logo);
			cmd.Parameters.AddWithValue("@create_date", o.create_date);
		}
		
		public static void Update(Manual o)
		{
			string updatesql = @"UPDATE manual SET title = @title,logo = @logo,create_date = @create_date WHERE manual_id = @manual_id";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				MySqlCommand cmd = Utilities.GetCommand(updatesql, conn);
                BuildSqlParameters(cmd,o, false);
                cmd.ExecuteNonQuery();
            }
		}
		
		public static void Delete(int manual_id)
		{
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = Utilities.GetCommand("DELETE FROM manual WHERE manual_id = @id" , conn);
                cmd.Parameters.AddWithValue("@id", manual_id);
                cmd.ExecuteNonQuery();
            }
		}
		
		
	}
}
			
			