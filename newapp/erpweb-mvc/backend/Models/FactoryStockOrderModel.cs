using erp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace backend.Models
{
    public class FactoryStockOrderEditModel
    {
        public Factory_stock_order Order { get; set; }
        public List<Company> Clients { get; set; }
        public List<Currencies> Currencies { get; set; }
        public List<Company> Factories { get; set; }
        public List<Factory_stock_order> Orders { get; set; }
    }

    public class FactoryStockOrderListModel
    {
        public List<Company> Factories { get; set; }
        public List<Currencies> Currencies { get; set; }
    }

    public class FactoryStockOrderReportModel
    {
        public List<Factory_stock_order_lines> Lines { get; set; }
        public List<Brand> Brands { get; set; }
    }
}