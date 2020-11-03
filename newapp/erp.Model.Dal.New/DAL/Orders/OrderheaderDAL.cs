
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
    public class OrderHeaderDAL : IOrderHeaderDAL
    {
	    private MySqlConnection conn;
	    private IOrderLinesDAL orderLinesDal;

	    public OrderHeaderDAL(IDbConnection conn, IOrderLinesDAL orderLinesDal)
	    {
		    this.conn = (MySqlConnection) conn;
		    this.orderLinesDal = orderLinesDal;
	    }


		public List<Order_header> GetAll()
		{
			return conn.Query<Order_header>("SELECT * FROM order_header").ToList();
		}

        public List<Order_header> GetCreatedInPeriod(DateTime? from, DateTime? to, bool gbie_only = true)
        {
	        return conn.Query<Order_header, Company, Order_header>(
		        $@"SELECT order_header.*,(SELECT MAX(po_req_etd)
                FROM porder_header WHERE soorderid = order_header.orderid) AS po_req_etd, users.* FROM order_header INNER JOIN users ON order_header.userid1 = users.user_id
                WHERE
                (orderdate >= @from OR @from IS NULL) AND (orderdate <= @to OR @to IS NULL) AND COALESCE(order_header.container_type,0) NOT IN(3,4,5)
                AND EXISTS (SELECT orderid FROM order_line_detail2_v7 WHERE orderid = order_header.orderid AND category1 <> 13)
                AND users.distributor > 0 AND  order_header.`status` NOT IN ('X','Y') {(gbie_only ? "AND users.user_country IN ('GB','IE')" : "")}",
		        (oh, client) =>
		        {
			        oh.Client = client;
			        return oh;
		        }, new {from, to}, splitOn: "user_id").ToList();
			
        }

        public List<Order_header> GetProducedInPeriod(DateTime? from = null, DateTime? to = null,DateTime? created_until = null, bool gbie_only = true)
        {
	        return conn.Query<Order_header, Company, Order_header>(
		        $@"SELECT order_header.*,(SELECT MAX(po_req_etd) FROM porder_header WHERE soorderid = order_header.orderid) AS po_req_etd, users.*
               FROM order_header INNER JOIN users ON order_header.userid1 = users.user_id
               WHERE COALESCE(order_header.container_type,0) NOT IN(3,4,5)
                AND EXISTS (SELECT orderid FROM order_line_detail2_v7 WHERE orderid = order_header.orderid AND category1 <> 13)
                AND users.distributor > 0 AND  order_header.`status` NOT IN ('X','Y') {(gbie_only ? "AND users.user_country IN ('GB','IE')" : "")} AND
                ((SELECT MAX(po_req_etd) FROM porder_header WHERE soorderid = order_header.orderid) >= @from OR  @from IS NULL) AND
                ((SELECT MAX(po_req_etd) FROM porder_header WHERE soorderid = order_header.orderid) <= @to OR @to IS NULL)  AND
                (order_header.orderdate < @created_until OR @created_until IS NULL)",
		        (oh, client) =>
		        {
			        oh.Client = client;
			        return oh;
		        }, new {from, to, created_until}, splitOn: "user_id").ToList();

        }

        public List<OrderMonthlyData> GetQtyByMonth(string cprod_code1, int company_id, DateTime fromMonth, DateTime toMonth)
        {
	        var data = conn.Query(
		        @"SELECT custpo,  YEAR(req_eta) AS year, MONTH(req_eta) AS month, COALESCE(SUM(orderqty),0) AS qty FROM
                order_header INNER JOIN order_lines ON order_lines.orderid = order_header.orderid
                INNER JOIN cust_products ON order_lines.cprod_id = cust_products.cprod_id
                WHERE order_header.userid1 = @company_id AND cust_products.cprod_code1 = @cprod_code1
                AND req_eta BETWEEN @fromMonth AND @toMonth
                GROUP BY YEAR(req_eta), MONTH(req_eta), order_header.custpo ORDER BY YEAR(req_eta), MONTH(req_eta)",
		        new {cprod_code1, company_id, fromMonth, toMonth}).ToList();

            return data.GroupBy(r => new {r.year, r.month})
                           .Select(
                               g =>
                               new OrderMonthlyData
                                   {
                                       MonthYear = new DateTime(g.Key.year, g.Key.month, 1),
                                       Orders =
                                           g.Select(r => new OrderAggregateData {custpo = r.custpo, Qty = r.qty}).ToList()
                                   })
                           .ToList();
        }

        public List<OrderWeeklyData> GetCountByWeek_Users(DateTime weekStart, int weekspan)
        {
	        return conn.Query<OrderWeeklyData>(
		        @"SELECT Count(order_header.orderid) AS OrderCount,Min(porder.po_req_etd) AS req_etd,
				order_header.userid1 AS company_id,users.user_name AS company_name,users.customer_code,
                users.distributor, users.hide_1,WEEK(porder.po_req_etd,7) AS week
                FROM order_header INNER JOIN users ON users.user_id = order_header.userid1
                INNER JOIN (SELECT soorderid AS orderid, MAX(po_req_etd) AS po_req_etd FROM  porder_header
                GROUP BY porder_header.soorderid) AS porder ON order_header.orderid = porder.orderid
                WHERE porder.po_req_etd >= @weekStart AND porder.po_req_etd < @weekEnd AND order_header.container_type IN (0,1,2,6) AND order_header.status NOT IN ('X','Y') 
				AND COALESCE (`users`.`test_account`, 0) = 0
                GROUP BY WEEK(porder.po_req_etd,7),order_header.userid1,users.user_name, users.customer_code,users.distributor, users.hide_1
                ORDER BY WEEK(porder.po_req_etd,7)",
		        new {weekStart, weekEnd = weekStart.AddDays(7 * weekspan - 1)}).ToList();
            
        }

        public List<OrderWeeklyData> GetCountByWeek_Factories(DateTime weekStart, int weekspan)
        {
	        return conn.Query<OrderWeeklyData>(
		        @"SELECT Count(order_header.orderid) AS OrderCount,Min(porder.po_req_etd) AS req_etd,
				porder.userid AS factory_id, porder.userid AS company_id,
				factory.user_name AS company_name,factory.factory_code AS customer_code,
                factory.consolidated_port, WEEK(porder.po_req_etd,7) AS week
                FROM order_header
                INNER JOIN (SELECT userid, soorderid AS orderid, MAX(po_req_etd) AS po_req_etd FROM  porder_header GROUP BY porder_header.userid,porder_header.soorderid) AS porder ON order_header.orderid = porder.orderid
                INNER JOIN users AS factory ON porder.userid = factory.user_id
				INNER JOIN users client ON order_header.userid1 = client.user_id
                WHERE porder.po_req_etd >= @weekStart AND porder.po_req_etd < @weekEnd 
				AND order_header.container_type IN (0,1,2,6) AND order_header.status NOT IN ('X','Y')
				AND COALESCE (client.`test_account`, 0) = 0
                GROUP BY porder.userid,WEEK(porder.po_req_etd,7),factory.user_name, factory.consolidated_port
                ORDER BY WEEK(porder.po_req_etd,7),factory.consolidated_port, factory.factory_code",
		        new {weekStart, weekEnd = weekStart.AddDays(7 * weekspan - 1)}).ToList();
			
        }

        public int? GetTotalQty(string cprod_code1, int company_id)
        {
	        return conn.ExecuteScalar<int?>(
		        @"SELECT SUM(orderqty) FROM
                order_header INNER JOIN order_lines ON order_lines.orderid = order_header.orderid
                INNER JOIN cust_products ON order_lines.cprod_id = cust_products.cprod_id
                WHERE order_header.userid1 = @company_id AND cust_products.cprod_code1 = @cprod_code1",
		        new {company_id, cprod_code1});
            
        }


        public Order_header GetById(int id,bool includeLines = false)
        {
	        var order = conn.QueryFirstOrDefault<Order_header>(
		        "SELECT * FROM order_header WHERE orderid = @id", new {id});

			if (includeLines && order != null)
				order.Lines = orderLinesDal.GetByOrderId(id);
            
			return order;
		}

        public DateTime? GetMaxEtd(List<int> ids )
        {
	        return conn.ExecuteScalar<DateTime?>(
		        @"SELECT MAX(po_req_etd) AS MaxETD FROM porder_header INNER JOIN order_header ON porder_header.soorderid = order_header.orderid
                  WHERE order_header.orderid IN @ids", new {ids});
        }

		
		public void Create(Order_header o)
        {
            string insertsql = @"INSERT INTO order_header (orderid,orderdate,userid1,locid,stock_order,status,new_status,delivery_address1,delivery_address2,delivery_address3,delivery_address4,delivery_address5,currency,surcharge,notes,custpo,req_etd,original_eta,req_eta,actual_eta,loading_details,reference_no,factory_pl,lme,packing_list,edit_sort,mod_flag,eta_flag,process_id,combined_order,loading_factory,upload,upload_flag,entered_by,payment,documents,documents2,loading_date,container_type,loading_perc,forwarder_name,despatch_note) VALUES(@orderid,@orderdate,@userid1,@locid,@stock_order,@status,@new_status,@delivery_address1,@delivery_address2,@delivery_address3,@delivery_address4,@delivery_address5,@currency,@surcharge,@notes,@custpo,@req_etd,@original_eta,@req_eta,@actual_eta,@loading_details,@reference_no,@factory_pl,@lme,@packing_list,@edit_sort,@mod_flag,@eta_flag,@process_id,@combined_order,@loading_factory,@upload,@upload_flag,@entered_by,@payment,@documents,@documents2,@loading_date,@container_type,@loading_perc,@forwarder_name,@despatch_note)";
			
		    var tr = conn.BeginTransaction();
		    try
		    {

		        o.orderid = conn.ExecuteScalar<int>("SELECT nextorderid FROM nextorderid",  transaction:tr);
		        
		        conn.Execute("UPDATE nextorderid SET nextorderid = nextorderid+1");

			    conn.Execute(insertsql, o);

		        if (o.Lines != null)
		        {
		            foreach (var line in o.Lines)
		            {
		                line.orderid = o.orderid;
		                orderLinesDal.Create(line, tr);
		            }
		        }
		        tr.Commit();

		    }
		    catch
		    {
		        tr.Rollback();
		    }
		    finally
		    {
		        tr = null;
                conn.Close();
		    }
        }

		
		public void Update(Order_header o)
		{
			string updatesql = @"UPDATE order_header SET orderdate = @orderdate,userid1 = @userid1,locid = @locid,stock_order = @stock_order,status = @status,new_status = @new_status,delivery_address1 = @delivery_address1,delivery_address2 = @delivery_address2,delivery_address3 = @delivery_address3,delivery_address4 = @delivery_address4,delivery_address5 = @delivery_address5,currency = @currency,surcharge = @surcharge,notes = @notes,custpo = @custpo,req_etd = @req_etd,original_eta = @original_eta,req_eta = @req_eta,actual_eta = @actual_eta,loading_details = @loading_details,reference_no = @reference_no,factory_pl = @factory_pl,lme = @lme,packing_list = @packing_list,edit_sort = @edit_sort,mod_flag = @mod_flag,eta_flag = @eta_flag,process_id = @process_id,combined_order = @combined_order,loading_factory = @loading_factory,upload = @upload,upload_flag = @upload_flag,entered_by = @entered_by,payment = @payment,documents = @documents,documents2 = @documents2,loading_date = @loading_date,container_type = @container_type,loading_perc = @loading_perc,forwarder_name = @forwarder_name,despatch_note = @despatch_note WHERE orderid = @orderid";

			conn.Execute(updatesql, o);
		}

		public void Delete(int orderid)
		{
			conn.Execute("DELETE FROM order_header WHERE orderid = @orderid", new {orderid});
		}

        public Order_header GetByCustpo(string custpo)
        {
            return conn.QueryFirstOrDefault<Order_header>("SELECT * FROM order_header WHERE custpo = @custpo", new {custpo});
        }

        public List<Company> GetClientsOnOrders(IList<int> factory_ids=null, bool combined = false)
        {
            var result = new List<Company>();
            
            var sql =
                $@"SELECT users.* FROM users WHERE user_id IN (SELECT DISTINCT userid1 FROM order_header
                                            {(factory_ids != null ? @" INNER JOIN order_lines ON order_header.orderid = order_lines.orderid
                                            INNER JOIN cust_products ON order_lines.cprod_id = cust_products.cprod_id
                                            INNER JOIN mast_products ON cust_products.cprod_mast = mast_products.mast_id
                                            WHERE mast_products.factory_id IN @factory_ids" : "")})";

            //cmd.Parameters.AddWithValue("@factory_id", factory_id != null ? (object) factory_id : DBNull.Value);
            result = conn.Query<Company>(sql, new { factory_ids }).ToList();
            if (combined)
                result.AddRange(result.Where(c=>c.combined_factory > 0).GroupBy(c => c.combined_factory).
                    Where(g=>g.Count() > 1).Select(g => new Company { user_id = -1 * g.Key.Value, customer_code = string.Join("/", g.Select(c => c.customer_code)) }));
            
            return result;
        }

        
        public List<Cust_products> GetProductsOnOrders(int? location_id = null,int? factory_id=null, int? client_id=null, 
			string criteria = "", bool spares=false, bool discontinued=false,int? brand_user_id = null, 
			IList<int> factory_ids = null, bool outofstockonly = false, string analysis_d = null, 
			string excluded_distributors = null, int? category1Id = null, bool spares_only = false, string excluded_custproducts_cprodusers = null)
        {
                
            var factoryCriteria = factory_ids != null ? " mast_products.factory_id IN @factory_ids"
                                : "  (mast_products.factory_id = @factory_id OR @factory_id IS NULL)";
            //client_id can be -1 (all clients that are brand distributors)
            string sql;
            var discont_criteria = (!discontinued ? " AND cust_products.cprod_status <> 'D' " : "");
            var analysis_d_criteria = (!string.IsNullOrEmpty(analysis_d) && !discontinued ? 
				$" AND cust_products.analysis_d IN @analysis_d_list " : "");
            var distrib_criteria = !string.IsNullOrEmpty(excluded_distributors) ? 
				$" AND order_header.userid1 NOT IN @excluded_distributors_list" : "";
            var custproducts_cprodusers_criteria = !string.IsNullOrEmpty(excluded_custproducts_cprodusers) ?
                $" AND cust_products.cprod_user NOT IN @excluded_custproducts_cprodusers_list" : "";

            var spares_only_criteria = spares_only ? $" AND mast_products.category1 = 13" : "AND (@spares = 1 OR mast_products.category1 <> 13) AND (mast_products.category1 = @category1 OR @category1 IS NULL)";

            if (client_id != null)
            {
                sql = $@"SELECT cust_products.*,mast_products.* FROM cust_products
                                            INNER JOIN mast_products ON cust_products.cprod_mast = mast_products.mast_id
                                            INNER JOIN users factory ON mast_products.factory_id = factory.user_id
											INNER JOIN order_lines ON cust_products.cprod_id = order_lines.cprod_id 
											INNER JOIN order_header ON order_lines.orderid = order_header.orderid
                                            WHERE {factoryCriteria}
                                                AND (@location_id IS NULL OR (order_header.location_override IS NOT NULL AND order_header.location_override = @location_id)
													OR (order_header.location_override IS NULL AND factory.consolidated_port = @location_id))
                                            AND (@criteria IS NULL OR cprod_code1 LIKE @criteria OR cprod_name LIKE @criteria OR factory_ref LIKE @criteria OR asaq_name LIKE @criteria)
                                            {spares_only_criteria}
                                            {discont_criteria} {analysis_d_criteria} {custproducts_cprodusers_criteria}
											AND  order_header.status NOT IN ('X','Y')  
											AND (cust_products.brand_user_id = @brand_user_id OR @brand_user_id IS NULL)
											AND (order_header.userid1 = @client_id 
                                                        OR (@client_id = -1 AND EXISTS(SELECT user_id FROM users WHERE order_header.userid1 = users.user_id AND distributor > 0 {distrib_criteria} ))
                                                        OR (@client_id < -1 AND EXISTS(SELECT user_id FROM users WHERE order_header.userid1 = users.user_id AND combined_factory = -1*@client_id ))
												)
										GROUP BY cust_products.cprod_id";
            }
            else
            {
                sql = $@"SELECT GROUP_CONCAT(cprod_code1 SEPARATOR ', ')  AS cprod_code1,
                        SUM(cprod_stock)  AS cprod_stock,
						mast_products.*
                            FROM mast_products INNER JOIN users factory ON mast_products.factory_id = factory.user_id
                        INNER JOIN cust_products ON mast_products.mast_id = cust_products.cprod_mast
                        INNER JOIN order_lines ON cust_products.cprod_id = order_lines.cprod_id 
                        INNER JOIN order_header ON order_lines.orderid = order_header.orderid
                        WHERE {factoryCriteria} 
							AND (@location_id IS NULL OR (order_header.location_override IS NOT NULL AND order_header.location_override = @location_id)
								OR (order_header.location_override IS NULL AND factory.consolidated_port = @location_id))
                        AND (@criteria IS NULL OR factory_ref LIKE @criteria OR asaq_name LIKE @criteria OR
                            cust_products.cprod_code1 LIKE @criteria
                        )
                        {spares_only_criteria}
						{discont_criteria} {analysis_d_criteria} {custproducts_cprodusers_criteria}
                        AND  order_header.status NOT IN ('X','Y')  
                        AND (cust_products.brand_user_id = @brand_user_id OR @brand_user_id IS NULL)
                        GROUP BY mast_products.mast_id";
            }

			return conn.Query<Cust_products, Mast_products, Cust_products>(sql, 
				(cp, mp) =>
				{
					cp.MastProduct = mp;
					List<int> stock_codes = null;
					if (client_id != null)
					{
						var stockCode = cp.cprod_stock_code;
						if(stockCode != null)
							stock_codes = new int[] { stockCode.Value }.ToList();
						cp.cprod_stock_codes = stock_codes;
					}
					return cp;
				}, new 
				{ 
					location_id, 
					factory_id, 
					client_id, 
					brand_user_id, 
					criteria =  !string.IsNullOrEmpty(criteria) ? (object) ("%" + criteria + "%") : null,
					spares = spares ? 1 : 0,
					category1 = category1Id,
					factory_ids,
					analysis_d_list = analysis_d?.Split(','),
                    excluded_custproducts_cprodusers_list = excluded_custproducts_cprodusers?.Split(','),
                    excluded_distributors_list = excluded_distributors?.Split(',')
                }, splitOn: "mast_id").ToList();
			            
        }

        public int GetNumberOfOrders(int cprod_id, DateTime? from = null, DateTime? to = null)
        {
			return conn.ExecuteScalar<int>(
				@"SELECT COUNT(*) FROM order_header WHERE (orderdate >= @from OR @from IS NULL) AND (orderdate <= @to OR @to IS NULL) AND
                 EXISTS (SELECT cprod_id FROM order_lines WHERE orderid = order_header.orderid AND cprod_id = @cprod_id AND orderqty > 0) 
				  AND status NOT IN ('X','Y') AND stock_order = 1",
				new {from, to, cprod_id});            
        }
               
        

        public List<Order_header> GetByClient(int user_id, string custpo = null)
        {
			return conn.Query<Order_header>(
				$"SELECT * FROM order_header WHERE order_header.userid1 = @user_id {(!string.IsNullOrEmpty(custpo) ? " AND custpo LIKE @custpo" : "")}",
				new {user_id, custpo = custpo + "%"}).ToList();            
        }

        public List<Order_header> GetCombinedOrders(int orderid)
        {
			return conn.Query<Order_header>(@"SELECT * FROM order_header WHERE combined_order = @orderid", new {orderid }).ToList();            
        }                        
	}
}

