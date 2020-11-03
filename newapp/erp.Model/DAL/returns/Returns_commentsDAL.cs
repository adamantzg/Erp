
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace asaq2.Model
{
    public class Returns_commentsDAL
	{
	
		public static List<Returns_comments> GetAll()
		{
			var result = new List<Returns_comments>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT * FROM returns_comments", conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}

        public static List<Returns_comments> GetByReturn(int return_id)
        {
            var result = new List<Returns_comments>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT * FROM returns_comments INNER JOIN userusers ON returns_comments.comments_from = userusers.useruserid WHERE return_id = @return_id", conn);
                cmd.Parameters.AddWithValue("@return_id", return_id);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    var comment = GetFromDataReader(dr);
                    comment.Creator = UserDAL.GetFromDataReader(dr);
                    comment.Files = Returns_comments_filesDAL.GetForComment(comment.comments_id);
                    result.Add(comment);
                }
                dr.Close();
            }
            return result;
        }
		
		
		public static Returns_comments GetById(int id)
		{
			Returns_comments result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT * FROM returns_comments WHERE comments_id = @id", conn);
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
		
	
		private static Returns_comments GetFromDataReader(MySqlDataReader dr)
		{
			Returns_comments o = new Returns_comments();
		
			o.comments_id =  (int) dr["comments_id"];
			o.return_id = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"return_id"));
			o.comments_type = string.Empty + Utilities.GetReaderField(dr,"comments_type");
			o.comments_from = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"comments_from"));
			o.comments_to = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"comments_to"));
			o.comments = string.Empty + Utilities.GetReaderField(dr,"comments");
			o.comments_date = Utilities.FromDbValue<DateTime>(Utilities.GetReaderField(dr,"comments_date"));
			o.decision_flag = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"decision_flag"));
			o.fc_response = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"fc_response"));
			
			return o;

		}
		
		
		public static void Create(Returns_comments o)
        {
            string insertsql = @"INSERT INTO returns_comments (return_id,comments_type,comments_from,comments_to,comments,comments_date,decision_flag,fc_response) VALUES(@return_id,@comments_type,@comments_from,@comments_to,@comments,@comments_date,@decision_flag,@fc_response)";

		    var conn = new MySqlConnection(Properties.Settings.Default.ConnString);
            conn.Open();
		    var tr = conn.BeginTransaction();

		    try
		    {
                
		        MySqlCommand cmd = new MySqlCommand(insertsql, conn, tr);
		        BuildSqlParameters(cmd, o);
		        cmd.ExecuteNonQuery();
		        cmd.CommandText = "SELECT comments_id FROM returns_comments WHERE comments_id = LAST_INSERT_ID()";
		        o.comments_id = (int) cmd.ExecuteScalar();

		        if (o.Files != null)
		        {
                    foreach (var file in o.Files)
                    {
                        file.return_comment_id = o.comments_id;
                        Returns_comments_filesDAL.Create(file, tr);
                    }
		        }
		        tr.Commit();
		    }
		    catch (Exception)
		    {
		        tr.Rollback();
		        throw;
		    }
		    finally
		    {
		        tr = null;
		        conn = null;
		    }


        }
		
		private static void BuildSqlParameters(MySqlCommand cmd, Returns_comments o, bool forInsert = true)
        {
			
			if(!forInsert)
				cmd.Parameters.AddWithValue("@comments_id", o.comments_id);
			cmd.Parameters.AddWithValue("@return_id", o.return_id);
			cmd.Parameters.AddWithValue("@comments_type", o.comments_type);
			cmd.Parameters.AddWithValue("@comments_from", o.comments_from);
			cmd.Parameters.AddWithValue("@comments_to", o.comments_to);
			cmd.Parameters.AddWithValue("@comments", o.comments);
			cmd.Parameters.AddWithValue("@comments_date", o.comments_date);
			cmd.Parameters.AddWithValue("@decision_flag", o.decision_flag);
			cmd.Parameters.AddWithValue("@fc_response", o.fc_response);
		}
		
		public static void Update(Returns_comments o)
		{
			string updatesql = @"UPDATE returns_comments SET return_id = @return_id,comments_type = @comments_type,comments_from = @comments_from,comments_to = @comments_to,comments = @comments,comments_date = @comments_date,decision_flag = @decision_flag,fc_response = @fc_response WHERE comments_id = @comments_id";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				MySqlCommand cmd = new MySqlCommand(updatesql, conn);
                BuildSqlParameters(cmd,o, false);
                cmd.ExecuteNonQuery();
            }
		}
		
		public static void Delete(int comments_id)
		{
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = new MySqlCommand("DELETE FROM returns_comments WHERE comments_id = @id" , conn);
                cmd.Parameters.AddWithValue("@id", comments_id);
                cmd.ExecuteNonQuery();
            }
		}
		
		
	}
}
			
			