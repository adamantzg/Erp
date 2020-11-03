using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace erp.Model
{
    [NotMapped]
    public class DAM_functions
    {
        public int id { get; set; }
        public string name { get; set; }
    }
}
