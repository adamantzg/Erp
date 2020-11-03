
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace Instructions.Model
{
    public class ChapterDAL
	{
	
		public static List<Chapter> GetAll()
		{
			var result = new List<Chapter>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utilities.GetCommand("SELECT * FROM chapter", conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}

        public static List<Chapter> GetForManual(int manual_id)
        {
            var result = new List<Chapter>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utilities.GetCommand("SELECT * FROM chapter WHERE manual_id = @manual_id", conn);
                cmd.Parameters.AddWithValue("@manual_id", manual_id);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
        }
		
		
		public static Chapter GetById(int id)
		{
			Chapter result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utilities.GetCommand("SELECT chapter.*, manual.title AS manual_title FROM chapter INNER JOIN manual ON chapter.manual_id = manual.manual_id WHERE chapter_id = @id", conn);
				cmd.Parameters.AddWithValue("@id", id);
                var dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    result = GetFromDataReader(dr);
                    result.Sections = SectionDAL.GetForChapter(id);
                }
                dr.Close();
            }
			return result;
		}


		
	
		private static Chapter GetFromDataReader(MySqlDataReader dr)
		{
			Chapter o = new Chapter();
		
			o.chapter_id =  (int) dr["chapter_id"];
			o.manual_id = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"manual_id"));
			o.title = string.Empty + Utilities.GetReaderField(dr,"title");
			o.sequence = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"sequence"));
		    if (Utilities.ColumnExists(dr, "manual_title"))
		        o.manual_title = string.Empty + dr["manual_title"];
			
			return o;

		}
		
		
		public static void Create(Chapter o)
        {
            string insertsql = @"INSERT INTO chapter (manual_id,title,sequence) VALUES(@manual_id,@title,@sequence)";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				
				MySqlCommand cmd = Utilities.GetCommand(insertsql, conn);
                BuildSqlParameters(cmd,o);
                cmd.ExecuteNonQuery();
				cmd.CommandText = "SELECT chapter_id FROM chapter WHERE chapter_id = LAST_INSERT_ID()";
                o.chapter_id = (int) cmd.ExecuteScalar();
				
            }
		}
		
		private static void BuildSqlParameters(MySqlCommand cmd, Chapter o, bool forInsert = true)
        {
			
			if(!forInsert)
				cmd.Parameters.AddWithValue("@chapter_id", o.chapter_id);
			cmd.Parameters.AddWithValue("@manual_id", o.manual_id);
			cmd.Parameters.AddWithValue("@title", o.title);
			cmd.Parameters.AddWithValue("@sequence", o.sequence);
		}
		
		public static void Update(Chapter o)
		{
			string updatesql = @"UPDATE chapter SET manual_id = @manual_id,title = @title,sequence = @sequence WHERE chapter_id = @chapter_id";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				MySqlCommand cmd = Utilities.GetCommand(updatesql, conn);
                BuildSqlParameters(cmd,o, false);
                cmd.ExecuteNonQuery();
            }
		}
		
		public static void Delete(int chapter_id)
		{
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = Utilities.GetCommand("DELETE FROM chapter WHERE chapter_id = @id" , conn);
                cmd.Parameters.AddWithValue("@id", chapter_id);
                cmd.ExecuteNonQuery();
            }
		}
		
		
	}
}
			
			