
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public partial class Technical_product_data
	{
        [Key]
		public int unique_id { get; set; }
		public int? mast_id { get; set; }
		public int? technical_data_type { get; set; }
		public string technical_data { get; set; }

        public virtual Technical_data_type TechnicalDataType { get; set; }
        public virtual Mast_products MastProduct { get;set; }

    }
}	
	