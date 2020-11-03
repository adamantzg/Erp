using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;



namespace erp.Model
{
    public class AsaqColor
    {
        [Key]
        public int color_id { get; set; }
        public string color_hex_code { get; set; }
        public string color_description { get; set; }
    }
}
