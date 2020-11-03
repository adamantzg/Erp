
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace asaq2.Model
{
    public class Invoice_linesDAL
	{
	
		public static List<Invoice_lines> GetAll()
		{
			var result = new List<Invoice_lines>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT * FROM invoice_lines", conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}
		
		
		public static Invoice_lines GetById(int id)
		{
			Invoice_lines result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT * FROM invoice_lines WHERE linenum = @id", conn);
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
		
	
		private static Invoice_lines GetFromDataReader(MySqlDataReader dr)
		{
			Invoice_lines o = new Invoice_lines();
		
			o.linenum =  Convert.ToInt32(dr["linenum"]);
			o.invoice_id = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"invoice_id"));
			o.linedate = Utilities.FromDbValue<DateTime>(Utilities.GetReaderField(dr,"linedate"));
			o.cprod_id = string.Empty + Utilities.GetReaderField(dr,"cprod_id");
			o.description = string.Empty + Utilities.GetReaderField(dr,"description");
			o.orderqty = Utilities.FromDbValue<double>(Utilities.GetReaderField(dr,"orderqty"));
			o.unitprice = Utilities.FromDbValue<double>(Utilities.GetReaderField(dr,"unitprice"));
			o.unitcurrency = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"unitcurrency"));
			o.linestatus = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"linestatus"));
			o.record_type = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"record_type"));
            o.qty_type = Utilities.FromDbValue<int>(dr["qty_type"]);
			
			return o;

		}
		
		
		public static void Create(Invoice_lines o,MySqlTransaction tr = null)
        {
            string insertsql = @"INSERT INTO invoice_lines (invoice_id,linedate,cprod_id,description,orderqty,unitprice,unitcurrency,linestatus,record_type,qty_type) 
                    VALUES(@invoice_id,@linedate,@cprod_id,@description,@orderqty,@unitprice,@unitcurrency,@linestatus,@record_type,@qty_type)";


		    var conn = (tr != null ? tr.Connection : new MySqlConnection(Properties.Settings.Default.ConnString));
            if(tr == null)
                conn.Open();
				
			MySqlCommand cmd = new MySqlCommand(insertsql, conn,tr);
            BuildSqlParameters(cmd,o);
            cmd.ExecuteNonQuery();
			cmd.CommandText = "SELECT linenum FROM invoice_lines WHERE linenum = LAST_INSERT_ID()";
            o.linenum = Convert.ToInt32(cmd.ExecuteScalar());

            if(tr == null)
                conn.Close();
				
            
		}
		
		private static void BuildSqlParameters(MySqlCommand cmd, Invoice_lines o, bool forInsert = true)
        {
			
			if(!forInsert)
				cmd.Parameters.AddWithValue("@linenum", o.linenum);
			cmd.Parameters.AddWithValue("@invoice_id", o.invoice_id);
			cmd.Parameters.AddWithValue("@linedate", o.linedate);
			cmd.Parameters.AddWithValue("@cprod_id", o.cprod_id);
			cmd.Parameters.AddWithValue("@description", o.description);
			cmd.Parameters.AddWithValue("@orderqty", o.orderqty);
			cmd.Parameters.AddWithValue("@unitprice", o.unitprice);
			cmd.Parameters.AddWithValue("@unitcurrency", o.unitcurrency);
			cmd.Parameters.AddWithValue("@linestatus", o.linestatus);
			cmd.Parameters.AddWithValue("@record_type", o.record_type);
            cmd.Parameters.AddWithValue("@qty_type", o.qty_type);
        }
		
		public static void Update(Invoice_lines o, MySqlTransaction tr = null)
		{
			string updatesql = @"UPDATE invoice_lines SET invoice_id = @invoice_id,linedate = @linedate,cprod_id = @cprod_id,description = @description,orderqty = @orderqty,unitprice = @unitprice,
                    unitcurrency = @unitcurrency,linestatus = @linestatus,record_type = @record_type,qty_type = @qty_type WHERE linenum = @linenum";

		    var conn = tr != null ? tr.Connection : new MySqlConnection(Properties.Settings.Default.ConnString);
            if(tr == null)
                conn.Open();
			var cmd = new MySqlCommand(updatesql, conn,tr);
            BuildSqlParameters(cmd,o, false);
            cmd.ExecuteNonQuery();

            if(tr==null)
                conn.Close();
            
		}
		
		public static void Delete(int linenum, MySqlTransaction tr = null)
		{
			var conn = tr != null ? tr.Connection : new MySqlConnection(Properties.Settings.Default.ConnString);
            if(tr == null)
                conn.Open();
			var cmd = new MySqlCommand("DELETE FROM invoice_lines WHERE linenum = @id" , conn,tr);
            cmd.Parameters.AddWithValue("@id", linenum);
            cmd.ExecuteNonQuery();
            if (tr == null)
                conn.Close();
		}


        public static List<Invoice_lines> GetByInvoice(int id)
        {
            var result = new List<Invoice_lines>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT * FROM invoice_lines WHERE invoice_id = @invoice", conn);
                cmd.Parameters.AddWithValue("@invoice", id);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
        }
	}
}
			
			