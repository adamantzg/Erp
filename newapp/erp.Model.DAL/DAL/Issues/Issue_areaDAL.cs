
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace erp.Model.DAL
{
    public class Issue_areaDAL
	{
	
		public static List<Issue_area> GetAll()
		{
			List<Issue_area> result = new List<Issue_area>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = Utils.GetCommand("SELECT * FROM issue_area", conn);
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}
		
		
		public static Issue_area GetById(int id)
		{
			Issue_area result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = Utils.GetCommand("SELECT * FROM issue_area WHERE area_id = @id", conn);
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

        public static List<Issue_area> GetForCategory(int category_id)
        {
            List<Issue_area> result = new List<Issue_area>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = Utils.GetCommand("SELECT issue_area.* FROM issuecategory_areas INNER JOIN issue_area ON issuecategory_areas.area_id = issue_area.area_id WHERE category_id = @category_id", conn);
                cmd.Parameters.AddWithValue("@category_id", category_id);
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
        }

        
	
		private static Issue_area GetFromDataReader(MySqlDataReader dr)
		{
			Issue_area o = new Issue_area();
		
			o.area_id =  (int) dr["area_id"];
			o.areaname = string.Empty + dr["areaname"];
			
			return o;

		}

        public static void Create(Issue_area o)
        {
            string insertsql = @"INSERT INTO issue_area (area_id,areaname) VALUES(@area_id,@areaname)";

            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();

                MySqlCommand cmd = Utils.GetCommand("SELECT MAX(area_id)+1 FROM issue_area", conn);
                o.area_id = Convert.ToInt32(cmd.ExecuteScalar());
                cmd.CommandText = insertsql;

                BuildSqlParameters(cmd, o);
                cmd.ExecuteNonQuery();

            }
        }
		
		private static void BuildSqlParameters(MySqlCommand cmd, Issue_area o, bool forInsert = true)
        {
			
			if(!forInsert)
				cmd.Parameters.AddWithValue("@area_id", o.area_id);
			cmd.Parameters.AddWithValue("@areaname", o.areaname);
		}
		
		public static void Update(Issue_area o)
		{
			string updatesql = @"UPDATE issue_area SET areaname = @areaname WHERE area_id = @area_id";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				MySqlCommand cmd = Utils.GetCommand(updatesql, conn);
                BuildSqlParameters(cmd,o, false);
                cmd.ExecuteNonQuery();
            }
		}
		
		public static void Delete(int area_id)
		{
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				MySqlCommand cmd = Utils.GetCommand("DELETE FROM issue_area WHERE area_id = @id" , conn);
                cmd.Parameters.AddWithValue("@id", area_id);
                cmd.ExecuteNonQuery();
            }
		}
	}
}
			
			