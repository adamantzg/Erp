
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace erp.Model.DAL
{
    public class ContinentsDAL
	{
	
		public static List<Continents> GetAll(bool all = false)
		{
			List<Continents> result = new List<Continents>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var query = string.Format("SELECT * FROM continents {0}", all ? "" : "WHERE status=1");
                MySqlCommand cmd = Utils.GetCommand(query, conn);
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}
		

		
		public static Continents GetById(int id)
		{
			Continents result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = Utils.GetCommand("SELECT * FROM continents WHERE id = @id", conn);
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
        public static Continents GetByCode(string code)
        {
            Continents result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = Utils.GetCommand("SELECT * FROM continents WHERE code = @code", conn);
                cmd.Parameters.AddWithValue("@code", code);
                MySqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    result = GetFromDataReader(dr);
                }
                dr.Close();
            }
			return result;

        }

		private static Continents GetFromDataReader(MySqlDataReader dr)
		{
			Continents o = new Continents();
		
			o.id = (int)dr["id"];
			o.name = (string)dr["name"];
			o.status = (int)dr["status"];
            o.code = string.Empty + dr["code"];
			
			return o;

		}
		
		public static void Create(Continents o)
        {
            string insertsql = @"INSERT INTO continents (name,status,code) VALUES(@name,@status,@code)";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				
				MySqlCommand cmd = Utils.GetCommand(insertsql, conn);
                BuildSqlParameters(cmd,o);
                cmd.ExecuteNonQuery();
				cmd.CommandText = "SELECT id FROM continents WHERE id = LAST_INSERT_ID()";
                o.id = (int) cmd.ExecuteScalar();
				
            }
		}
		
		private static void BuildSqlParameters(MySqlCommand cmd, Continents o, bool forInsert = true)
        {
            if (!forInsert)
                cmd.Parameters.AddWithValue("@id", o.id);
			cmd.Parameters.AddWithValue("@name", o.name);
			cmd.Parameters.AddWithValue("@code", o.code);
			cmd.Parameters.AddWithValue("@status", o.status);
		}
		
		public static void Update(Continents o)
		{
            string updatesql = @"UPDATE continents SET name=@name,status=@status,code=@code  WHERE id=@id";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				MySqlCommand cmd = Utils.GetCommand(updatesql, conn);
                BuildSqlParameters(cmd,o, false);
                cmd.ExecuteNonQuery();
            }
		}
		
		public static void Delete(int country_id)
		{
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				MySqlCommand cmd = Utils.GetCommand("DELETE FROM continents WHERE id = @id" , conn);
                cmd.Parameters.AddWithValue("@id", country_id);
                cmd.ExecuteNonQuery();
            }
		}

        public static List<Continents> GetForBrand(int brandid)
        {
            List<Continents> result = new List<Continents>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = Utils.GetCommand(@"SELECT c.* FROM continents AS C INNER JOIN dealers AS d ON d.user_country=c.ISO2 
                                                    INNER JOIN dealer_brandstatus AS db ON db.dealer_id=d.user_id
                                                    INNER JOIN brands AS b ON b.brand_id=db.brand_id
                                                    WHERE d.user_country IS NOT NULL AND b.code=@brand_code ORDER BY db.brand_status ASC, c.country_id ASC", conn);
                cmd.Parameters.AddWithValue("@brandid", brandid);
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
        }
	}
}
			
			