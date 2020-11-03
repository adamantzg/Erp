using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace erp.Model
{
    public class ReturnsSimple
    {
        public int? cprod_id { get; set; }
        public int? ReturnQty { get; set; }
        public DateTime? RequestDate { get; set; }
        public int? ClaimType { get; set; }
        public double? ClaimValue { get; set; }
        public double? CreditValue { get; set; }
        
        public double? TotalValue
        {
            get { return ClaimType == 2 ? ClaimValue : ReturnQty * CreditValue; }
        }
    }
}
