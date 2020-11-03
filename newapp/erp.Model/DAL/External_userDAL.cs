
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace asaq2.Model
{
    public class External_userDAL
	{
	
		public static List<External_user> GetAll()
		{
			List<External_user> result = new List<External_user>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(GetSelectSql(), conn);
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}

        private static string GetSelectSql()
        {
            return @"SELECT external_user.*,Creator.userwelcome AS creator, Editor.userwelcome AS editor 
                        FROM external_user
                        INNER JOIN userusers AS Creator ON Creator.useruserid = external_user.created_userid
                        LEFT JOIN userusers AS Editor ON Editor.useruserid = external_user.modified_userid";
        }
		
		
		public static External_user GetById(int id)
		{
			External_user result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(GetSelectSql() + " WHERE external_user.user_id = @id", conn);
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

        public static External_user GetByLoginData(string username, string password = "")
        {
            External_user result = null;
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(GetSelectSql() + " WHERE username = @username" + (password.Length > 0 ? " AND password = @password":""), conn);
                cmd.Parameters.AddWithValue("@username", username);
                if(password.Length > 0)
                    cmd.Parameters.AddWithValue("@password", password);
                MySqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    result = GetFromDataReader(dr);
                }
                dr.Close();
            }
            return result;
        }
	
		private static External_user GetFromDataReader(MySqlDataReader dr)
		{
			External_user o = new External_user();
		
			o.user_id =  (int) dr["user_id"];
			o.surname = string.Empty + dr["surname"];
			o.firstname = string.Empty + dr["firstname"];
			o.username = string.Empty + dr["username"];
			o.password = string.Empty + dr["password"];
			o.email = string.Empty + dr["email"];
			o.postcode = string.Empty + dr["postcode"];
			o.address1 = string.Empty + dr["address1"];
			o.address2 = string.Empty + dr["address2"];
			o.address3 = string.Empty + dr["address3"];
			o.address4 = string.Empty + dr["address4"];
			o.address5 = string.Empty + dr["address5"];
			o.country = string.Empty + dr["country"];
			o.city = Utilities.FromDbValue<int>(dr["city"]);
			o.tel = string.Empty + dr["tel"];
			o.mobile = string.Empty + dr["mobile"];
			o.company_id = Utilities.FromDbValue<int>(dr["company_id"]);
			o.datecreated = Utilities.FromDbValue<DateTime>(dr["datecreated"]);
			o.created_userid = Utilities.FromDbValue<int>(dr["created_userid"]);
			o.datemodified = Utilities.FromDbValue<DateTime>(dr["datemodified"]);
			o.modified_userid = Utilities.FromDbValue<int>(dr["modified_userid"]);
            if (Utilities.ColumnExists(dr, "creator"))
                o.creator = string.Empty + dr["creator"];
            if (Utilities.ColumnExists(dr, "editor"))
                o.editor = string.Empty + dr["editor"];
			return o;

		}
		
		public static void Create(External_user o)
        {
            string insertsql = @"INSERT INTO external_user (surname,firstname,username,password,email,postcode,address1,address2,address3,address4,address5,country,city,tel,mobile,company_id,datecreated,created_userid,datemodified,modified_userid) VALUES(@surname,@firstname,@username,@password,@email,@postcode,@address1,@address2,@address3,@address4,@address5,@country,@city,@tel,@mobile,@company_id,@datecreated,@created_userid,@datemodified,@modified_userid)";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				
				MySqlCommand cmd = new MySqlCommand(insertsql, conn);
                BuildSqlParameters(cmd,o);
                cmd.ExecuteNonQuery();
				cmd.CommandText = "SELECT user_id FROM external_user WHERE user_id = LAST_INSERT_ID()";
                o.user_id = (int) cmd.ExecuteScalar();
				
            }
		}
		
		private static void BuildSqlParameters(MySqlCommand cmd, External_user o, bool forInsert = true)
        {
			
			if(!forInsert)
				cmd.Parameters.AddWithValue("@user_id", o.user_id);
			cmd.Parameters.AddWithValue("@surname", o.surname);
			cmd.Parameters.AddWithValue("@firstname", o.firstname);
			cmd.Parameters.AddWithValue("@username", o.username);
			cmd.Parameters.AddWithValue("@password", o.password);
			cmd.Parameters.AddWithValue("@email", o.email);
			cmd.Parameters.AddWithValue("@postcode", o.postcode);
			cmd.Parameters.AddWithValue("@address1", o.address1);
			cmd.Parameters.AddWithValue("@address2", o.address2);
			cmd.Parameters.AddWithValue("@address3", o.address3);
			cmd.Parameters.AddWithValue("@address4", o.address4);
			cmd.Parameters.AddWithValue("@address5", o.address5);
			cmd.Parameters.AddWithValue("@country", o.country);
			cmd.Parameters.AddWithValue("@city", o.city);
			cmd.Parameters.AddWithValue("@tel", o.tel);
			cmd.Parameters.AddWithValue("@mobile", o.mobile);
			cmd.Parameters.AddWithValue("@company_id", o.company_id);
			cmd.Parameters.AddWithValue("@datecreated", o.datecreated);
			cmd.Parameters.AddWithValue("@created_userid", o.created_userid);
			cmd.Parameters.AddWithValue("@datemodified", o.datemodified);
			cmd.Parameters.AddWithValue("@modified_userid", o.modified_userid);
		}
		
		public static void Update(External_user o)
		{
			string updatesql = @"UPDATE external_user SET surname = @surname,firstname = @firstname,username = @username,password = @password,email = @email,postcode = @postcode,address1 = @address1,address2 = @address2,address3 = @address3,address4 = @address4,address5 = @address5,country = @country,city = @city,tel = @tel,mobile = @mobile,company_id = @company_id,datecreated = @datecreated,created_userid = @created_userid,datemodified = @datemodified,modified_userid = @modified_userid WHERE user_id = @user_id";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				MySqlCommand cmd = new MySqlCommand(updatesql, conn);
                BuildSqlParameters(cmd,o, false);
                cmd.ExecuteNonQuery();
            }
		}
		
		public static void Delete(int user_id)
		{
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				MySqlCommand cmd = new MySqlCommand("DELETE FROM external_user WHERE user_id = @id" , conn);
                cmd.Parameters.AddWithValue("@id", user_id);
                cmd.ExecuteNonQuery();
            }
		}
	}
}
			
			