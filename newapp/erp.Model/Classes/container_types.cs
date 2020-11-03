
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public class Container_types
	{
	    public const int Gp40 = 0;
	    public const int Gp20 = 1;
	    public const int Hc40 = 2;
	    public const int Lcl = 3;
        public const int Gp40b = 6;

        [Key]
		public int container_type_id { get; set; }
		public string container_type_desc { get; set; }
		public double? width { get; set; }
		public double? length { get; set; }
		public double? height { get; set; }
        public string full_description { get; set; }
        public string shortname { get; set; }
        public double? usable_sqm { get; set; }
        public double? usable_cbm { get; set; }

        public double? Volume
        {
            get
            {
                return usable_cbm ?? width * 1.0 / 1000 * length * 1.0 / 1000 * height * 1.0 / 1000;
            }
        }
    }
}	
	