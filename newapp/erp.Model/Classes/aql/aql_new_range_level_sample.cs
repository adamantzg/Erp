using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace erp.Model
{
	public class aql_new_range_level_sample
	{
		public int range_id { get; set; }
		public int level_id { get; set; }
		public int sample_size_id { get; set; }

		public virtual aql_new_sample_size SampleSize { get; set; }
	}
}
