using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using erp.Model;

namespace backend.Models
{
    public class ImageModel
    {
        public ImageFile  Image { get; set; }
        public IEnumerable<ImageFile> Images { get; set; }

        public List<ImageFile> ImagesList { get; set; }
    }
}