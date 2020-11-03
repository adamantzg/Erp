
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace asaq2.Model
{
    public class Client_pages_allocatedDAL
	{
	
		public static List<Client_pages_allocated> GetAll()
		{
			List<Client_pages_allocated> result = new List<Client_pages_allocated>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM client_pages_allocated", conn);
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}

        public static List<Client_pages_allocated> GetByPageAndUser(int user_id, string page_Url = "")
        {
            List<Client_pages_allocated> result = new List<Client_pages_allocated>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(@"SELECT client_pages_allocated.userpage_id AS userpage_id,
                                    client_pages_allocated.userid AS userid,
                                    client_pages_allocated.page_id AS page_id,
                                    client_pages.page_type AS page_type,
                                    client_pages.top_level AS top_level,
                                    client_pages.sub_level AS sub_level,
                                    client_pages.sub_sub_level AS sub_sub_level,
                                    client_pages.notes AS notes,
                                    client_pages.page_URL AS page_URL,
                                    client_pages.parameter1 AS parameter1,
                                    client_pages.parameter1_value AS parameter1_value,
                                    client_pages.URL_value AS URL_value,
                                    client_pages_allocated.option1 AS option1
                                FROM
                                    client_pages_allocated
                                    INNER JOIN client_pages ON client_pages_allocated.page_id = client_pages.page_id
                                WHERE (page_URL = @page OR @page = '') AND userid = @user_id", conn);
                cmd.Parameters.AddWithValue("@page", page_Url);
                cmd.Parameters.AddWithValue("@user_id", user_id);
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Client_pages_allocated p = GetFromDataReader(dr);
                    p.Page = Client_pagesDAL.GetFromDataReader(dr);
                    result.Add(p);
                }
                dr.Close();
            }
            return result;
        }
		
		public static Client_pages_allocated GetById(int id)
		{
			Client_pages_allocated result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM client_pages_allocated WHERE userpage_id = @id", conn);
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
	
		private static Client_pages_allocated GetFromDataReader(MySqlDataReader dr)
		{
			Client_pages_allocated o = new Client_pages_allocated();
		
			o.userpage_id =  (int) dr["userpage_id"];
			o.userid = Utilities.FromDbValue<int>(dr["userid"]);
			o.page_id = Utilities.FromDbValue<int>(dr["page_id"]);
			o.option1 = Utilities.FromDbValue<int>(dr["option1"]);
			
			return o;

		}
		
		public static void Create(Client_pages_allocated o)
        {
            string insertsql = @"INSERT INTO client_pages_allocated (userid,page_id,option1) VALUES(@userid,@page_id,@option1)";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				
				MySqlCommand cmd = new MySqlCommand(insertsql, conn);
                BuildSqlParameters(cmd,o);
                cmd.ExecuteNonQuery();
				cmd.CommandText = "SELECT userpage_id FROM client_pages_allocated WHERE userpage_id = LAST_INSERT_ID()";
                o.userpage_id = (int) cmd.ExecuteScalar();
				
            }
		}
		
		private static void BuildSqlParameters(MySqlCommand cmd, Client_pages_allocated o, bool forInsert = true)
        {
			
			if(!forInsert)
				cmd.Parameters.AddWithValue("@userpage_id", o.userpage_id);
			cmd.Parameters.AddWithValue("@userid", o.userid);
			cmd.Parameters.AddWithValue("@page_id", o.page_id);
			cmd.Parameters.AddWithValue("@option1", o.option1);
		}
		
		public static void Update(Client_pages_allocated o)
		{
			string updatesql = @"UPDATE client_pages_allocated SET userid = @userid,page_id = @page_id,option1 = @option1 WHERE userpage_id = @userpage_id";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				MySqlCommand cmd = new MySqlCommand(updatesql, conn);
                BuildSqlParameters(cmd,o, false);
                cmd.ExecuteNonQuery();
            }
		}
		
		public static void Delete(int userpage_id)
		{
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				MySqlCommand cmd = new MySqlCommand("DELETE FROM client_pages_allocated WHERE userpage_id = @id" , conn);
                cmd.Parameters.AddWithValue("@id", userpage_id);
                cmd.ExecuteNonQuery();
            }
		}
	}
}
			
			