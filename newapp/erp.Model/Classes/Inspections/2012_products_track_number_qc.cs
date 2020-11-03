
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;


namespace erp.Model
{
	[Table("2012_products_track_number_qc")]
	public partial class products_track_number_qc
	{
        [Key]
		public int producttrack_id { get; set; }
		public int? mastid { get; set; }
		public string track_number { get; set; }
		public int? insp_id { get; set; }
		public string producttrack_date { get; set; }

        public virtual Mast_products MastProduct { get; set; }
	
	}
}	
	