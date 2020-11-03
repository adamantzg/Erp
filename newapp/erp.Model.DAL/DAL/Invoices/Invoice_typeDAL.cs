
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace erp.Model.DAL
{
    public class Invoice_typeDAL
	{
	
		public static List<Invoice_type> GetAll()
		{
			var result = new List<Invoice_type>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM invoice_type", conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}
		
		
		public static Invoice_type GetById(int id)
		{
			Invoice_type result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM invoice_type WHERE invoice_type_id = @id", conn);
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
		
	
		private static Invoice_type GetFromDataReader(MySqlDataReader dr)
		{
			Invoice_type o = new Invoice_type();
		
			o.invoice_type_id =  (int) dr["invoice_type_id"];
			o.invoice_type_name = string.Empty + Utilities.GetReaderField(dr,"invoice_type_name");
		    o.showOnForm = Utilities.BoolFromLong(dr["showOnForm"]);
			
			return o;

		}
		
		
		public static void Create(Invoice_type o)
        {
            string insertsql = @"INSERT INTO invoice_type (invoice_type_id,invoice_type_name) VALUES(@invoice_type_id,@invoice_type_name)";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				
                MySqlCommand cmd = Utils.GetCommand("SELECT MAX(invoice_type_id)+1 FROM invoice_type", conn);
                o.invoice_type_id = Convert.ToInt32(cmd.ExecuteScalar());
				cmd.CommandText = insertsql;
				
                BuildSqlParameters(cmd,o);
                cmd.ExecuteNonQuery();
				
            }
		}
		
		private static void BuildSqlParameters(MySqlCommand cmd, Invoice_type o, bool forInsert = true)
        {
			
			cmd.Parameters.AddWithValue("@invoice_type_id", o.invoice_type_id);
			cmd.Parameters.AddWithValue("@invoice_type_name", o.invoice_type_name);
		}
		
		public static void Update(Invoice_type o)
		{
			string updatesql = @"UPDATE invoice_type SET invoice_type_name = @invoice_type_name WHERE invoice_type_id = @invoice_type_id";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				MySqlCommand cmd = Utils.GetCommand(updatesql, conn);
                BuildSqlParameters(cmd,o, false);
                cmd.ExecuteNonQuery();
            }
		}
		
		public static void Delete(int invoice_type_id)
		{
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = Utils.GetCommand("DELETE FROM invoice_type WHERE invoice_type_id = @id" , conn);
                cmd.Parameters.AddWithValue("@id", invoice_type_id);
                cmd.ExecuteNonQuery();
            }
		}
		
		
	}
}
			
			