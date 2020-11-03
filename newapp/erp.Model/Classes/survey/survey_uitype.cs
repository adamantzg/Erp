
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public class Survey_uitype
	{
	    public const int Radio = 1;
	    public const int Checkbox = 2;
	    public const int Text = 3;
	    public const int TextArea = 4;
	    public const int DropDown = 5;

		public int uitype_id { get; set; }
		public string name { get; set; }
	
	}
}	
	