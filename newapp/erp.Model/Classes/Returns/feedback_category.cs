
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations.Schema;


namespace erp.Model
{
	
    [DataContract]
	public class Feedback_category
	{
        [NotMapped]
        public ExtensionDataObject ExtensionData {get;set; }

        [Key]
        [DataMember]
		public int feedback_cat_id { get; set; }
        [DataMember]
		public string name { get; set; }
        [DataMember]
		public int? feedback_type { get; set; }

        
        public List<Returns> Feedbacks { get; set; }
	
	}
}	
	