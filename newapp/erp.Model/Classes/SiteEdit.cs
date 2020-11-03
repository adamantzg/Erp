using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace erp.Model
{
    public class SiteEdit
    {
        public Brand Brand { get; set; }
        public Language Lang { get; set; }
        public bool CanEdit { get; set; }
    }
}
