
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;
using asaq2.Model.Properties;

namespace asaq2.Model
{
    public class Order_headerDAL
	{
	
		public static List<Order_header> GetAll()
		{
			List<Order_header> result = new List<Order_header>();
            using (var conn = new MySqlConnection(Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM order_header", conn);
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}

        public static List<Order_header> GetCreatedInPeriod(DateTime? from, DateTime? to, bool gbie_only = true)
        {
            List<Order_header> result = new List<Order_header>();
            using (var conn = new MySqlConnection(Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand(string.Format(@"SELECT order_header.*,(SELECT MAX(po_req_etd) 
                            FROM porder_header WHERE soorderid = order_header.orderid) AS po_req_etd, users.* FROM order_header INNER JOIN users ON order_header.userid1 = users.user_id
                            WHERE 
                            (orderdate >= @from OR @from IS NULL) AND (orderdate <= @to OR @to IS NULL) AND COALESCE(order_header.container_type,0) NOT IN(3,4,5) 
                            AND EXISTS (SELECT orderid FROM order_line_detail2_v7 WHERE orderid = order_header.orderid AND category1 <> 13)
                            AND users.distributor > 0 AND  order_header.`status` NOT IN ('X','Y') {0}", gbie_only ? "AND users.user_country IN ('GB','IE')" : ""), conn);
                cmd.Parameters.AddWithValue("@from", from != null ? (object)from : DBNull.Value);
                cmd.Parameters.AddWithValue("@to", to != null ? (object)to : DBNull.Value);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    var order = GetFromDataReader(dr);
                    order.Client = CompanyDAL.GetFromDataReader(dr);
                    result.Add(order);
                }
                dr.Close();
            }
            return result;
        }

        public static List<Order_header> GetProducedInPeriod(DateTime? from = null, DateTime? to = null,DateTime? created_until = null, bool gbie_only = true)
        {
            List<Order_header> result = new List<Order_header>();
            using (var conn = new MySqlConnection(Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand(string.Format(@"SELECT order_header.*,(SELECT MAX(po_req_etd) FROM porder_header WHERE soorderid = order_header.orderid) AS po_req_etd, users.*
                                           FROM order_header INNER JOIN users ON order_header.userid1 = users.user_id
                                           WHERE COALESCE(order_header.container_type,0) NOT IN(3,4,5) 
                                            AND EXISTS (SELECT orderid FROM order_line_detail2_v7 WHERE orderid = order_header.orderid AND category1 <> 13)
                                            AND users.distributor > 0 AND  order_header.`status` NOT IN ('X','Y') {0} AND
                                            ((SELECT MAX(po_req_etd) FROM porder_header WHERE soorderid = order_header.orderid) >= @from OR  @from IS NULL) AND 
                                            ((SELECT MAX(po_req_etd) FROM porder_header WHERE soorderid = order_header.orderid) <= @to OR @to IS NULL)  AND
                                            (order_header.orderdate < @created_until OR @created_until IS NULL)", gbie_only ? "AND users.user_country IN ('GB','IE')" : ""), conn);
                cmd.Parameters.AddWithValue("@from", from != null ? (object) from : DBNull.Value);
                cmd.Parameters.AddWithValue("@to", to != null ? (object) to : DBNull.Value);
                cmd.Parameters.AddWithValue("@created_until",
                                            created_until != null ? (object) created_until : DBNull.Value);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    var order = GetFromDataReader(dr);
                    order.Client = CompanyDAL.GetFromDataReader(dr);
                    result.Add(order);
                }
                dr.Close();
            }
            return result;
        }

        public static List<OrderMonthlyData> GetQtyByMonth(string cprod_code1, int company_id, DateTime fromMonth, DateTime toMonth)
        {
            var result = new List<OrderMonthlyData>();
            using (var conn = new MySqlConnection(Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(@"SELECT custpo,  YEAR(req_eta) AS year, MONTH(req_eta) AS month, COALESCE(SUM(orderqty),0) AS qty FROM
                                    order_header INNER JOIN order_lines ON order_lines.orderid = order_header.orderid
                                    INNER JOIN cust_products ON order_lines.cprod_id = cust_products.cprod_id
                                    WHERE order_header.userid1 = @company_id AND cust_products.cprod_code1 = @code
                                    AND req_eta BETWEEN @fromMonth AND @toMonth
                                    GROUP BY YEAR(req_eta), MONTH(req_eta), order_header.custpo ORDER BY YEAR(req_eta), MONTH(req_eta)", conn);
                cmd.Parameters.AddWithValue("@company_id", company_id);
                cmd.Parameters.AddWithValue("@code", cprod_code1);
                cmd.Parameters.AddWithValue("@fromMonth", fromMonth);
                cmd.Parameters.AddWithValue("@toMonth", toMonth);
                MySqlDataReader dr = cmd.ExecuteReader();
                int month = 0, year = 0;
                double qty = 0;
                string custpo=String.Empty;
                var record = new {month, year, custpo, qty};
                var results = new[] {record}.ToList();  //trick to get a list of anonymous objects
                while (dr.Read())
                {
                    results.Add(
                        new
                            {
                                month = (int) dr["month"],
                                year = (int) dr["year"],
                                custpo = String.Empty + dr["custpo"],
                                qty = (double) dr["qty"]
                            });
                }
                dr.Close();
                results.RemoveAt(0); //remove dummy record
                result =
                    results.GroupBy(r => new {r.year, r.month})
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
            return result;
        }

        public static List<OrderWeeklyData> GetCountByWeek_Users(DateTime weekStart, int weekspan)
        {
            var result = new List<OrderWeeklyData>();
            using (var conn = new MySqlConnection(Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand(@"SELECT Count(order_header.orderid) AS count,Min(porder.po_req_etd) AS req_etd,order_header.userid1,users.user_name,users.customer_code, 
                            users.distributor, users.hide_1,WEEK(porder.po_req_etd,7) AS week 
                            FROM order_header INNER JOIN users ON users.user_id = order_header.userid1
                            INNER JOIN (SELECT soorderid AS orderid, MAX(po_req_etd) AS po_req_etd FROM  porder_header 
                            GROUP BY porder_header.soorderid) AS porder ON order_header.orderid = porder.orderid
                            WHERE porder.po_req_etd >= @weekStart AND porder.po_req_etd < @weekEnd AND order_header.container_type IN (0,1,2,6) AND order_header.status NOT IN ('X','Y')
                            GROUP BY WEEK(porder.po_req_etd,7),order_header.userid1,users.user_name, users.customer_code,users.distributor, users.hide_1
                            ORDER BY WEEK(porder.po_req_etd,7)", conn);
                cmd.Parameters.AddWithValue("@weekStart", weekStart);
                cmd.Parameters.AddWithValue("@weekEnd", weekStart.AddDays(7*weekspan - 1));
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    var data = new OrderWeeklyData();
                    data.company_id = (int) dr["userid1"];
                    data.company_name = String.Empty + dr["user_name"];
                    data.customer_code = String.Empty + dr["customer_code"];
                    data.isBrandDistributor = ((int) dr["distributor"]) > 0 && ((int) dr["hide_1"]) == 0;
                    data.WeekStart = Utilities.GetFirstDayInWeek((DateTime) dr["req_etd"]);
                    data.OrderCount = Convert.ToInt32(dr["count"]);
                    result.Add(data);
                }

            }
            return result;
        }

        public static List<OrderWeeklyData> GetCountByWeek_Factories(DateTime weekStart, int weekspan)
        {
             var result = new List<OrderWeeklyData>();
            using (var conn = new MySqlConnection(Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand(@"SELECT Count(order_header.orderid) AS count,Min(porder.po_req_etd) AS req_etd,porder.userid,factory.user_name,factory.factory_code,
                                            factory.consolidated_port, WEEK(porder.po_req_etd,7) AS week
                                            FROM order_header 
                                            INNER JOIN (SELECT userid, soorderid AS orderid, MAX(po_req_etd) AS po_req_etd FROM  porder_header GROUP BY porder_header.userid,porder_header.soorderid) AS porder ON order_header.orderid = porder.orderid
                                            INNER JOIN users AS factory ON porder.userid = factory.user_id
                                            WHERE porder.po_req_etd >= @weekStart AND porder.po_req_etd < @weekEnd AND order_header.container_type IN (0,1,2,6) AND order_header.status NOT IN ('X','Y')
                                            GROUP BY porder.userid,WEEK(porder.po_req_etd,7),factory.user_name, factory.consolidated_port
                                            ORDER BY WEEK(porder.po_req_etd,7),factory.consolidated_port, factory.factory_code", conn);
                cmd.Parameters.AddWithValue("@weekStart", weekStart);
                cmd.Parameters.AddWithValue("@weekEnd", weekStart.AddDays(7*weekspan - 1));
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    var data = new OrderWeeklyData();
                    data.company_id = (int) dr["userid"];
                    data.company_name = String.Empty + dr["user_name"];
                    data.customer_code = String.Empty + dr["factory_code"];
                    data.consolidated_port = (int) dr["consolidated_port"];
                    data.WeekStart = Utilities.GetFirstDayInWeek((DateTime) dr["req_etd"]);
                    data.OrderCount = Convert.ToInt32(dr["count"]);
                    result.Add(data);
                }

            }
            return result;
        }

        public static int? GetTotalQty(string cprod_code1, int company_id)
        {
            int? result = 0;
            using (var conn = new MySqlConnection(Settings.Default.ConnString))
            {
                conn.Open();
                var cmd =new MySqlCommand(@"SELECT SUM(orderqty) FROM
                                    order_header INNER JOIN order_lines ON order_lines.orderid = order_header.orderid
                                    INNER JOIN cust_products ON order_lines.cprod_id = cust_products.cprod_id
                                    WHERE order_header.userid1 = @company_id AND cust_products.cprod_code1 = @code", conn);
                cmd.Parameters.AddWithValue("@company_id", company_id);
                cmd.Parameters.AddWithValue("@code", cprod_code1);
                result = Convert.ToInt32(Utilities.FromDbValue<long>(cmd.ExecuteScalar()));
            }
            return result;
        }


        public static Order_header GetById(int id)
		{
			Order_header result = null;
			using (var conn = new MySqlConnection(Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM order_header WHERE orderid = @id", conn);
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

        public static DateTime? GetMaxEtd(List<int> ids )
        {
            DateTime? result = null;
            using (var conn = new MySqlConnection(Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("", conn);
                cmd.CommandText =
                    String.Format(@"SELECT MAX(po_req_etd) AS MaxETD FROM porder_header INNER JOIN order_header ON porder_header.soorderid = order_header.orderid 
                                                    WHERE order_header.orderid IN ({0})",
                                  Utilities.CreateParametersFromIdList(cmd, ids));
                //cmd.Parameters.AddWithValue("@id", id);
                result = Utilities.FromDbValue<DateTime>(cmd.ExecuteScalar());
            }
            return result;
        }
	
		public static Order_header GetFromDataReader(MySqlDataReader dr)
		{
			Order_header o = new Order_header();
		
			o.orderid =  (int) dr["orderid"];
			o.orderdate = Utilities.FromDbValue<DateTime>(dr["orderdate"]);
			o.userid1 = Utilities.FromDbValue<int>(dr["userid1"]);
			o.locid = Utilities.FromDbValue<int>(dr["locid"]);
			o.stock_order = Utilities.FromDbValue<int>(dr["stock_order"]);
			o.status = String.Empty + dr["status"];
			o.new_status = String.Empty + dr["new_status"];
			o.delivery_address1 = String.Empty + dr["delivery_address1"];
			o.delivery_address2 = String.Empty + dr["delivery_address2"];
			o.delivery_address3 = String.Empty + dr["delivery_address3"];
			o.delivery_address4 = String.Empty + dr["delivery_address4"];
			o.delivery_address5 = String.Empty + dr["delivery_address5"];
			o.currency = Utilities.FromDbValue<int>(dr["currency"]);
			o.surcharge = Utilities.FromDbValue<double>(dr["surcharge"]);
			o.notes = String.Empty + dr["notes"];
			o.custpo = String.Empty + dr["custpo"];
			o.req_etd = Utilities.FromDbValue<DateTime>(dr["req_etd"]);
			o.original_eta = Utilities.FromDbValue<DateTime>(dr["original_eta"]);
			o.req_eta = Utilities.FromDbValue<DateTime>(dr["req_eta"]);
			o.actual_eta = Utilities.FromDbValue<DateTime>(dr["actual_eta"]);
			o.loading_details = String.Empty + dr["loading_details"];
			o.reference_no = String.Empty + dr["reference_no"];
			o.factory_pl = Utilities.FromDbValue<int>(dr["factory_pl"]);
			o.lme = Utilities.FromDbValue<double>(dr["lme"]);
			o.packing_list = String.Empty + dr["packing_list"];
			o.edit_sort = Utilities.FromDbValue<int>(dr["edit_sort"]);
			o.mod_flag = Utilities.FromDbValue<int>(dr["mod_flag"]);
			o.eta_flag = Utilities.FromDbValue<int>(dr["eta_flag"]);
			o.process_id = Utilities.FromDbValue<int>(dr["process_id"]);
			o.combined_order = Utilities.FromDbValue<int>(dr["combined_order"]);
			o.loading_factory = Utilities.FromDbValue<int>(dr["loading_factory"]);
			o.upload = String.Empty + dr["upload"];
			o.upload_flag = Utilities.FromDbValue<int>(dr["upload_flag"]);
			o.entered_by = Utilities.FromDbValue<int>(dr["entered_by"]);
			o.payment = Utilities.FromDbValue<int>(dr["payment"]);
			o.documents = Utilities.FromDbValue<int>(dr["documents"]);
			o.documents2 = Utilities.FromDbValue<int>(dr["documents2"]);
			o.loading_date = Utilities.FromDbValue<DateTime>(dr["loading_date"]);
			o.container_type = Utilities.FromDbValue<int>(dr["container_type"]);
			o.loading_perc = Utilities.FromDbValue<double>(dr["loading_perc"]);
			o.forwarder_name = String.Empty + dr["forwarder_name"];
			o.despatch_note = String.Empty + dr["despatch_note"];
		    o.booked_in_date = Utilities.FromDbValue<DateTime>(dr["booked_in_date"]);

		    if (Utilities.ColumnExists(dr, "po_req_etd"))
		        o.po_req_etd = Utilities.FromDbValue<DateTime>(dr["po_req_etd"]);
            if (Utilities.ColumnExists(dr, "original_po_req_etd"))
                o.original_po_req_etd = Utilities.FromDbValue<DateTime>(dr["original_po_req_etd"]);
		    if (Utilities.ColumnExists(dr, "po_comments"))
		        o.po_comments = string.Empty + dr["po_comments"];
			
			return o;

		}
		
		public static void Create(Order_header o)
        {
            string insertsql = @"INSERT INTO order_header (orderid,orderdate,userid1,locid,stock_order,status,new_status,delivery_address1,delivery_address2,delivery_address3,delivery_address4,delivery_address5,currency,surcharge,notes,custpo,req_etd,original_eta,req_eta,actual_eta,loading_details,reference_no,factory_pl,lme,packing_list,edit_sort,mod_flag,eta_flag,process_id,combined_order,loading_factory,upload,upload_flag,entered_by,payment,documents,documents2,loading_date,container_type,loading_perc,forwarder_name,despatch_note) VALUES(@orderid,@orderdate,@userid1,@locid,@stock_order,@status,@new_status,@delivery_address1,@delivery_address2,@delivery_address3,@delivery_address4,@delivery_address5,@currency,@surcharge,@notes,@custpo,@req_etd,@original_eta,@req_eta,@actual_eta,@loading_details,@reference_no,@factory_pl,@lme,@packing_list,@edit_sort,@mod_flag,@eta_flag,@process_id,@combined_order,@loading_factory,@upload,@upload_flag,@entered_by,@payment,@documents,@documents2,@loading_date,@container_type,@loading_perc,@forwarder_name,@despatch_note)";

		    var conn = new MySqlConnection(Settings.Default.ConnString);
            conn.Open();
		    var tr = conn.BeginTransaction();
		    try
		    {

		        var cmd = new MySqlCommand("SELECT nextorderid FROM nextorderid", conn, tr);
		        o.orderid = Convert.ToInt32(cmd.ExecuteScalar());

		        cmd.CommandText = "UPDATE nextorderid SET nextorderid = nextorderid+1";
		        cmd.ExecuteNonQuery();

		        cmd.CommandText = insertsql;

		        BuildSqlParameters(cmd, o);
		        cmd.ExecuteNonQuery();

		        if (o.Lines != null)
		        {
		            foreach (var line in o.Lines)
		            {
		                line.orderid = o.orderid;
		                Order_linesDAL.Create(line, tr);
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
		
		private static void BuildSqlParameters(MySqlCommand cmd, Order_header o, bool forInsert = true)
        {
			
			cmd.Parameters.AddWithValue("@orderid", o.orderid);
			cmd.Parameters.AddWithValue("@orderdate", o.orderdate);
			cmd.Parameters.AddWithValue("@userid1", o.userid1);
			cmd.Parameters.AddWithValue("@locid", o.locid);
			cmd.Parameters.AddWithValue("@stock_order", o.stock_order);
			cmd.Parameters.AddWithValue("@status", o.status);
			cmd.Parameters.AddWithValue("@new_status", o.new_status);
			cmd.Parameters.AddWithValue("@delivery_address1", o.delivery_address1);
			cmd.Parameters.AddWithValue("@delivery_address2", o.delivery_address2);
			cmd.Parameters.AddWithValue("@delivery_address3", o.delivery_address3);
			cmd.Parameters.AddWithValue("@delivery_address4", o.delivery_address4);
			cmd.Parameters.AddWithValue("@delivery_address5", o.delivery_address5);
			cmd.Parameters.AddWithValue("@currency", o.currency);
			cmd.Parameters.AddWithValue("@surcharge", o.surcharge);
			cmd.Parameters.AddWithValue("@notes", o.notes);
			cmd.Parameters.AddWithValue("@custpo", o.custpo);
			cmd.Parameters.AddWithValue("@req_etd", o.req_etd);
			cmd.Parameters.AddWithValue("@original_eta", o.original_eta);
			cmd.Parameters.AddWithValue("@req_eta", o.req_eta);
			cmd.Parameters.AddWithValue("@actual_eta", o.actual_eta);
			cmd.Parameters.AddWithValue("@loading_details", o.loading_details);
			cmd.Parameters.AddWithValue("@reference_no", o.reference_no);
			cmd.Parameters.AddWithValue("@factory_pl", o.factory_pl);
			cmd.Parameters.AddWithValue("@lme", o.lme);
			cmd.Parameters.AddWithValue("@packing_list", o.packing_list);
			cmd.Parameters.AddWithValue("@edit_sort", o.edit_sort);
			cmd.Parameters.AddWithValue("@mod_flag", o.mod_flag);
			cmd.Parameters.AddWithValue("@eta_flag", o.eta_flag);
			cmd.Parameters.AddWithValue("@process_id", o.process_id);
			cmd.Parameters.AddWithValue("@combined_order", o.combined_order);
			cmd.Parameters.AddWithValue("@loading_factory", o.loading_factory);
			cmd.Parameters.AddWithValue("@upload", o.upload);
			cmd.Parameters.AddWithValue("@upload_flag", o.upload_flag);
			cmd.Parameters.AddWithValue("@entered_by", o.entered_by);
			cmd.Parameters.AddWithValue("@payment", o.payment);
			cmd.Parameters.AddWithValue("@documents", o.documents);
			cmd.Parameters.AddWithValue("@documents2", o.documents2);
			cmd.Parameters.AddWithValue("@loading_date", o.loading_date);
			cmd.Parameters.AddWithValue("@container_type", o.container_type);
			cmd.Parameters.AddWithValue("@loading_perc", o.loading_perc);
			cmd.Parameters.AddWithValue("@forwarder_name", o.forwarder_name);
			cmd.Parameters.AddWithValue("@despatch_note", o.despatch_note);
		}
		
		public static void Update(Order_header o)
		{
			string updatesql = @"UPDATE order_header SET orderdate = @orderdate,userid1 = @userid1,locid = @locid,stock_order = @stock_order,status = @status,new_status = @new_status,delivery_address1 = @delivery_address1,delivery_address2 = @delivery_address2,delivery_address3 = @delivery_address3,delivery_address4 = @delivery_address4,delivery_address5 = @delivery_address5,currency = @currency,surcharge = @surcharge,notes = @notes,custpo = @custpo,req_etd = @req_etd,original_eta = @original_eta,req_eta = @req_eta,actual_eta = @actual_eta,loading_details = @loading_details,reference_no = @reference_no,factory_pl = @factory_pl,lme = @lme,packing_list = @packing_list,edit_sort = @edit_sort,mod_flag = @mod_flag,eta_flag = @eta_flag,process_id = @process_id,combined_order = @combined_order,loading_factory = @loading_factory,upload = @upload,upload_flag = @upload_flag,entered_by = @entered_by,payment = @payment,documents = @documents,documents2 = @documents2,loading_date = @loading_date,container_type = @container_type,loading_perc = @loading_perc,forwarder_name = @forwarder_name,despatch_note = @despatch_note WHERE orderid = @orderid";

			using (var conn = new MySqlConnection(Settings.Default.ConnString))
            {
                conn.Open();
				MySqlCommand cmd = new MySqlCommand(updatesql, conn);
                BuildSqlParameters(cmd,o, false);
                cmd.ExecuteNonQuery();
            }
		}
		
		public static void Delete(int orderid)
		{
			using (var conn = new MySqlConnection(Settings.Default.ConnString))
            {
                conn.Open();
				MySqlCommand cmd = new MySqlCommand("DELETE FROM order_header WHERE orderid = @id" , conn);
                cmd.Parameters.AddWithValue("@id", orderid);
                cmd.ExecuteNonQuery();
            }
		}

        public static Order_header GetByCustpo(string custpo)
        {
            Order_header result = null;
            using (var conn = new MySqlConnection(Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM order_header WHERE custpo = @custpo", conn);
                cmd.Parameters.AddWithValue("@custpo", custpo);
                MySqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    result = GetFromDataReader(dr);
                }
                dr.Close();
            }
            return result;
        }

        public static List<Company> GetClientsOnOrders(int? factory_id)
        {
            var result = new List<Company>();
            using (var conn = new MySqlConnection(Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand(@"SELECT users.* FROM users WHERE user_id IN (SELECT DISTINCT userid1 FROM order_header
                                             INNER JOIN order_lines ON order_header.orderid = order_lines.orderid
                                             INNER JOIN cust_products ON order_lines.cprod_id = cust_products.cprod_id
                                             INNER JOIN mast_products ON cust_products.cprod_mast = mast_products.mast_id
                                             WHERE mast_products.factory_id = @factory_id OR @factory_id IS NULL)", conn);
                cmd.Parameters.AddWithValue("@factory_id", factory_id != null ? (object) factory_id : DBNull.Value);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(CompanyDAL.GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
        }

        public static List<Cust_products> GetProductsOnOrders(int? location_id,int? factory_id, int? client_id, string criteria, bool spares, bool discontinued)
        {
            var result = new List<Cust_products>();
            using (var conn = new MySqlConnection(Settings.Default.ConnString))
            {
                conn.Open();
                //client_id can be -1 (all clients that are brand distributors)
                string sql;
                if (client_id != null)
                {
                    sql = @"SELECT cust_products.*,mast_products.* FROM cust_products
                                             INNER JOIN mast_products ON cust_products.cprod_mast = mast_products.mast_id 
                                             INNER JOIN users factory ON mast_products.factory_id = factory.user_id
                                             WHERE (mast_products.factory_id = @factory_id OR @factory_id IS NULL) 
                                                AND (factory.consolidated_port = @location OR @location IS NULL)
                                                AND (@criteria IS NULL OR cprod_code1 LIKE @criteria OR factory_ref LIKE @criteria OR asaq_name LIKE @criteria)
                                                AND (@spares = 1 OR mast_products.category1 <> 13)
                                                AND cust_products.cprod_status <> 'D'
                                                AND cprod_id IN 
                                             (SELECT DISTINCT cprod_id FROM order_lines
                                                 INNER JOIN  order_header ON order_header.orderid = order_lines.orderid
                                                 WHERE (order_header.userid1 = @client_id 
                                                          OR (@client_id = -1 AND EXISTS(SELECT user_id FROM users WHERE order_header.userid1 = users.user_id AND distributor > 0 ) )  
                                                       ) AND order_header.status NOT IN ('X','Y')
                                             )";
                }
                else
                {
                    sql = @"SELECT mast_products.*, 
                            (SELECT GROUP_CONCAT(cprod_code1,', ') FROM cust_products WHERE cust_products.cprod_status <> 'D' AND cust_products.cprod_mast = mast_products.mast_id AND (@criteria IS NULL OR cust_products.cprod_code1 LIKE @criteria)) AS cprod_code1,
                            (SELECT SUM(cprod_stock) FROM cust_products WHERE cust_products.cprod_status <> 'D' AND cust_products.cprod_mast = mast_products.mast_id AND (@criteria IS NULL OR cust_products.cprod_code1 LIKE @criteria)) AS cprod_stock,
                            (SELECT GROUP_CONCAT(cprod_stock_code, ', ') FROM cust_products WHERE cust_products.cprod_status <> 'D' AND cust_products.cprod_mast = mast_products.mast_id AND (@criteria IS NULL OR cust_products.cprod_code1 LIKE @criteria)) AS cprod_stock_code
                            FROM mast_products INNER JOIN users factory ON mast_products.factory_id = factory.user_id
                            WHERE (mast_products.factory_id = @factory_id OR @factory_id IS NULL) AND (factory.consolidated_port = @location OR @location IS NULL) AND
                            (@criteria IS NULL OR factory_ref LIKE @criteria OR asaq_name LIKE @criteria OR 
                                EXISTS (SELECT order_lines.cprod_id FROM order_lines
                                                 INNER JOIN  order_header ON order_header.orderid = order_lines.orderid
                                                 INNER JOIN cust_products ON order_lines.cprod_id = cust_products.cprod_id
                                                 WHERE cust_products.cprod_status <> 'D' AND cust_products.cprod_mast = mast_products.mast_id AND order_header.status NOT IN ('X','Y')
                                                    AND cust_products.cprod_code1 LIKE @criteria)
                            ) 
                            AND (@spares = 1 OR mast_products.category1 <> 13)
                            AND mast_id IN 
                            (SELECT cprod_mast FROM cust_products
                                                WHERE cust_products.cprod_status <> 'D' AND cprod_id IN 
                                             (SELECT DISTINCT cprod_id FROM order_lines
                                                 INNER JOIN  order_header ON order_header.orderid = order_lines.orderid
                                                 WHERE order_header.status NOT IN ('X','Y')
                                             )
                            )";
                }
                var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@location", Utilities.ToDBNull(location_id));
                cmd.Parameters.AddWithValue("@factory_id", Utilities.ToDBNull(factory_id));
                cmd.Parameters.AddWithValue("@client_id", Utilities.ToDBNull(client_id));
                cmd.Parameters.AddWithValue("@criteria",!string.IsNullOrEmpty(criteria) ? (object) ("%" + criteria + "%") : DBNull.Value);
                cmd.Parameters.AddWithValue("@spares", spares ? 1 : 0);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    List<int> stock_codes = null;
                    if (client_id == null)
                    {
                        stock_codes = (string.Empty + dr["cprod_stock_code"]).Split(',').Where(s=>!string.IsNullOrEmpty(s.Trim())).Select(int.Parse).Distinct().ToList();
                    }
                    else
                    {
                        stock_codes = new int[] {(int) dr["cprod_stock_code"]}.ToList();
                    }
                    var prod = (client_id != null ? Cust_productsDAL.GetFromDataReader(dr) : 
                        new Cust_products{cprod_code1 = string.Empty + dr["cprod_code1"], cprod_stock = Utilities.FromDbValue<int>(dr["cprod_stock"]),cprod_stock_codes = stock_codes});
                    prod.MastProduct = Mast_productsDAL.GetFromDataReader(dr);
                    if (client_id != null)
                        prod.cprod_stock_codes = stock_codes;
                    result.Add(prod);
                }
                dr.Close();
            }
            return result;
        }

        public static int GetNumberOfOrders(int cprod_id, DateTime? from = null, DateTime? to = null)
        {
            var result = 0;
            using (var conn = new MySqlConnection(Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand(@"SELECT COUNT(*) FROM order_header WHERE (orderdate >= @from OR @from IS NULL) AND (orderdate <= @to OR @to IS NULL) AND 
                                             EXISTS (SELECT cprod_id FROM order_lines WHERE orderid = order_header.orderid AND cprod_id = @cprod_id) AND status NOT IN ('X','Y') AND stock_order = 1", conn);
                cmd.Parameters.AddWithValue("@from", from != null ? (object) from : DBNull.Value);
                cmd.Parameters.AddWithValue("@to", to != null ? (object) to : DBNull.Value);
                cmd.Parameters.AddWithValue("@cprod_id", cprod_id);
                result = Convert.ToInt32(cmd.ExecuteScalar());
            }
            return result;
        }
	}


}
			
			