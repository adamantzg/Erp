using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using erp.Model;

namespace erp.Model.DAL
{
    public class ViewsDAL
    {
        public static List<DistCustProductDetail2> GetProductStockStatus(int userid)
        {
            var results = new List<DistCustProductDetail2>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = Utils.GetCommand(@"SELECT * FROM asaq.dist_cust_product_details2 WHERE client_id=@userid", conn);
                cmd.Parameters.AddWithValue("@userid", userid.ToString());
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    var o = new DistCustProductDetail2
                    {
                        cprod_code1 = string.Empty + dr["cprod_code1"],
                        cprod_name = string.Empty + dr["cprod_name"],
                        old_data = string.Empty + dr["old_data"],
                        new_data = string.Empty + dr["new_data"],
                    };
                    results.Add(o);
                }
                dr.Close();
            }
            return results;
        }

        public static List<AmendmentsLocation> GetProductAvailabilityChanges(int userid)
        {
            var results = new List<AmendmentsLocation>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = Utils.GetCommand(@"SELECT cprod_code1, (SUM(old_data)-sum(new_data)) as changetot, count(ref1) as entries, SUM(old_data) as old_data, sum(new_data) as new_data,
                                                    cprod_code1, cprod_name FROM asaq.amendments_location WHERE ref2 = @userid GROUP BY cprod_code1 HAVING (SUM(old_data)-sum(new_data)) <> 0 ORDER BY processid DESC", conn);
                cmd.Parameters.AddWithValue("@userid", userid);
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    var o = new AmendmentsLocation
                    {
                        cprod_id = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "cprod_id")),
                        cprod_mast = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "cprod_mast")),
                        cprod_code1 = string.Empty + dr["cprod_code1"],
                        cprod_name = string.Empty + dr["cprod_name"],
                        old_data = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "old_data")),
                        new_data = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "new_data"))
                    };
                    results.Add(o);
                }
                dr.Close();
            }
            return results;
        }


        public static AmendmentsLocation GetProductAvailabilityChanges2(string cprod_code, int userid)
        {
            AmendmentsLocation result = new AmendmentsLocation();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = Utils.GetCommand(@"SELECT * FROM asaq.amendments_location WHERE cprod_code1 = @cprod_code AND ref2 = @userid ORDER BY processid DESC", conn);
                cmd.Parameters.AddWithValue("@cprod_code", cprod_code);
                cmd.Parameters.AddWithValue("@userid", userid);
                MySqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    result.cprod_code1 = string.Empty + dr["cprod_code1"];
                    result.cprod_name = string.Empty + dr["cprod_name"];
                    result.old_data = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "old_data"));
                    result.new_data = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "new_data"));
                    result.process = string.Empty + dr["process"];
                }
                dr.Close();
            }
            return result;
        }

        public static List<IndexOrderSummary> GetOrdersInHand(int userid)
        {
            var results = new List<IndexOrderSummary>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = Utils.GetCommand(@"SELECT * FROM asaq.index_order_summary WHERE userid1 = @userid AND (req_eta >= @datenow AND po_req_etd <= @datenow) and combined_order = 0 GROUP BY orderid 
                                                    ORDER BY req_eta,combined_order ASC", conn);
                cmd.Parameters.AddWithValue("@userid", userid);
                cmd.Parameters.AddWithValue("@datenow", DateTime.Now);
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    var o = new IndexOrderSummary
                    {
                        po_req_etd = Utilities.FromDbValue<DateTime>(Utilities.GetReaderField(dr, "po_req_etd")),
                        userid1 = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "userid1")),
                        orderid = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "orderid")),
                        invoice = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "invoice")),
                        custpo = string.Empty + dr["custpo"],
                        req_eta = Utilities.FromDbValue<DateTime>(Utilities.GetReaderField(dr, "req_eta")),
                        actual_eta = Utilities.FromDbValue<DateTime>(Utilities.GetReaderField(dr, "actual_eta")),
                        process_id = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "process_id")),
                        combined_order = Convert.ToBoolean(Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "combined_order"))),
                        shipped_from = string.Empty + dr["shipped_from"]
                    };
                    results.Add(o);
                }
                dr.Close();
            }
            return results;
        }

        public static List<OrderStatusLog2011> GetUpcomingPayments(int userid)
        {
            var results = new List<OrderStatusLog2011>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = Utils.GetCommand(@"SELECT * FROM asaq.2011_order_status_log4a WHERE userid1 = @userid AND status <> 'X' AND status <> 'Y' and payment = 0 and inv_amount > 0 
                                                    AND po_req_etd <= @datenow AND process_id > 200 GROUP BY orderid ORDER BY req_eta,combined_order ASC", conn);
                cmd.Parameters.AddWithValue("@userid", userid);
                cmd.Parameters.AddWithValue("@datenow", DateTime.Now);
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    var o = new OrderStatusLog2011
                    {
                        po_req_etd = Utilities.FromDbValue<DateTime>(Utilities.GetReaderField(dr, "po_req_etd")),
                        userid1 = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "userid1")),
                        orderid = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "orderid")),
                        invoice = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "invoice")),
                        custpo = string.Empty + dr["custpo"],
                        req_eta = Utilities.FromDbValue<DateTime>(Utilities.GetReaderField(dr, "req_eta")),
                        actual_eta = Utilities.FromDbValue<DateTime>(Utilities.GetReaderField(dr, "actual_eta")),
                        duedate2 = Utilities.FromDbValue<DateTime>(Utilities.GetReaderField(dr, "duedate2")),
                        process_id = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "process_id")),
                        inv_status = string.Empty + dr["inv_status"],
                        combined_order = Convert.ToBoolean(Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "combined_order"))),
                    };
                    results.Add(o);
                }
                dr.Close();
            }
            return results;
        }

        public static List<IndexOrderSummary> GetUpcomingDeliveries(int userid)
        {
            var results = new List<IndexOrderSummary>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = Utils.GetCommand(@"SELECT * FROM asaq.index_order_summary WHERE userid1 = @userid AND (req_eta >= @datenow AND po_req_etd <= @datenow) GROUP BY orderid  
                                                    ORDER BY req_eta,combined_order ASC", conn);
                cmd.Parameters.AddWithValue("@userid", userid);
                cmd.Parameters.AddWithValue("@datenow", DateTime.Now);
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    var o = new IndexOrderSummary
                    {
                        po_req_etd = Utilities.FromDbValue<DateTime>(Utilities.GetReaderField(dr, "po_req_etd")),
                        userid1 = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "userid1")),
                        orderid = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "orderid")),
                        invoice = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "invoice")),
                        custpo = string.Empty + dr["custpo"],
                        req_eta = Utilities.FromDbValue<DateTime>(Utilities.GetReaderField(dr, "req_eta")),
                        actual_eta = Utilities.FromDbValue<DateTime>(Utilities.GetReaderField(dr, "actual_eta")),
                        process_id = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "process_id")),
                        combined_order = Convert.ToBoolean(Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "combined_order"))),
                        shipped_from = string.Empty + dr["shipped_from"]
                    };
                    results.Add(o);
                }
                dr.Close();
            }
            return results;
        }

        public static List<OrderLineDetail> GetOrderLineDetails(int userid, int cprodid, bool history = false)
        {
            var results = new List<OrderLineDetail>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var query = string.Format("SELECT * FROM asaq.order_line_detail2_v6 WHERE cprod_id = @cprodid AND userid1 = @userid AND req_eta {0} @datenow AND orderqty > 0 order by po_req_etd DESC", history ? "<" : ">=");
                MySqlCommand cmd = Utils.GetCommand(query, conn);
                cmd.Parameters.AddWithValue("@userid", userid);
                cmd.Parameters.AddWithValue("@cprodid", cprodid);
                cmd.Parameters.AddWithValue("@datenow", DateTime.Now);
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    var o = new OrderLineDetail
                    {
                        po_req_etd = Utilities.FromDbValue<DateTime>(Utilities.GetReaderField(dr, "po_req_etd")),
                        userid1 = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "userid1")),
                        orderid = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "orderid")),
                        custpo = string.Empty + dr["custpo"],
                        req_eta = Utilities.FromDbValue<DateTime>(Utilities.GetReaderField(dr, "req_eta")),
                        combined_order = Convert.ToBoolean(Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "combined_order"))),
                        orderqty = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "orderqty"))
                    };
                    results.Add(o);
                }
                dr.Close();
            }
            return results;
        }

        public static List<BrandSalesAnalysisProduct2> GetBrandSalesAnalysisProduct2(int userid, string cprod_code1)
        {
            var results = new List<BrandSalesAnalysisProduct2>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = Utils.GetCommand(@"SELECT sum(orderqty) as orderqty, month21 FROM asaq.brand_sales_analysis_product2 WHERE cprod_code1=@cprod_code1 AND userid1 = @userid AND orderqty > 0 GROUP BY month21 order by month21 ASC", conn);
                cmd.Parameters.AddWithValue("@userid", userid);
                cmd.Parameters.AddWithValue("@cprod_code1", cprod_code1);
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    var o = new BrandSalesAnalysisProduct2
                    {
                        month21 = string.Empty + dr["month21"],
                        orderqty = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "orderqty"))
                    };
                    results.Add(o);
                }
                dr.Close();
            }
            return results;
        }

        public static List<ChangeNotice2012> Get2012ChangeNoticeCprod(int cprod_id)
        {
            var results = new List<ChangeNotice2012>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = Utils.GetCommand(@"SELECT DISTINCT cn_id,cn_details, cn_date FROM asaq.2012_change_notice_cprod WHERE cprod_id = @cprodid", conn);
                cmd.Parameters.AddWithValue("@cprodid", cprod_id);
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    var o = new ChangeNotice2012
                    {
                        cn_details = string.Empty + dr["cn_details"],
                        cn_id = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "cn_id")),
                        cn_date = Utilities.FromDbValue<DateTime>(Utilities.GetReaderField(dr, "cn_date"))
                    };
                    results.Add(o);
                }
                dr.Close();
            }
            return results;
        }

        public static List<Returns2Lite> GetAcceptedReturns(int userid, string cprod_code1)
        {
            var results = new List<Returns2Lite>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = Utils.GetCommand(@"SELECT request_date,return_no,return_qty,client_comments FROM asaq.returns2_lite WHERE cprod_code1 = @cprod_code1 AND status1 = 1 AND client_id = @userid 
                                                    AND request_date > now()-interval 1 year AND decision_final = 1 AND return_qty > 0 GROUP BY returnsid ORDER BY request_date DESC", conn);
                cmd.Parameters.AddWithValue("@cprod_code1", cprod_code1);
                cmd.Parameters.AddWithValue("@userid", userid);
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    var o = new Returns2Lite
                    {
                        return_no = string.Empty + dr["return_no"],
                        return_qty = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "return_qty")),
                        request_date = Utilities.FromDbValue<DateTime>(Utilities.GetReaderField(dr, "request_date")),
                        client_comments = string.Empty + dr["client_comments"]
                    };
                    results.Add(o);
                }
                dr.Close();
            }
            return results;
        }

        public static List<OrderLineDetail> GetSparesOrderLineDetails(int orderid)
        {
            var results = new List<OrderLineDetail>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = Utils.GetCommand("SELECT * FROM asaq.order_line_detail2_v2 WHERE orderid = @orderid and orderqty > 0 ORDER BY description ASC", conn);
                cmd.Parameters.AddWithValue("@orderid", orderid);
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    var o = new OrderLineDetail
                    {
                        cprod_code1 = string.Empty + dr["cprod_code1"],
                        orderqty = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "orderqty"))
                    };
                    results.Add(o);
                }
                dr.Close();
            }
            return results;
        }

        public static DistCustProductDetailIncDelete GetDistCustProductDetailIncDelete(int cprod_id)
        {
            DistCustProductDetailIncDelete result = new DistCustProductDetailIncDelete();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = Utils.GetCommand("SELECT * FROM asaq.dist_cust_product_details_inc_delete WHERE cprod_id = @cprod_id", conn);
                cmd.Parameters.AddWithValue("@cprod_id", cprod_id);

                MySqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    result.cprod_code1 = string.Empty + dr["cprod_code1"];
                    result.cprod_id = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "cprod_id"));
                    result.cprod_name = string.Empty + dr["cprod_name"];
                    result.cprod_spares = string.Empty + dr["cprod_spares"];
                }
                dr.Close();
            }
            return result;
        }

        public static List<DistCustProductDetailIncDelete> GetDistCustProductDetailIncDelete(int userid, string term = null, int? catid = null, int? brandid = null)
        {
            var results = new List<DistCustProductDetailIncDelete>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var brandstring = brandid != null ? brandid.Value == 32 ? " AND cprod_user = 42 AND cprod_cgflag= 1 " : brandid.Value == 42 ? " AND cprod_user = 42 AND cprod_cgflag= 0 " : " AND cprod_user=@brandid" : "";
                brandstring = !string.IsNullOrEmpty(term) ? "" : brandstring;
                var catstring = catid != null ? " AND cprod_brand_cat = @catid" : "";
                var query = string.Format("SELECT * FROM asaq.dist_cust_product_details_inc_delete WHERE (cprod_code1 like @term OR cprod_name like @term) AND client_id = @userid {0} {1} and cprod_brand_cat NOT IN(203,108,305,707,608,913) order by cprod_brand_cat,cprod_seq,cprod_code1 ASC", brandstring, catstring);
                MySqlCommand cmd = Utils.GetCommand(query, conn);
                cmd.Parameters.AddWithValue("@userid", userid);
                cmd.Parameters.AddWithValue("@term", string.Format("%{0}%", term));
                if (brandid != null)
                    cmd.Parameters.AddWithValue("@brandid", brandid.Value);
                if (catid != null)
                    cmd.Parameters.AddWithValue("@catid", catid.Value);
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    var o = new DistCustProductDetailIncDelete
                    {
                        cprod_code1 = string.Empty + dr["cprod_code1"],
                        cprod_id = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "cprod_id")),
                        cprod_name = string.Empty + dr["cprod_name"],
                    };
                    results.Add(o);
                }
                dr.Close();
            }
            return results;
        }

        public static List<Spares2> GetAllSpares2()
        {
            var results = new List<Spares2>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = Utils.GetCommand("SELECT * FROM asaq.spares2", conn);
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    var o = new Spares2
                    {
                        cprod_code1 = string.Empty + dr["cprod_code1"],
                        product_cprod = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "product_cprod")),
                        cprod_id = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "cprod_id")),
                        cprod_retail = Utilities.FromDbValue<double>(Utilities.GetReaderField(dr, "cprod_retail")),
                        moq = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "moq")),
                        discount_ddp_cash_40 = Utilities.FromDbValue<double>(Utilities.GetReaderField(dr, "discount_ddp_cash_40")),
                        discount_ddp_credit_40 = Utilities.FromDbValue<double>(Utilities.GetReaderField(dr, "discount_ddp_credit_40")),
                        dist_spec_disc = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "dist_spec_disc")),
                        spare_desc = string.Empty + dr["spare_desc"],
                        cprod_name = string.Empty + dr["cprod_name"],
                        prod_image1 = string.Empty + dr["prod_image1"],
                        units_per_carton = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "units_per_carton"))
                    };
                    results.Add(o);
                }
                dr.Close();
            }
            return results;
        }

        public static List<Spares2> GetAllSpares3(int userid)
        {
            var results = new List<Spares2>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = Utils.GetCommand("SELECT * FROM asaq.spares3 WHERE client_id = @userid", conn);
                cmd.Parameters.AddWithValue("@userid", userid);
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    var o = new Spares2
                    {
                        cprod_code1 = string.Empty + dr["cprod_code1"],
                        product_cprod = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "product_cprod")),
                        cprod_id = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "cprod_id")),
                        cprod_retail = Utilities.FromDbValue<double>(Utilities.GetReaderField(dr, "cprod_retail")),
                        moq = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "moq")),
                        discount_ddp_cash_40 = Utilities.FromDbValue<double>(Utilities.GetReaderField(dr, "discount_ddp_cash_40")),
                        discount_ddp_credit_40 = Utilities.FromDbValue<double>(Utilities.GetReaderField(dr, "discount_ddp_credit_40")),
                        dist_spec_disc = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "dist_spec_disc")),
                        spare_desc = string.Empty + dr["spare_desc"],
                        cprod_name = string.Empty + dr["cprod_name"],
                        prod_image1 = string.Empty + dr["prod_image1"],
                        units_per_carton = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "units_per_carton"))
                    };
                    results.Add(o);
                }
                dr.Close();
            }
            return results;
        }

        public static List<ReturnsProductListings> GetReturnProductListings(int userid, int? cprod_id = null, string term = null)
        {
            var results = new List<ReturnsProductListings>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var client_specific = userid == 283 ? " (userid1 = 79 OR userid1 = 283) " : userid == 322 ? " (userid1 = 79 OR userid1 = 322) " : " userid1 = @userid ";
                var hardcode_user = userid == 208 || userid == 275 ? " AND cprod_user = 81 " : "";
                var hardcode_prod = userid == 211 ? "" : " AND cprod_id <> 6000 ";
                var query = string.Empty;
                if (cprod_id != null)
                    query = string.Format("SELECT DISTINCT cprod_code1, cprod_name, cprod_id, consolidated_port FROM asaq.returns_products_listing WHERE cprod_id = @cprod_id AND {0} ", client_specific);
                else
                    query = string.Format(@"SELECT DISTINCT cprod_code1, cprod_name, cprod_id, consolidated_port FROM asaq.returns_products_listing WHERE (cprod_code1 like @term OR cprod_name like @term) AND {0} 
                                            AND cprod_id <> 1069 AND cprod_id <> 1070 and cprod_id <> 1068 AND (cprod_id <> 1672 OR userid1 = 238) {1}{2} ", client_specific, hardcode_user, hardcode_prod);
                MySqlCommand cmd = Utils.GetCommand(query, conn);
                if (userid != 283 && userid != 322)
                    cmd.Parameters.AddWithValue("@userid", userid);
                if (cprod_id != null)
                    cmd.Parameters.AddWithValue("@cprod_id", cprod_id.Value);
                if (!string.IsNullOrEmpty(term) && cprod_id == null)
                    cmd.Parameters.AddWithValue("@term", string.Format("%{0}%", term));
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    var o = new ReturnsProductListings
                    {
                        cprod_code1 = string.Empty + dr["cprod_code1"],
                        cprod_id = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "cprod_id")),
                        cprod_name = string.Empty + dr["cprod_name"],
                        consolidated_port = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "consolidated_port"))
                    };
                    results.Add(o);
                }
                dr.Close();
            }
            return results;
        }

        public static List<ReturnsV2> GetAllReturnsV2(int userid, int useruserid, string month21 = null)
        {
            var results = new List<ReturnsV2>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var useridhardcode = useruserid != 156 && useruserid != 339 && useruserid != 380 && useruserid != 375 && useruserid != 377 ? " and request_userid <> 156 and request_userid <> 339 and request_userid <> 380 and request_userid <> 375 and request_userid <> 377 " : " OR request_userid IN(156,339,375,377,380) ";
                var query = string.Format(@"SELECT DISTINCT reference, cprod_name, cprod_code1, request_date, status1, returnsid, client_id, return_no, customer_code,factory_code, factory_ref, 
                                            factory_id, decision, decision_final, cprod_user, cc_response_date, daysdiff, sort4, user_id, openclosed, decision_flag, flagged, flagged_reason 
                                            FROM asaq.returns_v2 WHERE (client_id = @userid OR client_id = @userid OR request_userid = @useruserid {0}) and status1 <> 2 and claim_type = 5
                                            ORDER BY openclosed, sort4,status1, flagged, user_id  ASC", useridhardcode);
                MySqlCommand cmd = Utils.GetCommand(query, conn);
                cmd.Parameters.AddWithValue("@userid", userid);
                cmd.Parameters.AddWithValue("@useruserid", useruserid);
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    var o = new ReturnsV2
                    {
                        cprod_code1 = string.Empty + dr["cprod_code1"],
                        decision_final = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "decision_final")),
                        status1 = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "status1")),
                        daysdiff = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "daysdiff")),
                        returnsid = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "returnsid")),
                        client_id = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "client_id")),
                        request_date = Utilities.FromDbValue<DateTime>(Utilities.GetReaderField(dr, "request_date")),
                        openclosed = Utilities.FromDbValue<double>(Utilities.GetReaderField(dr, "openclosed")),
                        cprod_name = string.Empty + dr["cprod_name"],
                        sort4 = string.Empty + dr["sort4"],
                        flagged = string.Empty + dr["flagged"],
                        flagged_reason = string.Empty + dr["flagged_reason"],
                        reference = string.Empty + dr["reference"],
                        return_no = string.Empty + dr["return_no"]
                    };
                    results.Add(o);
                }
                dr.Close();
            }
            return results;
        }

        public static List<OrderLinesDetail7Graph> GetAllOrderLineDetail7Graph(int userid, int window, int? period = null)
        {
            var results = new List<OrderLinesDetail7Graph>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var query = string.Empty;
                switch(window)
                {
                    case 1:
                        query = "SELECT userid1, orderqty, unitprice, unitcurrency, orderid, month23, po_req_etd, cprod_user FROM asaq.order_lines_detail7_graphs WHERE userid1 = @userid and category1 <> 13 GROUP by orderid";
                        break;
                    case 2:
                    case 7:
                        query = "SELECT userid1, orderqty, unitprice, unitcurrency, orderid, month23, po_req_etd, cprod_user FROM asaq.order_lines_detail7_graphs WHERE cprod_user = @userid AND userid1 <> 184 AND userid1 <> 227 and category1 <> 13 order by orderid ASC";
                        break;
                    case 3:
                        query = string.Format("SELECT userid1, orderqty, unitprice, unitcurrency, orderid, month23, po_req_etd, cprod_user FROM asaq.order_lines_detail7_graphs WHERE STOCK_ORDER <> 1 and STOCK_ORDER <> 2 and cprod_user = @userid and category1 <> 13 AND userid1 <> 184 AND userid1 <> 227 {0} order by orderid ASC ",userid == 53 ?  "AND custpo not like \"IR%\"" : "");
                        break;
                    default:
                        query = string.Format("SELECT custpo, orderdate, max(po_req_etd) as po_req_etd, orderid FROM asaq.order_lines_detail7_graphs WHERE orderid > 0 {0} AND userid1 = @userid and category1 <> 13 group by orderid order by orderid ASC", period != null && period == 0 ? " AND po_req_ETD > now() " : " AND month23 = " + ((DateTime.Now.Year - 2000) * 100 + DateTime.Now.Month));
                        break;
                }
                MySqlCommand cmd = Utils.GetCommand(query, conn);
                cmd.Parameters.AddWithValue("@userid", userid);
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    var o = new OrderLinesDetail7Graph
                    {
                        userid1 = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "userid1")),
                        orderqty = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "orderqty")),
                        unitprice = Utilities.FromDbValue<double>(Utilities.GetReaderField(dr, "unitprice")),
                        unitcurrency = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "unitcurrency")),
                        orderid = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "orderid")),
                        month23 = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "month23")),
                        po_req_etd = Utilities.FromDbValue<DateTime>(Utilities.GetReaderField(dr, "po_req_etd")),
                        cprod_user = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "cprod_user")),
                        custpo = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "custpo")),
                        orderdate = Utilities.FromDbValue<DateTime>(Utilities.GetReaderField(dr, "orderdate"))
                    };
                    results.Add(o);
                }
                dr.Close();
            }
            return results;
        }

        public static List<ReturnsV2> GetAllReturnsForHome(int userid, int window)
        {
            var results = new List<ReturnsV2>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var query = string.Empty;
                switch (window)
                {
                    case 1:
                        query = "SELECT decision_final,openclosed,request_date,decision,cc_response_date,returns_comments_discussion2.User_id FROM asaq.returns LEFT JOIN returns_comments_discussion2 on((returns.returnsid = returns_comments_discussion2.return_id)) WHERE client_id = @userid AND status1 = 1 and claim_type <> 5 AND request_date >= @date";
                        break;
                    case 2:
                        query = "SELECT client_id, decision_final, returnsid, claim_value, credit_value, return_qty, month21, month22, request_date, openclosed FROM asaq.returns2 WHERE client_id <> 184 and client_id <> 227 AND cprod_user = @userid AND status1 = 1 and claim_type <> 5 AND request_date >= @date GROUP BY client_id, returnsid order by returnsid ASC ";
                        break;
                    case 3:
                        query = "SELECT client_id, decision_final, returnsid, claim_value, credit_value, return_qty, month21, month22, request_date, openclosed FROM asaq.returns2 WHERE client_id <> 184 and client_id <> 227 AND cprod_user = @userid AND status1 = 1 AND request_date >= @date GROUP BY client_id, returnsid order by returnsid ASC ";
                        break;
                    default:
                        break;
                }
                MySqlCommand cmd = Utils.GetCommand(query, conn);
                cmd.Parameters.AddWithValue("@userid", userid);
                cmd.Parameters.AddWithValue("@date", DateTime.Now.AddYears(-1));
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    var o = new ReturnsV2
                    {
                        decision_final = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "decision_final")),
                        request_date = Utilities.FromDbValue<DateTime>(Utilities.GetReaderField(dr, "request_date")),
                        openclosed = Utilities.FromDbValue<double>(Utilities.GetReaderField(dr, "openclosed")),
                        decision = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "decision")),
                        cc_response_date = Utilities.FromDbValue<DateTime>(Utilities.GetReaderField(dr, "cc_response_date")),
                    };

                    if (window == 2 || window == 3)
                    {
                        o.month21 = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "month21"));
                        o.month22 = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "month22"));
                        o.credit_value = Utilities.FromDbValue<double>(Utilities.GetReaderField(dr, "credit_value"));
                        o.claim_value = Utilities.FromDbValue<double>(Utilities.GetReaderField(dr, "claim_value"));
                        o.return_qty = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "return_qty"));
                        o.client_id = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "client_id"));
                    }
                    else
                    {
                        o.user_id = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "user_id"));
                    }
                        
                    results.Add(o);
                }
                dr.Close();
            }
            return results;
        }

        public static List<Standards2> GetStandards2()
        {
            var results = new List<Standards2>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = Utils.GetCommand(@"SELECT * from standards2", conn);
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    var o = new Standards2
                    {
                        std_id = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "std_id")),
                        std_code = string.Empty + dr["std_code"],
                        std_description = string.Empty + dr["std_description"],
                        cat1_name = string.Empty + dr["cat1_name"],
                        cat2_desc = string.Empty + dr["cat2_desc"],
                        cat2_desc2 = string.Empty + dr["cat2_desc2"]
                    };
                    results.Add(o);
                }
                dr.Close();
            }
            return results;
        }

        public static List<StandardsDetail> GetStandardsDetails(int id, string type)
        {
            var results = new List<StandardsDetail>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = Utils.GetCommand(@"SELECT * FROM standards_detail WHERE std_id = @id AND std_detail_type = @type AND sub_link = 0 order by std_id, std_detail_seq, std_detail_unique  ASC", conn);
                cmd.Parameters.AddWithValue("@id",id);
                cmd.Parameters.AddWithValue("@type",type);
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    var o = new StandardsDetail
                    {
                        std_id = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "std_id")),
                        std_code = string.Empty + dr["std_code"],
                        std_description = string.Empty + dr["std_description"],
                        std_detail_header = string.Empty + dr["std_detail_header"],
                        std_detail_seq = string.Empty + dr["std_detail_seq"],
                        std_detail_text = string.Empty + dr["std_detail_text"],
                        std_detail_text2 = string.Empty + dr["std_detail_text2"],
                        std_detail_text3 = string.Empty + dr["std_detail_text3"],
                        std_detail_type = string.Empty + dr["std_detail_type"],
                        std_dimension_prefix = string.Empty + dr["std_dimension_prefix"],
                        std_image = string.Empty + dr["std_image"],
                        std_image_left = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "std_image_left")),
                        std_image2 = string.Empty + dr["std_image2"],
                        std_symbol = string.Empty + dr["std_symbol"],
                        std_positive = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "std_positive")),
                        std_detail_unique = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "std_detail_unique")),
                        std_dimension = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "std_dimension")),
                        std_negative = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "std_negative")),
                        rowspan = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "rowspan")),
                        definition_flag = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "definition_flag")),
                        examination_method = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "examination_method")),
                        inspection_valid = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "inspection_valid")),
                        sub_link = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "sub_link"))
                    };
                    results.Add(o);
                }
                dr.Close();
            }
            return results;
        }

        public static List<StandardsDetail> GetStandardsDetailsForCategory(int cat1, int cat2, string type)
        {
            var results = new List<StandardsDetail>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = Utils.GetCommand(@"SELECT * FROM asaq.standards_detail WHERE ((std_category = @cat1 AND std_sub_category = 0) OR std_sub_category = @cat2 OR std_sub_category2 = @cat2 OR std_sub_category3 = @cat2) AND std_detail_type = @type AND sub_link = 0 order by std_id, std_detail_seq  ASC", conn);
                cmd.Parameters.AddWithValue("@cat1", cat1);
                cmd.Parameters.AddWithValue("@cat2", cat2);
                cmd.Parameters.AddWithValue("@type", type);
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    var o = new StandardsDetail
                    {
                        std_id = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "std_id")),
                        std_code = string.Empty + dr["std_code"],
                        std_description = string.Empty + dr["std_description"],
                        std_detail_header = string.Empty + dr["std_detail_header"],
                        std_detail_seq = string.Empty + dr["std_detail_seq"],
                        std_detail_text = string.Empty + dr["std_detail_text"],
                        std_detail_text2 = string.Empty + dr["std_detail_text2"],
                        std_detail_text3 = string.Empty + dr["std_detail_text3"],
                        std_detail_type = string.Empty + dr["std_detail_type"],
                        std_dimension_prefix = string.Empty + dr["std_dimension_prefix"],
                        std_image = string.Empty + dr["std_image"],
                        std_image_left = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "std_image_left")),
                        std_image2 = string.Empty + dr["std_image2"],
                        std_symbol = string.Empty + dr["std_symbol"],
                        std_positive = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "std_positive")),
                        std_detail_unique = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "std_detail_unique")),
                        std_dimension = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "std_dimension")),
                        std_negative = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "std_negative")),
                        rowspan = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "rowspan")),
                        definition_flag = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "definition_flag")),
                        examination_method = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "examination_method")),
                        inspection_valid = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "inspection_valid")),
                        sub_link = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "sub_link"))
                    };
                    results.Add(o);
                }
                dr.Close();
            }
            return results;
        }

        public static List<StandardsDetail> GetAllStandardsDetails()
        {
            var results = new List<StandardsDetail>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = Utils.GetCommand(@"SELECT * FROM standards_detail", conn);
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    var o = new StandardsDetail()
                    {
                        std_id = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "std_id")),
                        std_code = string.Empty + dr["std_code"],
                        std_description = string.Empty + dr["std_description"],
                        std_detail_header = string.Empty + dr["std_detail_header"],
                        std_detail_seq = string.Empty + dr["std_detail_seq"],
                        std_detail_text = string.Empty + dr["std_detail_text"],
                        std_detail_text2 = string.Empty + dr["std_detail_text2"],
                        std_detail_text3 = string.Empty + dr["std_detail_text3"],
                        std_detail_type = string.Empty + dr["std_detail_type"],
                        std_dimension_prefix = string.Empty + dr["std_dimension_prefix"],
                        std_image = string.Empty + dr["std_image"],
                        std_image_left = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "std_image_left")),
                        std_image2 = string.Empty + dr["std_image2"],
                        std_symbol = string.Empty + dr["std_symbol"],
                        std_positive = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "std_positive")),
                        std_detail_unique = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "std_detail_unique")),
                        std_dimension = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "std_dimension")),
                        std_negative = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "std_negative")),
                        rowspan = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "rowspan")),
                        definition_flag = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "definition_flag")),
                        examination_method = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "examination_method")),
                        inspection_valid = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "inspection_valid")),
                        sub_link = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "sub_link"))
                    };
                    results.Add(o);
                }
                dr.Close();
            }
            return results;
        }

        public static List<Categories2> GetCategories2()
        {
            var results = new List<Categories2>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = Utils.GetCommand(@"SELECT * from categories2", conn);
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    var o = new Categories2
                    {
                        cat2_code = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "cat2_code")),
                        cat1_name = string.Empty + dr["cat1_name"],
                        cat2_desc = string.Empty + dr["cat2_desc"],
                    };
                    results.Add(o);
                }
                dr.Close();
            }
            return results;
        }

        public static aql_clientsub GetAQLClientSub(int cat2)
        {
            aql_clientsub result = null;
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = Utils.GetCommand(@"SELECT * FROM asaq.aql_clientsub WHERE subcat_id = @cat2 AND client_id = 0", conn);
                cmd.Parameters.AddWithValue("@cat2", cat2);
                MySqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    result = new aql_clientsub
                    {
                        unique_aql_sub = (int)dr["unique_aql_sub"]
                    };
                }
                dr.Close();
            }
            return result;
        }

        public static List<product_criteria_subs> GetAllProductCriteriaSubs()
        {
            var results = new List<product_criteria_subs>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = Utils.GetCommand(@"SELECT * FROM asaq.product_criteria_subs", conn);
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    var o = new product_criteria_subs()
                    {
                        prod_criteria_unique = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "prod_criteria_unique")),
                        std_dimension = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "std_dimension")),
                        std_positive = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "std_positive")),
                        prod_cprod_id = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "prod_cprod_id")),
                        std_detail_id = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "std_detail_id")),
                    };
                    results.Add(o);
                    
                }
                dr.Close();
            }
            return results;
        }

        public static List<AdminFactories> GetAllAdminFactories()
        {
            var results = new List<AdminFactories>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = Utils.GetCommand(@"SELECT * FROM asaq.admin_factories", conn);
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    var o = new AdminFactories()
                    {
                        user_id = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "user_id")),
                        userid = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "userid")),
                        permission_id = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "permission_id")),
                        userusername = string.Empty + dr["userusername"]
                    };
                    results.Add(o);
                    
                }
                dr.Close();
            }
            return results;
        }

        public static List<returns_stats1_client_6months> GetAllReurnsStatsClient6Months()
        {
            var results = new List<returns_stats1_client_6months>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = Utils.GetCommand(@"SELECT * FROM returns_stats2_client_6months WHERE orderqty > 0 AND customer_code <> 'te' AND not isnull(total_returns)", conn);
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    var o = new returns_stats1_client_6months()
                    {
                        total_returns = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "total_returns")),
                        userid1 = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "userid1")),
                        GBP_returns = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "GBP_returns")),
                        customer_code = string.Empty + dr["customer_code"],
                        orderqty = Utilities.FromDbValue<double>(Utilities.GetReaderField(dr, "orderqty")),
                        GBP = Utilities.FromDbValue<double>(Utilities.GetReaderField(dr, "GBP"))
                    };
                    results.Add(o);

                }
                dr.Close();
            }
            return results;
        }

        public static List<brand_sales_analysis> GetAllBrandSalesAnalysis(int month21, int type = 1, int? cprod_user = null, int? cusid = null, int? brand = null)
        {
            var results = new List<brand_sales_analysis>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var query = string.Empty;
                if (type == 1)
                    query = string.Format("SELECT * FROM asaq.brand_sales_analysis2 WHERE unitcurrency = 1 AND customer_code <> 'te' {0} {1} AND month21 >= @month21 and category1 <> 13 group by cprod_user order by cprod_user ASC",brand != null ? " AND cprod_user=@cprod_user " : "", cusid != null ? " AND userid1=@cusid ":"");
                else
                    query = string.Format("SELECT sum(rowprice) as rowprice, customer_code, month21, unitcurrency FROM asaq.brand_sales_analysis2 WHERE {0} {1} AND month21 >= @month21 and category1 <> 13 group by month21 order by month21 ASC", brand != null ? " cprod_user=@cprod_user " : "", cusid != null ? " AND userid1=@cusid " : "");
                MySqlCommand cmd = Utils.GetCommand(query, conn);
                if (cusid != null)
                    cmd.Parameters.AddWithValue("@cusid", cusid.Value);
                if (cprod_user != null)
                    cmd.Parameters.AddWithValue("@cprod_user",cprod_user.Value);
                cmd.Parameters.AddWithValue("@month21", month21);
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    var o = new brand_sales_analysis()
                    {
                        cprod_user = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "cprod_user")),
                        month21 = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "month21")),
                        unitcurrency = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "unitcurrency")),
                        customer_code = string.Empty + dr["customer_code"],
                        rowprice = Utilities.FromDbValue<double>(Utilities.GetReaderField(dr, "rowprice"))
                    };
                    results.Add(o);

                }
                dr.Close();
            }
            return results;
        }

        public static List<returns_v4> GetAllPendingClaims(int month21, int? cusid, string type = null)
        {
            var results = new List<returns_v4>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                
                var specifications = " AND claim_type <> 5 AND ((decision_final = 0 AND (user_id <> 1 OR isnull(user_id))) OR (decision_final = 999 AND daysdiff < 28 AND user_id <> 1 AND user_id <> 42)) AND decision <> 999 ";
                var clientspec = cusid != null ? " AND client_id = " + cusid.Value : "";
                switch (type)
                {
                    case "respond":
                        specifications = " AND claim_type <> 5 AND (decision_final = 0 OR decision_final = 999) AND daysdiff < 28 AND user_id <> 1 AND user_id <> 42 AND decision <> 999 ";
                        break;
                    case "accepted":
                        specifications = " AND claim_type <> 5 AND decision_final = 1 AND month21 = @month21 ";
                        break;
                    case "declined":
                        specifications = " AND claim_type <> 5 AND decision_final = 999 AND month21 = @month21 ";
                        break;
                    case "replacement":
                        specifications = " AND claim_type <> 5 AND decision_final = 500 AND month21 = @month21 ";
                        break;
                    default:
                        specifications = " AND claim_type <> 5 AND ((decision_final = 0 AND (user_id <> 1 OR isnull(user_id))) OR (decision_final = 999 AND daysdiff < 28 AND user_id <> 1 AND user_id <> 42)) AND decision <> 999 ";
                        break;
                }
                var query = string.Format("SELECT * FROM asaq.returns_v4 WHERE openclosed = 0 AND status1 = 1 and client_id <> 184 {0} {1}  ORDER BY claim_type, comments_date ASC",specifications, clientspec);
                MySqlCommand cmd = Utils.GetCommand(query, conn);
                cmd.Parameters.AddWithValue("@month21", month21);
                if(cusid != null)
                    cmd.Parameters.AddWithValue("@cusid", cusid);
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    var o = new returns_v4
                    {
                        claim_type = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "claim_type")),
                        decision_final = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "decision_final")),
                        daysdiff = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "daysdiff")),
                        user_id = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "user_id")),
                        highlight = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "highlight")),
                        status1 = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "status1")),
                        returnsid = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "returnsid")),
                        client_id = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "client_id")),
                        comments_date = Utilities.FromDbValue<DateTime>(Utilities.GetReaderField(dr, "comments_date")),
                        request_date = Utilities.FromDbValue<DateTime>(Utilities.GetReaderField(dr, "request_date")),
                        factory_code = string.Empty + dr["factory_code"],
                        customer_code = string.Empty + dr["customer_code"],
                        return_no = string.Empty + dr["return_no"],
                        cprod_code1 = string.Empty + dr["cprod_code1"]
                    };
                    results.Add(o);
                }
                dr.Close();
            }
            return results;
        }

        public static List<order_status_log> GetAllOrderStatusLogByClient(int userid)
        {
            var results = new List<order_status_log>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = Utils.GetCommand(@"SELECT DISTINCT factory_code, factory_id FROM asaq.order_status_log1 WHERE userid1 = @userid AND status <> 'X' AND status <> 'Y' ORDER BY factory_code ASC ", conn);
                cmd.Parameters.AddWithValue("@userid", userid);
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    var o = new order_status_log()
                    {
                        factory_id = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "factory_id")),
                        factory_code = string.Empty + dr["factory_code"]
                    };
                    results.Add(o);
                }
                dr.Close();
            }
            return results;
        }

        public static List<order_status_log> GetAllOrderStatusLogByClient(int userid, string statusstring, string posearch, string orderby)
        {
            var results = new List<order_status_log>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = Utils.GetCommand(string.Format(@"SELECT * FROM asaq.2011_order_status_log4b WHERE userid1 = @userid {0} AND status <> 'X' {1} {2}", statusstring, posearch, orderby), conn);
                cmd.Parameters.AddWithValue("@userid", userid);
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    var o = new order_status_log()
                    {
                        factory_id = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "factory_id")),
                        factory_code = string.Empty + dr["factory_code"],
                        process_id = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "process_id")),
                        invoice = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "invoice")),
                        orderid = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "orderid")),
                        userid1 = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "userid1")),
                        loading_factory = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "loading_factory")),
                        custpo = string.Empty + dr["custpo"],
                        po_req_etd = Utilities.FromDbValue<DateTime>(Utilities.GetReaderField(dr, "po_req_etd")),
                        orderdate = Utilities.FromDbValue<DateTime>(Utilities.GetReaderField(dr, "orderdate")),
                        customer_code = string.Empty + dr["customer_code"],
                        notes = string.Empty + dr["notes"],
                        status = string.Empty + dr["status"],
                        porderid = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "porderid")),
                        inv_status = string.Empty + dr["inv_status"],
                        customs_deduction = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "customs_deduction")),
                        po_brief = string.Empty + dr["po_brief"],
                        so_brief = string.Empty + dr["so_brief"],
                        po_process_id = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "po_process_id")),
                        combined_order = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "combined_order")),
                        stock_order = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "stock_order"))
                    };
                    results.Add(o);
                }
                dr.Close();
            }
            return results;
        }

        public static List<order_status_log> GetFactoriesFromOrderLines(int orderid)
        {
            var results = new List<order_status_log>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = Utils.GetCommand(@"SELECT order_lines.orderid, mast_products.factory_id, users.factory_code FROM order_lines INNER JOIN cust_products ON order_lines.cprod_id = cust_products.cprod_id INNER JOIN mast_products ON cust_products.cprod_mast = mast_products.mast_id INNER JOIN users ON mast_products.factory_id = users.user_id WHERE orderid = @orderid and orderqty > 0 group by orderid, factory_id", conn);
                cmd.Parameters.AddWithValue("@orderid", orderid);
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    var o = new order_status_log()
                    {
                        factory_id = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "factory_id")),
                        factory_code = string.Empty + dr["factory_code"]
                    };
                    results.Add(o);
                }
                dr.Close();
            }
            return results;
        }
        
    }
        
}
