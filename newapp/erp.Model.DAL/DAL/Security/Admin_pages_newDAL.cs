
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using erp.Model;
using MySql.Data.MySqlClient;

namespace erp.Model.DAL
{
    public class Admin_pages_newDAL
	{
	
		public static List<Admin_pages_new> GetAll()
		{
			var result = new List<Admin_pages_new>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM admin_pages_new", conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}

        public static List<Admin_pages_new> GetForUser(int user_id)
        {
            var result = new List<Admin_pages_new>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT admin_pages_new.* FROM admin_pages_new INNER JOIN user_pages ON admin_pages_new.page_id = user_pages.page_id WHERE userid = @user_id ORDER BY admin_pages_new.page_id", conn);
                cmd.Parameters.AddWithValue("@user_id", user_id);
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
                //foreach (var page in result)
                //{
                    
                //}
                conn.Close();
            }
            return result;
        }

        private static void GetParent(List<Admin_pages_new> list, int? parent_id, MySqlConnection conn)
        {
            if (parent_id == null) return;
            var cmd = Utils.GetCommand("SELECT admin_pages_new.* FROM admin_pages_new WHERE page_id = @parent_id",
                                       conn);
            cmd.Parameters.AddWithValue("@parent_id", parent_id);
            var dr = cmd.ExecuteReader();
            if (dr.Read())
            {
                var page = GetFromDataReader(dr);
                list.Add(page);
                GetParent(list, page.parent_id, conn);
            }
            dr.Close();
        }


        public static Admin_pages_new GetById(int id)
		{
			Admin_pages_new result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM admin_pages_new WHERE page_id = @id", conn);
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
		
	
		public static Admin_pages_new GetFromDataReader(MySqlDataReader dr)
		{
			var o = new Admin_pages_new();
		
			o.page_id =  (int) dr["page_id"];
			o.parent_id = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"parent_id"));
			o.page_title = string.Empty + Utilities.GetReaderField(dr,"page_title");
			o.page_type = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"page_type"));
			o.notes = string.Empty + Utilities.GetReaderField(dr,"notes");
			o.page_URL = string.Empty + Utilities.GetReaderField(dr,"page_URL");
			o.parameter1 = string.Empty + Utilities.GetReaderField(dr,"parameter1");
			o.parameter1_value = string.Empty + Utilities.GetReaderField(dr,"parameter1_value");
			o.URL_value = string.Empty + Utilities.GetReaderField(dr,"URL_value");
			o.hide_menu = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"hide_menu")) != null ? (bool?) Convert.ToBoolean(Utilities.GetReaderField(dr,"hide_menu")) : null;
			o.path = string.Empty + Utilities.GetReaderField(dr,"path");
		    o.icon = string.Empty + dr["icon"];
			return o;

		}
		
		
		public static void Create(Admin_pages_new o)
        {
            string insertsql = @"INSERT INTO admin_pages_new (page_id,parent_id,page_title,page_type,notes,page_URL,parameter1,parameter1_value,URL_value,hide_menu,path) VALUES(@page_id,@parent_id,@page_title,@page_type,@notes,@page_URL,@parameter1,@parameter1_value,@URL_value,@hide_menu,@path)";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				
                var cmd = Utils.GetCommand("SELECT MAX(page_id)+1 FROM admin_pages_new", conn);
                o.page_id = Convert.ToInt32(cmd.ExecuteScalar());
				cmd.CommandText = insertsql;
				
                BuildSqlParameters(cmd,o);
                cmd.ExecuteNonQuery();
				
            }
		}
		
		private static void BuildSqlParameters(MySqlCommand cmd, Admin_pages_new o, bool forInsert = true)
        {
			
			cmd.Parameters.AddWithValue("@page_id", o.page_id);
			cmd.Parameters.AddWithValue("@parent_id", o.parent_id);
			cmd.Parameters.AddWithValue("@page_title", o.page_title);
			cmd.Parameters.AddWithValue("@page_type", o.page_type);
			cmd.Parameters.AddWithValue("@notes", o.notes);
			cmd.Parameters.AddWithValue("@page_URL", o.page_URL);
			cmd.Parameters.AddWithValue("@parameter1", o.parameter1);
			cmd.Parameters.AddWithValue("@parameter1_value", o.parameter1_value);
			cmd.Parameters.AddWithValue("@URL_value", o.URL_value);
			cmd.Parameters.AddWithValue("@hide_menu", o.hide_menu);
			cmd.Parameters.AddWithValue("@path", o.path);
		}
		
		public static void Update(Admin_pages_new o)
		{
			string updatesql = @"UPDATE admin_pages_new SET parent_id = @parent_id,page_title = @page_title,page_type = @page_type,notes = @notes,page_URL = @page_URL,parameter1 = @parameter1,parameter1_value = @parameter1_value,URL_value = @URL_value,hide_menu = @hide_menu,path = @path WHERE page_id = @page_id";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = Utils.GetCommand(updatesql, conn);
                BuildSqlParameters(cmd,o, false);
                cmd.ExecuteNonQuery();
            }
		}
		
		public static void Delete(int page_id)
		{
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = Utils.GetCommand("DELETE FROM admin_pages_new WHERE page_id = @id" , conn);
                cmd.Parameters.AddWithValue("@id", page_id);
                cmd.ExecuteNonQuery();
            }
		}
		
		
	}
}
			
			