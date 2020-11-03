using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using erp.Model;
using erp.Model.Dal.New;
using backend.Controllers;

namespace backend.Models
{
    
    public class InvoiceModel
    {
        public Invoices Invoice { get; set; }
        public List<Invoice_type> InvoiceTypes { get; set; }
        public List<Company> Companies { get; set; }
        public string user_text { get; set; }
        public List<Payment_details> PaymentDetails { get; set; }
        public List<Currencies> Currencies { get; set; }
        public int Currency_id { get; set; }
        public EditMode EditMode { get; set; }
        public List<QuantityType> QuantityTypes { get; set; }
        public List<Delivery_locations> DeliveryLocations { get; set; }
        public string UrlToReturnTo { get; set; }
        //public int? Month21 { get; set; }

    }

    public class CreditNotePrintModel
    {
        public Invoices Invoice { get; set; }
        public List<Brand> Brands { get; set; }
    }

    public class QuantityType
    {
        public const int QuantityTypeNormal = 1;
        public const int QuantityTypePercentage = 2;

        public int id { get; set; }
        public string name { get; set; }
    }

    public class InvoiceListModel
    {
        public DateTime? From { get; set;}
        public DateTime? To { get; set; }
        public int? Month21 { get; set; }
        public int? custid { get; set; }
        public List<Invoices> Invoices { get; set; }
        public List<Invoice_type> InvoiceTypes { get; set; }
        public bool AllowEdit { get; set; }
        public string ActionName { get; set; }
        public bool? brands { get; set; }
        public List<Brand> Brands { get; set; }
        public string custIds { get; set; }
        public bool ForPrint { get; set; }
        public bool ShowTotals { get; set; }
        public string UrlReturnTo { get; set; }
        public bool ShowGenerateButton { get; set; }
        public string ReferencePrefix { get; set; }
    }

    

    public class BrandDocumentsListModel
    {
        public const int SearchTypeInvoice = 1;
        public const int SearchTypeOrder = 2;
        public const int SearchTypeETD = 3;
        
        //public DateTime From { get; set; }
        //public DateTime To { get; set; }
        public int? Month21 { get; set; }
        public int SearchType { get; set; }
        public string PoRef { get; set; }
        public List<int> ClientIds { get; set; }
        public List<int> FactoryIds { get; set; }
        public List<int> ExcludedClientIds { get; set; }
        public bool ShowInvoiceNumber { get; set; }
        public string ActionName { get; set; }
        //public bool UseETA { get; set; }
        public OrderDateMode DateMode { get; set; }
        public bool ShowEUR { get; set; }

        public List<Order_lines> Lines { get; set; }

        public List<Brand> Brands { get; set; }
        public string Title { get; set; }
        public int InvoiceLinkType { get; set; }
        public bool ApplyVAT { get; set; }
        public List<CheckBoxItem> Clients { get; set;}
        public bool ShowClientSelection { get; set; }
        public bool ShowFactorySelection { get; set; }
        public bool IncludePoLines { get; set; }
        public bool IgnoreClients { get; set; }

        public List<CheckBoxItem> Factories { get; set; }

        public bool ShowFactoryCode { get; set; }

        public bool? UKOnly { get; set; }
        public bool ShowBBSInvoice { get; set; }
        public bool BBSDataOnly { get; set; }
        public DateTime? Invoices_NewCalculationStartDate { get; set; }
        public List<int> Invoices_NewCalculationExceptions_OrderIds { get; set; }
        public DateTime? Invoices_AllocationBasedValueCalculationStartDate { get; set; }
        public List<int?> Invoices_AllocationBasedValueCalculationClientsIds { get; set; }
        public List<int?> Invoices_AllocationBasedValueCalculationFactoryIds { get; set; }
		public List<Exchange_rates> ExchangeRates { get; set; }
    }

    public class BrandDocumentsTableModel
    {
        public BrandDocumentsListModel ListModel { get; set; }
        public Func<Order_lines, bool> LinesPredicate { get; set; }
        public string TableTitle { get; set; }
        public List<Company> Factories { get; set; } 
        public bool ShowInvoiceLink { get; set; }
        public int InvoiceLinkType { get; set; }
        public bool ApplyVAT { get; set; }
        public bool ShowEUR { get; set; }
        //public bool UseETA { get; set; }
        public OrderDateMode DateMode { get; set; }
        public bool ShowFactoryCode { get; set; }
        public bool ShowBBSInvoice { get; set; }
        
    }

    public class MonthSelectorModel
    {
        public string ActionName { get; set; }
        public int? Month21 { get; set; }
        public string TitlePrefix { get; set; }
        public Dictionary<string,object> Params { get; set; }
    }

    public class InvoiceLogModel
    {
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public List<Invoice_type> InvoiceTypes { get; set; }
        public List<Currencies> Currencies { get; set; }
        public List<Invoices> Invoices { get; set; }
    }

    
}