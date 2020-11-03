using erp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace backend.Models
{
    public class TcDocument
    {
        public tc_document Current { get; set;}
        public List<tc_document> PreviousVersions { get; set; }
        public List<TcUser> Users { get; set; }
    }

    public class TcUser
    {
        public List<string> roles { get; set; }
        public int userid { get; set; }
        public string name { get; set; }
    }
}