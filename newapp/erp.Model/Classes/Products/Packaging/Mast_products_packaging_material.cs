using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace erp.Model
{
	public class Mast_products_packaging_material
	{
		public int id { get; set; }
		public int? mast_id { get; set; }
		public int? packaging_id { get; set; }
		public int? material_id { get; set; }
		public double? amount { get; set; }

		public virtual Packaging Packaging { get; set; }
		public virtual Material Material { get; set; }
		public virtual Mast_products MastProduct { get; set; }
	}
}
