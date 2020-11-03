
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace erp.Model.DAL.DAL
{
    public class Admin_pagesDAL
	{
	
		public static List<Admin_pages> GetAll()
		{
			List<Admin_pages> result = new List<Admin_pages>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = Utils.GetCommand("SELECT * FROM admin_pages", conn);
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}

        public static List<Admin_pages> GetForUser(int user_id)
        {
            List<Admin_pages> result = new List<Admin_pages>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = Utils.GetCommand("SELECT admin_pages.* FROM admin_pages INNER JOIN user_pages ON admin_pages.page_id = user_pages.page_id WHERE userid = @user_id", conn);
                cmd.Parameters.AddWithValue("@user_id", user_id);
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
        }
		
		
		public static Admin_pages GetById(int id)
		{
			Admin_pages result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = Utils.GetCommand("SELECT * FROM admin_pages WHERE page_id = @id", conn);
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
	
		private static Admin_pages GetFromDataReader(MySqlDataReader dr)
		{
			Admin_pages o = new Admin_pages();
		
			o.page_id =  (int) dr["page_id"];
			o.page_type = Utilities.FromDbValue<int>(dr["page_type"]);
			o.top_level = string.Empty + dr["top_level"];
			o.sub_level = string.Empty + dr["sub_level"];
			o.sub_sub_level = string.Empty + dr["sub_sub_level"];
			o.notes = string.Empty + dr["notes"];
			o.page_URL = string.Empty + dr["page_URL"];
			o.parameter1 = string.Empty + dr["parameter1"];
			o.parameter1_value = string.Empty + dr["parameter1_value"];
			o.URL_value = string.Empty + dr["URL_value"];
		    o.hide_menu = Utilities.BoolFromLong(dr["hide_menu"]);
			
			return o;

		}
		
		public static void Create(Admin_pages o)
        {
            string insertsql = @"INSERT INTO admin_pages (page_id,page_type,top_level,sub_level,sub_sub_level,notes,page_URL,parameter1,parameter1_value,URL_value) VALUES(@page_id,@page_type,@top_level,@sub_level,@sub_sub_level,@notes,@page_URL,@parameter1,@parameter1_value,@URL_value)";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				
                MySqlCommand cmd = Utils.GetCommand("SELECT MAX(page_id)+1 FROM admin_pages", conn);
                o.page_id = Convert.ToInt32(cmd.ExecuteScalar());
				cmd.CommandText = insertsql;
				
                BuildSqlParameters(cmd,o);
                cmd.ExecuteNonQuery();
				
            }
		}
		
		private static void BuildSqlParameters(MySqlCommand cmd, Admin_pages o, bool forInsert = true)
        {
			
			cmd.Parameters.AddWithValue("@page_id", o.page_id);
			cmd.Parameters.AddWithValue("@page_type", o.page_type);
			cmd.Parameters.AddWithValue("@top_level", o.top_level);
			cmd.Parameters.AddWithValue("@sub_level", o.sub_level);
			cmd.Parameters.AddWithValue("@sub_sub_level", o.sub_sub_level);
			cmd.Parameters.AddWithValue("@notes", o.notes);
			cmd.Parameters.AddWithValue("@page_URL", o.page_URL);
			cmd.Parameters.AddWithValue("@parameter1", o.parameter1);
			cmd.Parameters.AddWithValue("@parameter1_value", o.parameter1_value);
			cmd.Parameters.AddWithValue("@URL_value", o.URL_value);
		}
		
		public static void Update(Admin_pages o)
		{
			string updatesql = @"UPDATE admin_pages SET page_type = @page_type,top_level = @top_level,sub_level = @sub_level,sub_sub_level = @sub_sub_level,notes = @notes,page_URL = @page_URL,parameter1 = @parameter1,parameter1_value = @parameter1_value,URL_value = @URL_value WHERE page_id = @page_id";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				MySqlCommand cmd = Utils.GetCommand(updatesql, conn);
                BuildSqlParameters(cmd,o, false);
                cmd.ExecuteNonQuery();
            }
		}
		
		public static void Delete(int page_id)
		{
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				MySqlCommand cmd = Utils.GetCommand("DELETE FROM admin_pages WHERE page_id = @id" , conn);
                cmd.Parameters.AddWithValue("@id", page_id);
                cmd.ExecuteNonQuery();
            }
		}
	}
}
			
			