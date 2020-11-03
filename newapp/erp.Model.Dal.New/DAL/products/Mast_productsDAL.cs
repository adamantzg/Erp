
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using erp.Model;
using Dapper;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace erp.Model.Dal.New
{
	public class MastProductsDal : IMastProductsDal
	{
		private MySqlConnection conn;

		public MastProductsDal(IDbConnection conn)
		{
			this.conn = (MySqlConnection) conn;
		}

		public List<Mast_products> GetAll()
		{
			return conn.Query<Mast_products>("SELECT * FROM mast_products").ToList();
		}
		
		public Mast_products GetById(int id, bool loadCharacteristics = false)
		{
			var result = conn.QueryFirst<Mast_products>("SELECT * FROM mast_products WHERE mast_id = @id", new {id});
			if (loadCharacteristics)
			{
				if (result != null)
				{
					var sql =
						@"SELECT mastproduct_characteristics.`value`,mastproduct_characteristics.characteristics_id,
							catdop_characteristics.`name` AS characteristics_name
								FROM mastproduct_characteristics INNER JOIN catdop_characteristics ON catdop_characteristics.characteristic_id = mastproduct_characteristics.characteristics_id
					  WHERE mastproduct_characteristics.mast_id = @id";
					result.Characteristics = conn.Query<Mastproduct_characteristics>(sql, new {id}).ToList();
				} 
			}
			return result;
		}
	
		
		public void Create(Mast_products o)
		{
			string insertsql = @"INSERT INTO mast_products (factory_id,factory_ref,factory_name,asaq_ref,asaq_name,price_dollar,price_euro,price_pound,price_dollar_ex,price_euro_ex,
                        price_pound_ex,stock_qty,prod_length,prod_width,prod_height,prod_depth,prod_nw,pack_length,pack_width,pack_depth,pack_height,pack_GW,packunits,
                        pack_loading_ratio,carton_width,carton_length,carton_height,carton_GW,units_per_carton,pallet_width,pallet_length,pallet_height,pallet_height_upper,
                        pallet_height_lower,units_per_pallet_single,units_per_pallet_lower,units_per_pallet_upper,pallets_per_20,pallets_per_40,units_per_20pallet,units_per_20nopallet,
                        units_per_40pallet_gp,units_per_40nopallet_gp,units_per_40pallet_hc,units_per_40nopallet_hc,prod_image1,prod_image2,prod_image3,prod_image4,prod_image5,
                        prod_instructions2,prod_instructions,prod_image1_w,prod_image1_h,min_ord_qty,category1,category1_sub,category2,catagory3,product_status,prod_finish,
                        glass_thickness,prod_material,comments,prod_createdate,last_update,lme,product_group,product_group_override,product_group_CMA,pack2_l,pack2_w,pack2_h,pack2_gw,
                        pack3_l,pack3_w,pack3_h,pack3_gw,pack4_l,pack4_w,pack4_h,pack4_gw,prod_type,tariff_code,lme_adjust,flow01,flow02,flow05,flow10,flow20,flow30,aerator01,aerator02,
                        aerator05,aerator10,aerator20,aerator30,pending_price_dollar,pending_price_pound,pending_price_euro,pending_price_date,threemonths,special_comments,om_seq_number,
                        range1,lead_time,factory_moq,maxweight_unit, maxweight_carton, maxweight_pallet,factory_stock) 
                        VALUES(@factory_id,@factory_ref,@factory_name,@asaq_ref,@asaq_name,@price_dollar,@price_euro,@price_pound,@price_dollar_ex,@price_euro_ex,@price_pound_ex,
                        @stock_qty,@prod_length,@prod_width,@prod_height,@prod_depth,@prod_nw,@pack_length,@pack_width,@pack_depth,@pack_height,@pack_GW,@packunits,@pack_loading_ratio,
                        @carton_width,@carton_length,@carton_height,@carton_GW,@units_per_carton,@pallet_width,@pallet_length,@pallet_height,@pallet_height_upper,@pallet_height_lower,
                        @units_per_pallet_single,@units_per_pallet_lower,@units_per_pallet_upper,@pallets_per_20,@pallets_per_40,@units_per_20pallet,@units_per_20nopallet,
                        @units_per_40pallet_gp,@units_per_40nopallet_gp,@units_per_40pallet_hc,@units_per_40nopallet_hc,@prod_image1,@prod_image2,@prod_image3,@prod_image4,
                        @prod_image5,@prod_instructions2,@prod_instructions,@prod_image1_w,@prod_image1_h,@min_ord_qty,@category1,@category1_sub,@category2,@catagory3,
                        @product_status,@prod_finish,@glass_thickness,@prod_material,@comments,@prod_createdate,@last_update,@lme,@product_group,@product_group_override,
                        @product_group_CMA,@pack2_l,@pack2_w,@pack2_h,@pack2_gw,@pack3_l,@pack3_w,@pack3_h,@pack3_gw,@pack4_l,@pack4_w,@pack4_h,@pack4_gw,@prod_type,@tariff_code,
                        @lme_adjust,@flow01,@flow02,@flow05,@flow10,@flow20,@flow30,@aerator01,@aerator02,@aerator05,@aerator10,@aerator20,@aerator30,@pending_price_dollar,
                        @pending_price_pound,@pending_price_euro,@pending_price_date,@threemonths,@special_comments,@om_seq_number,@range1,@lead_time,@factory_moq,@maxweight_unit,
                        @maxweight_carton, @maxweight_pallet,@factory_stock)";

			conn.Execute(insertsql, o);
				
			var sql = "SELECT LAST_INSERT_ID()";
			o.mast_id = conn.ExecuteScalar<int>(sql);
			
		}
		
		
		public void Update(Mast_products o)
		{
			string updatesql = @"UPDATE mast_products SET factory_id = @factory_id,factory_ref = @factory_ref,factory_name = @factory_name,asaq_ref = @asaq_ref,asaq_name = @asaq_name,
                    price_dollar = @price_dollar,price_euro = @price_euro,price_pound = @price_pound,price_dollar_ex = @price_dollar_ex,price_euro_ex = @price_euro_ex,
                    price_pound_ex = @price_pound_ex,stock_qty = @stock_qty,prod_length = @prod_length,prod_width = @prod_width,prod_height = @prod_height,prod_depth = @prod_depth,
                    prod_nw = @prod_nw,pack_length = @pack_length,pack_width = @pack_width,pack_depth = @pack_depth,pack_height = @pack_height,pack_GW = @pack_GW,packunits = @packunits,
                    pack_loading_ratio = @pack_loading_ratio,carton_width = @carton_width,carton_length = @carton_length,carton_height = @carton_height,carton_GW = @carton_GW,
                    units_per_carton = @units_per_carton,pallet_width = @pallet_width,pallet_length = @pallet_length,pallet_height = @pallet_height,pallet_height_upper = @pallet_height_upper,
                    pallet_height_lower = @pallet_height_lower,units_per_pallet_single = @units_per_pallet_single,units_per_pallet_lower = @units_per_pallet_lower,
                    units_per_pallet_upper = @units_per_pallet_upper,pallets_per_20 = @pallets_per_20,pallets_per_40 = @pallets_per_40,units_per_20pallet = @units_per_20pallet,
                    units_per_20nopallet = @units_per_20nopallet,units_per_40pallet_gp = @units_per_40pallet_gp,units_per_40nopallet_gp = @units_per_40nopallet_gp,
                    units_per_40pallet_hc = @units_per_40pallet_hc,units_per_40nopallet_hc = @units_per_40nopallet_hc,prod_image1 = @prod_image1,prod_image2 = @prod_image2,
                    prod_image3 = @prod_image3,prod_image4 = @prod_image4,prod_image5 = @prod_image5,prod_instructions2 = @prod_instructions2,prod_instructions = @prod_instructions,
                    prod_image1_w = @prod_image1_w,prod_image1_h = @prod_image1_h,min_ord_qty = @min_ord_qty,category1 = @category1,category1_sub = @category1_sub,category2 = @category2,
                    catagory3 = @catagory3,product_status = @product_status,prod_finish = @prod_finish,glass_thickness = @glass_thickness,prod_material = @prod_material,comments = @comments,
                    prod_createdate = @prod_createdate,last_update = @last_update,lme = @lme,product_group = @product_group,product_group_override = @product_group_override,
                    product_group_CMA = @product_group_CMA,pack2_l = @pack2_l,pack2_w = @pack2_w,pack2_h = @pack2_h,pack2_gw = @pack2_gw,pack3_l = @pack3_l,pack3_w = @pack3_w,
                    pack3_h = @pack3_h,pack3_gw = @pack3_gw,pack4_l = @pack4_l,pack4_w = @pack4_w,pack4_h = @pack4_h,pack4_gw = @pack4_gw,prod_type = @prod_type,tariff_code = @tariff_code,
                    lme_adjust = @lme_adjust,flow01 = @flow01,flow02 = @flow02,flow05 = @flow05,flow10 = @flow10,flow20 = @flow20,flow30 = @flow30,aerator01 = @aerator01,
                    aerator02 = @aerator02,aerator05 = @aerator05,aerator10 = @aerator10,aerator20 = @aerator20,aerator30 = @aerator30,pending_price_dollar = @pending_price_dollar,
                    pending_price_pound = @pending_price_pound,pending_price_euro = @pending_price_euro,pending_price_date = @pending_price_date,threemonths = @threemonths,
						special_comments = @special_comments,om_seq_number = @om_seq_number,range1 = @range1,lead_time = @lead_time,factory_moq = @factory_moq, maxweight_unit = @maxweight_unit, 
						maxweight_carton = @maxweight_carton, maxweight_pallet = @maxweight_pallet,factory_stock = @factory_stock WHERE mast_id = @mast_id";

			conn.Execute(updatesql, o);
		}
		
		public void Delete(int mast_id)
		{
			conn.Execute("DELETE FROM mast_products WHERE mast_id = @id", new {id = mast_id});
		}

		public Mast_products GetByRefAndCode(string factoryRef, string factoryCode)
		{
			return conn.QueryFirst<Mast_products>(@"SELECT * FROM mast_products INNER JOIN users ON mast_products.factory_id = users.user_id 
				WHERE factory_ref = @ref AND users.factory_code = @code", new {@ref=factoryRef, code = factoryCode});
		}

        public List<ProductDate> ProductFirstShipmentDates(IList<int> ids)
        {
            return conn.Query<ProductDate>(@"SELECT cust_products.cprod_mast as id, MIN(porder_header.po_req_etd) AS po_req_etd
                                            FROM porder_lines INNER JOIN porder_header ON porder_lines.porderid = porder_header.porderid 
                                            INNER JOIN cust_products ON porder_lines.cprod_id = cust_products.cprod_id
                                            INNER JOIN order_lines ON porder_lines.soline = order_lines.linenum 
                                            INNER JOIN order_header ON order_lines.orderid = order_header.orderid
                                            INNER JOIN users on order_header.userid1 = users.user_id
                                            WHERE 
                                            order_header.`status` NOT IN ('X','Y') AND porder_lines.orderqty > 0 
											AND cust_products.cprod_mast IN @ids AND order_header.stock_order <> 1
                                            AND (coalesce(`users`.`test_account`,0) = 0)
                                            GROUP BY cust_products.cprod_mast", new {ids}).ToList();
        }

		public void DeletePackaging(int id)
		{
			conn.Execute("DELETE FROM Mast_products_packaging_material WHERE id = @id", new { id});
		}

		public List<Mast_products> GetProductsWithSalesHistory(int? factoryId, int? categoryId, DateTime fromDate)
		{
			return conn.Query<Mast_products>(
				@"SELECT mast_products.* FROM mast_products WHERE 
					(@factoryId IS NULL OR mast_products.factory_id = @factoryId)  AND
					(@categoryId IS NULL OR mast_products.category1 = @categoryId) AND
					mast_id IN (
					SELECT cp.cprod_mast FROM cust_products cp INNER JOIN order_lines ol ON cp.cprod_id = ol.cprod_id 
					INNER JOIN order_header oh ON ol.orderid = oh.orderid 
					WHERE cp.cprod_status <> 'D' AND oh.status NOT IN ('X','Y') AND ol.orderqty > 0 
					AND	oh.orderdate >= @fromDate)",
				new {fromDate, factoryId, categoryId}
				).ToList();
		}

		public void RemoveFile(int mastProductid, int fileId)
		{
			conn.Execute("DELETE FROM mast_product_file WHERE mast_id = @mastProductId AND file_id = @fileId", new { mastProductid, fileId});
		}

		public void UpdateOtherFiles(Mast_products mp, int? fileType = null)
		{
			if(mp.OtherFiles != null)
			{
				if(conn.State != ConnectionState.Open) 
					conn.Open();
				var tr = conn.BeginTransaction();

				try
				{
					conn.Execute(@"DELETE FROM mast_product_file 
						WHERE (@fileType IS NULL OR file_id IN (SELECT id FROM file WHERE type_id = @fileType)) AND mast_id = @mast_id",
						new { mp.mast_id, fileType });
					foreach (var f in mp.OtherFiles)
					{
						conn.Execute("INSERT INTO mast_product_file (mast_id, file_id) VALUES (@mast_id, @file_id)", new { mp.mast_id, file_id = f.id });
					}
					tr.Commit();
				}
				catch
				{
					tr.Rollback();
					throw;
				}
			}
		}
	}
}
			
			