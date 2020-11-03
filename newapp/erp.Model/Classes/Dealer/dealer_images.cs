
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public class Dealer_images
	{
		public int image_unique { get; set; }
		public int? dealer_id { get; set; }
		public string dealer_image { get; set; }
		public int? seq { get; set; }
		public int? hide { get; set; }
        public int? reviewed { get; set; }
        public bool? store_page { get; set; }
        public DateTime? DateModified { get; set; }
        public DateTime? DateCreated { get; set; }


        public List<Brand> Brands { get; set; }
        public virtual List<Dealer_image_displays> Displays { get; set; }
        [NotMapped]
        public bool FileExists { get; set; }

        public virtual Dealer Dealer { get; set; }

        public Dealer_images()
        {
            reviewed = 0;
        }
        [NotMapped]
        public long length { get; set; }
	}
}	
	