using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using erp.Model;

namespace backend.Models
{
    public class WeeklyAnalyticsModel
    {
        public List<StockSummary> StockSummaries { get; set; }
        public double Duty { get; set; }
        public double Freight { get; set; }
        public Guid ChartKey { get; set; }
        public int StockWeekFrom { get; set; }
        public int StockWeekTo { get; set; }
        public Dictionary<string,StockCodeProductCount> StockCodeProductCounts { get; set; }
        public string Logo { get; set; }
    }

    public class ProductForecastingData
    {
        public List<Sales_forecast> SalesForecasts { get; set; }
        public List<Order_lines> ArrivingLines { get; set; }
        public List<Order_lines> StockOrderLines { get; set; }
        public List<Order_lines> RegularOrderLines { get; set; }
        public List<Order_lines> CalloffOrderLines { get; set; }
    }

    public class StockCodeProductCount
    {
        public int ProductCount { get; set; }
        public int NegativeStockProductCount { get; set; }
    }
}
