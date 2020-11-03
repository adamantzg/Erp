using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using erp.Model;

namespace backend.Models
{
    public class StockOrderListModel
    {
        public List<Stock_order_header> Orders { get; set; }
        public List<Company> Factories { get; set;}
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public int? Factory_id { get; set; }
    }

    public class StockOrderEditModel
    {
        public List<Company> Factories { get; set; }
        public List<Company> Companies { get; set; }
        public Stock_order_header Order { get; set; }
        public EditMode EditMode { get; set; }
    }

    public class StockOrderCalculationModel
    {
        public List<ProductSaleMonthSummary> ProductSalesData { get; set; }
        public List<Cust_products> Products { get; set; }
        public List<ProductOrderSummary> OrderSummary { get; set; }
    }
}