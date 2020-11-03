
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace erp.Model.DAL
{
    public class Sales_dataDAL
	{
	
		public static List<Sales_data> GetAll()
		{
			List<Sales_data> result = new List<Sales_data>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = Utils.GetCommand("SELECT * FROM sales_data", conn);
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
        /* sale period (clearance sale)*/
        public static ClearanceSale GetClearanceSale(string webPage="")
        {
            ClearanceSale clearanceSale = new ClearanceSale();
           // int result=1;
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString)) 
            {
                conn.Open();
                var cmd=Utils.GetCommand("SELECT count(IdSale) AS IsSale FROM sale WHERE NOW() BETWEEN SaleStart and SaleEnd AND WebSiteId = @webPage",conn);
                //var fromDbValue = Utilities.FromDbValue<int>(cmd.ExecuteScalar());
                cmd.Parameters.AddWithValue("@webPage", webPage);

                MySqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    clearanceSale.CountSale=Utilities.FromDbValue<int>( dr["IsSale"]);
                }
                //if (fromDbValue != null)
                //    result = (int) fromDbValue;
                //else
                //{
                //    result = 0;
                //}
            }


            return clearanceSale;
        }

        public static List<Cust_products> GetForProdUserAndPeriod(int monthFrom, int monthTo, IList<int> cprod_user = null ) 
       
        {
            //public static List<Sales_data> GetForProdUserAndPeriod(int monthFrom, int monthTo, IList<int> cprod_user = null)
           // var result = new List<Sales_data>();
            var result=new List<Cust_products>();
           
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
               conn.Open();
                var cmd=Utils.GetCommand("",conn);
                cmd.CommandText =string.Format("SELECT * FROM asaq.cust_products " +
                                               "INNER JOIN asaq.sales_data ON cust_products.cprod_id = sales_data.cprod_id " +
                                               "WHERE {0} AND month21 BETWEEN @from AND @to ORDER BY cust_products.cprod_id",cprod_user != null ? string.Format("cprod_user IN ({0})",Utils.CreateParametersFromIdList(cmd,cprod_user)):"");
                //cmd.Parameters.AddWithValue("@cprod_user", cprod_user);
                cmd.Parameters.AddWithValue("@from", monthFrom);
                cmd.Parameters.AddWithValue("@to", monthTo);
               
                var dr = cmd.ExecuteReader();
                Cust_products cprod = null;
                var cprod_id = -1;
                while (dr.Read())
                {
                    //Sales_data s = GetFromDataReader(dr);
                    //s.Product = Cust_productsDAL.GetFromDataReader(dr);

                    var id = (int) dr["cprod_id"];
                    var sales = GetFromDataReader(dr);
                    if (id != cprod_id)
                    {
                        cprod_id = id;
                        cprod = Cust_productsDAL.GetFromDataReader(dr);

                        //s.SalesProducts = GetFromDataReader(dr);
                        result.Add(cprod);
                        cprod.SalesProducts = new List<Sales_data>();
                        
                    }
                    cprod.SalesProducts.Add(sales);
                }
                dr.Close();
            }
           
            return result;
        }

        public static List<Sales_data> GetForPeriod(int cprod_id, int monthFrom, int monthTo)
        {
            return GetForPeriod(new List<int>{cprod_id},monthFrom,monthTo);
        }

        /***/
        public static List<Sales_data> GetForPeriod(IList<int> cprod_ids,int monthFrom, int monthTo)
        {
            var result = new List<Sales_data>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand("",conn); 
                cmd.CommandText = string.Format("SELECT * FROM sales_data WHERE cprod_id IN ({0}) AND month21 BETWEEN @from AND @to", Utils.CreateParametersFromIdList(cmd,cprod_ids));
        
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

        public static List<Sales_data> GetForCompanyAndPeriod(IList<int> company_ids, int monthFrom, int monthTo)
        {
            var result = new List<Sales_data>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("", conn);
                cmd.CommandText =
                    string.Format(@"SELECT sales_data.* FROM sales_data INNER JOIN cust_products ON sales_data.cprod_id = cust_products.cprod_id 
                                    WHERE cust_products.cprod_user IN({0}) AND month21 BETWEEN @from AND @to",
                                  Utils.CreateParametersFromIdList(cmd, company_ids));
                
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

        public static List<Sales_data> GetForMastProdAndPeriod(int mast_id, int monthFrom, int monthTo)
        {
            return GetForMastProdAndPeriod(new List<int>{mast_id},monthFrom,monthTo);
        }

        public static List<Sales_data> GetForMastProdAndPeriod(IList<int> mast_ids, int monthFrom, int monthTo)
        {
            var result = new List<Sales_data>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand("",conn);
                cmd.CommandText = string.Format(@"SELECT sales_data.*, cust_products.* FROM sales_data INNER JOIN cust_products ON sales_data.cprod_id = cust_products.cprod_id 
                                             WHERE cust_products.cprod_mast IN ({0}) AND month21 BETWEEN @from AND @to", Utils.CreateParametersFromIdList(cmd,mast_ids));
                
                cmd.Parameters.AddWithValue("@from", monthFrom);
                cmd.Parameters.AddWithValue("@to", monthTo);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    var data = GetFromDataReader(dr);
                    data.Product = Cust_productsDAL.GetFromDataReader(dr);
                    result.Add(data);
                }
                dr.Close();
            }
            return result;
        }
		
		
		public static Sales_data GetById(int id)
		{
			Sales_data result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = Utils.GetCommand("SELECT * FROM sales_data WHERE sales_unique = @id", conn);
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

        public static Sales_data GetByProdAndMonth(int cprod_id, int month)
        {
            Sales_data result = null;
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = Utils.GetCommand("SELECT * FROM sales_data WHERE cprod_id = @cprod_id AND month21 = @month", conn);
                cmd.Parameters.AddWithValue("@cprod_id", cprod_id);
                cmd.Parameters.AddWithValue("@month", month);
                MySqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    result = GetFromDataReader(dr);
                }
                dr.Close();
            }
            return result;
        }
	
		public static Sales_data GetFromDataReader(MySqlDataReader dr)
		{
			Sales_data o = new Sales_data();
		
			o.sales_unique =  (int) dr["sales_unique"];
			o.cprod_id = Utilities.FromDbValue<int>(dr["cprod_id"]);
			o.sales_qty = Utilities.FromDbValue<int>(dr["sales_qty"]);
			o.month21 = Utilities.FromDbValue<int>(dr["month21"]);
			
			return o;

		}
		
		public static void Create(Sales_data o)
        {
            string insertsql = @"INSERT INTO sales_data (cprod_id,sales_qty,month21) VALUES(@cprod_id,@sales_qty,@month21)";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				
				MySqlCommand cmd = Utils.GetCommand(insertsql, conn);
                BuildSqlParameters(cmd,o);
                cmd.ExecuteNonQuery();
				cmd.CommandText = "SELECT sales_unique FROM sales_data WHERE sales_unique = LAST_INSERT_ID()";
                o.sales_unique = (int) cmd.ExecuteScalar();
				
            }
		}
		
		private static void BuildSqlParameters(MySqlCommand cmd, Sales_data o, bool forInsert = true)
        {
			
			if(!forInsert)
				cmd.Parameters.AddWithValue("@sales_unique", o.sales_unique);
			cmd.Parameters.AddWithValue("@cprod_id", o.cprod_id);
			cmd.Parameters.AddWithValue("@sales_qty", o.sales_qty);
			cmd.Parameters.AddWithValue("@month21", o.month21);
		}
		
		public static void Update(Sales_data o)
		{
			string updatesql = @"UPDATE sales_data SET cprod_id = @cprod_id,sales_qty = @sales_qty,month21 = @month21 WHERE sales_unique = @sales_unique";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				MySqlCommand cmd = Utils.GetCommand(updatesql, conn);
                BuildSqlParameters(cmd,o, false);
                cmd.ExecuteNonQuery();
            }
		}
		
		public static void Delete(int sales_unique)
		{
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				MySqlCommand cmd = Utils.GetCommand("DELETE FROM sales_data WHERE sales_unique = @id" , conn);
                cmd.Parameters.AddWithValue("@id", sales_unique);
                cmd.ExecuteNonQuery();
            }
		}

        public static void DeleteByIdAndMonth(int cprod_id, int? monthFrom = null, int? monthTo = null)
        {
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = Utils.GetCommand("DELETE FROM sales_data WHERE cprod_id = @cprod_id AND (month21>= @monthFrom OR @monthFrom IS NULL) AND (month21<= @monthTo OR @monthTo IS NULL)", conn);
                cmd.Parameters.AddWithValue("@cprod_id", cprod_id);
                cmd.Parameters.AddWithValue("@monthFrom", monthFrom);
                cmd.Parameters.AddWithValue("@monthTo", monthTo);
                cmd.ExecuteNonQuery();
            }
        }
        //private static ClearanceSale GetFromDataReaderSale()
        //{
        //    ClearanceSale 
        //}
	}


    
    public class ClearanceSale
    {
        public int IdSale { get; set; }
        public string WebSiteId { get; set; }
        public DateTime SaleStart { get; set; }
        public DateTime SaleEnd { get; set; }
        /*
         * if is not sale count=0
         */
        public int? CountSale { get; set; }
    }
}
			
			