using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace backend.Models
{
    public class TimePeriodModel
    {
        public List<string> TimesFrom { get; set; }
        public List<string> TimesTo { get; set; }
        public string FromField { get; set; }
        public string ToField { get; set; }

    }
}