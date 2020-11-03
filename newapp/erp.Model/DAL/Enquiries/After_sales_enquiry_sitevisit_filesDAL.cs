
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace asaq2.Model
{
    public class After_sales_enquiry_sitevisit_filesDAL
	{
	
		public static List<After_sales_enquiry_sitevisit_files> GetAll()
		{
			List<After_sales_enquiry_sitevisit_files> result = new List<After_sales_enquiry_sitevisit_files>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM after_sales_enquiry_sitevisit_files", conn);
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}
		
		
		public static After_sales_enquiry_sitevisit_files GetById(int id)
		{
			After_sales_enquiry_sitevisit_files result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM after_sales_enquiry_sitevisit_files WHERE file_id = @id", conn);
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
	
		private static After_sales_enquiry_sitevisit_files GetFromDataReader(MySqlDataReader dr)
		{
			After_sales_enquiry_sitevisit_files o = new After_sales_enquiry_sitevisit_files();
		
			o.file_id =  (int) dr["file_id"];
			o.filename = string.Empty + dr["filename"];
			o.sitevisit_id = Utilities.FromDbValue<int>(dr["sitevisit_id"]);
			o.datecreated = Utilities.FromDbValue<DateTime>(dr["datecreated"]);
			o.created_userid = Utilities.FromDbValue<int>(dr["created_userid"]);
			o.datemodified = Utilities.FromDbValue<DateTime>(dr["datemodified"]);
			o.modified_userid = Utilities.FromDbValue<int>(dr["modified_userid"]);
			
			return o;

		}
		
		public static void Create(After_sales_enquiry_sitevisit_files o, MySqlTransaction tr = null)
        {
            string insertsql = @"INSERT INTO after_sales_enquiry_sitevisit_files (filename,sitevisit_id,datecreated,created_userid,datemodified,modified_userid) VALUES(@filename,@sitevisit_id,@datecreated,@created_userid,@datemodified,@modified_userid)";

            MySqlConnection conn;
            if (tr != null)
            {
                conn = tr.Connection;
            }
            else
            {
                conn = new MySqlConnection();
                conn.Open();
            }

            MySqlCommand cmd = new MySqlCommand(insertsql, conn);
            if (tr != null)
                cmd.Transaction = tr;
            BuildSqlParameters(cmd, o);
            cmd.ExecuteNonQuery();
            cmd.CommandText = "SELECT file_id FROM after_sales_enquiry_sitevisit_files WHERE file_id = LAST_INSERT_ID()";
            o.file_id = (int)cmd.ExecuteScalar();

            if (tr == null)
                conn = null;
		}
		
		private static void BuildSqlParameters(MySqlCommand cmd, After_sales_enquiry_sitevisit_files o, bool forInsert = true)
        {
			
            if(!forInsert)
			    cmd.Parameters.AddWithValue("@file_id", o.file_id);
			cmd.Parameters.AddWithValue("@filename", o.filename);
			cmd.Parameters.AddWithValue("@sitevisit_id", o.sitevisit_id);
			cmd.Parameters.AddWithValue("@datecreated", o.datecreated);
			cmd.Parameters.AddWithValue("@created_userid", o.created_userid);
			cmd.Parameters.AddWithValue("@datemodified", o.datemodified);
			cmd.Parameters.AddWithValue("@modified_userid", o.modified_userid);
		}
		
		public static void Update(After_sales_enquiry_sitevisit_files o)
		{
			string updatesql = @"UPDATE after_sales_enquiry_sitevisit_files SET filename = @filename,sitevisit_id = @sitevisit_id,datecreated = @datecreated,created_userid = @created_userid,datemodified = @datemodified,modified_userid = @modified_userid WHERE file_id = @file_id";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				MySqlCommand cmd = new MySqlCommand(updatesql, conn);
                BuildSqlParameters(cmd,o, false);
                cmd.ExecuteNonQuery();
            }
		}
		
		public static void Delete(int file_id)
		{
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				MySqlCommand cmd = new MySqlCommand("DELETE FROM after_sales_enquiry_sitevisit_files WHERE file_id = @id" , conn);
                cmd.Parameters.AddWithValue("@id", file_id);
                cmd.ExecuteNonQuery();
            }
		}

        public static List<After_sales_enquiry_sitevisit_files> GetForVisit(int sitevisit_id)
        {
            List<After_sales_enquiry_sitevisit_files> result = new List<After_sales_enquiry_sitevisit_files>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM after_sales_enquiry_sitevisit_files WHERE sitevisit_id = @sitevisit_id", conn);
                cmd.Parameters.AddWithValue("@sitevisit_id", sitevisit_id);
                MySqlDataReader dr = cmd.ExecuteReader();
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
			
			