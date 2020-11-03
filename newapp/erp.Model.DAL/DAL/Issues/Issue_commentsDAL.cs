
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace erp.Model.DAL
{
    public class Issue_commentsDAL
	{
	
		public static List<Issue_comments> GetForIssue(int issue_id, bool includeFiles = true)
		{
			List<Issue_comments> result = new List<Issue_comments>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = Utils.GetCommand(@"SELECT issue_comments.*,  Creator.userwelcome AS creator
                                                    FROM issue_comments LEFT JOIN userusers AS Creator ON Creator.useruserid = issue_comments.created_userid
                                                    WHERE issue_id = @issue_id", conn);
                cmd.Parameters.AddWithValue("@issue_id", issue_id);
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Issue_comments c = GetFromDataReader(dr);
                    if (includeFiles)
                        c.Files = Issuecomment_filesDAL.GetForComment(c.comment_id);
                    result.Add(c);
                }
                dr.Close();
            }
            return result;
		}
		
		
		public static Issue_comments GetById(int id)
		{
			Issue_comments result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = Utils.GetCommand("SELECT * FROM issue_comments WHERE comment_id = @id", conn);
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
	
		private static Issue_comments GetFromDataReader(MySqlDataReader dr)
		{
			Issue_comments o = new Issue_comments();
		
			o.comment_id =  (int) dr["comment_id"];
			o.issue_id = Utilities.FromDbValue<int>(dr["issue_id"]);
			o.comment_text = string.Empty + dr["comment_text"];
			o.datecreated = Utilities.FromDbValue<DateTime>(dr["datecreated"]);
			o.created_userid = Utilities.FromDbValue<int>(dr["created_userid"]);
			o.datemodified = Utilities.FromDbValue<DateTime>(dr["datemodified"]);
			o.modified_userid = Utilities.FromDbValue<int>(dr["modified_userid"]);

            if (Utilities.ColumnExists(dr, "creator"))
                o.creator = string.Empty + dr["creator"];
			return o;

		}
	}
}
			
			