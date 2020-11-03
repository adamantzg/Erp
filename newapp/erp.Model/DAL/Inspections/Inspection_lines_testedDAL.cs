
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace asaq2.Model
{
    public class Inspection_lines_testedDAL
	{
	
		public static List<Inspection_lines_tested> GetAll()
		{
			var result = new List<Inspection_lines_tested>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT * FROM inspection_lines_tested", conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}

        public static List<Inspection_lines_tested> GetByInspection(int insp_id)
        {
            var result = new List<Inspection_lines_tested>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand(@"SELECT Inspection_lines_tested.*, order_lines.* FROM Inspection_lines_tested LEFT OUTER JOIN order_lines ON Inspection_lines_tested.order_linenum = order_lines.linenum 
                                            WHERE insp_id = @insp_id", conn);
                cmd.Parameters.AddWithValue("@insp_id", insp_id);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    var line = GetFromDataReader(dr);
                    if (line.order_linenum > 0)
                        line.OrderLine = Order_linesDAL.GetFromDataReader(dr);
                    result.Add(line);
                }
                dr.Close();
            }
            return result;
        }
		
		
		public static Inspection_lines_tested GetById(int id)
		{
			Inspection_lines_tested result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT * FROM inspection_lines_tested WHERE insp_line_unique = @id", conn);
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
		
	
		private static Inspection_lines_tested GetFromDataReader(MySqlDataReader dr)
		{
			Inspection_lines_tested o = new Inspection_lines_tested();
		
			o.insp_line_unique =  (int) dr["insp_line_unique"];
			o.insp_id = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"insp_id"));
			o.insp_factory_ref = string.Empty + Utilities.GetReaderField(dr,"insp_factory_ref");
			o.insp_client_ref = string.Empty + Utilities.GetReaderField(dr,"insp_client_ref");
			o.insp_client_desc = string.Empty + Utilities.GetReaderField(dr,"insp_client_desc");
			o.insp_qty = Utilities.FromDbValue<double>(Utilities.GetReaderField(dr,"insp_qty"));
			o.insp_override_qty = Utilities.FromDbValue<double>(Utilities.GetReaderField(dr,"insp_override_qty"));
			o.insp_custpo = string.Empty + Utilities.GetReaderField(dr,"insp_custpo");
			o.order_linenum = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"order_linenum"));
			o.photo_confirm = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"photo_confirm"));
			o.photo_confirma = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"photo_confirma"));
			o.photo_confirmm = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"photo_confirmm"));
			o.photo_confirmd = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"photo_confirmd"));
			o.photo_confirmf = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"photo_confirmf"));
			o.photo_confirmp = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"photo_confirmp"));
			o.packaging_rej = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"packaging_rej"));
			o.label_rej = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"label_rej"));
			o.instructions_rej = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"instructions_rej"));
			
			return o;

		}
		
		
		public static void Create(Inspection_lines_tested o)
        {
            string insertsql = @"INSERT INTO inspection_lines_tested (insp_id,insp_factory_ref,insp_client_ref,insp_client_desc,insp_qty,insp_override_qty,insp_custpo,order_linenum,photo_confirm,photo_confirma,photo_confirmm,photo_confirmd,photo_confirmf,photo_confirmp,packaging_rej,label_rej,instructions_rej) VALUES(@insp_id,@insp_factory_ref,@insp_client_ref,@insp_client_desc,@insp_qty,@insp_override_qty,@insp_custpo,@order_linenum,@photo_confirm,@photo_confirma,@photo_confirmm,@photo_confirmd,@photo_confirmf,@photo_confirmp,@packaging_rej,@label_rej,@instructions_rej)";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				
				var cmd = new MySqlCommand(insertsql, conn);
                BuildSqlParameters(cmd,o);
                cmd.ExecuteNonQuery();
				cmd.CommandText = "SELECT insp_line_unique FROM inspection_lines_tested WHERE insp_line_unique = LAST_INSERT_ID()";
                o.insp_line_unique = (int) cmd.ExecuteScalar();
				
            }
		}
		
		private static void BuildSqlParameters(MySqlCommand cmd, Inspection_lines_tested o, bool forInsert = true)
        {
			
			if(!forInsert)
				cmd.Parameters.AddWithValue("@insp_line_unique", o.insp_line_unique);
			cmd.Parameters.AddWithValue("@insp_id", o.insp_id);
			cmd.Parameters.AddWithValue("@insp_factory_ref", o.insp_factory_ref);
			cmd.Parameters.AddWithValue("@insp_client_ref", o.insp_client_ref);
			cmd.Parameters.AddWithValue("@insp_client_desc", o.insp_client_desc);
			cmd.Parameters.AddWithValue("@insp_qty", o.insp_qty);
			cmd.Parameters.AddWithValue("@insp_override_qty", o.insp_override_qty);
			cmd.Parameters.AddWithValue("@insp_custpo", o.insp_custpo);
			cmd.Parameters.AddWithValue("@order_linenum", o.order_linenum);
			cmd.Parameters.AddWithValue("@photo_confirm", o.photo_confirm);
			cmd.Parameters.AddWithValue("@photo_confirma", o.photo_confirma);
			cmd.Parameters.AddWithValue("@photo_confirmm", o.photo_confirmm);
			cmd.Parameters.AddWithValue("@photo_confirmd", o.photo_confirmd);
			cmd.Parameters.AddWithValue("@photo_confirmf", o.photo_confirmf);
			cmd.Parameters.AddWithValue("@photo_confirmp", o.photo_confirmp);
			cmd.Parameters.AddWithValue("@packaging_rej", o.packaging_rej);
			cmd.Parameters.AddWithValue("@label_rej", o.label_rej);
			cmd.Parameters.AddWithValue("@instructions_rej", o.instructions_rej);
		}
		
		public static void Update(Inspection_lines_tested o)
		{
			string updatesql = @"UPDATE inspection_lines_tested SET insp_id = @insp_id,insp_factory_ref = @insp_factory_ref,insp_client_ref = @insp_client_ref,insp_client_desc = @insp_client_desc,insp_qty = @insp_qty,insp_override_qty = @insp_override_qty,insp_custpo = @insp_custpo,order_linenum = @order_linenum,photo_confirm = @photo_confirm,photo_confirma = @photo_confirma,photo_confirmm = @photo_confirmm,photo_confirmd = @photo_confirmd,photo_confirmf = @photo_confirmf,photo_confirmp = @photo_confirmp,packaging_rej = @packaging_rej,label_rej = @label_rej,instructions_rej = @instructions_rej WHERE insp_line_unique = @insp_line_unique";

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
				var cmd = new MySqlCommand("DELETE FROM inspection_lines_tested WHERE insp_line_unique = @id" , conn);
                cmd.Parameters.AddWithValue("@id", insp_line_unique);
                cmd.ExecuteNonQuery();
            }
		}
		
		
	}
}
			
			