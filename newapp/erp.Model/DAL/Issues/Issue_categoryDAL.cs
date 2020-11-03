
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace asaq2.Model
{
    public class Issue_categoryDAL
	{
	
		public static List<Issue_category> GetAll()
		{
			List<Issue_category> result = new List<Issue_category>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM issue_category", conn);
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}

        public static List<Issue_category> GetChildren(int? parent_id, int? area_id = null)
        {
            List<Issue_category> result = new List<Issue_category>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("", conn);
                cmd.Parameters.AddWithValue("@parent_id", parent_id == null ? (object)DBNull.Value : parent_id);
                if (area_id == null)
                    cmd.CommandText = "SELECT issue_category.*, (SELECT COUNT(*) FROM issue_category child WHERE child.parent_id = issue_category.category_id) AS 'childCount'  FROM issue_category WHERE COALESCE(parent_id, 0) = COALESCE(@parent_id, 0)";
                else
                {
                    cmd.CommandText = @"SELECT issue_category.*, 
                                    (SELECT COUNT(*) FROM issue_category child INNER JOIN issuecategory_areas ON child.category_id = issuecategory_areas.category_id
                                        WHERE issuecategory_areas.area_id = @area_id AND child.parent_id = issue_category.category_id) AS 'childCount' FROM issuecategory_areas INNER JOIN issue_category ON issue_category.category_id = issuecategory_areas.category_id
                                                      WHERE issuecategory_areas.area_id = @area_id AND COALESCE(parent_id, 0) = COALESCE(@parent_id, 0)";
                    cmd.Parameters.AddWithValue("@area_id", area_id);
                }

                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
        }

        public static List<Issue_category> GetCategoriesForArea(int area_id)
        {
            List<Issue_category> result = new List<Issue_category>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(@"SELECT issue_category.* FROM issuecategory_areas INNER JOIN issue_category ON issue_category.category_id = issuecategory_areas.category_id
                                                      WHERE issuecategory_areas.area_id = @id", conn);
                cmd.Parameters.AddWithValue("@id", area_id);
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
        }

        public static List<Issue_area> GetAreasForCategory(int category_id)
        {
            return null;
        }
		
		
		public static Issue_category GetById(int id)
		{
			Issue_category result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM issue_category WHERE category_id = @id", conn);
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

        public static List<Issue_category> GetCategoriesForIssue(int issue_id, int? parent_id = null)
        {
            List<Issue_category> result = new List<Issue_category>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(@"SELECT issue_category.* FROM issue_categories INNER JOIN issue_category ON issue_category.category_id = issue_categories.category_id
                                                      WHERE issue_categories.issue_id = @issue_id AND COALESCE(parent_id, 0) = COALESCE(@parent_id, 0)", conn);
                cmd.Parameters.AddWithValue("@issue_id", issue_id);
                cmd.Parameters.AddWithValue("@parent_id", parent_id == null ? (object)DBNull.Value : parent_id);
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
        }

        public static List<Issue_category> GetAllCategoriesForIssue(int issue_id)
        {
            List<Issue_category> result = new List<Issue_category>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(@"SELECT issue_category.* FROM issue_categories INNER JOIN issue_category ON issue_category.category_id = issue_categories.category_id
                                                      WHERE issue_categories.issue_id = @issue_id ", conn);
                cmd.Parameters.AddWithValue("@issue_id", issue_id);
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
        }

        public static bool CanBeDeleted(int category_id, ref List<Issue> issues)
        {
            bool result = true;
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                result = CanBeDeleted(category_id, ref issues, conn);
            }
            return result;
        }
        
        private static bool CanBeDeleted(int category_id, ref List<Issue> issues, MySqlConnection conn)
        {
            bool result = true;
            
            MySqlCommand cmd = new MySqlCommand(@"SELECT issue.issue_id, issue.title FROM issue_categories  INNER JOIN issue ON issue_categories.issue_id = issue.issue_id 
                                                WHERE category_id = @category_id LIMIT 3", conn);
            cmd.Parameters.AddWithValue("@category_id", category_id);
            MySqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                result = false;
                if(issues == null)
                    issues = new List<Issue>();
                issues.Add( new Issue { issue_id = (int) dr["issue_id"], title = string.Empty + dr["title"] });
            }
            dr.Close();
            if (result)
            { 
                //check if subcategories assigned to issues
                List<Issue_category> children = GetChildren(category_id);
                foreach (var child in children)
                {
                    if (!(result = CanBeDeleted(child.category_id, ref issues, conn)))
                        break;
                }
            }

            
        return result;
        }




		private static Issue_category GetFromDataReader(MySqlDataReader dr)
		{
			Issue_category o = new Issue_category();
		
			o.category_id =  (int) dr["category_id"];
			o.name = string.Empty + dr["name"];
			o.parent_id = Utilities.FromDbValue<int>(dr["parent_id"]);
            if (Utilities.ColumnExists(dr, "childCount"))
                o.child_count = Convert.ToInt32(Utilities.FromDbValue<long>(dr["childCount"]));
			
			return o;

		}

        public static void Create(Issue_category o)
        {
            string insertsql = @"INSERT INTO issue_category (category_id,name,parent_id) VALUES(@category_id,@name,@parent_id)";

            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("SELECT COALESCE(MAX(category_id),0)+1 FROM issue_category", conn);
                o.category_id = Convert.ToInt32(cmd.ExecuteScalar());
                cmd.CommandText = insertsql;

                BuildSqlParameters(cmd, o);
                cmd.ExecuteNonQuery();

            }
        }
		
		private static void BuildSqlParameters(MySqlCommand cmd, Issue_category o, bool forInsert = true)
        {
			
			cmd.Parameters.AddWithValue("@category_id", o.category_id);
			cmd.Parameters.AddWithValue("@name", o.name);
			cmd.Parameters.AddWithValue("@parent_id", o.parent_id);
		}
		
		public static void Update(Issue_category o)
		{
			string updatesql = @"UPDATE issue_category SET name = @name,parent_id = @parent_id WHERE category_id = @category_id";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				MySqlCommand cmd = new MySqlCommand(updatesql, conn);
                BuildSqlParameters(cmd,o, false);
                cmd.ExecuteNonQuery();
            }
		}
		
		public static void Delete(int category_id)
		{
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				MySqlCommand cmd = new MySqlCommand("DELETE FROM issue_category WHERE category_id = @id" , conn);
                cmd.Parameters.AddWithValue("@id", category_id);
                cmd.ExecuteNonQuery();
            }
		}

        public static void UpdateAreas(int id, int[] areas)
        {
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("DELETE FROM issuecategory_areas WHERE category_id = @id", conn);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
                if (areas != null && areas.Length > 0)
                {
                    cmd.CommandText = "INSERT INTO issuecategory_areas(category_id, area_id) VALUES(@id,@area_id)";
                    cmd.Parameters.AddWithValue("@area_id", 0);
                    foreach (var area_id in areas)
                    {
                        cmd.Parameters[1].Value = area_id;
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }
    }
}
			
			