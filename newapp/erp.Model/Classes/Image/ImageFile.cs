using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace erp.Model
{
    public class ImageFile
    {
        public int IdImage { get; set; }
       // public char[] Thumbnail { get; set; }
        public byte[] BigImage { get; set; }
        public string Big { get; set; }
        public string Description { get; set; }
        
       // public string Name { get; set; }
    }


}
