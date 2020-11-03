
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace Instructions.Model
{
    public class SectionDAL
	{
	
		public static List<Section> GetAll()
		{
			var result = new List<Section>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utilities.GetCommand("SELECT * FROM section", conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}

        public static List<Section> GetForChapter(int chapter_id)
        {
            var result = new List<Section>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utilities.GetCommand("SELECT * FROM section WHERE chapter_id = @chapter_id", conn);
                cmd.Parameters.AddWithValue("@chapter_id", chapter_id);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
        }
		
		
		public static Section GetById(int id)
		{
			Section result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utilities.GetCommand("SELECT section.*, chapter.title AS chapter_title FROM section INNER JOIN chapter ON section.chapter_id = chapter.chapter_id WHERE section_id = @id", conn);
				cmd.Parameters.AddWithValue("@id", id);
                var dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    result = GetFromDataReader(dr);
                    result.Details = DetailDAL.GetForSection(id);
                }
                dr.Close();
            }
			return result;
		}
		
	
		private static Section GetFromDataReader(MySqlDataReader dr)
		{
			Section o = new Section();
		
			o.section_id =  (int) dr["section_id"];
			o.chapter_id = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"chapter_id"));
			o.heading = string.Empty + Utilities.GetReaderField(dr,"heading");
			o.image = string.Empty + Utilities.GetReaderField(dr,"image");
			o.sequence = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"sequence"));
		    if (Utilities.ColumnExists(dr, "chapter_title"))
		        o.chapter_title = string.Empty + dr["chapter_title"];
			
			return o;

		}
		
		
		public static void Create(Section o)
        {
            string insertsql = @"INSERT INTO section (chapter_id,heading,image,sequence) VALUES(@chapter_id,@heading,@image,@sequence)";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				
				MySqlCommand cmd = Utilities.GetCommand(insertsql, conn);
                BuildSqlParameters(cmd,o);
                cmd.ExecuteNonQuery();
				cmd.CommandText = "SELECT section_id FROM section WHERE section_id = LAST_INSERT_ID()";
                o.section_id = (int) cmd.ExecuteScalar();
				
            }
		}
		
		private static void BuildSqlParameters(MySqlCommand cmd, Section o, bool forInsert = true)
        {
			
			if(!forInsert)
				cmd.Parameters.AddWithValue("@section_id", o.section_id);
			cmd.Parameters.AddWithValue("@chapter_id", o.chapter_id);
			cmd.Parameters.AddWithValue("@heading", o.heading);
			cmd.Parameters.AddWithValue("@image", o.image);
			cmd.Parameters.AddWithValue("@sequence", o.sequence);
		}
		
		public static void Update(Section o)
		{
			string updatesql = @"UPDATE section SET chapter_id = @chapter_id,heading = @heading,image = @image,sequence = @sequence WHERE section_id = @section_id";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				MySqlCommand cmd = Utilities.GetCommand(updatesql, conn);
                BuildSqlParameters(cmd,o, false);
                cmd.ExecuteNonQuery();
            }
		}
		
		public static void Delete(int section_id)
		{
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = Utilities.GetCommand("DELETE FROM section WHERE section_id = @id" , conn);
                cmd.Parameters.AddWithValue("@id", section_id);
                cmd.ExecuteNonQuery();
            }
		}
		
		
	}
}
			
			