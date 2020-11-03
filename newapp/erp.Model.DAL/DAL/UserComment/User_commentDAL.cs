
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using erp.Model;
using MySql.Data.MySqlClient;

namespace erp.Model.DAL
{
    public class User_commentDAL
	{
	
		public static List<User_comment> GetAll()
		{
			var result = new List<User_comment>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM user_comment", conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}

        public static List<User_comment> GetForDealer(int dealer_id)
        {
            var result = new List<User_comment>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT user_comment.*,userusers.userwelcome FROM user_comment INNER JOIN userusers ON user_Comment.user_id = userusers.useruserid WHERE dealer_id = @dealer_id", conn);
                cmd.Parameters.AddWithValue("@dealer_id", dealer_id);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    var c = GetFromDataReader(dr);
                    c.User = new User {userwelcome = string.Empty + dr["userwelcome"]};
                    result.Add(c);
                }
                dr.Close();
            }
            return result;
        }
		
		
		public static User_comment GetById(int id)
		{
			User_comment result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM user_comment WHERE comment_id = @id", conn);
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
		
	
		public static User_comment GetFromDataReader(MySqlDataReader dr)
		{
			User_comment o = new User_comment();
		
			o.comment_id =  (int) dr["comment_id"];
			o.type_id =  (int) dr["type_id"];
			o.date = Utilities.FromDbValue<DateTime>(Utilities.GetReaderField(dr,"date"));
			o.user_id = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"user_id"));
			o.text = string.Empty + Utilities.GetReaderField(dr,"text");
			o.dealer_id = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"dealer_id"));
			
			return o;

		}
		
		
		public static void Create(User_comment o)
        {
            string insertsql = @"INSERT INTO user_comment (type_id,date,user_id,text,dealer_id) VALUES(@type_id,@date,@user_id,@text,@dealer_id)";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				
				var cmd = Utils.GetCommand(insertsql, conn);
                BuildSqlParameters(cmd,o);
                cmd.ExecuteNonQuery();
				cmd.CommandText = "SELECT comment_id FROM user_comment WHERE comment_id = LAST_INSERT_ID()";
                o.comment_id = (int) cmd.ExecuteScalar();
				
            }
		}
		
		private static void BuildSqlParameters(MySqlCommand cmd, User_comment o, bool forInsert = true)
        {
			
			if(!forInsert)
				cmd.Parameters.AddWithValue("@comment_id", o.comment_id);
			cmd.Parameters.AddWithValue("@type_id", o.type_id);
			cmd.Parameters.AddWithValue("@date", o.date);
			cmd.Parameters.AddWithValue("@user_id", o.user_id);
			cmd.Parameters.AddWithValue("@text", o.text);
			cmd.Parameters.AddWithValue("@dealer_id", o.dealer_id);
		}
		
		public static void Update(User_comment o)
		{
			string updatesql = @"UPDATE user_comment SET type_id = @type_id,date = @date,user_id = @user_id,text = @text,dealer_id = @dealer_id WHERE comment_id = @comment_id";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = Utils.GetCommand(updatesql, conn);
                BuildSqlParameters(cmd,o, false);
                cmd.ExecuteNonQuery();
            }
		}
		
		public static void Delete(int comment_id)
		{
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = Utils.GetCommand("DELETE FROM user_comment WHERE comment_id = @id" , conn);
                cmd.Parameters.AddWithValue("@id", comment_id);
                cmd.ExecuteNonQuery();
            }
		}
		
		
	}
}
			
			