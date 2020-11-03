
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;
using erp.Model.DAL.Properties;
using Dapper;

namespace erp.Model.DAL
{
	public enum OrderDateMode
	{
		Etd,
		Eta,
		Sale,
		EtaPlusWeek,
		OrderDate
	}

	public class Order_linesDAL
	{

		public const string LinesJoin =
			@"order_lines INNER JOIN order_header ON order_lines.orderid = order_header.orderid 
										  INNER JOIN porder_lines ON porder_lines.soline = order_lines.linenum
										  INNER JOIN porder_header ON porder_lines.porderid = porder_header.porderid";
	
		public static List<Order_lines> GetByOrderId(int order_id, IDbConnection conn = null)
		{
			var result = new List<Order_lines>();
		    var dispose = false;
		    if (conn == null)
		    {
		        conn = new MySqlConnection(Properties.Settings.Default.ConnString);
		        dispose = true;
                conn.Open();
		    }

		    {
		        var cmd = Utils.GetCommand(@"SELECT order_lines.*, cust_products.*,order_header.*, mast_products.*, factory.*,
								(CASE order_header.stock_order 
									WHEN 1 THEN 
									   DATE_ADD(order_header.orderdate,INTERVAL (CASE WHEN mast_products.factory_id IN (16,18) THEN 4 ELSE 6 END) MONTH)
									ELSE COALESCE(porder_header.po_req_etd, DATE_ADD(order_header.req_eta, INTERVAL -35 DAY)) END) AS po_req_etd
									FROM porder_lines INNER JOIN porder_header ON porder_lines.porderid = porder_header.porderid 
								RIGHT OUTER JOIN order_lines ON porder_lines.soline = order_lines.linenum INNER JOIN cust_products ON order_lines.cprod_id = cust_products.cprod_id INNER JOIN order_header ON 
									order_lines.orderid = order_header.orderid INNER JOIN mast_products ON cust_products.cprod_mast = mast_products.mast_id INNER JOIN users factory ON mast_products.factory_id = factory.user_id
									WHERE order_lines.orderid = @order_id", (MySqlConnection) conn);
		        cmd.Parameters.AddWithValue("@order_id", order_id);
		        var dr = cmd.ExecuteReader();
		        while (dr.Read())
		        {
		            var line = GetFromDataReader(dr);
		            result.Add(line);
		            line.Header = Order_headerDAL.GetFromDataReader(dr);
		            line.Cust_Product = Cust_productsDAL.GetFromDataReader(dr);
		            if (line.Cust_Product.cprod_mast != null)
		            {
		                line.Cust_Product.MastProduct = Mast_productsDAL.GetFromDataReader(dr);
		                    // Mast_productsDAL.GetById(line.Cust_Product.cprod_mast.Value);
		                if (line.Cust_Product.MastProduct.factory_id != null)
		                    line.Cust_Product.MastProduct.Factory = CompanyDAL.GetFromDataReader(dr);
		                //CompanyDAL.GetById(line.Cust_Product.MastProduct.factory_id.Value);
		            }
		        }
		        dr.Close();
		    }
            if(dispose)
                conn.Dispose();
		    return result;
		}

		/***/
		public static List<Order_lines> GetByOrder()
		{
			var result = new List<Order_lines>();
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
			{
				conn.Open();
				var cmd = Utils.GetCommand("SELECT " +
											   "Count(order_line_detail2_v6.orderid) AS orderFreq , " +
												"order_line_detail2_v6.cprod_id " +
												"FROM order_line_detail2_v6 " +
										   "INNER JOIN cust_products ON cust_products.cprod_id = order_line_detail2_v6.cprod_id  " +
										   "WHERE order_line_detail2_v6.orderqty > 0 " +
											   "AND order_line_detail2_v6.req_eta >= DATE_SUB(NOW(),INTERVAL 12 MONTH) " +
											   "AND order_line_detail2_v6.stock_order IN (8) " +
											   "AND cust_products.cprod_user IN (45,55,149) " +
										   "GROUP BY order_line_detail2_v6.cprod_id", conn);
				var dr = cmd.ExecuteReader();
				while (dr.Read())
				{
					result.Add(new Order_lines
						{
							orderid = Convert.ToInt32(dr["orderFreq"]),
							cprod_id = Convert.ToInt32(dr["cprod_id"])

						});
				}
				dr.Close();
			}



			return result;
		} 


		/***/
        public static List<Order_line_detail2_v6> GetOrderingPatterns(string customer_code,DateTime ordersFrom,DateTime ordersTo)
        {
            var result = new List<Order_line_detail2_v6>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("",conn);
                cmd.CommandText=string.Format(@"SELECT
                                                    order_line_detail2_v6.linenum,
                                                    order_line_detail2_v6.orderid,
                                                    COUNT(order_line_detail2_v6.orderid)as 'counted_orders',
                                                    order_line_detail2_v6.linedate,
                                                DAYNAME(order_line_detail2_v6.linedate) AS 'week_day',
                                                SUM(order_line_detail2_v6.orderqty),
                                                    order_line_detail2_v6.cprod_user,
                                                    order_line_detail2_v6.customer_code,
                                                    order_line_detail2_v6.container_type
                                                FROM
                                                    order_line_detail2_v6
                                                WHERE
                                                    order_line_detail2_v6.container_type IN (0, 1, 2, 6) AND
                                                    order_line_detail2_v6.customer_code IN ({0}) AND
                                                    order_line_detail2_v6.linedate BETWEEN @from and @to
                                                GROUP BY
                                                orderid", customer_code);
                cmd.Parameters.AddWithValue("from", ordersFrom);
                cmd.Parameters.AddWithValue("to", ordersTo);
                var dr = cmd.ExecuteReader();
                while(dr.Read())
                {
                    result.Add(new Order_line_detail2_v6
                    {
                        orderid= Convert.ToInt32(dr["linenum"]),
                        counted_orders =  Convert.ToInt32(dr["counted_orders"]),
                        week_day=string.Empty+dr["week_day"],
                        customer_code=string.Empty+dr["customer_code"],
                        linedate = Utilities.FromDbValue<DateTime>(dr["linedate"])


                        
                    });
                }
                dr.Close();

            }
            return result;
        }

		public static List<Order_lines> GetByCustPo(string custPo, int? company_id = null)
		{
			var result = new List<Order_lines>();
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
			{
				conn.Open();
				var cmd = Utils.GetCommand(@"SELECT order_lines.*, cust_products.*,order_header.*,
							(CASE order_header.stock_order 
									WHEN 1 THEN 
									   DATE_ADD(order_header.orderdate,INTERVAL (CASE WHEN mast_products.factory_id IN (16,18) THEN 4 ELSE 6 END) MONTH)
									ELSE COALESCE(porder_header.po_req_etd, DATE_ADD(order_header.req_eta, INTERVAL -35 DAY)) END) AS po_req_etd
														FROM porder_lines INNER JOIN porder_header ON porder_lines.porderid = porder_header.porderid 
								RIGHT OUTER JOIN order_lines ON porder_lines.soline = order_lines.linenum INNER JOIN cust_products ON order_lines.cprod_id = cust_products.cprod_id INNER JOIN order_header ON 
														order_lines.orderid = order_header.orderid INNER JOIN mast_products ON cust_products.cprod_mast = mast_products.mast_id
														WHERE order_header.custpo = @custpo AND (order_header.userid1 = @company_id OR @company_id IS NULL)", conn);
				cmd.Parameters.AddWithValue("@custpo", custPo);
				cmd.Parameters.AddWithValue("@company_id", company_id != null ? (object) company_id : DBNull.Value);
				var dr = cmd.ExecuteReader();
				while (dr.Read())
				{
					var line = GetFromDataReader(dr);
					result.Add(line);
					line.Header = Order_headerDAL.GetFromDataReader(dr);
					line.Cust_Product = Cust_productsDAL.GetFromDataReader(dr);
					if (line.Cust_Product.cprod_mast != null)
					{
						line.Cust_Product.MastProduct = Mast_productsDAL.GetById(line.Cust_Product.cprod_mast.Value);
						if (line.Cust_Product.MastProduct.factory_id != null)
							line.Cust_Product.MastProduct.Factory =
								CompanyDAL.GetById(line.Cust_Product.MastProduct.factory_id.Value);
					}
				}
				dr.Close();
			}
			return result;
		}

		public static List<Order_lines> GetByCustPos(string[] custPos)
		{
			var result = new List<Order_lines>();
			if (custPos.Length > 0)
			{
				using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
				{
					conn.Open();
					var cmd = Utils.GetCommand(string.Empty, conn);
					var sql =
						string.Format(@"SELECT order_lines.*, cust_products.*, order_header.*,(SELECT MAX(po_req_etd) FROM porder_header WHERE soorderid = order_header.orderid) AS po_req_etd,
														porder_header.special_comments, mast_products.factory_ref,mast_products.special_comments AS product_special_comments
														FROM order_lines INNER JOIN cust_products ON order_lines.cprod_id = cust_products.cprod_id 
														INNER JOIN mast_products ON cust_products.cprod_mast = mast_products.mast_id INNER JOIN order_header ON 
														order_lines.orderid = order_header.orderid INNER JOIN porder_lines ON porder_lines.soline = order_lines.linenum INNER JOIN 
														porder_header ON porder_lines.porderid = porder_header.porderid
														WHERE order_header.status NOT IN ('X','Y') AND (order_lines.orderqty > 0) 
														AND order_header.custpo IN ({0})",
									  Utils.CreateParametersFromIdList(cmd, new List<string>(custPos)));
					cmd.CommandText = sql;
					var dr = cmd.ExecuteReader();
					while (dr.Read())
					{
						var line = GetFromDataReader(dr);
						line.Cust_Product = Cust_productsDAL.GetFromDataReader(dr);
						line.Header = Order_headerDAL.GetFromDataReader(dr);
						line.POHeader = new Porder_header {special_comments = string.Empty + dr["special_comments"]};
						line.Cust_Product.MastProduct = new Mast_products
							{
								special_comments = string.Empty + dr["product_special_comments"],
								factory_ref = string.Empty + dr["factory_ref"]
							};
						result.Add(line);
					}
					dr.Close();
				}
			}
			return result;
		}

		public static List<Order_lines> GetByCriteria(int? month, int? year, string criteria, int company_id)
		{
			var result = new List<Order_lines>();
			using (var conn = new MySqlConnection(Settings.Default.ConnString))
			{
				conn.Open();
				DateTime? date = null;
				if(month != null)
					date = new DateTime(year.Value, month.Value,1);
				var cmd = Utils.GetCommand(@"SELECT users.consolidated_port, order_lines.cprod_id, cust_products.cprod_name, cust_products.cprod_code1,
	(SELECT line.unitprice FROM order_lines line INNER JOIN order_header oh ON line.orderid = oh.orderid 
		WHERE cprod_id = order_lines.cprod_id AND (oh.req_eta < @date OR @date IS NULL) ORDER BY oh.req_eta DESC LIMIT 1)  AS unitprice
													FROM
													  order_lines INNER JOIN cust_products ON cust_products.cprod_id = order_lines.cprod_id
													INNER JOIN order_header ON order_lines.orderid = order_header.orderid
													INNER JOIN mast_products ON mast_products.mast_id = cust_products.cprod_mast
													INNER JOIN users ON mast_products.factory_id = users.user_id
													WHERE (order_header.req_eta < @date OR @date IS NULL) AND (cprod_name LIKE @criteria OR cprod_code1 LIKE @criteria) AND order_header.userid1 = @company_id
													AND order_header.status NOT IN ('X','Y') AND (order_lines.orderqty > 0) AND (cust_products.cprod_returnable = 0) 
													AND order_header.req_eta < now() AND order_header.req_eta > (now() - interval 24 month)
													GROUP BY order_lines.cprod_id,cust_products.cprod_name,cust_products.cprod_code1,users.consolidated_port", conn);
				cmd.Parameters.AddWithValue("@date", date != null ? (object) date : DBNull.Value);
				cmd.Parameters.AddWithValue("@criteria", "%" + criteria + "%");
				cmd.Parameters.AddWithValue("@company_id", company_id);
				var dr = cmd.ExecuteReader();
				while (dr.Read())
				{
					result.Add(new Order_lines
						{
							cprod_id = (int)dr["cprod_id"],
							unitprice = (double)dr["unitprice"],
							Cust_Product = new Cust_products
							{
								cprod_code1 = string.Empty + dr["cprod_code1"],
								cprod_name = string.Empty + dr["cprod_name"],
								consolidated_port = (int)dr["consolidated_port"]
							}
						});
				}
				dr.Close();
			}
			return result;
		}

		public static List<Order_lines> GetForProductAndCriteria(int? month, int? year, int cprod_id, int? company_id = null, bool limitETA = false)
		{
			var result = new List<Order_lines>();
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
			{
				conn.Open();
				DateTime? date = null;
				if (month != null)
					date = new DateTime(year.Value, month.Value, 1);
				var cmd = Utils.GetCommand($@"SELECT order_lines.cprod_id, cust_products.cprod_name, cust_products.cprod_code1,order_header.custpo,order_header.orderid,
												order_lines.unitprice, order_lines.orderqty
													FROM
													  order_lines INNER JOIN cust_products ON cust_products.cprod_id = order_lines.cprod_id
													INNER JOIN order_header ON order_lines.orderid = order_header.orderid
													WHERE (order_header.req_eta < @date OR @date IS NULL) AND cust_products.cprod_id =@cprod_id AND (order_header.userid1 = @company_id OR @company_id IS NULL)
													AND order_header.status NOT IN ('X','Y') AND (order_lines.orderqty > 0) AND (cust_products.cprod_returnable = 0) 
													{ (limitETA ? " AND order_header.req_eta < now()" : "") } AND order_header.req_eta > (now() - interval 24 month)
													", conn);
				cmd.Parameters.AddWithValue("@date", date != null ? (object)date : DBNull.Value);
				cmd.Parameters.AddWithValue("@cprod_id", cprod_id);
				cmd.Parameters.AddWithValue("@company_id", Utilities.ToDBNull(company_id));
				MySqlDataReader dr = cmd.ExecuteReader();
				while (dr.Read())
				{
					result.Add(new Order_lines
					{
						cprod_id = (int)dr["cprod_id"],
						unitprice = (double)dr["unitprice"],
						orderqty = Utilities.FromDbValue<double>(dr["orderqty"]),
                        orderid = (int) dr["orderid"],
						Cust_Product = new Cust_products
						{
							cprod_code1 = string.Empty + dr["cprod_code1"],
							cprod_name = string.Empty + dr["cprod_name"]
						},
						Header = new Order_header { orderid = (int) dr["orderid"], custpo = string.Empty + dr["custpo"]}
					});
				}
				dr.Close();
			}
			return result;
		}

		public static int GetNumOfPreviousShipments(DateTime po_req_etd, int cprod_id)
		{
			int result = 0;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
			{
				conn.Open();
				var cmd =
					Utils.GetCommand(@"SELECT COUNT(*) AS numOfLines FROM order_lines INNER JOIN porder_lines ON porder_lines.soline = order_lines.linenum 
											 INNER JOIN porder_header ON porder_lines.porderid = porder_header.porderid WHERE order_lines.cprod_id = @cprod_id 
											 AND porder_header.po_req_etd < @po_req_etd", conn);
				cmd.Parameters.AddWithValue("@po_req_etd", po_req_etd);
				cmd.Parameters.AddWithValue("@cprod_id", cprod_id);
				result = Convert.ToInt32(cmd.ExecuteScalar());
			}
			return result;
		}


		public static Order_lines GetById(int id)
		{
			Order_lines result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
			{
				conn.Open();
				MySqlCommand cmd = Utils.GetCommand("SELECT * FROM order_lines WHERE linenum = @id", conn);
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

        public static List<Order_lines> GetByProduct(int id)
        {
            var results = new List<Order_lines>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = Utils.GetCommand("SELECT * FROM order_lines WHERE cprod_id = @id", conn);
                cmd.Parameters.AddWithValue("@id", id);
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    results.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return results;
        }
	
		public static Order_lines GetFromDataReader(MySqlDataReader dr)
		{
			Order_lines o = new Order_lines();
		
			o.linenum =  (int) dr["linenum"];
			o.orderid = Utilities.FromDbValue<int>(dr["orderid"]);
			o.original_orderid = Utilities.FromDbValue<int>(dr["original_orderid"]);
			o.linedate = Utilities.FromDbValue<DateTime>(dr["linedate"]);
			o.cprod_id = Utilities.FromDbValue<int>(dr["cprod_id"]);
			o.description = string.Empty + dr["description"];
			o.spec_code = string.Empty + dr["spec_code"];
			o.orderqty = Utilities.FromDbValue<double>(dr["orderqty"]);
			o.orig_orderqty = Utilities.FromDbValue<double>(dr["orig_orderqty"]);
			o.unitprice = Utilities.FromDbValue<double>(dr["unitprice"]);
			o.unitcurrency = Utilities.FromDbValue<int>(dr["unitcurrency"]);
			o.linestatus = Utilities.FromDbValue<int>(dr["linestatus"]);
			o.lineunit = Utilities.FromDbValue<int>(dr["lineunit"]);
			o.factory_group = Utilities.FromDbValue<int>(dr["factory_group"]);
			o.mc_qty = Utilities.FromDbValue<int>(dr["mc_qty"]);
			o.pallet_qty = Utilities.FromDbValue<int>(dr["pallet_qty"]);
			o.unit_qty = Utilities.FromDbValue<int>(dr["unit_qty"]);
			o.lme = Utilities.FromDbValue<double>(dr["lme"]);
			o.allow_change = Utilities.FromDbValue<int>(dr["allow_change"]);
			o.allow_change_down = Utilities.FromDbValue<int>(dr["allow_change_down"]);
			o.fi_line = Utilities.FromDbValue<int>(dr["fi_line"]);
			o.pack_qty = Utilities.FromDbValue<double>(dr["pack_qty"]);
			o.li_line = Utilities.FromDbValue<int>(dr["li_line"]);
            if (Utilities.ColumnExists(dr, "PORowPrice"))
                o.PORowPrice = Utilities.FromDbValue<double>(dr["PORowPrice"]);
            if (Utilities.ColumnExists(dr, "POCurrency"))
                o.POCurrency = Utilities.FromDbValue<int>(dr["POCurrency"]);

            if (Utilities.ColumnExists(dr, "po_unitprice"))
                o.po_unitprice = Utilities.FromDbValue<double>(dr["po_unitprice"]);

            if (Utilities.ColumnExists(dr, "po_unitcurrency"))
                o.po_unitcurrency = Utilities.FromDbValue<int>(dr["po_unitcurrency"]);

            if (Utilities.ColumnExists(dr, "usd_gbp"))
                o.usd_gbp = Utilities.FromDbValue<double>(dr["usd_gbp"]);

            if (Utilities.ColumnExists(dr, "PO_USD"))
                o.PO_USD = Utilities.FromDbValue<double>(dr["PO_USD"]);

            if(Utilities.ColumnExists(dr, "commission_rate"))
                o.commission_rate = Utilities.FromDbValue<double>(dr["commission_rate"]);

            //if (Utilities.ColumnExists(dr, "cprod_mast"))
            //    o.cprod_mast = Utilities.FromDbValue<int>(dr["cprod_mast"]);
            //if (Utilities.ColumnExists(dr, "cprod_code1"))
            //    o.cprod_code1 = string.Empty + dr["cprod_code1"];
            //if (Utilities.ColumnExists(dr, "cprod_name"))
            //    o.cprod_name = string.Empty + dr["cprod_name"];
            return o;

		}
		
		public static void Create(Order_lines o, MySqlTransaction  tr = null)
		{
			string insertsql = @"INSERT INTO order_lines (orderid,original_orderid,linedate,cprod_id,description,spec_code,orderqty,orig_orderqty,unitprice,unitcurrency,linestatus,lineunit,factory_group,mc_qty,pallet_qty,unit_qty,lme,allow_change,allow_change_down,fi_line,pack_qty,li_line) VALUES(@orderid,@original_orderid,@linedate,@cprod_id,@description,@spec_code,@orderqty,@orig_orderqty,@unitprice,@unitcurrency,@linestatus,@lineunit,@factory_group,@mc_qty,@pallet_qty,@unit_qty,@lme,@allow_change,@allow_change_down,@fi_line,@pack_qty,@li_line)";

			var conn = tr == null ? new MySqlConnection(Properties.Settings.Default.ConnString) : tr.Connection;
			if(tr == null)
				conn.Open();
				
			MySqlCommand cmd = Utils.GetCommand(insertsql, conn,tr);
			BuildSqlParameters(cmd,o);

			cmd.ExecuteNonQuery();
			cmd.CommandText = "SELECT linenum FROM order_lines WHERE linenum = LAST_INSERT_ID()";
			o.linenum = (int) cmd.ExecuteScalar();
				
			
		}
		
		private static void BuildSqlParameters(MySqlCommand cmd, Order_lines o, bool forInsert = true)
		{
			
			if(!forInsert)
				cmd.Parameters.AddWithValue("@linenum", o.linenum);
			cmd.Parameters.AddWithValue("@orderid", o.orderid);
			cmd.Parameters.AddWithValue("@original_orderid", o.original_orderid);
			cmd.Parameters.AddWithValue("@linedate", o.linedate);
			cmd.Parameters.AddWithValue("@cprod_id", o.cprod_id);
			cmd.Parameters.AddWithValue("@description", o.description);
			cmd.Parameters.AddWithValue("@spec_code", o.spec_code);
			cmd.Parameters.AddWithValue("@orderqty", o.orderqty);
			cmd.Parameters.AddWithValue("@orig_orderqty", o.orig_orderqty);
			cmd.Parameters.AddWithValue("@unitprice", o.unitprice);
			cmd.Parameters.AddWithValue("@unitcurrency", o.unitcurrency);
			cmd.Parameters.AddWithValue("@linestatus", o.linestatus);
			cmd.Parameters.AddWithValue("@lineunit", o.lineunit);
			cmd.Parameters.AddWithValue("@factory_group", o.factory_group);
			cmd.Parameters.AddWithValue("@mc_qty", o.mc_qty);
			cmd.Parameters.AddWithValue("@pallet_qty", o.pallet_qty);
			cmd.Parameters.AddWithValue("@unit_qty", o.unit_qty);
			cmd.Parameters.AddWithValue("@lme", o.lme);
			cmd.Parameters.AddWithValue("@allow_change", o.allow_change);
			cmd.Parameters.AddWithValue("@allow_change_down", o.allow_change_down);
			cmd.Parameters.AddWithValue("@fi_line", o.fi_line);
			cmd.Parameters.AddWithValue("@pack_qty", o.pack_qty);
			cmd.Parameters.AddWithValue("@li_line", o.li_line);
		}
		
		public static void Update(Order_lines o)
		{
			string updatesql = @"UPDATE order_lines SET orderid = @orderid,original_orderid = @original_orderid,linedate = @linedate,cprod_id = @cprod_id,description = @description,spec_code = @spec_code,orderqty = @orderqty,orig_orderqty = @orig_orderqty,unitprice = @unitprice,unitcurrency = @unitcurrency,linestatus = @linestatus,lineunit = @lineunit,factory_group = @factory_group,mc_qty = @mc_qty,pallet_qty = @pallet_qty,unit_qty = @unit_qty,lme = @lme,allow_change = @allow_change,allow_change_down = @allow_change_down,fi_line = @fi_line,pack_qty = @pack_qty,li_line = @li_line WHERE linenum = @linenum";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
			{
				conn.Open();
				MySqlCommand cmd = Utils.GetCommand(updatesql, conn);
				BuildSqlParameters(cmd,o, false);
				cmd.ExecuteNonQuery();
			}
		}
		
		public static void Delete(int linenum)
		{
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
			{
				conn.Open();
				MySqlCommand cmd = Utils.GetCommand("DELETE FROM order_lines WHERE linenum = @id" , conn);
				cmd.Parameters.AddWithValue("@id", linenum);
				cmd.ExecuteNonQuery();
			}
		}

		public static List<Order_lines> GetByOrderIds(IList<int> order_ids)
		{
			List<Order_lines> result = new List<Order_lines>();
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
			{
				conn.Open();
				MySqlCommand cmd = Utils.GetCommand("",conn);
				cmd.CommandText = string.Format(@"SELECT order_lines.*, cust_products.*,order_header.*,mast_products.*
														FROM order_lines INNER JOIN cust_products ON order_lines.cprod_id = cust_products.cprod_id 
														INNER JOIN mast_products ON cust_products.cprod_mast = mast_products.mast_id
														INNER JOIN order_header ON order_lines.orderid = order_header.orderid
														WHERE order_lines.orderid IN ({0})",
												Utils.CreateParametersFromIdList(cmd, order_ids));
				//cmd.Parameters.AddWithValue("@order_id", order_id);
				MySqlDataReader dr = cmd.ExecuteReader();
				while (dr.Read())
				{
					var line = GetFromDataReader(dr);
					line.Cust_Product = Cust_productsDAL.GetFromDataReader(dr);
					line.Cust_Product.MastProduct = Mast_productsDAL.GetFromDataReader(dr);
					line.Header = Order_headerDAL.GetFromDataReader(dr);
					result.Add(line);
					
				}
				dr.Close();
			}
			return result;
		}

		public static List<Order_lines> GetByExportCriteria(IList<int> cprod_ids=null, List<int> mast_ids=null, int? client_id=null, DateTime? etd_from = null,DateTime? 
            etd_to=null, int? factory_id=null, List<string> custpoList = null, IList<int> factory_ids = null, IList<int> excludedClients = null,
			IList<int> orderids = null, bool useLikeForCustpos = true, int? location_id = null)
		{
			List<Order_lines> result = new List<Order_lines>();
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
			{
			    conn.Open();
				var cmd = Utils.GetCommand("", conn);
			    var factoryCriteria = factory_ids != null ?  string.Format("AND factory_id IN ({0})",Utils.CreateParametersFromIdList(cmd,factory_ids,"factId")) 
                                    : " AND (factory_id=@factory_id OR @factory_id IS NULL)";
				string sql = string.Format(@"SELECT v_lines.*,
							  GROUP_CONCAT(special_comments, ', ') AS special_comments,
                              SUM(po_orderqty*po_unitprice) AS PORowPrice                              
							  FROM v_lines", factoryCriteria);

                var criteria = new List<string>();
                criteria.Add(" orderqty > 0");

				if (cprod_ids != null)
					criteria.Add(string.Format(" cprod_id IN ({0})",
										 Utils.CreateParametersFromIdList(cmd, cprod_ids)));
				else if(mast_ids != null)
				{
					criteria.Add(string.Format(" cprod_mast IN ({0})",
										 Utils.CreateParametersFromIdList(cmd, mast_ids)));
				}
				if (client_id != null)
				{
					if(client_id > 0)
						criteria.Add(" (userid1 = @client_id) ");
					else
					{
                        if (client_id == -1)
                            criteria.Add(" (userid1 IN (SELECT user_id FROM users WHERE distributor > 0))");
                        else
                            criteria.Add($" (userid1 IN (SELECT user_id FROM users WHERE combined_factory = {-1 * client_id} ))");
					}
				}

                if (excludedClients != null)
                    criteria.Add($" userid1 NOT IN ({string.Join(",", excludedClients)}) ");

                if(orderids != null && orderids.Count > 0)
                    criteria.Add($" orderid IN ({string.Join(",", orderids)}) ");

                if (custpoList != null && custpoList.Count > 0)
				{
                    if (useLikeForCustpos)
                    {
                        var parts = new List<string>();
                        for (int i = 0; i < custpoList.Count; i++)
                        {
                            cmd.Parameters.AddWithValue(string.Format("@custpo{0}", i + 1),
                                                                    "%" + custpoList[i] + "%");
                            parts.Add(string.Format(" custpo LIKE @custpo{0} ", i + 1));
                        }
                        criteria.Add(" (" + string.Join(" OR ", parts) + ")");
                    }
                    else
                        criteria.Add($" custpo IN ({string.Join(",", custpoList.Select(c => $"'{c}'"))})");
				}
				criteria.Add("  status NOT IN ('X','Y') ");
				if (etd_from != null)
                    criteria.Add(string.Format(" po_req_etd IS NOT NULL AND po_req_etd >= @etd_from",factoryCriteria));
				if (etd_to != null)
                    criteria.Add(string.Format(" po_req_etd IS NOT NULL AND po_req_etd <= @etd_to", factoryCriteria));
				if (location_id != null)
					criteria.Add($@"(@location IS NULL OR (location_override IS NOT NULL AND location_override = @location)
									OR (location_override IS NULL AND consolidated_port = @location))");

                sql += " WHERE " + string.Join(" AND ", criteria);
                sql += " GROUP BY linenum";

                cmd.CommandText = sql;
				cmd.Parameters.AddWithValue("@etd_from", etd_from != null ? (object) etd_from : DBNull.Value );
				cmd.Parameters.AddWithValue("@etd_to", etd_to != null ? (object) etd_to : DBNull.Value);
				cmd.Parameters.AddWithValue("@factory_id", factory_id);
				cmd.Parameters.AddWithValue("@client_id", client_id);
				cmd.Parameters.AddWithValue("@location", location_id);
				MySqlDataReader dr = cmd.ExecuteReader();
                result = GetLinesFromReader(dr, false);
				dr.Close();
			}
			return result;
		}

		public static List<Order_lines> GetByProdIdsAndETA(List<int> cprod_ids, List<int> mast_ids, int? client_id,
														   DateTime? eta_from, DateTime? eta_to, int? factory_id)
		{
			return GetByProdIdsAndETA(cprod_ids, mast_ids,client_id != null ? new[] {client_id.Value}.ToList() : null, eta_from, eta_to, factory_id);
		}

		public static List<Order_lines> GetByProdIdsAndETA(List<int> cprod_ids, List<int> mast_ids,List<int> client_ids = null, DateTime? eta_from = null, DateTime? eta_to = null, int? factory_id = null)
		{
			List<Order_lines> result = new List<Order_lines>();
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
			{
				conn.Open();
				var cmd = Utils.GetCommand("", conn);
				string sql = @"SELECT order_lines.*, cust_products.*, mast_products.*, order_header.*,porder_header.instructions
							  FROM order_lines INNER JOIN cust_products ON order_lines.cprod_id = cust_products.cprod_id
							  INNER JOIN order_header ON order_lines.orderid = order_header.orderid
							  INNER JOIN mast_products ON mast_products.mast_id = cust_products.cprod_mast
                              INNER JOIN porder_lines ON order_lines.linenum = porder_lines.soline 
                              INNER JOIN porder_header ON porder_header.porderid = porder_lines.porderid";
				if (cprod_ids != null)
					sql += string.Format(" WHERE order_lines.cprod_id IN ({0})",
										 Utils.CreateParametersFromIdList(cmd, cprod_ids));
				else
				{
					sql += string.Format(" WHERE cust_products.cprod_mast IN ({0})",
										 Utils.CreateParametersFromIdList(cmd, mast_ids));
				}
				sql += " AND order_header.stock_order <> 1 ";
				if (client_ids != null && client_ids.Count > 0)
				{

                    if (client_ids[0] > 0) {
                        sql += string.Format(" AND (order_header.userid1 IN ({0})) ", Utils.CreateParametersFromIdList(cmd, client_ids, "clientid"));
                        
					}
                    else {
                        if (client_ids[0] == -1) {
                            sql += " AND (order_header.userid1 IN (SELECT user_id FROM users WHERE distributor > 0))";
                            cmd.Parameters.AddWithValue("@client_id", client_ids[0]);
                        }
                        else
                            sql += $" AND (order_header.userid1 IN (SELECT user_id FROM users WHERE combined_factory = {-1*client_ids[0]} ))";
                    }
                }
				sql += " AND order_header.status NOT IN ('X','Y') ";
				if (eta_from != null)
					sql += " AND order_header.req_eta >= @etd_from";
				if (eta_to != null)
					sql += " AND order_header.req_eta <= @etd_to";
				cmd.CommandText = sql;
				cmd.Parameters.AddWithValue("@etd_from", eta_from);
				cmd.Parameters.AddWithValue("@etd_to", eta_to);
				cmd.Parameters.AddWithValue("@factory_id", factory_id);
				
				MySqlDataReader dr = cmd.ExecuteReader();
				while (dr.Read())
				{
					var line = GetFromDataReader(dr);
					line.Cust_Product = Cust_productsDAL.GetFromDataReader(dr);
					line.Header = Order_headerDAL.GetFromDataReader(dr);
					line.Cust_Product.MastProduct = Mast_productsDAL.GetFromDataReader(dr);
					result.Add(line);

				}
				dr.Close();
			}
			return result;
		}

		public static List<Order_lines> GetStockOrderLinesInFactory(IList<int> cprod_ids, DateTime? from = null)
		{
			var result = new List<Order_lines>();
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
			{
				conn.Open();
			    var date = from ?? DateTime.Today;
				var cmd = Utils.GetCommand("", conn);
				cmd.CommandText = string.Format(@"SELECT order_lines.linenum,porder_header.po_req_etd, order_lines.cprod_id, order_lines.orderqty, order_header.req_eta,order_header.orderdate,order_header.custpo,
									cust_products.cprod_id, mast_products.factory_id
									FROM order_lines
									INNER JOIN porder_lines ON porder_lines.soline = order_lines.linenum
									INNER JOIN porder_header ON porder_header.porderid = porder_lines.porderid
									INNER JOIN order_header ON order_lines.orderid = order_header.orderid
									INNER JOIN cust_products ON order_lines.cprod_id = cust_products.cprod_id
									INNER JOIN mast_products ON mast_products.mast_id = cust_products.cprod_mast
									WHERE order_header.stock_order = 1 AND order_lines.cprod_id IN ({0})
									AND porder_header.po_req_etd > NOW() 
								   OR (order_lines.orderqty - (SELECT SUM(alloc_qty) FROM stock_order_allocation so WHERE so.st_line = order_lines.linenum) > 0)
									OR EXISTS (SELECT co_lines.linenum FROM order_lines co_lines INNER JOIN stock_order_allocation so ON co_lines.linenum = so.so_line INNER JOIN porder_lines co_pline
										ON co_lines.linenum = co_pline.soline INNER JOIN porder_header co_pheader ON co_pline.porderid = co_pheader.porderid WHERE so.st_line = order_lines.linenum AND so.alloc_qty > 0
										 AND co_pheader.po_req_etd > @date)
									", Utils.CreateParametersFromIdList(cmd,cprod_ids));
			    cmd.Parameters.AddWithValue("@date", date);
				var dr = cmd.ExecuteReader();
				while (dr.Read())
				{
					var line = new Order_lines{linenum = (int) dr["linenum"], Header = new Order_header{po_req_etd = Utilities.FromDbValue<DateTime>(dr["po_req_etd"]),req_eta = Utilities.FromDbValue<DateTime>(dr["req_eta"]),orderdate = Utilities.FromDbValue<DateTime>(dr["orderdate"]),custpo = string.Empty + dr["custpo"]},
											cprod_id = Utilities.FromDbValue<int>(dr["cprod_id"]),orderqty = Utilities.FromDbValue<double>(dr["orderqty"]),Cust_Product = new Cust_products{cprod_id = (int) dr["cprod_id"],MastProduct = new Mast_products{factory_id = Utilities.FromDbValue<int>(dr["factory_id"])}}};
					result.Add(line);
				}
				foreach (var line in result)
				{
					line.AllocatedLines = Stock_order_allocationDAL.GetAllocationCalloffLines(line.linenum);
				}
				dr.Close();
			}
			return result;
		}

        public static List<Order_lines> GetNonShippedRegularOrderLines(IList<int> cprod_ids, DateTime? from = null)
		{
			var result = new List<Order_lines>();
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
			{
				conn.Open();
                var date = from ?? DateTime.Today;
				var cmd = Utils.GetCommand("", conn);
				cmd.CommandText = string.Format(@"SELECT order_lines.linenum,porder_header.po_req_etd, order_lines.cprod_id, order_lines.orderqty, order_header.req_eta,order_header.orderdate,order_header.custpo
									FROM order_lines
									INNER JOIN porder_lines ON porder_lines.soline = order_lines.linenum
									INNER JOIN porder_header ON porder_header.porderid = porder_lines.porderid
									INNER JOIN order_header ON order_lines.orderid = order_header.orderid
									WHERE order_header.stock_order IN (0,2) AND order_lines.cprod_id IN ({0})
									AND order_header.req_eta > @date                                    
									", Utils.CreateParametersFromIdList(cmd, cprod_ids));
                cmd.Parameters.AddWithValue("@date", date);
				var dr = cmd.ExecuteReader();
				while (dr.Read())
				{
					var line = new Order_lines
					{
						linenum = (int)dr["linenum"],
						Header = new Order_header { po_req_etd = Utilities.FromDbValue<DateTime>(dr["po_req_etd"]), req_eta = Utilities.FromDbValue<DateTime>(dr["req_eta"]), orderdate = Utilities.FromDbValue<DateTime>(dr["orderdate"]),custpo = string.Empty + dr["custpo"]},
						cprod_id = Utilities.FromDbValue<int>(dr["cprod_id"]),
						orderqty = Utilities.FromDbValue<double>(dr["orderqty"])
					};
					result.Add(line);
				}
				
				dr.Close();
			}
			return result;
		}

        public static List<Order_lines> GetCalloffOrdersInShipment(IList<int> cprod_ids, DateTime? from = null)
		{
			var result = new List<Order_lines>();
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
			{
				conn.Open();
				var cmd = Utils.GetCommand("", conn);
                var date = from ?? DateTime.Today;
				cmd.CommandText = string.Format(@"SELECT order_lines.linenum,porder_header.po_req_etd, order_lines.cprod_id, order_lines.orderqty, order_header.req_eta,order_header.custpo
									FROM order_lines
									INNER JOIN porder_lines ON porder_lines.soline = order_lines.linenum
									INNER JOIN porder_header ON porder_header.porderid = porder_lines.porderid
									INNER JOIN order_header ON order_lines.orderid = order_header.orderid
									WHERE order_header.stock_order <> 1 AND order_lines.cprod_id IN ({0})
									AND porder_header.po_req_etd < @date AND order_header.req_eta > @date ", Utils.CreateParametersFromIdList(cmd, cprod_ids));
                cmd.Parameters.AddWithValue("@date", date);
				var dr = cmd.ExecuteReader();
				while (dr.Read())
				{
					var line = new Order_lines
					{
						linenum = (int)dr["linenum"],
						Header = new Order_header { po_req_etd = Utilities.FromDbValue<DateTime>(dr["po_req_etd"]), req_eta = Utilities.FromDbValue<DateTime>(dr["req_eta"]),custpo = string.Empty + dr["custpo"]},
						cprod_id = Utilities.FromDbValue<int>(dr["cprod_id"]),
						orderqty = Utilities.FromDbValue<double>(dr["orderqty"])
					};
					result.Add(line);
				}
				foreach (var line in result)
				{
					line.AllocatedLines = Order_linesDAL.GetAllocationCalloffLines(line.linenum);
				}
				dr.Close();
			}
			return result;
		}

        

		private static List<Order_lines> GetAllocationCalloffLines(int linenum)
		{
			var result = new List<Order_lines>();
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
			{
				conn.Open();
				var cmd = Utils.GetCommand(@"SELECT order_lines.linenum,order_lines.orderqty,order_header.req_eta,stock_order_allocation.alloc_qty,porder_header.po_req_etd,order_lines.cprod_id
										FROM order_lines INNER JOIN stock_order_allocation ON order_lines.linenum = stock_order_allocation.so_line 
										INNER JOIN order_header ON order_lines.orderid = order_header.orderid
										INNER JOIN porder_lines ON porder_lines.soline = order_lines.linenum
										INNER JOIN porder_header ON porder_header.porderid = porder_lines.porderid
										WHERE st_line = @stockline", conn);
				cmd.Parameters.AddWithValue("@stockline", linenum);
				var dr = cmd.ExecuteReader();
				while (dr.Read())
				{
					var line = new Order_lines
					{
						linenum = (int)dr["linenum"],
						Header = new Order_header { po_req_etd = Utilities.FromDbValue<DateTime>(dr["po_req_etd"]), req_eta = Utilities.FromDbValue<DateTime>(dr["req_eta"]) },
						AllocQty = Utilities.FromDbValue<int>(dr["alloc_qty"]),
						cprod_id = Utilities.FromDbValue<int>(dr["cprod_id"]),
						orderqty = Utilities.FromDbValue<double>(dr["orderqty"])
					};
					result.Add(line);
				}
			}
			return result;
		}

		/*public static List<Order_lines> GetInvoicedOrderLines(string custpo, DateTime? ciDateFrom = null,
															  DateTime? ciDateTo = null, DateTime? etFrom = null,
															  DateTime? etTo = null,IList<int> clientIds = null, bool? useETA = false,
                                                              IList<int> excludedClientIds = null,bool loadPorderLines = false,IList<int> factoryIds = null, 
                                                              bool usePriceOverride = false, int? invoice_sequence = null, bool? UK = null, bool? checkCompanyEtdFrom = false )
		{
			var result = new List<Order_lines>();
			var searchByInvoiceDate = ciDateFrom != null || ciDateTo != null;
			var ignoreDates = !string.IsNullOrEmpty(custpo);
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
			{
				conn.Open();
                var cmd = Utils.GetCommand("",conn);
				cmd.CommandText = $@"SELECT order_lines.*,order_header.*,cust_products.*,mast_products.*,porder_header.po_req_etd, porder_header.userid AS po_factory_id, order_invoice_sequence.sequence,
                                                  porder_lines.unitprice AS po_unitprice,
                                                  porder_lines.unitcurrency AS po_unitcurrency,
                                                  exchange_rates.usd_gbp,
                                                  ROUND(IF(porder_lines.unitcurrency=1,porder_lines.unitprice*exchange_rates.usd_gbp,porder_lines.unitprice),2) AS PO_USD,
                                                  exchange_rates.commission_rate
                                                  FROM {LinesJoin} INNER JOIN invoices ON invoices.orderid = order_header.orderid
													  INNER JOIN cust_products ON order_lines.cprod_id = cust_products.cprod_id
													  INNER JOIN mast_products ON cust_products.cprod_mast = mast_products.mast_id
                                                      INNER JOIN users client ON order_header.userid1 = client.user_id
                                                      LEFT OUTER JOIN order_invoice_sequence ON order_header.orderid = order_invoice_sequence.orderid
                                                        LEFT JOIN exchange_rates ON (((year(COALESCE(invoices.cidate, invoices.invdate)) - 2000) * 100) + month(COALESCE(invoices.cidate, invoices.invdate))) = exchange_rates.month21
													  WHERE (order_header.custpo LIKE @custpo OR invoices.eb_invoice LIKE @custpo OR @custpo IS NULL) AND order_header.status <> 'X' AND order_header.status <> 'Y' AND stock_order <> 1
													  {(!ignoreDates && searchByInvoiceDate ? "AND (cidate >= @cidateFrom OR @cidateFrom IS NULL) AND (cidate <= @cidateTo OR @cidateTo IS NULL)" : "")}
													  {(!ignoreDates && !searchByInvoiceDate ? useETA == true ?
				    usePriceOverride ?
				        "AND (IF(order_header.price_type_override = 'FOB',porder_header.po_req_etd,order_header.req_eta) >= @etdFrom OR @etdFrom IS NULL) AND (IF(order_header.price_type_override = 'FOB',porder_header.po_req_etd,order_header.req_eta) <= @etdTo OR @etdTo IS NULL)"
				        : "AND (order_header.req_eta >= @etdFrom OR @etdFrom IS NULL) AND (order_header.req_eta <= @etdTo OR @etdTo IS NULL) " :
				    "AND (porder_header.po_req_etd >= @etdFrom OR @etdFrom IS NULL) AND (porder_header.po_req_etd <= @etdTo OR @etdTo IS NULL)" : "")}
													  {Utils.CreateWhereClauseFromIdList(cmd, "order_header.userid1", clientIds)}
                                                      {Utils.CreateWhereClauseFromIdList(cmd, "order_header.userid1", excludedClientIds, "exid", negate: true)}
                                                      {Utils.CreateWhereClauseFromIdList(cmd, "mast_products.factory_id ", factoryIds, "factId")}
                                                      AND (order_invoice_sequence.type = @invoice_sequence OR  @invoice_sequence IS NULL {(checkCompanyEtdFrom == true ? " OR porder_header.po_req_etd >= client.bbs_etd_from " : "")})
													  AND invoices.status = 'C'";
                //used this as test AND order_header.orderid = 36961

                cmd.Parameters.AddWithValue("@cidateFrom", Utilities.ToDBNull(ciDateFrom));
				cmd.Parameters.AddWithValue("@cidateTo", Utilities.ToDBNull(ciDateTo));
				cmd.Parameters.AddWithValue("@etdFrom", Utilities.ToDBNull(etFrom));
				cmd.Parameters.AddWithValue("@etdTo", Utilities.ToDBNull(etTo));
				cmd.Parameters.AddWithValue("@custpo", !string.IsNullOrEmpty(custpo) ? (object) ("%" + custpo + "%") : DBNull.Value);
                cmd.Parameters.AddWithValue("@invoice_sequence", Utilities.ToDBNull(invoice_sequence));
				var dr = cmd.ExecuteReader();
				while (dr.Read())
				{
					var line = GetFromDataReader(dr);
					line.Header = Order_headerDAL.GetFromDataReader(dr);
                    
					line.Cust_Product = Cust_productsDAL.GetFromDataReader(dr);
					line.Cust_Product.MastProduct = Mast_productsDAL.GetFromDataReader(dr);
					result.Add(line);
				}
				dr.Close();
                               

                foreach (var g in result.GroupBy(l=>l.orderid))
				{
					var client = CompanyDAL.GetById(g.First().Header.userid1 ?? 0);
					var inv = InvoicesDAL.GetByOrder(g.First().orderid,conn);
					inv.Client = client;
					foreach (var orderLine in g)
					{
						orderLine.Header.Client = client;
						orderLine.Header.Invoice = inv;
                        
					}
                    var header = g.First().Header;
                    header.ManualLines = Order_lines_manualDAL.GetForOrder(header.orderid,conn);
					
				}
				if (loadPorderLines)
				{
					foreach (var line in result )
					{
						line.PorderLines = Porder_linesDAL.GetForOrderLine(line.linenum,conn);
					}
				}
                
			}

            //TODO: names of countries shold be in settings not hard coded
           if (UK.HasValue)
            {
                if (UK ?? false)
                {
                    result = result.Where(m => new[] { "GB", "IE" }.Contains(m.Header.Client.user_country)).ToList();
                }
                else
                {
                    result = result.Where(m => m.Header.Client.user_country != "GB" && m.Header.Client.user_country != "IE").ToList();
                }
            }

            return result;
		}*/

        public static List<Order_lines> GetInvoicedOrderLines2(string custpo, DateTime? ciDateFrom = null,
                                                              DateTime? ciDateTo = null, DateTime? etFrom = null,
                                                              DateTime? etTo = null, IList<int> clientIds = null, OrderDateMode orderDateMode = OrderDateMode.EtaPlusWeek,
                                                              IList<int> excludedClientIds = null, bool loadPorderLines = false, IList<int> factoryIds = null,
                                                              bool usePriceOverride = false, int? invoice_sequence = null, bool? UK = null, bool? checkCompanyEtdFrom = false)
        {
            var result = new List<Order_lines>();
            var searchByInvoiceDate = ciDateFrom != null || ciDateTo != null;
            var ignoreDates = !string.IsNullOrEmpty(custpo);
			DateTime? etFrom2 = null, etTo2 = null;
			var dateCriteria = string.Empty;
			
			if (orderDateMode == OrderDateMode.EtaPlusWeek )
			{
				if (etFrom != null)
					etFrom = etFrom.AddDays(-7);
				if (etTo != null)
					etTo = etTo.AddDays(-7);
			}
			if(usePriceOverride)
			{
				if (etFrom != null)
					etFrom2 = etFrom.AddDays(-7);
				if (etTo != null)
					etTo2 = etTo.AddDays(-7);
			}
			//var etaField = orderDateMode == OrderDateMode.Eta ? "req_eta" : "req_eta1week";
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString)) {
                conn.Open();
                var cmd = Utils.GetCommand("", conn);

				var sqlBase = @"SELECT v_invoicedlines.* FROM v_invoicedlines
						WHERE stock_order <> 1 {0} {1}";

				var otherCriteria = $@"{Utils.CreateWhereClauseFromIdList(cmd, "userid1", clientIds)}
                            {Utils.CreateWhereClauseFromIdList(cmd, "userid1", excludedClientIds, "exid", negate: true)}
                            {Utils.CreateWhereClauseFromIdList(cmd, "factory_id ", factoryIds, "factId")}
                            AND (sequence_type = @invoice_sequence OR  @invoice_sequence IS NULL {(checkCompanyEtdFrom == true ? " OR po_req_etd >= bbs_etd_from " : "")})
							AND (invdate IS NULL OR invoice_status = 'C')";
				if (!string.IsNullOrEmpty(custpo))
					otherCriteria += " AND (custpo LIKE @custpo OR eb_invoice LIKE @custpo OR @custpo IS NULL)";
				if (!ignoreDates)
				{
					if (searchByInvoiceDate)
						dateCriteria = " AND(cidate >= @cidateFrom OR @cidateFrom IS NULL) AND(cidate <= @cidateTo OR @cidateTo IS NULL)";
					if(!usePriceOverride)
					{
						var field = orderDateMode == OrderDateMode.Etd ? "po_req_etd" : "req_eta";
						dateCriteria = $" AND({field} >= @etdFrom OR @etdFrom IS NULL) AND({field} <= @etdTo OR @etdTo IS NULL)";
					}
					if (string.IsNullOrEmpty(dateCriteria))
					{
						dateCriteria = " AND (po_req_etd >= @etdFrom OR @etdFrom IS NULL) AND (po_req_etd <= @etdTo OR @etdTo IS NULL)";
						var dateCriteria2 = "AND (req_eta >= @etdFrom2 OR @etdFrom2 IS NULL) AND (req_eta <= @etdTo2 OR @etdTo2 IS NULL)";
						cmd.CommandText = string.Format(sqlBase, dateCriteria, otherCriteria) + " UNION " +
									string.Format(sqlBase, dateCriteria2, otherCriteria);
					}

				}
				if(string.IsNullOrEmpty(cmd.CommandText))
					cmd.CommandText = string.Format(sqlBase, dateCriteria, otherCriteria);
				                
                cmd.Parameters.AddWithValue("@cidateFrom", Utilities.ToDBNull(ciDateFrom));
                cmd.Parameters.AddWithValue("@cidateTo", Utilities.ToDBNull(ciDateTo));
                cmd.Parameters.AddWithValue("@etdFrom", Utilities.ToDBNull(etFrom));
                cmd.Parameters.AddWithValue("@etdTo", Utilities.ToDBNull(etTo));
                cmd.Parameters.AddWithValue("@custpo", !string.IsNullOrEmpty(custpo) ? (object)("%" + custpo + "%") : DBNull.Value);
                cmd.Parameters.AddWithValue("@invoice_sequence", Utilities.ToDBNull(invoice_sequence));
				if(usePriceOverride)
				{
					cmd.Parameters.AddWithValue("@etdFrom2", Utilities.ToDBNull(etFrom2));
					cmd.Parameters.AddWithValue("@etdTo2", Utilities.ToDBNull(etTo2));
				}

                var dr = cmd.ExecuteReader();
                result = GetLinesFromReader(dr, true);
				
                dr.Close();

				if (usePriceOverride)
				{
					var price_Types = new[] { "CIF", "FOB" };
					result = result.Where(l => (l.Header.price_type_override == "FOB" || price_Types.Contains(l.Header.Client.user_price_type) ? l.Header.po_req_etd >= etFrom : l.Header.req_eta >= etFrom2) &&
											   (l.Header.price_type_override == "FOB" || price_Types.Contains(l.Header.Client.user_price_type) ? l.Header.po_req_etd <= etTo : l.Header.req_eta <= etTo2)).ToList();
				}

				var manualLines = Order_lines_manualDAL.GetForOrders(result.Select(r => r.orderid).Distinct(),conn);
                foreach(var g in manualLines.GroupBy(ml=>ml.orderid)) {
                    foreach (var l in result.Where(r => r.orderid == g.Key))
                        l.Header.ManualLines = g.ToList();
                }
                
                if (loadPorderLines) {

                    foreach (var l in result)
                        l.PorderLines = new List<Porder_lines>() { new Porder_lines { unitcurrency = l.po_unitcurrency, unitprice = l.po_unitprice, orderqty = l.po_orderqty } };
                    
                }

            }

            //TODO: names of countries shold be in settings not hard coded
            if (UK.HasValue) {
                if (UK ?? false) {
                    result = result.Where(m => new[] { "GB", "IE" }.Contains(m.Header.Client.user_country)).ToList();
                }
                else {
                    result = result.Where(m => m.Header.Client.user_country != "GB" && m.Header.Client.user_country != "IE").ToList();
                }
            }

            return result;
        }

        private static List<Order_lines> GetLinesFromReader(IDataReader dr, bool includeInvoice = false)
        {
            var result = new List<Order_lines>();
            var columnNames = Utilities.GetColumnNames(dr);
            while(dr.Read()) {
                result.Add(new Order_lines
                {
                    linenum = (int)dr["linenum"],
                    orderid = Utilities.FromDbValue<int>(dr["orderid"]),
                    linedate = Utilities.FromDbValue<DateTime>(dr["linedate"]),
                    cprod_id = Utilities.FromDbValue<int>(dr["cprod_id"]),
                    description = string.Empty + dr["description"],
                    orderqty = Utilities.FromDbValue<double>(dr["orderqty"]),
                    unitprice = Utilities.FromDbValue<double>(dr["unitprice"]),
                    unitcurrency = Utilities.FromDbValue<int>(dr["unitcurrency"]),
                    mc_qty = Utilities.FromDbValue<int>(dr["mc_qty"]),
                    pallet_qty = Utilities.FromDbValue<int>(dr["pallet_qty"]),
                    unit_qty = Utilities.FromDbValue<int>(dr["unit_qty"]),
                    original_orderid = Utilities.FromDbValue<int>(dr["original_orderid"]),
                    po_unitprice = Utilities.FromDbValue<double>(dr["po_unitprice"]),
                    po_unitcurrency = Utilities.FromDbValue<int>(dr["po_unitcurrency"]),
                    po_orderqty = Utilities.FromDbValue<double>(dr["po_orderqty"]),
                    orig_orderqty = Utilities.FromDbValue<double>(dr["orig_orderqty"]),
                    PO_USD = columnNames.Contains("PO_USD") ? Utilities.FromDbValue<double>(dr["PO_USD"]) : null,
                    commission_rate = columnNames.Contains("commission_rate") ? Utilities.FromDbValue<double>(dr["commission_rate"]) : null,
                    PORowPrice = columnNames.Contains("PORowPrice") ? Utilities.FromDbValue<double>(dr["PORowPrice"]) : null,
                    Header = new Order_header
                    {
                        orderid = Utilities.FromDbValue<int>(dr["orderid"]) ?? 0,
                        status = string.Empty + dr["status"],
                        po_req_etd = Utilities.FromDbValue<DateTime>(dr["po_req_etd"]),
                        original_po_req_etd = Utilities.FromDbValue<DateTime>(dr["original_po_req_etd"]),
                        po_comments = string.Empty + dr["special_comments"],
                        po_instructions = string.Empty + dr["instructions"],
                        userid1 = Utilities.FromDbValue<int>(dr["userid1"]),
                        custpo = string.Empty + dr["custpo"],
                        combined_order = Utilities.FromDbValue<int>(dr["combined_order"]),
                        req_eta = Utilities.FromDbValue<DateTime>(dr["req_eta"]),
                        original_eta = Utilities.FromDbValue<DateTime>(dr["original_eta"]),
                        orderdate = Utilities.FromDbValue<DateTime>(dr["orderdate"]),
                        stock_order = Utilities.FromDbValue<int>(dr["stock_order"]),
                        container_type = Utilities.FromDbValue<int>(dr["container_type"]),
                        booked_in_date = Utilities.FromDbValue<DateTime>(dr["booked_in_date"]),
                        price_type_override = string.Empty + dr["price_type_override"],
                        order_eb_invoice = Utilities.FromDbValue<int>(dr["order_eb_invoice"]),
                        process_id = Utilities.FromDbValue<int>(dr["process_id"]),
                        factory_id = Utilities.FromDbValue<int>(dr["po_factory_id"]),
                        po_process_id = Utilities.FromDbValue<int>(dr["po_process_id"]),
						location_override = Utilities.FromDbValue<int>(dr["location_override"]),
						system_eta = Utilities.ColumnExists(dr, "system_eta") ? Utilities.FromDbValue<DateTime>(dr["system_eta"]) : null,
						Client = new Company
                        {
                            distributor = Utilities.FromDbValue<int>(dr["distributor"]),
                            customer_code = string.Empty + dr["customer_code"],
                            user_country = string.Empty + dr["user_country"],
							user_price_type = string.Empty + dr["user_price_type"]
                        },
                        Invoice = includeInvoice && dr["invdate"] != DBNull.Value ? new Invoices
                        {
                            invdate = Utilities.FromDbValue<DateTime>(dr["invdate"]),
                            eb_invoice = Utilities.FromDbValue<int>(dr["eb_invoice"]),
                            sequence = Utilities.FromDbValue<int>(dr["sequence"]),
                            invoice = (int)(dr["invoice"]),
                            invoice_sequence_type = Utilities.FromDbValue<int>(dr["sequence_type"]),
                            status = string.Empty + dr["invoice_status"],
                            cidate = Utilities.FromDbValue<DateTime>(dr["cidate"]),
                            invoice_from = Utilities.FromDbValue<int>(dr["invoice_from"]),
                            currency = Utilities.FromDbValue<int>(dr["invoice_currency"]),
                            inv_discount = Utilities.FromDbValue<double>(dr["inv_discount"]),
                            inv_amount2 = Utilities.FromDbValue<double>(dr["inv_amount2"]),
                            Client = new Company
                            {
                                distributor = Utilities.FromDbValue<int>(dr["distributor"]),
                                customer_code = string.Empty + dr["customer_code"],
                                user_country = string.Empty + dr["user_country"]
                            }
                        } : null,
                    },
                    Cust_Product = new Cust_products
                    {
                        cprod_id = Utilities.FromDbValue<int>(dr["cprod_id"]) ?? 0,
                        cprod_code1 = string.Empty + dr["cprod_code1"],
                        cprod_name = string.Empty + dr["cprod_name"],
                        cprod_mast = Utilities.FromDbValue<int>(dr["cprod_mast"]),
                        cprod_stock = Utilities.FromDbValue<int>(dr["cprod_stock"]),
                        cprod_stock_date = Utilities.FromDbValue<DateTime>(dr["cprod_stock_date"]),
                        cprod_user = Utilities.FromDbValue<int>(dr["cprod_user"]),
                        cprod_brand_cat = Utilities.FromDbValue<int>(dr["cprod_brand_cat"]),
                        cprod_status = string.Empty + dr["cprod_status"],
                        brand_userid = Utilities.FromDbValue<int>(dr["brand_user_id"]),
                        MastProduct = new Mast_products
                        {
                            factory_id = Utilities.FromDbValue<int>(dr["factory_id"]),
                            factory_ref = string.Empty + dr["factory_ref"],
                            factory_name = string.Empty + dr["factory_name"],
                            asaq_ref = string.Empty + dr["asaq_ref"],
                            asaq_name = string.Empty + dr["asaq_name"],
                            prod_length = Utilities.FromDbValue<double>(dr["prod_length"]),
                            prod_width = Utilities.FromDbValue<double>(dr["prod_width"]),
                            prod_height = Utilities.FromDbValue<double>(dr["prod_height"]),
                            om_seq_number = Utilities.FromDbValue<int>(dr["om_seq_number"]),
                            range1 = Utilities.FromDbValue<int>(dr["range1"]),
                            stock_qty = Utilities.FromDbValue<int>(dr["stock_qty"]),
                            threemonths = Utilities.FromDbValue<double>(dr["threemonths"]),
                            category1 = Utilities.FromDbValue<int>(dr["category1"]),
                            special_comments = string.Empty + dr["product_special_comments"],
                            lme = Utilities.FromDbValue<int>(dr["lme"]),
                            price_dollar = Utilities.FromDbValue<double>(dr["price_dollar"]),
                            price_pound = Utilities.FromDbValue<double>(dr["price_pound"]),
                            product_group = columnNames.Contains("product_group") ? string.Empty + dr["product_group"] : null,
                            Factory = new Company
                            {
                                user_id = Utilities.FromDbValue<int>(dr["factory_id"]) ?? 0,
                                user_name = string.Empty + dr["user_name"],
                                consolidated_port = Utilities.FromDbValue<int>(dr["consolidated_port"]),
                                combined_factory = Utilities.FromDbValue<int>(dr["combined_factory"]),
                                factory_code = string.Empty + dr["factory_code"]
                            }
                        }
                    }
                });
            }

            return result;

        }

        public static List<Order_lines> GetLiveOrdersAnalysis(DateTime from, CountryFilter countryFilter = CountryFilter.UKOnly,int? client_id = null, 
			IList<int> includedNonDistributorsIds = null, bool brands = true)
	    {
	        var lines = new List<Order_lines>();
	        
	        using (var conn = new MySqlConnection(Settings.Default.ConnString))
	        {
	            conn.Open();
	            var cmd = new MySqlCommand("", conn);
				string distributorCriteria = client_id == null ? $@" (distributor {(brands ? ">" : "<=")} 0 
                                    {(includedNonDistributorsIds != null && includedNonDistributorsIds.Count > 0 ? $" OR userid1 IN ({Utils.CreateParametersFromIdList(cmd, includedNonDistributorsIds, "dist_")})" : "")})"
									: "";

                var testAccountIds = conn.Query<int>("SELECT user_id FROM users WHERE test_account = 1").ToList();

                var criteria = new List<string>()
                    {
                        AnalyticsDAL.GetCountryCondition(countryFilter, useAnd: false),                        
                        "po_req_etd > @from",
                        "(userid1 = @client_id OR @client_id IS NULL)",
                        "orderqty > 0 ",
                        $"userid1 NOT IN ({Utils.CreateParametersFromIdList(cmd,testAccountIds)})",
                        "category1 <> 13 ",
                        AnalyticsDAL.ExcludedCprodBrandCatsCondition.Replace("AND",string.Empty)
                    };
				if (!string.IsNullOrEmpty(distributorCriteria))
					criteria.Add(distributorCriteria);

                var sql = $@"SELECT v_lines.* FROM v_lines 
                                    WHERE {string.Join(" AND ", criteria)}  ";
	            cmd.CommandText = sql;
                cmd.Parameters.AddWithValue("@from", from);
	            cmd.Parameters.AddWithValue("@client_id", client_id);
	            var dr = cmd.ExecuteReader();
                lines = GetLinesFromReader(dr, false);
                dr.Close();

                
	        }
	        return lines;
	    }

        public static List<int> GetByOriginalOrderids(string ids)
        {
            var result = new List<int>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand(string.Format("SELECT DISTINCT orderid FROM order_lines {0}",ids), conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(Convert.ToInt32(dr["orderid"]));
                }
                dr.Close();
            }



            return result;
        }
	}
}
			
			