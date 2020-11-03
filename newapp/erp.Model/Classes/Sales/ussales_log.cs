
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public partial class Ussales_log
	{
        public const int type_GoodsIn = 1;
        public const int type_SalesOut = 2;

        public int id { get; set; }
		public int? type { get; set; }
		public int? lineid { get; set; }
		public DateTime? logdate { get; set; }
		public int? user_id { get; set; }
		public double? old_qty { get; set; }
		public double? new_qty { get; set; }
                
        public  string product { get; set; }

        
	
	}
}	
	