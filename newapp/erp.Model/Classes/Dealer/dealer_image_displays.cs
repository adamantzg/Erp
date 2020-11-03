
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public class Dealer_image_displays
	{
		public int image_id { get; set; }
		public int web_unique { get; set; }
		public int? qty { get; set; }
        public int? claimed { get; set; }
        [NotMapped]
        public DateTime? datecreated { get; set; }
        [NotMapped]
        public bool FileExists { get; set; }

        
        public virtual Dealer_images Image { get; set; }
        //public virtual WebProduct Product { get; set; }
        
        public virtual Web_product_new ProductNew { get; set; }
	
	}
}	
	