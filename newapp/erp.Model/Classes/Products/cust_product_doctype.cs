
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace erp.Model
{
	/// <summary>
    /// Old class used on inspections etc.
    /// </summary>
	public partial class Cust_product_doctype
	{
	    public const int TechDataSheet = 1;
	    public const int BasicDrawing = 2;
	    public const int DetailedDrawing = 3;
	    public const int Instructions = 4;
	    public const int Label = 5;
	    public const int Packaging = 6;
	    public const int CAD = 7;
        public const int HiRes = 8;
        public const int Photo = 9;

		public int id { get; set; }
		public string name { get; set; }
	
	}
}	
	