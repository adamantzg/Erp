
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public class Issue_files
	{
		public int id { get; set; }
		public int? issue_id { get; set; }
		public string filename { get; set; }
        public DateTime? datecreated { get; set; }
        public int? created_userid { get; set; }
        public DateTime? datemodified { get; set; }
        public int? modified_userid { get; set; }

        public string creator { get; set; }
        public string editor { get; set; }
        //public byte[] filedata { get; set; }
        public string extension
        {
            get
            {
                if (string.IsNullOrEmpty(filename))
                    return string.Empty;
                else
                    return System.IO.Path.GetExtension(filename).Substring(1);
            }
        }
	}
}	
	