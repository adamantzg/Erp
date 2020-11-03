using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace erp.Model
{
    public class Instructions
    {
        public int unique_id { get; set; }
        public int? mast_id { get; set; }
        public string instruction_filename { get; set; }
        public int? language_id { get; set; }

		[NotMapped]
		public string languageName { get; set; }
        
        public virtual Language Language { get; set; }
    }
}
