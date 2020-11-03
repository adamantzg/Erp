using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace erp.Model
{
	public class File
	{ 
		public int id { get;set;}
		public string name { get;set;}
		public int? type_id { get;set;}
		public string description { get;set;}

		[NotMapped]
		public string file_id { get;set;}
		[NotMapped]
		public string url { get;set;}

		public List<Company> Companies { get;set;}
		public List<Mast_products> MastProducts { get;set;}
	}
}
