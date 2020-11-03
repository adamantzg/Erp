using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace erp.Model
{
    [NotMapped]
    public class Spares
    {
        public int id { get; set; }
        public string factory_code { get; set; }
        public string spare_code { get; set; }
        public string spare_description { get; set; }
        public string related_code { get; set; }
        public string related_description { get; set; }
        public string prod_image1 { get; set; }
    }



    [NotMapped]
    public class SparesProducts
    {
        public int id { get; set; }
        public int spare_cprod { get; set; }
        public int product_cprod { get; set; }

        public string cprod_code { get; set; }

        public string desc { get; set; }

        public string status { get; set; }
        public double? order_qty { get; set; }
        public string process { get; set; }

        public System.DateTime? timedate { get; set; }

        public int months { get; set; }

        //public int product_product_cprod { get; set; }

        public string product_cprod_name { get; set; }

        public string product_cprod_code1 { get; set; }

        public int product_cprod_id { get; set; }
        public System.DateTime? product_timedate { get; set; }
        public int product_month { get; set; }

        public string product_cprod_status { get; set; }

        public string brand_user_id { get; set; }

        public string brand_name { get; set; }
    }

    public class Spare
    {
        [Key]
        public int spare_id { get; set; }
        public int spare_cprod { get; set; }
        public int product_cprod { get; set; }
        public string spare_desc { get; set; }
		public int? hide_flag { get; set; }

		[NotMapped]
		public Cust_products SpareProduct { get; set; }
    }
}
