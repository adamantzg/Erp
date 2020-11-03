using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace backend.Models
{
    public class ListSortModel
    {
        public string action { get; set; }
        public string controller { get; set; }
        public string caption { get; set; }
        public string fieldName { get; set; }
        public string sortField { get; set; }
        public string sortDir { get; set; }
    }
}