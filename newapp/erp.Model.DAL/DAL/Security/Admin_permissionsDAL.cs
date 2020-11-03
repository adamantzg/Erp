
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace erp.Model.DAL
{
    public class Admin_permissionsDAL
	{
	
		public static List<Admin_permissions> GetAll()
		{
			var result = new List<Admin_permissions>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM admin_permissions", conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}

        public static List<Admin_permissions> GetForCompany(int company_id)
        {
            var result = new List<Admin_permissions>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM admin_permissions INNER JOIN userusers ON admin_permissions.userid = userusers.useruserid WHERE cusid = @company_id", conn);
                cmd.Parameters.AddWithValue("@company_id", company_id);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    var adminPerm = GetFromDataReader(dr);
                    adminPerm.User = UserDAL.GetFromDataReader(dr);
                    result.Add(adminPerm);
                }
                dr.Close();
            }
            return result;
        }

         
		
		
		public static Admin_permissions GetById(int id)
		{
			Admin_permissions result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM admin_permissions WHERE permission_id = @id", conn);
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
		
	
		private static Admin_permissions GetFromDataReader(MySqlDataReader dr)
		{
			Admin_permissions o = new Admin_permissions();
		
			o.permission_id =  (int) dr["permission_id"];
			o.userid = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"userid"));
			o.cusid = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"cusid"));
			o.agent = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"agent"));
			o.clientid = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"clientid"));
			o.returns = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"returns"));
			o.processing = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"processing"));
			o.feedbacks = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"feedbacks"));
			
			return o;

		}
		
		
		public static void Create(Admin_permissions o)
        {
            string insertsql = @"INSERT INTO admin_permissions (userid,cusid,agent,clientid,returns,processing,feedbacks) VALUES(@userid,@cusid,@agent,@clientid,@returns,@processing,@feedbacks)";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				
				var cmd = Utils.GetCommand(insertsql, conn);
                BuildSqlParameters(cmd,o);
                cmd.ExecuteNonQuery();
				cmd.CommandText = "SELECT permission_id FROM admin_permissions WHERE permission_id = LAST_INSERT_ID()";
                o.permission_id = (int) cmd.ExecuteScalar();
				
            }
		}
		
		private static void BuildSqlParameters(MySqlCommand cmd, Admin_permissions o, bool forInsert = true)
        {
			
			if(!forInsert)
				cmd.Parameters.AddWithValue("@permission_id", o.permission_id);
			cmd.Parameters.AddWithValue("@userid", o.userid);
			cmd.Parameters.AddWithValue("@cusid", o.cusid);
			cmd.Parameters.AddWithValue("@agent", o.agent);
			cmd.Parameters.AddWithValue("@clientid", o.clientid);
			cmd.Parameters.AddWithValue("@returns", o.returns);
			cmd.Parameters.AddWithValue("@processing", o.processing);
			cmd.Parameters.AddWithValue("@feedbacks", o.feedbacks);
		}
		
		public static void Update(Admin_permissions o)
		{
			string updatesql = @"UPDATE admin_permissions SET userid = @userid,cusid = @cusid,agent = @agent,clientid = @clientid,returns = @returns,processing = @processing,feedbacks = @feedbacks WHERE permission_id = @permission_id";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = Utils.GetCommand(updatesql, conn);
                BuildSqlParameters(cmd,o, false);
                cmd.ExecuteNonQuery();
            }
		}
		
		public static void Delete(int permission_id)
		{
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = Utils.GetCommand("DELETE FROM admin_permissions WHERE permission_id = @id" , conn);
                cmd.Parameters.AddWithValue("@id", permission_id);
                cmd.ExecuteNonQuery();
            }
		}
		
		
	}
}
			
			