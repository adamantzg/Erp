
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public class Inspection_controller : IInspectionController
	{
		public int id { get; set; }
		public int inspection_id { get; set; }
		public int controller_id { get; set; }
		public DateTime startdate { get; set; }
		public int duration { get; set; }

        public User Controller { get; set; }

	    public Inspection_controller(IInspectionController iCont)
	    {
	        id = iCont.id;
	        inspection_id = iCont.inspection_id;
	        controller_id = iCont.controller_id;
	        startdate = iCont.startdate;
	        duration = iCont.duration;
	    }

	    public Inspection_controller()
	    {
	        
	    }
    	
	}
}	
	