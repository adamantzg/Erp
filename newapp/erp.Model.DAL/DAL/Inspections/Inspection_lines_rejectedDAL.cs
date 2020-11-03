
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace erp.Model.DAL
{
    public class Inspection_lines_rejectedDAL
	{
	
		public static List<Inspection_lines_rejected> GetAll()
		{
			var result = new List<Inspection_lines_rejected>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM inspection_lines_rejected", conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}

        public static List<Inspection_lines_rejected> GetByInspection(int insp_id)
        {
            var result = new List<Inspection_lines_rejected>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM Inspection_lines_rejected WHERE insp_unique = @insp_id", conn);
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
		
		public static Inspection_lines_rejected GetById(int id)
		{
			Inspection_lines_rejected result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM inspection_lines_rejected WHERE insp_line_unique = @id", conn);
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
		
	
		private static Inspection_lines_rejected GetFromDataReader(MySqlDataReader dr)
		{
			Inspection_lines_rejected o = new Inspection_lines_rejected();
		
			o.insp_line_unique =  (int) dr["insp_line_unique"];
			o.insp_unique = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"insp_unique"));
			o.insp_line_id = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"insp_line_id"));
			o.insp_line_type = string.Empty + Utilities.GetReaderField(dr,"insp_line_type");
			o.insp_line_rejection = string.Empty + Utilities.GetReaderField(dr,"insp_line_rejection");
			o.insp_line_action = string.Empty + Utilities.GetReaderField(dr,"insp_line_action");
			o.insp_po_linenum = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"insp_po_linenum"));
			o.insp_qty2 = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"insp_qty2"));
			o.insp_qty3 = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"insp_qty3"));
			o.insp_ca = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"insp_ca"));
			o.insp_comments = string.Empty + Utilities.GetReaderField(dr,"insp_comments");
			o.insp_reason = string.Empty + Utilities.GetReaderField(dr,"insp_reason");
			o.insp_permanent_action = string.Empty + Utilities.GetReaderField(dr,"insp_permanent_action");
			o.insp_document = string.Empty + Utilities.GetReaderField(dr,"insp_document");
			o.insp_pdf = string.Empty + Utilities.GetReaderField(dr,"insp_pdf");
			o.master_line = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"master_line"));
			o.criteria_id = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"criteria_id"));
			o.reworked = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"reworked"));
			
			return o;

		}
		
		
		public static void Create(Inspection_lines_rejected o)
        {
            string insertsql = @"INSERT INTO inspection_lines_rejected (insp_unique,insp_line_id,insp_line_type,insp_line_rejection,insp_line_action,insp_po_linenum,insp_qty2,insp_qty3,insp_ca,insp_comments,insp_reason,insp_permanent_action,insp_document,insp_pdf,master_line,criteria_id,reworked) VALUES(@insp_unique,@insp_line_id,@insp_line_type,@insp_line_rejection,@insp_line_action,@insp_po_linenum,@insp_qty2,@insp_qty3,@insp_ca,@insp_comments,@insp_reason,@insp_permanent_action,@insp_document,@insp_pdf,@master_line,@criteria_id,@reworked)";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				
				var cmd = Utils.GetCommand(insertsql, conn);
                BuildSqlParameters(cmd,o);
                cmd.ExecuteNonQuery();
				cmd.CommandText = "SELECT insp_line_unique FROM inspection_lines_rejected WHERE insp_line_unique = LAST_INSERT_ID()";
                o.insp_line_unique = (int) cmd.ExecuteScalar();
				
            }
		}
		
		private static void BuildSqlParameters(MySqlCommand cmd, Inspection_lines_rejected o, bool forInsert = true)
        {
			
			if(!forInsert)
				cmd.Parameters.AddWithValue("@insp_line_unique", o.insp_line_unique);
			cmd.Parameters.AddWithValue("@insp_unique", o.insp_unique);
			cmd.Parameters.AddWithValue("@insp_line_id", o.insp_line_id);
			cmd.Parameters.AddWithValue("@insp_line_type", o.insp_line_type);
			cmd.Parameters.AddWithValue("@insp_line_rejection", o.insp_line_rejection);
			cmd.Parameters.AddWithValue("@insp_line_action", o.insp_line_action);
			cmd.Parameters.AddWithValue("@insp_po_linenum", o.insp_po_linenum);
			cmd.Parameters.AddWithValue("@insp_qty2", o.insp_qty2);
			cmd.Parameters.AddWithValue("@insp_qty3", o.insp_qty3);
			cmd.Parameters.AddWithValue("@insp_ca", o.insp_ca);
			cmd.Parameters.AddWithValue("@insp_comments", o.insp_comments);
			cmd.Parameters.AddWithValue("@insp_reason", o.insp_reason);
			cmd.Parameters.AddWithValue("@insp_permanent_action", o.insp_permanent_action);
			cmd.Parameters.AddWithValue("@insp_document", o.insp_document);
			cmd.Parameters.AddWithValue("@insp_pdf", o.insp_pdf);
			cmd.Parameters.AddWithValue("@master_line", o.master_line);
			cmd.Parameters.AddWithValue("@criteria_id", o.criteria_id);
			cmd.Parameters.AddWithValue("@reworked", o.reworked);
		}
		
		public static void Update(Inspection_lines_rejected o)
		{
			string updatesql = @"UPDATE inspection_lines_rejected SET insp_unique = @insp_unique,insp_line_id = @insp_line_id,insp_line_type = @insp_line_type,insp_line_rejection = @insp_line_rejection,insp_line_action = @insp_line_action,insp_po_linenum = @insp_po_linenum,insp_qty2 = @insp_qty2,insp_qty3 = @insp_qty3,insp_ca = @insp_ca,insp_comments = @insp_comments,insp_reason = @insp_reason,insp_permanent_action = @insp_permanent_action,insp_document = @insp_document,insp_pdf = @insp_pdf,master_line = @master_line,criteria_id = @criteria_id,reworked = @reworked WHERE insp_line_unique = @insp_line_unique";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = Utils.GetCommand(updatesql, conn);
                BuildSqlParameters(cmd,o, false);
                cmd.ExecuteNonQuery();
            }
		}
		
		public static void Delete(int insp_line_unique)
		{
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = Utils.GetCommand("DELETE FROM inspection_lines_rejected WHERE insp_line_unique = @id" , conn);
                cmd.Parameters.AddWithValue("@id", insp_line_unique);
                cmd.ExecuteNonQuery();
            }
		}
		
		
	}
}
			
			