using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace asaq2.Model.Dal.New.Test
{
	[TestClass]
	[Table("mast_products")]
	public class MastProductTests : DatabaseTestBase
	{
		protected FileTests fileTests;

		public MastProductTests()
		{

		}

		public MastProductTests(IDbConnection conn) : base(conn)
		{
			
		}

		protected override string IdField => "mast_id";
		private PropertyInfo pInfo = null;

		[TestInitialize]
		public override void Init()
		{
			base.Init();
			fileTests = new FileTests(conn);
		}

		protected override string GetCreateSql()
		{
			return
				@"INSERT INTO `mast_products`
				(`mast_id`,`factory_id`,`factory_ref`,`factory_name`,`asaq_ref`,`asaq_name`,`price_dollar`,
				`price_euro`,`price_pound`,`price_dollar_ex`,`price_euro_ex`,`price_pound_ex`,`stock_qty`,
				`prod_length`,`prod_width`,`prod_height`,`prod_depth`,`prod_nw`,`pack_length`,`pack_width`,
				`pack_depth`,`pack_height`,`pack_GW`,`packunits`,`pack_loading_ratio`,`carton_width`,
				`carton_length`,`carton_height`,`carton_GW`,`units_per_carton`,`pallet_width`,
				`pallet_length`,`pallet_height`,`pallet_height_upper`,`pallet_height_lower`,`units_per_pallet_single`,
				`units_per_pallet_lower`,`units_per_pallet_upper`,`pallet_weight`,`pallets_per_20`,
				`pallets_per_40`,`units_per_20pallet`,`units_per_20nopallet`,`units_per_40pallet_gp`,
				`units_per_40nopallet_gp`,`units_per_40pallet_hc`,`units_per_40nopallet_hc`,`prod_image1`,
				`prod_image2`,`prod_image3`,`prod_image4`,`prod_image5`,`prod_instructions2`,`prod_instructions`,
				`prod_image1_w`,`prod_image1_h`,`min_ord_qty`,`category1`,`category1_sub`,`category2`,
				`catagory3`,`product_status`,`prod_finish`,`glass_thickness`,`prod_material`,`comments`,
				`prod_createdate`,`last_update`,`lme`,`product_group`,`product_group_override`,
				`product_group_suggested`,`product_group_CMA`,`pack2_l`,`pack2_w`,`pack2_h`,`pack2_gw`,
				`pack3_l`,`pack3_w`,`pack3_h`,`pack3_gw`,`pack4_l`,`pack4_w`,`pack4_h`,`pack4_gw`,
				`prod_type`,`tariff_code`,`lme_adjust`,`flow01`,`flow02`,`flow05`,`flow10`,`flow20`,
				`flow30`,`aerator01`,`aerator02`,`aerator05`,`aerator10`,`aerator20`,`aerator30`,
				`pending_price_dollar`,`pending_price_pound`,`pending_price_euro`,`pending_price_date`,
				`threemonths`,`special_comments`,`om_seq_number`,`range1`,`lead_time`,`factory_moq`,
				`original_product_group`,`so_units`,`so_cartons`,`so_pallets`,`co_units`,`co_cartons`,
				`co_pallets`,`maxweight_unit`,`maxweight_carton`,`maxweight_pallet`,`category_dop`,
				`factory_stock`,`dimensions_note`,`stock_order_moq`,`insp_check_override_percent`)
				VALUES
				(@mast_id,@factory_id,@factory_ref,@factory_name,@asaq_ref,@asaq_name,@price_dollar,@price_euro,
				@price_pound,@price_dollar_ex,@price_euro_ex,@price_pound_ex,@stock_qty,@prod_length,@prod_width,
				@prod_height,@prod_depth,@prod_nw,@pack_length,@pack_width,@pack_depth,@pack_height,
				@pack_GW,@packunits,@pack_loading_ratio,@carton_width,@carton_length,@carton_height,
				@carton_GW,@units_per_carton,@pallet_width,@pallet_length,@pallet_height,@pallet_height_upper,
				@pallet_height_lower,@units_per_pallet_single,@units_per_pallet_lower,@units_per_pallet_upper,
				@pallet_weight,@pallets_per_20,@pallets_per_40,@units_per_20pallet,@units_per_20nopallet,
				@units_per_40pallet_gp,@units_per_40nopallet_gp,@units_per_40pallet_hc,@units_per_40nopallet_hc,
				@prod_image1,@prod_image2,@prod_image3,@prod_image4,@prod_image5,@prod_instructions2,
				@prod_instructions,@prod_image1_w,@prod_image1_h,@min_ord_qty,@category1,@category1_sub,
				@category2,@catagory3,@product_status,@prod_finish,@glass_thickness,@prod_material,
				@comments,@prod_createdate,@last_update,@lme,@product_group,@product_group_override,
				@product_group_suggested,@product_group_CMA,@pack2_l,@pack2_w,@pack2_h,@pack2_gw,@pack3_l,
				@pack3_w,@pack3_h,@pack3_gw,@pack4_l,@pack4_w,@pack4_h,@pack4_gw,@prod_type,@tariff_code,
				@lme_adjust,@flow01,@flow02,@flow05,@flow10,@flow20,@flow30,@aerator01,@aerator02,@aerator05,
				@aerator10,@aerator20,@aerator30,@pending_price_dollar,@pending_price_pound,@pending_price_euro,
				@pending_price_date,@threemonths,@special_comments,@om_seq_number,@range1,@lead_time,@factory_moq,
				@original_product_group,@so_units,@so_cartons,@so_pallets,@co_units,@co_cartons,@co_pallets,
				@maxweight_unit,@maxweight_carton,@maxweight_pallet,@category_dop,@factory_stock,@dimensions_note,
				@stock_order_moq,@insp_check_override_percent)";
		}

		protected string GetPackagingMaterialCreateSql()
		{
			return @"INSERT INTO `asaq`.`mast_products_packaging_material`
					(`id`,	`mast_id`,	`packaging_id`,	`material_id`,`amount`)
					VALUES (@id,@mast_id,@packaging_id,	@material_id,@amount)";
		}

		public override void GenerateRecord<T>(T data, PropertyInfo propId = null, IDbConnection conn = null)
		{
			GenerateRecord(data as Mast_products, conn);
		}

		public void GenerateRecord(Mast_products data, IDbConnection conn = null)
		{
			var c = conn ?? this.conn;
			if(pInfo == null)
				pInfo = data.GetType().GetProperty(IdField);
			base.GenerateRecord(data, pInfo, conn);
			if(data.PackagingMaterials != null)
			{
				foreach(var pm in data.PackagingMaterials)
				{
					pm.mast_id = data.mast_id;
					c.Execute(GetPackagingMaterialCreateSql(), pm);
				}
			}
		}

		public override void Cleanup()
		{
			conn.Execute("DELETE FROM mast_products_packaging_material");
			conn.Execute("DELETE FROM file");
			base.Cleanup();			
		}

	}
}
