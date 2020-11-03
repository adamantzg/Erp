using System;
using System.Collections.Generic;
using erp.Model;

namespace backend.Models
{
    public class DealerModel
    {
        public Dealer Dealer { get; set; }
        public List<Brand> Brands { get; set; }
        public List<Web_site> WebSites { get; set; }
        public List<Brand> BrandsForPictures { get; set; }
        public List<Brand> BrandsForDefaultDdl { get; set; }
        public List<Brand> BrandsForProducts { get; set; }
        //public List<Company> Distributors { get; set; }
        public UserRole OverriddenRole { get; set; }
        public List<Lookup> BrandStatuses { get; set; }
        public bool ContinueEditing { get; set; }
		//public List<CheckBoxItem> Distributors { get; set; }
        public List<CheckBoxItem2> Distributors { get; set; }
        public bool ShowDistributors { get; set; }
        public double? ArcadeDisplayRebateNew { get; set; }
        public double? GeneralDisplayRebateNew { get; set; }
        public double? ArcadeSalesRebateNew { get; set; }
        public List<Countries> Countries { get; set; }

        public DealerModel()
        {
            OverriddenRole = UserRole.Empty;

            BrandStatuses = new List<Lookup>
                {
                    new Lookup {Id = (int) DealerBrandStatus.Standard, Title = DealerBrandStatus.Standard.ToString()},
                    new Lookup {Id = (int) DealerBrandStatus.Bronze, Title = DealerBrandStatus.Bronze.ToString()},
                    new Lookup {Id = (int) DealerBrandStatus.Silver, Title = DealerBrandStatus.Silver.ToString()},
                    new Lookup {Id = (int) DealerBrandStatus.Gold, Title = DealerBrandStatus.Gold.ToString()}
                };
        }
    }

    public class DealerListModel
    {
        public List<Dealer> Dealers { get; set; }
        public Dictionary<int, List<Dealer>> DealersByBrands { get; set; } 
        public List<Brand> Brands { get; set; }
        public List<Web_site> WebSites { get; set; }
        public string brand_code { get; set; }
        public int? distributor_id { get; set; }
        public int NonDisplayingDealersCount { get; set; }
        public List<Dealer> PendingDealers { get; set; }
        public List<Dealer> ProcessedDealers { get; set; }
        public UserRole OverriddenRole { get; set; }

        public DealerListModel()
        {
            OverriddenRole = UserRole.Empty;
        }
    }

    public class DealerReportModel
    {
        public List<Brand> Brands { get; set; }
        public Dictionary<int, List<DealerStat>> BrandDealerStat { get; set; }
        public Dictionary<int, List<DealerStat>> BrandDealerStatSales { get; set; }
        public Dictionary<int, List<DealerStat>> BrandDealerStatAll { get; set; }
        public List<DealerMultiBrandStat> DistributorMultiBrandCount { get; set; }
        public List<DealerMultiBrandStat> DistributorMultiBrandCountSales { get; set; }
        public List<DealerMultiBrandStat> DistributorMultiBrandCountAll { get; set; }
        public List<DealerMultiBrandStat2> MultiBrandStats { get; set; }

        public DateTime From { get; set; }
        public DateTime To { get; set; }

        //public List<Company> Distributors { get; set; }
    }

    public class DealerBrandStatusReportModel
    {
        public List<DealerBrandStatusSummary> StatusSummaries { get; set; }
        public List<Brand> Brands { get; set; }
    }

    public class DealerImagesUnallocatedModel
    {
        public List<Brand> Brands { get; set; }
        public List<Dealer_images> Images { get; set; }
    }

    public class CWImportAdminModel
    {
        public List<CWCustomerItem> MatchedDealers { get; set; }
        public DateTime? From { get; set; }
    }

    public class CWCustomerItem
    {
        public string customer { get; set; }
        public string name { get; set; }
        public string address1 { get; set; }
        public string address2 { get; set; }
        public string address3 { get; set; }
        public string address4 { get; set; }
        public string address5 { get; set; }
        public string address6 { get; set; }
        public DealerItem Dealer { get; set; }
    }

    public class DealerItem
    {
        public int user_id { get; set; }
        public string user_name { get; set; }
        public string user_address1 { get; set; }
        public string user_address2 { get; set; }
        public string user_address3 { get; set; }
        public string user_address4 { get; set; }
        public string user_address5 { get; set; }
        public string postcode { get; set; }
        public string cw_code { get; set; }
    }

    public class DealerExportModel
    {
        public Dictionary<int, List<Dealer>> BrandDealers { get; set; }
        public List<Brand> Brands { get; set; }
        public List<Dealer> RegisteredDealers { get; set; }
    }

    public class DealerExportModel2
    {
        public List<Dealers_all_brands_new_export> DealersAllBrandsNewExports { get; set; }
        public List<Company> Distributors { get; set; }
    }

        

}