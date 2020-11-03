
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;


namespace erp.Model
{
	[Table("Role")]
	public partial class Role
	{
		public int id { get; set; }
		public string name { get; set; }
	
	}
}	
	