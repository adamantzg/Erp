using System;
using erp.Model;
//using MVCControlsToolkit.DataAnnotations;

namespace backend.Models
{
    public class ContractSalesForecastModel
    {
        public int company_id { get; set; }
        public Contract_sales_forecast Forecast { get; set; }
        //[DateRange(SMinimum = "Today+1M", SMaximum = "Today+18M")]
        public DateTime? StartMonth { get; set; }
    }
}