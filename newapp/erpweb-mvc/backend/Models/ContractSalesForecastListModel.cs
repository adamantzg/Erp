using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using erp.Model;

namespace backend.Models
{
    public class ContractSalesForecastListModel
    {
        public List<Contract_sales_forecast> List { get; set; }
        public bool CanDelete { get; set; }
    }
}