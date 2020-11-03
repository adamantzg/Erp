
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace erp.Model.DAL
{
    public class Npd_commentsDAL
	{
	
		public static List<Npd_comments> GetAll()
		{
			var result = new List<Npd_comments>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM npd_comments", conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}

        public static List<Npd_comments> GetForNpd(int npd_id)
        {
            var result = new List<Npd_comments>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand(@"SELECT npd_comments.*,userusers.*,npd_comments_files.* 
                                            FROM npd_comments_files RIGHT OUTER JOIN npd_comments ON npd_comments.comments_id = npd_comments_files.npd_comment_id 
                                            INNER JOIN userusers ON npd_comments.comments_from = userusers.useruserid 
                                             WHERE npd_id = @npd_id", conn);
                cmd.Parameters.AddWithValue("@npd_id", npd_id);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    var id = (int) dr["comments_id"];
                    var c = result.FirstOrDefault(r => r.comments_id == id);
                    if (c == null)
                    {
                        c = GetFromDataReader(dr);
                        c.FromUser = UserDAL.GetFromDataReader(dr);
                        result.Add(c);
                        c.Files = new List<Npd_comments_files>();
                    }

                    var file_id = dr["file_id"];
                    if(file_id != DBNull.Value)
                        c.Files.Add(Npd_comments_filesDAL.GetFromDataReader(dr));

                }
                dr.Close();
            }
            return result;
        }
		
		
		public static Npd_comments GetById(int id)
		{
			Npd_comments result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM npd_comments WHERE comments_id = @id", conn);
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
		
	
		private static Npd_comments GetFromDataReader(MySqlDataReader dr)
		{
			Npd_comments o = new Npd_comments();
		
			o.comments_id =  (int) dr["comments_id"];
			o.npd_id = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"npd_id"));
			o.comments_from = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"comments_from"));
			o.comments = string.Empty + Utilities.GetReaderField(dr,"comments");
			o.comments_date = Utilities.FromDbValue<DateTime>(Utilities.GetReaderField(dr,"comments_date"));
		    o.type = Utilities.FromDbValue<int>(dr["type"]);
			
			return o;

		}
		
		
		public static void Create(Npd_comments o, DbTransaction tr = null)
        {
            string insertsql = @"INSERT INTO npd_comments (npd_id,comments_from,comments,comments_date,type) VALUES(@npd_id,@comments_from,@comments,@comments_date,@type)";

		    var conn = tr == null ? new MySqlConnection(Properties.Settings.Default.ConnString) : (MySqlConnection) tr.Connection;
            if(tr == null)
                conn.Open();
            MySqlCommand cmd = Utils.GetCommand(insertsql, conn,(MySqlTransaction) tr);
            BuildSqlParameters(cmd,o);
            cmd.ExecuteNonQuery();
			cmd.CommandText = "SELECT comments_id FROM npd_comments WHERE comments_id = LAST_INSERT_ID()";
            o.comments_id = (int) cmd.ExecuteScalar();

            if(o.Files != null)
            {
                foreach (var file in o.Files)
                {
                    file.npd_comment_id = o.comments_id;
                    Npd_comments_filesDAL.Create(file,(MySqlTransaction) tr);
                }
            }
		}
		
		private static void BuildSqlParameters(MySqlCommand cmd, Npd_comments o, bool forInsert = true)
        {
			
			if(!forInsert)
				cmd.Parameters.AddWithValue("@comments_id", o.comments_id);
			cmd.Parameters.AddWithValue("@npd_id", o.npd_id);
			cmd.Parameters.AddWithValue("@comments_from", o.comments_from);
			cmd.Parameters.AddWithValue("@comments", o.comments);
			cmd.Parameters.AddWithValue("@comments_date", o.comments_date);
		    cmd.Parameters.AddWithValue("@type", o.type);
        }
		
		public static void Update(Npd_comments o, MySqlTransaction tr = null)
		{
			string updatesql = @"UPDATE npd_comments SET npd_id = @npd_id,comments_from = @comments_from,comments = @comments,comments_date = @comments_date,type=@type WHERE comments_id = @comments_id";

		    var conn = tr == null ? new MySqlConnection(Properties.Settings.Default.ConnString) : tr.Connection;
            if(tr == null)
            conn.Open();
			var cmd = Utils.GetCommand(updatesql, conn,tr);
            BuildSqlParameters(cmd,o, false);
            cmd.ExecuteNonQuery();
		}
		
		public static void Delete(int comments_id)
		{
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = Utils.GetCommand("DELETE FROM npd_comments WHERE comments_id = @id" , conn);
                cmd.Parameters.AddWithValue("@id", comments_id);
                cmd.ExecuteNonQuery();
            }
		}
		
		
	}
}
			
			