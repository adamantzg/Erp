
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;
using Dapper;

namespace erp.Model.DAL
{
	public class ReturnsDAL
	{

		public static List<Returns> GetAll()
		{
			var result = new List<Returns>();
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
			{
				conn.Open();
				var cmd = Utils.GetCommand("SELECT * FROM returns", conn);
				var dr = cmd.ExecuteReader();
				while (dr.Read())
				{
					result.Add(GetFromDataReader(dr));
				}
				dr.Close();
			}
			return result;
		}
        
        public static List<Returns> GetAllForProduct(int cprod_id)
        {
            var result = new List<Returns>();
            using (var conn=new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM returns WHERE returns.cprod_id=@cprod_id", conn);
                cmd.Parameters.AddWithValue("@cprod_id",cprod_id);

                var dr= cmd.ExecuteReader();

                while(dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
        }

        public static List<Returns> GetByClient(int client_id)
        {
            var result = new List<Returns>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM returns WHERE client_id=@client_id  AND status1 = 1 ORDER BY request_date DESC", conn);
                cmd.Parameters.AddWithValue("@client_id", client_id);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
        }

		public static List<Returns> GetInPeriod(DateTime? from = null, DateTime? to = null)
		{
			var result = new List<Returns>();
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
			{
				conn.Open();
				var cmd = Utils.GetCommand(@"SELECT returns.*,cust_products.* FROM returns INNER JOIN cust_products ON returns.cprod_id = cust_products.cprod_id 
											WHERE claim_type <> 5 AND decision_final = 1 AND  (cc_response_date >= @from OR @from IS NULL) AND (cc_response_date <= @to OR @to IS NULL)", conn);
				cmd.Parameters.AddWithValue("@from", from != null ? (object) from : DBNull.Value);
				cmd.Parameters.AddWithValue("@to", to != null ? (object)to : DBNull.Value);
				var dr = cmd.ExecuteReader();
				while (dr.Read())
				{
					var r = GetFromDataReader(dr);
					r.Product = Cust_productsDAL.GetFromDataReader(dr);
					result.Add(r);
				}
				dr.Close();
			}
			return result;
		}

		public static List<Returns> GetForClaimType(int claim_type, bool products=false, IList<int?> groupsId = null)
		{
			var result = new List<Returns>();
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
			{
				conn.Open();
				var cmd = Utils.GetCommand($@"SELECT returns.*,(SELECT COUNT(*) FROM returns_comments WHERE return_id = returns.returnsid AND comments NOT LIKE 'Authorization%') AS no_of_comments,
                                        (SELECT comments_from FROM returns_comments WHERE return_id = returns.returnsid AND comments NOT LIKE 'Authorization%' ORDER BY comments_date DESC LIMIT 1) AS last_commenter_id
										FROM returns 
										WHERE claim_type = @claim_type {(groupsId != null && groupsId.Count > 0 ? $" AND usergroup_id IN ({string.Join(",",groupsId)})" : "")}", conn);
				cmd.Parameters.AddWithValue("@claim_type", claim_type);
				var dr = cmd.ExecuteReader();
				while (dr.Read())
				{
					var r = GetFromDataReader(dr);

                    r.HasComments = Convert.ToInt64(dr["no_of_comments"]) > 0;
                    if (r.request_userid != null)
                        r.Creator = UserDAL.GetById(r.request_userid.Value);
         //           if(products)
					    //r.Product = Cust_productsDAL.GetById(r.cprod_id??0);
					result.Add(r);
				}
				dr.Close();

                //if (products)
                //{
                //    foreach (var p in result)
                //    {
                //        p.Product = Cust_productsDAL.GetById(p.cprod_id??0);
                //    }
                //}
			}
			return result;
		}

        public static List<Returns> GetForClaimTypeSimple(int claim_type, bool products = false, IList<int?> groupsId = null)
        {
            var result = new List<Returns>();

            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand($@"SELECT returns.returnsid,
                                            returns.status1,
                                            returns.request_userid,
                                            returns.claim_type,
                                            returns.issue_type_id,
                                            returns.authorization_level,
                                        (SELECT COUNT(*) FROM returns_comments WHERE return_id = returns.returnsid AND comments NOT LIKE 'Authorization%') AS no_of_comments,
                                        (SELECT comments_from FROM returns_comments WHERE return_id = returns.returnsid AND comments NOT LIKE 'Authorization%' ORDER BY comments_date DESC LIMIT 1) AS last_commenter_id
										FROM returns 
										WHERE claim_type = @claim_type {(groupsId != null && groupsId.Count > 0 ? $" AND usergroup_id IN ({string.Join(",", groupsId)})" : "")}", conn);

                cmd.Parameters.AddWithValue("@claim_type", claim_type);

                var dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    var r = new Returns();

                    r.returnsid = (int)dr["returnsid"];
                    r.Last_Commenter_Id = Utilities.FromDbValue<int>(dr["last_commenter_id"]);
                    r.HasComments = Convert.ToInt64(dr["no_of_comments"]) > 0;

                    r.status1 = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "status1"));
                    r.request_userid = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "request_userid"));
                    r.claim_type = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "claim_type"));
                    r.issue_type_id = Utilities.FromDbValue<int>(dr["issue_type_id"]);
                    r.authorization_level = Utilities.FromDbValue<int>(dr["authorization_level"]);

                    result.Add(r);
                }

                dr.Close();

            }
            return result;
        }

        public static List<Returns> Search(int? claim_type, string text)
		{
			var result = new List<Returns>();
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
			{
				conn.Open();
				var cmd =
					Utils.GetCommand(
						@"SELECT returns.*,(SELECT COUNT(*) FROM returns_comments WHERE return_id = returns.returnsid) AS no_of_comments, 
							(SELECT comments_from FROM returns_comments WHERE return_id = returns.returnsid ORDER BY comments_date DESC LIMIT 1) AS last_commenter_id 
							FROM returns WHERE claim_type = @claim_type 
							AND (client_comments LIKE @text OR return_no LIKE @text OR EXISTS (SELECT comments_id FROM returns_comments WHERE return_id = returns.returnsid AND comments LIKE @text ) )",
						conn);
				cmd.Parameters.AddWithValue("@claim_type", claim_type);
				cmd.Parameters.AddWithValue("@text", "%"+text+"%");
				var dr = cmd.ExecuteReader();
				while (dr.Read())
				{
					result.Add(GetFromDataReader(dr));
				}
				dr.Close();
			}
			return result;
		}




		//public static List<Product_investigations> GetClaimInvestigationForProduct(int cprodId)
		//{
		//    var productInvestigations = new List<Product_investigations>();
		//    using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
		//    {
		//        conn.Open();
		//        var cmd = Utils.GetCommand(
		//                @"SELECT * FROM product_investigations WHERE product_investigations.cprod_id = @cprod_id",
		//                conn);
		//        cmd.Parameters.AddWithValue("@cprod_id", cprodId);
		//        var dr = cmd.ExecuteReader();
		//        while (dr.Read())
		//        {
		//            productInvestigations.Add(GetFromClaimDataReader(dr));
		//        }
		//        dr.Close();

		//    }
		//    return productInvestigations;

		//}

		//private static void BuildSqlClaimParameters(MySqlCommand cmd, Product_investigations o, bool forInsert = true)
		//{
		//    if (!forInsert)
		//        cmd.Parameters.AddWithValue("@id", o.Id);
		//    cmd.Parameters.AddWithValue("@CprodId", o.CprodId);
		//    cmd.Parameters.AddWithValue("@MastId", o.MastId);
		//    cmd.Parameters.AddWithValue("@Date", o.Date);
		//    cmd.Parameters.AddWithValue("@Status", o.Status);
		//    cmd.Parameters.AddWithValue("@Comments", o.Comments);
		//    cmd.Parameters.AddWithValue("@MonitoredBy", o.MonitoredBy);


		//}
		//public static void CreateStatus(ProductInvestigations o)
		//{
		//    string insertsql = @"INSERT INTO product_investigations (cprod_id,mast_id,date,status,comments,monitored_by) VALUES(@CprodId,@MastId,@Date,@Status,@Comments,@MonitoredBy)";
		//    using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
		//    {
		//        conn.Open();
		//        var cmd = Utils.GetCommand(insertsql, conn);
		//        BuildSqlClaimParameters(cmd, o);
		//        cmd.ExecuteNonQuery();
		//        cmd.Parameters.Clear();
		//        cmd.CommandText = "SELECT id FROM product_investigations WHERE id = LAST_INSERT_ID()";
		//        o.Id = (int)cmd.ExecuteScalar();
		//    }
		//}

		//public static ProductInvestigations GetFromClaimDataReader(MySqlDataReader dr)
		//{
		//    ProductInvestigations o = new ProductInvestigations();
		//    o.Id = (int)dr["id"];
		//    o.CprodId = (int)dr["cprod_id"];
		//    o.MastId = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "mast_id"));
		//    o.MonitoredBy = string.Empty + Utilities.GetReaderField(dr, "monitored_by");
		//    o.Date = Utilities.FromDbValue<DateTime>(Utilities.GetReaderField(dr, "date"));
		//    o.Status = (int)dr["status"];
		//    o.Comments = string.Empty + Utilities.GetReaderField(dr, "comments");
		//    return o;
		//}

		public static Returns GetById(int id)
		{
            var columnMap = new Dictionary<string, string>();
            columnMap.Add("importance", "importance_id");
            

            SqlMapper.SetTypeMap(typeof(Returns), new CustomPropertyTypeMap(typeof(Returns), 
                (type, columnName) => type.GetProperty(columnMap.ContainsKey(columnName) ? columnMap[columnName] : columnName)));

            Returns result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
			{
				conn.Open();
				result = conn.Query<Returns, Feedback_category,User,Returns_importance, feedback_issue_type,Returns>
                    (@"SELECT r.*, c.*, u.*, i.*, it.* 
                       FROM returns r LEFT OUTER JOIN feedback_category c ON r.feedback_category_id = c.feedback_cat_id
                        LEFT OUTER JOIN userusers u ON r.request_userid = u.useruserid 
                        LEFT OUTER JOIN Returns_importance i ON r.importance = i.importance_id
                        LEFT OUTER JOIN feedback_issue_type it ON r.issue_type_id = it.id
                       WHERE r.returnsid = @id", 
                       (r,c,u,i,it) => {

                           r.Category = c;
                           r.Creator = u;
                           r.Importance = i;
                           r.IssueType = it;
                           return r;
                       },
                    
                    new { id = id},splitOn: "feedback_cat_id,useruserid,importance_id,id").FirstOrDefault();
				
				if (result != null)
				{
					result.Images = conn.Query<Returns_images>("SELECT * FROM Returns_images WHERE return_id = @id",new { id = id}).ToList();
                    result.Comments = new List<Returns_comments>();
                    conn.Query<Returns_comments, User, Returns_comments_files, Returns_comments>
                        (@"SELECT c.*,u.*,f.* 
                            FROM Returns_comments c LEFT OUTER JOIN userusers u ON c.comments_from = u.useruserid
                            LEFT OUTER JOIN Returns_comments_files f ON f.return_comment_id = c.comments_id 
                            WHERE c.return_id = @id", (c,u, f) => {

                            var comm = result.Comments.FirstOrDefault(com => com.comments_id == c.comments_id);
                            if(comm == null)
                            {
                                comm = c;
                                comm.Creator = u;
                                result.Comments.Add(comm);
                            }
                                
                            if (comm.Files == null)
                                comm.Files = new List<Returns_comments_files>();
                            if(f != null)
                                comm.Files.Add(f);
                            return comm;
                        }, new { id = id },splitOn: "useruserid,return_comment_file_id").ToList();
                    result.Subscriptions = new List<Feedback_subscriptions>();
                    conn.Query<Feedback_subscriptions, User, Feedback_subscriptions>
                        (@"SELECT s.*,u.* 
                            FROM Feedback_subscriptions s LEFT OUTER JOIN userusers u ON s.subs_useruserid = u.useruserid
                            WHERE s.subs_returnid = @id", (s, u) => {
                            result.Subscriptions.Add(s);
                            s.User = u;
                            return s;
                        }, new { id = id },splitOn: "useruserid").ToList();


                    /*
                    if (result.assigned_qc != null)
                        result.AssignedQC = conn.QueryFirst("SELECT * FROM userusers WHERE useruserid = @id", new { id = result.assigned_qc });
                    */
                }
				
			}
			return result;
		}
		/*-*/
		public static List<ReturnAggregateDataByMonth> GetQtyForPeriodGroupedByMonths(int cprod_user, IList<string> cprodCode)
		{
			var result=new List<ReturnAggregateDataByMonth>();
			using(var conn=new MySqlConnection(Properties.Settings.Default.ConnString))
			{
				conn.Open();
				var cmd = Utils.GetCommand("",conn);
					cmd.CommandText = string.Format(@"SELECT
						Count(returns.returnsid) AS count_returns,
						SUM(returns.return_qty)AS sum_returns_product,
						CAST(DATE_FORMAT(returns.cc_response_date,""%y%m"")AS UNSIGNED) AS created_month,
						cust_products.cprod_code1
						FROM
						returns
						INNER JOIN cust_products ON returns.cprod_id = cust_products.cprod_id
						WHERE
						cust_products.cprod_user = @cprod_user AND
						returns.decision_final  = 1 AND
						cust_products.cprod_code1 IN ({0})
						GROUP BY created_month",cprodCode!=null?Utils.CreateParametersFromIdList(cmd,cprodCode):"");
					cmd.Parameters.AddWithValue("@cprod_user",cprod_user);
				var dr = cmd.ExecuteReader();
				while(dr.Read())
				{
					var data=new ReturnAggregateDataByMonth();
					data.CountReturns=Convert.ToInt32(dr["count_returns"]);
					data.SumReturnsProduct=Convert.ToInt32(dr["sum_returns_product"]);
					//data.created_month = Utilities.FromDbValue<DateTime>(Utilities.GetReaderField(dr, "created_month"));
					data.created_month =Convert.ToInt32(dr["created_month"]);
					data.cprod_code1=string.Empty+dr["cprod_code1"];
					result.Add(data);
				}
				dr.Close();

			}
			 return result;
		}

		public static int GetNoOfReturns(int company_id, DateTime? date = null,int? claim_type = null)
		{
			int result = 0;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
			{
				conn.Open();
				var cmd = Utils.GetCommand("SELECT COUNT(*) FROM returns WHERE client_id = @id AND (request_date BETWEEN @date AND (@date + INTERVAL 1 day - INTERVAL 1 second) OR @date IS NULL) AND (claim_type = @claim_type OR @claim_type IS NULL)", conn);
				cmd.Parameters.AddWithValue("@id", company_id);
				cmd.Parameters.AddWithValue("@date", date != null ? (object) date : DBNull.Value);
				cmd.Parameters.AddWithValue("@claim_type", claim_type != null ? (object) claim_type : DBNull.Value);
				result = Convert.ToInt32(cmd.ExecuteScalar());
			}
			return result;
		}

        public static int GetNextITFeedbackNum()
        {

            return GetNextFeedbackNum(Returns.ClaimType_ITFeedback);
        }

        public static int GetNextFeedbackNum(int type)
        {

            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString)) {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT typename FROM feedback_type WHERE type_id = @type",conn);
                cmd.Parameters.AddWithValue("@type", type);
                var typename = string.Empty + cmd.ExecuteScalar();
                cmd.CommandText =
                        $"SELECT MAX(CAST(REPLACE(return_no,'{typename}-','') AS UNSIGNED)) FROM returns WHERE claim_type=@type";
                return Convert.ToInt32(Utilities.FromDbValue<long>(cmd.ExecuteScalar()) ?? 0) + 1;
            }
        }

        public static ReturnAggregateData GetQtyInPeriod(string cprod_code1, int company_id, DateTime? dateFrom = null, DateTime? dateTo = null)
		{
			var result = new ReturnAggregateData();

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
			{
				conn.Open();
				MySqlCommand cmd = Utils.GetCommand(@"SELECT COUNT(*) AS qty FROM returns INNER JOIN cust_products ON  returns.cprod_id = cust_products.cprod_id
									WHERE cprod_code1 = @code AND client_id = @company_id and status1 = 1 AND (request_date >= @dateFrom OR @dateFrom IS NULL) AND
									(request_date <= @dateTo OR @dateTo IS NULL) ", conn);
				cmd.Parameters.AddWithValue("@company_id", company_id);
				cmd.Parameters.AddWithValue("@code", cprod_code1);
				cmd.Parameters.AddWithValue("@dateFrom", dateFrom != null ? (object) dateFrom : DBNull.Value);
				cmd.Parameters.AddWithValue("@dateTo", dateTo != null ? (object) dateTo : DBNull.Value);
				result.TotalQty = Convert.ToInt32(cmd.ExecuteScalar());

				cmd.CommandText += " AND decision_final = @decision";
				cmd.Parameters.AddWithValue("@decision", 1);
				result.TotalAccepted = Convert.ToInt32(cmd.ExecuteScalar());

				cmd.Parameters[cmd.Parameters.Count - 1].Value = 999;
				result.TotalRejected = Convert.ToInt32(cmd.ExecuteScalar());
			}
			return result;
		}

		public static List<ReturnAggregateDataProduct> GetTopNTotalsPerProduct(
            DateTime? dateFrom = null,DateTime? dateTo = null, int? top = 10, IList<int> incClients = null,
            IList<int> exClients = null, bool groupByBrands = true,bool excludeSpares = true,
            bool groupByReason = true,bool useETA=false,bool extendToEndOfMonthForUnits = false, bool filterCprodStatus=true,CountryFilter countryFilter = CountryFilter.UKOnly,
            SortField sortBy = SortField.ReturnToSalesRatio,int? minUnitsShipped = null)
		{
			var result = new List<ReturnAggregateDataProduct>();
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
			{
                //var filterbyCountry = filterCountry.Split(',');
				conn.Open();
				var cmd = Utils.GetCommand("", conn);
			    var cprod_codes = new List<string>();
			    if (groupByReason)
			    {
			        cmd.CommandText =
			            string.Format(
                            @"SELECT cust_products.cprod_code1,SUM(returns.return_qty) AS numOfReturns,
                           COALESCE ((COALESCE( SUM(returns.return_qty),0 )/  COALESCE((SELECT SUM(orderqty) FROM om_detail1 WHERE om_detail1.cprod_code1 = cust_products.cprod_code1 {6} AND ({4} >= @datefrom OR @datefrom IS NULL) 
                                                                                AND ({4} <= @dateto OR @dateto IS NULL)),0)),9999) AS ReturnToSalesRatio,
                            SUM(returns.return_qty*returns.credit_value) AS TotalReturnValue
                            FROM returns INNER JOIN cust_products ON returns.cprod_id = cust_products.cprod_id  
                                INNER JOIN users ON returns.client_id = users.user_id 
								INNER JOIN mast_products ON cust_products.cprod_mast = mast_products.mast_id
								INNER JOIN brands ON cust_products.brand_user_id = brands.user_id                               
                                WHERE {5} decision_final = 1 AND status1 = 1 AND (cc_response_date >= @dateFrom OR @dateFrom IS NULL) AND
									(cc_response_date <= @dateTo OR @dateTo IS NULL) {0} {3} {6} {8} GROUP BY {2} cust_products.cprod_code1 ORDER BY {2} {7} DESC {1}",
			                AnalyticsDAL.HandleClients(cmd, "client_id", incClients, exClients),
			                groupByBrands || top == null ? "" : string.Format(" LIMIT {0}", top), groupByBrands ? "brands.brand_id," : ""
                            , excludeSpares ? " AND mast_products.category1 <> 13" : "", useETA ? "om_detail1.req_eta" : "om_detail1.po_req_etd",
                            filterCprodStatus ? string.Format("cprod_status <>'D' AND ") : " ", AnalyticsDAL.GetCountryCondition(countryFilter),
                            sortBy == SortField.ReturnToSalesRatio ? "ReturnToSalesRatio" : "TotalReturnValue",
                            minUnitsShipped != null ? string.Format(
                        @" AND (SELECT COALESCE(SUM(orderqty),0) FROM om_detail1 WHERE om_detail1.cprod_code1 = cust_products.cprod_code1 {1} AND ({0} >= @datefrom OR @datefrom IS NULL) 
                                            AND ({0} <= @dateto OR @dateto IS NULL)) >= @minUnits",useETA? "om_detail1.req_eta" : "om_detail1.po_req_etd",AnalyticsDAL.GetCountryCondition(countryFilter)) : "");
			        cmd.Parameters.AddWithValue("@dateFrom", Utilities.ToDBNull(dateFrom));
			        cmd.Parameters.AddWithValue("@dateTo", Utilities.ToDBNull(dateTo));
			        if (minUnitsShipped != null)
			            cmd.Parameters.AddWithValue("@minUnits", minUnitsShipped);
			        var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        cprod_codes.Add(string.Empty + reader["cprod_code1"]);
                    }
                    reader.Close();
                    cmd.Parameters.Clear();
			    }

			    cmd.CommandText =
                    string.Format(@"SELECT returns.cprod_id, cust_products.cprod_name, cust_products.cprod_code1,cust_products.cprod_user, brands.brandname, 
                                            SUM(returns.return_qty) AS numOfReturns, SUM(returns.return_qty*returns.credit_value) AS totalReturnValue {4} FROM
												returns INNER JOIN cust_products ON returns.cprod_id = cust_products.cprod_id 
												INNER JOIN mast_products ON cust_products.cprod_mast = mast_products.mast_id
												INNER JOIN brands ON cust_products.brand_user_id = brands.user_id
												WHERE decision_final = 1 AND status1 = 1 AND (cc_response_date >= @dateFrom OR @dateFrom IS NULL) AND
									(cc_response_date <= @dateTo OR @dateTo IS NULL) {0} {3} {5} GROUP BY {2} cust_products.cprod_code1 {4} ORDER BY {2} numOfReturns DESC {1}",
                                    AnalyticsDAL.HandleClients(cmd, "client_id", incClients, exClients),groupByBrands || top == null ? "" : string.Format(" LIMIT {0}",groupByReason ? top*5 : top) ,
                                    groupByBrands ? "brands.brand_id," : "", excludeSpares ? " AND mast_products.category1 <> 13" : "",groupByReason ? ",reason" : "",
                                    groupByReason && cprod_codes.Count > 0 ? string.Format(" AND cust_products.cprod_code1 IN ({0})",Utils.CreateParametersFromIdList(cmd,cprod_codes,"cprodid")) : "" );
				cmd.Parameters.AddWithValue("@dateFrom", Utilities.ToDBNull(dateFrom));
				cmd.Parameters.AddWithValue("@dateTo", Utilities.ToDBNull(dateTo));
				var dr = cmd.ExecuteReader();
				while (dr.Read())
				{
					var data = new ReturnAggregateDataProduct();
					data.cprod_id = (int) dr["cprod_id"];
                    if (groupByReason)
                        data.Reason = string.Empty + dr["reason"];
					data.brand = string.Empty + dr["brandname"];
					data.cprod_code1 = string.Empty + dr["cprod_code1"];
                    data.cprod_user = (int)dr["cprod_user"];
					data.cprod_name = string.Empty + dr["cprod_name"];
					data.TotalAccepted = Convert.ToInt32(dr["numOfReturns"]);
				    data.TotalAcceptedValue = Utilities.FromDbValue<double>(dr["totalReturnValue"]);
					result.Add(data);
				}
				dr.Close();
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@dateFrom", Utilities.ToDBNull(dateFrom));
                cmd.Parameters.AddWithValue("@dateTo", Utilities.ToDBNull(extendToEndOfMonthForUnits ? company.Common.Utilities.GetMonthEnd(dateTo) : dateTo));
			    cmd.Parameters.AddWithValue("@cprod_code1", "");
                cmd.CommandText =string.Format(
                        @"SELECT COALESCE(SUM(orderqty),0) FROM om_detail1 WHERE om_detail1.cprod_code1 = @cprod_code1 {1} AND ({0} >= @datefrom OR @datefrom IS NULL) 
                                            AND ({0} <= @dateto OR @dateto IS NULL)",useETA? "om_detail1.req_eta" : "om_detail1.po_req_etd",AnalyticsDAL.GetCountryCondition(countryFilter));
			    foreach (var g in result.GroupBy(r=>r.cprod_code1))
			    {
			        cmd.Parameters[2].Value = g.Key;
			        var units = Convert.ToInt32(cmd.ExecuteScalar());
			        foreach (var data in g)
			        {
			            data.UnitsShipped = units;
			        }
			    }
			}
			return result;
		}

        public static List<ReturnAggregateDataPrice> GetTotalsPerClient(DateTime? dateFrom = null, DateTime? dateTo = null, int? brand_user_id = null, IList<int> incClients = null, IList<int> exClients = null, CountryFilter countryFilter = CountryFilter.UKOnly, string excludedCustomers = "NK2")
		{
			var result = new List<ReturnAggregateDataPrice>();
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
			{
				conn.Open();
				var cmd = Utils.GetCommand("", conn);
				cmd.CommandText = string.Format(@"SELECT users.user_id,users.customer_code, decision_final,claim_type, 
									SUM((CASE WHEN claim_type = 2 THEN claim_value ELSE return_qty * credit_value END)) AS Total,
									SUM(CASE ebuk WHEN 1 THEN (CASE WHEN claim_type = 2 THEN claim_value ELSE return_qty * credit_value END) ELSE 0 END) AS TotalEBUK
									FROM returns INNER JOIN users ON returns.client_id = users.user_id INNER JOIN cust_products ON returns.cprod_id = cust_products.cprod_id
									WHERE status1 = 1 AND (request_date >= @dateFrom OR @dateFrom IS NULL) AND
									(request_date <= @dateTo OR @dateTo IS NULL) AND decision_final IN (1,999,500) AND claim_type IS NOT NULL AND claim_type <> 5
									AND (cust_products.brand_user_id = @userid OR @userid IS NULL) {0} {1}
                                    AND users.customer_code NOT IN ({2}) AND COALESCE(users.test_account,999) <> 1
                                    GROUP BY users.user_id,users.customer_code, decision_final,claim_type
									ORDER BY users.user_id,users.customer_code, decision_final,claim_type",
									  AnalyticsDAL.HandleClients(cmd,"client_id",incClients,exClients),AnalyticsDAL.GetCountryCondition(countryFilter), Utils.CreateParametersFromIdList(cmd, excludedCustomers.Split(','), "cust_"));

				cmd.Parameters.AddWithValue("@dateFrom", dateFrom != null ? (object)dateFrom : DBNull.Value);
				cmd.Parameters.AddWithValue("@dateTo", dateTo != null ? (object)dateTo : DBNull.Value);
				cmd.Parameters.AddWithValue("@userid", brand_user_id != null ? (object) brand_user_id : DBNull.Value);
				var dr = cmd.ExecuteReader();
				while (dr.Read())
				{
					string code = string.Empty + dr["customer_code"];
					int claim_type = (int) dr["claim_type"];
					var data = result.FirstOrDefault(r => r.code == code && r.claim_type == claim_type);
					if (data == null)
					{
						data = new ReturnAggregateDataPrice {id=(int) dr["user_id"],code = code,claim_type = claim_type};
						result.Add(data);
					}
					int decision_final = (int) dr["decision_final"];

					double total = (double) dr["Total"];
					double totalebuk = (double) dr["TotalEBUK"];
					switch (decision_final)
					{
						case 1:
							data.TotalAccepted += total;
							data.TotalAcceptedEBUK += totalebuk;
							break;
						case 500:
							data.TotalReplacementParts += total;
							break;
						case  999:
							data.TotalRejected += total;
							break;
					}


				}
			}
			return result;
		}

        public static List<ReturnAggregateDataPrice> GetTotalsPerFactory(DateTime? dateFrom = null, DateTime? dateTo = null, int? brand_user_id = null, IList<int> incClients = null, IList<int> exClients = null, CountryFilter countryFilter = CountryFilter.UKOnly, string excludedCustomers = "NK2")
        {
            var result = new List<ReturnAggregateDataPrice>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("", conn);
                cmd.CommandText = string.Format(@"SELECT factory.user_id,factory.factory_code, decision_final,claim_type, 
									SUM((CASE WHEN claim_type = 2 THEN claim_value ELSE return_qty * credit_value END)) AS Total,
									SUM(CASE ebuk WHEN 1 THEN (CASE WHEN claim_type = 2 THEN claim_value ELSE return_qty * credit_value END) ELSE 0 END) AS TotalEBUK
									FROM returns INNER JOIN users ON returns.client_id = users.user_id INNER JOIN cust_products ON returns.cprod_id = cust_products.cprod_id
                                    INNER JOIN mast_products ON cust_products.cprod_mast = mast_products.mast_id INNER JOIN users factory ON mast_products.factory_id = factory.user_id
									WHERE status1 = 1 AND (request_date >= @dateFrom OR @dateFrom IS NULL) AND
									(request_date <= @dateTo OR @dateTo IS NULL) AND decision_final IN (1,999,500) AND claim_type IS NOT NULL AND claim_type <> 5
									AND (cust_products.brand_user_id = @userid OR @userid IS NULL) {0} {1}
                                    AND users.customer_code NOT IN ({2}) AND COALESCE(users.test_account,999) <> 1
                                    GROUP BY factory.user_id,factory.factory_code, decision_final,claim_type
									ORDER BY factory.user_id,factory.factory_code, decision_final,claim_type",
                                      AnalyticsDAL.HandleClients(cmd, "client_id", incClients, exClients), AnalyticsDAL.GetCountryCondition(countryFilter,"users."), Utils.CreateParametersFromIdList(cmd, excludedCustomers.Split(','), "cust_"));

                cmd.Parameters.AddWithValue("@dateFrom", dateFrom != null ? (object)dateFrom : DBNull.Value);
                cmd.Parameters.AddWithValue("@dateTo", dateTo != null ? (object)dateTo : DBNull.Value);
                cmd.Parameters.AddWithValue("@userid", brand_user_id != null ? (object)brand_user_id : DBNull.Value);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    string code = string.Empty + dr["factory_code"];
                    int claim_type = (int)dr["claim_type"];
                    var data = result.FirstOrDefault(r => r.code == code && r.claim_type == claim_type);
                    if (data == null)
                    {
                        data = new ReturnAggregateDataPrice { id = (int)dr["user_id"], code = code, claim_type = claim_type };
                        result.Add(data);
                    }
                    int decision_final = (int)dr["decision_final"];

                    double total = (double)dr["Total"];
                    double totalebuk = (double)dr["TotalEBUK"];
                    switch (decision_final)
                    {
                        case 1:
                            data.TotalAccepted += total;
                            data.TotalAcceptedEBUK += totalebuk;
                            break;
                        case 500:
                            data.TotalReplacementParts += total;
                            break;
                        case 999:
                            data.TotalRejected += total;
                            break;
                    }


                }
            }
            return result;
        }


        public static Returns GetFromDataReader(MySqlDataReader dr)
		{
			Returns o = new Returns();

			o.returnsid =  (int) dr["returnsid"];
			o.return_no = string.Empty + Utilities.GetReaderField(dr,"return_no");
			o.client_id = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"client_id"));
			o.request_date = Utilities.FromDbValue<DateTime>(Utilities.GetReaderField(dr,"request_date"));
			o.request_user = string.Empty + Utilities.GetReaderField(dr,"request_user");
			o.request_userid = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"request_userid"));
			o.cprod_id = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"cprod_id"));
			o.return_qty = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"return_qty"));
			o.custpo = string.Empty + Utilities.GetReaderField(dr,"custpo");
			o.custpo_certainty = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"custpo_certainty"));
			o.custpo_estimate = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"custpo_estimate"));
			o.order_id = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"order_id"));
			o.status1 = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"status1"));
			o.client_comments = string.Empty + Utilities.GetReaderField(dr,"client_comments");
			o.client_comments2 = string.Empty + Utilities.GetReaderField(dr,"client_comments2");
			o.client_comments3 = string.Empty + Utilities.GetReaderField(dr,"client_comments3");
			o.fc_comments = string.Empty + Utilities.GetReaderField(dr,"fc_comments");
			o.factory_comments = string.Empty + Utilities.GetReaderField(dr,"factory_comments");
			o.agent_comments = string.Empty + Utilities.GetReaderField(dr,"agent_comments");
			o.cc_comments = string.Empty + Utilities.GetReaderField(dr,"cc_comments");
			o.fc_response_date = Utilities.FromDbValue<DateTime>(Utilities.GetReaderField(dr,"fc_response_date"));
			o.cc_response_date = Utilities.FromDbValue<DateTime>(Utilities.GetReaderField(dr,"cc_response_date"));
			o.agent_response_date = Utilities.FromDbValue<DateTime>(Utilities.GetReaderField(dr,"agent_response_date"));
			o.closed_date = Utilities.FromDbValue<DateTime>(Utilities.GetReaderField(dr,"closed_date"));
			o.reason = string.Empty + Utilities.GetReaderField(dr,"reason");
			o.brand = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"brand"));
			o.reference = string.Empty + Utilities.GetReaderField(dr,"reference");
			o.factory_decision = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"factory_decision"));
			o.factory_decision_date = Utilities.FromDbValue<DateTime>(Utilities.GetReaderField(dr,"factory_decision_date"));
			o.decision = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"decision"));
			o.decision_final = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"decision_final"));
			o.credit_po = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"credit_po"));
			o.credit_value = Utilities.FromDbValue<double>(Utilities.GetReaderField(dr,"credit_value"));
			o.credit_value_override = Utilities.FromDbValue<double>(Utilities.GetReaderField(dr,"credit_value_override"));
			o.claim_type = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"claim_type"));
			o.claim_value = Utilities.FromDbValue<double>(Utilities.GetReaderField(dr,"claim_value"));
			o.warning_flag = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"warning_flag"));
			o.openclosed = Utilities.FromDbValue<double>(Utilities.GetReaderField(dr,"openclosed"));
			o.awaiting_user = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"awaiting_user"));
			o.flagged = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"flagged"));
			o.flagged_reason = string.Empty + Utilities.GetReaderField(dr,"flagged_reason");
			o.importance_id = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"importance"));
			o.highlight = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"highlight"));
			o.std_unique_id = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"std_unique_id"));
			o.resolution = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"resolution"));
			o.ip_address = string.Empty + Utilities.GetReaderField(dr,"ip_address");
			o.delaminating = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"delaminating"));
			o.quote1 = string.Empty + Utilities.GetReaderField(dr,"quote1");
			o.quote1_price = Utilities.FromDbValue<double>(Utilities.GetReaderField(dr,"quote1_price"));
			o.quote1a = string.Empty + Utilities.GetReaderField(dr,"quote1a");
			o.quote1a_price = Utilities.FromDbValue<double>(Utilities.GetReaderField(dr,"quote1a_price"));
			o.quote1b = string.Empty + Utilities.GetReaderField(dr,"quote1b");
			o.quote1b_price = Utilities.FromDbValue<double>(Utilities.GetReaderField(dr,"quote1b_price"));
			o.quote1c = string.Empty + Utilities.GetReaderField(dr,"quote1c");
			o.quote1c_price = Utilities.FromDbValue<double>(Utilities.GetReaderField(dr,"quote1c_price"));
			o.quote2 = string.Empty + Utilities.GetReaderField(dr,"quote2");
			o.quote2_price = Utilities.FromDbValue<double>(Utilities.GetReaderField(dr,"quote2_price"));
			o.quote3 = string.Empty + Utilities.GetReaderField(dr,"quote3");
			o.quote3_price = Utilities.FromDbValue<double>(Utilities.GetReaderField(dr,"quote3_price"));
			o.quotechoice = string.Empty + Utilities.GetReaderField(dr,"quotechoice");
			o.factory_reason = string.Empty + Utilities.GetReaderField(dr,"factory_reason");
			o.spec_code1 = string.Empty + Utilities.GetReaderField(dr,"spec_code1");
			o.spec_name = string.Empty + Utilities.GetReaderField(dr,"spec_name");
			o.fc_po_sufficient = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"fc_po_sufficient"));
			o.fc_evidence_sufficient = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"fc_evidence_sufficient"));
			o.fc_evidence_required = string.Empty + Utilities.GetReaderField(dr,"fc_evidence_required");
			o.fc_evidence_file = string.Empty + Utilities.GetReaderField(dr,"fc_evidence_file");
			o.fc_acceptance = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"fc_acceptance"));
			o.fc_product_change_description = string.Empty + Utilities.GetReaderField(dr,"fc_product_change_description");
			o.fc_cnid = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"fc_cnid"));
			o.ebuk = Utilities.FromDbValue<int>(dr["ebuk"]);
			o.feedback_category_id = Utilities.FromDbValue<int>(dr["feedback_category_id"]);
            o.rejection = string.Empty + Utilities.GetReaderField(dr, "rejection");
            o.rejection_date= Utilities.FromDbValue<DateTime>(Utilities.GetReaderField(dr, "rejection_date"));
            o.inspection_qty= Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "inspection_qty"));
            o.sample_qty = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "sample_qty"));
            o.rejection_qty = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "rejection_qty"));
            o.recheck_required = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "recheck_required"));
            o.recheck_date = Utilities.FromDbValue<DateTime>(Utilities.GetReaderField(dr, "recheck_date"));
            o.recheck_status= Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "recheck_status"));
            o.authorization_level = Utilities.FromDbValue<int>(dr["authorization_level"]);
            o.issue_type_id = Utilities.FromDbValue<int>(dr["issue_type_id"]);
            o.usergroup_id = Utilities.FromDbValue<int>(dr["usergroup_id"]);

            //if (Utilities.ColumnExists(dr,"no_of_comments"))
				//o.HasComments = Convert.ToInt64(dr["no_of_comments"]) > 0;
			if (Utilities.ColumnExists(dr, "last_commenter_id"))
				o.Last_Commenter_Id = Utilities.FromDbValue<int>(dr["last_commenter_id"]);

            if (Utilities.ColumnExists(dr, "assigned_qc"))
                o.assigned_qc = Utilities.FromDbValue<int>(dr["assigned_qc"]);
           
            return o;

		}


		public static void Create(Returns o)
		{
			string insertsql = @"INSERT INTO returns 
                                (
                                    return_no,client_id,request_date,
                                    request_user,request_userid,cprod_id,
                                    return_qty,custpo,custpo_certainty,
                                    custpo_estimate,order_id,status1,
							        client_comments,client_comments2,client_comments3,
                                    fc_comments,factory_comments,agent_comments,
                                    cc_comments,fc_response_date,cc_response_date,
                                    agent_response_date,
							        closed_date,reason,brand,reference,
                                    factory_decision,factory_decision_date,decision,
                                    decision_final,credit_po,credit_value,
                                    credit_value_override,claim_type,claim_value,
							        warning_flag,openclosed,awaiting_user,
                                    flagged,flagged_reason,importance,
                                    highlight,std_unique_id,resolution,
                                    ip_address,delaminating,quote1,quote1_price,quote1a,
							        quote1a_price,quote1b,quote1b_price,
                                    quote1c,quote1c_price,quote2,
                                    quote2_price,quote3,quote3_price,
                                    quotechoice,factory_reason,spec_code1,
                                    spec_name,fc_po_sufficient,
							        fc_evidence_sufficient,fc_evidence_required,
                                    fc_evidence_file,fc_acceptance,fc_product_change_description,
                                    fc_cnid,feedback_category_id,rejection,issue_type_id,authorization_level,usergroup_id) 
							    VALUES
                                (
                                    @return_no,@client_id,@request_date,
                                    @request_user,@request_userid,@cprod_id,
                                    @return_qty,@custpo,@custpo_certainty,
                                    @custpo_estimate,@order_id,@status1,
							        @client_comments,@client_comments2,@client_comments3,
                                    @fc_comments,@factory_comments,@agent_comments,@cc_comments,@fc_response_date,@cc_response_date,
							        @agent_response_date,@closed_date,@reason,@brand,
                                    @reference,@factory_decision,@factory_decision_date,
                                    @decision,@decision_final,@credit_po,@credit_value,
							        @credit_value_override,@claim_type,@claim_value,
                                    @warning_flag,@openclosed,@awaiting_user,
                                    @flagged,@flagged_reason,@importance,@highlight,@std_unique_id,
							        @resolution,@ip_address,@delaminating,@quote1,
                                    @quote1_price,@quote1a,@quote1a_price,@quote1b,
                                    @quote1b_price,@quote1c,@quote1c_price,@quote2,@quote2_price,
							        @quote3,@quote3_price,@quotechoice,@factory_reason,
                                    @spec_code1,@spec_name,@fc_po_sufficient,@fc_evidence_sufficient,
                                    @fc_evidence_required,@fc_evidence_file,
							        @fc_acceptance,@fc_product_change_description,@fc_cnid,@feedback_category_id,@rejection,@issue_type_id,@authorization_level,@usergroup_id
                                )";

			string checkSQL = "SELECT * FROM returns WHERE return_no = @return_no";

			var conn = new MySqlConnection(Properties.Settings.Default.ConnString);
			conn.Open();
			MySqlTransaction tr = conn.BeginTransaction();
			try
			{

				MySqlCommand cmd = Utils.GetCommand(checkSQL, conn, tr);
				cmd.Parameters.AddWithValue("@return_no", o.return_no);
				var returns = new List<Returns>();
				var dr = cmd.ExecuteReader();
				while (dr.Read())
				{
					returns.Add(GetFromDataReader(dr));
				}
				dr.Close();
				if (returns.Count > 0)
				{
					//duplicate found
					var highestLetterRec =
						returns.OrderByDescending(r => r.return_no.Substring(r.return_no.LastIndexOf('-')))
							   .Take(1)
							   .First();
					var retNoParts = highestLetterRec.return_no.Split('-');
					if (retNoParts.Length == 5)
						o.return_no = string.Format("{0}-{1}-{2}-{3}-{4}", retNoParts[0], retNoParts[1], retNoParts[2],
													retNoParts[3],
													company.Common.Utilities.OrdinalToLetters(
														company.Common.Utilities.LettersToOrdinal(retNoParts[4]) + 1));
				}

				cmd.CommandText = insertsql;
				cmd.Parameters.Clear();
				BuildSqlParameters(cmd,o);
				cmd.ExecuteNonQuery();
				cmd.CommandText = "SELECT returnsid FROM returns WHERE returnsid = LAST_INSERT_ID()";
				o.returnsid = (int) cmd.ExecuteScalar();

				if (o.Images != null)
				{
					foreach (var image in o.Images)
					{
						image.return_id = o.returnsid;
						Returns_imagesDAL.Create(image,tr);
					}
				}
				if (o.Subscriptions != null && o.Subscriptions.Count > 0)
				{
					foreach (var s in o.Subscriptions)
					{
						s.subs_returnid = o.returnsid;
						Feedback_subscriptionsDAL.Create(s,tr);
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
				conn = null;
			}
		}

		private static void BuildSqlParameters(MySqlCommand cmd, Returns o, bool forInsert = true)
		{

			if(!forInsert)
				cmd.Parameters.AddWithValue("@returnsid", o.returnsid);
			cmd.Parameters.AddWithValue("@return_no", o.return_no);
			cmd.Parameters.AddWithValue("@client_id", o.client_id);
			cmd.Parameters.AddWithValue("@request_date", o.request_date);
			cmd.Parameters.AddWithValue("@request_user", o.request_user);
			cmd.Parameters.AddWithValue("@request_userid", o.request_userid);
			cmd.Parameters.AddWithValue("@cprod_id", o.cprod_id);
			cmd.Parameters.AddWithValue("@return_qty", o.return_qty);
			cmd.Parameters.AddWithValue("@custpo", o.custpo);
			cmd.Parameters.AddWithValue("@custpo_certainty", o.custpo_certainty);
			cmd.Parameters.AddWithValue("@custpo_estimate", o.custpo_estimate);
			cmd.Parameters.AddWithValue("@order_id", o.order_id);
			cmd.Parameters.AddWithValue("@status1", o.status1);
			cmd.Parameters.AddWithValue("@client_comments", o.client_comments);
			cmd.Parameters.AddWithValue("@client_comments2", o.client_comments2);
			cmd.Parameters.AddWithValue("@client_comments3", o.client_comments3);
			cmd.Parameters.AddWithValue("@fc_comments", o.fc_comments);
			cmd.Parameters.AddWithValue("@factory_comments", o.factory_comments);
			cmd.Parameters.AddWithValue("@agent_comments", o.agent_comments);
			cmd.Parameters.AddWithValue("@cc_comments", o.cc_comments);
			cmd.Parameters.AddWithValue("@fc_response_date", o.fc_response_date);
			cmd.Parameters.AddWithValue("@cc_response_date", o.cc_response_date);
			cmd.Parameters.AddWithValue("@agent_response_date", o.agent_response_date);
			cmd.Parameters.AddWithValue("@closed_date", o.closed_date);
			cmd.Parameters.AddWithValue("@reason", o.reason);
			cmd.Parameters.AddWithValue("@brand", o.brand);
			cmd.Parameters.AddWithValue("@reference", o.reference);
			cmd.Parameters.AddWithValue("@factory_decision", o.factory_decision);
			cmd.Parameters.AddWithValue("@factory_decision_date", o.factory_decision_date);
			cmd.Parameters.AddWithValue("@decision", o.decision);
			cmd.Parameters.AddWithValue("@decision_final", o.decision_final);
			cmd.Parameters.AddWithValue("@credit_po", o.credit_po);
			cmd.Parameters.AddWithValue("@credit_value", o.credit_value);
			cmd.Parameters.AddWithValue("@credit_value_override", o.credit_value_override);
			cmd.Parameters.AddWithValue("@claim_type", o.claim_type);
			cmd.Parameters.AddWithValue("@claim_value", o.claim_value);
			cmd.Parameters.AddWithValue("@warning_flag", o.warning_flag);
			cmd.Parameters.AddWithValue("@openclosed", o.openclosed);
			cmd.Parameters.AddWithValue("@awaiting_user", o.awaiting_user);
			cmd.Parameters.AddWithValue("@flagged", o.flagged);
			cmd.Parameters.AddWithValue("@flagged_reason", o.flagged_reason);
			cmd.Parameters.AddWithValue("@importance", o.importance_id);
			cmd.Parameters.AddWithValue("@highlight", o.highlight);
			cmd.Parameters.AddWithValue("@std_unique_id", o.std_unique_id);
			cmd.Parameters.AddWithValue("@resolution", o.resolution);
			cmd.Parameters.AddWithValue("@ip_address", o.ip_address);
			cmd.Parameters.AddWithValue("@delaminating", o.delaminating);
			cmd.Parameters.AddWithValue("@quote1", o.quote1);
			cmd.Parameters.AddWithValue("@quote1_price", o.quote1_price);
			cmd.Parameters.AddWithValue("@quote1a", o.quote1a);
			cmd.Parameters.AddWithValue("@quote1a_price", o.quote1a_price);
			cmd.Parameters.AddWithValue("@quote1b", o.quote1b);
			cmd.Parameters.AddWithValue("@quote1b_price", o.quote1b_price);
			cmd.Parameters.AddWithValue("@quote1c", o.quote1c);
			cmd.Parameters.AddWithValue("@quote1c_price", o.quote1c_price);
			cmd.Parameters.AddWithValue("@quote2", o.quote2);
			cmd.Parameters.AddWithValue("@quote2_price", o.quote2_price);
			cmd.Parameters.AddWithValue("@quote3", o.quote3);
			cmd.Parameters.AddWithValue("@quote3_price", o.quote3_price);
			cmd.Parameters.AddWithValue("@quotechoice", o.quotechoice);
			cmd.Parameters.AddWithValue("@factory_reason", o.factory_reason);
			cmd.Parameters.AddWithValue("@spec_code1", o.spec_code1);
			cmd.Parameters.AddWithValue("@spec_name", o.spec_name);
			cmd.Parameters.AddWithValue("@fc_po_sufficient", o.fc_po_sufficient);
			cmd.Parameters.AddWithValue("@fc_evidence_sufficient", o.fc_evidence_sufficient);
			cmd.Parameters.AddWithValue("@fc_evidence_required", o.fc_evidence_required);
			cmd.Parameters.AddWithValue("@fc_evidence_file", o.fc_evidence_file);
			cmd.Parameters.AddWithValue("@fc_acceptance", o.fc_acceptance);
			cmd.Parameters.AddWithValue("@fc_product_change_description", o.fc_product_change_description);
			cmd.Parameters.AddWithValue("@fc_cnid", o.fc_cnid);
			cmd.Parameters.AddWithValue("@feedback_category_id", o.feedback_category_id);
            cmd.Parameters.AddWithValue("@rejection", o.rejection);
            cmd.Parameters.AddWithValue("@inspection_qty", o.inspection_qty);
            cmd.Parameters.AddWithValue("@sample_qty", o.sample_qty);
            cmd.Parameters.AddWithValue("@rejection_qty", o.rejection_qty);
            cmd.Parameters.AddWithValue("@recheck_required", o.recheck_required);
            cmd.Parameters.AddWithValue("@recheck_date", o.recheck_date);
            cmd.Parameters.AddWithValue("@rejection_date", o.rejection_date);
            cmd.Parameters.AddWithValue("@recheck_status", o.recheck_status);
            cmd.Parameters.AddWithValue("@assigned_qc", o.assigned_qc);
            
            cmd.Parameters.AddWithValue("@issue_type_id", o.issue_type_id);
            cmd.Parameters.AddWithValue("@authorization_level", o.authorization_level);
            cmd.Parameters.AddWithValue("@usergroup_id", o.usergroup_id);
        }

        public static void UpdateRejection(Returns o)
        {

            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                string updatesql = @"UPDATE 
                                        returns 
                                     SET
                                        recheck_date=@recheck_date,
                                        rejection_date=@rejection_date,
                                        recheck_status=@recheck_status,
                                        recheck_required=@recheck_required ,
                                        rejection_qty=@rejection_qty, 
                                        inspection_qty=@inspection_qty,  
                                        sample_qty=@sample_qty,
                                        client_comments2= @client_comments2,
                                        client_comments3 = @client_comments3,
                                        assigned_qc = @assigned_qc
                                    WHERE 
                                        returnsid = @returnsid";

                conn.Open();
                var cmd = Utils.GetCommand(updatesql, conn);
                BuildSqlParameters(cmd, o, false);
                cmd.ExecuteNonQuery();
            }
        }
        public static void UpdateSimpleRecheck(Recheck o)
        {

            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                string updatesql = @"UPDATE 
                                        returns 
                                     SET
                                        recheck_date=@recheck_date,                                       
                                        client_comments2= @client_comments2,
                                       recheck_status=@recheck_status
                                    WHERE 
                                        returnsid = @returnsid";
                //                recheck_status=@recheck_status,
                //                recheck_required = @recheck_required ,
                //                rejection_qty = @rejection_qty, 
                //                inspection_qty=@inspection_qty,  
                //                

                
                conn.Open();
                


                //BuildSqlParameters(cmd, o, false);
                //cmd.ExecuteNonQuery();
                MySqlTransaction tr = conn.BeginTransaction();
                try
                {
                    var cmd = Utils.GetCommand(updatesql, conn);
                    cmd.Parameters.AddWithValue("@returnsid", o.returnsid);
                    cmd.Parameters.AddWithValue("@client_comments2", o.client_comments2);
                    cmd.Parameters.AddWithValue("@recheck_status", o.recheck_status);
                    cmd.Parameters.AddWithValue("@recheck_date", o.recheck_date);                    
                    
                    cmd.ExecuteNonQuery();

                    foreach (var image in o.Images)
                    {
                        Returns_imagesDAL.Create(image, tr);

                    }

                    tr.Commit();
                }
                catch (Exception)
                {
                    tr.Rollback();
                    throw;
                }
                //finally
                //{
                //    conn = null;
                //}

            }
        }
        
        public static void Update(Returns o, List<int> deletedImagesIds = null)
		{
			string updatesql = @"UPDATE returns 
                               SET 
                                        return_no = @return_no,
                                        client_id = @client_id,
                                        request_date = @request_date,
                                        request_user = @request_user,
                                        request_userid = @request_userid,
								        cprod_id = @cprod_id,
                                        return_qty = @return_qty,
                                        custpo = @custpo,
                                        custpo_certainty = @custpo_certainty,
                                        custpo_estimate = @custpo_estimate,
                                        order_id = @order_id,
								        status1 = @status1,
                                        client_comments = @client_comments,
                                        client_comments2 = @client_comments2,
                                        client_comments3 = @client_comments3,
                                        fc_comments = @fc_comments,
								        factory_comments = @factory_comments,
                                        agent_comments = @agent_comments,
                                        cc_comments = @cc_comments,
                                        fc_response_date = @fc_response_date,
								        cc_response_date = @cc_response_date,
                                        agent_response_date = @agent_response_date,
                                        closed_date = @closed_date,
                                        reason = @reason,
                                        brand = @brand,
                                        reference = @reference,
								        factory_decision = @factory_decision,
                                        factory_decision_date = @factory_decision_date,
                                        decision = @decision,
                                        decision_final = @decision_final,
                                        credit_po = @credit_po,
								        credit_value = @credit_value,
                                        credit_value_override = @credit_value_override,
                                        claim_type = @claim_type,
                                        claim_value = @claim_value,
                                        warning_flag = @warning_flag,
								        openclosed = @openclosed,
                                        awaiting_user = @awaiting_user,
                                        flagged = @flagged,
                                        flagged_reason = @flagged_reason,
                                        importance = @importance,
                                        highlight = @highlight,
								        std_unique_id = @std_unique_id,
                                        resolution = @resolution,
                                        ip_address = @ip_address,
                                        delaminating = @delaminating,
                                        quote1 = @quote1,
                                        quote1_price = @quote1_price,
								        quote1a = @quote1a,
                                        quote1a_price = @quote1a_price,
                                        quote1b = @quote1b,
                                        quote1b_price = @quote1b_price,
                                        quote1c = @quote1c,
                                        quote1c_price = @quote1c_price,
								        quote2 = @quote2,
                                        quote2_price = @quote2_price,
                                        quote3 = @quote3,
                                        quote3_price = @quote3_price,
                                        quotechoice = @quotechoice,
                                        factory_reason = @factory_reason,
								        spec_code1 = @spec_code1,
                                        spec_name = @spec_name,
                                        fc_po_sufficient = @fc_po_sufficient,
                                        fc_evidence_sufficient = @fc_evidence_sufficient,
								        fc_evidence_required = @fc_evidence_required,
                                        fc_evidence_file = @fc_evidence_file,
                                        fc_acceptance = @fc_acceptance,
								        fc_product_change_description = @fc_product_change_description,
                                        fc_cnid = @fc_cnid,
                                        feedback_category_id = @feedback_category_id,
                                        recheck_date=@recheck_date,
                                        rejection_date=@rejection_date,
                                        recheck_status=@recheck_status,
                                        inspection_qty=@inspection_qty,  
                                        sample_qty=@sample_qty, 
                                        rejection_qty=@rejection_qty, 
                                        recheck_required=@recheck_required,
                                        assigned_qc = @assigned_qc
                                  WHERE 
                                        returnsid = @returnsid";

			var conn = new MySqlConnection(Properties.Settings.Default.ConnString);
			conn.Open();
			MySqlTransaction tr = conn.BeginTransaction();
			try
			{
				MySqlCommand cmd = Utils.GetCommand(updatesql, conn,tr);
				BuildSqlParameters(cmd,o, false);
				cmd.ExecuteNonQuery();

				if (o.Images != null)
				{
					foreach (var image in o.Images)
					{
						if (image.image_unique <= 0)
						{
							image.return_id= o.returnsid;
							Returns_imagesDAL.Create(image, tr);
						}
						else
						{
							Returns_imagesDAL.Update(image, tr);
						}
					}
				}
				if (deletedImagesIds != null)
				{
					foreach (var di in deletedImagesIds)
					{
						Returns_imagesDAL.Delete(di, tr);
					}
				}
				tr.Commit();
			}
			catch
			{
				tr.Rollback();
				throw;
			}
			finally
			{
				conn = null;
			}
		}

		public static void Delete(int returnsid)
		{
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
			{
				conn.Open();
				var cmd = Utils.GetCommand("DELETE FROM returns WHERE returnsid = @id" , conn);
				cmd.Parameters.AddWithValue("@id", returnsid);
				cmd.ExecuteNonQuery();
			}
		}

		public static List<ClaimsAnalyticsRow> GetAnalytics(List<int> ids, DateTime? from = null, DateTime? to = null)
		{
			var result = new List<ClaimsAnalyticsRow>();
			if (ids.Count > 0)
			{
				using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
				{
					conn.Open();
					var cmd = Utils.GetCommand("", conn);
					cmd.CommandText =
						string.Format(
                            @"SELECT cprod_id, cprod_code1,cprod_name, COALESCE((SELECT SUM(orderqty) FROM om_detail1 WHERE om_detail1.cprod_id = cust_products.cprod_id AND (om_detail1.po_req_etd >= @from OR @from IS NULL) AND (om_detail1.po_req_etd <= @to OR @to IS NULL)),0) AS orderqty, 
									COALESCE((SELECT SUM(return_qty) FROM returns WHERE returns.cprod_id = cust_products.cprod_id AND decision_final = 1 AND (returns.request_date >= @from OR @from IS NULL) AND (returns.request_date <= @to OR @to IS NULL)),0) AS claims 
                                FROM cust_products WHERE cprod_id IN ({0}) ",
							Utils.CreateParametersFromIdList(cmd, ids));
				    cmd.Parameters.AddWithValue("@from", Utilities.ToDBNull(from));
				    cmd.Parameters.AddWithValue("@to", Utilities.ToDBNull(to));
					var dr = cmd.ExecuteReader();
					while (dr.Read())
					{
						result.Add(new ClaimsAnalyticsRow
							{
								cprod_id = (int) dr["cprod_id"],
								cprod_code1 = string.Empty + dr["cprod_code1"],
								cprod_name = string.Empty + dr["cprod_name"],
								orderqty = Convert.ToInt32(dr["orderqty"]),
								claims = Convert.ToInt32(dr["claims"]),
                                From = from,
                                To = to
							});
					}
					dr.Close();
				}
			}
			return result;
		}

        public static List<ClaimsAnalyticsRow> GetAnalytics(string cprod_code1, DateTime? from = null, DateTime? to = null)
        {
            var result = new List<ClaimsAnalyticsRow>();
            if (!string.IsNullOrEmpty(cprod_code1))
            {
                using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
                {
                    conn.Open();
                    var cmd = Utils.GetCommand("", conn);
                    cmd.CommandText =

                            @"SELECT cprod_id, cprod_code1,cprod_name, COALESCE((SELECT SUM(orderqty) FROM om_detail1 WHERE om_detail1.cprod_id = cust_products.cprod_id AND (om_detail1.po_req_etd >= @from OR @from IS NULL) AND (om_detail1.po_req_etd <= @to OR @to IS NULL)),0) AS orderqty, 
									COALESCE((SELECT SUM(return_qty) FROM returns WHERE returns.cprod_id = cust_products.cprod_id AND decision_final = 1 AND (returns.request_date >= @from OR @from IS NULL) AND (returns.request_date <= @to OR @to IS NULL)),0) AS claims 
                                FROM cust_products WHERE cprod_code1 =@code ";
                    cmd.Parameters.AddWithValue("@code", cprod_code1);
                    cmd.Parameters.AddWithValue("@from", Utilities.ToDBNull(from));
                    cmd.Parameters.AddWithValue("@to", Utilities.ToDBNull(to));
                    var dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        result.Add(new ClaimsAnalyticsRow
                        {
                            cprod_id = (int)dr["cprod_id"],
                            cprod_code1 = string.Empty + dr["cprod_code1"],
                            cprod_name = string.Empty + dr["cprod_name"],
                            orderqty = Convert.ToInt32(dr["orderqty"]),
                            claims = Convert.ToInt32(dr["claims"]),
                            From = from,
                            To = to
                        });
                    }
                    dr.Close();
                }
            }
            return result;
        }

		public static void UpdateOpenClose(int feedback_id,int openClose)
		{
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
			{
				conn.Open();
				var cmd = Utils.GetCommand("UPDATE returns SET openclosed = @openclose WHERE returnsid=@feedback_id",
										   conn);
				cmd.Parameters.AddWithValue("@feedback_id", feedback_id);
				cmd.Parameters.AddWithValue("@openClose", openClose);
				cmd.ExecuteNonQuery();
			}
		}

		public static void UpdateStatus(int id, FeedbackStatus status, int? authorizationLevel = null)
		{
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
			{
				conn.Open();
				var cmd = Utils.GetCommand($"UPDATE returns SET status1 = @status {(authorizationLevel != null ? ",authorization_level =  @level" : "")} WHERE returnsid=@feedback_id",
										   conn);
				cmd.Parameters.AddWithValue("@feedback_id", id);
                cmd.Parameters.AddWithValue("@level", Utilities.ToDBNull(authorizationLevel));
                cmd.Parameters.AddWithValue("@status", status);
				cmd.ExecuteNonQuery();
			}
		}


        public static List<Returns> GetForMastProducts(IList<int> mast_ids, bool commentsOnly = false, DateTime? fromDate = null)
        {
            var result = new List<Returns>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString)) {
                conn.Open();
                var cmd = Utils.GetCommand("", conn);
                cmd.CommandText = $@"SELECT returns.*,cust_products.* FROM returns INNER JOIN cust_products ON returns.cprod_id = cust_products.cprod_id
                                     WHERE cust_products.cprod_mast IN ({Utils.CreateParametersFromIdList(cmd, mast_ids)}) 
                                     {(commentsOnly ? " AND returns.factory_comments IS NOT NULL" : "")} {(fromDate != null ? " AND returns.request_date >= @from" : "")}";
                if (fromDate != null)
                    cmd.Parameters.AddWithValue("@from", fromDate);
                var dr = cmd.ExecuteReader();
                while(dr.Read()) {
                    var r = GetFromDataReader(dr);
                    r.Product = Cust_productsDAL.GetFromDataReader(dr);
                    result.Add(r);
                }
                dr.Close();
            }
            return result;
        }

        public static string GetCustomerIdInWhereClause(string customerIds, string inOut)
        {
            var whereClause = string.Empty;

	        if (!string.IsNullOrEmpty(customerIds))
	        {
                var sb = new StringBuilder();

	            sb.Append("AND users_customers.user_id ");
	            sb.Append(inOut);
	            sb.Append(" (");
	            sb.Append(customerIds);
	            sb.Append(")");

	            whereClause = sb.ToString();
	        }

            return whereClause;
        }
	

        public static List<CAReportCAItem> GetCAReportCAItems(DateTime dateForCA,string customerIdsIn,string customerIdsOut)
        {
            List<CAReportCAItem> items = new List<CAReportCAItem>();

            var dateForCAWeek = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(dateForCA, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
            var dateForCAYear = CultureInfo.CurrentCulture.Calendar.GetYear(dateForCA);


            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();

                var cmd = Utils.GetCommand("", conn);

                cmd.CommandText = $@"
                                SELECT 
	                             r.RETURN_NO AS REF
                                ,r.request_date AS DATA_CREATED
                                ,r.custpo AS PO
	                            ,r.spec_code1 AS PRODUCTS
	                            ,users.factory_code AS FACTORY
	                            ,r.client_comments AS DESCRIPTION
	                            ,r.cprod_id
                                ,users_customers.customer_code as CUSTOMER_CODE
                            FROM `returns` r
                            LEFT JOIN returns_cust_products RCP ON r.returnsid = RCP.returns_id
	                            LEFT JOIN cust_products cp_returns ON RCP.cprod_id = cp_returns.cprod_id
	                            LEFT JOIN cust_products cp ON r.cprod_id = cp.cprod_id
		                            LEFT JOIN  mast_products mp ON mp.mast_id = cp.cprod_mast
			                            LEFT JOIN users ON users.user_id = mp.factory_id
                                            LEFT JOIN users users_customers on cp.cprod_user = users_customers.user_id
                            WHERE claim_type = 7 AND r.spec_code1 IS NOT NULL AND (YEAR(r.request_date) = @dateForCAYear AND WEEK(r.request_date) = @dateForCAWeek
                            {GetCustomerIdInWhereClause(customerIdsIn,"IN")}
                            {GetCustomerIdInWhereClause(customerIdsOut, "NOT IN")})
                            UNION
                            SELECT 
	                             r.RETURN_NO AS REF
                                ,r.request_date AS DATA_CREATED
                                ,r.custpo AS PO
                                ,cp.cprod_code1 as PRODUCTS
	                            ,users.factory_code AS FACTORY
	                            ,r.client_comments AS DESCRIPTION
	                            ,rcp.cprod_id
                                ,users_customers.customer_code as CUSTOMER_CODE
                            FROM `returns` r
                            LEFT JOIN returns_cust_products RCP ON r.returnsid = RCP.returns_id
	                            LEFT JOIN cust_products cp ON RCP.cprod_id = cp.cprod_id
		                            LEFT JOIN  mast_products mp ON mp.mast_id = cp.cprod_mast
			                            LEFT JOIN users ON users.user_id = mp.factory_id
                                            LEFT JOIN users users_customers on cp.cprod_user = users_customers.user_id
                            WHERE claim_type = 7 AND cp.cprod_code1 IS NOT NULL AND (YEAR(r.request_date) = @dateForCAYear AND WEEK(r.request_date) = @dateForCAWeek
                                 {GetCustomerIdInWhereClause(customerIdsIn,"IN")}
                                 {GetCustomerIdInWhereClause(customerIdsOut,"NOT IN")})";

                cmd.Parameters.AddWithValue("@dateForCAWeek", dateForCAWeek);
                cmd.Parameters.AddWithValue("@dateForCAYear", dateForCAYear);

                var dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    items.Add(new CAReportCAItem
                    {
                        Reference = string.Empty + dr["REF"],
                        DataCreated = Utilities.FromDbValue<DateTime>(Utilities.GetReaderField(dr, "DATA_CREATED")),
                        PO = string.Empty + dr["PO"],
                        Products = string.Empty + dr["PRODUCTS"],
                        Factory = string.Empty + dr["FACTORY"],
                        Description = dr["DESCRIPTION"].ToString(),
                        CustomerCode = dr["CUSTOMER_CODE"].ToString()
                    });
                }

                dr.Close();
            }

            return items;
        }

        public static CAReportInspectionItem GetCAReportInspectionItem(DateTime dateForCAInspectionItem, string customerIdsIn, string customersIdOut)
        {
            CAReportInspectionItem item = new CAReportInspectionItem();

            //current week - 1!!!
            var dateForCAInspectionItemWeek = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(dateForCAInspectionItem, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
            var dateForCAInspectionItemYear = CultureInfo.CurrentCulture.Calendar.GetYear(dateForCAInspectionItem);

            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();

                var cmd = Utils.GetCommand("", conn);

                var joinClause =
                    " LEFT OUTER JOIN users users_customers ON insp.customer_code = users_customers.customer_code";

                var isCustomerCondition = 
                    string.IsNullOrEmpty(customerIdsIn);

                if(isCustomerCondition)
                    isCustomerCondition = string.IsNullOrEmpty(customersIdOut);

                cmd.CommandText = $@"
                                    SELECT
                                        COALESCE(SUM(insp.insp_type = 'LO'),0) AS LO,
                                        COALESCE(SUM(insp.insp_type = 'FI'),0) AS FI
                                    FROM inspections insp
                                        {(isCustomerCondition ? "" : joinClause)}
                                    WHERE insp.insp_type IN('FI', 'LO')
                                    AND WEEK(insp.insp_start) = @dateForCaInspectionItemWeek AND YEAR(insp.insp_start) = @dateForCaInspectionItemYear
                                    { GetCustomerIdInWhereClause(customerIdsIn,"IN")}
                                    { GetCustomerIdInWhereClause(customersIdOut, "NOT IN")}";

                cmd.Parameters.AddWithValue("@dateForCaInspectionItemWeek", dateForCAInspectionItemWeek);
                cmd.Parameters.AddWithValue("@dateForCaInspectionItemYear", dateForCAInspectionItemYear);
                var dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    item.FICount = Convert.ToInt32(dr["FI"]);
                    item.LICount = Convert.ToInt32(dr["LO"]);
                }

                dr.Close();

            }

            return item;
        }

    }
}

