using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using erp.Model;

namespace backend.Models
{
    public class NpdListModel
    {
        public List<Category1> Categories { get; set; }
        public List<Npd> Npds { get; set; }
        public int? category_id { get; set; }
        public string text { get; set; }
    }

    public class NpdModel
    {
        public List<Category1> Categories { get; set; }
        public List<Npd> Npds { get; set; }
        public List<Brand> Brands { get; set; }
        public List<Npd_filetype> FileTypes { get; set; }
        public List<Npd_status> Statuses { get; set; }
        public Npd Npd { get; set; }
        public bool CanViewInternalComments { get; set; }
        public bool CanEditInternalComments { get; set; }
        public bool CanViewExternalComments { get; set; }
        public bool CanEditExternalComments { get; set; }
        public string User { get; set; }

        public NpdModel()
        {
            CanEditExternalComments = true;
            CanViewExternalComments = true;
            CanEditInternalComments = true;
            CanViewInternalComments = true;
        }
    }

    public class NpdEditModel
    {
        public List<Category1> Categories { get; set; }
        
        public List<CheckBoxItem> Brands { get; set; }
        public List<Npd_status> Statuses { get; set; }
        public Npd Npd { get; set; }
        public EditMode EditMode { get; set; }
        public string NewComment { get; set; }
        public Cust_products cprod { get; set; }
    }

    
}