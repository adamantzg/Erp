using erp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace backend.Models
{
    public class UserAdministrationModel
    {
        public List<Role> Roles { get; set; }
        public List<TreeNode> TreeNodes { get; set; }
    }

    public class TreeNode
    {
        public string text { get; set; }
        public List<TreeNode> children { get; set; }
        public TreeNode Parent { get; set; }
        public int? parent_id { get; set; }
        public int id { get; set; }
        public Navigation_item Item { get; set; }
    }
}