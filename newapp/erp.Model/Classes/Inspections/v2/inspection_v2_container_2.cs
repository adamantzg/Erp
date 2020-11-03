
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public partial class Inspection_v2_container
	{
        
        public virtual List<Inspection_v2_container_images> Images { get; set; }
        public virtual List<Inspection_v2_loading> Loadings { get; set; }
        public virtual Inspection_v2 Inspection { get; set; }
        public virtual Container_types ContainerType { get; set; }
	}
}	
	