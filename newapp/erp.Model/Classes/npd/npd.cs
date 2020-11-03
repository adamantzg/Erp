
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public class Npd
	{
		public int prod_id { get; set; }
		public string prod_name { get; set; }
		public string tracking_num { get; set; }
		public string description { get; set; }
		public int? prod_type { get; set; }
        public int? Month_Sequence { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? DateModified { get; set; }
        public string prod_type_text { get; set; }
        public int? status_id { get; set; }
        public int? cprod_id { get; set; }
        public string prod_code { get; set; }

        public string status_name { get; set; }
        public string brand_names { get; set; }
        public string category_names { get; set; }
        public int? createdby { get; set; }
        public int? modifiedby { get; set; }

        public User Creator { get; set; }
        public User Editor { get; set; }

        public List<Npd_comments> Comments { get; set; }
        public List<Npd_file> Files { get; set; }
        //public List<BrandCategory> Categories { get; set; }
        public List<Category1> Categories { get; set; }
        public List<Brand> Brands { get; set; }
        

        public DateTime? LastCommentDate { get; set; }
	
	}
}	
	