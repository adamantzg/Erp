
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public partial class Technical_subcategory_template
	{
		public int category1_sub { get; set; }
		public int technical_data_type { get; set; }
		public int? sequence { get; set; }

        public virtual Technical_data_type TechnicalDataType { get; set; }
	
	}
}	
	