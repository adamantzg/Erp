
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations.Schema;



namespace erp.Model
{


    [DataContract]
	public class Returns_comments : IExtensibleDataObject
	{
        [NotMapped]
        public ExtensionDataObject ExtensionData {get;set; }

        [DataMember]
		public int comments_id { get; set; }
        [DataMember]
		public int? return_id { get; set; }
        [DataMember]
		public string comments_type { get; set; }
        [DataMember]
		public int? comments_from { get; set; }
        [DataMember]
		public int? comments_to { get; set; }
        [DataMember]
		public string comments { get; set; }
        [DataMember]
		public DateTime? comments_date { get; set; }
        [DataMember]
		public int? decision_flag { get; set; }
        [DataMember]
		public int? fc_response { get; set; }
        [DataMember]
        public User Creator { get; set; }

        public virtual Returns Return { get; set; }

        [DataMember]
        public List<Returns_comments_files> Files { get; set; }
	
	}
}	
	