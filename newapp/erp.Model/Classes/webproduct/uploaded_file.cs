using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace erp.Model
{
    [NotMapped]
    public partial class Uploaded_file
    {
        
        public int id { get; set; }
        public string name { get; set; }
        public int type_id { get; set; }
        public DateTime upload_date { get; set; }
        public int upload_user { get; set; }
        public bool related { get; set; }
        public string comment { get; set; }
        public string folder { get; set; }
    }
}
