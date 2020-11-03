using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace erp.Model
{
    public partial class change_notice_image
    {
        public int id { get; set; }
        public int notice_id { get; set; }
        public string notice_image { get; set; }
        public int type_id { get; set; }
        public int order { get; set; }
        public string comments { get; set; }

        public virtual Change_notice ChangeNotice { get; set; }
        public virtual change_notice_image_type ImageType { get; set; }
    
    }
}
