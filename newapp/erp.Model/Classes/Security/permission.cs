
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;


namespace erp.Model
{
	[Table("Permission")]
	public partial class Permission
	{
		public int id { get; set; }
		public string name { get; set; }
	
	}
}	
	