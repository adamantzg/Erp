
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace asaq2.Model
{
    public class Email_recipientsDAL
	{
	
		public static List<Email_recipients> GetAll()
		{
			var result = new List<Email_recipients>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT * FROM email_recipients", conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}

        public static List<Email_recipients> GetByCriteria(int company_id,string area = null)
        {
            var result = new List<Email_recipients>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT * FROM email_recipients WHERE company_id = @company_id AND (area = @area OR @area IS NULL)", conn);
                cmd.Parameters.AddWithValue("@company_id", company_id);
                cmd.Parameters.AddWithValue("@area", (object) area ?? DBNull.Value);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
        }
		
		
		public static Email_recipients GetById(int id)
		{
			Email_recipients result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT * FROM email_recipients WHERE id = @id", conn);
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
		
	
		private static Email_recipients GetFromDataReader(MySqlDataReader dr)
		{
			Email_recipients o = new Email_recipients();
		
			o.id =  (int) dr["id"];
			o.company_id = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"company_id"));
			o.area = string.Empty + Utilities.GetReaderField(dr,"area");
			o.to = string.Empty + Utilities.GetReaderField(dr,"to");
			o.cc = string.Empty + Utilities.GetReaderField(dr,"cc");
			o.bcc = string.Empty + Utilities.GetReaderField(dr,"bcc");
			
			return o;

		}
		
		
		public static void Create(Email_recipients o)
        {
            string insertsql = @"INSERT INTO email_recipients (company_id,area,to,cc,bcc) VALUES(@company_id,@area,@to,@cc,@bcc)";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				
				var cmd = new MySqlCommand(insertsql, conn);
                BuildSqlParameters(cmd,o);
                cmd.ExecuteNonQuery();
				cmd.CommandText = "SELECT id FROM email_recipients WHERE id = LAST_INSERT_ID()";
                o.id = (int) cmd.ExecuteScalar();
				
            }
		}
		
		private static void BuildSqlParameters(MySqlCommand cmd, Email_recipients o, bool forInsert = true)
        {
			
			if(!forInsert)
				cmd.Parameters.AddWithValue("@id", o.id);
			cmd.Parameters.AddWithValue("@company_id", o.company_id);
			cmd.Parameters.AddWithValue("@area", o.area);
			cmd.Parameters.AddWithValue("@to", o.to);
			cmd.Parameters.AddWithValue("@cc", o.cc);
			cmd.Parameters.AddWithValue("@bcc", o.bcc);
		}
		
		public static void Update(Email_recipients o)
		{
			string updatesql = @"UPDATE email_recipients SET company_id = @company_id,area = @area,to = @to,cc = @cc,bcc = @bcc WHERE id = @id";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = new MySqlCommand(updatesql, conn);
                BuildSqlParameters(cmd,o, false);
                cmd.ExecuteNonQuery();
            }
		}
		
		public static void Delete(int id)
		{
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = new MySqlCommand("DELETE FROM email_recipients WHERE id = @id" , conn);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }
		}
		
		
	}
}
			
			