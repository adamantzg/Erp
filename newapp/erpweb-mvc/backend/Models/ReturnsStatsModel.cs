using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using erp.Model;


namespace backend.Models
{
    public class ReturnsStatsModel
    {
        public List<Company> Distributors { get; set; }
        public List<Order_line_Summary> Sales { get; set; }
        public List<ReturnAggregateDataPrice> ReturnsSummary { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime StartDate { get; set; }
        
    }
}