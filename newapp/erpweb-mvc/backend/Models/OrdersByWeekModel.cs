using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using erp.Model;

namespace backend.Models
{
    public class OrdersByWeekModel
    {
        public List<OrderWeeklyData> DistributorData { get; set; }
        public List<OrderWeeklyData> FactoryData { get; set; }
        public DateTime FirstDate { get; set; }
        public int Weeks { get; set; }
    }
}