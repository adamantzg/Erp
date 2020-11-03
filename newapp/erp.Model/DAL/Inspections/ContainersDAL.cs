
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace asaq2.Model
{
    public class ContainersDAL
	{
	
		public static List<Containers> GetAll()
		{
			var result = new List<Containers>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT * FROM containers", conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}

        public static List<Containers> GetForInspection(int insp_id)
        {
            var result = new List<Containers>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT * FROM containers WHERE insp_id = @insp_id", conn);
                cmd.Parameters.AddWithValue("@insp_id", insp_id);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
        }
		
		
		public static Containers GetById(int id)
		{
			Containers result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT * FROM containers WHERE container_id = @id", conn);
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
		
	
		private static Containers GetFromDataReader(MySqlDataReader dr)
		{
			Containers o = new Containers();
		
			o.container_id =  (int) dr["container_id"];
			o.insp_id = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"insp_id"));
			o.container_no = string.Empty + Utilities.GetReaderField(dr,"container_no");
			o.seal_no = string.Empty + Utilities.GetReaderField(dr,"seal_no");
			o.container_size = string.Empty + Utilities.GetReaderField(dr,"container_size");
			o.container_count = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"container_count"));
			o.container_space = Utilities.FromDbValue<double>(Utilities.GetReaderField(dr,"container_space"));
			o.container_comments = string.Empty + Utilities.GetReaderField(dr,"container_comments");
			
			return o;

		}
		
		
		public static void Create(Containers o)
        {
            string insertsql = @"INSERT INTO containers (insp_id,container_no,seal_no,container_size,container_count,container_space,container_comments) VALUES(@insp_id,@container_no,@seal_no,@container_size,@container_count,@container_space,@container_comments)";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				
				var cmd = new MySqlCommand(insertsql, conn);
                BuildSqlParameters(cmd,o);
                cmd.ExecuteNonQuery();
				cmd.CommandText = "SELECT container_id FROM containers WHERE container_id = LAST_INSERT_ID()";
                o.container_id = (int) cmd.ExecuteScalar();
				
            }
		}
		
		private static void BuildSqlParameters(MySqlCommand cmd, Containers o, bool forInsert = true)
        {
			
			if(!forInsert)
				cmd.Parameters.AddWithValue("@container_id", o.container_id);
			cmd.Parameters.AddWithValue("@insp_id", o.insp_id);
			cmd.Parameters.AddWithValue("@container_no", o.container_no);
			cmd.Parameters.AddWithValue("@seal_no", o.seal_no);
			cmd.Parameters.AddWithValue("@container_size", o.container_size);
			cmd.Parameters.AddWithValue("@container_count", o.container_count);
			cmd.Parameters.AddWithValue("@container_space", o.container_space);
			cmd.Parameters.AddWithValue("@container_comments", o.container_comments);
		}
		
		public static void Update(Containers o)
		{
			string updatesql = @"UPDATE containers SET insp_id = @insp_id,container_no = @container_no,seal_no = @seal_no,container_size = @container_size,container_count = @container_count,container_space = @container_space,container_comments = @container_comments WHERE container_id = @container_id";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = new MySqlCommand(updatesql, conn);
                BuildSqlParameters(cmd,o, false);
                cmd.ExecuteNonQuery();
            }
		}
		
		public static void Delete(int container_id)
		{
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = new MySqlCommand("DELETE FROM containers WHERE container_id = @id" , conn);
                cmd.Parameters.AddWithValue("@id", container_id);
                cmd.ExecuteNonQuery();
            }
		}
		
		
	}
}
			
			