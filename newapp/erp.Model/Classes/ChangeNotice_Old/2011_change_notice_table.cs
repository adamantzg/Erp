
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public partial class change_notice_table
	{
        [Key]
		public int cn_id { get; set; }
		public string cn_category { get; set; }
		public string cn_reason { get; set; }
		public string cn_details { get; set; }
		public DateTime? cn_estimated_timeline { get; set; }
		public string cn_before_image { get; set; }
		public string cn_after_image { get; set; }
		public string cn_status { get; set; }
		public DateTime? cn_date { get; set; }
		public string cn_progress_update { get; set; }
		public int? cn_client { get; set; }
		public int? cn_factory { get; set; }
		public int? cn_drawing { get; set; }
		public int? cn_instruction { get; set; }
		public int? cn_label { get; set; }
		public int? cn_packing { get; set; }
		public int? cn_photo { get; set; }
		public DateTime? cn_confirm_date { get; set; }
		public string cn_add_id { get; set; }
		public int? cn_price { get; set; }
		public int? cn_progress { get; set; }
		public string cn_client_2 { get; set; }
		public string cn_category_2 { get; set; }
		public string cn_reason_2 { get; set; }
		public string cn_nonr_reason { get; set; }
	
	}
}	
	