
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace asaq2.Model
{
    public class ReturnsDAL
	{
	
		public static List<Returns> GetAll()
		{
			var result = new List<Returns>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT * FROM returns", conn);
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
                var cmd = new MySqlCommand(@"SELECT returns.*,cust_products.* FROM returns INNER JOIN cust_products ON returns.cprod_id = cust_products.cprod_id 
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

        public static List<Returns> GetForClaimType(int claim_type)
        {
            var result = new List<Returns>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand(@"SELECT returns.*,(SELECT COUNT(*) FROM returns_comments WHERE return_id = returns.returnsid) AS no_of_comments
                                        FROM returns 
                                        WHERE claim_type = @claim_type", conn);
                cmd.Parameters.AddWithValue("@claim_type", claim_type);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    var r = GetFromDataReader(dr);
                    
                    r.HasComments = Convert.ToInt64(dr["no_of_comments"]) > 0;
                    //r.Product = Cust_productsDAL.GetFromDataReader(dr);
                    result.Add(r);
                }
                dr.Close();
            }
            return result;
        }
		
		
		public static Returns GetById(int id)
		{
			Returns result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT * FROM returns WHERE returnsid = @id", conn);
				cmd.Parameters.AddWithValue("@id", id);
                var dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    result = GetFromDataReader(dr);
                    if (result.feedback_category_id != null)
                        result.Category = Feedback_categoryDAL.GetById(result.feedback_category_id.Value);
                    if (result.request_userid != null)
                        result.Creator = UserDAL.GetById(result.request_userid.Value);
                    result.Images = Returns_imagesDAL.GetByReturn(id);
                    result.Comments = Returns_commentsDAL.GetByReturn(id);
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
                var cmd = new MySqlCommand("SELECT COUNT(*) FROM returns WHERE client_id = @id AND (request_date BETWEEN @date AND (@date + INTERVAL 1 day - INTERVAL 1 second) OR @date IS NULL) AND (claim_type = @claim_type OR @claim_type IS NULL)", conn);
                cmd.Parameters.AddWithValue("@id", company_id);
                cmd.Parameters.AddWithValue("@date", date != null ? (object) date : DBNull.Value);
                cmd.Parameters.AddWithValue("@claim_type", claim_type != null ? (object) claim_type : DBNull.Value);
                result = Convert.ToInt32(cmd.ExecuteScalar());
            }
            return result;
        }

        public static ReturnAggregateData GetQtyInPeriod(string cprod_code1, int company_id, DateTime? dateFrom = null, DateTime? dateTo = null)
        {
            var result = new ReturnAggregateData();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(@"SELECT COUNT(*) AS qty FROM returns INNER JOIN cust_products ON  returns.cprod_id = cust_products.cprod_id
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

        public static List<ReturnAggregateDataClientPrice> GetTotalsPerClient(DateTime? dateFrom = null, DateTime? dateTo = null, int? brand_user_id = null,IList<int> incClients = null, IList<int> exClients = null)
        {
            var result = new List<ReturnAggregateDataClientPrice>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand("", conn);
                cmd.CommandText = string.Format(@"SELECT users.user_id,users.customer_code, decision_final,claim_type, 
                                    SUM((CASE WHEN claim_type = 2 THEN claim_value ELSE return_qty * credit_value END)) AS Total,
                                    SUM(CASE ebuk WHEN 1 THEN (CASE WHEN claim_type = 2 THEN claim_value ELSE return_qty * credit_value END) ELSE 0 END) AS TotalEBUK
                                    FROM returns INNER JOIN users ON returns.client_id = users.user_id INNER JOIN cust_products ON returns.cprod_id = cust_products.cprod_id
                                    WHERE status1 = 1 AND (request_date >= @dateFrom OR @dateFrom IS NULL) AND
                                    (request_date <= @dateTo OR @dateTo IS NULL) AND decision_final IN (1,999,500) AND claim_type IS NOT NULL AND claim_type <> 5
                                    AND (cust_products.brand_user_id = @userid OR @userid IS NULL) {0}
                                    GROUP BY users.user_id,users.customer_code, decision_final,claim_type
                                    ORDER BY users.user_id,users.customer_code, decision_final,claim_type",
                                      AnalyticsDAL.HandleClients(cmd,"client_id",incClients,exClients));
                
                cmd.Parameters.AddWithValue("@dateFrom", dateFrom != null ? (object)dateFrom : DBNull.Value);
                cmd.Parameters.AddWithValue("@dateTo", dateTo != null ? (object)dateTo : DBNull.Value);
                cmd.Parameters.AddWithValue("@userid", brand_user_id != null ? (object) brand_user_id : DBNull.Value);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    string code = string.Empty + dr["customer_code"];
                    int claim_type = (int) dr["claim_type"];
                    var data = result.FirstOrDefault(r => r.customer_code == code && r.claim_type == claim_type);
                    if (data == null)
                    {
                        data = new ReturnAggregateDataClientPrice {customer_code = code,claim_type = claim_type};
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
			return o;

		}
		
		
		public static void Create(Returns o)
        {
            string insertsql = @"INSERT INTO returns (return_no,client_id,request_date,request_user,request_userid,cprod_id,return_qty,custpo,custpo_certainty,custpo_estimate,order_id,status1,
                            client_comments,client_comments2,client_comments3,fc_comments,factory_comments,agent_comments,cc_comments,fc_response_date,cc_response_date,agent_response_date,
                            closed_date,reason,brand,reference,factory_decision,factory_decision_date,decision,decision_final,credit_po,credit_value,credit_value_override,claim_type,claim_value,
                            warning_flag,openclosed,awaiting_user,flagged,flagged_reason,importance,highlight,std_unique_id,resolution,ip_address,delaminating,quote1,quote1_price,quote1a,
                            quote1a_price,quote1b,quote1b_price,quote1c,quote1c_price,quote2,quote2_price,quote3,quote3_price,quotechoice,factory_reason,spec_code1,spec_name,fc_po_sufficient,
                            fc_evidence_sufficient,fc_evidence_required,fc_evidence_file,fc_acceptance,fc_product_change_description,fc_cnid,feedback_category_id) 
                            VALUES(@return_no,@client_id,@request_date,@request_user,@request_userid,@cprod_id,@return_qty,@custpo,@custpo_certainty,@custpo_estimate,@order_id,@status1,
                            @client_comments,@client_comments2,@client_comments3,@fc_comments,@factory_comments,@agent_comments,@cc_comments,@fc_response_date,@cc_response_date,
                            @agent_response_date,@closed_date,@reason,@brand,@reference,@factory_decision,@factory_decision_date,@decision,@decision_final,@credit_po,@credit_value,
                            @credit_value_override,@claim_type,@claim_value,@warning_flag,@openclosed,@awaiting_user,@flagged,@flagged_reason,@importance,@highlight,@std_unique_id,
                            @resolution,@ip_address,@delaminating,@quote1,@quote1_price,@quote1a,@quote1a_price,@quote1b,@quote1b_price,@quote1c,@quote1c_price,@quote2,@quote2_price,
                            @quote3,@quote3_price,@quotechoice,@factory_reason,@spec_code1,@spec_name,@fc_po_sufficient,@fc_evidence_sufficient,@fc_evidence_required,@fc_evidence_file,
                            @fc_acceptance,@fc_product_change_description,@fc_cnid,@feedback_category_id)";

			var conn = new MySqlConnection(Properties.Settings.Default.ConnString);
			conn.Open();
			MySqlTransaction tr = conn.BeginTransaction();
            try
            {
                
				MySqlCommand cmd = new MySqlCommand(insertsql, conn, tr);
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
        }

        public static void Update(Returns o, List<int> deletedImagesIds = null)
		{
			string updatesql = @"UPDATE returns SET return_no = @return_no,client_id = @client_id,request_date = @request_date,request_user = @request_user,request_userid = @request_userid,
                                cprod_id = @cprod_id,return_qty = @return_qty,custpo = @custpo,custpo_certainty = @custpo_certainty,custpo_estimate = @custpo_estimate,order_id = @order_id,
                                status1 = @status1,client_comments = @client_comments,client_comments2 = @client_comments2,client_comments3 = @client_comments3,fc_comments = @fc_comments,
                                factory_comments = @factory_comments,agent_comments = @agent_comments,cc_comments = @cc_comments,fc_response_date = @fc_response_date,
                                cc_response_date = @cc_response_date,agent_response_date = @agent_response_date,closed_date = @closed_date,reason = @reason,brand = @brand,reference = @reference,
                                factory_decision = @factory_decision,factory_decision_date = @factory_decision_date,decision = @decision,decision_final = @decision_final,credit_po = @credit_po,
                                credit_value = @credit_value,credit_value_override = @credit_value_override,claim_type = @claim_type,claim_value = @claim_value,warning_flag = @warning_flag,
                                openclosed = @openclosed,awaiting_user = @awaiting_user,flagged = @flagged,flagged_reason = @flagged_reason,importance = @importance,highlight = @highlight,
                                std_unique_id = @std_unique_id,resolution = @resolution,ip_address = @ip_address,delaminating = @delaminating,quote1 = @quote1,quote1_price = @quote1_price,
                                quote1a = @quote1a,quote1a_price = @quote1a_price,quote1b = @quote1b,quote1b_price = @quote1b_price,quote1c = @quote1c,quote1c_price = @quote1c_price,
                                quote2 = @quote2,quote2_price = @quote2_price,quote3 = @quote3,quote3_price = @quote3_price,quotechoice = @quotechoice,factory_reason = @factory_reason,
                                spec_code1 = @spec_code1,spec_name = @spec_name,fc_po_sufficient = @fc_po_sufficient,fc_evidence_sufficient = @fc_evidence_sufficient,
                                fc_evidence_required = @fc_evidence_required,fc_evidence_file = @fc_evidence_file,fc_acceptance = @fc_acceptance,
                                fc_product_change_description = @fc_product_change_description,fc_cnid = @fc_cnid,feedback_category_id = @feedback_category_id WHERE returnsid = @returnsid";

            var conn = new MySqlConnection(Properties.Settings.Default.ConnString);
            conn.Open();
            MySqlTransaction tr = conn.BeginTransaction();
            try
            {
                MySqlCommand cmd = new MySqlCommand(updatesql, conn,tr);
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
				var cmd = new MySqlCommand("DELETE FROM returns WHERE returnsid = @id" , conn);
                cmd.Parameters.AddWithValue("@id", returnsid);
                cmd.ExecuteNonQuery();
            }
		}

        public static List<ClaimsAnalyticsRow> GetAnalytics(List<int> ids)
        {
            var result = new List<ClaimsAnalyticsRow>();
            if (ids.Count > 0)
            {
                using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
                {
                    conn.Open();
                    var cmd = new MySqlCommand("", conn);
                    cmd.CommandText =
                        string.Format(
                            @"SELECT cprod_id, cprod_code1,cprod_name, COALESCE((SELECT SUM(orderqty) FROM om_detail1 WHERE om_detail1.cprod_id = cust_products.cprod_id),0) AS orderqty, 
                                    COALESCE((SELECT SUM(return_qty) FROM returns WHERE returns.cprod_id = cust_products.cprod_id AND decision_final = 1),0) AS claims FROM cust_products WHERE cprod_id IN ({0})",
                            Utilities.CreateParametersFromIdList(cmd, ids));
                    var dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        result.Add(new ClaimsAnalyticsRow
                            {
                                cprod_id = (int) dr["cprod_id"],
                                cprod_code1 = string.Empty + dr["cprod_code1"],
                                cprod_name = string.Empty + dr["cprod_name"],
                                orderqty = Convert.ToInt32(dr["orderqty"]),
                                claims = Convert.ToInt32(dr["claims"])
                            });
                    }
                    dr.Close();
                }
            }
            return result;
        }


	}
}
			
			