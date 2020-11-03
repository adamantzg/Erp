using System;
using System.Collections.Generic;
using System.Linq;
using company.Common;
using erp.DAL.EF.Repositories;
using erp.Model;
using RefactorThis.GraphDiff;
using LinqKit;

namespace erp.DAL.EF
{
	public class ReturnRepository : GenericRepository<Returns>,IReturnRepository
    {

        public ReturnRepository(Model context) : base(context)  
        {

        }

        public static List<ReturnAggregateDataPrice> GetTotalsPerBrand(DateTime? dateFrom = null,
            DateTime? dateTo = null, IList<int> incClients = null,
            IList<int> exClients = null, CountryFilter countryFilter = CountryFilter.UKOnly)
        {
            var ukCountries = new[] { "GB", "IE", "UK" };
            
            using (var m = Model.CreateModel())
            {
                var asianCountries = m.Set<Countries>().Where(c => c.continent_code == "AS").Select(c => c.ISO2).ToList();
                int?[] decisions = {1, 999, 500};
                var where = PredicateBuilder.True<Returns>();
                where = where.And(r => r.status1 == 1 && (dateFrom == null || r.request_date >= dateFrom) && r.Client.distributor > 0 && r.Client.hide_1 == 0 &&
                               (dateTo == null || r.request_date <= dateTo) && decisions.Contains(r.decision_final) &&
                               r.claim_type != null && r.claim_type != 5
                    );

                if (incClients != null)
                {
                    var clientWhere = PredicateBuilder.False<Returns>() ;
                    foreach (var cl in incClients)
                    {
                        clientWhere = clientWhere.Or(r => r.client_id == cl).Expand();
                    }
                    where = where.And(clientWhere).Expand();
                }
			    else if (exClients != null)
			    {
                    var clientWhere = PredicateBuilder.True<Returns>();
				    foreach (var cl in exClients)
                    {
                        clientWhere = clientWhere.And(r => r.client_id != cl).Expand();
                    }
                    where = where.And(clientWhere).Expand();
			    }

                where =
                    where.And(
                        r => (countryFilter == CountryFilter.UKOnly && ukCountries.Contains(r.Client.user_country))
                             || (countryFilter == CountryFilter.NonUK && !ukCountries.Contains(r.Client.user_country))
                             || (countryFilter == CountryFilter.NonUKExcludingAsia && !ukCountries.Contains(r.Client.user_country) && !asianCountries.Contains(r.Client.user_country))
                             );

                return
                    m.Returns.Include("Client")
                        .Include("Product")
                        .AsExpandable()
                        .Where(where)
                        .GroupBy(r => new {r.Product.brand_userid,r.claim_type})
                        .OrderBy(g => g.Key.brand_userid).ToList()
                        .Select(g => new ReturnAggregateDataPrice {id = g.Key.brand_userid,claim_type = g.Key.claim_type ?? 0,
                            TotalAccepted = g.Where(r=>r.decision_final == 1).Sum(r=>ReturnsSelector(r)) ?? 0,
                            TotalAcceptedEBUK = g.Where(r => r.ebuk == 1 && r.decision_final == 1).Sum(r => ReturnsSelector(r)) ?? 0,
                            TotalRejected =  g.Where(r=>r.decision_final == 999).Sum(r=>ReturnsSelector(r)) ?? 0,
                            TotalReplacementParts = g.Where(r=>r.decision_final == 500).Sum(r=>ReturnsSelector(r)) ?? 0
                        }).ToList();

            }

        }

        public static List<ReturnAggregateDataByMonth> GetReturnsByMonth(DateTime? from = null, DateTime? to = null, bool acceptedOnly = true, IList<int> cprod_ids = null )
        {
            using (var m = Model.CreateModel())
            {
                var predicate = PredicateBuilder.True<Returns>();
                predicate =
                predicate.And(
                    r => (r.request_date >= from || from == null) && (r.request_date <= to || to == null) &&
                            (!acceptedOnly || r.decision_final == 1) && r.request_date != null);
                if (cprod_ids != null)
                    predicate = predicate.And(r => r.cprod_id != null && cprod_ids.Contains(r.cprod_id.Value));
                return m.Returns.AsExpandable().Where(predicate).GroupBy(r => new {year = r.request_date.Value.Year, month = r.request_date.Value.Month}).ToList()
                .Select(
                    g =>
                        new ReturnAggregateDataByMonth
                        {
                            created_month = Month21.FromDate(new DateTime(g.Key.year, g.Key.month, 1)).Value,
                            CountReturns = g.Count(),
                            ReturnValue = g.Sum(r=>r.TotalValue)
                        }).ToList();
            }
        }

        public static List<ReturnAggregateDataProduct> GetReturnsByReason(DateTime? from = null, DateTime? to = null, IList<int> cprod_ids = null,
            bool acceptedOnly = true)
        {
            using (var m = Model.CreateModel())
            {
                var predicate = PredicateBuilder.True<Returns>();
                predicate =
                    predicate.And(
                        r => (r.request_date >= from || from == null) && (r.request_date <= to || to == null) &&
                             (!acceptedOnly || r.decision_final == 1) && r.request_date != null);
                if (cprod_ids != null)
                    predicate = predicate.And(r => r.cprod_id != null && cprod_ids.Contains(r.cprod_id.Value));
                return
                    m.Returns.AsExpandable().Where(predicate)
                        .GroupBy(r => r.reason).ToList()
                        .Select(
                            g =>
                                new ReturnAggregateDataProduct
                                {
                                    Reason = g.Key,
                                    TotalAccepted = g.Sum(r=>r.return_qty) ?? 0
                                }).ToList();
            }
        }

        public static List<Returns> GetForPeriodAndBrand(int brand_id, DateTime? from = null, DateTime? to = null)
        {
            using (var m = Model.CreateModel())
            {
                return
                    m.Returns.Include("Product").Include("Client").Where(
                        r => (r.request_date >= from || from == null) && (r.request_date <= to || to == null) &&
                             (r.decision_final == 1) && r.Product.brand_id == brand_id).ToList();
            }
        }

        public static List<Returns> GetForPeriodAndProduct(int cprod_id, DateTime? from = null, DateTime? to = null)
        {
            using (var m = Model.CreateModel())
            {
                return
                    m.Returns.Include("Product").Include("Client").Where(
                        r => (r.request_date >= from || from == null) && (r.request_date <= to || to == null) &&
                             (r.decision_final == 1) && r.cprod_id == cprod_id).ToList();
            }
        }

        public static List<Returns> GetFeedbacks(IList<int> cprod_ids, int type)
        {
            using (var m = Model.CreateModel())
            {
                return m.Returns.Include("Comments").Include("Comments.Creator").Where(r => r.cprod_id != null && cprod_ids.Contains(r.cprod_id.Value) && r.claim_type == type).ToList();
            }
        }

        public static List<Returns_comments> GetComments(int return_id)
        {
            using (var m = Model.CreateModel())
            {
                var ret = m.Returns.Include("Comments").FirstOrDefault(r => r.returnsid == return_id);
                if (ret != null)
                    return ret.Comments;
                return null;
            }
        }

        private static double? ReturnsSelector(Returns r)
        {
            return r.claim_type == 2 ? r.claim_value : r.return_qty*r.credit_value;
        }

        public List<Returns> GetForFactories(IList<int> factoryIds, bool commentsOnly = false, DateTime? fromDate= null)
        {
            using (var m = Model.CreateModel())
            {
                return m.Returns.Include("Product.MastProduct").
                    Where(
                        r =>
                            r.Product != null && r.Product.MastProduct != null &&
                            r.Product.MastProduct.factory_id != null &&
                            factoryIds.Contains(r.Product.MastProduct.factory_id.Value)

                            && (!commentsOnly || !string.IsNullOrEmpty(r.factory_comments))
                            && (fromDate == null || r.request_date >= fromDate)
                            ).ToList();
            }
        }

        public override void Update(Returns ret)
        {
            context.UpdateGraph(ret, map => map.AssociatedCollection(r => r.Products).AssociatedCollection(r=>r.ReturnsQCUsers).AssociatedCollection(r=>r.AssignedQCUsers).OwnedCollection(r=>r.Comments, m=>m.OwnedCollection(c=>c.Files)).OwnedCollection(r => r.Images).OwnedCollection(r=>r.Subscriptions));
        }

        public override void Insert(Returns r)
        {
            if (r.Product != null) {
                //var p = context.CustProducts.FirstOrDefault(c => c.cprod_id == r.Product.cprod_id);
                //if(p == null)
                    context.Set<Cust_products>().Attach(r.Product);
            }
                
            if (r.Products != null)
            {
                var ids = r.Products.Select(p => p.cprod_id).ToList();
                r.Products = context.Set<Cust_products>().Where(p => ids.Contains(p.cprod_id)).ToList();
            }
            if (r.ReturnsQCUsers != null)
            {
                //REFACTORING
                //type == null is for qc users, type 1 is for assigned qc users
                var ids = r.ReturnsQCUsers.Select(u => Convert.ToInt32(u.useruser_id)).ToList();

                var qclist = context.Set<User>().Where(u => ids.Contains(u.userid)).ToList();

                var ret_qclist = qclist.Select(u =>
                    new 
                    {
                        Return = r,
                        User = u,
                        return_id = r.returnsid,
                        useruser_id = u.userid
                    }
                    ).AsEnumerable().Select(q => new Returns_qcusers {
                        Return = q.Return,
                        User = q.User,
                        return_id = q.return_id,
                        useruser_id = q.useruser_id,
                        type = 0
                    }).ToList();

                //r.ReturnsQCUsers = context.Set<User>().Where(u => ids.Contains(u.userid)).ToList();
                r.ReturnsQCUsers = ret_qclist.ToList();
            }

            if (r.Creator != null)
                r.Creator = context.Set<User>().FirstOrDefault(u => u.userid == r.Creator.userid);

            base.Insert(r);
        }

        public List<ClaimSimple> GetClaimsSimple(int type, int? status1 = null)
        {
            return context.Database.SqlQuery<ClaimSimple>(
                   @"SELECT r.returnsid, r.return_no, r.request_date, r.client_id, r.request_userid, recheck_required,recheck_date,client_comments2,recheck_status,assigned_qc,closed_date,
                    (SELECT factory_code FROM users INNER JOIN mast_products ON users.user_id = mast_products.factory_id INNER JOIN cust_products ON mast_products.mast_id = cust_products.cprod_mast INNER JOIN returns_cust_products 
                    ON cust_products.cprod_id = returns_cust_products.cprod_id WHERE returns_cust_products.returns_id = r.returnsid LIMIT 1
                    UNION SELECT factory_code FROM users INNER JOIN mast_products ON users.user_id = mast_products.factory_id INNER JOIN cust_products ON mast_products.mast_id = cust_products.cprod_mast 
                    WHERE cust_products.cprod_id = r.cprod_id LIMIT 1) factory,
                    r.client_comments AS description, cat.name as category, creator.userwelcome creator, r.feedback_category_id,
                    (SELECT userusers.userwelcome FROM userusers INNER JOIN returns_comments ON userusers.useruserid = returns_comments.comments_from 
                        WHERE returns_comments.return_id = r.returnsid ORDER BY returns_comments.comments_date DESC LIMIT 1
                     ) lastUpdatedBy,
                    returns_importance.importance_text as importance, r.openclosed, (SELECT COUNT(*) FROM returns_comments WHERE return_id = r.returnsid) AS commentCount, r.importance as importance_id
                    FROM returns r INNER JOIN feedback_category cat ON r.feedback_category_id = cat.feedback_cat_id
                    INNER JOIN userusers creator ON r.request_userid = creator.useruserid LEFT OUTER JOIN returns_importance ON r.importance = returns_importance.importance_id
                    WHERE r.claim_type = @p0 AND (r.status1 = @p1 OR @p1 IS NULL)", type, status1).ToList();
        }

        public List<ClaimSimple> GetClaimsSimpleAll(User user, int month, bool closedOnly = false, int? qc_id = null, bool loadProducts = false)
        {

            var monthCriteria = "fn_Month21(COALESCE(closed_date, request_date)) = @p0";

            List<string> factoryCodes = null;
            string factoryCriteria =  (user == null || user.admin_type == User.adminType_PowerUser ? "1=1" : "1=0");
            if (user != null && user.admin_type != User.adminType_PowerUser)
            {
                factoryCodes = context.Set<Admin_permissions>().Include("Company").Where(a => a.userid == user.userid).Select(a => a.Company.factory_code).ToList();
                if (factoryCodes.Count > 0)
                    factoryCriteria = $"returns.factory IN ({string.Join(",",factoryCodes.Where(c=>c != null).Select(c => $"'{c}'"))})";
            }
            var qcCriteria = qc_id != null ? " AND inspection_controller.controller_id = @p1" : "";

            //subsql for returns - once used for returns with orderid set (!= 0 or 9999), second time for returns without orderid
            var subSql = @"SELECT r.returnsid,r.order_id, 
                            COALESCE(returns_cust_products.cprod_id,r.cprod_id) AS cprod_id,
                            r.return_no,
                            r.client_id,
                            r.request_date,
                            userusers.userusername as request_user,
                            feedback_type.typename as type,
                            users.customer_code as client,
                            r.decision_final,   
                            r.openclosed,
                            r.status1,
                            r.claim_type,
                            r.recheck_date,
                            r.recheck_status,
                            return_category.category_name,
                             (SELECT factory_code FROM users INNER JOIN mast_products ON users.user_id = mast_products.factory_id INNER JOIN cust_products ON mast_products.mast_id = cust_products.cprod_mast INNER JOIN returns_cust_products 
                                                ON cust_products.cprod_id = returns_cust_products.cprod_id WHERE returns_cust_products.returns_id = r.returnsid LIMIT 1
                                                UNION SELECT factory_code FROM users INNER JOIN mast_products ON users.user_id = mast_products.factory_id INNER JOIN cust_products ON mast_products.mast_id = cust_products.cprod_mast 
                                                WHERE cust_products.cprod_id = r.cprod_id LIMIT 1) factory,
                            (SELECT returns_comments.comments_from FROM returns_comments 
                                WHERE returns_comments.return_id = r.returnsid ORDER BY returns_comments.comments_date DESC LIMIT 1
                            ) lastCommenterId
                            
                            FROM
                            `returns` r
                            INNER JOIN userusers ON r.request_userid = userusers.useruserid
                            INNER JOIN feedback_type ON r.claim_type = feedback_type.type_id
                            INNER JOIN users ON r.client_id = users.user_id
                            LEFT OUTER JOIN return_category ON r.reason = return_category.category_code
                            LEFT OUTER JOIN returns_cust_products ON r.returnsid = returns_cust_products.returns_id
                            where request_date > now() - interval 3 month and claim_type <> 6 and status1 = 1
                            AND COALESCE(r.order_id,0) {0} IN (0,9999)" +
                            " AND " + (!closedOnly ? $"(COALESCE(r.openClosed,0) = 0 OR {monthCriteria})" : monthCriteria)
                            ;

            //Start with first sql for returns with orders
            var sql = $@"SELECT returns.returnsid, 
                        `returns`.return_no,
                        `returns`.request_date,
                        returns.request_user as creator,
                        returns.type,
                        returns.client,
                        `returns`.decision_final, 
                        returns.factory, 
                        returns.claim_type,
                        returns.category_name as category,
                        returns.openclosed,
                        returns.status1,
                        returns.recheck_date,
                        returns.recheck_status,
                        returns.lastCommenterId,
                        returns.cprod_id,
                        returns.order_id,
                        GROUP_CONCAT(DISTINCT userusers.userwelcome SEPARATOR ', ') AS Qc 
                        FROM order_lines INNER JOIN ({string.Format(subSql, "NOT")}) returns ON  returns.order_id = order_lines.orderid AND returns.cprod_id = order_lines.cprod_id 
                        LEFT OUTER JOIN inspection_lines_tested ON order_lines.linenum = inspection_lines_tested.order_linenum LEFT OUTER JOIN inspections ON inspection_lines_tested.insp_id = inspections.insp_unique
                        LEFT OUTER JOIN inspection_controller ON inspections.insp_unique = inspection_controller.inspection_id LEFT OUTER JOIN userusers ON inspection_controller.controller_id = userusers.useruserid
                        WHERE {factoryCriteria} {qcCriteria} 
                        GROUP BY returns.returnsid";
            //Union with second sql for returns without orders
            sql += " UNION " +
                     $@"SELECT returns.returnsid,
                        `returns`.return_no,
                        `returns`.request_date,
                        returns.request_user as creator,
                        returns.type,
                        returns.client,
                        `returns`.decision_final, 
                        returns.factory,
                        returns.claim_type,
                        returns.category_name as category,
                        returns.openclosed,
                        returns.status1,
                        returns.recheck_date,
                        returns.recheck_status,
                        returns.lastCommenterId,
                        returns.cprod_id,
                        returns.order_id,
                        GROUP_CONCAT(DISTINCT userusers.userwelcome SEPARATOR ', ') AS Qc  
                        FROM order_lines INNER JOIN order_header ON order_lines.orderid = order_header.orderid 
                        INNER JOIN cust_products ON order_lines.cprod_id = cust_products.cprod_id 
                        INNER JOIN ({string.Format(subSql, "")}) returns ON  (returns.client_id = cust_products.brand_user_id OR returns.client_id = order_header.userid1) AND returns.cprod_id = order_lines.cprod_id 
                        LEFT OUTER JOIN inspection_lines_tested ON order_lines.linenum = inspection_lines_tested.order_linenum LEFT OUTER JOIN inspections ON inspection_lines_tested.insp_id = inspections.insp_unique
                        LEFT OUTER JOIN inspection_controller ON inspections.insp_unique = inspection_controller.inspection_id LEFT OUTER JOIN userusers ON inspection_controller.controller_id = userusers.useruserid
                        WHERE order_header.req_eta > now() - interval 3 month
                        AND {factoryCriteria} {qcCriteria}
                        GROUP BY returns.returnsid
                        ";
            //Union with sql for returns for client_id = 3 (no qcs and no factory)
            sql += " UNION " +
                $@"SELECT  
                    r.returnsid, 
                    r.return_no,
                    r.request_date,
                    userusers.userusername as creator,
                    feedback_type.typename as type,
                    users.customer_code as client,
                    r.decision_final,
                    'General feedback' factory,
                    r.claim_type,                    
                    return_category.category_name as category,
                    r.openclosed,
                    r.status1,
                    r.recheck_date,
                    r.recheck_status,
                    (SELECT returns_comments.comments_from FROM returns_comments 
                                WHERE returns_comments.return_id = r.returnsid ORDER BY returns_comments.comments_date DESC LIMIT 1
                            ) lastCommenterId,
                    r.cprod_id,
                    r.order_id,
                    'n/a' Qc
                    FROM
                    `returns` r
                    INNER JOIN userusers ON r.request_userid = userusers.useruserid
                    INNER JOIN feedback_type ON r.claim_type = feedback_type.type_id
                    INNER JOIN users ON r.client_id = users.user_id
                    LEFT OUTER JOIN return_category ON r.reason = return_category.category_code
                    where request_date > now() - interval 3 month  and status1 = 1
                    and r.client_id = 3 AND " +
                    (!closedOnly ? $"(r.openClosed = 0 OR {monthCriteria})" : monthCriteria);

            //UNION for CA, QA
            /*sql += " UNION " +
                $@"SELECT  
                    r.returnsid, 
                    r.return_no,
                    r.request_date,
                    userusers.userusername as creator,
                    feedback_type.typename as type,
                    users.customer_code as client,
                    r.decision_final,
                     (SELECT factory_code FROM users INNER JOIN mast_products ON users.user_id = mast_products.factory_id INNER JOIN cust_products ON mast_products.mast_id = cust_products.cprod_mast INNER JOIN returns_cust_products 
                                        ON cust_products.cprod_id = returns_cust_products.cprod_id WHERE returns_cust_products.returns_id = r.returnsid LIMIT 1
                                        UNION SELECT factory_code FROM users INNER JOIN mast_products ON users.user_id = mast_products.factory_id INNER JOIN cust_products ON mast_products.mast_id = cust_products.cprod_mast 
                                        WHERE cust_products.cprod_id = r.cprod_id LIMIT 1) factory,
                     r.claim_type,
                    return_category.category_name as category,
                    r.openclosed,
                    r.recheck_date,
                    r.recheck_status,
                    (SELECT returns_comments.comments_from FROM returns_comments 
                                WHERE returns_comments.return_id = r.returnsid ORDER BY returns_comments.comments_date DESC LIMIT 1
                            ) lastCommenterId,
                     IF(r.claim_type = 7,userusers.userwelcome,'') Qc
                    FROM
                    `returns` r
                    INNER JOIN userusers ON r.request_userid = userusers.useruserid
                    INNER JOIN feedback_type ON r.claim_type = feedback_type.type_id
                    INNER JOIN users ON r.client_id = users.user_id
                    LEFT OUTER JOIN return_category ON r.reason = return_category.category_code
                    where request_date > now() - interval 3 month and claim_type > 6 and status1 = 1 AND "
                    + (!closedOnly ? $"(r.openClosed = 0 OR {monthCriteria})" : monthCriteria);*/

            var claims = context.Database.SqlQuery<ClaimSimple>(sql,month,qc_id).ToList();
            if(loadProducts)
            {
                var cprod_ids = claims.Select(c => c.cprod_id).Distinct().ToList();
                var products = context.Set<Cust_products>().Where(p => cprod_ids.Contains(p.cprod_id)).ToDictionary(p=>(int?) p.cprod_id);
                foreach (var c in claims)
                {
                    if (products.ContainsKey(c.cprod_id))
                        c.Product = products[c.cprod_id];
                }
            }

            return claims;

        }

        public List<ClaimsStatsOtherProductRow> GetOtherProductsSales(DateTime? etaFrom, DateTime? etaTo, IList<int?> factory_Ids = null, IList<int?> cprod_ids = null,IList<int?> brand_ids = null)
        {
            var criteria = new List<string>();

            if (factory_Ids != null && factory_Ids.Count > 0)
                criteria.Add($"mast_products.factory_id IN ({string.Join(",", factory_Ids)})");
            if (brand_ids != null && brand_ids.Count > 0)
                criteria.Add($"cust_products.brand_id IN ({string.Join(",", brand_ids)})");
            if (cprod_ids != null && cprod_ids.Count > 0)
                criteria.Add($"order_lines.cprod_id NOT IN ({string.Join(",", cprod_ids)})");
            var criteriaString = string.Empty;
            if (criteria.Count > 0)
                criteriaString = " AND " + string.Join(" AND ", criteria);
            
            return context.Database.SqlQuery<ClaimsStatsOtherProductRow>(
                $@"SELECT brands.brandname, f.factory_code,cust_products.cprod_id, SUM(order_lines.orderqty * order_lines.unitprice) AS value 
                    FROM order_lines 
                    INNER JOIN order_header ON order_lines.orderid = order_header.orderid 
                    INNER JOIN cust_products ON order_lines.cprod_id = cust_products.cprod_id
                    INNER JOIN mast_products ON cust_products.cprod_mast = mast_products.mast_id
                    INNER JOIN users f ON mast_products.factory_id = f.user_id
                    INNER JOIN brands ON cust_products.brand_id = brands.brand_id
                    WHERE 
                    req_eta >= @p0 AND req_eta <= @p1
                    AND orderqty > 0
                    {criteriaString}
                    AND order_header.status NOT IN ('X','Y')                    
                    GROUP BY cust_products.brand_id, f.factory_code, cust_products.cprod_id",etaFrom, etaTo).ToList();
        }

        //public List<int?> GetFactoriesFromClaimsReports()
        //{
        //    return context.ClaimsReportData.Select(r => r.factory_id).Distinct().ToList();
        //}
    }
}
