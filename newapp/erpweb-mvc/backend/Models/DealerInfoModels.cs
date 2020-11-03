using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using erp.Model;


namespace DealerInfo.Models
{
    public class DealerModel
    {
        public Dealer Dealer { get; set; }
        public List<Brand> Brands { get; set; }
        public List<Web_site> WebSites { get; set; }
        public List<Brand> BrandsForPictures { get; set; }
        public List<Brand> BrandsForDefaultDdl { get; set; }
        public List<Brand> BrandsForProducts { get; set; }
        public UserRole OverriddenRole { get; set; }
        public List<Lookup> BrandStatuses { get; set; }
        public bool ContinueEditing { get; set; }
        public List<CheckBoxItem> Distributors { get; set; }
        public double? ArcadeDisplayRebateNew { get; set; }
        public double? GeneralDisplayRebateNew { get; set; }
        public double? ArcadeSalesRebateNew { get; set; }
        public List<Dealer_images> DealerImages { get; set; }
        public List<Dealer_image_displays> DealerDisplays { get; set; }
        public List<Picture> UploadedFiles { get; set; }
        public List<Dealer_displays_activity_bydateandbrand> DisplayActivities { get; set; }
        public List<Invoices> DisplayRebates { get; set; }
        public List<Invoices> ArcadeSalesRebates { get; set; }
        public List<Countries> Countries { get; set; }
        public List<DealerBrandStatusDisplay> DealerBrandStatuses { get; set; }
        //public List<OpenCloseTime> OpenTimes { get; set; }
        //public List<OpenCloseTime> CloseTimes { get; set; }

        public DealerModel()
        {
            OverriddenRole = UserRole.Empty;

            BrandStatuses = new List<Lookup>
                {
                    new Lookup {Id = -1, Title = "By Displays"},
                    new Lookup {Id = (int) DealerBrandStatus.Standard, Title = DealerBrandStatus.Standard.ToString()},
                    new Lookup {Id = (int) DealerBrandStatus.Bronze, Title = DealerBrandStatus.Bronze.ToString()},
                    new Lookup {Id = (int) DealerBrandStatus.Silver, Title = DealerBrandStatus.Silver.ToString()},
                    new Lookup {Id = (int) DealerBrandStatus.Gold, Title = DealerBrandStatus.Gold.ToString()}
                };
        }
    }

    

    public class CheckBoxItem
    {
        public int Code { get; set; }
        public bool IsChecked { get; set; }
        public string Label { get; set; }
    }

    public class DealerViewModel
    {
        public Dealer Dealer { get; set; }
        public int NextDealerId { get; set; }
        public int PrevDealerId { get; set; }
        public int FirstDealerId { get; set; }
        public bool ShowPdfButton { get; set; }
        public bool PrintMode { get; set; }
        public DateTime ArcadeDealerSince { get; set; }
        public double TotalDisplaysValue { get; set; }
    }

    public class DealerImagesModel
    {
        public int image_unique { get; set; }
        public int? dealer_id { get; set; }
        public string dealer_image { get; set; }
        public int? seq { get; set; }
        public int? hide { get; set; }
        public int? reviewed { get; set; }
        public bool? store_page { get; set; }

        public List<Brand> Brands { get; set; }
        public virtual List<Dealer_image_displays> Displays { get; set; }

        public Dealer Dealer { get; set; }

        public long length { get; set; }
    }

    public class DealerListModel
    {
        public Dealer Dealer { get; set; }
        public List<Dealer_images> DealerImages { get; set; }
        public List<Dealer_image_displays> DealerDisplays { get; set; }
        public int NextDealerId { get; set; }
        public int PrevDealerId { get; set; }
        public int FirstDealerId { get; set; }

    }

    public class DealersListModel
    {
        public List<Dealer> Dealers { get; set; }
        public List<double> RetailValues { get; set; }
        public List<int> DisplaysCount { get; set; }
        public List<int> PhotosCount { get; set; }
    }

    public class Picture
    {
        public int PictureId { get; set; }
        public IEnumerable<HttpPostedFile> Image { get; set; }
        public string Name { get; set; }
        public long Size { get; set; }
        public string Path { get; set; }
    }

    public class DealerDisplayHistoryModel
    {
        public string Code { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public double Value { get; set; }
    }

    public class DealerBrandStatusDisplay : Dealer_brandstatus
    {
        public int? display_status { get; set; }
    }

}