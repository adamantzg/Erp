using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace backend.Models
{
    public class jsTreeNode
    {
        public string data { get; set; }
        public jsTreeNodeAttr attr { get; set; }
        public string state { get; set; }
        public jsTreeNode[] children { get; set; }
    }

    public class jsTreeNodeAttr
    {
        public int id { get; set; }
        public int? parentid { get; set; }
        public int? ChildCount { get; set; }
        public int? LeafCount { get; set; }
    }
}