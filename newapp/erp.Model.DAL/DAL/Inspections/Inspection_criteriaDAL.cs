
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace erp.Model.DAL
{
    public class Inspection_criteriaDAL
	{
	
		public static List<Inspection_criteria> GetAll()
		{
			var result = new List<Inspection_criteria>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM inspection_criteria", conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}

        public static List<Inspection_criteria> GetForCategory1(int category1_id)
        {
            var result = new List<Inspection_criteria>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM inspection_criteria WHERE category1_id = @category1", conn);
                cmd.Parameters.AddWithValue("@category1", category1_id);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
        }
		
		
		public static Inspection_criteria GetById(int id)
		{
			Inspection_criteria result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM inspection_criteria WHERE insp_criteria_id = @id", conn);
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
		
	
		private static Inspection_criteria GetFromDataReader(MySqlDataReader dr)
		{
			Inspection_criteria o = new Inspection_criteria();
		
			o.insp_criteria_id =  (int) dr["insp_criteria_id"];
			o.category1_id =  (int) dr["category1_id"];
			o.insp_type = string.Empty + Utilities.GetReaderField(dr,"insp_type");
			o.point = string.Empty + Utilities.GetReaderField(dr,"point");
			o.requirement = string.Empty + Utilities.GetReaderField(dr,"requirement");
			
			return o;

		}
		
		
		public static void Create(Inspection_criteria o)
        {
            string insertsql = @"INSERT INTO inspection_criteria (category1_id,insp_type,point,requirement) VALUES(@category1_id,@insp_type,@point,@requirement)";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				
				var cmd = Utils.GetCommand(insertsql, conn);
                BuildSqlParameters(cmd,o);
                cmd.ExecuteNonQuery();
				cmd.CommandText = "SELECT insp_criteria_id FROM inspection_criteria WHERE insp_criteria_id = LAST_INSERT_ID()";
                o.insp_criteria_id = (int) cmd.ExecuteScalar();
				
            }
		}
		
		private static void BuildSqlParameters(MySqlCommand cmd, Inspection_criteria o, bool forInsert = true)
        {
			
			if(!forInsert)
				cmd.Parameters.AddWithValue("@insp_criteria_id", o.insp_criteria_id);
			cmd.Parameters.AddWithValue("@category1_id", o.category1_id);
			cmd.Parameters.AddWithValue("@insp_type", o.insp_type);
			cmd.Parameters.AddWithValue("@point", o.point);
			cmd.Parameters.AddWithValue("@requirement", o.requirement);
		}
		
		public static void Update(Inspection_criteria o)
		{
			string updatesql = @"UPDATE inspection_criteria SET category1_id = @category1_id,insp_type = @insp_type,point = @point,requirement = @requirement WHERE insp_criteria_id = @insp_criteria_id";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = Utils.GetCommand(updatesql, conn);
                BuildSqlParameters(cmd,o, false);
                cmd.ExecuteNonQuery();
            }
		}
		
		public static void Delete(int insp_criteria_id)
		{
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = Utils.GetCommand("DELETE FROM inspection_criteria WHERE insp_criteria_id = @id" , conn);
                cmd.Parameters.AddWithValue("@id", insp_criteria_id);
                cmd.ExecuteNonQuery();
            }
		}
		
		
	}
}
			
			