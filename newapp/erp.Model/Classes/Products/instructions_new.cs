using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace erp.Model
{
    public class instructions_new
    {
        public int id { get; set; }
        public string filename { get; set; }
        public int? language_id { get; set; }
        public DateTime? dateCreated { get; set; }
        public int? created_by { get; set; }
        
        public User CreatedBy { get; set; }
        public List<Mast_products> Products { get; set; }
        public Language Language { get; set; }

        [NotMapped]
        public string file_id { get; set; }
    }
}
