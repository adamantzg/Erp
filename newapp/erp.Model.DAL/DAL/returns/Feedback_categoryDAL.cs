
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace erp.Model.DAL
{
    public class Feedback_categoryDAL
	{
	
		public static List<Feedback_category> GetAll()
		{
			var result = new List<Feedback_category>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM feedback_category", conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}

        public static List<Feedback_category> GetForType(int feedback_type)
        {
            var result = new List<Feedback_category>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM feedback_category WHERE feedback_type = @feedback_type", conn);
                cmd.Parameters.AddWithValue("@feedback_type", feedback_type);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
        }
		
		
		public static Feedback_category GetById(int id)
		{
			Feedback_category result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM feedback_category WHERE feedback_cat_id = @id", conn);
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
		
	
		public static Feedback_category GetFromDataReader(MySqlDataReader dr)
		{
			Feedback_category o = new Feedback_category();
		
			o.feedback_cat_id =  (int) dr["feedback_cat_id"];
			o.name = string.Empty + Utilities.GetReaderField(dr,"name");
			o.feedback_type = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"feedback_type"));
			
			return o;

		}
		
		
		public static void Create(Feedback_category o)
        {
            string insertsql = @"INSERT INTO feedback_category (feedback_cat_id,name,feedback_type) VALUES(@feedback_cat_id,@name,@feedback_type)";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				
                var cmd = Utils.GetCommand("SELECT MAX(feedback_cat_id)+1 FROM feedback_category", conn);
                o.feedback_cat_id = Convert.ToInt32(cmd.ExecuteScalar());
				cmd.CommandText = insertsql;
				
                BuildSqlParameters(cmd,o);
                cmd.ExecuteNonQuery();
				
            }
		}
		
		private static void BuildSqlParameters(MySqlCommand cmd, Feedback_category o, bool forInsert = true)
        {
			
			cmd.Parameters.AddWithValue("@feedback_cat_id", o.feedback_cat_id);
			cmd.Parameters.AddWithValue("@name", o.name);
			cmd.Parameters.AddWithValue("@feedback_type", o.feedback_type);
		}
		
		public static void Update(Feedback_category o)
		{
			string updatesql = @"UPDATE feedback_category SET name = @name,feedback_type = @feedback_type WHERE feedback_cat_id = @feedback_cat_id";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = Utils.GetCommand(updatesql, conn);
                BuildSqlParameters(cmd,o, false);
                cmd.ExecuteNonQuery();
            }
		}
		
		public static void Delete(int feedback_cat_id)
		{
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = Utils.GetCommand("DELETE FROM feedback_category WHERE feedback_cat_id = @id" , conn);
                cmd.Parameters.AddWithValue("@id", feedback_cat_id);
                cmd.ExecuteNonQuery();
            }
		}
		
		
	}
}
			
			