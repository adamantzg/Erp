
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
	public class Returns_importance
	{
        [NotMapped]
        public ExtensionDataObject ExtensionData {get;set; }

        [Key]
        [DataMember]
		public int importance_id { get; set; }
        [DataMember]
		public string importance_text { get; set; }
        [DataMember]
		public int? feedback_type_id { get; set; }
        [DataMember]
        public int? days { get; set; }

        public List<Returns> Returns { get; set; }
	
	}
}	
	