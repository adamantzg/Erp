using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using erp.Model;

namespace backend.Models
{
    public class SalesRepModel
    {
        public const int ProductChoice_All = 0;
        public const int ProductChoice_displayedOnly = -1;
        public const int ProductChoiceTopN = -2;
        public const int ProductChoiceBottomN = -3;

        public const int ProductChoice_TopNumber = 10;

        public const int CompareWith_LocalDisplaying = 1;
        public const int CompareWith_LocalNonDisplaying = 2;
        public const int CompareWith_Previous = 3;
        public const int CompareWith_LocalDealers = 4;

        //public Dealer Dealer { get; set; }
        public List<string> OpeningStoreTimes { get; set; }
        public List<string> ClosingStoreTimes { get; set; }
        public List<Brand> Brands { get; set; }
        public List<Brand_external> ExternalBrands { get; set; }
        public List<Brand_external> SelectedExternalsBrands { get; set; }
        public double? Latitude { get; set; }
        public List<Customer_type> CustomerTypes { get; set; }
        public double? Longitude { get; set; }

        public List<string> SqFeetValues { get; set; }
        public List<string> AnnualTurnoverValues { get; set; }

        public List<LookupItem> CompareValues { get; set; }
        public List<LookupItem> Radiuses { get; set; }
    }

    public class SalesRepParameterObject
    {
        public int brand_id { get; set; }
        public int? range { get; set; }
        public int? cprod_id { get; set; }
        public int dealer_id { get; set; }
        public int? CompareWith { get; set; }
        public int ChartType { get; set; }
        public int Distance { get; set; }
        public int Months { get; set; }
        

        public SalesRepParameterObject()
        {
        }

        public SalesRepEmailModel EmailModel { get; set; }
    }

    public class SalesRepParameterObjectEx
    {
        public SalesRepParameterObject Base { get; set; }
        public Brand Brand { get; set; }
        public Range Range { get; set; }
        public Cust_products Product { get; set; }
        public Dealer Dealer { get; set; }
    }

    public class SalesRepEmailModel
    {
        public string To { get; set; }
        public string Cc { get; set; }
        public string Bcc { get; set; }
        public string Subject { get; set; }
        public string GraphUrl1 { get; set; }
        public string GraphUrl2 { get; set; }

        public SalesRepEmailModel()
        {

        }
    }

    public class ProductSalesData
    {
        public Cust_products Product { get; set; }
        public float value { get; set; }
        public int qty { get; set; }
    }

    public class GroupedSalesData
    {
        public DateTime Date { get; set; }
        public List<Client_sales_data> SalesData { get; set; }
    }

    public class SalesRepDealer
    {
        public Dealer_external Dealer { get; set; }
        public List<GroupedSalesData> GroupedSales { get; set; }
        public List<ClientSalesAggregate> TopNProducts { get; set; }
    }

}