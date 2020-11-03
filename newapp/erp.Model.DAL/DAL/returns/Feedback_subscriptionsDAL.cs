
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace erp.Model.DAL
{
    public class Feedback_subscriptionsDAL
	{
	
		public static List<Feedback_subscriptions> GetAll()
		{
			var result = new List<Feedback_subscriptions>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM feedback_subscriptions", conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}

        public static List<Feedback_subscriptions> GetForReturn(int return_id)
        {
            var result = new List<Feedback_subscriptions>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand(@"SELECT * FROM feedback_subscriptions INNER JOIN userusers ON feedback_subscriptions.subs_useruserid = userusers.useruserid 
                                        WHERE subs_returnid = @return_id", conn);
                cmd.Parameters.AddWithValue("@return_id", return_id);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    var sub = GetFromDataReader(dr);
                    sub.User = UserDAL.GetFromDataReader(dr);
                    result.Add(sub);
                }
                dr.Close();
            }
            return result;
        }
		
		
		public static Feedback_subscriptions GetById(int id)
		{
			Feedback_subscriptions result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM feedback_subscriptions WHERE subs_id = @id", conn);
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
		
	
		private static Feedback_subscriptions GetFromDataReader(MySqlDataReader dr)
		{
			Feedback_subscriptions o = new Feedback_subscriptions();
		
			o.subs_id =  (int) dr["subs_id"];
			o.subs_returnid = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"subs_returnid"));
			o.subs_useruserid = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"subs_useruserid"));
			o.subs_type = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"subs_type"));
			o.subs_leader = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"subs_leader"));
			
			return o;

		}
		
		
		public static void Create(Feedback_subscriptions o,DbTransaction tr=null)
        {
            string insertsql = @"INSERT INTO feedback_subscriptions (subs_returnid,subs_useruserid,subs_type,subs_leader) VALUES(@subs_returnid,@subs_useruserid,@subs_type,@subs_leader)";

			var conn = tr != null ? tr.Connection : new MySqlConnection(Properties.Settings.Default.ConnString);
			if(tr == null)
                conn.Open();
							
			var cmd = Utils.GetCommand(insertsql, (MySqlConnection) conn, (MySqlTransaction) tr);
            BuildSqlParameters(cmd,o);
            cmd.ExecuteNonQuery();
			cmd.CommandText = "SELECT subs_id FROM feedback_subscriptions WHERE subs_id = LAST_INSERT_ID()";
            o.subs_id = (int) cmd.ExecuteScalar();
            
		}
		
		private static void BuildSqlParameters(MySqlCommand cmd, Feedback_subscriptions o, bool forInsert = true)
        {
			
			if(!forInsert)
				cmd.Parameters.AddWithValue("@subs_id", o.subs_id);
			cmd.Parameters.AddWithValue("@subs_returnid", o.subs_returnid);
			cmd.Parameters.AddWithValue("@subs_useruserid", o.subs_useruserid);
			cmd.Parameters.AddWithValue("@subs_type", o.subs_type);
			cmd.Parameters.AddWithValue("@subs_leader", o.subs_leader);
		}
		
		public static void Update(Feedback_subscriptions o)
		{
			string updatesql = @"UPDATE feedback_subscriptions SET subs_returnid = @subs_returnid,subs_useruserid = @subs_useruserid,subs_type = @subs_type,subs_leader = @subs_leader WHERE subs_id = @subs_id";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = Utils.GetCommand(updatesql, conn);
                BuildSqlParameters(cmd,o, false);
                cmd.ExecuteNonQuery();
            }
		}
		
		public static void Delete(int subs_id)
		{
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = Utils.GetCommand("DELETE FROM feedback_subscriptions WHERE subs_id = @id" , conn);
                cmd.Parameters.AddWithValue("@id", subs_id);
                cmd.ExecuteNonQuery();
            }
		}
		
		
	}
}
			
			