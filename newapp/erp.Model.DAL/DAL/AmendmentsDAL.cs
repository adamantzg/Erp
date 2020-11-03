
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace erp.Model.DAL
{
    public class AmendmentsDAL
	{
		public static List<Amendments> GetAll()
		{
			List<Amendments> result = new List<Amendments>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = Utils.GetCommand("SELECT * FROM amendments", conn);
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}

        public static List<Amendments> GetByUserName(string username)
        {
            List<Amendments> result = new List<Amendments>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = Utils.GetCommand("SELECT * FROM amendments WHERE userid=@username", conn);
                cmd.Parameters.AddWithValue("@username", username);
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
        }
		
		
		public static Amendments GetById(int id)
		{
			Amendments result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = Utils.GetCommand("SELECT * FROM amendments WHERE processid = @id", conn);
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

        public static List<Amendments> GetByCriteria(string processName)
        {
            var result = new List<Amendments>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString)) {
                conn.Open();
                MySqlCommand cmd = Utils.GetCommand("SELECT * FROM amendments WHERE process = @name", conn);
                cmd.Parameters.AddWithValue("@name", processName);
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read()) {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
        }
	
		private static Amendments GetFromDataReader(MySqlDataReader dr)
		{
			Amendments o = new Amendments();
		
			o.processid =  (int) dr["processid"];
			o.userid = string.Empty + dr["userid"];
			o.timedate = Utilities.FromDbValue<DateTime>(dr["timedate"]);
			o.tablea = string.Empty + dr["tablea"];
			o.ref1 = string.Empty + dr["ref1"];
			o.ref2 = string.Empty + dr["ref2"];
			o.old_data = string.Empty + dr["old_data"];
			o.new_data = string.Empty + dr["new_data"];
			o.process = string.Empty + dr["process"];
			o._checked = Utilities.FromDbValue<int>(dr["checked"]);
			o.checked_user = Utilities.FromDbValue<int>(dr["checked_user"]);
			o.checked_date = Utilities.FromDbValue<DateTime>(dr["checked_date"]);
			o.reason = Utilities.FromDbValue<int>(dr["reason"]);
			
			return o;

		}
		
		public static void Create(Amendments o)
        {
            string insertsql = @"INSERT INTO amendments (userid,timedate,tablea,ref1,ref2,old_data,new_data,process,checked,checked_user,checked_date,reason) VALUES(@userid,@timedate,@tablea,@ref1,@ref2,@old_data,@new_data,@process,@checked,@checked_user,@checked_date,@reason)";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				
				MySqlCommand cmd = Utils.GetCommand(insertsql, conn);
                BuildSqlParameters(cmd,o);
                cmd.ExecuteNonQuery();
				cmd.CommandText = "SELECT processid FROM amendments WHERE processid = LAST_INSERT_ID()";
                o.processid = (int) cmd.ExecuteScalar();
				
            }
		}
		
		private static void BuildSqlParameters(MySqlCommand cmd, Amendments o, bool forInsert = true)
        {
			
			if(!forInsert)
				cmd.Parameters.AddWithValue("@processid", o.processid);
			cmd.Parameters.AddWithValue("@userid", o.userid);
			cmd.Parameters.AddWithValue("@timedate", o.timedate);
			cmd.Parameters.AddWithValue("@tablea", o.tablea);
			cmd.Parameters.AddWithValue("@ref1", o.ref1);
			cmd.Parameters.AddWithValue("@ref2", o.ref2);
			cmd.Parameters.AddWithValue("@old_data", o.old_data);
			cmd.Parameters.AddWithValue("@new_data", o.new_data);
			cmd.Parameters.AddWithValue("@process", o.process);
			cmd.Parameters.AddWithValue("@checked", o._checked);
			cmd.Parameters.AddWithValue("@checked_user", o.checked_user);
			cmd.Parameters.AddWithValue("@checked_date", o.checked_date);
			cmd.Parameters.AddWithValue("@reason", o.reason);
		}
		
		public static void Update(Amendments o)
		{
			string updatesql = @"UPDATE amendments SET userid = @userid,timedate = @timedate,tablea = @tablea,ref1 = @ref1,ref2 = @ref2,old_data = @old_data,new_data = @new_data,process = @process,checked = @checked,checked_user = @checked_user,checked_date = @checked_date,reason = @reason WHERE processid = @processid";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				MySqlCommand cmd = Utils.GetCommand(updatesql, conn);
                BuildSqlParameters(cmd,o, false);
                cmd.ExecuteNonQuery();
            }
		}
		
		public static void Delete(int processid)
		{
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				MySqlCommand cmd = Utils.GetCommand("DELETE FROM amendments WHERE processid = @id" , conn);
                cmd.Parameters.AddWithValue("@id", processid);
                cmd.ExecuteNonQuery();
            }
		}
	}
}
			
			