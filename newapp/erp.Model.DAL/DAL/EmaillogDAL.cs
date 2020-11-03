
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using erp.Model;
using MySql.Data.MySqlClient;

namespace erp.Model.DAL
{
    public partial class EmaillogDAL
	{
	
		public static List<Emaillog> GetAll()
		{
			var result = new List<Emaillog>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM emaillog", conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}

        public static List<Emaillog> GetFromTo(DateTime from,DateTime to)
        {
            var result = new List<Emaillog>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand(@"SELECT * FROM emaillog WHERE emaillog.logtime BETWEEN @from and @to ", conn);
                cmd.Parameters.AddWithValue("@from",from);
                cmd.Parameters.AddWithValue("@to", to);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
           
        }
		
		
		public static Emaillog GetById(int id)
		{
			Emaillog result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM emaillog WHERE id = @id", conn);
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
		
	
		public static Emaillog GetFromDataReader(MySqlDataReader dr)
		{
			Emaillog o = new Emaillog();
		
			o.id =  (int) dr["id"];
			o.email = string.Empty + Utilities.GetReaderField(dr,"email");
			o.logtime = Utilities.FromDbValue<DateTime>(Utilities.GetReaderField(dr,"logtime"));
			o.type = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"type"));
            o.name = string.Empty + Utilities.GetReaderField(dr, "name");
            o.company = string.Empty + Utilities.GetReaderField(dr, "company");
			
			return o;

		}
		
		
		public static void Create(Emaillog o)
        {
            string insertsql = @"INSERT INTO emaillog (email,logtime,type) VALUES(@email,@logtime,@type)";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				
				var cmd = Utils.GetCommand(insertsql, conn);
                BuildSqlParameters(cmd,o);
                cmd.ExecuteNonQuery();
				cmd.CommandText = "SELECT id FROM emaillog WHERE id = LAST_INSERT_ID()";
                o.id = (int) cmd.ExecuteScalar();
				
            }
		}
		
		private static void BuildSqlParameters(MySqlCommand cmd, Emaillog o, bool forInsert = true)
        {
			
			if(!forInsert)
				cmd.Parameters.AddWithValue("@id", o.id);
			cmd.Parameters.AddWithValue("@email", o.email);
			cmd.Parameters.AddWithValue("@logtime", o.logtime);
			cmd.Parameters.AddWithValue("@type", o.type);
		}
		
		public static void Update(Emaillog o)
		{
			string updatesql = @"UPDATE emaillog SET email = @email,logtime = @logtime,type = @type WHERE id = @id";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = Utils.GetCommand(updatesql, conn);
                BuildSqlParameters(cmd,o, false);
                cmd.ExecuteNonQuery();
            }
		}
		
		public static void Delete(int id)
		{
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = Utils.GetCommand("DELETE FROM emaillog WHERE id = @id" , conn);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }
		}
		
		
	}
}
			
			