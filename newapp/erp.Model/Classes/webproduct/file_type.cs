
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public partial class File_type
	{
	    public const int Image = 1;
	    public const int Drawing = 2;
	    public const int Instructions = 3;
        public const int Dwg = 4;
		public const int Certificate = 5;

		public int id { get; set; }
		public string name { get; set; }
        public string path { get; set; }
        public string previewpath { get; set; }
	}
}	
	