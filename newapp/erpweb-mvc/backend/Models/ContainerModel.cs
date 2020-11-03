using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using erp.Model;

namespace backend.Models
{
    public class ContainerModel
    {
        public List<Mast_products> PacksData { get; set; }
        public List<Container_types> ContainersData { get; set; }

        public List<ContainerCalculation> CalculationData { get; set; }
    }

    //public class Mast_products : Mast_products {
    //    public int broj { get; set; } 
    //}
    public class ContainerCalculation  
    {
        public int Mast_id{get;set;}

        public string Pack_carton { get; set; }
        public double? PackCartonLen  { get; set; }
        public double? PackCartonWidth  { get; set; }
        public double? PackCartonHeight { get; set; }
        
        public double? PaletLen{ get; set; }
        public double? PaletWidth { get; set; }
        public double? PaletHeight{ get; set; }

        public int? UnitsInCarton{ get; set; }
        public int? UnitsPerPaletSingle { get; set; }
        
        /*Rezultati */
        public int? Pallet40gp { get; set; }
        public int? UnitsIn40gp{ get; set; }
                  
        public int? Pallet20gp { get; set; }
        public int? UnitsIn20gp { get; set; }
                  
        public int? Pallet40hc { get; set; }
        public int? UnitIn40hc { get; set; }

        public ContainerFill UnitInContainer { get; set; }
        /**/
        public int? Pallets_per_20 { get; set; }
        public int? Pallets_per_40 { get; set; }
        public int? Units_per_20pallet { get; set; }
        public int? Units_per_20nopallet { get; set; }
        public int? Units_per_40pallet_gp { get; set; }
        public int? Units_per_40pallet_hc { get; set; }
        public int? Untis_per_40nopallet_gp { get; set; }
        public int? Untis_per_40nopallet_hc { get; set; }

    }
    public class ContainerFill {
        public int Pallet { get; set; }
        public int UnitsOnPalets { get; set; }
        public int UnitsNoPalets { get; set; }
    }
}