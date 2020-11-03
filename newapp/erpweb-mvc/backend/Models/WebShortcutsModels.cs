using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using erp.Model;

namespace backend.Models
{
    public class WebShortcutsModel
    {
        public List<Web_shortcuts> Shortcuts { get; set; }
        public List<Brand> Brands { get; set; }
        public string BrandName { get; set; }
        public Web_shortcuts Web_shortcut { get; set; }
       
    }
}