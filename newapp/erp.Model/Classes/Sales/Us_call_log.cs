using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace erp.Model
{
    [Table("us_call_log")]
    public partial class Us_call_log
    {
        [Key]
        public int id { get; set; }
        public string dealer { get; set; }
        public int? userid { get; set; }
        public string note { get; set; }
        public DateTime? date_created { get; set; }
        public DateTime? date_edit { get; set; }

        public string person { get; set; }
        public int? type { get; set; }
        public int? in_out { get; set; }
        public int? category_id { get; set; }
        public string fedex_ref { get; set; }
        public string rl_note { get; set; }
        public string order_return_ref { get; set; }
        public int? parent_id { get; set; }
        public int? status { get; set; }
        public int? hide_person { get; set; }

        [NotMapped]
        public bool HasChildren { get; set; }

        [NotMapped]
        public DateTime? UsaDate { get; set; }

        public Us_dealers UsDealer { get; set; }

        public User User { get; set; }

        public Us_call_log_category Category { get; set; }

        public List<Us_call_log_category> Categories { get; set; }

        public List<Us_call_log_images> Images { get; set; }
        [NotMapped]
        public DateTime? SortDate { get; set; }
    }
}
