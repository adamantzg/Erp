using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace erp.Model
{
    public class BudgetActualData
    {
        public int id { get; set; }
        public int month21 { get; set; }
        public string record_type { get; set; }
        public int? brand_id { get; set; }
        public int? distributor_id { get; set; }
        public double? value { get; set; }
        public int? ukflag { get; set; }

        public virtual Brand Brand { get; set; }
        public virtual Company Distributor { get; set; }
    }
}
