
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using erp.Model;
using MySql.Data.MySqlClient;

namespace erp.Model.DAL
{
	public class Client_sales_dataDAL
	{

		public const int RangeType_Top10 = 1;
		public const int RangeType_Bottom10 = 2;

		public const int ProductType_Top10 = 1;
		public const int ProductType_Bottom10 = 2;
	
		public static List<Client_sales_data> GetAll()
		{
			var result = new List<Client_sales_data>();
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
			{
				conn.Open();
				var cmd = Utils.GetCommand("SELECT * FROM client_sales_data", conn);
				var dr = cmd.ExecuteReader();
				while (dr.Read())
				{
					result.Add(GetFromDataReader(dr));
				}
				dr.Close();
			}
			return result;
		}

		public static List<Client_sales_data> GetByCriteria(DateTime? from, DateTime? to, string dealer_code = null)
		{
			return GetByCriteria(from, to, new[] {dealer_code});
		}

		public static List<Range> GetRangesForBrand(int brand_id,string cw_code, int range_type = RangeType_Top10, int recordLimit = 10, DateTime? salesFrom = null, DateTime? salesTo = null)
		{
			var result = new List<Range>();

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
			{
				conn.Open();
                var cmd = Utils.GetCommand(string.Format(@"SELECT ranges.rangeid, ranges.range_name,SUM(value) AS Sales FROM cust_products INNER JOIN ranges ON ranges.rangeid = cust_products.cprod_range
											 INNER JOIN client_sales_data ON client_sales_data.cprod_id = cust_products.cprod_id  
											 WHERE brand_id = @brand_id AND invoice_date BETWEEN @from AND @to AND client_sales_data.customer = @code
											 GROUP BY ranges.rangeid, ranges.range_name ORDER BY Sales {0} LIMIT {1}",range_type == RangeType_Top10 ? "DESC" : "",recordLimit), conn);
				cmd.Parameters.AddWithValue("@brand_id", brand_id);
			    cmd.Parameters.AddWithValue("@from", salesFrom ?? DateTime.Now.AddMonths(-12));
			    cmd.Parameters.AddWithValue("@to", salesTo ?? DateTime.Now.Date);
			    cmd.Parameters.AddWithValue("@code", cw_code);
				var dr = cmd.ExecuteReader();
                //int counter = 0;
				while (dr.Read())
				{
				    //if (counter < recordLimit)
				    {
				        var r = new Range();
				        r.rangeid = (int) dr["rangeid"];
				        r.range_name = string.Empty + dr["range_name"];
				        r.Sales = Utilities.FromDbValue<double>(dr["Sales"]);
				        result.Add(r);
				        //counter++;
				    }
				    /*else if(includeOthers)
				    {
                        if (result.Count == recordLimit)
                        {
                            var r = new Range();
                            r.rangeid = -1;
                            r.range_name = "Others";
                            r.Sales = Utilities.FromDbValue<double>(dr["Sales"]);
                            result.Add(r);
                        }
                        else
                        {
                            result[recordLimit].Sales += Utilities.FromDbValue<double>(dr["Sales"]);
                        }
				    }*/
				}
				dr.Close();
			}
			return result;
		}

		public static List<Cust_products> GetProducts(int brand_id, int? range, int range_type = RangeType_Top10)
		{
			var result = new List<Cust_products>();
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
			{
				conn.Open();
				var cmd = Utils.GetCommand(string.Format(@"SELECT cust_products.cprod_id, cust_products.cprod_name FROM cust_products INNER JOIN ranges ON ranges.rangeid = cust_products.cprod_range
											 INNER JOIN client_sales_data ON client_sales_data.cprod_id = cust_products.cprod_id  
											 WHERE brand_id = @brand_id AND (ranges.rangeid = @rangeid OR @rangeid IS NULL)
											 GROUP BY cust_products.cprod_id, cust_products.cprod_name ORDER BY SUM(value) {0} LIMIT 10", range_type == RangeType_Top10 ? "DESC" : ""), conn);

				cmd.Parameters.AddWithValue("@range", range);
				cmd.Parameters.AddWithValue("@brand_id", brand_id);
				var dr = cmd.ExecuteReader();
				while (dr.Read())
				{
					var c = new Cust_products();
					c.cprod_id = (int)dr["cprod_id"];
					c.cprod_name = string.Empty + dr["cprod_name"];
					result.Add(c);
				}
				dr.Close();
			}
			return result;
		}

		public static List<Client_sales_data> GetByCriteria(DateTime? from, DateTime? to, string[] dealer_codes = null, int? brand_id = null, int? range = null, bool? displayedOnly = null,int? topn = null, IList<int> rangeIds = null )
		{
			var result = new List<Client_sales_data>();
			List<Cust_products> displayedProducts = null;
			if (displayedOnly != null)
				displayedProducts = Cust_productsDAL.GetDisplayedComponents(brand_id);
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
			{
				conn.Open();
				var cmd = Utils.GetCommand("", conn);
				cmd.CommandText = string.Format(@"SELECT client_sales_data.id,client_sales_data.cprod_id,client_sales_data.invoice_date,client_sales_data.qty,client_sales_data.`value`,
												client_sales_data.customer,cust_products.* FROM client_sales_data INNER JOIN cust_products ON client_sales_data.cprod_id = cust_products.cprod_id
												WHERE (invoice_date >= @from OR @from IS NULL) AND (invoice_date <= @to OR @to IS NULL) AND (cust_products.brand_id = @brand_id OR @brand_id IS NULL)
													AND (cust_products.cprod_range = @range OR (@range IS NULL {1})) 
													{0} ",dealer_codes != null ? string.Format(" AND customer IN ({0})", Utils.CreateParametersFromIdList(cmd,dealer_codes)) : "",
                                                         rangeIds != null && rangeIds.Count > 0 ? string.Format(" AND cust_products.cprod_range IN ({0})", Utils.CreateParametersFromIdList(cmd,rangeIds,"rangeid")) : "");
				cmd.Parameters.AddWithValue("@from", Utilities.ToDBNull(from));
				cmd.Parameters.AddWithValue("@to", Utilities.ToDBNull(to));
				cmd.Parameters.AddWithValue("@brand_id", Utilities.ToDBNull(brand_id));
				cmd.Parameters.AddWithValue("@range", Utilities.ToDBNull(range));
				var dr = cmd.ExecuteReader();
				while (dr.Read())
				{
					var csd = GetFromDataReader(dr);
					csd.Product = Cust_productsDAL.GetFromDataReader(dr);
					result.Add(csd);
				}
				dr.Close();

			}
			return result.Where(d=>displayedOnly==true ? displayedProducts.Count(dp=>dp.cprod_id == d.cprod_id)>0 : true).ToList();
		}

		public static List<ClientSalesAggregate> GetForTopNProducts(int numOfProducts, int? brand_id = null, int? range = null, string[] dealer_codes = null,DateTime? from = null, DateTime? to = null)
		{
			var result = new List<ClientSalesAggregate>();
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
			{
				conn.Open();
				var cmd = Utils.GetCommand("", conn);
				cmd.CommandText = string.Format(@"SELECT cust_products.cprod_id, cust_products.cprod_name, cust_products.cprod_code1, SUM(qty) AS qty, SUM(value) {2} AS sales FROM 
									client_sales_data INNER JOIN cust_products ON client_sales_data.cprod_id = cust_products.cprod_id
									WHERE (cust_products.brand_id = @brand_id OR @brand_id IS NULL) AND (cust_products.cprod_range = @range OR @range IS NULL) 
									AND (invoice_date >= @from OR @from IS NULL) AND (invoice_date <= @to OR @to IS NULL) {1}
									GROUP BY cust_products.cprod_id ORDER BY sales DESC LIMIT {0}",numOfProducts,
									dealer_codes != null ? string.Format(" AND customer IN ({0})", Utils.CreateParametersFromIdList(cmd, dealer_codes)) : "",
									dealer_codes != null && dealer_codes.Length> 0 ? string.Format("/ {0}",dealer_codes.Length) : ""
									);
				
				cmd.Parameters.AddWithValue("@brand_id", Utilities.ToDBNull(brand_id));
				cmd.Parameters.AddWithValue("@range", Utilities.ToDBNull(range));
				cmd.Parameters.AddWithValue("@from", Utilities.ToDBNull(from));
				cmd.Parameters.AddWithValue("@to", Utilities.ToDBNull(to));
				var dr = cmd.ExecuteReader();
				while (dr.Read())
				{
					result.Add(new ClientSalesAggregate{Id = (int) dr["cprod_id"],Code=string.Empty + dr["cprod_code1"],Name=string.Empty + dr["cprod_name"],Qty = Utilities.FromDbValue<int>(dr["qty"]), Sum = Utilities.FromDbValue<double>(dr["sales"])});
				}
				dr.Close();

			}
			return result;
		}

        public static List<ClientSalesAggregate> GetForTopBottomNProducts(int numOfProducts, int? brand_id = null, int? range = null, string[] dealer_codes = null, DateTime? from = null, DateTime? to = null, bool top = true)
        {
            var result = new List<ClientSalesAggregate>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("", conn);
                cmd.CommandText = string.Format(@"SELECT cust_products.cprod_id, cust_products.cprod_name, cust_products.cprod_code1, SUM(qty*value) {2} AS sales FROM 
									client_sales_data INNER JOIN cust_products ON client_sales_data.cprod_id = cust_products.cprod_id
									WHERE (cust_products.brand_id = @brand_id OR @brand_id IS NULL) AND (cust_products.cprod_range = @range OR @range IS NULL) 
									AND (invoice_date >= @from OR @from IS NULL) AND (invoice_date <= @to OR @to IS NULL) {1}
									GROUP BY cust_products.cprod_id ORDER BY sales {3} LIMIT {0}", numOfProducts,
                                    dealer_codes != null ? string.Format(" AND customer IN ({0})", Utils.CreateParametersFromIdList(cmd, dealer_codes)) : "",
                                    dealer_codes != null && dealer_codes.Length > 0 ? string.Format("/ {0}", dealer_codes.Length) : "",top ? "DESC" : ""
                                    );

                cmd.Parameters.AddWithValue("@brand_id", Utilities.ToDBNull(brand_id));
                cmd.Parameters.AddWithValue("@range", Utilities.ToDBNull(range));
                cmd.Parameters.AddWithValue("@from", Utilities.ToDBNull(from));
                cmd.Parameters.AddWithValue("@to", Utilities.ToDBNull(to));
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(new ClientSalesAggregate { Id = (int)dr["cprod_id"], Code = string.Empty + dr["cprod_code1"], Name = string.Empty + dr["cprod_name"], Sum = Utilities.FromDbValue<double>(dr["sales"]) });
                }
                dr.Close();

            }
            return result;
        }


		public static List<Dealer> GetDealersInRadius(Dealer d, double radius, bool? displaying = null)
		{
			var result = new List<Dealer>();
			if (d.latitude != null && d.longitude != null)
			{
				using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
				{
					conn.Open();
					var dealers = new List<Dealer>();
					var cmd =
						Utils.GetCommand(
							string.Format(@"SELECT user_id, latitude, longitude, cw_code FROM dealers WHERE cw_code IS NOT NULL {0}",
								displaying != null ? 
								string.Format(@" AND {0} EXISTS (SELECT image_id FROM dealer_image_displays INNER JOIN dealer_images ON dealer_image_displays.image_id = dealer_images.image_unique 
																 WHERE dealer_images.dealer_id = dealers.user_id)",displaying == true ? "" : "NOT") : ""),conn);
					var dr = cmd.ExecuteReader();
					while (dr.Read())
					{
						dealers.Add(new Dealer
						{
							user_id = (int)dr["user_id"],
							latitude = Utilities.FromDbValue<double>(dr["latitude"]),
							longitude = Utilities.FromDbValue<double>(dr["longitude"]),
							cw_code = string.Empty + dr["cw_code"]
						});
					}
					dr.Close();

					cmd =
						Utils.GetCommand(
							"SELECT customer FROM client_sales_data GROUP BY customer ",
							conn);
					dr = cmd.ExecuteReader();
					while (dr.Read())
					{
						var code = string.Empty + dr["customer"];
						var dealer = dealers.FirstOrDefault(de => de.cw_code == code);
						if (dealer != null && dealer.latitude != null && dealer.longitude != null && dealer.user_id != d.user_id && 
							GeoUtils.distance(dealer.latitude.Value, dealer.longitude.Value, d.latitude.Value,
											  d.longitude.Value, 'M') <= radius)
						{
							result.Add(dealer);
						}
					}
					dr.Close();
				}

			}
			return result;
		}


		public static Client_sales_data GetById(int id)
		{
			Client_sales_data result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
			{
				conn.Open();
				var cmd = Utils.GetCommand("SELECT * FROM client_sales_data WHERE id = @id", conn);
				cmd.Parameters.AddWithValue("@id", id);
				var dr = cmd.ExecuteReader();
				if (dr.Read())
				{
					result = GetFromDataReader(dr);
				}
				dr.Close();
			}
			return result;
		}
		
	
		public static Client_sales_data GetFromDataReader(MySqlDataReader dr)
		{
			var o = new Client_sales_data();
		
			o.id =  (int) dr["id"];
			o.cprod_id = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"cprod_id"));
			o.invoice_date = Utilities.FromDbValue<DateTime>(Utilities.GetReaderField(dr,"invoice_date"));
			o.qty = Utilities.FromDbValue<double>(Utilities.GetReaderField(dr,"qty"));
			o.value = Utilities.FromDbValue<double>(Utilities.GetReaderField(dr,"value"));
			o.customer = string.Empty + dr["customer"];
			o.cprod_code1 = string.Empty + dr["cprod_code1"];
			return o;

		}
		
		
		public static void Create(Client_sales_data o)
		{
			string insertsql = @"INSERT INTO client_sales_data (cprod_id,invoice_date,qty,value,cprod_code1) VALUES(@cprod_id,@invoice_date,@qty,@value,@cprod_code1)";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
			{
				conn.Open();
				
				var cmd = Utils.GetCommand(insertsql, conn);
				BuildSqlParameters(cmd,o);
				cmd.ExecuteNonQuery();
				cmd.CommandText = "SELECT id FROM client_sales_data WHERE id = LAST_INSERT_ID()";
				o.id = (int) cmd.ExecuteScalar();
				
			}
		}
		
		private static void BuildSqlParameters(MySqlCommand cmd, Client_sales_data o, bool forInsert = true)
		{
			
			if(!forInsert)
				cmd.Parameters.AddWithValue("@id", o.id);
			cmd.Parameters.AddWithValue("@cprod_id", o.cprod_id);
			cmd.Parameters.AddWithValue("@invoice_date", o.invoice_date);
			cmd.Parameters.AddWithValue("@qty", o.qty);
			cmd.Parameters.AddWithValue("@value", o.value);
			cmd.Parameters.AddWithValue("@cprod_code1", o.cprod_code1);
		}
		
		public static void Update(Client_sales_data o)
		{
			string updatesql = @"UPDATE client_sales_data SET cprod_id = @cprod_id,invoice_date = @invoice_date,qty = @qty,value = @value,cprod_code1 = @cprod_code1 WHERE id = @id";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
			{
				conn.Open();
				var cmd = Utils.GetCommand(updatesql, conn);
				BuildSqlParameters(cmd,o, false);
				cmd.ExecuteNonQuery();
			}
		}
		
		public static void Delete(int id)
		{
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
			{
				conn.Open();
				var cmd = Utils.GetCommand("DELETE FROM client_sales_data WHERE id = @id" , conn);
				cmd.Parameters.AddWithValue("@id", id);
				cmd.ExecuteNonQuery();
			}
		}
		
		
	}

	
}
			
			