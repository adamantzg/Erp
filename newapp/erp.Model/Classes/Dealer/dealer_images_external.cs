
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public class Dealer_images_external
	{
		public string image_unique { get; set; }
        public string _id { get; set; }
		public string dealer_id { get; set; }
		public string dealer_image { get; set; }
        public DateTime? _createdAt { get; set; }
	}
}	
	