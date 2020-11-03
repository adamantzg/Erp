
using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace erp.Model.DAL
{
    public class RangeDAL
	{
	
		public static List<Range> GetAll()
		{
			List<Range> result = new List<Range>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = Utils.GetCommand("SELECT * FROM ranges", conn);
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}
		
		
		public static Range GetById(int id)
		{
			Range result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = Utils.GetCommand("SELECT * FROM ranges WHERE rangeid = @id", conn);
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

        public static List<Range> GetByCompanyId(int id, bool combined = true)
        {
            var result = new List<Range>();
            Company company = CompanyDAL.GetById(id);
            if (company != null)
            {
                using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
                {
                    conn.Open();
                    
                    var cmd =
                        Utils.GetCommand(string.Format(@"SELECT ranges.* FROM ranges WHERE LENGTH(TRIM(range_desc)) > 0 AND  rangeid 
                                            IN (SELECT cprod_range FROM cust_products INNER JOIN users ON cust_products.cprod_user = users.user_id 
                                            WHERE {0})",combined ? "users.combined_factory = @id" : "users.user_id = @id"),conn);
                    cmd.Parameters.AddWithValue("@id", combined ? company.combined_factory : id);
                    var dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        result.Add(GetFromDataReader(dr));
                    }
                    dr.Close();
                }
            }
            return result;
        }
	
		private static Range GetFromDataReader(MySqlDataReader dr)
		{
			Range o = new Range();
		
			o.rangeid =  (int) dr["rangeid"];
			o.range_name = string.Empty + dr["range_name"];
			o.range_desc = string.Empty + dr["range_desc"];
			o.range_image = string.Empty + dr["range_image"];
			o.category1 = Utilities.FromDbValue<int>(dr["category1"]);
			o.forecast_percentage = Utilities.FromDbValue<double>(dr["forecast_percentage"]);
			o.user_id = Utilities.FromDbValue<int>(dr["user_id"]);
			
			return o;

		}
		
		public static void Create(Range o)
        {
            string insertsql = @"INSERT INTO ranges (rangeid,range_name,range_desc,range_image,category1,forecast_percentage,user_id) VALUES(@rangeid,@range_name,@range_desc,@range_image,@category1,@forecast_percentage,@user_id)";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				
                MySqlCommand cmd = Utils.GetCommand("SELECT MAX(rangeid)+1 FROM ranges", conn);
                o.rangeid = Convert.ToInt32(cmd.ExecuteScalar());
				cmd.CommandText = insertsql;
				
                BuildSqlParameters(cmd,o);
                cmd.ExecuteNonQuery();
				
            }
		}
		
		private static void BuildSqlParameters(MySqlCommand cmd, Range o, bool forInsert = true)
        {
			
			cmd.Parameters.AddWithValue("@rangeid", o.rangeid);
			cmd.Parameters.AddWithValue("@range_name", o.range_name);
			cmd.Parameters.AddWithValue("@range_desc", o.range_desc);
			cmd.Parameters.AddWithValue("@range_image", o.range_image);
			cmd.Parameters.AddWithValue("@category1", o.category1);
			cmd.Parameters.AddWithValue("@forecast_percentage", o.forecast_percentage);
			cmd.Parameters.AddWithValue("@user_id", o.user_id);
		}
		
		public static void Update(Range o)
		{
			string updatesql = @"UPDATE ranges SET range_name = @range_name,range_desc = @range_desc,range_image = @range_image,category1 = @category1,forecast_percentage = @forecast_percentage,user_id = @user_id WHERE rangeid = @rangeid";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				MySqlCommand cmd = Utils.GetCommand(updatesql, conn);
                BuildSqlParameters(cmd,o, false);
                cmd.ExecuteNonQuery();
            }
		}
		
		public static void Delete(int rangeid)
		{
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				MySqlCommand cmd = Utils.GetCommand("DELETE FROM ranges WHERE rangeid = @id" , conn);
                cmd.Parameters.AddWithValue("@id", rangeid);
                cmd.ExecuteNonQuery();
            }
		}
	}
}
			
			