
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace asaq2.Model
{
    public class Inspection_lines_acceptedDAL
	{
	
		public static List<Inspection_lines_accepted> GetAll()
		{
			var result = new List<Inspection_lines_accepted>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT * FROM inspection_lines_accepted", conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}

        public static List<Inspection_lines_accepted> GetByInspection(int insp_id)
        {
            var result = new List<Inspection_lines_accepted>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT * FROM inspection_lines_accepted WHERE insp_unique = @insp_id", conn);
                cmd.Parameters.AddWithValue("@insp_id", insp_id);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
        }
		
		
		public static Inspection_lines_accepted GetById(int id)
		{
			Inspection_lines_accepted result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT * FROM inspection_lines_accepted WHERE insp_line_unique = @id", conn);
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
		
	
		private static Inspection_lines_accepted GetFromDataReader(MySqlDataReader dr)
		{
			Inspection_lines_accepted o = new Inspection_lines_accepted();
		
			o.insp_line_unique =  (int) dr["insp_line_unique"];
			o.insp_unique = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"insp_unique"));
			o.insp_line_id = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"insp_line_id"));
			o.insp_line_type = string.Empty + Utilities.GetReaderField(dr,"insp_line_type");
			o.insp_line_comments = string.Empty + Utilities.GetReaderField(dr,"insp_line_comments");
			o.insp_po_linenum = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"insp_po_linenum"));
			o.insp_qty2 = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"insp_qty2"));
			o.insp_qty3 = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"insp_qty3"));
			
			return o;

		}
		
		
		public static void Create(Inspection_lines_accepted o)
        {
            string insertsql = @"INSERT INTO inspection_lines_accepted (insp_unique,insp_line_id,insp_line_type,insp_line_comments,insp_po_linenum,insp_qty2,insp_qty3) VALUES(@insp_unique,@insp_line_id,@insp_line_type,@insp_line_comments,@insp_po_linenum,@insp_qty2,@insp_qty3)";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				
				var cmd = new MySqlCommand(insertsql, conn);
                BuildSqlParameters(cmd,o);
                cmd.ExecuteNonQuery();
				cmd.CommandText = "SELECT insp_line_unique FROM inspection_lines_accepted WHERE insp_line_unique = LAST_INSERT_ID()";
                o.insp_line_unique = (int) cmd.ExecuteScalar();
				
            }
		}
		
		private static void BuildSqlParameters(MySqlCommand cmd, Inspection_lines_accepted o, bool forInsert = true)
        {
			
			if(!forInsert)
				cmd.Parameters.AddWithValue("@insp_line_unique", o.insp_line_unique);
			cmd.Parameters.AddWithValue("@insp_unique", o.insp_unique);
			cmd.Parameters.AddWithValue("@insp_line_id", o.insp_line_id);
			cmd.Parameters.AddWithValue("@insp_line_type", o.insp_line_type);
			cmd.Parameters.AddWithValue("@insp_line_comments", o.insp_line_comments);
			cmd.Parameters.AddWithValue("@insp_po_linenum", o.insp_po_linenum);
			cmd.Parameters.AddWithValue("@insp_qty2", o.insp_qty2);
			cmd.Parameters.AddWithValue("@insp_qty3", o.insp_qty3);
		}
		
		public static void Update(Inspection_lines_accepted o)
		{
			string updatesql = @"UPDATE inspection_lines_accepted SET insp_unique = @insp_unique,insp_line_id = @insp_line_id,insp_line_type = @insp_line_type,insp_line_comments = @insp_line_comments,insp_po_linenum = @insp_po_linenum,insp_qty2 = @insp_qty2,insp_qty3 = @insp_qty3 WHERE insp_line_unique = @insp_line_unique";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = new MySqlCommand(updatesql, conn);
                BuildSqlParameters(cmd,o, false);
                cmd.ExecuteNonQuery();
            }
		}
		
		public static void Delete(int insp_line_unique)
		{
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = new MySqlCommand("DELETE FROM inspection_lines_accepted WHERE insp_line_unique = @id" , conn);
                cmd.Parameters.AddWithValue("@id", insp_line_unique);
                cmd.ExecuteNonQuery();
            }
		}
		
		
	}
}
			
			