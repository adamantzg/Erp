using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace erp.Model
{
    [NotMapped]
    public class Jobs
    {
        [DisplayName("Serial Number")]
        public int id { get; set; }
        [DisplayName("Position")]
        public string position { get; set; }
        [DisplayName("Company")]
        public string company { get; set; }
        [DisplayName("Description")]
        public string description { get; set; }
        [DisplayName("Qualifications")]
        public string qualifications { get; set; }
        [DisplayName("Location")]
        public string location { get; set; }
        [DisplayName("City")]
        public string city { get; set; }
        [DisplayName("Date posted")]
        public DateTime date_posted { get; set; }
        [DisplayName("Date valid")]
        public DateTime? date_valid { get; set; }
        [DisplayName("Language")]
        public int? language { get; set; }
        [DisplayName("Type")]
        public int? type { get; set; }
        public int? type2 { get; set; }
        public string googlemap_link { get; set; }
    }
}
