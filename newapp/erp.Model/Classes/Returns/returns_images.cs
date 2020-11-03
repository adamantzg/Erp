
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public class Returns_images
	{

        public const int ReturnedByFactory = 1;
        public const int RecheckPhotos = 2;

        [Key]
		public int image_unique { get; set; }
		public int? return_id { get; set; }
		public string return_image { get; set; }
		public int? user_type { get; set; }
		public int? cc_use { get; set; }
		public int? added_by { get; set; }
		public DateTime? added_date { get; set; }
        public int? file_category { get; set; }

        public virtual Returns Return { get; set; }
        [NotMapped]
        public string file_id { get; set; }
	
	}
}	
	