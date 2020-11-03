using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace erp.Model
{
    public partial class Web_product_new
    {
        public string option_type;
        public string marketing_description { get; set; }

        public virtual Web_category Category { get; set; }

        public virtual List<Web_product_info> ProductInfo { get; set; }
        public virtual List<Web_product_flow> ProductFlows { get; set; }
        public virtual List<Web_product_file> WebFiles { get; set; }
        public virtual List<Web_product_component> Components { get; set; }
        public virtual List<Web_category> SelectedCategories { get; set; }
        public virtual List<Sale> SelectedSalePeriods { get; set; }
        public virtual List<Web_product_new> RelatedProducts { get; set; }
        

        //public virtual ICollection<Web_product_new> CompatibleProducts { get; set; }
        //public virtual ICollection<Web_product_new> SuggestedProducts { get; set; }


        //public virtual ICollection<Web_product_new> InRelation { get; set; }    //Products that relate to this product

        //public virtual List<Dealer_image_displays> DealerImageDisplays { get; set; }

        public ICollection<Web_product_new> Siblings { get; set; }
        public ICollection<Web_product_new> Children { get; set; }

        public List<Dealer_external_display> DealerExternalDisplays { get; set; }

        public virtual  Web_product_new Parent { get; set; }
        public virtual Web_site WebSite { get; set; }

        public bool ImageExists { get; set; }
        public bool ThumbExists { get; set; }

        public int? whitebook_template_id { get; set; }

        public List<Whitebook_option> WhitebookOptions { get; set; }
        public Whitebook_template WhitebookTemplate { get; set; }

        private string whitebookOptionsCode { get; set; }
        private string whitebookOptionsCodeNoChildren { get; set; }

        //   public  List<Whitebook_option> ChildOption{ get; set; }
        //  public virtual Whitebook_option_group Group { get; set; }
        public string WhitebookOptionsCode
        {
            get
            {

                if (string.IsNullOrEmpty(whitebookOptionsCode) && WhitebookOptions != null && WhitebookTemplate != null) {
                    whitebookOptionsCode = GetWhiteBookOptionsCode();
                }
                return whitebookOptionsCode;
            }
        }

        private string GetWhiteBookOptionsCode(bool includeChildSequence = true)
        {
            var whitebookOptionsCode = string.Empty;
            foreach (var wo in WhitebookTemplate.OptionGroups.OrderBy(g => g.sequence)) {
                var option = WhitebookOptions.FirstOrDefault(o => o.group_id == wo.group_id);
                string code;
                if (option != null)
                    code = option.sequence.ToString("00");
                else {
                    option = WhitebookOptions.FirstOrDefault(o => o.Parent?.group_id == wo.group_id);
                    if (option != null)
                        code = option.Parent.sequence.ToString("00") + (includeChildSequence ? option.sequence.ToString("00") : "");
                    else
                        code = "00";
                }
                whitebookOptionsCode += code;
            }
            return whitebookOptionsCode;
        }

        public string WhitebookOptionsCodeNoChildren
        {
            get
            {
                if (string.IsNullOrEmpty(whitebookOptionsCodeNoChildren) && WhitebookOptions != null && WhitebookTemplate != null) {
                    whitebookOptionsCodeNoChildren = GetWhiteBookOptionsCode(false);
                }
                return whitebookOptionsCodeNoChildren;
            }
        }

        public string WebCode
        {
            get { return string.IsNullOrEmpty(web_code_override) ? (web_code ?? string.Empty).Trim() : (web_code_override ?? string.Empty).Trim(); }
        }

        public double Price
        {
            get
            {
                double? sum = Components != null ? Components.Where(c=>c.Component != null).Sum(c => c.qty*(c.Component.cprod_retail_web_override > 0 ? c.Component.cprod_retail_web_override : c.Component.cprod_retail)) : null;
                if (sum != null)
                    return sum.Value;
                return 0;
            }
        }
        public double PriceII
        {
            get
            {
                double? sum = Components != null ? Components.Where(c => c.Component != null).Sum(c => c.qty * (c.Component.cprod_retail)) : null;

            if(sum != null)
                return sum.Value;
           return 0;
            }
        }

        public double SalePrice
        {
            get
            {
                double? sum = Components != null ? Components.Where(c => c.Component != null).Sum(c => c.qty * (Convert.ToDouble(c.Component.sale_retail)>0?c.Component.sale_retail:c.Component.cprod_retail)) : null;
                if (sum != null)
                    return sum.Value;
                return 0;
            }
        }

        public Web_product_component FirstComponent
        {
            get { return Components == null ? null : Components.FirstOrDefault(c => c.order == 1); }
        }

        public int category_id { get; set; }
        //public virtual List<Web_product_file> WebProductFiles { get; set; }
        //public ICollection<Web_product_component> WebProductComponent { get; set; }

        public Web_product_file GetFileByType(int type)
        {
            if(this.WebFiles != null)
            {
                var file= this.WebFiles.FirstOrDefault(f => f.file_type == type);
                return file;
            }
            return null;
        }


        public string Material
        {
            get { return !string.IsNullOrEmpty(whitebook_material) ? whitebook_material : tech_material; }
        }



    }
}
