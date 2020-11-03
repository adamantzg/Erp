using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace erp.Model
{
    public class Invoice_export_settings
    {
        public string EAN { get; set; }
        public int id { get; set; }
        public int client_id { get; set; }
        public int FileNumber { get; set; }
    }
}
