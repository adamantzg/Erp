using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using erp.Model;
using backend.ContainerLoadService;

namespace backend.Models
{
    public class ContainerLoadResultsModel
    {
        public CalculationResult CalculationResult { get; set; }
        public List<Company> Customers { get; set; }
        public List<Order_header> Orders { get; set; }
        public DateTime? ETD { get; set; }
    }
}