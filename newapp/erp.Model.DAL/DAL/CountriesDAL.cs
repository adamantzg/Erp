
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using erp.Model.DAL.Properties;
using Dapper;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace erp.Model.DAL
{
    public class CountriesDAL
	{
	
		public static List<Countries> GetAll()
		{
			List<Countries> result = new List<Countries>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = Utils.GetCommand("SELECT * FROM countries ORDER BY countries.CountryName ASC", conn);
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}
		

		
		public static Countries GetById(int id)
		{
			Countries result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = Utils.GetCommand("SELECT * FROM countries WHERE country_id = @id", conn);
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
        public static Countries GetById_ISO2(string iso2)
        {
            Countries result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
            conn.Open();
                MySqlCommand cmd = Utils.GetCommand("SELECT * FROM countries WHERE ISO2 = @iso2", conn);
				cmd.Parameters.AddWithValue("@iso2", iso2);
                MySqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    result = GetFromDataReader(dr);
                }
                dr.Close();
            }
			return result;

        }

        public static List<Countries> GetForExistingDealers()
        {
            List<Countries> result = new List<Countries>();
            using (var conn= new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = Utils.GetCommand(string.Format(@"SELECT
                                                                    
                                                                    countries.*
                                                                    FROM
                                                                    dealers
                                                                    INNER JOIN countries ON dealers.user_country = countries.ISO2
                                                                    GROUP BY
                                                                    dealers.user_country"), conn);
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
        }

		private static Countries GetFromDataReader(MySqlDataReader dr)
		{
			Countries o = new Countries();
		
			o.ISO2 = string.Empty + dr["ISO2"];
			o.ISO3 = string.Empty + dr["ISO3"];
			o.CountryName = string.Empty + dr["CountryName"];
            o.local_name = string.Empty + dr["local_name"];
            o.flag_filename = string.Empty + dr["flag_filename"];
            o.exchange_rate = Utilities.FromDbValue<double>(dr["exchange_rate"]);
            o.continent_code = string.Empty + dr["continent_code"];
            if(Utilities.ColumnExists(dr, "country_id"))
			    o.country_id =  (int) dr["country_id"];

           // o.latitude = Utilities.FromDbValue<double>(dr["latitude"]);
           // o.longitude=Utilities.FromDbValue<double>(dr["longitude"]);
			
			return o;

		}
		
		public static void Create(Countries o)
        {
            string insertsql = @"INSERT INTO countries (ISO2,ISO3,CountryName,local_name,flag_filename,exchange_rate,continent_code) 
                                VALUES(@ISO2,@ISO3,@CountryName,@local_name,@flag_filename,@exchange_rate,@continent_code)";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				
				MySqlCommand cmd = Utils.GetCommand(insertsql, conn);
                BuildSqlParameters(cmd,o);
                cmd.ExecuteNonQuery();
				cmd.CommandText = "SELECT country_id FROM countries WHERE country_id = LAST_INSERT_ID()";
                o.country_id = (int) cmd.ExecuteScalar();
				
            }
		}
		
		private static void BuildSqlParameters(MySqlCommand cmd, Countries o, bool forInsert = true)
        {
			
			cmd.Parameters.AddWithValue("@ISO2", o.ISO2);
			cmd.Parameters.AddWithValue("@ISO3", o.ISO3);
			cmd.Parameters.AddWithValue("@CountryName", o.CountryName);
            cmd.Parameters.AddWithValue("@local_name", o.local_name);
            cmd.Parameters.AddWithValue("@flag_filename", o.flag_filename);
            cmd.Parameters.AddWithValue("@exchange_rate", o.exchange_rate);
            cmd.Parameters.AddWithValue("@continent_code", o.continent_code);
			if(!forInsert)
				cmd.Parameters.AddWithValue("@country_id", o.country_id);
		}
		
		public static void Update(Countries o)
		{
            string updatesql = @"UPDATE countries SET ISO2 = @ISO2,ISO3 = @ISO3,CountryName = @CountryName, local_name=@local_name, flag_filename=@flag_filename, exchange_rate=@exchange_rate, continent_code=@continent_code  WHERE country_id = @country_id";

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
				MySqlCommand cmd = Utils.GetCommand("DELETE FROM countries WHERE country_id = @id" , conn);
                cmd.Parameters.AddWithValue("@id", country_id);
                cmd.ExecuteNonQuery();
            }
		}

        public static List<Countries> GetForDealersByBrand(string brand_code)
        {
            List<Countries> result = new List<Countries>();
            if (string.IsNullOrEmpty(Properties.Settings.Default.DealerSearchView))
            {
                using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
                {
                    conn.Open();
                    MySqlCommand cmd = Utils.GetCommand(@"SELECT DISTINCT c.* FROM countries AS C INNER JOIN dealers AS d ON d.user_country=c.ISO2 
                                                    INNER JOIN dealer_brandstatus AS db ON db.dealer_id=d.user_id
                                                    INNER JOIN brands AS b ON b.brand_id=db.brand_id
                                                    WHERE d.user_country IS NOT NULL AND b.code=@brand_code AND d.user_country IS NOT NULL 
                                                    ORDER BY db.brand_status ASC, c.country_id ASC", conn);
                    cmd.Parameters.AddWithValue("@brand_code", brand_code);
                    MySqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        result.Add(GetFromDataReader(dr));
                    }
                    dr.Close();
                }
            }
            else
            {
                using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
                {
                    result = conn.Query<Countries>($"SELECT DISTINCT c.* FROM countries AS C INNER JOIN {Settings.Default.DealerSearchView} AS d ON d.user_country = c.ISO2").ToList();
                }
            }
            
            return result;
        }

        public static List<Countries> GetCountriesByContinentAndBrand(string code, string brand_code)
        {
            List<Countries> result = new List<Countries>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = Utils.GetCommand(@"SELECT c.* FROM countries AS C INNER JOIN continents AS co ON co.code=c.continent_code LEFT JOIN dealers AS d ON d.user_country=c.ISO2 
                                                    INNER JOIN dealer_brandstatus AS db ON db.dealer_id=d.user_id
                                                    INNER JOIN brands AS b ON b.brand_id=db.brand_id
                                                    WHERE d.user_country IS NOT NULL AND b.code=@brand_code AND co.code=@code  
                                                    ORDER BY db.brand_status ASC, c.country_id ASC", conn);
                cmd.Parameters.AddWithValue("@code", code);
                cmd.Parameters.AddWithValue("@brand_code", brand_code);
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
        }

        public static List<Countries> GetByLanguage(int language_id, IDbConnection conn = null)
        {
            var result = new List<Countries>();
            if (conn == null)
            {
                conn = new MySqlConnection(Properties.Settings.Default.ConnString);
                conn.Open();
            }
            var cmd = Utils.GetCommand(@"SELECT countries.* FROM countries INNER JOIN language_country ON countries.country_id = language_country.country_id
                                             WHERE language_id = @language_id",(MySqlConnection) conn);
            cmd.Parameters.AddWithValue("@language_id", language_id);
            var dr = cmd.ExecuteReader();
            while (dr.Read()) {
                result.Add(GetFromDataReader(dr));
            }
            dr.Close();
            
            return result;
        }
	}
}
			
			