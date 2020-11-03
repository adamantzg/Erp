using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public partial class Inspection_v2_loading
	{
        public virtual Inspection_v2_line Line { get; set; }
        public virtual Inspection_v2_container Container { get; set; }
        public virtual List<Inspection_v2_area> Areas { get; set; }
        public virtual List<Inspection_v2_loading_mixedpallet> QtyMixedPallets { get; set; }

        [NotMapped]
        public virtual Inspection_v2_area Area { get; set; }
	}
}	
	