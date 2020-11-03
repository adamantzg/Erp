
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;
using erp.Model.DAL.Properties;

namespace erp.Model.DAL
{
    public class Sales_forecastDAL
	{
	
		public static List<Sales_forecast> GetAll()
		{
			List<Sales_forecast> result = new List<Sales_forecast>();
            using (var conn = new MySqlConnection(Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = Utils.GetCommand("SELECT * FROM sales_forecast", conn);
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}
        public static List<Sales_forecast> GetForecastForPeriod(int monthFrom, int monthTo, List<int> numCprodUser)
        {
            List<Sales_forecast> result = new List<Sales_forecast>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("", conn);

                cmd.CommandText = string.Format("SELECT * FROM sales_forecast " +
                                                "INNER JOIN cust_products ON sales_forecast.cprod_id=cust_products.cprod_id " +
                                                "WHERE {0} AND month21 BETWEEN @from AND @to",
                                                numCprodUser != null ? string.Format(" cprod_user IN ({0})",Utils.CreateParametersFromIdList(cmd,numCprodUser)):null);
                    
                cmd.Parameters.AddWithValue("@from", monthFrom);
                cmd.Parameters.AddWithValue("@to", monthTo);

                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
        }
        /***/
        /* part 1 */

        //public static List<Cust_products> GetForecastForProdUserAndPeriod(int monthFrom, int monthTo, List<int> cprod_user = null)
        //{
        //    var result = new List<Cust_products>();

        //    using (var conn = new MySqlConnection(Settings.Default.ConnString))
        //    {
        //        conn.Open();
        //        var cmd = Utils.GetCommand("", conn);
        //        cmd.CommandText = string.Format("SELECT * FROM cust_products " +
        //                                            " INNER JOIN sales_forecast ON cust_products.cprod_id=sales_forecast.cprod_id " +
        //                                            "WHERE {0} AND month21 BETWEEN @from AND @to", cprod_user != null ? string.Format("cprod_user IN ({0})", Utilities.CreateParametersFromIdList(cmd, cprod_user)) : "");
                                                    
        //        //cmd.Parameters.AddWithValue("@cprod_user", cprod_user);
        //        cmd.Parameters.AddWithValue("@from", monthFrom);
        //        cmd.Parameters.AddWithValue("@to", monthTo);
        //        MySqlDataReader dr = cmd.ExecuteReader();
        //        while (dr.Read())
        //        {
        //            //result.Add(GetFromDataReader(dr));
        //            Cust_products sf = Cust_productsDAL.GetFromDataReader(dr);
        //            sf.SalesForecastProducts = GetFromDataReader(dr);
        //            result.Add(sf);
        //        }
        //        dr.Close();
        //    }

        //    return result;
        //}

        /***/

        public static List<Sales_forecast> GetForPeriod( int cprod_id,int monthFrom, int monthTo)
        {
            var result = new List<Sales_forecast>();
            using (var conn = new MySqlConnection(Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM sales_forecast_all WHERE cprod_id = @cprod_id AND month21 BETWEEN @from AND @to", conn);
                cmd.Parameters.AddWithValue("@cprod_id", cprod_id);
                cmd.Parameters.AddWithValue("@from", monthFrom);
                cmd.Parameters.AddWithValue("@to", monthTo);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
        }

        public static List<Sales_forecast> GetForPeriod(IList<int> cprod_ids, int monthFrom, int monthTo)
        {
            var result = new List<Sales_forecast>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("", conn);
                cmd.CommandText =
                    string.Format(
                        "SELECT * FROM sales_forecast_all WHERE cprod_id IN ({0}) AND month21 BETWEEN @from AND @to",
                        Utils.CreateParametersFromIdList(cmd,cprod_ids));
                
                cmd.Parameters.AddWithValue("@from", monthFrom);
                cmd.Parameters.AddWithValue("@to", monthTo);
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
        }

        public static List<Sales_forecast> GetForMastProdAndPeriod(int id, int monthFrom, int monthTo)
        {
            return GetForMastProdAndPeriod(new List<int>{id},monthFrom,monthTo );
        }

        public static List<Sales_forecast> GetForMastProdAndPeriod(IList<int> ids, int monthFrom, int monthTo)
        {
            var result = new List<Sales_forecast>();
            using (var conn = new MySqlConnection(Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand("",conn);
                cmd.CommandText =
                    string.Format(
                        @"SELECT * FROM sales_forecast_all INNER JOIN cust_products ON sales_forecast_all.cprod_id = cust_products.cprod_id 
                                                    WHERE cust_products.cprod_mast IN ({0}) AND month21 BETWEEN @from AND @to",
                        Utils.CreateParametersFromIdList(cmd, ids));
                
                cmd.Parameters.AddWithValue("@from", monthFrom);
                cmd.Parameters.AddWithValue("@to", monthTo);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
        }
		
		public static Sales_forecast GetById(int id)
		{
			Sales_forecast result = null;
			using (var conn = new MySqlConnection(Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = Utils.GetCommand("SELECT * FROM sales_forecast WHERE sales_unique = @id", conn);
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

        public static Sales_forecast GetFromDataReader(MySqlDataReader dr)
		{
			Sales_forecast o = new Sales_forecast();
		
			o.sales_unique =  (int) dr["sales_unique"];
			o.cprod_id = Utilities.FromDbValue<int>(dr["cprod_id"]);
			o.sales_qty = Utilities.FromDbValue<int>(dr["sales_qty"]);
			o.month21 = Utilities.FromDbValue<int>(dr["month21"]);
            if (Utilities.ColumnExists(dr, "type"))
                o.Type = (ForecastType) Convert.ToInt32(dr["type"]);
			//o.sales_qty_c = Utilities.FromDbValue<int>(dr["sales_qty_c"]);
			
			return o;

		}
		
		public static void Create(Sales_forecast o)
        {
            string insertsql = @"INSERT INTO sales_forecast (cprod_id,sales_qty,month21) VALUES(@cprod_id,@sales_qty,@month21)";

			using (var conn = new MySqlConnection(Settings.Default.ConnString))
            {
                conn.Open();
				
				MySqlCommand cmd = Utils.GetCommand(insertsql, conn);
                BuildSqlParameters(cmd,o);
                cmd.ExecuteNonQuery();
				cmd.CommandText = "SELECT sales_unique FROM sales_forecast WHERE sales_unique = LAST_INSERT_ID()";
                o.sales_unique = (int) cmd.ExecuteScalar();
				
            }
		}
		
		private static void BuildSqlParameters(MySqlCommand cmd, Sales_forecast o, bool forInsert = true)
        {
			
			if(!forInsert)
				cmd.Parameters.AddWithValue("@sales_unique", o.sales_unique);
			cmd.Parameters.AddWithValue("@cprod_id", o.cprod_id);
			cmd.Parameters.AddWithValue("@sales_qty", o.sales_qty);
			cmd.Parameters.AddWithValue("@month21", o.month21);
			
		}
		
		public static void Update(Sales_forecast o)
		{
			string updatesql = @"UPDATE sales_forecast SET cprod_id = @cprod_id,sales_qty = @sales_qty,month21 = @month21 WHERE sales_unique = @sales_unique";

			using (var conn = new MySqlConnection(Settings.Default.ConnString))
            {
                conn.Open();
				MySqlCommand cmd = Utils.GetCommand(updatesql, conn);
                BuildSqlParameters(cmd,o, false);
                cmd.ExecuteNonQuery();
            }
		}
		
		public static void Delete(int sales_unique)
		{
			using (var conn = new MySqlConnection(Settings.Default.ConnString))
            {
                conn.Open();
				MySqlCommand cmd = Utils.GetCommand("DELETE FROM sales_forecast WHERE sales_unique = @id" , conn);
                cmd.Parameters.AddWithValue("@id", sales_unique);
                cmd.ExecuteNonQuery();
            }
		}

        public static void DeleteByIdAndMonth(int cprod_id, int? monthFrom = null, int? monthTo = null)
        {
            using (var conn = new MySqlConnection(Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = Utils.GetCommand("DELETE FROM sales_forecast WHERE cprod_id = @cprod_id AND (month21>= @monthFrom OR @monthFrom IS NULL) AND (month21<= @monthTo OR @monthTo IS NULL)", conn);
                cmd.Parameters.AddWithValue("@cprod_id", cprod_id);
                cmd.Parameters.AddWithValue("@monthFrom", monthFrom);
                cmd.Parameters.AddWithValue("@monthTo", monthTo);
                cmd.ExecuteNonQuery();
            }
        }
	}
}
			
			