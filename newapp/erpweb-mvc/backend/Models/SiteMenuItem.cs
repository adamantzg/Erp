using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using erp.Model;

namespace backend.Models
{
    public class SiteMenuItem
    {
        public int Id { get; set; }
        public int ParentId { get; set; }
        public string Text { get; set; }
        public string Url { get; set; }
        public bool OpenInNewWindow { get; set; }
        public int SortOrder { get; set; }
        public string ImageUrl { get; set; }

        public List<SiteMenuItem> Children { get; set; }
        public SiteMenuItem Parent { get; set; }
        public object DataItem { get; set; }
    }
}