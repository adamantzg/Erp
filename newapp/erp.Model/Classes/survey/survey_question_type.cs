
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public class Survey_question_type
	{
	    public const int SingleChoice = 1;
	    public const int MultiChoice = 2;
	    public const int Text = 3;
	    public const int Group = 4;

		public int question_type_id { get; set; }
		public string name { get; set; }
	
	}
}	
	