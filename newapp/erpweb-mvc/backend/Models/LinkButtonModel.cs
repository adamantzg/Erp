using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace backend.Models
{
    public class LinkButtonModel
    {
        public string Text { get; set; }
        public string NavigateUrl { get; set; }
        public string CssClass { get; set; }
        public int TabIndex { get; set; }

        public LinkButtonModel()
        {
            CssClass = "linkbutton";
            TabIndex = -1;
        }
    }
}