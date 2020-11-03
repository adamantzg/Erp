using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace erp.Model
{
    public partial class Us_call_log_images
    {
        [Key]
        public int image_unique { get; set; }
        public int log_id { get; set; }
        public string log_image { get; set; }
        //public int user_type { get; set; }
        //public int cc_use { get; set; }
        //public int added_by { get; set; }
        public DateTime added_date { get; set; }
        //public int file_category { get; set; }
    }
}
