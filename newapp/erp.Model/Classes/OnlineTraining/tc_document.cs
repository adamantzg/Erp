using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace erp.Model
{
    public class tc_document
    {
        public int id {get;set;}
        public string filename { get; set; }
        public int? version { get; set; }
        public string subject { get; set; }
        public string note { get; set; }
        public int? acknowledgePeriod { get; set; }
        public int? original_id { get; set; }
        public DateTime? dateCreated { get; set; }
        public DateTime? dateLastModified { get; set; }
        public int? createdBy { get; set; }
        public int? modifiedBy { get; set; }        

        public virtual tc_document Original { get; set; }
        public virtual List<tc_document> Children { get; set; }

        public User Creator { get; set; }
        public User Editor { get; set; }

        [NotMapped]
        public string file_id { get; set; }

        public virtual List<User> Users { get; set; }

    }
}
