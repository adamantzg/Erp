
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace asaq2.Model
{
    public class Contract_sales_forecast_linesDAL
	{
	
		public static List<Contract_sales_forecast_lines> GetAll()
		{
			List<Contract_sales_forecast_lines> result = new List<Contract_sales_forecast_lines>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM contract_sales_forecast_lines", conn);
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}

        public static List<Contract_sales_forecast_lines> GetForPeriod(int cprod_id, DateTime dateFrom, DateTime dateTo)
        {
            var result = new List<Contract_sales_forecast_lines>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand(@"SELECT cf_line.lines_id,cf_line.forecast_id,cf_line.cprod_id,cf_line.qty,cf_line.monthduration,cf.forecast_id,cf.client_id,cf.startmonth,cf.monthduration AS monthduration_header,
                                        cf.datecreated,cf.created_userid,cf.datemodified,cf.modified_userid,cf.reference FROM contract_sales_forecast_lines cf_line INNER JOIN contract_sales_forecast cf ON cf_line.forecast_id = cf.forecast_id  
                                          WHERE cprod_id = @cprod_id AND (startMonth BETWEEN @from AND @to OR DATE_ADD(startMonth, INTERVAL (CASE WHEN cf_line.monthDuration IS NOT NULL THEN cf_line.monthDuration ELSE cf.monthDuration END) MONTH) BETWEEN @from AND @to) ", conn);
                cmd.Parameters.AddWithValue("@cprod_id", cprod_id);
                cmd.Parameters.AddWithValue("@from", dateFrom);
                cmd.Parameters.AddWithValue("@to", dateTo);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    var line = GetFromDataReader(dr);
                    line.Forecast = Contract_sales_forecastDAL.GetFromDataReader(dr);
                    line.Forecast.monthduration = Utilities.FromDbValue<int>(dr["monthduration_header"]);
                    result.Add(line);
                }
                dr.Close();
            }
            return result;
        }

        public static List<Contract_sales_forecast_lines> GetForPeriod(IList<int> cprod_ids, DateTime dateFrom, DateTime dateTo)
        {
            var result = new List<Contract_sales_forecast_lines>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand("", conn);
                cmd.CommandText =
                    string.Format(
                        @"SELECT cf_line.lines_id,cf_line.forecast_id,cf_line.cprod_id,cf_line.qty,cf_line.monthduration,cf.forecast_id,cf.client_id,cf.startmonth,cf.monthduration AS monthduration_header,
                                        cf.datecreated,cf.created_userid,cf.datemodified,cf.modified_userid,cf.reference FROM contract_sales_forecast_lines cf_line INNER JOIN contract_sales_forecast cf ON cf_line.forecast_id = cf.forecast_id  
                                          WHERE cprod_id IN ({0}) AND (startMonth BETWEEN @from AND @to OR DATE_ADD(startMonth, INTERVAL (CASE WHEN cf_line.monthDuration IS NOT NULL THEN cf_line.monthDuration ELSE cf.monthDuration END) MONTH) BETWEEN @from AND @to) ",
                        Utilities.CreateParametersFromIdList(cmd, cprod_ids));
                cmd.Parameters.AddWithValue("@from", dateFrom);
                cmd.Parameters.AddWithValue("@to", dateTo);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    var line = GetFromDataReader(dr);
                    line.Forecast = Contract_sales_forecastDAL.GetFromDataReader(dr);
                    line.Forecast.monthduration = Utilities.FromDbValue<int>(dr["monthduration_header"]);
                    result.Add(line);
                }
                dr.Close();
            }
            return result;
        }

        public static List<Contract_sales_forecast_lines> GetForMastProductAndPeriod(int mast_id, DateTime dateFrom, DateTime dateTo)
        {
            var result = new List<Contract_sales_forecast_lines>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand(@"SELECT cf_line.lines_id,cf_line.forecast_id,cf_line.cprod_id,cf_line.qty,cf_line.monthduration,cf.forecast_id,cf.client_id,cf.startmonth,cf.monthduration AS monthduration_header,
                                        cf.datecreated,cf.created_userid,cf.datemodified,cf.modified_userid,cf.reference FROM contract_sales_forecast_lines cf_line INNER JOIN contract_sales_forecast cf ON cf_line.forecast_id = cf.forecast_id  
                                        INNER JOIN cust_products ON cf_line.cprod_id = cust_products.cprod_id
                                          WHERE cprod_mast = @mast_id AND (startMonth BETWEEN @from AND @to OR DATE_ADD(startMonth, INTERVAL (CASE WHEN cf_line.monthDuration IS NOT NULL THEN cf_line.monthDuration ELSE cf.monthDuration END) MONTH) BETWEEN @from AND @to) ", conn);
                cmd.Parameters.AddWithValue("@mast_id", mast_id);
                cmd.Parameters.AddWithValue("@from", dateFrom);
                cmd.Parameters.AddWithValue("@to", dateTo);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    var line = GetFromDataReader(dr);
                    line.Forecast = Contract_sales_forecastDAL.GetFromDataReader(dr);
                    line.Forecast.monthduration = Utilities.FromDbValue<int>(dr["monthduration_header"]);
                    result.Add(line);
                }
                dr.Close();
            }
            return result;
        }

        public static List<Contract_sales_forecast_lines> GetByForecastId(int forecast_id)
        {
            List<Contract_sales_forecast_lines> result = new List<Contract_sales_forecast_lines>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT contract_sales_forecast_lines.*, cust_products.cprod_name, cust_products.cprod_code1 AS cprod_code FROM contract_sales_forecast_lines INNER JOIN cust_products ON contract_sales_forecast_lines.cprod_id = cust_products.cprod_id WHERE forecast_id = @forecast_id", conn);
                cmd.Parameters.AddWithValue("@forecast_id", forecast_id);
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
        }
		
		
		public static Contract_sales_forecast_lines GetById(int id)
		{
			Contract_sales_forecast_lines result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM contract_sales_forecast_lines WHERE lines_id = @id", conn);
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
	
		private static Contract_sales_forecast_lines GetFromDataReader(MySqlDataReader dr)
		{
			Contract_sales_forecast_lines o = new Contract_sales_forecast_lines();
		
			o.lines_id =  (int) dr["lines_id"];
			o.forecast_id = Utilities.FromDbValue<int>(dr["forecast_id"]);
			o.cprod_id = Utilities.FromDbValue<int>(dr["cprod_id"]);
            
			o.qty = Utilities.FromDbValue<double>(dr["qty"]);
            o.monthduration = Utilities.FromDbValue<int>(dr["monthduration"]);
		    if (Utilities.ColumnExists(dr, "cprod_name"))
		        o.cprod_name = string.Empty + dr["cprod_name"];
            if (Utilities.ColumnExists(dr, "cprod_code"))
                o.cprod_code = string.Empty + dr["cprod_code"];

		    return o;

		}
		
		public static void Create(Contract_sales_forecast_lines o, MySqlTransaction tr )
        {
            string insertsql = @"INSERT INTO contract_sales_forecast_lines (forecast_id,cprod_id,qty,monthduration) VALUES(@forecast_id,@cprod_id,@qty,@monthduration)";
            var conn = tr != null ? tr.Connection : new MySqlConnection(Properties.Settings.Default.ConnString);
			if(tr == null)
                conn.Open();
            
            var cmd = new MySqlCommand(insertsql, conn, tr);
		    BuildSqlParameters(cmd,o);
            cmd.ExecuteNonQuery();
			cmd.CommandText = "SELECT lines_id FROM contract_sales_forecast_lines WHERE lines_id = LAST_INSERT_ID()";
            o.lines_id = (int) cmd.ExecuteScalar();

            if(tr == null)
		        conn = null;

        }
		
		private static void BuildSqlParameters(MySqlCommand cmd, Contract_sales_forecast_lines o, bool forInsert = true)
        {
			
			if(!forInsert)
				cmd.Parameters.AddWithValue("@lines_id", o.lines_id);
			cmd.Parameters.AddWithValue("@forecast_id", o.forecast_id);
			cmd.Parameters.AddWithValue("@cprod_id", o.cprod_id);
			cmd.Parameters.AddWithValue("@qty", o.qty);
		    cmd.Parameters.AddWithValue("@monthduration", o.monthduration);
        }
		
		public static void Update(Contract_sales_forecast_lines o, MySqlTransaction tr)
		{
			string updatesql = @"UPDATE contract_sales_forecast_lines SET forecast_id = @forecast_id,cprod_id = @cprod_id,qty = @qty, monthduration = @monthduration WHERE lines_id = @lines_id";
            var conn = tr != null ? tr.Connection : new MySqlConnection(Properties.Settings.Default.ConnString);
            if (tr == null)
                conn.Open();

            MySqlCommand cmd = new MySqlCommand(updatesql, conn, tr);
			BuildSqlParameters(cmd,o, false);
            cmd.ExecuteNonQuery();
        }
		
		public static void Delete(int lines_id, MySqlTransaction tr)
		{
            var conn = tr != null ? tr.Connection : new MySqlConnection(Properties.Settings.Default.ConnString);
            if (tr == null)
                conn.Open();

			MySqlCommand cmd = new MySqlCommand("DELETE FROM contract_sales_forecast_lines WHERE lines_id = @id" , conn,tr);
            cmd.Parameters.AddWithValue("@id", lines_id);
            cmd.ExecuteNonQuery();
            
		}

        public static void DeleteByForecast(int forecast_id, MySqlTransaction tr)
        {
            var conn = tr != null ? tr.Connection : new MySqlConnection(Properties.Settings.Default.ConnString);
            if (tr == null)
                conn.Open();

            MySqlCommand cmd = new MySqlCommand("DELETE FROM contract_sales_forecast_lines WHERE forecast_id = @forecast_id", conn, tr);
            cmd.Parameters.AddWithValue("@forecast_id", forecast_id);
            cmd.ExecuteNonQuery();

        }
	}
}
			
			