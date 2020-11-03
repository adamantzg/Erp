using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace asaq2.Model
{
    public partial class Downloaded_file
    {
        [NotMapped]
        public int id { get; set; }
        [NotMapped]
        public int web_unique { get; set; }
        [NotMapped]
        public int user { get; set; }
        [NotMapped]
        public DateTime date { get; set; }
        [NotMapped]
        public int file_type { get; set; }
        [NotMapped]
        public string fileext { get; set; }
    }
}