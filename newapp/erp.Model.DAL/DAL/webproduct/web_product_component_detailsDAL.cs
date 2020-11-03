using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using MySql.Data.MySqlClient;

namespace erp.Model.DAL
{
	public class Web_product_component_detailsDAL
	{
		public static List<Web_product_component_details> GetForIds(IList<int> cprod_ids)
		{
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
			{
				conn.Open();
				return conn.Query<Web_product_component_details>(@"SELECT
				`mast_products`.`pack_GW`,
				`mast_products`.`prod_nw`,				
				`cust_products`.`cprod_code1` AS `cprod_code1`,
				`cust_products`.`cprod_name` AS `cprod_name`,
				cust_products.cprod_retail_web_override,
				`product_type`.`product_type_desc` AS `product_type_desc`,
				`mast_products`.`pack_length` AS `pack_length`,
				`mast_products`.`pack_width` AS `pack_width`,
				`mast_products`.`pack_height` AS `pack_height`,
				`mast_products`.`prod_finish` AS `prod_finish`,
				`mast_products`.`prod_length` AS `prod_length`,
				`mast_products`.`prod_width` AS `prod_width`,
				`mast_products`.`prod_height` AS `prod_height`,
				`cust_products`.`cprod_retail` AS `cprod_retail`,
				`product_type`.`prouct_type_seq` AS `product_type_seq`,
				`mast_products`.`flow02` AS `flow02`,
				`mast_products`.`flow05` AS `flow05`,
				`mast_products`.`flow10` AS `flow10`,
				`mast_products`.`flow20` AS `flow20`,
				`mast_products`.`flow30` AS `flow30`,
				`mast_products`.`aerator02` AS `aerator02`,
				`mast_products`.`aerator05` AS `aerator05`,
				`mast_products`.`aerator10` AS `aerator10`,
				`mast_products`.`aerator20` AS `aerator20`,
				`mast_products`.`aerator30` AS `aerator30`,				
				`mast_products`.`prod_material` AS `prod_material`,
				`cust_products`.`whitebook_cprod_name` AS `whitebook_cprod_name`,
				`cust_products`.`cprod_status` AS `cprod_status`,
				cust_products.cprod_id,
				`mast_products`.`glass_thickness` AS `glass_thickness`,
				`cust_products`.`sale_retail` AS `sale_retail`,				
				`cust_products`.`whitebook_cprod_code1` AS `whitebook_cprod_code1`,
				`mast_products`.`mast_id` AS `mast_id`,
				`cust_products`.`product_type` AS `product_type`,
				`mast_products`.`category1` AS `category1`,				
				`mast_products`.`dimensions_note` AS `dimensions_note`,				
				`cust_products`.`cprod_instructions` AS `cprod_instructions`
			FROM
				`cust_products` INNER JOIN `mast_products` ON `cust_products`.`cprod_mast` = `mast_products`.`mast_id`
				LEFT OUTER JOIN `product_type` ON `cust_products`.`product_type` = `product_type`.`product_type_id`
				WHERE cust_products.cprod_id IN @ids
			", new { ids = cprod_ids}).ToList();
			}
		}
	}
}
		