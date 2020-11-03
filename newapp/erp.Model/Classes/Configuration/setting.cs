using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace erp.Model
{
    public class ConfigSetting
    {
        public int id { get; set; }
        public string name { get; set; }
        public int idApplication { get; set; }
        public int? idWebSite { get; set; }
        public string value { get; set; }

        public ConfigApplication Application { get; set; }
        public Web_site WebSite { get; set; }
    }
}
