
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace erp.Model.DAL
{
    public class Issue_subscriptionDAL
	{
	
		public static List<Issue_subscription> GetForIssue(int issue_id, bool loadDependents = true)
		{
			List<Issue_subscription> result = new List<Issue_subscription>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = Utils.GetCommand("SELECT * FROM issue_subscription WHERE issue_id = @issue_id", conn);
                cmd.Parameters.AddWithValue("@issue_id", issue_id);
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
                if (loadDependents)
                {
                    foreach (var item in result)
                    {
                        if (item.user_id != null)
                            item.User = UserDAL.GetById(item.user_id.Value);
                        //TODO: Groups
                        //if(item.group_id != null)
                            
                    }
                }
            }
            return result;
		}
		
		
		public static Issue_subscription GetById(int id)
		{
			Issue_subscription result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = Utils.GetCommand("SELECT * FROM issue_subscription WHERE subs_id = @id", conn);
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
	
		private static Issue_subscription GetFromDataReader(MySqlDataReader dr)
		{
			Issue_subscription o = new Issue_subscription();
		
			o.subs_id =  (int) dr["subs_id"];
			o.issue_id = (int) dr["issue_id"];
			o.user_id = Utilities.FromDbValue<int>(dr["user_id"]);
			o.group_id = Utilities.FromDbValue<int>(dr["group_id"]);
			
			return o;

		}
		
		public static void Create(Issue_subscription o)
        {
            string insertsql = @"INSERT INTO issue_subscription (issue_id,user_id,group_id) VALUES(@issue_id,@user_id,@group_id)";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				
				MySqlCommand cmd = Utils.GetCommand(insertsql, conn);
                BuildSqlParameters(cmd,o);
                cmd.ExecuteNonQuery();
				cmd.CommandText = "SELECT subs_id FROM issue_subscription WHERE subs_id = LAST_INSERT_ID()";
                o.subs_id = (int) cmd.ExecuteScalar();
				
            }
		}
		
		private static void BuildSqlParameters(MySqlCommand cmd, Issue_subscription o, bool forInsert = true)
        {
			
			if(!forInsert)
				cmd.Parameters.AddWithValue("@subs_id", o.subs_id);
			cmd.Parameters.AddWithValue("@issue_id", o.issue_id);
			cmd.Parameters.AddWithValue("@user_id", o.user_id);
			cmd.Parameters.AddWithValue("@group_id", o.group_id);
		}
		
		public static void Update(Issue_subscription o)
		{
			string updatesql = @"UPDATE issue_subscription SET issue_id = @issue_id,user_id = @user_id,group_id = @group_id WHERE subs_id = @subs_id";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				MySqlCommand cmd = Utils.GetCommand(updatesql, conn);
                BuildSqlParameters(cmd,o, false);
                cmd.ExecuteNonQuery();
            }
		}
		
		public static void Delete(int subs_id)
		{
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				MySqlCommand cmd = Utils.GetCommand("DELETE FROM issue_subscription WHERE subs_id = @id" , conn);
                cmd.Parameters.AddWithValue("@id", subs_id);
                cmd.ExecuteNonQuery();
            }
		}
	}
}
			
			