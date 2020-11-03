
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;
using Dapper;

namespace erp.Model.Dal.New
{
    public class BrandSalesAnalysis2DAL : GenericDal<Brand_sales_analysis2>, IBrandSalesAnalysis2DAL
	{
		private readonly IAnalyticsOptionDal analyticsOptionDal;
		private readonly ICountriesDAL countriesDAL;

		public BrandSalesAnalysis2DAL(IDbConnection conn, IAnalyticsOptionDal analyticsOptionDal,
			ICountriesDAL countriesDAL) : base(conn)
		{
			this.analyticsOptionDal = analyticsOptionDal;
			this.countriesDAL = countriesDAL;
		}

		public List<Brand_sales_analysis2> GetForBrand(int? brand_user_id, DateTime? etdFrom, 
	        bool useETA = false, CountryFilter countryFilter = CountryFilter.UKOnly, string excludedCustomers = "NK2,CWB", 
			OrderDateMode dateMode = OrderDateMode.Etd)
        {
            var options = analyticsOptionDal.GetAll();
			var exCustomers = excludedCustomers.Split(',');
            var sql = brand_user_id != null ?
                
                    $@"SELECT brand_sales_analysis2.*, analytics_subcategory.*,analytics_categories.* FROM brand_sales_analysis2
                        LEFT OUTER JOIN analytics_subcategory ON brand_sales_analysis2.analytics_category = analytics_subcategory.subcat_id 
                        LEFT OUTER JOIN analytics_categories ON analytics_subcategory.category_id = analytics_categories.category_id
                        WHERE (brand_user_id = @brand_user_id 
                                    OR 
                                (brand_user_id IS NOT NULL 
                                    AND EXISTS(SELECT cprod_id FROM cust_products WHERE cprod_mast = brand_sales_analysis2.cprod_mast AND brand_user_id = @brand_user_id)))
                        AND ({(dateMode == OrderDateMode.Eta ? "req_eta_nooffset" : dateMode == OrderDateMode.Etd ? "po_req_etd" : "orderdate")} >= @etd 
						OR @etd IS NULL) {countriesDAL.GetCountryCondition(countryFilter)}
                        AND customer_code NOT IN @exCustomers" :

					$@"SELECT brand_sales_analysis2.*, analytics_subcategory.*,analytics_categories.* FROM brand_sales_analysis2
                        INNER JOIN analytics_subcategory ON brand_sales_analysis2.analytics_category = analytics_subcategory.subcat_id 
                        INNER JOIN analytics_categories ON analytics_subcategory.category_id = analytics_categories.category_id
                        WHERE analytics_categories.category_type IS NULL
                        AND customer_code NOT IN @exCustomers {countriesDAL.GetCountryCondition(countryFilter)}
                        AND ({(useETA ? "req_eta_nooffset" : "po_req_etd")} >= @etd OR @etd IS NULL)";                    
                
			var data = conn.Query<Brand_sales_analysis2, Analytics_subcategory, Analytics_categories, Brand_sales_analysis2>(
				sql, 
				(bsa, asub, ac) =>
				{
					bsa.Category = ac;
					bsa.Subcategory = asub;
					return bsa;
				}, new { etd = etdFrom, brand_user_id, exCustomers}, splitOn: "subcat_id, category_id").ToList();
                
            foreach(var d in data)
			{
				d.Option = options.FirstOrDefault(o=>o.option_id == d.analytics_option);
			}
            return data;
        }

        public List<Brand_sales_analysis2> GetForAnalyticsSubcats(DateTime? etdFrom,DateTime? etdTo = null, bool useETA = false, 
			CountryFilter countryFilter = CountryFilter.UKOnly)
        {
            
            var options = analyticsOptionDal.GetAll();
			var sql = 
					$@"SELECT brand_sales_analysis2.*, analytics_subcategory.*,analytics_categories.* FROM brand_sales_analysis2
                        INNER JOIN analytics_subcategory ON brand_sales_analysis2.analytics_category = analytics_subcategory.subcat_id 
                        INNER JOIN analytics_categories ON analytics_subcategory.category_id = analytics_categories.category_id
                        WHERE ({(useETA ? "req_eta_nooffset" : "po_req_etd")} >= @etdFrom OR @etdFrom IS NULL) 
						AND ({(useETA ? "req_eta_nooffset" : "po_req_etd")} <= @etdTo OR @etdTo IS NULL) {countriesDAL.GetCountryCondition(countryFilter)} 
						AND cprod_status <> 'D'";
			var data = conn.Query<Brand_sales_analysis2, Analytics_subcategory, Analytics_categories, Brand_sales_analysis2>(
				sql, 
				(bsa, asub, ac) =>
				{
					bsa.Category = ac;
					bsa.Subcategory = asub;
					return bsa;
				}, new { etdFrom, etdTo}, splitOn: "subcat_id, category_id").ToList();
                
            foreach(var d in data)
			{
				d.Option = options.FirstOrDefault(o=>o.option_id == d.analytics_option);
			}
            return data;                
        }

		protected override string GetAllSql()
		{
			throw new NotImplementedException();
		}

		protected override string GetByIdSql()
		{
			throw new NotImplementedException();
		}

		protected override string GetCreateSql()
		{
			throw new NotImplementedException();
		}

		protected override string GetUpdateSql()
		{
			throw new NotImplementedException();
		}

		protected override string GetDeleteSql()
		{
			throw new NotImplementedException();
		}
	}
}
			
			