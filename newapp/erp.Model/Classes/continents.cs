using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace erp.Model
{
    public class Continents
	{
		public const string Africa = "AF";
		public const string Asia			="AS";
		public const string Europe			="EU";
		public const string NorthAmerica	="NA";
		public const string SouthAmerica	="SA";
		public const string Oceania = "OC";
		public const string Antarctica = "AN";

		[Key]
        public int id { get; set; }
        public string name { get; set; }
        public string code { get; set; }
        public int? status { get; set; }
    }

	
}
