
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace erp.Model.DAL
{
    public class After_sales_enquiry_filesDAL
	{
	
		public static List<After_sales_enquiry_files> GetForEnquiry(int enquiry_id)
		{
			List<After_sales_enquiry_files> result = new List<After_sales_enquiry_files>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = Utils.GetCommand("SELECT * FROM after_sales_enquiry_files WHERE enquiry_id = @enquiry_id ", conn);
                cmd.Parameters.AddWithValue("@enquiry_id", enquiry_id);
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}
		
		
		public static After_sales_enquiry_files GetById(int id)
		{
			After_sales_enquiry_files result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = Utils.GetCommand("SELECT * FROM after_sales_enquiry_files WHERE file_id = @id", conn);
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
	
		private static After_sales_enquiry_files GetFromDataReader(MySqlDataReader dr)
		{
			After_sales_enquiry_files o = new After_sales_enquiry_files();
		
			o.file_id =  (int) dr["file_id"];
			o.enquiry_id = Utilities.FromDbValue<int>(dr["enquiry_id"]);
			o.file_name = string.Empty + dr["file_name"];
			o.datecreated = Utilities.FromDbValue<DateTime>(dr["datecreated"]);
			o.created_userid = Utilities.FromDbValue<int>(dr["created_userid"]);
			o.datemodified = Utilities.FromDbValue<DateTime>(dr["datemodified"]);
			o.modified_userid = Utilities.FromDbValue<int>(dr["modified_userid"]);
			
			return o;

		}
		
		public static void Create(After_sales_enquiry_files o)
        {
            string insertsql = @"INSERT INTO after_sales_enquiry_files (enquiry_id,file_name,datecreated,created_userid,datemodified,modified_userid) VALUES(@enquiry_id,@file_name,@datecreated,@created_userid,@datemodified,@modified_userid)";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				
				MySqlCommand cmd = Utils.GetCommand(insertsql, conn);
                BuildSqlParameters(cmd,o);
                cmd.ExecuteNonQuery();
				cmd.CommandText = "SELECT file_id FROM after_sales_enquiry_files WHERE file_id = LAST_INSERT_ID()";
                o.file_id = (int) cmd.ExecuteScalar();
				
            }
		}
		
		private static void BuildSqlParameters(MySqlCommand cmd, After_sales_enquiry_files o, bool forInsert = true)
        {
			
			if(!forInsert)
				cmd.Parameters.AddWithValue("@file_id", o.file_id);
			cmd.Parameters.AddWithValue("@enquiry_id", o.enquiry_id);
			cmd.Parameters.AddWithValue("@file_name", o.file_name);
			cmd.Parameters.AddWithValue("@datecreated", o.datecreated);
			cmd.Parameters.AddWithValue("@created_userid", o.created_userid);
			cmd.Parameters.AddWithValue("@datemodified", o.datemodified);
			cmd.Parameters.AddWithValue("@modified_userid", o.modified_userid);
		}
		
		public static void Update(After_sales_enquiry_files o)
		{
			string updatesql = @"UPDATE after_sales_enquiry_files SET enquiry_id = @enquiry_id,file_name = @file_name,datecreated = @datecreated,created_userid = @created_userid,datemodified = @datemodified,modified_userid = @modified_userid WHERE file_id = @file_id";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				MySqlCommand cmd = Utils.GetCommand(updatesql, conn);
                BuildSqlParameters(cmd,o, false);
                cmd.ExecuteNonQuery();
            }
		}

        public static void Delete(int file_id)
        {
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = Utils.GetCommand("DELETE FROM after_sales_enquiry_files WHERE file_id = @id", conn);
                cmd.Parameters.AddWithValue("@id", file_id);
                cmd.ExecuteNonQuery();
            }
        }
	}
}
			
			