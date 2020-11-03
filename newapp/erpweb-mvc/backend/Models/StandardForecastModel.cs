using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using erp.Model;

namespace backend.Models
{
    public class StandardForecastModel
    {
        public int? User_id { get; set; }
        public List<Range> Ranges { get; set; }
        public List<Company> Companies { get; set; }
    }
}