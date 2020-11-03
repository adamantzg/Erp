
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace asaq2.Model
{
    public class Issuecomment_filesDAL
	{
	
		public static List<Issuecomment_files> GetForComment(int comment_id)
		{
			List<Issuecomment_files> result = new List<Issuecomment_files>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM issuecomment_files WHERE comment_id = @comment_id", conn);
                cmd.Parameters.AddWithValue("@comment_id", comment_id);
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}
		
		
		public static Issuecomment_files GetById(int id)
		{
			Issuecomment_files result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM issuecomment_files WHERE id = @id", conn);
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
	
		private static Issuecomment_files GetFromDataReader(MySqlDataReader dr)
		{
			Issuecomment_files o = new Issuecomment_files();
		
			o.id =  (int) dr["id"];
			o.comment_id = Utilities.FromDbValue<int>(dr["comment_id"]);
			o.filename = string.Empty + dr["filename"];
			
			return o;

		}
	}
}
			
			