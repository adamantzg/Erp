
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public class Issuecomment_files
	{
		public int id { get; set; }
		public int? comment_id { get; set; }
		public string filename { get; set; }

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
	