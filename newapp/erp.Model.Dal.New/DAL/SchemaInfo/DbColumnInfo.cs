using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace erp.Model.Dal.New
{
	public class DbColumnInfo
	{
		public string table_name { get;set;}
		public string column_name { get;set;}
		public string data_type { get;set;}
		public int? ordinal_position { get; set; }
		public string column_type { get;set; }
		public string column_key { get;set; }
	}
}
