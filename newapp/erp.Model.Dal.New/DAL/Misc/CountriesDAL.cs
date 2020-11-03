
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

using Dapper;
using MySql.Data;
using MySql.Data.MySqlClient;


namespace erp.Model.Dal.New
{
    public class CountriesDAL : ICountriesDAL
    {
		private MySqlConnection conn;		
		public string AsiaCountries = "";

		public CountriesDAL(IDbConnection conn)
		{
			this.conn = (MySqlConnection) conn;			
		}

		public List<Countries> GetAll()
		{
			return conn.Query<Countries>("SELECT * FROM countries ORDER BY countries.CountryName ASC").ToList();			
		}
		
		public Countries GetById(int id)
		{
			return conn.QueryFirstOrDefault<Countries>("SELECT * FROM countries WHERE country_id = @id", new {id });			
		}
        public Countries GetById_ISO2(string iso2)
        {
			return conn.QueryFirstOrDefault<Countries>("SELECT * FROM countries WHERE ISO2 = @iso2", new {iso2});
        }

        public List<Countries> GetForExistingDealers()
        {
			return conn.Query<Countries>(@"SELECT countries.*
										FROM
										dealers
										INNER JOIN countries ON dealers.user_country = countries.ISO2
										GROUP BY
										dealers.user_country").ToList();            
        }

		
		public void Create(Countries o)
        {
            string insertsql = @"INSERT INTO countries (ISO2,ISO3,CountryName,local_name,flag_filename,exchange_rate,continent_code) 
                                VALUES(@ISO2,@ISO3,@CountryName,@local_name,@flag_filename,@exchange_rate,@continent_code)";
			conn.Execute(insertsql, o);
			o.country_id = conn.ExecuteScalar<int>("SELECT LAST_INSERT_ID()");            
		}		
		
		public void Update(Countries o)
		{
            string updatesql = @"UPDATE countries SET ISO2 = @ISO2,ISO3 = @ISO3,CountryName = @CountryName, local_name=@local_name, flag_filename=@flag_filename, exchange_rate=@exchange_rate, continent_code=@continent_code  WHERE country_id = @country_id";
			conn.Execute(updatesql, o);
		}
		
		public void Delete(int country_id)
		{
			conn.Execute("DELETE FROM countries WHERE country_id = @country_id", new { country_id });			
		}

        public List<Countries> GetForDealersByBrand(string brand_code)
        {
            List<Countries> result = new List<Countries>();
			string sql;
            if (string.IsNullOrEmpty(Properties.Settings.Default.DealerSearchView))
            	sql = @"SELECT DISTINCT c.* FROM countries WHERE c.ISO2 IN (SELECT d.user_country FROM dealers AS d ON d.user_country=c.ISO2 
                        INNER JOIN dealer_brandstatus AS db ON db.dealer_id=d.user_id
                        INNER JOIN brands AS b ON b.brand_id=db.brand_id
                        WHERE d.user_country IS NOT NULL AND b.code=@brand_code AND d.user_country IS NOT NULL 
                        ORDER BY c.country_id ASC";
			else
				sql = $"SELECT DISTINCT c.* FROM countries AS C INNER JOIN {Dal.New.Properties.Settings.Default.DealerSearchView} AS d ON d.user_country = c.ISO2";

            return conn.Query<Countries>(sql, new {brand_code}).ToList();
        }

        public List<Countries> GetCountriesByContinentAndBrand(string code, string brand_code)
        {
			return conn.Query<Countries>(
				@"SELECT c.* FROM countries AS C INNER JOIN continents AS co ON co.code=c.continent_code LEFT JOIN dealers AS d ON d.user_country=c.ISO2 
                    INNER JOIN dealer_brandstatus AS db ON db.dealer_id=d.user_id
                    INNER JOIN brands AS b ON b.brand_id=db.brand_id
                    WHERE d.user_country IS NOT NULL AND b.code=@brand_code AND co.code=@code  
                    ORDER BY db.brand_status ASC, c.country_id ASC",
				new {code, brand_code}).ToList();            
        }

        public List<Countries> GetByLanguage(int language_id, IDbConnection conn = null)
        {
			return conn.Query<Countries>(
				@"SELECT countries.* FROM countries INNER JOIN language_country ON countries.country_id = language_country.country_id
                 WHERE language_id = @language_id", new {language_id}).ToList();            
        }

		public string GetCountryCondition(CountryFilter countryFilter, string prefix = "", bool useAnd = true, string countryField = "user_country")
		{
			var countries = GetUkCountryCodes();
            if(countryFilter == CountryFilter.NonUKExcludingAsia)
            {
                var asianCountries = new List<string>();
                if (string.IsNullOrEmpty(AsiaCountries))
                {
                    asianCountries = conn.Query<string>("SELECT ISO2 FROM countries WHERE continent_code = 'AS'").ToList();
					AsiaCountries = string.Join(",", asianCountries);                    
                }
                else
                    asianCountries = AsiaCountries.Split(',').ToList();
                countries.AddRange(asianCountries);
            }
			return countryFilter != CountryFilter.All
					   ? $@" {(useAnd ? " AND " : "")} {prefix}{countryField} {(countryFilter == CountryFilter.UKOnly ? "" : "NOT")} 
								IN ({string.Join(",",countries.Select(c=>$"'{c}'"))})"
									   
					   : "";
		}

	    public List<string> GetUkCountryCodes()
	    {
			return new List<string>() { "GB", "UK", "IE" };
	    }

		public List<Countries> GetForContinent(string code)
		{
			return conn.Query<Countries>("SELECT * FROM countries WHERE continent_code = @code", new { code }).ToList();
		}
	}
}
			
			