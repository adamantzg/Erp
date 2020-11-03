
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Text;
using company.Common;


namespace erp.Model
{
	
	public class Mast_products
	{

		public string factory_ref { get; set; }
		public string factory_name { get; set; }
		public string asaq_ref { get; set; }
		public string asaq_name { get; set; }
		public string prod_image1 { get; set; }
		public string prod_image2 { get; set; }
		public string prod_image3 { get; set; }
		public string prod_image4 { get; set; }
		public string prod_image5 { get; set; }
		public string prod_instructions2 { get; set; }
		public string prod_instructions { get; set; }
		public string prod_finish { get; set; }
		public string glass_thickness { get; set; }
		public string prod_material { get; set; }
		public string comments { get; set; }
		public string product_group { get; set; }
		public string product_group_override { get; set; }
		public string product_group_suggested { get; set; }
		public string special_comments { get; set; }
		public string original_product_group { get; set; }
		public string dimensions_note { get; set; }
		public DateTime? prod_createdate { get; set; }
		public DateTime? last_update { get; set; }
		[Key]
		public int mast_id { get; set; }
		public int? factory_id { get; set; }
		public int? stock_qty { get; set; }
		public int? packunits { get; set; }
		public int? units_per_carton { get; set; }
		public int? units_per_pallet_single { get; set; }
		public int? units_per_pallet_lower { get; set; }
		public int? units_per_pallet_upper { get; set; }
		public int? pallets_per_20 { get; set; }
		public int? pallets_per_40 { get; set; }
		public int? units_per_20pallet { get; set; }
		public int? units_per_20nopallet { get; set; }
		public int? units_per_40pallet_gp { get; set; }
		public int? units_per_40nopallet_gp { get; set; }
		public int? units_per_40pallet_hc { get; set; }
		public int? units_per_40nopallet_hc { get; set; }
		public int? prod_image1_w { get; set; }
		public int? prod_image1_h { get; set; }
		public int? min_ord_qty { get; set; }
		public int? category1 { get; set; }
		public int? category1_sub { get; set; }
		public int? category2 { get; set; }
		public int? catagory3 { get; set; }
		public int? product_status { get; set; }
		public int? prod_type { get; set; }
		public int? tariff_code { get; set; }
		public int? om_seq_number { get; set; }
		public int? range1 { get; set; }
		public int? lead_time { get; set; }
		public int? factory_moq { get; set; }
		public int? so_units { get; set; }
		public int? so_cartons { get; set; }
		public int? so_pallets { get; set; }
		public int? co_units { get; set; }
		public int? co_cartons { get; set; }
		public int? co_pallets { get; set; }
		public int? category_dop { get; set; }
		public int? factory_stock { get; set; }
		public int? stock_order_moq { get; set; }
		public double? price_dollar { get; set; }
		public double? price_euro { get; set; }
		public double? price_pound { get; set; }
		public double? price_dollar_ex { get; set; }
		public double? price_euro_ex { get; set; }
		public double? price_pound_ex { get; set; }
		public double? prod_length { get; set; }
		public double? prod_width { get; set; }
		public double? prod_height { get; set; }
		public double? prod_depth { get; set; }
		public double? prod_nw { get; set; }
		public double? pack_length { get; set; }
		public double? pack_width { get; set; }
		public double? pack_depth { get; set; }
		public double? pack_height { get; set; }
		public double? pack_GW { get; set; }
		public double? pack_loading_ratio { get; set; }
		public double? carton_width { get; set; }
		public double? carton_length { get; set; }
		public double? carton_height { get; set; }
		public double? carton_GW { get; set; }
		public double? pallet_width { get; set; }
		public double? pallet_length { get; set; }
		public double? pallet_height { get; set; }
		public double? pallet_height_upper { get; set; }
		public double? pallet_height_lower { get; set; }
		public double? pallet_weight { get; set; }
		public double? lme { get; set; }
		public double? product_group_CMA { get; set; }
		public double? pack2_l { get; set; }
		public double? pack2_w { get; set; }
		public double? pack2_h { get; set; }
		public double? pack2_gw { get; set; }
		public double? pack3_l { get; set; }
		public double? pack3_w { get; set; }
		public double? pack3_h { get; set; }
		public double? pack3_gw { get; set; }
		public double? pack4_l { get; set; }
		public double? pack4_w { get; set; }
		public double? pack4_h { get; set; }
		public double? pack4_gw { get; set; }
		public double? lme_adjust { get; set; }
		public double? flow01 { get; set; }
		public double? flow02 { get; set; }
		public double? flow05 { get; set; }
		public double? flow10 { get; set; }
		public double? flow20 { get; set; }
		public double? flow30 { get; set; }
		public double? aerator01 { get; set; }
		public double? aerator02 { get; set; }
		public double? aerator05 { get; set; }
		public double? aerator10 { get; set; }
		public double? aerator20 { get; set; }
		public double? aerator30 { get; set; }
		public double? pending_price_dollar { get; set; }
		public double? pending_price_pound { get; set; }
		public double? pending_price_euro { get; set; }
		public double? threemonths { get; set; }
		public double? maxweight_unit { get; set; }
		public double? maxweight_carton { get; set; }
		public double? maxweight_pallet { get; set; }
		public double? insp_check_override_percent { get; set; }
		public DateTime? pending_price_date { get; set; }

		public virtual Company Factory { get; set; }
		public virtual List<Cust_products> CustProducts { get; set; }

		public virtual Category1_sub SubCategory { get; set; }
		public virtual Category1 Category { get; set; }

		//public int? categorydop_id { get; set; }


		[NotMapped]
		public double? LastPoLinePrice { get; set; }

		public double? factory_stock_value
		{
			get
			{
				return factory_stock * (LastPoLinePrice ?? GetPriceGBP());
			}
		}

		public double? GetPriceGBP()
		{
			return price_pound > 0 ? price_pound : price_dollar / 1.6;
		}

		public double? GetPrice()
		{
			return price_pound > 0 ? price_pound : price_dollar > 0 ? price_dollar : price_euro;
		}

		[NotMapped]
		public int? SpecCount { get; set; }

		public virtual List<Mastproduct_characteristics> Characteristics { get; set; }

		public virtual List<Technical_product_data> TechnicalProductData { get; set; }

		public virtual List<MastProduct_Component> Components { get; set; }

		//[NotMapped]
		public virtual List<Porder_lines> PorderLines { get; set; }

		public virtual List<mastproduct_price> Prices { get; set; }

		
		public virtual ProductPricingMastProductData ProductPricingData { get; set; }

		public virtual List<Mast_products_packaging_material> PackagingMaterials { get; set; }

		[NotMapped]
		public List<MastProductFile> Files { get; set; }

		public List<product_file> ProductFiles { get; set; }

		public const int typeProductPicture = 1;
		public const int typeDwgDrawing = 2;
		public const int typeBasicDrawing = 3;
		public const int typeDetailedDrawing = 4;
		public const int typeInstructions = 5;
		public const int typeLabel = 6;
		public const int typePackaging = 7;

		public List<File> OtherFiles { get; set; }


		private Dictionary<int, string> typeLegacyFields = new Dictionary<int, string>
		{
			{typeProductPicture, "prod_image1"},
			{ typeDwgDrawing, "prod_image3"},
			{ typeInstructions, "prod_instructions"},
			{typeLabel, "prod_image4" },
			{typePackaging, "prod_image5" }
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

		public List<MastProductFile> LegacyFieldsToFiles()
		{
			var result = new List<MastProductFile>();
			foreach (var kv in typeLegacyFields)
			{
				var prop = GetProperty(kv.Value);
				string value;
				if (prop != null)
				{
					value = (prop.GetValue(this) ?? string.Empty).ToString();
					if (value.Length > 0)
					{
						result.Add(new MastProductFile { id = 1, file_type_id = kv.Key, filename = value });
					}

				}
			}
			return result;
		}

		private PropertyInfo GetProperty(string name)
		{
			return GetType().GetProperty(name);
		}

		public mastproduct_price DefaultPrice
		{
			get
			{
				if (Prices != null)
				{
					var mp = Prices.FirstOrDefault(p => p.isDefault == true);
					if (mp == null && Prices.Count > 0)
					{
						mp = Prices[0];
					}
					if (mp != null)
						return mp;
					return null;
				}
				return null;
			}
		}

		public int? CurrencyId
		{
			get
			{
				return DefaultPrice?.currency_id ??
				 (this.price_dollar > 0
					? Currencies.USD
					: price_pound > 0 ? Currencies.GBP : price_euro > 0 ? Currencies.EUR : Currencies.USD);
			}
		}

	}
}	
	