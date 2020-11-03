using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Instructions.Model
{
    public class SearchResult
    {
        //public Chapter Chapter { get; set; }
        //public Section Section { get; set; }
        //public Detail Detail { get; set; }
        public int? chapter_id { get; set; }
        public string title { get; set; }
        public int? section_id { get; set; }
        public string detail { get; set; }
    }
}
