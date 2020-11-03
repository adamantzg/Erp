using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace erp.Model
{
    [Table("component_factory_product_table")]
    public partial class MastProduct_Component
    {
        [Key]
        public int component_product_id { get; set; }
        public int? component_id { get; set; }
        public int? mast_id { get; set; }
        [Column("component_product_per")]
        public int? qty { get; set; }
        [Column("show_on_invoice2")]
        public int? show_on_invoice { get; set; }

        public virtual ProductComponent Component { get;set; }
        public virtual Mast_products MastProduct { get; set; }
	
	}
}	
	