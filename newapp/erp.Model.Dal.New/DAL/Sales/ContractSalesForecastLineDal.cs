
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
    public class ContractSalesForecastLinesDal : GenericDal<Contract_sales_forecast_lines>, IContractSalesForecastLinesDal
	{
		public ContractSalesForecastLinesDal(IDbConnection conn) : base(conn)
		{
		}

		
        public List<Contract_sales_forecast_lines> GetForPeriod(int cprod_id, DateTime dateFrom, DateTime dateTo)
        {
            return GetForPeriod(new int[cprod_id], dateFrom, dateTo);
        }

        public List<Contract_sales_forecast_lines> GetForPeriod(IList<int> cprod_ids, DateTime dateFrom, DateTime dateTo)
        {
			return conn.Query<Contract_sales_forecast, Contract_sales_forecast_lines, Contract_sales_forecast_lines>(
				@"SELECT cf.*, cf_line.* FROM contract_sales_forecast_lines cf_line INNER JOIN contract_sales_forecast cf 
				ON cf_line.forecast_id = cf.forecast_id  
                WHERE cprod_id IN @cprod_ids AND (startMonth BETWEEN @from AND @to OR DATE_ADD(startMonth, 
				INTERVAL (CASE WHEN cf_line.monthDuration IS NOT NULL THEN cf_line.monthDuration ELSE cf.monthDuration END) MONTH) BETWEEN @from AND @to) ",
				(cf, cfl) =>
				{
					cfl.Forecast = cf;
					return cfl;
				}, new {from = dateFrom, to = dateTo, cprod_ids}, splitOn: "lines_id").ToList();
            
        }

        public List<Contract_sales_forecast_lines> GetForMastProductAndPeriod(int mast_id, DateTime dateFrom, DateTime dateTo)
        {
			return conn.Query<Contract_sales_forecast, Contract_sales_forecast_lines, Contract_sales_forecast_lines>(
				@"SELECT cf.*,cf_line.* FROM contract_sales_forecast_lines cf_line INNER JOIN contract_sales_forecast cf 
					ON cf_line.forecast_id = cf.forecast_id  
                    INNER JOIN cust_products ON cf_line.cprod_id = cust_products.cprod_id
                    WHERE cprod_mast = @mast_id 
					AND (startMonth BETWEEN @from AND @to OR DATE_ADD(startMonth, 
						INTERVAL (CASE WHEN cf_line.monthDuration IS NOT NULL THEN cf_line.monthDuration ELSE cf.monthDuration END) MONTH) 
						BETWEEN @from AND @to) ",
				(cf, cfl) =>
				{
					cfl.Forecast = cf;
					return cfl;
				}, new {from = dateFrom, to = dateTo, mast_id}, splitOn: "lines_id").ToList();
            
        }

        public List<Contract_sales_forecast_lines> GetByForecastId(int forecast_id)
        {
			return conn.Query<Contract_sales_forecast_lines>(
				@"SELECT contract_sales_forecast_lines.*, cust_products.cprod_name, cust_products.cprod_code1 AS cprod_code 
				FROM contract_sales_forecast_lines INNER JOIN cust_products ON contract_sales_forecast_lines.cprod_id = cust_products.cprod_id 
				WHERE forecast_id = @forecast_id",
				new {forecast_id}).ToList();            
        }
				
		
		
		protected override string GetAllSql()
		{
			return "SELECT * FROM contract_sales_forecast_lines";
		}

		protected override string GetByIdSql()
		{
			return "SELECT * FROM contract_sales_forecast_lines WHERE lines_id = @id";
		}

		protected override string GetCreateSql()
		{
			return @"INSERT INTO contract_sales_forecast_lines (forecast_id,cprod_id,qty,monthduration) VALUES(@forecast_id,@cprod_id,@qty,@monthduration)";
		}

		protected override string GetUpdateSql()
		{
			return @"UPDATE contract_sales_forecast_lines SET forecast_id = @forecast_id,cprod_id = @cprod_id,qty = @qty, monthduration = @monthduration 
				WHERE lines_id = @lines_id";
		}

		protected override string GetDeleteSql()
		{
			return "DELETE FROM contract_sales_forecast_lines WHERE lines_id = @id";
		}
	}
}
			
			
