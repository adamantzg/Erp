
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace erp.Model.DAL
{
    public class Creditnote_lineDAL
	{
	
		public static List<Creditnote_line> GetAll()
		{
			var result = new List<Creditnote_line>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM creditnote_line", conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}

        public static List<Creditnote_line> GetForInvoice(int id)
        {
            var result = new List<Creditnote_line>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM creditnote_line WHERE invoice_id = @id", conn);
                cmd.Parameters.AddWithValue("@id", id);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
        }
		
		
		public static Creditnote_line GetById(int id)
		{
			Creditnote_line result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM creditnote_line WHERE line_id = @id", conn);
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
		
	
		private static Creditnote_line GetFromDataReader(MySqlDataReader dr)
		{
			Creditnote_line o = new Creditnote_line();
		
			o.line_id =  (int) dr["line_id"];
			o.invoice_id =  (int) dr["invoice_id"];
			o.return_no = string.Empty + Utilities.GetReaderField(dr,"return_no");
			o.cprod_code = string.Empty + Utilities.GetReaderField(dr,"cprod_code");
			o.client_ref = string.Empty + Utilities.GetReaderField(dr,"client_ref");
			o.cprod_name = string.Empty + Utilities.GetReaderField(dr,"cprod_name");
			o.unitprice = Utilities.FromDbValue<double>(Utilities.GetReaderField(dr,"unitprice"));
			o.quantity = Utilities.FromDbValue<double>(Utilities.GetReaderField(dr,"quantity"));
		    o.overridden = Utilities.BoolFromLong(dr["overridden"]);
			return o;

		}

        public static void Create(Creditnote_line o, MySqlTransaction tr = null)
        {
            string insertsql = @"INSERT INTO creditnote_line (invoice_id,return_no,cprod_code,client_ref,cprod_name,unitprice,quantity,overridden) VALUES(@invoice_id,@return_no,@cprod_code,@client_ref,@cprod_name,@unitprice,@quantity,@overridden)";


            var conn = (tr != null ? tr.Connection : new MySqlConnection(Properties.Settings.Default.ConnString));
            if (tr == null)
                conn.Open();

            MySqlCommand cmd = Utils.GetCommand(insertsql, conn, tr);
            BuildSqlParameters(cmd, o);
            cmd.ExecuteNonQuery();
            cmd.CommandText = "SELECT line_id FROM creditnote_line WHERE line_id = LAST_INSERT_ID()";
            o.line_id = (int)cmd.ExecuteScalar();

            if (tr == null)
                conn.Close();


        }
		
		
		private static void BuildSqlParameters(MySqlCommand cmd, Creditnote_line o, bool forInsert = true)
        {
			
			if(!forInsert)
				cmd.Parameters.AddWithValue("@line_id", o.line_id);
			cmd.Parameters.AddWithValue("@invoice_id", o.invoice_id);
			cmd.Parameters.AddWithValue("@return_no", o.return_no);
			cmd.Parameters.AddWithValue("@cprod_code", o.cprod_code);
			cmd.Parameters.AddWithValue("@client_ref", o.client_ref);
			cmd.Parameters.AddWithValue("@cprod_name", o.cprod_name);
			cmd.Parameters.AddWithValue("@unitprice", o.unitprice);
			cmd.Parameters.AddWithValue("@quantity", o.quantity);
		    cmd.Parameters.AddWithValue("@overridden", o.overridden);
        }
		
		public static void Update(Creditnote_line o)
		{
            string updatesql = @"UPDATE creditnote_line SET invoice_id = @invoice_id,return_no = @return_no,cprod_code = @cprod_code,client_ref = @client_ref,cprod_name = @cprod_name,unitprice = @unitprice,quantity = @quantity,overridden = @overridden 
                                WHERE line_id = @line_id";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = Utils.GetCommand(updatesql, conn);
                BuildSqlParameters(cmd,o, false);
                cmd.ExecuteNonQuery();
            }
		}
		
		public static void Delete(int line_id)
		{
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = Utils.GetCommand("DELETE FROM creditnote_line WHERE line_id = @id" , conn);
                cmd.Parameters.AddWithValue("@id", line_id);
                cmd.ExecuteNonQuery();
            }
		}
		
		
	}
}
			
			