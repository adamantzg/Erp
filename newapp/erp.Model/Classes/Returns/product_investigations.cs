
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
//using System.Web.Mvc;

namespace erp.Model
{
	
	public partial class Product_investigations
	{
		public int id { get; set; }
		public int cprod_id { get; set; }
		public int? mast_id { get; set; }
		public DateTime? date { get; set; }
		public string monitored_by { get; set; }
		public int status { get; set; }

        //[Required]
        //[AllowHtml]
        [UIHint("tinymce_jquery_full")]
        //[Display(Name="Comments")]
		public string comments { get; set; }

        public Cust_products Product { get; set; }
	}
}
/*
     
 @*<div class="editor-label">
                    @Html.LabelFor(model=>model.StatusDetail.comments)
                </div>
                <div class="editor-field" style="width:1600px">
                    @Html.TextAreaFor(model => model.StatusDetail.comments, new { @class="mceEditor", id="textfield", name="textfield"})
                    @Html.ValidationMessageFor(model=>model.StatusDetail.comments)
                </div>*@
 */