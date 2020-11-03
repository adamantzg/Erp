using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace asaq2.Model
{
    
        public class ProductInvestigations
        {
            public int Id { get; set; }
            public int CprodId { get; set; }
            public int? MastId { get; set; }
            public DateTime? Date { get; set; }
            public int Status { get; set; }
            public string Comments { get; set; }
            public string MonitoredBy { get; set; }
        }
    
}
