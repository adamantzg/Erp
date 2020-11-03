using System;
using System.Collections.Generic;
//using MVCControlsToolkit.DataAnnotations;
using erp.Model;
using System.Web.Mvc;
using System.Web.SessionState;


namespace backend.Models
{
    public class ReturnsModel
    {
        //[DateRange(SMinimum = "Today-3y", SMaximum = "Today-1d")]
        public DateTime? SaleMonth { get; set; }
        public int? searchBy { get; set; }  //custpo //datetime, text
        public bool? dateKnown { get; set; }
        public string custpo { get; set; }
        public int? cprod_id { get; set; }  //Product selected in checked list
        public List<Order_lines> Lines { get; set; }
        public Returns Return { get; set; }
        public ReturnStatus Status { get; set; }
        public List<OrderMonthlyData> OrderMonthlyData { get; set; }
        public int? TotalOrdered { get; set; }
        public ReturnAggregateData ReturnsDataLastYear { get; set; }
        public ReturnAggregateData ReturnsDataTotal { get; set; }
        public int orderid { get; set; }    //line selected in orders dropdown on edit page
        public List<Return_category> ReturnCategories { get; set; }
        public string Return_category { get; set; }
        //public int? Return_qty { get; set; }
        //public string Return_reason { get; set; }
        //public string Reference { get; set; }

        public List<Return_resolution> ReturnResolutions { get; set; }
    }

    public class ReturnAnalyticsModel
    {
        public List<Brand> Brands { get; set; }
        public List<CheckBoxItem> Categories { get; set; }
        public int? brand_id { get; set; }
    }

    public class ProductFeedbackModel : ISubTotals
    {
        public Company Company { get; set; }
        public Returns Feedback { get; set; }
        public List<Brand> Brands { get; set; }
        public List<LookupItem> Months { get; set; }
        public List<Standard_response> StandardResponses { get; set; }
        public List<Product_faults> Faults { get; set; }
        public string CustPo { get; set; }
        
        public List<OrderMonthlyData> OrderMonthlyData { get; set; }
        public List<Sales_data> SalesData { get; set; }
        public List<Returns_importance> Importances { get; set; }
        public List<Lookup> Resolutions { get; set; }
        public List<TicketStatus> TicketStatuses { get; set; }

        public bool CanViewInternalComments { get; set; }
        public bool CanEditInternalComments { get; set; }
        public bool CanViewExternalComments { get; set; }
        public bool CanEditExternalComments { get; set; }
        public List<Inspections> Inspections { get; set; }
        public List<Order_header> UpcomingOrders { get; set; }
        //public Guid ChartKey { get; set; }
        public SubTotal[] FaultSubTotals { get; set; }
        public SubTotal[] FaultXpg2SubTotals { get; set; }
        public SubTotal[] SalesSubTotals { get; set; }
    }

    public class TicketStatus
    {
        public double Id { get; set; }
        public string Text { get; set; }
    }

    public interface ISubTotals
    {
        SubTotal[] FaultSubTotals { get; set; }
        SubTotal[] FaultXpg2SubTotals { get; set; }
        SubTotal[] SalesSubTotals { get; set; }
    }

    public class ProductFaultModel
    {
        public List<Product_faults> Faults { get; set; }
        public List<Productfault_reason_description> FaultDescriptions { get; set; }
    }

    public class ListPGFaultsModel
    {
        public List<PGListRow> Rows { get; set; }
        public string ClientList { get; set; }
        public int? Ratio { get; set; }
        public int? Sort { get; set; }
        public int ListType { get; set; }     //1 - PG  2-XPG2
        public List<Company> Factories { get; set; }
        public bool ShowPopupLink { get; set; }
    }

    public class SalesPGFaultsModel : ISubTotals
    {
        public List<Product_faults> Faults { get; set; }
        public List<Sales_data> SalesData { get; set; }
        public SubTotal[] FaultSubTotals { get; set; }
        public SubTotal[] FaultXpg2SubTotals { get; set; }
        public SubTotal[] SalesSubTotals { get; set; }
        public bool ShowPopup { get; set; }
        public bool ClientSide { get; set; }
        public int cprod_id { get; set; }
        public List<OrderMonthlyData> OrderMonthlyData { get; set; }
        public bool ShowOrders { get; set; }
    }

    public class OrderTableModel
    {
        public List<OrderMonthlyData> OrderMonthlyData { get; set; }
    }

    public class SubTotal
    {
        public int month21 { get; set; }
        public double? subtotal { get; set; }
    }

    public class PGListRow
    {
        public Cust_products Product { get; set; }
        public double Sales { get; set; }
     
        public double PgData { get; set; }
        public double Ratio { get; set; }
    }

    public class ITFeedbackListModel
    {
        public List<Returns> Feedbacks { get; set; }
        public string SearchTerm { get; set; }
        public bool ShowAllCompleted { get; set; }
        public int? ListLimit { get; set; }
        public string ListAction { get; set; }
        public string CreateAction { get; set; }
        public string EditAction { get; set; }
        public bool Export { get; internal set; }
        public List<ITFeedbackListModelHelper> FeedbacksAdditionalData {get;set; }
        public List<feedback_authorization_level> FeedbackAuthorizationLevels { get; set; }
        public User CurrentUser { get; set; }
        public bool ShowIssueType { get; set; }
        public bool ShowExport { get; set; }
        public Dictionary<int, string> ExportReturnsQCSubscribers { get; set; }
        public DateTime? ExportFrom { get; set; }
        public DateTime? ExportTo { get; set; }

        public ITFeedbackListModel()
        {
            ListAction = "ITFeedbacks";
            CreateAction = "CreateItFeedback";
            EditAction = "ITFeedback";

        }
    }

    public class ITFeedbackListModelHelper
    {
        public int  returnsid {get;set; }
        public int? cprod_id { get;set;}
        public string factory { get;set;}
    }

    

    public class ITFeedbackModel
    {
        public Returns Feedback { get; set; }
        public List<Feedback_category> Categories { get; set; }
        public List<Returns_importance> Importances { get; set; }
        public EditMode EditMode { get; set; }
        public List<feedback_issue_type> IssueTypes { get; set; }
        public feedback_authorization FeedbackAuthorization { get; set; }

        public List<Standard_response> StandardResponses { get; set; }
        public bool CanViewInternalComments { get; set; }
        public bool CanEditInternalComments { get; set; }
        public bool CanViewExternalComments { get; set; }
        public bool CanEditExternalComments { get; set; }
        
        
    }

    public enum RoportType
    {
       Brands=0,
        NonBrands=1
 
    }

    public enum Roller
    {
        ThreeMonths=3,
        SixMonths=6,
        TwelveMonths=12
    }

    public class ParamChart
    {
        public double Returns { get; set; }
        public string Name { get; set; }
    }

    public class ClaimsAllModel
    {

        public List<Returns> Claims { get; set; }

        public List<Company> Clients { get; set; }

        public bool PrintMode { get; set; }
    }

    public class ShippingHistoryModel
    {

        public List<Order_line_export> ShippingHistory { get; set; }


        public bool PrintMode { get; set; }

        public List<Inspection_lines_rejected> InspectionLinesRejected { get; set; }
    }
    public class InspectionForProduct
    {

    }

    [Serializable]    
    public class ClaimsInvestigationsModel
    {
        //public Roller Rollers { get; set; }
        public int CprodId { get; set; }
        public List<ReturnAggregateDataProduct> Top10ReturnedProducts6m { get; set; }
        public List<ReturnAggregateDataProduct> Top10ReturnedProducts12m { get; set; }
        public List<ReturnAggregateDataProduct> TopReturnedProducts { get; set; }

        public List<ProductSales> CurrentProductSalesData { get; set; }
        public List<ProductSales> CurrentProductSalesDataAll { get; set; }
        public List<Order_line_export> ProductSalesAll { get; set; }
        public List<Order_lines> ProductSalesAllViaOrderLines { get; set; }
        public List<Order_line_export> ProductSalesAllV6 { get; set; }

        public ProductSales ProductSaleData { get; set; }

        public DateTime From { get; set; }
        public DateTime To { get; set; }

        public ReturnAggregateDataProduct ReturnAggregateDataProduct { get; set; }
        public List<Returns> ClaimForProduct12m { get; set; }
        public List<Returns> ClaimAgregateForeProdcut12m { get; set; }
        public List<Returns> ClaimForProductAll { get; set; }

        public List<Company> Clients { get; set; }
        //public List<StatusClaim> Statuses { get; set; }

        public List<Product_investigations> ProductInvestigations { get; set; }
        public Product_investigations StatusDetail { get; set; }
        public List<Product_investigation_status> Statuses { get; set; }
        public List<Product_investigation_images> ListProductImages { get; set; }
        public Product_investigation_images ProductImages { get; set; }

        public List<ReturnAggregateDataProduct> Top15ReturnedProduct6ago { get; set; }

        public int[] ArrSalesData { get; set; }
        public string[,] TableData{get;set;}
        //public Roller Rollers { get; set; }



        public List<Returns> ClaimForProductAllSixMonth { get; set; }

        public List<Returns> ClaimForProductAllThreeMonth { get; set; }

        public List<erp.Model.ReturnAggregateDataProduct> Top10ReturnedProducts3m { get; set; }
        //public List<asaq2.Model.ReturnAggregateDataProduct> Top10ReturnedProducts6m { get; set; }

        public List<Order_line_export> ProductSalesAllV6_12m { get; set; }

        public List<Order_line_export> ProductSalesAllV6_6m { get; set; }

        public List<Order_line_export> ProductSalesAllV6_3m { get; set; }

        public bool ShowPdfButton { get; set; }

        public bool PrintModel { get; set; }

        public List<SalesByMonth> ProductSalesDataByMonth { get; set; }

        public List<Order_line_Summary> CustomerSales { get; set; }

        public List<Order_line_Summary> CustomerSales6 { get; set; }

        public List<Order_line_Summary> CustomerSales3 { get; set; }

        public List<Product_investigation_images> ImageInvestigations { get; set; }

        public List<Product_investigation_images> ProductInvestImages { get; set; }

        public List<Product_investigation_images> ListInvestigationImages { get; set; }

        public int InvestigationId { get; set; }

        public List<Claims_investigation_reports> Reports { get; set; }

        public List<Claims_investigation_reports> Investigation { get; set; }

        public Cust_products Product { get; set; }
    }

    public class ListFotGraphs
    {
        public int CprodId { get; set; }
        public string CprodCode { get; set; }
    }
    public class StatusClaim
    {
        
        public int Id { get; set; }
        public string CurrentStatus { get; set; }
    }

    public class DetailedReport
    {

        public List<ReturnAggregateDataProduct> Top15ReturnedProducts6m { get; set; }

        public int Param { get; set; }

        public List<ProductSales> CurrentProductSalesData { get; set; }

        public List<Returns> ClaimForProduct12m { get; set; }

        public List<Returns> ClaimForProductAllSixMonth { get; set; }

        public int[] ArrSalesData { get; set; }

        public List<Product_investigations> ProductInvestigations { get; set; }

        public List<Claims_investigation_reports> Investigation { get; set; }
    }

    public class ClaimInvestigatinReport
    {
        public Claims_investigation_reports Reports { get; set; }

        public Claims_investigation_reports Report { get; set; }

        public Claims_investigation_reports_action ReportAction { get; set; }
    }

    public class ReportActionDetails
    {

        public List<Claims_investigation_reports_action> ReportActions { get; set; }
        public List<Claims_investigation_report_action_images> ImageActions { get; set; }

        public Claims_investigation_reports_action IdLastAction { get; set; }
    }
    public class CreateReport
    {
        public Claims_investigation_reports Report { get; set; }
        public Claims_investigation_reports_action Action { get; set; }
        public Claims_investigation_report_action_images Image { get; set; }

        public List<Claims_investigation_report_action_images> Images { get; set; }

        public List<Claims_investigation_reports_action> Actions { get; set; }

        public string CprodCode { get; set; }

        public bool Edit { get; set; }
    }

    public class ClaimInvestigationReports
    {

        public List<Claims_investigation_reports> Reports { get; set; }
    }

    public class ClaimInvestigationReportImage
    {
        public Claims_investigation_report_action_images Image { get; set; }
    }

    public class CAReportModel
    {
        public int? WeekNumber { get; set; }
        public int? CAItemsCount { get; set; }
        public List<CAReportCAItem> CAItems { get; set; }
        public CAReportInspectionItem InspectionItem { get; set; }

        public int? InspectionsTotalCount
        {
            get
            {
                return InspectionItem.LICount + InspectionItem.FICount;
            }
        }

        public string CAInsplWeekPercentage
        {
            get
            {
                if (CAItemsCount != null && CAItemsCount.Value != 0 && InspectionsTotalCount != null && InspectionsTotalCount.Value != 0)
                {
                    return ((decimal)CAItemsCount / (decimal)InspectionsTotalCount).ToString("0.##%");
                }
                else
                    return "0.00%";
            }
        }


    }

    public class ReturnsSimpleModel
    {
        public string client_comments { get; set; }
        public string client_comments2 { get; set; }
        public int? cprod_id { get; set; }
        public int? feedback_category_id { get; set; }
        
        public int? inspection_qty { get; set; }
        public int? recheck_required { get; set; }
        public int? rejection_qty { get; set; }
        public string return_no { get; set; }
        public int? sample_qty { get; set; }
        public int request_userid { get; set; }
        public string request_user{ get; set; }
        public int client_id { get; set; }
    }

    public class ClaimsStatsRow
    {
        public string brand { get; set; }
        public string factory_code { get; set; }
        public string cprod_code1 { get; set; }
        public string cprod_name { get; set; }
        public int? return_qty { get; set; }
        public double? credit_value { get; set; }
        public double? sales { get; set; }
        public List<ClaimsStatsRow> subData { get; set; }
        public double? otherValue { get; set; }
        public double? ratio
        {
            get
            {
                return credit_value / sales;
            }
        }
    }
}
/*
 client_comments:"reason"
client_comments2:"respčito"
cprod_id:11711
feedback_category_id:6
inspection:0
inspection_qty:20
recheck_required:true
rejection_qty:60
return_no:"CA-1797"
sample_qty:50
*/
