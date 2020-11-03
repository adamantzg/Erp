
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public partial class Dealer_external_comment
	{
        [Key]
		public int comment_id { get; set; }
		public DateTime? date { get; set; }
		public int? user_id { get; set; }
		public string text { get; set; }
		public int? dealer_id { get; set; }

        public virtual Dealer_external Dealer { get; set; }
        public virtual User User { get; set; }
	
	}
}	
	