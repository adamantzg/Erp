
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public class Countries
	{
		public string ISO2 { get; set; }
		public string ISO3 { get; set; }
		public string CountryName { get; set; }
        [Key]
        public int country_id { get; set; }
        public double? exchange_rate { get; set; }
        public string local_name { get; set; }
        public string flag_filename { get; set; }
        public string continent_code { get; set; }
	}
}	
	