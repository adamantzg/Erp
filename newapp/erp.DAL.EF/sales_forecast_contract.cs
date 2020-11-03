using erp.Model;

namespace erp.DAL.EF
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("asaq.sales_forecast_contracts")]
    public partial class sales_forecast_contract
    {
        [Key]
        public int sales_unique { get; set; }

        public int? cprod_id { get; set; }

        public int? sales_qty { get; set; }

        public int? month21 { get; set; }

        public virtual Cust_products Product { get; set; }
    }
}
