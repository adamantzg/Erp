
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace asaq2.Model
{
    public class After_sales_enquiry_sitevisit_statusDAL
	{
	
		public static List<After_sales_enquiry_sitevisit_status> GetAll()
		{
			List<After_sales_enquiry_sitevisit_status> result = new List<After_sales_enquiry_sitevisit_status>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM after_sales_enquiry_sitevisit_status", conn);
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}
		
		
		public static After_sales_enquiry_sitevisit_status GetById(int id)
		{
			After_sales_enquiry_sitevisit_status result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM after_sales_enquiry_sitevisit_status WHERE status_id = @id", conn);
				cmd.Parameters.AddWithValue("@id", id);
                MySqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    result = GetFromDataReader(dr);
                }
                dr.Close();
            }
			return result;
		}
	
		private static After_sales_enquiry_sitevisit_status GetFromDataReader(MySqlDataReader dr)
		{
			After_sales_enquiry_sitevisit_status o = new After_sales_enquiry_sitevisit_status();
		
			o.status_id =  (int) dr["status_id"];
			o.status_name = string.Empty + dr["status_name"];
			
			return o;

		}
		
		public static void Create(After_sales_enquiry_sitevisit_status o)
        {
            string insertsql = @"INSERT INTO after_sales_enquiry_sitevisit_status (status_id,status_name) VALUES(@status_id,@status_name)";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				
                MySqlCommand cmd = new MySqlCommand("SELECT MAX(status_id)+1 FROM after_sales_enquiry_sitevisit_status", conn);
                o.status_id = Convert.ToInt32(cmd.ExecuteScalar());
				cmd.CommandText = insertsql;
				
                BuildSqlParameters(cmd,o);
                cmd.ExecuteNonQuery();
				
            }
		}
		
		private static void BuildSqlParameters(MySqlCommand cmd, After_sales_enquiry_sitevisit_status o, bool forInsert = true)
        {
			
			cmd.Parameters.AddWithValue("@status_id", o.status_id);
			cmd.Parameters.AddWithValue("@status_name", o.status_name);
		}
		
		public static void Update(After_sales_enquiry_sitevisit_status o)
		{
			string updatesql = @"UPDATE after_sales_enquiry_sitevisit_status SET status_name = @status_name WHERE status_id = @status_id";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				MySqlCommand cmd = new MySqlCommand(updatesql, conn);
                BuildSqlParameters(cmd,o, false);
                cmd.ExecuteNonQuery();
            }
		}
		
		public static void Delete(int status_id)
		{
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				MySqlCommand cmd = new MySqlCommand("DELETE FROM after_sales_enquiry_sitevisit_status WHERE status_id = @id" , conn);
                cmd.Parameters.AddWithValue("@id", status_id);
                cmd.ExecuteNonQuery();
            }
		}
	}
}
			
			