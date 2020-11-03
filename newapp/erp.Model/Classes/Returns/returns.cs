
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;


namespace erp.Model
{

	public enum ReturnStatus
	{
		New,
		InProgress,
		Closed
	}

	public enum FeedbackStatus
	{
		Incomplete = 0,
		Live = 1, 
		Cancelled = 2
	}

    public enum ReturnEventType
    {
        Recheck = 0,
        CorrectiveActionCreate = 1
    }
    
	public class Returns
	{
        [NotMapped]
        public ExtensionDataObject ExtensionData {get;set; }

        /// <summary>
        /// Old value from asp page, converting in progress to new value definiton
        /// </summary>
        
        public const int ClaimType_Default = 0;
        public const int Claimtype_Return = 1;
		public const int ClaimType_Refit = 2;
		public const int ClaimType_Product = 5;
		public const int ClaimType_ITFeedback = 6;
        public const int ClaimType_CorrectiveAction = 7;
        public const int ClaimType_QualityAssurance = 8;

        [Key]
        
		public int returnsid { get; set; }
        
		public string return_no { get; set; }
        
		public int? client_id { get; set; }
        
		public DateTime? request_date { get; set; }
        
		public string request_user { get; set; }
        
		public int? request_userid { get; set; }
        
		public int? cprod_id { get; set; }
        
		public int? return_qty { get; set; }
        
		public string custpo { get; set; }
        
		public int? custpo_certainty { get; set; }
        
		public int? custpo_estimate { get; set; }
        
		public int? order_id { get; set; }
        
		public int? status1 { get; set; }
        
		public string client_comments { get; set; }
        
		public string client_comments2 { get; set; }
        
		public string client_comments3 { get; set; }
        
		public string fc_comments { get; set; }
        
		public string factory_comments { get; set; }
        
		public string agent_comments { get; set; }
        
		public string cc_comments { get; set; }
        
		public DateTime? fc_response_date { get; set; }
        
		public DateTime? cc_response_date { get; set; }
        
		public DateTime? agent_response_date { get; set; }
        
		public DateTime? closed_date { get; set; }
        
		public string reason { get; set; }
        
		public int? brand { get; set; }
        
		public string reference { get; set; }
        
		public int? factory_decision { get; set; }
        
		public DateTime? factory_decision_date { get; set; }
        
		public int? decision { get; set; }
        
		public int? decision_final { get; set; }
        
		public int? credit_po { get; set; }
        
		public double? credit_value { get; set; }
        
		public double? credit_value_override { get; set; }
        
		public int? claim_type { get; set; }
        
		public double? claim_value { get; set; }
        
		public int? warning_flag { get; set; }
        
		public double? openclosed { get; set; }
        
		public int? awaiting_user { get; set; }
        
		public int? flagged { get; set; }
        
		public string flagged_reason { get; set; }
        
		public int? importance_id { get; set; }
        
		public int? highlight { get; set; }
        
		public int? std_unique_id { get; set; }
        
		public int? resolution { get; set; }
        
		public string ip_address { get; set; }
        
		public int? delaminating { get; set; }
        
		public string quote1 { get; set; }
        
		public double? quote1_price { get; set; }
        
		public string quote1a { get; set; }
        
		public double? quote1a_price { get; set; }
        
		public string quote1b { get; set; }
        
		public double? quote1b_price { get; set; }
        
		public string quote1c { get; set; }
        
		public double? quote1c_price { get; set; }
        
		public string quote2 { get; set; }
        
		public double? quote2_price { get; set; }
        
		public string quote3 { get; set; }
        
		public double? quote3_price { get; set; }
        
		public string quotechoice { get; set; }
        
		public string factory_reason { get; set; }
        
		public string spec_code1 { get; set; }
        
		public string spec_name { get; set; }
        
		public int? fc_po_sufficient { get; set; }
        
		public int? fc_evidence_sufficient { get; set; }
        
		public string fc_evidence_required { get; set; }
        
		public string fc_evidence_file { get; set; }
        
		public int? fc_acceptance { get; set; }
        
		public string fc_product_change_description { get; set; }
        
		public int? fc_cnid { get; set; }
        
		public int? ebuk { get; set; }
        
		public int? feedback_category_id { get; set; }
        
        public string contact_name { get; set; }
        
        public string contact_email { get; set; }
        
        public string contact_tel { get; set; }
        
        public string rejection { get; set; }
        
        public DateTime? rejection_date  { get; set; }
        
        public int? inspection_qty { get; set; }
        
        public int? sample_qty { get; set; }
        
        public int? rejection_qty { get; set; }
        
        public int? recheck_required { get; set; }
        
        public DateTime? recheck_date { get; set; }
        
        public int? recheck_status { get; set; }

        public string dealer_id { get; set; }

        public int? assigned_qc { get; set; }
        public int? insp_id { get; set; }

        public int? issue_type_id { get; set; }

        public int? usergroup_id { get; set; }

        public int? authorization_level { get; set; }

        [NotMapped]
        public string factory { get; set; }
        [NotMapped]
        public bool Deploy { get; set; }

        [NotMapped]
        
        public User LastCommenter
        {
            get
            {
                var lastuser = Comments?.OrderByDescending(c => c.comments_date)?.Take(1).FirstOrDefault()?.Creator;

                if(lastuser != null)
                    return lastuser;
                else
                    return null;
            }
        }

        private bool? hasComments;
        [NotMapped]
		public bool? HasComments
        {
            get
            {
                return hasComments ?? Comments?.Count > 0 ? true : false;
            }
            set
            {
                hasComments = value;
            }
        }

        [NotMapped]
		public int? Last_Commenter_Id { get; set; }
        [NotMapped]
        public string Last_Commenter_Name
        { get; set; }

        
        
		public User Creator { get; set; }

        public Company Client { get; set; }
        
		public Cust_products Product { get; set; }

        
		public List<Returns_comments> Comments { get; set; }

        
		public List<Returns_images> Images { get; set; }

        
		public Feedback_category Category { get; set; }
        
		public Returns_importance Importance { get; set; }
        
        
		public List<Feedback_subscriptions> Subscriptions { get; set; }
        
        public virtual List<Returns_qcusers> ReturnsQCUsers{ get; set; }

        
        public Return_resolution Return_resolution { get; set; }

        
        public List<Cust_products> Products { get; set; }

        public List<Returns_qcusers> AssignedQCUsers { get; set; }         
        public virtual feedback_issue_type IssueType { get; set; }

	    public double? TotalValue
	    {
	        get { return claim_type == 2 ? claim_value : return_qty*credit_value; }
	    }

        public  User AssignedQC { get; set; }

        public returns_decision DecisionFinal { get; set; }

        public virtual List<returns_events> Events { get; set; }

        public Returns() {
            openclosed = 0;
        }
        
    }

	public class ReturnAggregateData
	{
		public int TotalQty { get; set; }
		public int TotalAccepted { get; set; }
		public int TotalRejected { get; set; }
	}

    public enum SortField
    {
        ReturnToSalesRatio,
        TotalAcceptedValue
    }

    public class Recheck
    {
        public int returnsid { get; set; }
        public string return_no { get; set; }
        public int? client_id { get; set; }
        public DateTime? recheck_date { get; set; }
        public int? recheck_status { get; set; }
        public string client_comments2 { get; set; }
        public List<Returns_images> Images { get; set; }
    }

	public class ReturnAggregateDataProduct
	{
		public int cprod_id { get; set; }
		public string cprod_code1 { get; set; }
        public int cprod_user { get; set; }
		public string cprod_name { get; set; }
		public string brand { get; set; }
		public string Reason { get; set; }
		public int TotalAccepted{ get; set; }
		public int TotalRejected { get; set; }
		public int? UnitsShipped { get; set; }
        public double? TotalAcceptedValue { get; set; }
        public string customerCode { get; set; }
        public string Decision { get; set; }
        public int? Year { get; set; }
        public int? Month { get; set; }

		public double? ReturnToSalesRatio
		{
			get { return UnitsShipped != 0 ? TotalAccepted*1.0/UnitsShipped : 1.0; }
		}

        public double? ReturnToSalesRatioIncludeNoShipped
        {
            get { return UnitsShipped != 0 ? TotalAccepted * 1.0 / UnitsShipped : Double.MaxValue; }
        }
	}

	public class ReturnAggregateDataPrice
	{
        public int? id { get; set; }
		public string code { get; set; }
		public double TotalAccepted { get; set; }
		public double TotalRejected { get; set; }
		public double TotalReplacementParts { get; set; }
		public double TotalAcceptedEBUK { get; set; }
		public int claim_type { get; set; }
	}

	public class ReturnAggregateDataByMonth
	{
		public int? CountReturns { get; set; }
		public int? SumReturnsProduct { get; set; }
		public int created_month { get; set; }
		public string cprod_code1 { get; set; }
        public double? ReturnValue { get; set; }
	}

    public class CAReportCAItem
    {
        public string Reference { get; set; }
        public DateTime? DataCreated { get; set; }
        public string PO { get; set; }
        public string Products { get; set; }
        public string Factory { get; set; }
        public string Description { get; set; }
        public string CustomerCode { get; set; }
    }

    public class CAReportInspectionItem
    {
        public int LICount { get; set; }
        public int FICount { get; set; }
    }

    public class CASimpleApiModel
    {
        public Returns CA { get; set; }
        public List<CASimpleProduct> CASimpleProducts { get; set; }
        public inspections_list2 Inspection { get; set; }
    }
    
    public class CASimpleProduct
    {
        public string CprodCode { get; set; }
        public int OrderLineNum { get; set; }
        public int? cprod_id { get; set; }
    }

    public class ClaimSimple
    {
        public int returnsid { get; set; }
        public int? openclosed { get; set; }
        public int? status1 { get; set; }
        public string return_no { get; set; }
        public int? client_id { get; set; }
        public DateTime? request_date { get; set; }
        public string creator { get; set; }
        public int? request_userid { get; set; }
        public string factory { get; set; }
        public string description { get; set; }
        public int? importance_id { get; set; }
        public string importance { get; set; }
        public int? feedback_category_id { get; set; }
        public string category { get; set; }
        public string lastUpdatedBy { get; set; }
        public int? commentCount { get; set; }
        public string type { get; set; }
        public string client { get; set; }
        public string Qc { get; set; }
        public int? claim_type { get; set; }
        public int? cprod_id { get; set; }
        public int? order_id { get; set; }
        public bool? decision_final { get; set; }

        public Cust_products Product { get; set; }
        
        public DateTime? recheck_date { get; set; }
        public int? recheck_status { get; set; }
        public int? lastCommenterId { get; set; }
        public int? recheck_required { get; set; }
        public string client_comments2 { get; set; }
        public int? assigned_qc { get; set; }
        public DateTime? closed_date { get; set; }

        public int? Days
        {
            get
            {
                return Convert.ToInt32((DateTime.Today - request_date)?.TotalDays);
            }
        }

    }

    //public class ProductInvestigations
    //{
    //    public int Id { get; set; }
    //    public int CprodId { get; set; }
    //    public int? MastId { get; set; }
    //    public DateTime? Date { get; set; }
    //    public int Status { get; set; }
    //    public string Comments { get; set; }
    //    public string MonitoredBy { get; set; }
    //}
}
