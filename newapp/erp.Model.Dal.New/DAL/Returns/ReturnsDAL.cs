
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;
using Dapper;
using System.Data;
using Dapper.FluentMap;
using Dapper.FluentMap.Mapping;

namespace erp.Model.Dal.New
{
	public class ReturnsDAL : GenericDal<Returns>, IReturnsDAL
	{
		private ICountriesDAL countriesDal;
		private IReturnsImagesDAL returnsImagesDal;
		private IFeedbackSubscriptionsDAL feedbackSubscriptionsDal;
		private static object mapper = 1;

		public ReturnsDAL(IDbConnection conn, ICountriesDAL countriesDal, IReturnsImagesDAL returnsImagesDal, 
			IFeedbackSubscriptionsDAL feedbackSubscriptionsDal) : base(conn)
		{
			this.feedbackSubscriptionsDal = feedbackSubscriptionsDal;
			this.returnsImagesDal = returnsImagesDal;
			this.countriesDal = countriesDal;
			lock(mapper)
			{
				try
				{
					if (!FluentMapper.EntityMaps.ContainsKey(typeof(Returns)))
						FluentMapper.Initialize(config => config.AddMap(new ReturnsMap()));
				}
				catch (InvalidOperationException)
				{
					//FluentMapper can raise this
				}
			}
		}

		        
        public List<Returns> GetAllForProduct(int cprod_id)
        {
			return conn.Query<Returns>("SELECT * FROM returns WHERE returns.cprod_id=@cprod_id", new {cprod_id}).ToList();            
        }

        public List<Returns> GetByClient(int client_id)
        {
			return conn.Query<Returns>("SELECT * FROM returns WHERE client_id=@client_id  AND status1 = 1 ORDER BY request_date DESC",
				new {client_id}).ToList();            
        }

		public List<Returns> GetInPeriod(DateTime? from = null, DateTime? to = null)
		{
			return conn.Query<Cust_products, Returns,Returns>(
				@"SELECT cust_products.*,returns.* FROM returns INNER JOIN cust_products ON returns.cprod_id = cust_products.cprod_id 
				WHERE claim_type <> 5 AND decision_final = 1 
				AND  (cc_response_date >= @from OR @from IS NULL) AND (cc_response_date <= @to OR @to IS NULL)",
				(cp, r) =>
				{
					r.Product = cp;
					return r;
				}, new {from, to}, splitOn: "returnsid" ).ToList();			
		}

		public List<Returns> GetForClaimType(int claim_type, bool products=false, IList<int?> groupsId = null)
		{
			return conn.Query<Returns, User, Returns_importance, Feedback_category, feedback_issue_type, Returns>(
				$@"SELECT returns.*,(SELECT COUNT(*) FROM returns_comments WHERE return_id = returns.returnsid AND comments NOT LIKE 'Authorization%') AS HasComments,
                    (SELECT comments_from FROM returns_comments WHERE return_id = returns.returnsid AND comments NOT LIKE 'Authorization%' ORDER BY comments_date DESC LIMIT 1) AS Last_Commenter_Id,
					creator.*, importance.*, category.*, issuetype.*
					FROM returns LEFT OUTER JOIN userusers creator ON returns.request_userid = creator.useruserid
                            LEFT OUTER JOIN returns_importance importance on returns.importance = importance.importance_id
                                LEFT OUTER JOIN feedback_category category ON returns.feedback_category_id = feedback_cat_id
                                    LEFT OUTER JOIN feedback_issue_type issuetype ON returns.issue_type_id = issuetype.id
					WHERE claim_type = @claim_type {(groupsId != null && groupsId.Count > 0 ? $" AND usergroup_id IN @groupsId" : "")}",
				(r,u,i,f,t) =>
				{
					r.Creator = u;
                    r.Importance = i;
                    r.Category = f;
                    r.IssueType = t;
					return r;
				}, new {claim_type, groupsId}, splitOn: "useruserid, importance_id, feedback_cat_id, id").ToList();

		}

        

        public List<Returns> Search(int? claim_type, string text)
		{
			return conn.Query<Returns>(
				@"SELECT returns.*,(SELECT COUNT(*) FROM returns_comments WHERE return_id = returns.returnsid) AS no_of_comments, 
				(SELECT comments_from FROM returns_comments WHERE return_id = returns.returnsid ORDER BY comments_date DESC LIMIT 1) AS last_commenter_id 
				FROM returns WHERE claim_type = @claim_type 
				AND (client_comments LIKE @text OR return_no LIKE @text 
					OR EXISTS (SELECT comments_id FROM returns_comments WHERE return_id = returns.returnsid AND comments LIKE @text ) )",
				new {claim_type, text = "%"+text+"%"}
				).ToList();						
		}

	
		public override Returns GetById(object id)
		{
            var columnMap = new Dictionary<string, string>();
            columnMap.Add("importance", "importance_id");
            
            SqlMapper.SetTypeMap(typeof(Returns), new CustomPropertyTypeMap(typeof(Returns), 
                (type, columnName) => type.GetProperty(columnMap.ContainsKey(columnName) ? columnMap[columnName] : columnName)));

            Returns result = null;
			
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
    
            }
			return result;
			
		}
		/*-*/
		public List<ReturnAggregateDataByMonth> GetQtyForPeriodGroupedByMonths(int cprod_user, IList<string> cprodCode)
		{
			return conn.Query<ReturnAggregateDataByMonth>(
				@"SELECT
				Count(returns.returnsid) AS CountReturns,
				SUM(returns.return_qty)AS SumReturnsProduct,
				CAST(DATE_FORMAT(returns.cc_response_date,""%y%m"")AS UNSIGNED) AS created_month,
				cust_products.cprod_code1
				FROM
				returns
				INNER JOIN cust_products ON returns.cprod_id = cust_products.cprod_id
				WHERE
				cust_products.cprod_user = @cprod_user AND
				returns.decision_final  = 1 AND
				cust_products.cprod_code1 IN ({0})
				GROUP BY created_month",
				new {cprod_user}
			).ToList();

			
		}

		public int GetNoOfReturns(int company_id, DateTime? date = null,int? claim_type = null)
		{
			return conn.ExecuteScalar<int>(
				@"SELECT COUNT(*) FROM returns WHERE client_id = @company_id 
				 AND (request_date BETWEEN @date AND (@date + INTERVAL 1 day - INTERVAL 1 second) OR @date IS NULL) 
				 AND (claim_type = @claim_type OR @claim_type IS NULL)",
				new {claim_type, company_id, date});
		}

        public int GetNextITFeedbackNum()
        {
            return GetNextFeedbackNum(Returns.ClaimType_ITFeedback);
        }

        public int GetNextFeedbackNum(int type)
        {
	        var typename =
		        conn.ExecuteScalar<string>("SELECT typename FROM feedback_type WHERE type_id = @type", new {type});
	        return (conn.ExecuteScalar<int?>(
		        $"SELECT MAX(CAST(REPLACE(return_no,'{typename}-','') AS UNSIGNED)) FROM returns WHERE claim_type=@type",
		        new {type}
	        ) ?? 0) + 1;
        }

        public ReturnAggregateData GetQtyInPeriod(string cprod_code1, int company_id, DateTime? dateFrom = null, DateTime? dateTo = null)
        {
	        return conn.QueryFirstOrDefault<ReturnAggregateData>(
		        @"SELECT COUNT(*) AS TotalQty, 
				COUNT(CASE decision_final WHEN 1 THEN 1 ELSE NULL END) AS TotalAccepted,
				COUNT(CASE decision_final WHEN 999 THEN 1 ELSE NULL END) AS TotalRejected
				FROM returns INNER JOIN cust_products ON  returns.cprod_id = cust_products.cprod_id
				WHERE cprod_code1 = @cprod_code1 AND client_id = @company_id and status1 = 1 AND (request_date >= @dateFrom OR @dateFrom IS NULL) AND
				(request_date <= @dateTo OR @dateTo IS NULL) ",
		        new {company_id, cprod_code1, dateFrom, dateTo}
	        );

		}

		public List<ReturnAggregateDataProduct> GetTopNTotalsPerProduct(
            DateTime? dateFrom = null,DateTime? dateTo = null, int? top = 10, IList<int> incClients = null,
            IList<int> exClients = null, bool groupByBrands = true,bool excludeSpares = true,
            bool groupByReason = true,bool useETA=false,bool extendToEndOfMonthForUnits = false, 
            bool filterCprodStatus=true,CountryFilter countryFilter = CountryFilter.UKOnly,
            SortField sortBy = SortField.ReturnToSalesRatio,int? minUnitsShipped = null)
		{

			var dateField = useETA ? "om_detail1.req_eta" : "om_detail1.po_req_etd";
			var groupBy = groupByBrands ? "brands.brand_id," : "";
			var countryCondition = countriesDal.GetCountryCondition(countryFilter);
			var clientIds = incClients ?? exClients;

			var cprod_codes = conn.Query<string>(
				$@"SELECT cust_products.cprod_code1
                FROM returns INNER JOIN cust_products ON returns.cprod_id = cust_products.cprod_id  
                    INNER JOIN users ON returns.client_id = users.user_id 
					INNER JOIN mast_products ON cust_products.cprod_mast = mast_products.mast_id
					INNER JOIN brands ON cust_products.brand_user_id = brands.user_id                               
                    WHERE {(filterCprodStatus ? "cprod_status <>'D' AND " : " ")} 
						decision_final = 1 AND status1 = 1 AND (cc_response_date >= @dateFrom OR @dateFrom IS NULL) AND
						(cc_response_date <= @dateTo OR @dateTo IS NULL) 
					{(incClients != null ? " AND client_id IN @clientIds" : exClients != null ? " AND client_id NOT IN @clientIds" : "")}
					{(excludeSpares ? " AND mast_products.category1 <> 13" : "")} 
					{countryCondition}                                             
				{(minUnitsShipped != null ? 
					$@" AND (SELECT COALESCE(SUM(orderqty),0) FROM om_detail1 WHERE om_detail1.cprod_code1 = cust_products.cprod_code1 
						{countryCondition} AND ({dateField} >= @datefrom OR @datefrom IS NULL) 
                      AND ({dateField} <= @dateto OR @dateto IS NULL)) >= @minUnitsShipped" : "")} 
				GROUP BY {groupBy}
					cust_products.cprod_code1 
				 {(groupByBrands || top == null ? "" : $" LIMIT {top}")}",
				new {minUnitsShipped, dateFrom, dateTo, clientIds}
			).ToList();

			return conn.Query<ReturnAggregateDataProduct>(
				$@"SELECT cust_products.cprod_code1,SUM(returns.return_qty) AS numOfReturns,
               COALESCE ((COALESCE( SUM(returns.return_qty),0 )/  COALESCE((SELECT SUM(orderqty) 
							FROM om_detail1 WHERE om_detail1.cprod_code1 = cust_products.cprod_code1 {countryCondition} 
							AND ({dateField} >= @datefrom OR @datefrom IS NULL) 
                            AND ({dateField} <= @dateto OR @dateto IS NULL)),0)),9999) AS ReturnToSalesRatio,
                SUM(returns.return_qty*returns.credit_value) AS TotalReturnValue
                FROM returns INNER JOIN cust_products ON returns.cprod_id = cust_products.cprod_id  
                    INNER JOIN users ON returns.client_id = users.user_id 
					INNER JOIN mast_products ON cust_products.cprod_mast = mast_products.mast_id
					INNER JOIN brands ON cust_products.brand_user_id = brands.user_id                               
                    WHERE {(filterCprodStatus ? "cprod_status <>'D' AND " : " ")} 
						decision_final = 1 AND status1 = 1 AND (cc_response_date >= @dateFrom OR @dateFrom IS NULL) AND
						(cc_response_date <= @dateTo OR @dateTo IS NULL) 
					{(incClients != null ? " AND client_id IN @clientIds" : exClients != null ? " AND client_id NOT IN @clientIds" : "")}
					{(excludeSpares ? " AND mast_products.category1 <> 13" : "")} 
					{countryCondition}                                             
				{(groupByReason && cprod_codes.Count > 0 ? " AND cust_products.cprod_code1 IN @cprod_codes" : "" )}
				GROUP BY {groupBy}
					cust_products.cprod_code1 {(groupByReason ? ",reason" : "")}
				ORDER BY {groupBy} 
				{(sortBy == SortField.ReturnToSalesRatio ? "ReturnToSalesRatio" : "TotalReturnValue")} 
				DESC {(groupByBrands || top == null ? "" : $" LIMIT {(groupByReason ? top*5 : top)}")}",
				new {minUnitsShipped, dateFrom, dateTo, cprod_codes, clientIds}
			).ToList();
		}

        public List<ReturnAggregateDataPrice> GetTotalsPerClient(DateTime? dateFrom = null, DateTime? dateTo = null, int? brand_user_id = null,
	        IList<int> incClients = null, IList<int> exClients = null, 
	        CountryFilter countryFilter = CountryFilter.UKOnly, string excludedCustomers = "NK2")
        {
	        var clientCriteria = (incClients != null ? " AND client_id IN @clientIds" :
		        exClients != null ? " AND client_id NOT IN @clientIds" : "");
	        var exCustomers = excludedCustomers.Split(',');
	        var clientIds = incClients ?? exClients;

	        return conn.Query<ReturnAggregateDataPrice>(
		        $@"SELECT users.user_id AS id,users.customer_code as code, claim_type, 
				SUM((CASE decision_final WHEN 1 THEN CASE WHEN claim_type = 2 THEN claim_value ELSE return_qty * credit_value END ELSE NULL END)) AS TotalAccepted,
				SUM(CASE ebuk WHEN 1 THEN (CASE WHEN claim_type = 2 THEN claim_value ELSE return_qty * credit_value END) ELSE 0 END) AS TotalAcceptedEBUK,
				SUM(CASE decision_final WHEN 500 THEN CASE WHEN claim_type = 2 THEN claim_value ELSE return_qty * credit_value END ELSE NULL END) AS TotalReplacementParts,
				SUM(CASE decision_final WHEN 999 THEN CASE WHEN claim_type = 2 THEN claim_value ELSE return_qty * credit_value END ELSE NULL END) AS TotalRejected
				FROM returns INNER JOIN users ON returns.client_id = users.user_id INNER JOIN cust_products ON returns.cprod_id = cust_products.cprod_id
				WHERE status1 = 1 AND (request_date >= @dateFrom OR @dateFrom IS NULL) AND
				(request_date <= @dateTo OR @dateTo IS NULL) AND decision_final IN (1,999,500) AND claim_type IS NOT NULL AND claim_type <> 5
				AND (cust_products.brand_user_id = @brand_user_id OR @brand_user_id IS NULL) 
				{clientCriteria} {countriesDal.GetCountryCondition(countryFilter)}
                AND users.customer_code NOT IN @exCustomers AND COALESCE(users.test_account,999) <> 1
                GROUP BY users.user_id,users.customer_code, claim_type
				ORDER BY users.user_id,users.customer_code, claim_type",
		        new {dateFrom, dateTo, brand_user_id, clientIds, exCustomers}
	        ).ToList();

			
		}

        public List<ReturnAggregateDataPrice> GetTotalsPerFactory(DateTime? dateFrom = null, DateTime? dateTo = null, 
	        int? brand_user_id = null, IList<int> incClients = null, IList<int> exClients = null, 
	        CountryFilter countryFilter = CountryFilter.UKOnly, string excludedCustomers = "NK2")
        {
	        var clientCriteria = (incClients != null ? " AND client_id IN @clientIds" :
		        exClients != null ? " AND client_id NOT IN @clientIds" : "");
	        var exCustomers = excludedCustomers.Split(',');
	        var clientIds = incClients ?? exClients;

	        return conn.Query<ReturnAggregateDataPrice>(
		        $@"SELECT factory.user_id AS id,factory.factory_code as code, claim_type, 
				SUM((CASE decision_final WHEN 1 THEN CASE WHEN claim_type = 2 THEN claim_value ELSE return_qty * credit_value END ELSE NULL END)) AS TotalAccepted,
				SUM(CASE ebuk WHEN 1 THEN (CASE WHEN claim_type = 2 THEN claim_value ELSE return_qty * credit_value END) ELSE 0 END) AS TotalAcceptedEBUK,
				SUM(CASE decision_final WHEN 500 THEN CASE WHEN claim_type = 2 THEN claim_value ELSE return_qty * credit_value END ELSE NULL END) AS TotalReplacementParts,
				SUM(CASE decision_final WHEN 999 THEN CASE WHEN claim_type = 2 THEN claim_value ELSE return_qty * credit_value END ELSE NULL END) AS TotalRejected
				FROM returns INNER JOIN users ON returns.client_id = users.user_id 
				INNER JOIN cust_products ON returns.cprod_id = cust_products.cprod_id
				INNER JOIN mast_products ON cust_products.cprod_mast = mast_products.mast_id 
				INNER JOIN users factory ON mast_products.factory_id = factory.user_id
				WHERE status1 = 1 AND (request_date >= @dateFrom OR @dateFrom IS NULL) AND
				(request_date <= @dateTo OR @dateTo IS NULL) AND decision_final IN (1,999,500) AND claim_type IS NOT NULL AND claim_type <> 5
				AND (cust_products.brand_user_id = @brand_user_id OR @brand_user_id IS NULL) 
				{clientCriteria} {countriesDal.GetCountryCondition(countryFilter,"users.")}
                AND users.customer_code NOT IN @exCustomers AND COALESCE(users.test_account,999) <> 1
                GROUP BY factory.user_id,factory.factory_code, claim_type
				ORDER BY factory.user_id,factory.factory_code, claim_type",
		        new {dateFrom, dateTo, brand_user_id, clientIds, exCustomers}
	        ).ToList();

            
        }
        
		public override void Create(Returns o, IDbTransaction tr = null)
		{
			string insertsql = GetCreateSql();

			string checkSQL = "SELECT * FROM returns WHERE return_no = @return_no";
			
			 if(conn.State != ConnectionState.Open)
				conn.Open();
			if(tr == null)
				tr = conn.BeginTransaction();
			try
			{
				var returns = conn.Query<Returns>(checkSQL, new {o.return_no}).ToList();
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

				base.Create(o, tr);

				if (o.Images != null)
				{
					foreach (var image in o.Images)
					{
						image.return_id = o.returnsid;
						returnsImagesDal.Create(image,tr);
					}
				}
				if (o.Subscriptions != null && o.Subscriptions.Count > 0)
				{
					foreach (var s in o.Subscriptions)
					{
						s.subs_returnid = o.returnsid;
						feedbackSubscriptionsDal.Create(s,tr);
					}
				}
				tr.Commit();
			}
			catch
			{
				tr.Rollback();
			}
		}

		
        public void UpdateRejection(Returns o)
        {
	        conn.Execute(@"UPDATE 
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
                        returnsid = @returnsid", o);
            
        }
        public void UpdateSimpleRecheck(Recheck o)
        {
			
                string updatesql = 
	                @"UPDATE 
                        returns 
                     SET
                        recheck_date=@recheck_date,                                       
                        client_comments2= @client_comments2,
                       recheck_status=@recheck_status
                    WHERE 
                        returnsid = @returnsid";
                if(conn.State != ConnectionState.Open)
					conn.Open();
                var tr = conn.BeginTransaction();
                try
                {
	                conn.Execute(updatesql, o);

                    foreach (var image in o.Images)
                    {
                        returnsImagesDal.Create(image, tr);
                    }

                    tr.Commit();
                }
                catch (Exception)
                {
                    tr.Rollback();
                    throw;
                }
                
        }
        
        public void Update(Returns o, List<int> deletedImagesIds = null)
		{
			if(conn.State != ConnectionState.Open)
				conn.Open();
			var tr = conn.BeginTransaction();
			try
			{
				base.Update(o, tr);
				if (o.Images != null)
				{
					foreach (var image in o.Images)
					{
						if (image.image_unique <= 0)
						{
							image.return_id= o.returnsid;
							returnsImagesDal.Create(image, tr);
						}
						else
						{
							returnsImagesDal.Update(image, tr);
						}
					}
				}
				if (deletedImagesIds != null)
				{
					foreach (var di in deletedImagesIds)
					{
						returnsImagesDal.Delete(di, tr);
					}
				}
				tr.Commit();
			}
			catch
			{
				tr.Rollback();
				throw;
			}
			
		}

		
		public List<ClaimsAnalyticsRow> GetAnalytics(List<int> ids, DateTime? from = null, DateTime? to = null)
		{
			var data = conn.Query<ClaimsAnalyticsRow>(
				@"SELECT cprod_id, cprod_code1,cprod_name, 
				COALESCE((SELECT SUM(orderqty) FROM om_detail1 
					WHERE om_detail1.cprod_id = cust_products.cprod_id AND (om_detail1.po_req_etd >= @from OR @from IS NULL) 
					AND (om_detail1.po_req_etd <= @to OR @to IS NULL)),0) AS orderqty, 
					COALESCE((SELECT SUM(return_qty) FROM returns 
						WHERE returns.cprod_id = cust_products.cprod_id AND decision_final = 1 
						AND (returns.request_date >= @from OR @from IS NULL) AND (returns.request_date <= @to OR @to IS NULL)),0) AS claims 
                FROM cust_products WHERE cprod_id IN @ids ",
				new { from, to, ids }
			).ToList();

			foreach (var r in data)
			{
				r.From = from;
				r.To = to;
			}

			return data;

			
		}

		public List<ClaimsAnalyticsRow> GetAnalytics(string cprod_code1, DateTime? from = null, DateTime? to = null)
		{
			var data = conn.Query<ClaimsAnalyticsRow>(
				@"SELECT cprod_id, cprod_code1,cprod_name, 
				COALESCE((SELECT SUM(orderqty) FROM om_detail1 
					WHERE om_detail1.cprod_id = cust_products.cprod_id AND (om_detail1.po_req_etd >= @from OR @from IS NULL) 
					AND (om_detail1.po_req_etd <= @to OR @to IS NULL)),0) AS orderqty, 
					COALESCE((SELECT SUM(return_qty) FROM returns 
						WHERE returns.cprod_id = cust_products.cprod_id AND decision_final = 1 
						AND (returns.request_date >= @from OR @from IS NULL) AND (returns.request_date <= @to OR @to IS NULL)),0) AS claims 
                FROM cust_products  WHERE cprod_code1 =@cprod_code1 ",
				new { from, to, cprod_code1 }
			).ToList();

			foreach (var r in data)
			{
				r.From = from;
				r.To = to;
			}

			return data;

			
		}

        
		public void UpdateOpenClose(int feedback_id,int openClose)
		{
			conn.Execute("UPDATE returns SET openclosed = @openclose WHERE returnsid=@feedback_id",
				new {feedback_id, openClose});
			
		}

		public void UpdateStatus(int id, FeedbackStatus status, int? authorizationLevel = null)
		{
			conn.Execute(
				$@"UPDATE returns SET status1 = @status {
						(authorizationLevel != null ? ",authorization_level =  @authorizationLevel" : "")
					} 
					WHERE returnsid=@id", new {id, authorizationLevel, status});
		}


        public List<Returns> GetForMastProducts(IList<int> mast_ids, bool commentsOnly = false, DateTime? fromDate = null)
        {
	        return conn.Query<Cust_products, Returns, Returns>(
		        $@"SELECT cust_products.*,returns.* FROM returns INNER JOIN cust_products ON returns.cprod_id = cust_products.cprod_id
                     WHERE cust_products.cprod_mast IN @mast_ids 
                     {(commentsOnly ? " AND returns.factory_comments IS NOT NULL" : "")} {
				        (fromDate != null ? " AND returns.request_date >= @fromDate" : "")
			        }",
		        (cp, r) =>
		        {
			        r.Product = cp;
			        return r;
		        }, new {mast_ids, fromDate}, splitOn: "returnsid").ToList();
			
        }

        public string GetCustomerIdInWhereClause(string customerIds, string inOut)
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
	

        public List<CAReportCAItem> GetCAReportCAItems(DateTime dateForCAFrom, DateTime dateForCATo,string customerIdsIn,string customerIdsOut)
        {
            List<CAReportCAItem> items = new List<CAReportCAItem>();

            return conn.Query<CAReportCAItem>(
		        $@"
                    SELECT 
                     r.RETURN_NO AS Reference
                    ,r.request_date AS DataCreated
                    ,r.custpo AS PO
                    ,r.spec_code1 AS PRODUCTS
                    ,users.factory_code AS FACTORY
                    ,r.client_comments AS DESCRIPTION
                    ,r.cprod_id
                    ,users_customers.customer_code as CustomerCode
                    ,r.request_user as RequestUser
                FROM `returns` r
                LEFT JOIN returns_cust_products RCP ON r.returnsid = RCP.returns_id
                    LEFT JOIN cust_products cp_returns ON RCP.cprod_id = cp_returns.cprod_id
                    LEFT JOIN cust_products cp ON r.cprod_id = cp.cprod_id
                        LEFT JOIN  mast_products mp ON mp.mast_id = cp.cprod_mast
                            LEFT JOIN users ON users.user_id = mp.factory_id
                                LEFT JOIN users users_customers on cp.cprod_user = users_customers.user_id
                WHERE claim_type = 7 AND r.spec_code1 IS NOT NULL AND (r.request_date >= @dateForCAFrom AND r.request_date < @dateForCATo)
                {GetCustomerIdInWhereClause(customerIdsIn, "IN")}
                {GetCustomerIdInWhereClause(customerIdsOut, "NOT IN")}
                UNION
                SELECT 
                     r.RETURN_NO AS Reference
                    ,r.request_date AS DataCreated
                    ,r.custpo AS PO
                    ,cp.cprod_code1 as PRODUCTS
                    ,users.factory_code AS FACTORY
                    ,r.client_comments AS DESCRIPTION
                    ,rcp.cprod_id
                    ,users_customers.customer_code as CustomerCode
                    ,r.request_user as RequestUser
                FROM `returns` r
                LEFT JOIN returns_cust_products RCP ON r.returnsid = RCP.returns_id
                    LEFT JOIN cust_products cp ON RCP.cprod_id = cp.cprod_id
                        LEFT JOIN  mast_products mp ON mp.mast_id = cp.cprod_mast
                            LEFT JOIN users ON users.user_id = mp.factory_id
                                LEFT JOIN users users_customers on cp.cprod_user = users_customers.user_id
                WHERE claim_type = 7 AND cp.cprod_code1 IS NOT NULL AND (r.request_date >= @dateForCAFrom AND r.request_date < @dateForCATo)
                     {GetCustomerIdInWhereClause(customerIdsIn, "IN")}
                     {GetCustomerIdInWhereClause(customerIdsOut, "NOT IN")}",
		        new {dateForCAFrom, dateForCATo}).ToList();
                
        }

        public CAReportInspectionItem GetCAReportInspectionItem(DateTime dateForCAInspectionItemFrom, DateTime dateForCAInspectionItemTo, string customerIdsIn, string customersIdOut)
        {
            CAReportInspectionItem item = new CAReportInspectionItem();

            var joinClause =
                " LEFT OUTER JOIN users users_customers ON insp.customer_code = users_customers.customer_code";

            var isCustomerCondition = 
                string.IsNullOrEmpty(customerIdsIn);

            if(isCustomerCondition)
                isCustomerCondition = string.IsNullOrEmpty(customersIdOut);

            return conn.QueryFirstOrDefault<CAReportInspectionItem>(
		        $@"
                    SELECT
                        COALESCE(SUM(insp.insp_type = 'LO'),0) AS LICount,
                        COALESCE(SUM(insp.insp_type = 'FI'),0) AS FICount
                    FROM inspections insp
                        {(isCustomerCondition ? "" : joinClause)}
                    WHERE insp.insp_type IN('FI', 'LO')
                    AND (insp.insp_start >= @dateForCaInspectionItemFrom AND insp.insp_start < @dateForCaInspectionItemTo)
                    {GetCustomerIdInWhereClause(customerIdsIn, "IN")}
                    {GetCustomerIdInWhereClause(customersIdOut, "NOT IN")}",
		        new {dateForCAInspectionItemFrom, dateForCAInspectionItemTo}
	        );
            
        }

		public List<Returns> GetForClaimTypeSimple(int claim_type, bool products = false, IList<int?> groupsId = null)
        {
			return conn.Query<Returns>(
			$@"SELECT returns.returnsid,
                returns.status1,
                returns.request_userid,
                returns.claim_type,
                returns.issue_type_id,
                returns.authorization_level,
            (SELECT COUNT(*) FROM returns_comments WHERE return_id = returns.returnsid AND comments NOT LIKE 'Authorization%') AS HasComments,
            (SELECT comments_from FROM returns_comments WHERE return_id = returns.returnsid AND comments NOT LIKE 'Authorization%' ORDER BY comments_date DESC LIMIT 1) AS last_commenter_id
			FROM returns 
			WHERE claim_type = @claim_type {(groupsId != null && groupsId.Count > 0 ? $" AND usergroup_id IN ({string.Join(",", groupsId)})" : "")}",
			new {claim_type}).ToList();
           
        }
		

		protected override string GetAllSql()
		{
			return "SELECT * FROM returns";
		}

		protected override string GetByIdSql()
		{
			return "SELECT * FROM returns WHERE returnsid = @id";
		}

		protected override string GetCreateSql()
		{
			return @"INSERT INTO returns 
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
                    @flagged,@flagged_reason,@importance_id,@highlight,@std_unique_id,
			        @resolution,@ip_address,@delaminating,@quote1,
                    @quote1_price,@quote1a,@quote1a_price,@quote1b,
                    @quote1b_price,@quote1c,@quote1c_price,@quote2,@quote2_price,
			        @quote3,@quote3_price,@quotechoice,@factory_reason,
                    @spec_code1,@spec_name,@fc_po_sufficient,@fc_evidence_sufficient,
                    @fc_evidence_required,@fc_evidence_file,
			        @fc_acceptance,@fc_product_change_description,@fc_cnid,@feedback_category_id,@rejection,@issue_type_id,@authorization_level,@usergroup_id
                )";
		}

		protected override string GetUpdateSql()
		{
			return 
				@"UPDATE returns 
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
                        importance = @importance_id,
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
		}

		protected override string GetDeleteSql()
		{
			return "DELETE FROM returns WHERE returnsid = @id";
		}
	}

	public class ReturnsMap : EntityMap<Returns>
	{
		public ReturnsMap()
		{
			Map(u => u.importance_id).ToColumn("importance");
		}

	}
}

