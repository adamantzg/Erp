using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace erp.Model
{
    public class AutomatedTaskIterator
    {
        public int id { get; set; }
        public string parameterValues { get; set; }
        public string toValues { get; set; }
    }
}
