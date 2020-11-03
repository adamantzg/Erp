
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public partial class Technical_data_type
	{
        [Key]
		public int data_type_id { get; set; }
		public string data_type_desc { get; set; }
	
	}
}	
	