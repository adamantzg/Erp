
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

using System.Reflection;
using company.Common;
using erp.Model.Properties;


namespace erp.Model
{

	public class Cust_products : ITrackable
	{

		public int? cprod_stock_lvh { get; set; }
		public string cprod_name { get; set; }
		public string cprod_name_web_override { get; set; }
		public string cprod_name2 { get; set; }
		public string whitebook_cprod_name { get; set; }
		public string cprod_code1 { get; set; }
		public string cprod_code1_web_override { get; set; }
		public string whitebook_cprod_code1 { get; set; }
		public string cprod_code2 { get; set; }
		public string cprod_image1 { get; set; }
		public string cprod_instructions2 { get; set; }
		public string cprod_instructions { get; set; }
		public string cprod_label { get; set; }
		public string cprod_packaging { get; set; }
		public string cprod_dwg { get; set; }
		public string cprod_spares { get; set; }
		public string cprod_pdf1 { get; set; }
		public string cprod_status { get; set; }
		public string cprod_status2 { get; set; }
		public string pack_image1 { get; set; }
		public string pack_image2 { get; set; }
		public string pack_image2b { get; set; }
		public string pack_image2c { get; set; }
		public string pack_image2d { get; set; }
		public string pack_image3 { get; set; }
		public string pack_image4 { get; set; }
		public string insp_level_a { get; set; }
		public string insp_level_D { get; set; }
		public string insp_level_F { get; set; }
		public string insp_level_M { get; set; }
		public string client_image { get; set; }
		public string cprod_track_image1 { get; set; }
		public string cprod_track_image2 { get; set; }
		public string cprod_track_image3 { get; set; }
		public string cprod_supplier { get; set; }
		public string client_range { get; set; }
		public string analysis_d { get; set; }
		public string bin_location { get; set; }
		public DateTime? cprod_opening_date { get; set; }
		public DateTime? cprod_stock_date { get; set; }
		public DateTime? cprod_pending_date { get; set; }
		public bool? EU_supplier { get; set; }
		public long? barcode { get; set; }
		public int cprod_id { get; set; }
		public int? cprod_mast { get; set; }
		public int? cprod_user { get; set; }
		public int? cprod_cgflag { get; set; }
		public int? cprod_curr { get; set; }
		public int? cprod_opening_qty { get; set; }
		public int? cprod_oldcode { get; set; }
		public int? cprod_lme { get; set; }
		public int? cprod_brand_cat { get; set; }
		public int? cprod_brand_subcat { get; set; }
		public int? cprod_disc { get; set; }
		public int? cprod_seq { get; set; }
		public int? cprod_stock_code { get; set; }
		public int? brand_grouping { get; set; }
		public int? b_gold { get; set; }
		public int? cprod_loading { get; set; }
		public int? moq { get; set; }
		public int? WC_2011 { get; set; }
		public int? cprod_stock { get; set; }
		public int? cprod_stock2 { get; set; }
		public int? cprod_priority { get; set; }
		public int? aql_A { get; set; }
		public int? aql_D { get; set; }
		public int? aql_F { get; set; }
		public int? aql_M { get; set; }
		public int? criteria_status { get; set; }
		public int? cprod_confirmed { get; set; }
		public int? tech_template { get; set; }
		public int? tech_template2 { get; set; }
		public int? cprod_returnable { get; set; }
		public int? client_cat1 { get; set; }
		public int? client_cat2 { get; set; }
		public int? bs_visible { get; set; }
		public int? original_cprod_id { get; set; }
		public int? cprod_range { get; set; }
		public int? cprod_combined_product { get; set; }
		public int? UK_production { get; set; }
		public int? report_exception { get; set; }
		public int? brand_userid { get; set; }
		public int? brand_id { get; set; }
		public int? buffer_stock_override_days { get; set; }
		public int? analytics_category { get; set; }
		public int? analytics_option { get; set; }
		public int? warning_report { get; set; }
		public int? cprod_special_payment_terms { get; set; }
		public int? product_type { get; set; }
		public bool? pending_discontinuation { get; set; }
		public int? cwb_stock_type { get; set; }
		public int? pallet_grouping { get; set; }
		public int? dist_status { get; set; }
		public bool? proposed_discontinuation { get; set; }
		public int? locked_sorder_qty { get; set; }
		public int? color_id { get; set; }
		public int? consolidated_port_override { get; set; }
		public int? stock_check { get; set; }
		public int? cust_product_range_id { get; set; }
		public int? discontinued_visible { get; set; }
		public double? cprod_price1 { get; set; }
		public double? cprod_price2 { get; set; }
		public double? cprod_price3 { get; set; }
		public double? cprod_price4 { get; set; }
		public double? cprod_retail { get; set; }
		public double? cprod_retail_uk { get; set; }
		public double? cprod_retail_pending { get; set; }
		public double? cprod_retail_web_override { get; set; }
		public double? cprod_old_retail { get; set; }
		public double? cprod_override_margin { get; set; }
		public double? days30_sales { get; set; }
		public double? cprod_pending_price { get; set; }
		public double? on_order_qty { get; set; }
		public double? sale_retail { get; set; }
		public double? wras { get; set; }
		public DateTime? cprod_retail_pending_date { get; set; }
		public int? product_group_id { get; set; }

		[NotMapped]
		public int? consolidated_port { get; set; }


		public virtual AsaqColor Color { get; set; }

		public virtual List<Sales_data> SalesProducts { get; set; }
		public List<Sales_forecast> SalesForecast { get; set; }

		public virtual Mast_products MastProduct { get; set; }

		public List<int> cprod_stock_codes { get; set; }

		public virtual List<Web_product_component> Products { get; set; }

		public virtual List<Order_lines> OrderLines { get; set; }

		public virtual List<Returns> Returns { get; set; }

		//Maybe one day
		//public virtual List<Returns> ReturnsMultiple { get; set; }

		public virtual Analytics_subcategory AnalyticsSubCategory { get; set; }

		public List<Dist_products> DistProducts { get; set; }

		[NotMapped]
		public virtual double? PriceWithVAT { get; set; }

		public List<Inspv2_template> Inspv2Templates { get; set; }

		public List<ProductPricingProject> Projects { get; set; }

		//public virtual List<WebProduct> Products { get; set; }

		public double? GetVolume()
		{
			if (MastProduct != null)
			{
				double? result = 0.0;
				if (MastProduct.pack_width > 0)
					result = MastProduct.pack_height * MastProduct.pack_width * MastProduct.pack_length;
				else if (MastProduct.carton_width > 0)
					result = MastProduct.units_per_carton > 0
									? (MastProduct.carton_height * MastProduct.carton_width * MastProduct.carton_length) /
									MastProduct.units_per_carton
									: 0;
				else if (MastProduct.pallet_width > 0)
					result = MastProduct.units_per_pallet_single > 0
									? (MastProduct.pallet_height > 0 ? MastProduct.pallet_height : 1000) *
									MastProduct.pallet_width * MastProduct.pallet_length : 0;
				result /= Math.Pow(1000, 3);
				return result;
			}
			return 0;
		}

		public double? GetPrice()
		{
			return MastProduct != null ? MastProduct.GetPriceGBP() : 0;
		}

		public double? StockGBP
		{
			get { return cprod_stock * GetPrice(); }
		}


		//public int? prod_type { get; set; }

		public virtual Product_type ProductType { get; set; }

		public virtual Brand Brand { get; set; }

		public virtual BrandCategory Category { get; set; }

		public List<Cust_products> Parents { get; set; }

		public Company BrandCompany { get; set; }

		public List<Market_product> MarketData { get; set; }

		public List<auto_add_products> AutoAddedProducts { get; set; }

		public List<File> OtherFiles { get; set; }

		[NotMapped]
		public double? Sales { get; set; }


		[NotMapped]
		public bool IsModified { get; set; }
		[NotMapped]
		public bool IsDeleted { get; set; }
		[NotMapped]
		public bool IsNew { get; set; }

		[NotMapped]
		public int? DisplayQty { get; set; }
		[NotMapped]
		public DateTime? FirstShipDate { get; set; }

		[NotMapped]
		public double? ComputedPrice { get; set; }

		[NotMapped]
		public Company Client { get; set; }

		[NotMapped]
		public List<CustProductFile> Files { get; set; }


		public const int typeProductPicture = 1;

		public const int typeDwgDrawing = 2;

		public const int typeBasicDrawing = 3;

		public const int typeDetailedDrawing = 4;

		public const int typeInstructions = 5;

		public const int typeLabel = 6;

		public const int typePackaging = 7;

		public const int typePackPicture = 8;

		public const int typeMainCarton1 = 9;

		public const int typeMainCarton2 = 10;

		public const int typeMainCarton3 = 11;

		public const int typeMainCarton4 = 12;

		public const int typeMasterCarton = 13;

		public const int typePallet = 14;


		public cust_products_range ProductRange { get; set; }

		public cust_products_extradata ExtraData { get; set; }

		public List<product_file> ProductFiles { get; set; }

		[NotMapped]
		public string factory_ref { get; set; }

		private Dictionary<int, string> typeLegacyFields = new Dictionary<int, string>
	{
		{typeProductPicture, "cprod_image1"},
		{typeDwgDrawing, "cprod_dwg"},
		{typeInstructions, "cprod_instructions"},
		{typeLabel, "cprod_label" },
		{typePackaging, "cprod_packaging" },
		{typePackPicture, "pack_image1" },
		{typeMainCarton1, "pack_image2" },
		{typeMainCarton2, "pack_image2b" },
		{typeMainCarton3, "pack_image2c" },
		{typeMainCarton4, "pack_image2d" },
		{typeMasterCarton, "pack_image3" },
		{typePallet, "pack_image4" }

	};

		public void FilesToLegacyFields()
		{
			if (Files != null)
			{
				foreach (var file in Files)
				{
					if (file.file_type_id != null && typeLegacyFields.ContainsKey(file.file_type_id.Value))
					{
						var prop = GetProperty(typeLegacyFields[file.file_type_id.Value]);
						if (prop != null)
							prop.SetValue(this, file.filename);

					}

				}
			}
			foreach (var kv in typeLegacyFields)
			{
				if (Files == null || Files.Count(f => f.file_type_id == kv.Key) == 0)
				{
					//NO type, set null
					var prop = GetProperty(typeLegacyFields[kv.Key]);
					if (prop != null)
						prop.SetValue(this, null);
				}
			}
		}


		public List<CustProductFile> LegacyFieldsToFiles()
		{
			var result = new List<CustProductFile>();
			foreach (var kv in typeLegacyFields)
			{
				var prop = GetProperty(kv.Value);
				string value;
				if (prop != null)
				{
					value = (prop.GetValue(this) ?? string.Empty).ToString();
					if (value.Length > 0)
					{
						result.Add(new CustProductFile { id = 1, file_type_id = kv.Key, filename = value });
					}

				}
			}
			return result;
		}

		private PropertyInfo GetProperty(string name)
		{
			return GetType().GetProperty(name);
		}

		public static implicit operator Cust_products(Web_product_new v)
		{
			throw new NotImplementedException();
		}
		
	}

    public class CustProductDistinctComparer : IEqualityComparer<Cust_products>
    {
        public bool Equals(Cust_products x, Cust_products y)
        {
            return x.cprod_id == y.cprod_id;
        }

        public int GetHashCode(Cust_products obj)
        {
            return obj.cprod_id.GetHashCode();
        }
    }

    public class CustProductCprodCodeDistinctComparer : IEqualityComparer<Cust_products>
    {
        public bool Equals(Cust_products x, Cust_products y)
        {
            return x.cprod_code1 == y.cprod_code1;
        }

        public int GetHashCode(Cust_products obj)
        {
            return obj.cprod_code1.GetHashCode();
        }
    }

}

