
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace asaq2.Model
{
    public class Mast_productsDAL
	{
	
		public static List<Mast_products> GetAll()
		{
			List<Mast_products> result = new List<Mast_products>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM mast_products", conn);
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}
		
		
		public static Mast_products GetById(int id, bool loadCharacteristics = false)
		{
			Mast_products result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM mast_products WHERE mast_id = @id", conn);
				cmd.Parameters.AddWithValue("@id", id);
                MySqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    result = GetFromDataReader(dr);
                }
                dr.Close();
                if (loadCharacteristics)
                {
                    if (result != null)
                    {
                        cmd.CommandText =
                            @"SELECT mastproduct_characteristics.`value`,mastproduct_characteristics.characteristics_id,catdop_characteristics.`name`
                                    FROM mastproduct_characteristics INNER JOIN catdop_characteristics ON catdop_characteristics.characteristic_id = mastproduct_characteristics.characteristics_id
                          WHERE mastproduct_characteristics.mast_id = @id";
                        dr = cmd.ExecuteReader();
                        result.Characteristics = new List<Mastproduct_characteristics>();
                        while (dr.Read())
                        {
                            result.Characteristics.Add(new Mastproduct_characteristics{characteristics_id = (int) dr["characteristics_id"],characteristics_name = string.Empty + dr["name"],value = string.Empty + dr["value"]});
                        }

                    } 
                }
            }
			return result;
		}
	
		public static Mast_products GetFromDataReader(MySqlDataReader dr)
		{
			Mast_products o = new Mast_products();
		
			o.mast_id =  (int) dr["mast_id"];
			o.factory_id = Utilities.FromDbValue<int>(dr["factory_id"]);
			o.factory_ref = string.Empty + dr["factory_ref"];
			o.factory_name = string.Empty + dr["factory_name"];
			o.asaq_ref = string.Empty + dr["asaq_ref"];
			o.asaq_name = string.Empty + dr["asaq_name"];
			o.price_dollar = Utilities.FromDbValue<double>(dr["price_dollar"]);
			o.price_euro = Utilities.FromDbValue<double>(dr["price_euro"]);
			o.price_pound = Utilities.FromDbValue<double>(dr["price_pound"]);
			o.price_dollar_ex = Utilities.FromDbValue<double>(dr["price_dollar_ex"]);
			o.price_euro_ex = Utilities.FromDbValue<double>(dr["price_euro_ex"]);
			o.price_pound_ex = Utilities.FromDbValue<double>(dr["price_pound_ex"]);
			o.stock_qty = Utilities.FromDbValue<int>(dr["stock_qty"]);
			o.prod_length = Utilities.FromDbValue<double>(dr["prod_length"]);
			o.prod_width = Utilities.FromDbValue<double>(dr["prod_width"]);
			o.prod_height = Utilities.FromDbValue<double>(dr["prod_height"]);
			o.prod_depth = Utilities.FromDbValue<double>(dr["prod_depth"]);
			o.prod_nw = Utilities.FromDbValue<double>(dr["prod_nw"]);
			o.pack_length = Utilities.FromDbValue<double>(dr["pack_length"]);
			o.pack_width = Utilities.FromDbValue<double>(dr["pack_width"]);
			o.pack_depth = Utilities.FromDbValue<double>(dr["pack_depth"]);
			o.pack_height = Utilities.FromDbValue<double>(dr["pack_height"]);
			o.pack_GW = Utilities.FromDbValue<double>(dr["pack_GW"]);
			o.packunits = Utilities.FromDbValue<int>(dr["packunits"]);
			o.pack_loading_ratio = Utilities.FromDbValue<double>(dr["pack_loading_ratio"]);
			o.carton_width = Utilities.FromDbValue<double>(dr["carton_width"]);
			o.carton_length = Utilities.FromDbValue<double>(dr["carton_length"]);
			o.carton_height = Utilities.FromDbValue<double>(dr["carton_height"]);
			o.carton_GW = Utilities.FromDbValue<double>(dr["carton_GW"]);
			o.units_per_carton = Utilities.FromDbValue<int>(dr["units_per_carton"]);
			o.pallet_width = Utilities.FromDbValue<double>(dr["pallet_width"]);
			o.pallet_length = Utilities.FromDbValue<double>(dr["pallet_length"]);
			o.pallet_height = Utilities.FromDbValue<double>(dr["pallet_height"]);
			o.pallet_height_upper = Utilities.FromDbValue<double>(dr["pallet_height_upper"]);
			o.pallet_height_lower = Utilities.FromDbValue<double>(dr["pallet_height_lower"]);
			o.units_per_pallet_single = Utilities.FromDbValue<int>(dr["units_per_pallet_single"]);
			o.units_per_pallet_lower = Utilities.FromDbValue<int>(dr["units_per_pallet_lower"]);
			o.units_per_pallet_upper = Utilities.FromDbValue<int>(dr["units_per_pallet_upper"]);
			o.pallets_per_20 = Utilities.FromDbValue<int>(dr["pallets_per_20"]);
			o.pallets_per_40 = Utilities.FromDbValue<int>(dr["pallets_per_40"]);
			o.units_per_20pallet = Utilities.FromDbValue<int>(dr["units_per_20pallet"]);
			o.units_per_20nopallet = Utilities.FromDbValue<int>(dr["units_per_20nopallet"]);
			o.units_per_40pallet_gp = Utilities.FromDbValue<int>(dr["units_per_40pallet_gp"]);
			o.units_per_40nopallet_gp = Utilities.FromDbValue<int>(dr["units_per_40nopallet_gp"]);
			o.units_per_40pallet_hc = Utilities.FromDbValue<int>(dr["units_per_40pallet_hc"]);
			o.units_per_40nopallet_hc = Utilities.FromDbValue<int>(dr["units_per_40nopallet_hc"]);
			o.prod_image1 = string.Empty + dr["prod_image1"];
			o.prod_image2 = string.Empty + dr["prod_image2"];
			o.prod_image3 = string.Empty + dr["prod_image3"];
			o.prod_image4 = string.Empty + dr["prod_image4"];
			o.prod_image5 = string.Empty + dr["prod_image5"];
			o.prod_instructions2 = string.Empty + dr["prod_instructions2"];
			o.prod_instructions = string.Empty + dr["prod_instructions"];
			o.prod_image1_w = Utilities.FromDbValue<int>(dr["prod_image1_w"]);
			o.prod_image1_h = Utilities.FromDbValue<int>(dr["prod_image1_h"]);
			o.min_ord_qty = Utilities.FromDbValue<int>(dr["min_ord_qty"]);
			o.category1 = Utilities.FromDbValue<int>(dr["category1"]);
			o.category1_sub = Utilities.FromDbValue<int>(dr["category1_sub"]);
			o.category2 = Utilities.FromDbValue<int>(dr["category2"]);
			o.catagory3 = Utilities.FromDbValue<int>(dr["catagory3"]);
			o.product_status = Utilities.FromDbValue<int>(dr["product_status"]);
			o.prod_finish = string.Empty + dr["prod_finish"];
			o.glass_thickness = string.Empty + dr["glass_thickness"];
			o.prod_material = string.Empty + dr["prod_material"];
			o.comments = string.Empty + dr["comments"];
			o.prod_createdate = Utilities.FromDbValue<DateTime>(dr["prod_createdate"]);
			o.last_update = Utilities.FromDbValue<DateTime>(dr["last_update"]);
			o.lme = Utilities.FromDbValue<int>(dr["lme"]);
			o.product_group = string.Empty + dr["product_group"];
			o.product_group_override = string.Empty + dr["product_group_override"];
			o.product_group_CMA = Utilities.FromDbValue<double>(dr["product_group_CMA"]);
			o.pack2_l = Utilities.FromDbValue<double>(dr["pack2_l"]);
			o.pack2_w = Utilities.FromDbValue<double>(dr["pack2_w"]);
			o.pack2_h = Utilities.FromDbValue<double>(dr["pack2_h"]);
			o.pack2_gw = Utilities.FromDbValue<double>(dr["pack2_gw"]);
			o.pack3_l = Utilities.FromDbValue<double>(dr["pack3_l"]);
			o.pack3_w = Utilities.FromDbValue<double>(dr["pack3_w"]);
			o.pack3_h = Utilities.FromDbValue<double>(dr["pack3_h"]);
			o.pack3_gw = Utilities.FromDbValue<double>(dr["pack3_gw"]);
			o.pack4_l = Utilities.FromDbValue<double>(dr["pack4_l"]);
			o.pack4_w = Utilities.FromDbValue<double>(dr["pack4_w"]);
			o.pack4_h = Utilities.FromDbValue<double>(dr["pack4_h"]);
			o.pack4_gw = Utilities.FromDbValue<double>(dr["pack4_gw"]);
			o.prod_type = Utilities.FromDbValue<int>(dr["prod_type"]);
			o.tariff_code = Utilities.FromDbValue<int>(dr["tariff_code"]);
			o.lme_adjust = Utilities.FromDbValue<double>(dr["lme_adjust"]);
			o.flow01 = Utilities.FromDbValue<double>(dr["flow01"]);
			o.flow02 = Utilities.FromDbValue<double>(dr["flow02"]);
			o.flow05 = Utilities.FromDbValue<double>(dr["flow05"]);
			o.flow10 = Utilities.FromDbValue<double>(dr["flow10"]);
			o.flow20 = Utilities.FromDbValue<double>(dr["flow20"]);
			o.flow30 = Utilities.FromDbValue<double>(dr["flow30"]);
			o.aerator01 = Utilities.FromDbValue<double>(dr["aerator01"]);
			o.aerator02 = Utilities.FromDbValue<double>(dr["aerator02"]);
			o.aerator05 = Utilities.FromDbValue<double>(dr["aerator05"]);
			o.aerator10 = Utilities.FromDbValue<double>(dr["aerator10"]);
			o.aerator20 = Utilities.FromDbValue<double>(dr["aerator20"]);
			o.aerator30 = Utilities.FromDbValue<double>(dr["aerator30"]);
			o.pending_price_dollar = Utilities.FromDbValue<double>(dr["pending_price_dollar"]);
			o.pending_price_pound = Utilities.FromDbValue<double>(dr["pending_price_pound"]);
			o.pending_price_euro = Utilities.FromDbValue<double>(dr["pending_price_euro"]);
			o.pending_price_date = Utilities.FromDbValue<DateTime>(dr["pending_price_date"]);
			o.threemonths = Utilities.FromDbValue<double>(dr["threemonths"]);
			o.special_comments = string.Empty + dr["special_comments"];
			o.om_seq_number = Utilities.FromDbValue<int>(dr["om_seq_number"]);
			o.range1 = Utilities.FromDbValue<int>(dr["range1"]);
			o.lead_time = Utilities.FromDbValue<int>(dr["lead_time"]);
			o.factory_moq = Utilities.FromDbValue<int>(dr["factory_moq"]);
            
		    o.maxweight_unit = Utilities.FromDbValue<double>(dr["maxweight_unit"]);
            o.maxweight_carton = Utilities.FromDbValue<double>(dr["maxweight_carton"]);
            o.maxweight_pallet = Utilities.FromDbValue<double>(dr["maxweight_pallet"]);
            if (Utilities.ColumnExists(dr, "category_dop"))
		        o.categorydop_id = Utilities.FromDbValue<int>(dr["category_dop"]);
			return o;

		}
		
		public static void Create(Mast_products o)
        {
            string insertsql = @"INSERT INTO mast_products (factory_id,factory_ref,factory_name,asaq_ref,asaq_name,price_dollar,price_euro,price_pound,price_dollar_ex,price_euro_ex,price_pound_ex,stock_qty,prod_length,prod_width,prod_height,prod_depth,prod_nw,pack_length,pack_width,pack_depth,pack_height,pack_GW,packunits,pack_loading_ratio,carton_width,carton_length,carton_height,carton_GW,units_per_carton,pallet_width,pallet_length,pallet_height,pallet_height_upper,pallet_height_lower,units_per_pallet_single,units_per_pallet_lower,units_per_pallet_upper,pallets_per_20,pallets_per_40,units_per_20pallet,units_per_20nopallet,units_per_40pallet_gp,units_per_40nopallet_gp,units_per_40pallet_hc,units_per_40nopallet_hc,prod_image1,prod_image2,prod_image3,prod_image4,prod_image5,prod_instructions2,prod_instructions,prod_image1_w,prod_image1_h,min_ord_qty,category1,category1_sub,category2,catagory3,product_status,prod_finish,glass_thickness,prod_material,comments,prod_createdate,last_update,lme,product_group,product_group_override,product_group_CMA,pack2_l,pack2_w,pack2_h,pack2_gw,pack3_l,pack3_w,pack3_h,pack3_gw,pack4_l,pack4_w,pack4_h,pack4_gw,prod_type,tariff_code,lme_adjust,flow01,flow02,flow05,flow10,flow20,flow30,aerator01,aerator02,aerator05,aerator10,aerator20,aerator30,pending_price_dollar,pending_price_pound,pending_price_euro,pending_price_date,threemonths,special_comments,om_seq_number,range1,lead_time,factory_moq,maxweight_unit, maxweight_carton, maxweight_pallet) VALUES(@factory_id,@factory_ref,@factory_name,@asaq_ref,@asaq_name,@price_dollar,@price_euro,@price_pound,@price_dollar_ex,@price_euro_ex,@price_pound_ex,@stock_qty,@prod_length,@prod_width,@prod_height,@prod_depth,@prod_nw,@pack_length,@pack_width,@pack_depth,@pack_height,@pack_GW,@packunits,@pack_loading_ratio,@carton_width,@carton_length,@carton_height,@carton_GW,@units_per_carton,@pallet_width,@pallet_length,@pallet_height,@pallet_height_upper,@pallet_height_lower,@units_per_pallet_single,@units_per_pallet_lower,@units_per_pallet_upper,@pallets_per_20,@pallets_per_40,@units_per_20pallet,@units_per_20nopallet,@units_per_40pallet_gp,@units_per_40nopallet_gp,@units_per_40pallet_hc,@units_per_40nopallet_hc,@prod_image1,@prod_image2,@prod_image3,@prod_image4,@prod_image5,@prod_instructions2,@prod_instructions,@prod_image1_w,@prod_image1_h,@min_ord_qty,@category1,@category1_sub,@category2,@catagory3,@product_status,@prod_finish,@glass_thickness,@prod_material,@comments,@prod_createdate,@last_update,@lme,@product_group,@product_group_override,@product_group_CMA,@pack2_l,@pack2_w,@pack2_h,@pack2_gw,@pack3_l,@pack3_w,@pack3_h,@pack3_gw,@pack4_l,@pack4_w,@pack4_h,@pack4_gw,@prod_type,@tariff_code,@lme_adjust,@flow01,@flow02,@flow05,@flow10,@flow20,@flow30,@aerator01,@aerator02,@aerator05,@aerator10,@aerator20,@aerator30,@pending_price_dollar,@pending_price_pound,@pending_price_euro,@pending_price_date,@threemonths,@special_comments,@om_seq_number,@range1,@lead_time,@factory_moq,@maxweight_unit,@maxweight_carton, @maxweight_pallet)";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				
				MySqlCommand cmd = new MySqlCommand(insertsql, conn);
                BuildSqlParameters(cmd,o);
                cmd.ExecuteNonQuery();
				cmd.CommandText = "SELECT mast_id FROM mast_products WHERE mast_id = LAST_INSERT_ID()";
                o.mast_id = (int) cmd.ExecuteScalar();
				
            }
		}
		
		private static void BuildSqlParameters(MySqlCommand cmd, Mast_products o, bool forInsert = true)
        {
			
			if(!forInsert)
				cmd.Parameters.AddWithValue("@mast_id", o.mast_id);
			cmd.Parameters.AddWithValue("@factory_id", o.factory_id);
			cmd.Parameters.AddWithValue("@factory_ref", o.factory_ref);
			cmd.Parameters.AddWithValue("@factory_name", o.factory_name);
			cmd.Parameters.AddWithValue("@asaq_ref", o.asaq_ref);
			cmd.Parameters.AddWithValue("@asaq_name", o.asaq_name);
			cmd.Parameters.AddWithValue("@price_dollar", o.price_dollar);
			cmd.Parameters.AddWithValue("@price_euro", o.price_euro);
			cmd.Parameters.AddWithValue("@price_pound", o.price_pound);
			cmd.Parameters.AddWithValue("@price_dollar_ex", o.price_dollar_ex);
			cmd.Parameters.AddWithValue("@price_euro_ex", o.price_euro_ex);
			cmd.Parameters.AddWithValue("@price_pound_ex", o.price_pound_ex);
			cmd.Parameters.AddWithValue("@stock_qty", o.stock_qty);
			cmd.Parameters.AddWithValue("@prod_length", o.prod_length);
			cmd.Parameters.AddWithValue("@prod_width", o.prod_width);
			cmd.Parameters.AddWithValue("@prod_height", o.prod_height);
			cmd.Parameters.AddWithValue("@prod_depth", o.prod_depth);
			cmd.Parameters.AddWithValue("@prod_nw", o.prod_nw);
			cmd.Parameters.AddWithValue("@pack_length", o.pack_length);
			cmd.Parameters.AddWithValue("@pack_width", o.pack_width);
			cmd.Parameters.AddWithValue("@pack_depth", o.pack_depth);
			cmd.Parameters.AddWithValue("@pack_height", o.pack_height);
			cmd.Parameters.AddWithValue("@pack_GW", o.pack_GW);
			cmd.Parameters.AddWithValue("@packunits", o.packunits);
			cmd.Parameters.AddWithValue("@pack_loading_ratio", o.pack_loading_ratio);
			cmd.Parameters.AddWithValue("@carton_width", o.carton_width);
			cmd.Parameters.AddWithValue("@carton_length", o.carton_length);
			cmd.Parameters.AddWithValue("@carton_height", o.carton_height);
			cmd.Parameters.AddWithValue("@carton_GW", o.carton_GW);
			cmd.Parameters.AddWithValue("@units_per_carton", o.units_per_carton);
			cmd.Parameters.AddWithValue("@pallet_width", o.pallet_width);
			cmd.Parameters.AddWithValue("@pallet_length", o.pallet_length);
			cmd.Parameters.AddWithValue("@pallet_height", o.pallet_height);
			cmd.Parameters.AddWithValue("@pallet_height_upper", o.pallet_height_upper);
			cmd.Parameters.AddWithValue("@pallet_height_lower", o.pallet_height_lower);
			cmd.Parameters.AddWithValue("@units_per_pallet_single", o.units_per_pallet_single);
			cmd.Parameters.AddWithValue("@units_per_pallet_lower", o.units_per_pallet_lower);
			cmd.Parameters.AddWithValue("@units_per_pallet_upper", o.units_per_pallet_upper);
			cmd.Parameters.AddWithValue("@pallets_per_20", o.pallets_per_20);
			cmd.Parameters.AddWithValue("@pallets_per_40", o.pallets_per_40);
			cmd.Parameters.AddWithValue("@units_per_20pallet", o.units_per_20pallet);
			cmd.Parameters.AddWithValue("@units_per_20nopallet", o.units_per_20nopallet);
			cmd.Parameters.AddWithValue("@units_per_40pallet_gp", o.units_per_40pallet_gp);
			cmd.Parameters.AddWithValue("@units_per_40nopallet_gp", o.units_per_40nopallet_gp);
			cmd.Parameters.AddWithValue("@units_per_40pallet_hc", o.units_per_40pallet_hc);
			cmd.Parameters.AddWithValue("@units_per_40nopallet_hc", o.units_per_40nopallet_hc);
			cmd.Parameters.AddWithValue("@prod_image1", o.prod_image1);
			cmd.Parameters.AddWithValue("@prod_image2", o.prod_image2);
			cmd.Parameters.AddWithValue("@prod_image3", o.prod_image3);
			cmd.Parameters.AddWithValue("@prod_image4", o.prod_image4);
			cmd.Parameters.AddWithValue("@prod_image5", o.prod_image5);
			cmd.Parameters.AddWithValue("@prod_instructions2", o.prod_instructions2);
			cmd.Parameters.AddWithValue("@prod_instructions", o.prod_instructions);
			cmd.Parameters.AddWithValue("@prod_image1_w", o.prod_image1_w);
			cmd.Parameters.AddWithValue("@prod_image1_h", o.prod_image1_h);
			cmd.Parameters.AddWithValue("@min_ord_qty", o.min_ord_qty);
			cmd.Parameters.AddWithValue("@category1", o.category1);
			cmd.Parameters.AddWithValue("@category1_sub", o.category1_sub);
			cmd.Parameters.AddWithValue("@category2", o.category2);
			cmd.Parameters.AddWithValue("@catagory3", o.catagory3);
			cmd.Parameters.AddWithValue("@product_status", o.product_status);
			cmd.Parameters.AddWithValue("@prod_finish", o.prod_finish);
			cmd.Parameters.AddWithValue("@glass_thickness", o.glass_thickness);
			cmd.Parameters.AddWithValue("@prod_material", o.prod_material);
			cmd.Parameters.AddWithValue("@comments", o.comments);
			cmd.Parameters.AddWithValue("@prod_createdate", o.prod_createdate);
			cmd.Parameters.AddWithValue("@last_update", o.last_update);
			cmd.Parameters.AddWithValue("@lme", o.lme);
			cmd.Parameters.AddWithValue("@product_group", o.product_group);
			cmd.Parameters.AddWithValue("@product_group_override", o.product_group_override);
			cmd.Parameters.AddWithValue("@product_group_CMA", o.product_group_CMA);
			cmd.Parameters.AddWithValue("@pack2_l", o.pack2_l);
			cmd.Parameters.AddWithValue("@pack2_w", o.pack2_w);
			cmd.Parameters.AddWithValue("@pack2_h", o.pack2_h);
			cmd.Parameters.AddWithValue("@pack2_gw", o.pack2_gw);
			cmd.Parameters.AddWithValue("@pack3_l", o.pack3_l);
			cmd.Parameters.AddWithValue("@pack3_w", o.pack3_w);
			cmd.Parameters.AddWithValue("@pack3_h", o.pack3_h);
			cmd.Parameters.AddWithValue("@pack3_gw", o.pack3_gw);
			cmd.Parameters.AddWithValue("@pack4_l", o.pack4_l);
			cmd.Parameters.AddWithValue("@pack4_w", o.pack4_w);
			cmd.Parameters.AddWithValue("@pack4_h", o.pack4_h);
			cmd.Parameters.AddWithValue("@pack4_gw", o.pack4_gw);
			cmd.Parameters.AddWithValue("@prod_type", o.prod_type);
			cmd.Parameters.AddWithValue("@tariff_code", o.tariff_code);
			cmd.Parameters.AddWithValue("@lme_adjust", o.lme_adjust);
			cmd.Parameters.AddWithValue("@flow01", o.flow01);
			cmd.Parameters.AddWithValue("@flow02", o.flow02);
			cmd.Parameters.AddWithValue("@flow05", o.flow05);
			cmd.Parameters.AddWithValue("@flow10", o.flow10);
			cmd.Parameters.AddWithValue("@flow20", o.flow20);
			cmd.Parameters.AddWithValue("@flow30", o.flow30);
			cmd.Parameters.AddWithValue("@aerator01", o.aerator01);
			cmd.Parameters.AddWithValue("@aerator02", o.aerator02);
			cmd.Parameters.AddWithValue("@aerator05", o.aerator05);
			cmd.Parameters.AddWithValue("@aerator10", o.aerator10);
			cmd.Parameters.AddWithValue("@aerator20", o.aerator20);
			cmd.Parameters.AddWithValue("@aerator30", o.aerator30);
			cmd.Parameters.AddWithValue("@pending_price_dollar", o.pending_price_dollar);
			cmd.Parameters.AddWithValue("@pending_price_pound", o.pending_price_pound);
			cmd.Parameters.AddWithValue("@pending_price_euro", o.pending_price_euro);
			cmd.Parameters.AddWithValue("@pending_price_date", o.pending_price_date);
			cmd.Parameters.AddWithValue("@threemonths", o.threemonths);
			cmd.Parameters.AddWithValue("@special_comments", o.special_comments);
			cmd.Parameters.AddWithValue("@om_seq_number", o.om_seq_number);
			cmd.Parameters.AddWithValue("@range1", o.range1);
			cmd.Parameters.AddWithValue("@lead_time", o.lead_time);
			cmd.Parameters.AddWithValue("@factory_moq", o.factory_moq);
            
		    cmd.Parameters.AddWithValue("@maxweight_unit", o.maxweight_unit);
		    cmd.Parameters.AddWithValue("@maxweight_carton", o.maxweight_carton);
		    cmd.Parameters.AddWithValue("@maxweight_pallet", o.maxweight_pallet);
        }
		
		public static void Update(Mast_products o)
		{
            string updatesql = @"UPDATE mast_products SET factory_id = @factory_id,factory_ref = @factory_ref,factory_name = @factory_name,asaq_ref = @asaq_ref,asaq_name = @asaq_name,price_dollar = @price_dollar,price_euro = @price_euro,price_pound = @price_pound,price_dollar_ex = @price_dollar_ex,price_euro_ex = @price_euro_ex,price_pound_ex = @price_pound_ex,stock_qty = @stock_qty,prod_length = @prod_length,prod_width = @prod_width,prod_height = @prod_height,prod_depth = @prod_depth,prod_nw = @prod_nw,pack_length = @pack_length,pack_width = @pack_width,pack_depth = @pack_depth,pack_height = @pack_height,pack_GW = @pack_GW,packunits = @packunits,pack_loading_ratio = @pack_loading_ratio,carton_width = @carton_width,carton_length = @carton_length,carton_height = @carton_height,carton_GW = @carton_GW,units_per_carton = @units_per_carton,pallet_width = @pallet_width,pallet_length = @pallet_length,pallet_height = @pallet_height,pallet_height_upper = @pallet_height_upper,pallet_height_lower = @pallet_height_lower,units_per_pallet_single = @units_per_pallet_single,units_per_pallet_lower = @units_per_pallet_lower,units_per_pallet_upper = @units_per_pallet_upper,pallets_per_20 = @pallets_per_20,pallets_per_40 = @pallets_per_40,units_per_20pallet = @units_per_20pallet,units_per_20nopallet = @units_per_20nopallet,units_per_40pallet_gp = @units_per_40pallet_gp,units_per_40nopallet_gp = @units_per_40nopallet_gp,units_per_40pallet_hc = @units_per_40pallet_hc,units_per_40nopallet_hc = @units_per_40nopallet_hc,prod_image1 = @prod_image1,prod_image2 = @prod_image2,prod_image3 = @prod_image3,prod_image4 = @prod_image4,prod_image5 = @prod_image5,prod_instructions2 = @prod_instructions2,prod_instructions = @prod_instructions,prod_image1_w = @prod_image1_w,prod_image1_h = @prod_image1_h,min_ord_qty = @min_ord_qty,category1 = @category1,category1_sub = @category1_sub,category2 = @category2,catagory3 = @catagory3,product_status = @product_status,prod_finish = @prod_finish,glass_thickness = @glass_thickness,prod_material = @prod_material,comments = @comments,prod_createdate = @prod_createdate,last_update = @last_update,lme = @lme,product_group = @product_group,product_group_override = @product_group_override,product_group_CMA = @product_group_CMA,pack2_l = @pack2_l,pack2_w = @pack2_w,pack2_h = @pack2_h,pack2_gw = @pack2_gw,pack3_l = @pack3_l,pack3_w = @pack3_w,pack3_h = @pack3_h,pack3_gw = @pack3_gw,pack4_l = @pack4_l,pack4_w = @pack4_w,pack4_h = @pack4_h,pack4_gw = @pack4_gw,prod_type = @prod_type,tariff_code = @tariff_code,lme_adjust = @lme_adjust,flow01 = @flow01,flow02 = @flow02,flow05 = @flow05,flow10 = @flow10,flow20 = @flow20,flow30 = @flow30,aerator01 = @aerator01,aerator02 = @aerator02,aerator05 = @aerator05,aerator10 = @aerator10,aerator20 = @aerator20,aerator30 = @aerator30,pending_price_dollar = @pending_price_dollar,pending_price_pound = @pending_price_pound,pending_price_euro = @pending_price_euro,pending_price_date = @pending_price_date,threemonths = @threemonths,
                        special_comments = @special_comments,om_seq_number = @om_seq_number,range1 = @range1,lead_time = @lead_time,factory_moq = @factory_moq, maxweight_unit = @maxweight_unit, 
                        maxweight_carton = @maxweight_carton, maxweight_pallet = @maxweight_pallet WHERE mast_id = @mast_id";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				MySqlCommand cmd = new MySqlCommand(updatesql, conn);
                BuildSqlParameters(cmd,o, false);
                cmd.ExecuteNonQuery();
            }
		}
		
		public static void Delete(int mast_id)
		{
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				MySqlCommand cmd = new MySqlCommand("DELETE FROM mast_products WHERE mast_id = @id" , conn);
                cmd.Parameters.AddWithValue("@id", mast_id);
                cmd.ExecuteNonQuery();
            }
		}

        public static Mast_products GetByRefAndCode(string factoryRef, string factoryCode)
        {
            Mast_products result = null;
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM mast_products INNER JOIN users ON mast_products.factory_id = users.user_id WHERE factory_ref = @ref AND users.factory_code = @code", conn);
                cmd.Parameters.AddWithValue("@ref", factoryRef);
                cmd.Parameters.AddWithValue("@code", factoryCode);
                MySqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    result = GetFromDataReader(dr);
                }
                dr.Close();
            }
            return result;
        }
	}
}
			
			