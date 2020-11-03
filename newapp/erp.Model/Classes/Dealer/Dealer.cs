using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Data;
using System.ComponentModel.DataAnnotations;
using erp.Model.Properties;

namespace erp.Model
{
    public enum DealerBrandStatus
    {
        Empty = -1,
        Platinum = 4,
        Gold=3,
        Silver=2,
        Bronze=1,
        Standard=0
    }

    public class Dealer
    {

        public Dealer()
        {
            hide_1 = DealerStatus_Inactive;
            brand_status = DealerBrandStatus.Standard;
            opening1_from = opening2_from = opening3_from = opening4_from = opening5_from = opening6_from = Settings.Default.OpeningTime_WorkingDay;
            opening1_to = opening2_to = opening3_to = opening4_to = opening5_to = opening6_to = Settings.Default.ClosingTime_WorkingDay;
            opening7_from = opening7_to = "closed";

        }

        public const int DealerStatus_Inactive = 0;
        public const int DealerStatus_Active = 1;
        public const int DealerStatus_Cancelled = 2;
        public const int DealerStatus_WaitingForImagePolicyAcceptance = 3;

        public int user_id { get; set; }
        [Required(ErrorMessageResourceName = "Dealer_Validation_name", ErrorMessageResourceType = typeof(Resources))]
        public string user_name { get; set; }
        public string user_account { get; set; }
        public string customer_code { get; set; }
        public Nullable<int> distributor { get; set; }
        public string user_welcomename { get; set; }
        public string user_address1 { get; set; }
        public string user_address2 { get; set; }
        public string user_address3 { get; set; }
        public string user_address4 { get; set; }
        public string user_address5 { get; set; }
        public string postcode { get; set; }
        public Nullable<int> ie_region { get; set; }
        public string user_country { get; set; }
        public string user_contact { get; set; }
        public string user_tel { get; set; }
        public string user_fax { get; set; }
        public string user_mobile { get; set; }
        public string user_website { get; set; }

        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}", ErrorMessage = "Email is not valid.")]
        [Required(ErrorMessage = "The email address is required")]//(ErrorMessageResourceName = "Dealer_Validation_email", ErrorMessageResourceType = typeof(Resources))]  NE POSTOJi RESURS U Resources.resx
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        [DataType(DataType.EmailAddress)]
        public string user_email { get; set; }

        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}", ErrorMessage = "Email is not valid.")]
        public string user_email2 { get; set; }
        public Nullable<int> user_type { get; set; }
        public Nullable<int> user_access { get; set; }
        public string user_pwd { get; set; }
        public Nullable<System.DateTime> user_created { get; set; }
        public Nullable<int> user_curr { get; set; }
        public Nullable<int> user_curr_pricing { get; set; }
        public Nullable<int> dynamic_pricing { get; set; }
        public Nullable<System.DateTime> lastlogin { get; set; }
        public Nullable<int> hide_1 { get; set; }
        public Nullable<int> gold_override { get; set; }
        public string opening { get; set; }
        public string directions1 { get; set; }
        public string directions2 { get; set; }
        public string directions3 { get; set; }
        public string directions4 { get; set; }
        public string image { get; set; }
        public Nullable<double> longitude { get; set; }
        public Nullable<double> latitude { get; set; }
        public string opening1_from { get; set; }
        public string opening1_to { get; set; }
        public string opening2_from { get; set; }
        public string opening2_to { get; set; }
        public string opening3_from { get; set; }
        public string opening3_to { get; set; }
        public string opening4_from { get; set; }
        public string opening4_to { get; set; }
        public string opening5_from { get; set; }
        public string opening5_to { get; set; }
        public string opening6_from { get; set; }
        public string opening6_to { get; set; }
        public string opening7_from { get; set; }
        public string opening7_to { get; set; }
        public Nullable<int> image_policy { get; set; }
        public Nullable<System.DateTime> image_policy_acceptance { get; set; }
        public string image_policy_ip { get; set; }
        public Nullable<int> training { get; set; }
        public Nullable<System.DateTime> training_date { get; set; }
        public Nullable<int> brand_b { get; set; }
        public Nullable<int> brand_wc { get; set; }
        public int? confirmed { get; set; }
        public int? default_brand { get; set; }
        public bool sales_registered { get; set; }
        public DateTime? user_modified { get; set; }
        public string brand_code { get; set; }
        public string cw_code { get; set; }
        public double? SqFeet { get; set; }
        public string SqFeetRange { get; set; }
        public double? AnnualTurnover { get; set; }
        public string AnnualTurnoverRange { get; set; }
        public int? created_by { get; set; }
        public int? modified_by { get; set; }
        public int? action_flag { get; set; }

        public bool? gold { get; set; }
        public bool? silver { get; set; }

        public DealerBrandStatus brand_status { get; set; }
        public DealerBrandStatus? brand_status_manual { get; set; }

        public int distributor_id { get; set; }
        public string DistributorName { get; set; }
        public string DistributorEmail { get; set; }

        public int numOfImages { get; set; }
        public int numOfDisplays { get; set; }
        public int? customer_type { get; set; }
        public int? sales_registered_2014 { get; set; }
        public bool sales_registered_2015 { get; set; }
        public bool sales_registered_2016 { get; set; }
        public bool sales_registered_2017 { get; set; }


        public string signoff_form { get; set; }
        public DateTime? signoff_date { get; set; }
        public int? signoff_displaysets { get; set; }
        public string survey_id { get; set; }
        public int? sequence { get; set; }


        public virtual List<Dealer_images> Dealer_Images { get; set; }
        //public List<Dealer_displays> DisplayedProducts { get; set; }
        public virtual List<Dealer_displays_activity> DisplayActivities { get; set; }

        public virtual List<Company> Distributors { get; set; }

        public Dictionary<int, DealerBrandStatus> BrandStatuses { get; set; }

        public List<Dealer_display_rebate> DisplayRebates { get; set; }

        [NotMapped]
        public List<Dealer_image_displays> AllDisplays
        {
            get
            {
                var result = new List<Dealer_image_displays>();
                if (Dealer_Images != null)
                {
                    foreach (var img in Dealer_Images)
                    {
                        if(img.Displays != null)
                            result.AddRange(img.Displays);
                    }
                }
                return result;
            }

        }

        public virtual List<Dealer_brandstatus> DealerBrandstatuses { get; set; }

        public DealerBrandStatus BrandStatusFinal
        {
            get { return brand_status_manual ?? brand_status; }
        }

        public bool IsGold
        {
            get
            {
                return (gold != null && gold.Value) || gold_override == 1 || (brand_status == DealerBrandStatus.Gold) || (brand_status_manual == DealerBrandStatus.Gold) ;
            }
        }

        public bool IsSilver
        {
            get
            {
                return (silver != null && silver.Value || brand_status == DealerBrandStatus.Silver || brand_status_manual == DealerBrandStatus.Silver) && !IsGold;
            }
        }

        //OLD property retained for compatibility with Burlington
        public DealerBrandStatus BrandStatus
        {
            get
            {
                if (IsGold)
                    return DealerBrandStatus.Gold;
                if (IsSilver)
                    return DealerBrandStatus.Silver;
                return BrandStatusFinal;
            }
        }

        public double Distance { get; set; }

        //old version, maintained for backward compatibility
        [NotMapped]
        public List<UserImage> Images { get; set; }

        public string DistributorCode { get; set; }

        [NotMapped]
        public virtual List<User_comment> UserComments { get; set; }
        [NotMapped]
        public virtual List<Client_sales_data> SalesData { get; set; }
        [NotMapped]
        public bool digital { get; set; }
    }

    public class UserImage
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class DealerProduct
    {
        public bool IsNew { get; set; }
        public Dealer Dealer { get; set; }
        //public WebProduct Product { get; set; }
        public int Qty { get; set; }
    }

    public class DealerStat
    {
        public int brand_id { get; set; }
        public int distributor_id { get; set; }
        public string distributor_code { get; set; }
        public int Count { get; set; }
        public int NumberOfDealersWithPics { get; set; }
    }

    public class DealerMultiBrandStat
    {
        public string distributor_code { get; set; }
        public int BrandCount { get; set; }
        public int DealerCount { get; set; }

    }

    public class DealerMultiBrandStat2
    {
        public Dealer Dealer { get; set; }
        //public string distributor_code { get; set; }
        public string brand_code { get; set; }
        public string brandname { get; set; }

    }

    public class DealerBrandInfo
    {
        public int user_id { get; set; }
        public int brand_id { get; set; }
    }

    public class DealerBrandStatusSummary
    {
        public int brand_id { get; set; }
        public DealerBrandStatus BrandStatus { get; set; }
        public int DealerCount { get; set; }
    }

    /***/

    public class PostcodeAreas
    {
        public int Id { get; set; }
        public string PostcodeArea { get; set; }
        public string PostTown { get; set; }
        public int NumOfRegion { get; set; }
        public string Region { get; set; }
    }

    public class Dot
    {
        public int id { get; set; }
        public double? latitude { get; set; }
        public double? longitude { get; set; }
        public string name { get; set; }
        public int radius{get;set;}

        public int number { get; set; }

        public string distributor { get; set; }

        public string postcode { get; set; }

        public string location { get; set; }

        public string user_address1 { get; set; }

        public string user_address2 { get; set; }

        public string user_address3 { get; set; }

        public string user_address4 { get; set; }

        public string user_address5 { get; set; }

        public string country { get; set; }
        public List<Company> Distributors { get; set; }
        public Web_site Brand { get; set; }



        public int action_flag { get; set; }
    }
    /// <summary>
    /// Lista korisnika,i podaci o prikazivanju images lat, lng
    /// </summary>
    public class DealerImagesWebOnRegion
    {
        public int user_id { get; set; }
        public int image_id { get; set; }
        public string user_name { get; set; }
        public string postcode { get; set; }
        public string dealer_image { get; set; }
        public string web_name { get; set; }
        public int? web_category { get; set; }
        public string web_site { get; set; }
        public int? web_sub_category { get; set; }
        public string category_name { get; set; }

        public string brand_sub_desc { get; set; }
        public int numbOfRegion { get; set; }

        public int? brand { get; set; }

        public double? latitude { get; set; }

        public double? longitude { get; set; }
        public string textInfoBox { get; set; }

        public int? web_site_id { get; set; }

        public int web_unique { get; set; }

        public int distributor_id { get; set; }

        public string reporting_name { get; set; }
    }

    /***/
}
