
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public partial class Inspection_v2_container_images
	{
		public int id { get; set; }
		public int? container_id { get; set; }
		public string insp_image { get; set; }
        public int? order { get; set; }

        public virtual Inspection_v2_container Container { get; set; }
	
	}
}	
	