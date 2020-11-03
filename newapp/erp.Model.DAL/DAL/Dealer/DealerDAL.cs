using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using erp.Model.DAL.Properties;
using Dapper;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace erp.Model.DAL
{
	public class DealerDAL
	{

		/// <summary>
		/// Simple variant without gold and silver
		/// </summary>
		/// <param name="website"></param>
		/// <param name="latitude"></param>
		/// <param name="longitude"></param>
		/// <returns></returns>
		public static List<Dealer> GetNearestDealers(string website, double latitude, double longitude,int? web_unique = null,int? web_sub_sub_category = null, int numOfResults = 20)
		{
//            List<Dealer> result = new List<Dealer>();
//            Brand b = BrandsDAL.GetByCode(website);
//            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
//            {
//                string sql = string.Empty;
//                conn.Open();
//                if (b.dealerstatus_manual == null || !b.dealerstatus_manual.Value)
//                {
//                    sql =
//                        string.Format(
//                            @"SELECT {1}.*, (SELECT user_email3 FROM users WHERE user_id = {1}.user_type LIMIT 1) AS dist_email FROM {1}
//                                                    WHERE hide_1 = 1 AND (EXISTS(SELECT dealer_displays.web_unique
//                                                                FROM dealer_displays INNER JOIN web_products ON web_products.web_unique = dealer_displays.web_unique
//                                                                WHERE dealer_displays.client_id = {1}.user_id AND web_products.web_site = @website) OR {1}.default_brand = {0})",
//                            b.brand_id, b.dealerstatus_view);
//                }
//                else
//                {
//                    sql = @"SELECT dealers.*,(SELECT user_email3 FROM users WHERE user_id = dealers.user_type LIMIT 1) AS dist_email FROM dealers
//                                                    WHERE hide_1 = 1 AND EXISTS (SELECT * FROM dealer_brandstatus WHERE dealer_id = dealers.user_id AND brand_id = @brand_id)";
//                }
//                var cmd = Utils.GetCommand(sql, conn);
//                cmd.Parameters.Add(new MySqlParameter("@website", website));
//                cmd.Parameters.AddWithValue("@brand_id", b.brand_id);
//                MySqlDataReader dr = cmd.ExecuteReader();
//                while (dr.Read())
//                {
//                    Dealer d = GetDealerFromReader(dr);
//                    if (d.latitude != null && d.longitude != null)
//                    {
//                        d.Distance = GeoUtils.distance(latitude, longitude, d.latitude.Value, d.longitude.Value, 'M');
//                        if (b.dealerstatus_manual != null && b.dealerstatus_manual.Value)
//                        {
//                            d.BrandStatuses = GetStatusesFromTable(d.user_id);
//                            if (d.BrandStatuses.ContainsKey(b.brand_id))
//                                d.brand_status = d.BrandStatuses[b.brand_id];
//                        }
//                        result.Add(d);
//                    }
//                }
//                result = result.OrderBy(d => d.Distance).ToList();
//                dr.Close();
//            }
//            return result;

			Brand b = BrandsDAL.GetByCode(website);
            double? lat = latitude, lng = longitude;

			var dealers = GetDealersForBrand(website, web_unique, web_sub_sub_category,latitude:lat, longitude: lng,numOfResults: numOfResults);
            
             
                        
			foreach (var d in dealers)
			{
				if (d.latitude != null && d.longitude != null)
				{
					d.Distance = GeoUtils.distance(latitude, longitude, d.latitude.Value, d.longitude.Value, 'M');
					if (b.dealerstatus_manual != null && b.dealerstatus_manual.Value)
					{
						var de = d;
						d.BrandStatuses = GetStatusesFromTable(d.user_id, ref de);
						if (d.BrandStatuses.ContainsKey(b.brand_id))
							d.brand_status = d.BrandStatuses[b.brand_id];
					}
				}
				else
				{
					d.Distance = Double.MaxValue;
				}
			}
			return dealers.OrderBy(d => d.Distance).ToList();
		}
		/***/
		public static List<PostcodeAreas> GetPostcodeAreas()
		{
			var result = new List<PostcodeAreas>();
			using (var conn=new MySqlConnection(Settings.Default.ConnString))
			{
				conn.Open();

				string sql = String.Empty;
				sql = String.Format(@"SELECT *" +
									"FROM postcode_areas_for_map ");
				var cmd = Utils.GetCommand(sql, conn);
				var dr = cmd.ExecuteReader();
				while (dr.Read())
				{
					result.Add(new PostcodeAreas
						{
							Id = (int)dr["Id"],
							PostcodeArea = String.Empty+dr["Postcode_Area"],
							PostTown = String.Empty+dr["Post_Town"],
							NumOfRegion = (int)dr["Nuber_of_Region"],
							Region = String.Empty+dr["Region"]
						});
				}
				dr.Close();
			}

			return result;
		}

		public static List<DealerImagesWebOnRegion> GetProductImagesOnWeb_New(int catId = 0, int brandId = 0,bool forBrandCategoriesAll = false, string ids="")
		{
			var result = new List<DealerImagesWebOnRegion>();

            var idsList = new List<int>();

			using (var conn = new MySqlConnection(Settings.Default.ConnString))
			{
				conn.Open();
				var cmd = Utils.GetCommand("", conn);

				var cats = new List<Web_category>();
				var catsIds = new List<Web_category>();

				if(forBrandCategoriesAll)
				{
					catsIds = Web_categoryDAL.GetForBrand(brandId);
					foreach(var i in catsIds)
					{
						cats.AddRange( Web_categoryDAL.GetAllChildren(i.category_id));
					}

				}
                else if(ids != "")
                {

                }
				else
				{
					cats.Add(new Web_category { category_id = catId });
					cats.AddRange(Web_categoryDAL.GetAllChildren(catId));
				}

				string sql = String.Empty;
				cmd.CommandText = String.Format(@"SELECT dealers.*,dealer_images.*,dealer_image_displays.*,web_product_new.*,web_product_category.*,web_category.*,dealer_distributors.*,users.* FROM
dealers
INNER JOIN dealer_images ON dealers.user_id = dealer_images.dealer_id
INNER JOIN dealer_image_displays ON dealer_images.image_unique = dealer_image_displays.image_id
INNER JOIN web_product_new ON dealer_image_displays.web_unique = web_product_new.web_unique
INNER JOIN web_product_category ON web_product_new.web_unique = web_product_category.web_unique
INNER JOIN web_category ON web_category.category_id = web_product_category.category_id
INNER JOIN dealer_distributors ON dealers.user_id = dealer_distributors.dealer_id
INNER JOIN users ON dealer_distributors.distributor_id = users.user_id
WHERE
web_product_category.category_id IN ({0})
AND

dealers.sales_registered_2014 = 1

", Utils.CreateParametersFromIdList(cmd,cats.Select(c=>c.category_id).ToList()));



				cmd.Parameters.AddWithValue("@catId", catId);
				cmd.Parameters.AddWithValue("@brandId", brandId);

				try
				{
					var dr = cmd.ExecuteReader();


				while (dr.Read())
				{

					result.Add(new DealerImagesWebOnRegion
					{
						user_id = (int)dr["user_id"],
						image_id = (int)dr["image_id"],
						longitude = Utilities.FromDbValue<double>(dr["longitude"]),
						latitude = Utilities.FromDbValue<double>(dr["latitude"]),
						user_name = String.Empty + dr["user_name"],
						postcode = String.Empty + dr["postcode"],
						dealer_image = String.Empty + dr["dealer_image"],
						web_name = String.Empty + dr["web_name"],
                        web_unique=(int)dr["web_unique"],
						web_category = Utilities.FromDbValue<int>(dr["category_id"]),
						category_name = String.Empty + dr["web_description"],
						numbOfRegion = AddRegion(String.Empty + dr["postcode"].ToString().ToUpper()),
						web_site_id = Utilities.FromDbValue<int>(dr["web_site_id"]),
                        distributor_id=(int)dr["distributor_id"],
                        reporting_name=String.Empty + dr["reporting_name"]
					});
				}
					dr.Close();
				}
				catch
				{

					return null;
				}



			}

			return result;
		}

        public static List<Dot> GetDots(string viewName)
        {
            var result = new List<Dot>();
            using (var conn = new MySqlConnection(Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("", conn);
                cmd.CommandText = String.Format(@"SELECT * FROM {0}",viewName);
                try
                {
                    var dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        Dot tempO = new Dot();

                        tempO.id = (int)dr["user_id"];
                        tempO.name = String.Empty + dr["user_name"];
                        tempO.longitude = Utilities.FromDbValue<double>(dr["longitude"]);
                        tempO.latitude = Utilities.FromDbValue<double>(dr["latitude"]);
                        try
                        {
                            tempO.action_flag = (int)dr["action_flag"];
                        }

                        catch (Exception)
                        {

                            //throw;
                        }
                        try
                        {
                            tempO.postcode = String.Empty + dr["postcode"];
                        }
                        catch (Exception)
                        {

                        }
                        try{ tempO.location = String.Empty + dr["text"]; }catch { }
                        try
                        {
                            tempO.location = String.Empty + dr["location"];
                        }
                        catch { }


                        result.Add(tempO);
                    }
                    dr.Close();
                }
                catch (Exception)
                {

                    //return null;
                }
            }
            return result;

        }


        public static List<Dot> GetDotsRed()
        {
            var result = new List<Dot>();
            using (var conn = new MySqlConnection(Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("", conn);
                cmd.CommandText = String.Format(@"SELECT * FROM map_red_dot");
                try
                {
                    var dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        result.Add(new Dot
                        {
                            id = (int)dr["user_id"],
                            name = String.Empty + dr["user_name"],
                            longitude = Utilities.FromDbValue<double>(dr["longitude"]),
                            latitude = Utilities.FromDbValue<double>(dr["latitude"]),

                        });
                    }
                    dr.Close();
                }
                catch (Exception)
                {

                    return null;
                }
            }
            return result;

        }
        public static List<Dot> GetDotsYelow()
        {
            var result = new List<Dot>();
            using(var conn=new MySqlConnection(Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("",conn);
                cmd.CommandText = String.Format(@"SELECT * FROM map_yellow_dot");
                try
	            {
                    var dr=cmd.ExecuteReader();
		            while(dr.Read())
                    {
                        Dot tempO = new Dot ();

                            tempO.id = (int)dr["user_id"];
                            tempO.name = String.Empty + dr["user_name"];
                            tempO.longitude = Utilities.FromDbValue<double>(dr["longitude"]);
                            tempO.latitude = Utilities.FromDbValue<double>(dr["latitude"]);
                            try
                            {
                                tempO.action_flag = (int)dr["action_flag"];
                            }
                            catch (Exception)
                            {

                                //throw;
                            }

                            result.Add(tempO);
                    }
                    dr.Close();
	            }
	            catch (Exception)
	            {

		            return null;
	            }
            }
            return result;

        }

        public static List<Brand> GetBrandsDealers(int year=0)
        {
            var result = new List<Brand>();
            using (var conn = new MySqlConnection(Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("",conn);
                cmd.CommandText = String.Format(
                        @"SELECT
                            dealers.user_id,
                            dealers.user_name,
                            dealers.distributor,
                            dealers.user_created,
                            dealers.sales_registered_2014
                            FROM
                            dealers
                            WHERE
                            (
                            dealers.user_created = null OR
                            YEAR(dealers.user_created) < @yearUpper)"
                    );
//                cmd.CommandText = String.Format(
//                @"SELECT
//                brands.brandname
//                FROM
//                dealers
//                INNER JOIN dealer_images ON dealers.user_id = dealer_images.dealer_id
//                INNER JOIN dealer_image_brand ON dealer_images.image_unique = dealer_image_brand.dealer_image_id
//                INNER JOIN brands ON dealer_image_brand.brand_id = brands.brand_id
//                WHERE
//                (YEAR(dealers.user_created) < @yearUpper AND
//                YEAR(dealers.user_created) > @yearLower ) AND
//                (dealers.user_country IS NULL  OR
//                dealers.user_country = '' OR dealers.user_created = NULL OR dealers.user_created = '')
//                GROUP BY
//                dealer_image_brand.brand_id");


                cmd.Parameters.AddWithValue("@yearUpper", year != 0 ? year + 1 : 2016);
                cmd.Parameters.AddWithValue("@yearLower", year != 0 ? year - 1 : 2009);

                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {

                   // var d =BrandsDAL.GetFromDataReader(dr);
                    result.Add(new Brand { brandname= string.Empty + dr["brandname"]});
                }
                dr.Close();

            }
            return result;

        }
        public static List<Dot> GetDealersForContinent(string continent="EU")
        {
            var result=new List<Dot>();
            using (var conn=new MySqlConnection(Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("",conn);
                cmd.CommandText = String.Format(@"SELECT dealers.*,countries.*
                                                    FROM dealers INNER JOIN countries ON dealers.user_country = countries.ISO2 
                                                    WHERE countries.continent_code = @continent_code AND COALESCE(dealers.hide_1,0) <> 2");
                cmd.Parameters.AddWithValue("@continent_code", continent);
                try
                {
                    var dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        var d = GetDotFromReader(dr);
                        //d.Country= Get
                        result.Add(d);
                    }
                    dr.Close();
                    foreach (var item in result)
                    {
                        item.Distributors=GetDistributors(item.id,conn);

                    }

                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Greška \t" + ex);
                    return null;
                    //throw;
                }
                return result;
            }
        }
        public static List<Dot> GetDealers(string country="",int id = 11,int year=0)
        {
            // Dictionary<int,string> brand = new Dictionary<int,string>();
            //{
            //    {1,"dealers_arcade"},{2,"dealers_burlington"},{3,"dealers_britton"}
            //}
            //Definiranje stringa koji pradstavlja ime view-a u bazi
            //int brand = 0;
            if (id == 99)
                return null;
            string brand_view = "";
            if(id > 0)
            {
                var brand = Web_siteDAL.GetByBrandId(id);
                var brandSplit = brand.name.ToLower().Split(' ');
                brand_view = String.Format("dealers_{0}",brandSplit.First());

            }




            var result = new List<Dot>();
            using (var conn = new MySqlConnection(Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("", conn);
                if (country == "UK")
                {
                    /*Određeni brend */
                    if(year==0 && id != 0)
                    {
                        cmd.CommandText = String.Format(@"SELECT user_id, longitude, latitude, user_name,distributor,postcode,user_address1,user_address2,user_address3,user_address4,user_address5
                        FROM {0}", brand_view);

                    }
                        /*Svi brendovi za određenu godinu ili za sve godine*/
                    else if(id==0)
                    {
                        cmd.CommandText = String.Format(
                            /*
                           @"SELECT
                                user_id, longitude, latitude, user_name,distributor,postcode,user_address1,user_address2,user_address3,user_address4,user_address5
                              FROM
                                dealers

                              WHERE
                                dealers.sales_registered_2014 = 1 AND(

                                YEAR(dealers.user_created) < @yearUpper OR ISNULL(dealers.user_created))"

                            */
                            /*2015*/
                            @"SELECT
                                user_id, longitude, latitude, user_name,distributor,postcode,user_address1,user_address2,user_address3,user_address4,user_address5,user_created
                              FROM
                                dealers

                              WHERE
                                ( YEAR( user_created) > 2014
                                AND
                                (YEAR(dealers.user_created) < 2016 OR ISNULL(dealers.user_created)))
                                AND
                                (
	                                ISNULL(user_country)
	                                OR
	                                user_country = ''
                                )
                                AND COALESCE(dealers.hide_1,0) <> 2
                                ORDER BY
                                dealers.user_created DESC"

                            /*2016*/
//                             @"SELECT
//                                user_id, longitude, latitude, user_name,distributor,postcode,user_address1,user_address2,user_address3,user_address4,user_address5
//                              FROM
//                                dealers
//
//                              WHERE
//                                dealers.hide_1 <> 2
//                                and
//                                dealers.sales_registered = 1
//                                AND
//                                (YEAR(dealers.user_created) < 2014 OR ISNULL(dealers.user_created))


                             );


                        cmd.Parameters.AddWithValue("@yearUpper", year != 0 ? year + 1 : 2016);
                        cmd.Parameters.AddWithValue("@yearLower",year != 0 ? year - 1 : 2009 );
                    }
                    /*Određeni brend za određenu godinu*/
                    else
                    {
                       cmd.CommandText = String.Format(@"SELECT
                                                                user_id, longitude, latitude, user_name,distributor,postcode,user_address1,user_address2,user_address3,user_address4,user_address5,user_created
                                                         FROM {0}
                                                        WHERE
                                                            (YEAR(dealers.user_created) < @yearUpper and YEAR(dealers.user_created) > @yearLower
                                                            OR {0}.user_created is null) AND COALESCE(dealers.hide_1,0) <> 2 ", brand_view);

                       cmd.Parameters.AddWithValue("@yearUpper", year + 1);
                       cmd.Parameters.AddWithValue("@yearLower", year - 1);

                    }
                }

                else
                {
                    cmd.CommandText = String.Format(@"SELECT user_id, longitude, latitude, user_name,distributor,postcode,user_country,user_address1,user_address2,
                                                        user_address3,user_address4,user_address5,hide_1   
                                                     FROM dealers WHERE user_country=@user_country and hide_1 = 1 ");
                    cmd.Parameters.AddWithValue("@user_country",country);
                }
                try
                {
                    var dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        var d = GetDotFromReader(dr);
                       // d.Brand = brand;
                        result.Add(d);
                    }
                    dr.Close();
                    foreach(var item in result)
                    {
                        item.Distributors = GetDistributors(item.id, conn);

                    }
                }
                catch (Exception ex)
                {
                    //System.Diagnostics.Debug.WriteLine("Greška \t"+ex);
                    return null;
                }
            }
            return result;

        }


        public static List<Dot> GetDotsWhite()
        {
            var result = new List<Dot>();
            using (var conn = new MySqlConnection(Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("", conn);
                cmd.CommandText = String.Format(@"SELECT * FROM map_white_dot");
                try
                {
                    var dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        result.Add(new Dot
                        {
                            id = (int)dr["user_id"],
                            name = String.Empty + dr["user_name"],
                            longitude = Utilities.FromDbValue<double>(dr["longitude"]),
                            latitude = Utilities.FromDbValue<double>(dr["latitude"])


                        });

                    }
                    dr.Close();
                }
                catch (Exception)
                {

                    return null;
                }
            }
            return result;

        }

        public static List<Dot> GetDotsGreen()
        {
            var result = new List<Dot>();
            using (var conn = new MySqlConnection(Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("", conn);
                cmd.CommandText = String.Format(@"SELECT * FROM map_green_dot");
                try
                {
                    var dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        result.Add(new Dot
                        {
                            id = (int)dr["user_id"],
                            name = String.Empty + dr["user_name"],
                            longitude = Utilities.FromDbValue<double>(dr["longitude"]),
                            latitude = Utilities.FromDbValue<double>(dr["latitude"]),
                            action_flag=(int)dr["action_flag"]

                        });

                    }
                    dr.Close();
                }
                catch (Exception)
                {

                    return null;
                }
            }
            return result;

        }

        public static List<Dot> GetDotsPink()
        {
            var result = new List<Dot>();
            using (var conn = new MySqlConnection(Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("", conn);
                cmd.CommandText = String.Format(@"SELECT * FROM map_pink_dot");
                try
                {
                    var dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        result.Add(new Dot
                        {
                            id = (int)dr["user_id"],
                            name = String.Empty + dr["user_name"],
                            longitude = Utilities.FromDbValue<double>(dr["longitude"]),
                            latitude = Utilities.FromDbValue<double>(dr["latitude"])

                        });
                    }
                    dr.Close();
                }
                catch (Exception)
                {

                    return null;
                }
            }
            return result;

        }


	   /***/

		//public static List<DealerImagesWebOnRegion> GetProductImagesOnWeb(int catId = 204, int brandId=42 )
		//{

		//    /*Category u WC brandcatid= 201, 204, to su Bath=201 Basins, Tups itd,
		//     * webCategory predstavlja  sub category u WC Moder, Traditional,  */
		//    var result=new List<DealerImagesWebOnRegion>();
		//    using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
		//    {
		//        //dealers.*, dealer_images.*, dealer_image_displays.*,web_products_wc.*
		//        conn.Open();

		//        string sql = string.Empty;
		//        sql  = string.Format(@"SELECT dealer_image_displays.image_id AS image_id, " +
		//                                "dealer_image_displays.web_unique AS web_unique, " +
		//                                "dealers.user_id AS user_id, " +
		//                                "dealers.user_name AS user_name, " +
		//                                "dealers.postcode AS postcode, " +
		//                                "dealers.longitude AS longitude, " +
		//                                "dealers.latitude AS latitude," +
		//                                "dealer_images.image_unique AS image_unique, " +
		//                                "dealer_images.dealer_image AS dealer_image, " +
		//                                "dealer_images.store_page AS store_page, " +
		//                                "web_products.web_name AS web_name, " +
		//                                "web_products.web_site AS web_site, " +
		//                                "web_products.web_category AS web_category, "+
		//                                "brand_categories.web_description AS web_description, "+
		//                                "brand_categories.brand_cat_desc AS category_name, " +
		//                                "brand_categories.brand AS brand, " +
		//                                "web_products.web_sub_category AS web_sub_category, " +
		//                                "brand_categories_sub.brand_sub_desc AS brand_sub_desc " +


		//                                //"web_products_wc.web_sub_category AS web_sub_category, " +
		//                                //"web_products_wc.category_name AS category_name, "  +
		//                                //"web_products_wc.brand_sub_desc AS subcategory_desc "+
		//                                "FROM " +
		//                                    "(((((dealers "+
		//                                    "join dealer_images on((dealers.user_id = dealer_images.dealer_id))) " +
		//                                    "join dealer_image_displays on((dealer_images.image_unique = dealer_image_displays.image_id))) " +
		//                                    "join web_products on((web_products.web_unique = dealer_image_displays.web_unique))) " +
		//                                    "join brand_categories on((web_products.web_category = brand_categories.brand_cat_id))) " +
		//                                    "join brand_categories_sub on((web_products.web_sub_category = brand_categories_sub.brand_sub_id)))" +

		//                            "WHERE " +
		//                             "brand_categories.brand_cat_id = @web_category " +
		//                            "OR " +
		//                             "brand_categories.brand = @web_code "
		//                            , conn);

		//        var cmd = Utils.GetCommand(sql, conn);
		//        cmd.Parameters.AddWithValue("@web_category", catId);
		//        cmd.Parameters.AddWithValue("@web_code", brandId);
		//        var dr = cmd.ExecuteReader();
		//        while (dr.Read())
		//        {

		//            result.Add(new DealerImagesWebOnRegion
		//                {
		//                    user_id= (int)dr["user_id"],
		//                    image_id= (int)dr["image_id"],
		//                    longitude=Utilities.FromDbValue<double>(dr["longitude"]),
		//                    latitude=Utilities.FromDbValue<double>(dr["latitude"]),
		//                    user_name=String.Empty + dr["user_name"],
		//                    postcode=String.Empty +dr["postcode"],
		//                    dealer_image=String.Empty +dr["dealer_image"],
		//                    web_name=String.Empty + dr["web_name"],
		//                    web_category = Utilities.FromDbValue<int>(dr["web_category"]),
		//                    web_site=String.Empty + dr["web_site"],
		//                    web_sub_category=Utilities.FromDbValue<int>(dr["web_sub_category"]),
		//                    brand = Utilities.FromDbValue<int>(dr["brand"]),
		//                    category_name = String.Empty + dr["web_description"],
		//                    brand_sub_desc = String.Empty + dr["brand_sub_desc"],
		//                    numbOfRegion = AddRegion(String.Empty + dr["postcode"].ToString().ToUpper())
		//                });
		//        }

		//        dr.Close();
		//    }

		//    return result;
		//}


		/***/
		public static List<Dealer> GetNearestDealers(string website, double latitude, double longitude, bool isGold = false, bool isSilver=false, int? web_unique = null)
		{
//            List<Dealer> result = new List<Dealer>();
//            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
//            {
//                conn.Open();
//                Brand b = BrandsDAL.GetByCode(website);
//                if (b != null && !string.IsNullOrEmpty(b.dealerstatus_view))
//                {
//                    MySqlCommand cmd =
//                        Utils.GetCommand(
//                            string.Format(@"SELECT {0}.*,(SELECT user_email3 FROM users WHERE user_id = {0}.user_type LIMIT 1) AS dist_email,
//                                                         (SELECT userwelcome FROM userusers WHERE user_id = {0}.user_type LIMIT 1) AS dist_name FROM {0} WHERE latitude BETWEEN -90 AND 90 AND longitude BETWEEN -180 AND 180 AND hide_1 = 1", b.dealerstatus_view),
//                            conn);
//                    if (isGold)
//                        cmd.CommandText += " AND (gold_override = 1 OR gold = 1)";
//                    if (isSilver)
//                        cmd.CommandText += " AND gold_override = 0 and gold = 0 and silver = 1";
//                    cmd.CommandText += string.Format(@" AND ( EXISTS (SELECT dealer_displays.web_unique FROM dealer_displays
//                                            INNER JOIN web_products ON dealer_displays.web_unique = web_products.web_unique
//                                            WHERE dealer_displays.client_id = {0}.client_id AND web_products.web_site = @website) OR {0}.default_brand = {1})", b.dealerstatus_view, b.brand_id);
//                    cmd.Parameters.Add(new MySqlParameter("@website", website));
//                    MySqlDataReader dr = cmd.ExecuteReader();
//                    while (dr.Read())
//                    {
//                        Dealer d = GetDealerFromReader(dr);
//                        if (d.latitude != null && d.longitude != null)
//                        {
//                            d.Distance = GeoUtils.distance(latitude, longitude, d.latitude.Value, d.longitude.Value, 'M');
//                            result.Add(d);
//                        }
//                    }
//                    result = result.OrderBy(d => d.Distance).ToList();
//                    dr.Close();
//                }
//            }
//            return result;
			return GetNearestDealers(new[] {website}, latitude, longitude, isGold, isSilver,web_unique);
		}

		public static List<Dealer> GetNearestDealers(string[] website_ids, double latitude, double longitude, bool isGold = false, bool isSilver = false, int? web_unique = null)
		{
			List<Dealer> result = new List<Dealer>();
			using (var conn = new MySqlConnection(Settings.Default.ConnString))
			{
				conn.Open();
				foreach (var website in website_ids)
				{
					Brand b = BrandsDAL.GetByCode(website);
					if (b != null && !String.IsNullOrEmpty(b.dealerstatus_view))
					{
						var cmd =
							Utils.GetCommand(
								String.Format(
									@"SELECT {0}.*,(SELECT user_email3 FROM users WHERE user_id = {0}.user_type LIMIT 1) AS dist_email,
															 (SELECT userwelcome FROM userusers WHERE user_id = {0}.user_type LIMIT 1) AS dist_name FROM {0} WHERE latitude BETWEEN -90 AND 90 AND longitude BETWEEN -180 AND 180 AND hide_1 = 1",
									b.dealerstatus_view),
								conn);
						if (isGold)
							cmd.CommandText += " AND (gold_override = 1 OR gold = 1)";
						if (isSilver)
							cmd.CommandText += " AND gold_override = 0 and gold = 0 and silver = 1";
						cmd.CommandText +=
							String.Format(@" AND ( EXISTS (SELECT dealer_displays.web_unique FROM dealer_displays
												INNER JOIN web_products ON dealer_displays.web_unique = web_products.web_unique
												WHERE dealer_displays.client_id = {0}.client_id AND web_products.web_site = @website) OR {0}.default_brand = {1})",
										  b.dealerstatus_view, b.brand_id);
						cmd.Parameters.Add(new MySqlParameter("@website", website));
						MySqlDataReader dr = cmd.ExecuteReader();
						while (dr.Read())
						{
							int dealer_id = (int) dr["user_id"];
							Dealer dealer = result.FirstOrDefault(d => d.user_id == dealer_id);
							if (dealer == null)
							{
								dealer = GetDealerFromReader(dr);
								if (dealer.latitude != null && dealer.longitude != null)
								{
									dealer.Distance = GeoUtils.distance(latitude, longitude, dealer.latitude.Value,
																   dealer.longitude.Value,
																   'M');
									result.Add(dealer);
								}
								dealer.BrandStatuses = new Dictionary<int, DealerBrandStatus>();

							}
							dealer.BrandStatuses[b.brand_id] = dealer.BrandStatus;

						}
						result = result.OrderBy(d => d.Distance).ToList();
						dr.Close();
					}
				}
			}
			return result;
		}

		public static Dealer GetDealerByLogin(string email, string password)
		{
			Dealer d = null;
			using (var conn = new MySqlConnection(Settings.Default.ConnString))
			{
				conn.Open();
				MySqlCommand cmd = Utils.GetCommand("SELECT * FROM dealers WHERE user_email = @email AND user_pwd = @password", conn);
				cmd.Parameters.AddWithValue("@email", email);
				cmd.Parameters.AddWithValue("@password", password);
				MySqlDataReader dr = cmd.ExecuteReader();
				if (dr.Read())
				{
					d = GetDealerFromReader(dr);
				}
				dr.Close();
			}
			return d;
		}

		public static Dealer GetDealerByEmail(string email)
		{
			Dealer d = null;
			using (var conn = new MySqlConnection(Settings.Default.ConnString))
			{
				conn.Open();
				var cmd = Utils.GetCommand("SELECT * FROM dealers  WHERE user_email = @email", conn);
				cmd.Parameters.AddWithValue("@email", email);
				var dr = cmd.ExecuteReader();
				if (dr.Read())
				{
					d = GetDealerFromReader(dr);
				}
				dr.Close();
			}
			return d;
		}

		public static List<Dealer> GetAll()
		{
			var result = new List<Dealer>();
			using (var conn = new MySqlConnection(Settings.Default.ConnString))
			{
				conn.Open();
				var cmd = Utils.GetCommand("SELECT * FROM dealers", conn);
				var dr = cmd.ExecuteReader();
				while (dr.Read())
				{
					result.Add(GetDealerFromReader(dr));
				}
                dr.Close();
			    foreach (var d in result)
			    {
			        d.Dealer_Images = Dealer_imagesDAL.GetByDealer(d.user_id);
			    }
			}
			return result;
		}

		public static List<DealerImagesWebOnRegion> GetAllAr()
		{
			var result = new List<DealerImagesWebOnRegion>();
			using (var conn = new MySqlConnection(Settings.Default.ConnString))
			{
				conn.Open();
				var cmd = Utils.GetCommand(" SELECT dealers_burlington.user_id ,dealers_burlington.user_name,dealers_burlington.postcode,dealers_burlington.longitude,dealers_burlington.latitude,dealers_burlington.brand_code FROM dealers_burlington  WHERE dealers_burlington.sales_registered = 1", conn);
				// WHERE dealers_burlington.brand_code <> \"\"
				var dr = cmd.ExecuteReader();
				while (dr.Read())
				{

					result.Add(new DealerImagesWebOnRegion
						{
							user_id = (int)dr["user_id"],
							user_name= String.Empty + dr["user_name"],
							longitude = Utilities.FromDbValue<double>(dr["longitude"]),
							latitude = Utilities.FromDbValue<double>(dr["latitude"]),
							postcode=String.Empty + dr["postcode"],
							numbOfRegion = AddRegion(String.Empty + dr["postcode"].ToString().ToUpper())
						});


				}
			}
			return result;
		}

        public static List<Dealer> GetByCountryCode(string code)
        {
            var result = new List<Dealer>();
            using (var conn = new MySqlConnection(Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM dealers AS d INNER JOIN countries as c ON c.ISO2=d.user_country WHERE d.user_country = @code AND hide_1 = 1", conn);
                cmd.Parameters.AddWithValue("@code",code);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetDealerFromReader(dr));
                }
                dr.Close();
            }
            return result;
        }

        public static bool HasDealers(string code)
        {
            var result = false;
            using (var conn = new MySqlConnection(Settings.Default.ConnString)) {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT COUNT(*) FROM dealers AS d INNER JOIN countries as c ON c.ISO2=d.user_country WHERE d.user_country = @code ", conn);
                cmd.Parameters.AddWithValue("@code", code);
                result = Utilities.BoolFromLong(cmd.ExecuteScalar()) ?? false;
            }
            return result;
        }

		public static Dealer GetDealerForBrand(int dealer_id, string brand_code)
		{
            //Dealer d = null;
            //Brand b = BrandsDAL.GetByCode(brand_code);
            //if (b != null && !string.IsNullOrEmpty(b.dealerstatus_view))
            //{
            //    using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            //    {
            //        conn.Open();
            //        var cmd = Utils.GetCommand(string.Format("SELECT * FROM {0}  WHERE client_id = @dealer_id", b.dealerstatus_view), conn);
            //        cmd.CommandTimeout = 300;
            //        cmd.Parameters.AddWithValue("@dealer_id", dealer_id);
            //        var dr = cmd.ExecuteReader();
            //        if (dr.Read())
            //        {
            //            d = GetDealerFromReader(dr);
            //            d.Images = GetImages(d);
            //            d.BrandStatuses = GetStatusesFromTable(dealer_id, ref d);
            //        }
            //        dr.Close();
            //    }
            //}
		    var dealers = GetDealersForBrand(brand_code, dealer_id: dealer_id);
		    return dealers.Count > 0 ? dealers.First() : null;
		}

		public static List<Dealer> GetDealersForBrand(string brand_code, int? web_unique = null, int? web_sub_sub_category = null, int? dealer_id = null, double? latitude = null, double? longitude = null, int numOfResults = 20)
		{
			var result = new List<Dealer>();
			Brand b = BrandsDAL.GetByCode(brand_code);

			if (string.IsNullOrEmpty(Settings.Default.DealerSearchView) && string.IsNullOrEmpty(b.dealersearch_view))
            {
                if (b != null)
                {
                    using (var conn = new MySqlConnection(Settings.Default.ConnString))
                    {
                        conn.Open();
                        string sql = String.Empty;
                        var cmd = Utils.GetCommand("", conn);
                        var productCondition = web_unique != null ?
                            @"{1} ( EXISTS (SELECT web_unique FROM dealer_displays WHERE client_id = {0}.user_id AND dealer_displays.web_unique = @web_unique) OR
											  EXISTS(SELECT web_unique FROM dealer_image_displays INNER JOIN dealer_images ON dealer_image_displays.image_id = dealer_images.image_unique
													  WHERE dealer_images.dealer_id = {0}.user_id AND dealer_image_displays.web_unique = @web_unique) )" : "";

                        var subCategoryCondition = web_sub_sub_category != null ?
                            @"{1} ( EXISTS (SELECT dealer_displays.web_unique FROM dealer_displays INNER JOIN web_products ON dealer_displays.web_unique = web_products.web_unique WHERE client_id = {0}.user_id AND web_products.web_sub_sub_category = @web_sub_sub_category) OR
											  EXISTS(SELECT dealer_image_displays.web_unique FROM dealer_image_displays INNER JOIN dealer_images ON dealer_image_displays.image_id = dealer_images.image_unique
													  INNER JOIN web_products ON dealer_image_displays.web_unique = web_products.web_unique
													  WHERE dealer_images.dealer_id = {0}.user_id AND web_products.web_sub_sub_category = @web_sub_sub_category ) )" : "";


                        if (!String.IsNullOrEmpty(b.dealerstatus_view))
                        {
                            cmd.CommandText = $@"SELECT {b.dealerstatus_view}.*,dist.user_id AS dist_id, dist.user_name AS dist_name, dist.customer_code AS dist_code,dist.user_email AS dist_email, NULL AS brand_status,
														(SELECT COUNT(*) FROM dealer_images WHERE dealer_id = {b.dealerstatus_view}.user_id) AS numImages,
                                                        {(latitude != null && longitude != null ? $"ST_Distance(Point(@lat,@long),Point(COALESCE({b.dealerstatus_view}.latitude,0), COALESCE({b.dealerstatus_view}.longitude,0)))" : "NULL")} AS distance
                                                        FROM {b.dealerstatus_view}
														INNER JOIN dealer_distributors ON {b.dealerstatus_view}.user_id = dealer_distributors.dealer_id
														INNER JOIN users AS dist ON dealer_distributors.distributor_id = dist.user_id WHERE ({b.dealerstatus_view}.user_id = @dealer_id OR @dealer_id IS NULL) AND
										 (EXISTS (SELECT dealer_image_displays.web_unique FROM dealer_image_displays INNER JOIN dealer_images ON dealer_image_displays.image_id = dealer_images.image_unique
															INNER JOIN web_product_new ON dealer_image_displays.web_unique = web_product_new.web_unique INNER JOIN
															web_site ON web_product_new.web_site_id = web_site.id
															WHERE web_site.brand_id = @brand_id AND dealer_images.dealer_id = {b.dealerstatus_view}.user_id) OR
																default_brand = @brand_id OR
															EXISTS (SELECT dealer_id FROM dealer_brandstatus WHERE dealer_id = {b.dealerstatus_view}.user_id AND brand_id = @brand_id))
										   {String.Format(productCondition, b.dealerstatus_view, "AND")} {String.Format(subCategoryCondition, b.dealerstatus_view, "AND")} 
                                            {(latitude != null ? $" AND {b.dealerstatus_view}.latitude IS NOT NULL AND {b.dealerstatus_view}.longitude IS NOT NULL" : "")}
                                            ORDER BY {(latitude != null ? $"distance LIMIT {numOfResults}" : $"{b.dealerstatus_view}.user_id")}";
                                             
                            cmd.CommandText += ";";
                        }


                        cmd.CommandText += $@"SELECT dealers.*,dist.user_id AS dist_id, dist.user_name AS dist_name, dist.customer_code AS dist_code,dist.user_email AS dist_email,dealer_brandstatus.brand_status,
												   (SELECT COUNT(*) FROM dealer_images WHERE dealer_id = dealers.user_id) AS numImages,
                                            {(latitude != null && longitude != null ? $"ST_Distance(Point(@lat, @long), Point(COALESCE(dealers.latitude,0), COALESCE(dealers.longitude,0)))" : "NULL")} AS distance
                                FROM dealers INNER JOIN dealer_distributors ON dealers.user_id = dealer_distributors.dealer_id INNER JOIN users AS dist ON dealer_distributors.distributor_id = dist.user_id
                                LEFT OUTER JOIN dealer_brandstatus ON dealers.user_id = dealer_brandstatus.dealer_id AND dealer_brandstatus.brand_id = @brand_id
					            WHERE (dealer_brandstatus.dealer_id IS NOT NULL OR
						        default_brand = @brand_id ) AND (dealers.user_id = @dealer_id OR @dealer_id IS NULL)
                                {String.Format(productCondition, "dealers", " AND ")} {String.Format(subCategoryCondition, "dealers", "AND")} 
                                {(latitude != null ? $" AND dealers.latitude IS NOT NULL AND dealers.longitude IS NOT NULL" : "")}
                                ORDER BY {(latitude != null ? $"distance LIMIT {numOfResults}" : "dealers.user_id")}";
                                
                        cmd.Parameters.AddWithValue("@code", brand_code);

                        //                    cmd.CommandText += string.Format(@"SELECT dealers.*,dist.user_name as dist_name, dist.customer_code As dist_code,
                        //                            (SELECT dealer_brandstatus.brand_status FROM dealer_brandstatus WHERE dealers.user_id = dealer_brandstatus.dealer_id AND dealer_brandstatus.brand_id = @brand_id) AS brand_status  ,
                        //                                                    (SELECT COUNT(*) FROM dealer_images WHERE dealer_id = dealers.user_id) AS numImages FROM dealers INNER JOIN users AS dist ON dealers.user_type = dist.user_id
                        //
                        //                    WHERE sales_registered=1 AND ( EXISTS(SELECT unique_id FROM dealer_displays INNER JOIN web_products ON dealer_displays.web_unique = web_products.web_unique
                        //                            WHERE web_products.web_site = @code AND dealer_displays.client_id = dealers.user_id) OR
                        //                        default_brand = @brand_id ) {0} {1}", string.Format(productCondition, "dealers", " AND "), string.Format(subCategoryCondition, "dealers", "AND"));
                        //                    cmd.Parameters.AddWithValue("@code", brand_code);
                        if (web_unique != null)
                            cmd.Parameters.AddWithValue("@web_unique", web_unique);
                        if (web_sub_sub_category != null)
                            cmd.Parameters.AddWithValue("@web_sub_sub_category", web_sub_sub_category);
                        cmd.Parameters.AddWithValue("@dealer_id", Utilities.ToDBNull(dealer_id));
                        cmd.Parameters.AddWithValue("@brand_id", b.brand_id);
                        if (latitude != null)
                            cmd.Parameters.AddWithValue("@lat", latitude);
                        if (longitude != null)
                            cmd.Parameters.AddWithValue("@long", longitude);
                        cmd.CommandTimeout = 300;
                        var dr = cmd.ExecuteReader();
                        var ids = new HashSet<int>();
                        int id = -1;
                        Dealer dealer = null;
                        while (dr.Read())
                        {
                            var currId = (int)dr["user_id"];
                            if (currId != id)
                            {
                                id = currId;
                                dealer = GetDealerFromReader(dr);
                                ids.Add(dealer.user_id);
                                result.Add(dealer);
                                dealer.Distributors = new List<Company>();

                            }
                            var dist = new Company
                            {
                                user_id = (int)dr["dist_id"],
                                user_name = String.Empty + dr["dist_name"],
                                customer_code = String.Empty + dr["dist_code"]

                            };
                            dealer.Distributors.Add(dist);

                        }
                        if (dr.NextResult())
                        {
                            id = -1;
                            while (dr.Read())
                            {
                                var currId = (int)dr["user_id"];
                                if (!ids.Contains(currId))
                                {
                                    if (currId != id)
                                    {
                                        id = currId;
                                        dealer = GetDealerFromReader(dr);
                                        result.Add(dealer);
                                        dealer.Distributors = new List<Company>();
                                    }
                                    var dist = new Company
                                    {
                                        user_id = (int)dr["dist_id"],
                                        user_name = String.Empty + dr["dist_name"],
                                        customer_code = String.Empty + dr["dist_code"]
                                    };
                                    dealer.Distributors.Add(dist);

                                }
                            }
                        }
                        dr.Close();

                    }
                }

            }
            else
            {
                using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
                {
					var view = b.dealersearch_view ?? Settings.Default.DealerSearchView;
                    result = conn.Query<Dealer>("SELECT * FROM " + view).ToList();
                }
            }


            
			return result;
		}

        public static int GetNextDealerForBrand(int dealer_id, int brand_id)
        {
            object result = 0;

            using (var conn = new MySqlConnection(Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand(@"SELECT * FROM dealers
                                                WHERE user_id > @dealer_id
                                                AND default_brand = @brand_id
                                                ORDER BY user_id LIMIT 1", conn);
                cmd.Parameters.AddWithValue("dealer_id", dealer_id);
                cmd.Parameters.AddWithValue("brand_id", brand_id);
                result = cmd.ExecuteScalar();
            }
            return result != null ? (int)result : 0;
        }

        public static int GetPrevDealerForBrand(int dealer_id, int brand_id)
        {
            object result = 0;

            using (var conn = new MySqlConnection(Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand(@"SELECT * FROM dealers
                                                WHERE user_id < @dealer_id
                                                AND default_brand = @brand_id
                                                ORDER BY user_id DESC LIMIT 1", conn);
                cmd.Parameters.AddWithValue("dealer_id", dealer_id);
                cmd.Parameters.AddWithValue("brand_id", brand_id);
                result = cmd.ExecuteScalar();
            }
            return result != null ? (int)result : 0;
        }

		public static int GetDealerCountForBrand(string brand_code)
		{
			int result = 0;
			Brand b = BrandsDAL.GetByCode(brand_code);
			if (b != null && !String.IsNullOrEmpty(b.dealerstatus_view))
			{
				using (var conn = new MySqlConnection(Settings.Default.ConnString))
				{
					conn.Open();
					var cmd = Utils.GetCommand(String.Format(@"SELECT COUNT(*) FROM {0} ", b.dealerstatus_view), conn);
					result = Convert.ToInt32(cmd.ExecuteScalar());
				}
			}
			return result;
		}

        public static int GetFirstDealerForBrand(int brand_id)
        {
            int result = 0;

            using (var conn = new MySqlConnection(Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand(@"SELECT user_id FROM dealers
                                                                WHERE default_brand = @brand_id
                                                                ORDER BY user_id LIMIT 1", conn);
                cmd.Parameters.AddWithValue("brand_id", brand_id);
                result = Convert.ToInt32(cmd.ExecuteScalar());
            }
            return result;
        }

        public static int GetLastDealerForBrand(int brand_id)
        {
            int result = 0;

            using (var conn = new MySqlConnection(Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand(@"SELECT user_id FROM dealers
                                                                WHERE default_brand = @brand_id
                                                                ORDER BY user_id DESC LIMIT 1", conn);
                cmd.Parameters.AddWithValue("brand_id", brand_id);
                result = Convert.ToInt32(cmd.ExecuteScalar());
            }
            return result;
        }

		public static List<DealerStat> GetDealerStatForBrand(int brand_id, DateTime? from= null, DateTime? to = null,bool salesregOnly = false)
		{
			var result = new List<DealerStat>();
			var b = BrandsDAL.GetById(brand_id);
			if (b != null)
			{
				if (!String.IsNullOrEmpty(b.dealerstatus_view))
				{
					using (var conn = new MySqlConnection(Settings.Default.ConnString))
					{
						conn.Open();
						string sql = String.Empty;

						sql = String.Format(
							@"SELECT COUNT(*) AS Count, Users.user_id, users.user_name,users.customer_code,
							(SELECT COUNT(DISTINCT {0}.user_id) FROM {0} INNER JOIN dealer_images ON dealer_images.dealer_id = {0}.user_id
								WHERE {0}.user_type = users.user_id AND (@from IS NULL OR {0}.user_created >= @from) AND (@to IS NULL OR {0}.user_created <= @to) {1}) AS DealersWithImages
							FROM {0} INNER JOIN users ON {0}.user_type = users.user_id
							WHERE (@from IS NULL OR {0}.user_created >= @from) AND (@to IS NULL OR {0}.user_created <= @to) {1}
							GROUP BY users.user_id, users.user_name,users.customer_code  ", b.dealerstatus_view,
							salesregOnly ? String.Format(" AND {0}.sales_registered = 1", b.dealerstatus_view) : "");

						var cmd = Utils.GetCommand(sql, conn);
						cmd.Parameters.AddWithValue("@from", @from != null ? (object)@from.Value : DBNull.Value);
						cmd.Parameters.AddWithValue("@to", to != null ? (object)to.Value : DBNull.Value);
						var dr = cmd.ExecuteReader();
						while (dr.Read())
						{
							result.Add(new DealerStat
							{
								brand_id = brand_id,
								Count = Convert.ToInt32(dr["Count"]),
								distributor_id = (int)dr["user_id"],
								distributor_code = String.Empty + dr["customer_code"],
								NumberOfDealersWithPics = Convert.ToInt32(dr["DealersWithImages"])
							});
						}
						dr.Close();
					}
				}
				else
				{
					var dealers = GetDealersForBrand(b.code);
					if (dealers != null)
					{
						dealers =
							dealers.Where(
								d =>
								(@from == null || d.user_created >= @from) && (to == null || d.user_created <= to) &&
								(!salesregOnly || d.sales_registered)).ToList();
						result.AddRange(dealers.GroupBy(d=>d.user_type).
							Select(g=>new DealerStat{brand_id = brand_id,Count = g.Count(),distributor_id = g.Key.Value,distributor_code = g.First().DistributorCode,NumberOfDealersWithPics = g.Count(d=>d.numOfImages>0)}));
					}

				}

			}
			return result;
		}

		public static List<DealerMultiBrandStat> GetDistributorMultiBrandCount(DateTime? from= null, DateTime? to = null,bool salesregOnly = false,List<string> specialBrandCodes = null )
		{
			var result = new List<DealerMultiBrandStat>();
			using (var conn = new MySqlConnection(Settings.Default.ConnString))
			{
				conn.Open();
				var cmd = Utils.GetCommand(String.Format(@"SELECT distributor.customer_code, numOfBrands, COUNT(*) AS numOfDealers FROM
						(SELECT dealers.customer_code,dealers.user_id, COUNT(*) AS numOfBrands FROM
							(SELECT users.customer_code, dealers.user_id, dealers.user_type
							FROM  dealer_displays
							INNER JOIN web_products ON web_products.web_unique = dealer_displays.web_unique
							INNER JOIN dealers ON dealers.user_id = dealer_displays.client_id
							INNER JOIN users ON dealers.user_type = users.user_id
							INNER JOIN brands ON web_products.web_site = brands.`code`
							WHERE (@from IS NULL OR dealers.user_created >= @from) AND (@to IS NULL OR dealers.user_created <= @to) AND COALESCE(dealers.hide_1,0) <> 2 {0} AND (brands.eb_brand = 1 {1})
							GROUP BY dealer_displays.client_id, web_products.web_site) AS dealers

						GROUP BY dealers.customer_code,dealers.user_id
						HAVING COUNT(*) >= 1
						ORDER BY dealers.user_id
						) AS distributor
						GROUP BY distributor.customer_code,numOfBrands
						UNION
						SELECT distributor.customer_code, 0 as numOfBrands, COUNT(*) AS numOfDealers FROM dealers INNER JOIN users distributor ON dealers.user_type = distributor.user_id
						INNER JOIN brands ON dealers.default_brand = brands.brand_id
						WHERE (@from IS NULL OR dealers.user_created >= @from) AND (@to IS NULL OR dealers.user_created <= @to) AND COALESCE(dealers.hide_1,0) <> 2
						AND NOT EXISTS (SELECT dealer_displays.client_id FROM dealer_displays WHERE dealer_displays.client_id = dealers.user_id) AND dealers.default_brand IS NOT NULL {0} AND (brands.eb_brand = 1 {1})
						GROUP BY distributor.customer_code
						ORDER BY customer_code", salesregOnly ? " AND dealers.sales_registered = 1" : "",specialBrandCodes != null ? String.Format(" OR brands.code IN ({0})",
											   String.Join(",",specialBrandCodes.Select(s=>String.Format("'{0}'",s)))) : ""), conn);
				cmd.Parameters.AddWithValue("@from", @from != null ? (object)@from.Value : DBNull.Value);
				cmd.Parameters.AddWithValue("@to", to != null ? (object)to.Value : DBNull.Value);
				var dr = cmd.ExecuteReader();
				while (dr.Read())
				{
					result.Add(new DealerMultiBrandStat
					{
						distributor_code = String.Empty + dr["customer_code"],
						BrandCount = Convert.ToInt32(dr["numOfBrands"]),
						DealerCount = Convert.ToInt32(dr["numOfDealers"])
					});
				}
				dr.Close();
			}
			return result;
		}

		/// <summary>
		/// Returns list of dealers and
		/// </summary>
		/// <returns></returns>
		public static List<DealerMultiBrandStat2> GetDealerMultiBrandList(List<string> specialBrandCodes = null)
		{
			var result = new List<DealerMultiBrandStat2>();
			using (var conn = new MySqlConnection(Settings.Default.ConnString))
			{
				conn.Open();
				var cmd = Utils.GetCommand(String.Format(@"SELECT dealers.*, distributor.user_name AS dist_name, brands.`code`, brands.brandname,distributor.customer_code AS dist_code
								FROM dealers
								INNER JOIN users AS distributor ON distributor.user_id = dealers.user_type
								INNER JOIN dealer_displays ON dealers.user_id = dealer_displays.client_id
								INNER JOIN web_products ON dealer_displays.web_unique = web_products.web_unique
								INNER JOIN brands ON web_products.web_site = brands.`code`
								WHERE (SELECT COUNT(DISTINCT web_products.web_site) FROM dealer_displays INNER JOIN web_products ON dealer_displays.web_unique = web_products.web_unique WHERE dealer_displays.client_id = dealers.user_id) > 1
								AND (brands.eb_brand = 1 {0})
								GROUP BY dealers.user_id, web_products.web_site",specialBrandCodes != null ? String.Format(" OR brands.code IN ({0})",
											   String.Join(",",specialBrandCodes.Select(s=>String.Format("'{0}'",s)))) : ""), conn);
				var dr = cmd.ExecuteReader();
				while (dr.Read())
				{
					result.Add(new DealerMultiBrandStat2{brand_code = String.Empty + dr["code"], brandname = String.Empty + dr["brandname"],Dealer = GetDealerFromReader(dr)});
				}
			}
			return result;
		}

		//public static List<Dealer> GetDealersForBrands(DateTime? from, DateTime? to)
		//{
		//    var result = new List<Dealer>();
		//    List<Brand> brands = BrandsDAL.GetAll().Where(b => !string.IsNullOrEmpty(b.dealerstatus_view)).ToList();

		//    if (b != null && !string.IsNullOrEmpty(b.dealerstatus_view))
		//    {
		//        using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
		//        {
		//            conn.Open();
		//            var cmd = Utils.GetCommand(string.Format("SELECT {0}.*, (SELECT COUNT(*) FROM dealer_images WHERE dealer_id = {0}.user_id) AS numImages FROM {0} ", b.dealerstatus_view), conn);

		//            var dr = cmd.ExecuteReader();
		//            while (dr.Read())
		//            {
		//                result.Add(GetDealerFromReader(dr));
		//            }
		//            dr.Close();
		//        }
		//    }
		//    return result;
		//}

		public static int GetNonDisplayingDealersCount(int? user_type = null)
		{
			var result = 0;
			using (var conn = new MySqlConnection(Settings.Default.ConnString))
			{
				conn.Open();
				var cmd =
					Utils.GetCommand(
						@"SELECT COUNT(*) FROM dealers WHERE
							NOT EXISTS(SELECT dealer_image_displays.image_id FROM dealer_image_displays INNER JOIN dealer_images ON dealer_image_displays.image_id = dealer_images.image_unique WHERE dealer_id = dealers.user_id)
							AND (dealers.user_type = @user_type
								  OR EXISTS (SELECT dealer_id FROM dealer_distributors WHERE dealer_distributors.distributor_id = @user_type AND dealer_distributors.dealer_id = dealers.user_id)
								  OR @user_type IS NULL)", conn);
				cmd.Parameters.AddWithValue("@user_type", user_type != null ? (object) user_type : DBNull.Value);
				result = Convert.ToInt32(cmd.ExecuteScalar());
			}
			return result;
		}

		private static string GetSelectForDealers()
		{
			return @"SELECT dealer_displays6.*,(SELECT user_email3 FROM users WHERE user_id = dealer_displays6.user_type LIMIT 1) AS dist_email,
													 (SELECT userwelcome FROM userusers WHERE user_id = dealer_displays6.user_type LIMIT 1) AS dist_name FROM dealer_displays6 ";

		}

	    public static Dealer GetForSearchDetails(int id, bool newProducts = true)
	    {

	        if (string.IsNullOrEmpty(Settings.Default.DealerSearchView))
	            return GetById(id, newProducts);
	        else
	        {
	            using (var conn = new MySqlConnection(Settings.Default.ConnString))
	            {
	                return
	                    conn.Query<Dealer>($"SELECT * FROM {Settings.Default.DealerSearchView} WHERE user_id = {id}")
	                        .FirstOrDefault();
	            }
	        }
	    }

		public static Dealer GetForSearchDetails2(string brand_code, int id, bool newProducts = true)
		{
			Brand b = BrandsDAL.GetByCode(brand_code);

			if (string.IsNullOrEmpty(Settings.Default.DealerSearchView) && string.IsNullOrEmpty(b.dealersearch_view))				
				return GetById(id, newProducts);
			else
			{
				var view = b.dealersearch_view ?? Settings.Default.DealerSearchView;
				using (var conn = new MySqlConnection(Settings.Default.ConnString))
				{
					return
						conn.Query<Dealer>($"SELECT * FROM {view} WHERE user_id = {id}")
							.FirstOrDefault();
				}
			}
		}

		public static Dealer GetById(int id, bool newProducts = true, string connectionString = null)
		{
			Dealer d = null;
			using (var conn = new MySqlConnection(connectionString ?? Settings.Default.ConnString))
			{
				conn.Open();
				var cmd = Utils.GetCommand("SELECT * FROM dealers WHERE user_id = @id", conn);
				cmd.Parameters.AddWithValue("@id", id);
				MySqlDataReader dr = cmd.ExecuteReader();
				if (dr.Read())
				{
					d = GetDealerFromReader(dr);
                    //get images

                    //d.DisplayedProducts = GetDisplaysForDealer(id);

                    if (connectionString == null) { // exlude if use DAM, quick fix
                        d.Images = GetImages(d);

                        //d.Dealer_Images = Dealer_imagesDAL.GetByDealer(id);
                        //var imageDisplays = newProducts ? Dealer_image_displaysDAL.GetForDealerNew(id) : Dealer_image_displaysDAL.GetForDealer(id);
                        var imageDisplays = Dealer_image_displaysDAL.GetForDealerNew(id);
                        var dispActivities = Dealer_displays_activityDAL.GetByCriteria(id);
                        if (imageDisplays != null)
                        {
                            foreach (var img in d.Dealer_Images)
                            {
                                img.Displays = imageDisplays.Where(imagedisp => imagedisp.image_id == img.image_unique).ToList();
                                foreach (var disp in img.Displays)
                                {
                                    var act =
                                        dispActivities.FirstOrDefault(
                                            a => a.web_unique == disp.web_unique && a.old_qty == null);
                                    if (act != null)
                                        disp.datecreated = act.datecreated;
                                }
                            }
                        }
                        d.BrandStatuses = GetStatusesFromTable(id, ref d);
                    }

				}
				dr.Close();
			    if (d != null)
			    {
			        d.Distributors = GetDistributors(d.user_id, conn);
			        d.DisplayRebates = Dealer_display_rebateDAL.GetByDealer(d.user_id, conn);
			    }
			}
			return d;
		}

		private static List<Company> GetDistributors(int user_id, MySqlConnection conn)
		{
			var result = new List<Company>();
			var cmd =
				Utils.GetCommand(
					"SELECT users.* FROM users INNER JOIN dealer_distributors ON users.user_id = dealer_distributors.distributor_id WHERE dealer_distributors.dealer_id = @user_id",conn);
			cmd.Parameters.AddWithValue("@user_id", user_id);
			var dr = cmd.ExecuteReader();
			while (dr.Read())
			{
				result.Add(CompanyDAL.GetFromDataReader(dr));
			}
			dr.Close();
			return result;
		}

		public static Dictionary<int, DealerBrandStatus> GetStatusesFromTable(int dealer_id, ref Dealer d)
		{
			var result = new Dictionary<int, DealerBrandStatus>();
			var statuses = new List<Dealer_brandstatus>();
			using (var conn = new MySqlConnection(Settings.Default.ConnString))
			{
				conn.Open();
				var cmd = Utils.GetCommand("SELECT * FROM dealer_brandstatus WHERE dealer_id = @dealer_id", conn);
				cmd.Parameters.AddWithValue("@dealer_id", dealer_id);
				var dr = cmd.ExecuteReader();
				while (dr.Read())
				{
					statuses.Add(new Dealer_brandstatus{brand_id = (int) dr["brand_id"],dealer_id = (int) dr["dealer_id"], brand_status = (int) dr["brand_status"]});
				}
				dr.Close();
				if (statuses.Count > 0)
				{
					d.DealerBrandstatuses = statuses;
					foreach (var dbs in statuses)
					{
						result[dbs.brand_id] = (DealerBrandStatus)dbs.brand_status;
					}
				}

			}
			return result;
		}

		private static List<UserImage> GetImages(Dealer d)
		{
			d.Dealer_Images = Dealer_imagesDAL.GetByDealer(d.user_id);
			return d.Dealer_Images
									  .Select(di => new UserImage { Id = di.image_unique, Name = di.dealer_image })
									  .ToList();
		}




		public static List<DealerBrandInfo> GetDealerBrandInfo()
		{
			var result = new List<DealerBrandInfo>();
			using (var conn = new MySqlConnection(Settings.Default.ConnString))
			{
				conn.Open();
				var cmd = Utils.GetCommand(@"select `dealers`.`user_id` AS `user_id`,`dealer_image_brand`.`brand_id` AS `brand_id`
										   from ((`dealers` join `dealer_images` on((`dealers`.`user_id` = `dealer_images`.`dealer_id`)))
											join `dealer_image_brand` on((`dealer_image_brand`.`dealer_image_id` = `dealer_images`.`image_unique`)))
											group by `dealers`.`user_id`,`dealer_image_brand`.`brand_id`", conn);
				var dr = cmd.ExecuteReader();
				while (dr.Read())
				{
					result.Add(new DealerBrandInfo{user_id = (int) dr["user_id"], brand_id = (int) dr["brand_id"]});
				}
			}
			return result;

		}

		public static void CreateReminderForBrochure(Dealer d, int brochureId, int days, int brandid)
		{
			using (var conn = new MySqlConnection(Settings.Default.ConnString))
			{
				conn.Open();
				MySqlCommand cmd = Utils.GetCommand(@"INSERT INTO auto_emails_dealers(autoe_dist, autoe_email, autoe_copy1, autoe_copy2, autoe_name, autoe_create_date,
													autoe_warning_date,autoe_step, autoe_ref, autoe_ref2)
													VALUES (@autoe_dist,@autoe_email, @autoe_copy1, @autoe_copy2,@autoe_name,@autoe_create_date,
													@autoe_warning_date,@autoe_step, @autoe_ref, @autoe_ref2)", conn);
				cmd.Parameters.Add(new MySqlParameter("@autoe_dist",brandid));
				cmd.Parameters.Add(new MySqlParameter("@autoe_email", d.user_email));
				cmd.Parameters.Add(new MySqlParameter("@autoe_copy1", ""));
				cmd.Parameters.Add(new MySqlParameter("@autoe_copy2", ""));
				cmd.Parameters.Add(new MySqlParameter("@autoe_name", d.user_name));
				cmd.Parameters.Add(new MySqlParameter("@autoe_create_date", DateTime.Now));
				cmd.Parameters.Add(new MySqlParameter("@autoe_warning_date", DateTime.Now.AddDays(days)));
				cmd.Parameters.Add(new MySqlParameter("@autoe_step", 1));
				cmd.Parameters.Add(new MySqlParameter("@autoe_ref", brochureId));
				cmd.Parameters.Add(new MySqlParameter("@autoe_ref2", ""));

				cmd.ExecuteNonQuery();
			}
		}

		public static void Create(Dealer o)
		{
			string insertsql = @"INSERT INTO dealers(user_id,user_name,user_account,customer_code,distributor,user_welcomename,user_address1,user_address2,
								user_address3,user_address4,user_address5,postcode,ie_region,user_country,user_contact,user_tel,user_fax,user_mobile,user_website,user_email,
								user_email2,user_type,user_access,user_pwd,user_created,user_curr,user_curr_pricing,dynamic_pricing,lastlogin,hide_1,gold_override,opening,directions1,
								directions2,directions3,directions4,image,longitude,latitude,opening1_from,opening1_to,opening2_from,opening2_to,opening3_from,opening3_to,opening4_from,opening4_to,
								opening5_from,opening5_to,opening6_from,opening6_to,opening7_from,opening7_to,image_policy,image_policy_acceptance,image_policy_ip,training,training_date,
								brand_b,brand_wc,confirmed,default_brand,sales_registered,user_modified,sqfeet,annual_turnover,created_by,sqfeetrange, annual_turnover_range,customer_type,
                                signoff_form,signoff_date,signoff_displaysets,action_flag,sales_registered_2015,digital)
								VALUES(@user_id,@user_name,@user_account,@customer_code,@distributor,@user_welcomename,@user_address1,@user_address2,@user_address3,
								@user_address4,@user_address5,@postcode,@ie_region,@user_country,@user_contact,@user_tel,@user_fax,@user_mobile,@user_website,@user_email,@user_email2,
								@user_type,@user_access,@user_pwd,@user_created,@user_curr,@user_curr_pricing,@dynamic_pricing,@lastlogin,@hide_1,@gold_override,@opening,@directions1,
								@directions2,@directions3,@directions4,@image,@longitude,@latitude,@opening1_from,@opening1_to,@opening2_from,@opening2_to,@opening3_from,@opening3_to,
								@opening4_from,@opening4_to,@opening5_from,@opening5_to,@opening6_from,@opening6_to,@opening7_from,@opening7_to,@image_policy,@image_policy_acceptance,
								@image_policy_ip,@training,@training_date,@brand_b,@brand_wc,@confirmed,@default_brand,@sales_registered,@user_modified,@sqfeet,@annual_turnover,@created_by,
								@sqfeetrange, @annual_turnover_range,@customer_type,@signoff_form,@signoff_date,@signoff_displaysets,@action_flag,@sales_registered_2015,@digital)";

			var conn = new MySqlConnection(Settings.Default.ConnString);
			conn.Open();
			MySqlTransaction tr = conn.BeginTransaction();
			try
			{
				MySqlCommand cmd = Utils.GetCommand("SELECT MAX(user_id)+1 FROM dealers", conn, tr);
				o.user_id = Convert.ToInt32(cmd.ExecuteScalar());

				cmd.CommandText = insertsql;
				BuildSqlParameters(cmd, o);
				cmd.ExecuteNonQuery();

				if (o.Dealer_Images != null)
				{
					foreach (var image in o.Dealer_Images)
					{
						image.dealer_id = o.user_id;
					    image.DateCreated = DateTime.Now;
						Dealer_imagesDAL.Create(image, tr);
					}
				}
			    if (o.DisplayRebates != null)
			    {
			        foreach (var dr in o.DisplayRebates.Where(r => r.unique_id == 0))
			        {
			            dr.dealer_id = o.user_id;
			            Dealer_display_rebateDAL.Create(dr, tr);
			        }
			    }
			    UpdateDistributors(o, cmd);
				tr.Commit();
			}
			catch
			{
				tr.Rollback();
				throw;
			}
			finally
			{
				conn = null;
			}
		}


		private static void BuildSqlParameters(MySqlCommand cmd, Dealer o)
		{
			cmd.Parameters.AddWithValue("@user_id", o.user_id);
			cmd.Parameters.AddWithValue("@user_name", o.user_name);
			cmd.Parameters.AddWithValue("@user_account", o.user_account);
			cmd.Parameters.AddWithValue("@customer_code", o.customer_code);
			cmd.Parameters.AddWithValue("@distributor", o.distributor);
			cmd.Parameters.AddWithValue("@user_welcomename", o.user_welcomename);
			cmd.Parameters.AddWithValue("@user_address1", o.user_address1);
			cmd.Parameters.AddWithValue("@user_address2", o.user_address2);
			cmd.Parameters.AddWithValue("@user_address3", o.user_address3);
			cmd.Parameters.AddWithValue("@user_address4", o.user_address4);
			cmd.Parameters.AddWithValue("@user_address5", o.user_address5);
			cmd.Parameters.AddWithValue("@postcode", o.postcode);
			cmd.Parameters.AddWithValue("@ie_region", o.ie_region);
			cmd.Parameters.AddWithValue("@user_country", o.user_country);
			cmd.Parameters.AddWithValue("@user_contact", o.user_contact);
			cmd.Parameters.AddWithValue("@user_tel", o.user_tel);
			cmd.Parameters.AddWithValue("@user_fax", o.user_fax);
			cmd.Parameters.AddWithValue("@user_mobile", o.user_mobile);
			cmd.Parameters.AddWithValue("@user_website", o.user_website);
			cmd.Parameters.AddWithValue("@user_email", o.user_email);
			cmd.Parameters.AddWithValue("@user_email2", o.user_email2);
			cmd.Parameters.AddWithValue("@user_type", o.user_type);
			cmd.Parameters.AddWithValue("@user_access", o.user_access);
			cmd.Parameters.AddWithValue("@user_pwd", o.user_pwd);
			cmd.Parameters.AddWithValue("@user_created", o.user_created);
			cmd.Parameters.AddWithValue("@user_curr", o.user_curr);
			cmd.Parameters.AddWithValue("@user_curr_pricing", o.user_curr_pricing);
			cmd.Parameters.AddWithValue("@dynamic_pricing", o.dynamic_pricing);
			cmd.Parameters.AddWithValue("@lastlogin", o.lastlogin);
			cmd.Parameters.AddWithValue("@hide_1", o.hide_1);
			cmd.Parameters.AddWithValue("@gold_override", o.gold_override);
			cmd.Parameters.AddWithValue("@opening", o.opening);
			cmd.Parameters.AddWithValue("@directions1", o.directions1);
			cmd.Parameters.AddWithValue("@directions2", o.directions2);
			cmd.Parameters.AddWithValue("@directions3", o.directions3);
			cmd.Parameters.AddWithValue("@directions4", o.directions4);
			cmd.Parameters.AddWithValue("@image", o.image);
			cmd.Parameters.AddWithValue("@longitude", o.longitude);
			cmd.Parameters.AddWithValue("@latitude", o.latitude);
			cmd.Parameters.AddWithValue("@opening1_from", o.opening1_from);
			cmd.Parameters.AddWithValue("@opening1_to", o.opening1_to);
			cmd.Parameters.AddWithValue("@opening2_from", o.opening2_from);
			cmd.Parameters.AddWithValue("@opening2_to", o.opening2_to);
			cmd.Parameters.AddWithValue("@opening3_from", o.opening3_from);
			cmd.Parameters.AddWithValue("@opening3_to", o.opening3_to);
			cmd.Parameters.AddWithValue("@opening4_from", o.opening4_from);
			cmd.Parameters.AddWithValue("@opening4_to", o.opening4_to);
			cmd.Parameters.AddWithValue("@opening5_from", o.opening5_from);
			cmd.Parameters.AddWithValue("@opening5_to", o.opening5_to);
			cmd.Parameters.AddWithValue("@opening6_from", o.opening6_from);
			cmd.Parameters.AddWithValue("@opening6_to", o.opening6_to);
			cmd.Parameters.AddWithValue("@opening7_from", o.opening7_from);
			cmd.Parameters.AddWithValue("@opening7_to", o.opening7_to);
			cmd.Parameters.AddWithValue("@image_policy", o.image_policy);
			cmd.Parameters.AddWithValue("@image_policy_acceptance", o.image_policy_acceptance);
			cmd.Parameters.AddWithValue("@image_policy_ip", o.image_policy_ip);
			cmd.Parameters.AddWithValue("@training", o.training);
			cmd.Parameters.AddWithValue("@training_date", o.training_date);
			cmd.Parameters.AddWithValue("@brand_b", o.brand_b);
			cmd.Parameters.AddWithValue("@brand_wc", o.brand_wc);
			cmd.Parameters.AddWithValue("@confirmed", o.confirmed);
			cmd.Parameters.AddWithValue("@default_brand", o.default_brand);
			cmd.Parameters.AddWithValue("@sales_registered", o.sales_registered);
            cmd.Parameters.AddWithValue("@sales_registered_2014", o.sales_registered_2014);
            cmd.Parameters.AddWithValue("@sales_registered_2015", o.sales_registered_2015);
            cmd.Parameters.AddWithValue("@sales_registered_2016", o.sales_registered_2016);
            cmd.Parameters.AddWithValue("@sales_registered_2017", o.sales_registered_2017);
            cmd.Parameters.AddWithValue("@user_modified", o.user_modified);
			cmd.Parameters.AddWithValue("@sqfeet", o.SqFeet);
			cmd.Parameters.AddWithValue("@annual_turnover", o.AnnualTurnover);
			cmd.Parameters.AddWithValue("@sqfeetrange", o.SqFeetRange);
			cmd.Parameters.AddWithValue("@annual_turnover_range", o.AnnualTurnoverRange);
			cmd.Parameters.AddWithValue("@created_by", o.created_by);
			cmd.Parameters.AddWithValue("@modified_by", o.modified_by);
			cmd.Parameters.AddWithValue("@customer_type", o.customer_type);
            cmd.Parameters.AddWithValue("@action_flag", o.action_flag);

		    cmd.Parameters.AddWithValue("@signoff_form", o.signoff_form);
		    cmd.Parameters.AddWithValue("@signoff_date", o.signoff_date);
		    cmd.Parameters.AddWithValue("@signoff_displaysets", o.signoff_displaysets);
            cmd.Parameters.AddWithValue("@digital", o.digital?1:0);
            cmd.Parameters.AddWithValue("@sequence", o.sequence);
        }

        public static void Update(Dealer d, List<int> deletedImagesIds=null, List<int> deletedProducts=null)
		{
			string updatesql = @"UPDATE dealers SET user_name = @user_name,user_account = @user_account,customer_code = @customer_code,distributor = @distributor,
								user_welcomename = @user_welcomename,user_address1 = @user_address1,user_address2 = @user_address2,user_address3 = @user_address3,
								user_address4 = @user_address4,user_address5 = @user_address5,postcode = @postcode,ie_region = @ie_region,user_country = @user_country,
								user_contact = @user_contact,user_tel = @user_tel,user_fax = @user_fax,user_mobile = @user_mobile,user_website = @user_website,user_email = @user_email,
								user_email2 = @user_email2,user_type = @user_type,user_access = @user_access,user_pwd = @user_pwd,user_created = @user_created,user_curr = @user_curr,
								user_curr_pricing = @user_curr_pricing,dynamic_pricing = @dynamic_pricing,lastlogin = @lastlogin,hide_1 = @hide_1,gold_override = @gold_override,
								opening = @opening,directions1 = @directions1,directions2 = @directions2,directions3 = @directions3,directions4 = @directions4,image = @image,
								longitude = @longitude,latitude = @latitude,opening1_from = @opening1_from,opening1_to = @opening1_to,opening2_from = @opening2_from,
								opening2_to = @opening2_to,opening3_from = @opening3_from,opening3_to = @opening3_to,opening4_from = @opening4_from,opening4_to = @opening4_to,
								opening5_from = @opening5_from,opening5_to = @opening5_to,opening6_from = @opening6_from,opening6_to = @opening6_to,opening7_from = @opening7_from,
								opening7_to = @opening7_to,image_policy = @image_policy,image_policy_acceptance = @image_policy_acceptance,image_policy_ip = @image_policy_ip,
								training = @training,training_date = @training_date,brand_b = @brand_b,brand_wc = @brand_wc, confirmed = @confirmed,default_brand = @default_brand,
								sales_registered = @sales_registered,sales_registered_2014 = @sales_registered_2014,user_modified = @user_modified,sqfeet = @sqfeet,
                                annual_turnover = @annual_turnover,modified_by = @modified_by,sqfeetrange = @sqfeetrange , annual_turnover_range = @annual_turnover_range,
                                customer_type = @customer_type, signoff_form = @signoff_form, signoff_date = @signoff_date, signoff_displaysets=@signoff_displaysets, action_flag=@action_flag,
                                sales_registered_2015=@sales_registered_2015,sales_registered_2016=@sales_registered_2016,sales_registered_2017=@sales_registered_2017,digital=@digital,sequence=@sequence
								WHERE user_id = @user_id";

			var conn = new MySqlConnection(Settings.Default.ConnString);
			conn.Open();
			MySqlTransaction tr = conn.BeginTransaction();
			try
			{
				d.user_modified = DateTime.Now;

				var cmd = Utils.GetCommand(updatesql, conn, tr);
				BuildSqlParameters(cmd, d);
				cmd.ExecuteNonQuery();

				if (d.Dealer_Images != null)
				{
					foreach (var image in d.Dealer_Images)
					{
						if (image.image_unique <= 0)
						{
							image.dealer_id = d.user_id;
						    image.DateCreated = DateTime.Now;
                            Dealer_imagesDAL.Create(image, tr);
						}
						else
						{
							Dealer_imagesDAL.Update(image, tr);
						}
					}
				}
				if (deletedImagesIds != null)
				{
					foreach (var di in deletedImagesIds)
					{
						Dealer_imagesDAL.Delete(di, tr);
					}
				}
                foreach (var dr in d.DisplayRebates.Where(r => r.unique_id == 0))
                {
                    dr.dealer_id = d.user_id;
                    Dealer_display_rebateDAL.Create(dr, tr);
                }
				UpdateDistributors(d,cmd);

				//if (d.DisplayedProducts != null)
				//{

				//    cmd.Parameters.Clear();
				//    cmd.Parameters.AddWithValue("@dealer_id", d.user_id);
				//    cmd.Parameters.AddWithValue("@web_unique", 0);
				//    cmd.Parameters.AddWithValue("@qty", 0);
				//    foreach (var dp in d.DisplayedProducts)
				//    {
				//        cmd.Parameters[1].Value = dp.Product.web_unique;
				//        cmd.Parameters[2].Value = dp.qty;

				//        if (dp.unique_id == 0)
				//        {
				//            cmd.CommandText = "INSERT INTO dealer_displays(client_id, web_unique, qty) VALUES(@dealer_id, @web_unique,@qty)";
				//        }
				//        else
				//        {
				//            cmd.CommandText = "UPDATE dealer_displays SET qty = @qty WHERE web_unique = @web_unique AND client_id = @dealer_id";
				//        }
				//        cmd.ExecuteNonQuery();
				//    }
				//}
				if (deletedProducts != null)
				{
					cmd.Parameters.Clear();
					cmd.Parameters.AddWithValue("@dealer_id", d.user_id);
					cmd.CommandText = String.Format("DELETE FROM dealer_displays WHERE client_id = @dealer_id AND web_unique IN ({0})",Utils.CreateParametersFromIdList(cmd, deletedProducts));
					cmd.ExecuteNonQuery();
				}

				if (d.BrandStatuses != null)
				{
					cmd.Parameters.Clear();
					cmd.Parameters.AddWithValue("@dealer_id", d.user_id);
					cmd.Parameters.AddWithValue("@brand_id", 0);
					cmd.Parameters.AddWithValue("@brandstatus", 0);

					cmd.CommandText = "DELETE FROM dealer_brandstatus WHERE dealer_id = @dealer_id";
					cmd.ExecuteNonQuery();

					cmd.CommandText = "INSERT INTO dealer_brandstatus (dealer_id, brand_id,brand_status) VALUES(@dealer_id, @brand_id, @brandstatus)";

					foreach (var kv in d.BrandStatuses)
					{
						cmd.Parameters["@brand_id"].Value = kv.Key;
						cmd.Parameters["@brandstatus"].Value = kv.Value;
						cmd.ExecuteNonQuery();
					}
				}

				tr.Commit();
			}
			catch
			{
				tr.Rollback();
				throw;
			}
			finally
			{
				conn = null;
			}
		}

		private static void UpdateDistributors(Dealer d,MySqlCommand cmd)
		{
			if (d.Distributors != null)
			{
				cmd.Parameters.Clear();
				cmd.Parameters.AddWithValue("@user_id", d.user_id);
				cmd.CommandText = "DELETE FROM dealer_distributors WHERE dealer_id = @user_id";
				cmd.ExecuteNonQuery();
				cmd.CommandText = "INSERT INTO dealer_distributors VALUES (@user_id,@distributor_id)";
				cmd.Parameters.AddWithValue("@distributor_id", 0);
				foreach (var dist in d.Distributors)
				{
					cmd.Parameters[1].Value = dist.user_id;
					cmd.ExecuteNonQuery();
				}

			}
		}

		public static void UpdateImages(Dealer d)
		{
			string deletesql = "DELETE FROM dealer_images WHERE dealer_id = @dealer_id";
			string insertsql = @"INSERT INTO dealer_images (dealer_id, dealer_image, seq, hide) VALUES(@dealer_id, @dealer_image, 0,0)";
			MySqlConnection conn;
			MySqlTransaction tr = null;
			try
			{
				conn = new MySqlConnection(Settings.Default.ConnString);
				conn.Open();
				tr = conn.BeginTransaction();
				MySqlCommand cmd = Utils.GetCommand(deletesql);
				cmd.Parameters.AddWithValue("@dealer_id", d.user_id);
				cmd.ExecuteNonQuery();

				cmd.Parameters.AddWithValue("@dealer_image", "");
				cmd.CommandText = insertsql;
				foreach (var img in d.Images)
				{
					cmd.Parameters[1].Value = img.Name;
					cmd.ExecuteNonQuery();
				}
				tr.Commit();
			}
			catch (Exception)
			{
				if(tr != null)
					tr.Rollback();
			}
			finally
			{
				conn = null;
			}
		}

		public static void DeleteImage(int image_id)
		{
			using (var conn = new MySqlConnection(Settings.Default.ConnString))
			{
				conn.Open();
				var cmd = Utils.GetCommand("DELETE FROM dealer_images WHERE image_unique = @id", conn);
				cmd.Parameters.AddWithValue("@id", image_id);
				cmd.ExecuteNonQuery();
			}
		}

		public static void InsertImage(int dealer_id, string image_name)
		{
			using (var conn = new MySqlConnection(Settings.Default.ConnString))
			{
				conn.Open();
				var cmd = Utils.GetCommand("INSERT INTO dealer_images (dealer_id, dealer_image, seq, hide) VALUES(@dealer_id, @dealer_image, 0,0)", conn);
				cmd.Parameters.AddWithValue("@dealer_id", dealer_id);
				cmd.Parameters.AddWithValue("@dealer_image", image_name);
				cmd.ExecuteNonQuery();
			}
		}

		/// <summary>
		/// Adds or removes product from list of dealer products
		/// </summary>
		/// <param name="dealer_id"></param>
		/// <param name="product_id"></param>
		/// <param name="add"></param>
		public static void AddOrRemoveProduct(int dealer_id, int product_id, bool add)
		{
			using (var conn = new MySqlConnection(Settings.Default.ConnString))
			{
				conn.Open();
				string commandText;
				if (add)
					commandText = "INSERT INTO dealer_displays(client_id, web_unique, qty) VALUES(@dealer_id, @product_id,1)";
				else
					commandText = "DELETE FROM dealer_displays WHERE client_id = @dealer_id AND web_unique = @product_id";
				MySqlCommand cmd = Utils.GetCommand(commandText, conn);
				cmd.Parameters.AddWithValue("@dealer_id", dealer_id);
				cmd.Parameters.AddWithValue("@product_id", product_id);
				cmd.ExecuteNonQuery();
			}
		}

        public static Dot GetDotFromReader(MySqlDataReader dr)
        {
            Dot d = new Dot();
            d.id = (int)dr["user_id"];
            d.name = String.Empty + dr["user_name"];
            d.distributor = String.Empty + dr["distributor"];
            d.postcode=String.Empty+ dr["postcode"];
            //d.user_country = String.Empty + dr["postcode"];
            d.longitude = Utilities.FromDbValue<double>(dr["longitude"]);
            d.latitude = Utilities.FromDbValue<double>(dr["latitude"]);
            d.user_address1 = String.Empty + dr["user_address1"];
            d.user_address2 = String.Empty + dr["user_address2"];
            d.user_address3 = String.Empty + dr["user_address3"];
            d.user_address4 = String.Empty + dr["user_address4"];
            d.user_address5 = String.Empty + dr["user_address5"];
            try
            {
                d.country = String.Empty + dr["CountryName"];
            }
            catch (Exception)
            {


            }

            return d;
        }

		public  static Dealer GetDealerFromReader(MySqlDataReader dr)
		{
			Dealer d = new Dealer();
			d.user_id = (int) dr["user_id"];
			d.user_name = String.Empty + dr["user_name"];
			d.user_account = String.Empty + dr["user_account"];
			d.customer_code = String.Empty + dr["customer_code"];
			d.distributor = Utilities.FromDbValue<int>(dr["distributor"]);
			d.user_welcomename = String.Empty + dr["user_welcomename"];
			d.user_address1 = String.Empty + dr["user_address1"];
			d.user_address2 = String.Empty + dr["user_address2"];
			d.user_address3 = String.Empty + dr["user_address3"];
			d.user_address4 = String.Empty + dr["user_address4"];
			d.user_address5 = String.Empty + dr["user_address5"];
			d.postcode = String.Empty + dr["postcode"];
			d.ie_region = Utilities.FromDbValue<int>(dr["ie_region"]);
			d.user_country = String.Empty + dr["user_country"];
			d.user_contact = String.Empty + dr["user_contact"];
			d.user_tel = String.Empty + dr["user_tel"];
			d.user_fax = String.Empty + dr["user_fax"];
			d.user_mobile = String.Empty + dr["user_mobile"];
			d.user_website = String.Empty + dr["user_website"];
			d.user_email = String.Empty + dr["user_email"];
			d.user_email2 = String.Empty + dr["user_email2"];
			d.user_type = Utilities.FromDbValue<int>(dr["user_type"]);
			d.user_access = Utilities.FromDbValue<int>(dr["user_access"]);
			d.user_pwd = String.Empty + dr["user_pwd"];
			d.user_created = Utilities.FromDbValue<DateTime>(dr["user_created"]);
			d.user_curr = Utilities.FromDbValue<int>(dr["user_curr"]);
			d.user_curr_pricing = Utilities.FromDbValue<int>(dr["user_curr_pricing"]);
			d.dynamic_pricing = Utilities.FromDbValue<int>(dr["dynamic_pricing"]);
			d.lastlogin = Utilities.FromDbValue<DateTime>(dr["lastlogin"]);
			d.hide_1 = Utilities.FromDbValue<int>(dr["hide_1"]);
			d.gold_override = Utilities.FromDbValue<int>(dr["gold_override"]);
			d.opening = String.Empty + dr["opening"];
			d.directions1 = String.Empty + dr["directions1"];
			d.directions2 = String.Empty + dr["directions2"];
			d.directions3 = String.Empty + dr["directions3"];
			d.directions4 = String.Empty + dr["directions4"];
			d.image = String.Empty + dr["image"];
			d.longitude = Utilities.FromDbValue<double>(dr["longitude"]);
			d.latitude = Utilities.FromDbValue<double>(dr["latitude"]);
			d.opening1_from = String.Empty + dr["opening1_from"];
			d.opening1_to = String.Empty + dr["opening1_to"];
			d.opening2_from = String.Empty + dr["opening2_from"];
			d.opening2_to = String.Empty + dr["opening2_to"];
			d.opening3_from = String.Empty + dr["opening3_from"];
			d.opening3_to = String.Empty + dr["opening3_to"];
			d.opening4_from = String.Empty + dr["opening4_from"];
			d.opening4_to = String.Empty + dr["opening4_to"];
			d.opening5_from = String.Empty + dr["opening5_from"];
			d.opening5_to = String.Empty + dr["opening5_to"];
			d.opening6_from = String.Empty + dr["opening6_from"];
			d.opening6_to = String.Empty + dr["opening6_to"];
			d.opening7_from = String.Empty + dr["opening7_from"];
			d.opening7_to = String.Empty + dr["opening7_to"];
			d.image_policy = Utilities.FromDbValue<int>(dr["image_policy"]);
			d.image_policy_acceptance = Utilities.FromDbValue<DateTime>(dr["image_policy_acceptance"]);
			d.image_policy_ip = String.Empty + dr["image_policy_ip"];
			d.training = Utilities.FromDbValue<int>(dr["training"]);
			d.training_date = Utilities.FromDbValue<DateTime>(dr["training_date"]);
			d.brand_b = Utilities.FromDbValue<int>(dr["brand_b"]);
			d.created_by = Utilities.FromDbValue<int>(dr["created_by"]);
			d.modified_by = Utilities.FromDbValue<int>(dr["modified_by"]);
            if (Utilities.ColumnExists(dr, "action_flag"))
                d.action_flag = Utilities.FromDbValue<int>(dr["action_flag"]);
		    if (Utilities.ColumnExists(dr,"signoff_form"))
		    {
                d.signoff_form = String.Empty + dr["signoff_form"];
                d.signoff_date = Utilities.FromDbValue<DateTime>(dr["signoff_date"]);
                d.signoff_displaysets = Utilities.FromDbValue<int>(dr["signoff_displaysets"]);
		    }

			if(Utilities.ColumnExists(dr,"user_modified"))
				d.user_modified = Utilities.FromDbValue<DateTime>(dr["user_modified"]);

			if(Utilities.ColumnExists(dr,"default_brand"))
				d.default_brand = Utilities.FromDbValue<int>(dr["default_brand"]);
			if(Utilities.ColumnExists(dr,"confirmed"))
				d.confirmed = Utilities.FromDbValue<int>(dr["confirmed"]);
			if(Utilities.ColumnExists(dr, "brand_wc"))
				d.brand_wc = Utilities.FromDbValue<int>(dr["brand_wc"]);
			if (Utilities.ColumnExists(dr, "dist_name"))
				d.DistributorName = String.Empty + dr["dist_name"];
			if (Utilities.ColumnExists(dr, "dist_code"))
				d.DistributorCode = String.Empty + dr["dist_code"];
			if (Utilities.ColumnExists(dr, "dist_email"))
				d.DistributorEmail = String.Empty + dr["dist_email"];

			if(Utilities.ColumnExists(dr, "gold"))
				d.gold = Utilities.BoolFromLong(dr["gold"]);
			if(Utilities.ColumnExists(dr, "silver"))
				d.silver = Utilities.BoolFromLong(dr["silver"]);
			if (Utilities.ColumnExists(dr, "brand_status"))
			{
				var value = dr["brand_status"];
				if(value != null && value != DBNull.Value)
					d.brand_status = (DealerBrandStatus) Convert.ToInt32(dr["brand_status"]);
			}
			if (Utilities.ColumnExists(dr, "numImages"))
				d.numOfImages = Convert.ToInt32(dr["numImages"]);
			if (Utilities.ColumnExists(dr, "numDisplays"))
				d.numOfDisplays = Convert.ToInt32(dr["numDisplays"]);
			if(Utilities.ColumnExists(dr,"sales_registered"))
				d.sales_registered = Convert.ToBoolean(dr["sales_registered"]);
			if (Utilities.ColumnExists(dr, "brand_status_manual"))
				d.brand_status_manual = (DealerBrandStatus?) Utilities.FromDbValue<int>(dr["brand_status_manual"]);
			if (Utilities.ColumnExists(dr, "brand_code"))
			{
				d.brand_code = String.Empty + dr["brand_code"];
			}
			if(Utilities.ColumnExists(dr,"sqfeet"))
				d.SqFeet = Utilities.FromDbValue<double>(dr["sqfeet"]);
			if(Utilities.ColumnExists(dr,"annual_turnover"))
				d.AnnualTurnover = Utilities.FromDbValue<double>(dr["annual_turnover"]);
			if (Utilities.ColumnExists(dr, "sqfeetrange"))
				d.SqFeetRange = String.Empty + dr["sqfeetrange"];
			if (Utilities.ColumnExists(dr, "annual_turnover_range"))
				d.AnnualTurnoverRange = String.Empty + dr["annual_turnover_range"];
			if(Utilities.ColumnExists(dr,"cw_code"))
				d.cw_code = String.Empty + dr["cw_code"];
			if (Utilities.ColumnExists(dr, "customer_type"))
				d.customer_type = Utilities.FromDbValue<int>(dr["customer_type"]);
            if (Utilities.ColumnExists(dr, "sales_registered_2014"))
		        d.sales_registered_2014 = Utilities.FromDbValue<int>(dr["sales_registered_2014"]);
		    if (Utilities.ColumnExists(dr, "sales_registered_2015"))
		    {
		        var temp = Utilities.FromDbValue<int>(dr["sales_registered_2015"]);
                if(temp == null)
                    d.sales_registered_2015 = false;
                else
                {
                    d.sales_registered_2015 = Convert.ToBoolean(temp);
                }
		    }
            if (Utilities.ColumnExists(dr, "sales_registered_2016"))
            {
                var temp = Utilities.FromDbValue<int>(dr["sales_registered_2016"]);
                if (temp == null)
                    d.sales_registered_2016 = false;
                else
                {
                    d.sales_registered_2016 = Convert.ToBoolean(temp);
                }
            }
            if (Utilities.ColumnExists(dr, "sales_registered_2017"))
            {
                var temp = Utilities.FromDbValue<int>(dr["sales_registered_2017"]);
                if (temp == null)
                    d.sales_registered_2017 = false;
                else
                {
                    d.sales_registered_2017 = Convert.ToBoolean(temp); 
                }
            }
            if (Utilities.ColumnExists(dr, "survey_id"))
		        d.survey_id = string.Empty + dr["survey_id"];

            if (Utilities.ColumnExists(dr, "digital"))
            {
                var tempDigital = Utilities.FromDbValue<int>(dr["digital"]);
                if (tempDigital == null)
                    d.digital = false;
                else
                {
                    d.digital = Convert.ToBoolean(tempDigital);
                }
            }
            if (Utilities.ColumnExists(dr, "sequence"))
                d.sequence = Utilities.FromDbValue<int>(dr["sequence"]);
            return d;

		}



		public static List<Dealer> GetDealersByDistributor(int distributor_id, string brand_code = "")
		{
			var result = new List<Dealer>();
			Brand brand = null;
			if (!String.IsNullOrEmpty(brand_code))
				brand = BrandsDAL.GetByCode(brand_code);
			using (var conn = new MySqlConnection(Settings.Default.ConnString))
			{
				conn.Open();

				string sql =
					@"SELECT dealers.*, (SELECT COUNT(*) FROM dealer_images WHERE dealer_id = dealers.user_id) AS numImages,
													  (SELECT COUNT(*) FROM dealer_image_displays INNER JOIN dealer_images ON dealer_image_displays.image_id = dealer_images.image_unique
													  WHERE dealer_images.dealer_id = dealers.user_id) AS numDisplays FROM dealers
													WHERE user_type = @user_type OR EXISTS (SELECT * FROM dealer_distributors WHERE dealer_distributors.dealer_id = dealers.user_id AND dealer_distributors.distributor_id = @user_type)";
				var cmd = Utils.GetCommand(sql, conn);
				if (!String.IsNullOrEmpty(brand_code))
				{
					cmd.CommandText =
						@"SELECT dealers.*, (SELECT COUNT(*) FROM dealer_images WHERE dealer_id = dealers.user_id) AS numImages,
													  (SELECT COUNT(*) FROM dealer_image_displays INNER JOIN dealer_images ON dealer_image_displays.image_id = dealer_images.image_unique
													  WHERE dealer_images.dealer_id = dealers.user_id) AS numDisplays
							FROM dealers WHERE (user_type = @user_type OR EXISTS (SELECT * FROM dealer_distributors WHERE dealer_distributors.dealer_id = dealers.user_id AND dealer_distributors.distributor_id = @user_type))
					AND (EXISTS(SELECT dealer_image_displays.web_unique FROM dealer_image_displays INNER JOIN dealer_images ON dealer_image_displays.image_id = dealer_images.image_unique
								INNER JOIN web_product_new ON dealer_image_displays.web_unique = web_product_new.web_unique INNER JOIN
								web_site ON web_product_new.web_site_id = web_site.id
								WHERE web_site.brand_id = @brand_id AND dealer_images.dealer_id = dealers.user_id) OR
						  default_brand = @brand_id   OR
							EXISTS (SELECT dealer_id FROM dealer_brandstatus WHERE dealer_id = dealers.user_id AND brand_id = @brand_id)
							)";
					cmd.Parameters.AddWithValue("@code", brand.code);
					cmd.Parameters.AddWithValue("@brand_id", brand.brand_id);
				}

				cmd.Parameters.AddWithValue("@user_type", distributor_id);
				MySqlDataReader dr = cmd.ExecuteReader();
				while (dr.Read())
				{
					result.Add(GetDealerFromReader(dr));

				}
				dr.Close();
			}
			return result;
		}

		/// <summary>
		/// returns dealers from other distributor that have specified brand code
		/// </summary>
		/// <param name="distributor_id"></param>
		/// <param name="brand_code"></param>
		/// <returns></returns>
		public static List<Dealer> GetNonOwnedDealersByBrandCode(int distributor_id, string brand_code = "")
		{
			Brand b = BrandsDAL.GetByCode(brand_code);
			var result = new List<Dealer>();
			using (var conn = new MySqlConnection(Settings.Default.ConnString))
			{
				conn.Open();
				var cmd = Utils.GetCommand(@"SELECT dealers.*, (SELECT COUNT(*) FROM dealer_images WHERE dealer_id = dealers.user_id) AS numImages,
													  (SELECT COUNT(*) FROM dealer_displays WHERE client_id = dealers.user_id) AS numDisplays
											FROM dealers WHERE user_type <> @user_type
									AND (EXISTS(SELECT unique_id FROM dealer_displays INNER JOIN web_products ON dealer_displays.web_unique = web_products.web_unique
												WHERE web_products.web_site = @code AND dealer_displays.client_id = dealers.user_id) OR
										  default_brand = @brand_id)",conn);

				cmd.Parameters.AddWithValue("@user_type", distributor_id);
				cmd.Parameters.AddWithValue("@code", brand_code);
				cmd.Parameters.AddWithValue("@brand_id", b.brand_id);
				var dr = cmd.ExecuteReader();
				while (dr.Read())
				{
					result.Add(GetDealerFromReader(dr));

				}
				dr.Close();
			}
			return result;
		}

		public static List<Dealer> GetPendingDealers(int? user_type = null)
		{
			List<Dealer> result = new List<Dealer>();
			using (var conn = new MySqlConnection(Settings.Default.ConnString))
			{
				conn.Open();
				var cmd = Utils.GetCommand(@"SELECT dealers.*, users.customer_code AS dist_code,
												(SELECT COUNT(*) FROM dealer_images WHERE dealer_id = dealers.user_id) AS numImages
											FROM dealers INNER JOIN users ON dealers.user_type = users.user_id
										WHERE ( COALESCE(dealers.hide_1,0) = 0 OR COALESCE(dealers.confirmed,0) = 0) AND dealers.hide_1 <> 2 AND (dealers.user_type = @user_type OR @user_type IS NULL)", conn);
				cmd.Parameters.AddWithValue("@user_type", user_type != null ? (object) user_type : DBNull.Value);
				MySqlDataReader dr = cmd.ExecuteReader();
				while (dr.Read())
				{
					result.Add(GetDealerFromReader(dr));
				}
				dr.Close();
			}
			return result;
		}

		public static List<Dealer> GetDealersProcessedInMonth(DateTime month, int status)
		{
			List<Dealer> result = new List<Dealer>();
			using (var conn = new MySqlConnection(Settings.Default.ConnString))
			{
				conn.Open();
				var from = Utilities.GetMonthStart(month);
				var to = Utilities.GetMonthEnd(month);
				var cmd = Utils.GetCommand(@"SELECT dealers.*, users.customer_code AS dist_code,
												(SELECT COUNT(*) FROM dealer_images WHERE dealer_id = dealers.user_id) AS numImages
											FROM dealers INNER JOIN users ON dealers.user_type = users.user_id
											WHERE ( COALESCE(dealers.hide_1,0) = @status) AND dealers.confirmed = 1 AND dealers.hide_1 <> 2
                                                    AND (dealers.user_created BETWEEN @from AND @to OR dealers.user_modified BETWEEN @from AND @to)", conn);
				cmd.Parameters.AddWithValue("@from", @from);
				cmd.Parameters.AddWithValue("@to", to);
				cmd.Parameters.AddWithValue("@status", status);
				var dr = cmd.ExecuteReader();
				while (dr.Read())
				{
					result.Add(GetDealerFromReader(dr));

				}
				dr.Close();
			}
			return result;
		}

		public static List<Dealer> GetDealersByCriteria(string searchText, bool includeCancelled = false, bool liveOnly = false)
		{
			List<Dealer> result = new List<Dealer>();
			using (var conn = new MySqlConnection(Settings.Default.ConnString))
			{
				conn.Open();
				MySqlCommand cmd = Utils.GetCommand(string.Format(@"SELECT dealers.* FROM  dealers WHERE
                        (user_name LIKE @criteria OR user_email LIKE @criteria OR user_email2 LIKE @criteria OR postcode LIKE @criteria) {0} {1} ",
                        !includeCancelled ? "AND COALESCE(hide_1,0) <> 2" : "", liveOnly ? " AND COALESCE(hide_1,0) = 1" : ""), conn);
				//cmd.Parameters.AddWithValue("@user_type", distributor_id);
				cmd.Parameters.AddWithValue("@criteria", "%" + searchText + "%");
				MySqlDataReader dr = cmd.ExecuteReader();
				while (dr.Read())
				{
					result.Add(GetDealerFromReader(dr));

				}
				dr.Close();
			}
			return result;
		}

		public static List<Dealer> GetDealersByPostcode(string postcode, int? user_type = null)
		{
			List<Dealer> result = new List<Dealer>();
			using (var conn = new MySqlConnection(Settings.Default.ConnString))
			{
				conn.Open();
				MySqlCommand cmd = Utils.GetCommand(@"SELECT dealers.* FROM  dealers WHERE postcode = @postcode AND (user_type = @user_type OR @user_type IS NULL)", conn);
				//cmd.Parameters.AddWithValue("@user_type", distributor_id);
				cmd.Parameters.AddWithValue("@postcode", postcode);
				cmd.Parameters.AddWithValue("@user_type", user_type != null ? (object) user_type : DBNull.Value);
				MySqlDataReader dr = cmd.ExecuteReader();
				while (dr.Read())
				{
					result.Add(GetDealerFromReader(dr));

				}
				dr.Close();
			}
			return result;
		}



//        public static List<Dealer_displays> GetDisplaysForDealer(int dealer_id)
//        {
//            List<Dealer_displays> result = new List<Dealer_displays>();
//            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
//            {
//                string sql = @"SELECT web_products2.*, dealer_displays.qty,dealer_displays.unique_id, dealers.*, brand_categories_sub.seq AS sub_seq FROM web_products2 INNER JOIN dealer_displays ON web_products2.web_unique = dealer_displays.web_unique
//                            INNER JOIN dealers ON dealer_displays.client_id  = dealers.user_id
//                            LEFT OUTER JOIN brand_categories_sub ON web_products2.web_sub_category = brand_categories_sub.brand_sub_id
//                            WHERE  dealer_displays.client_id = @dealer_id AND COALESCE(web_status,0) <> 2 ORDER BY web_seq2, sub_seq,brand_sub_desc";
//                conn.Open();
//                MySqlCommand cmd = Utils.GetCommand(sql, conn);
//                cmd.Parameters.Add(new MySqlParameter("@dealer_id", dealer_id));

//                MySqlDataReader dr = cmd.ExecuteReader();
//                while (dr.Read())
//                {
//                    Dealer_displays dd = new Dealer_displays();
//                    dd.Product = WebProductsDAL.GetProductFromReader(dr);
//                    dd.Product.Components = WebProductsDAL.GetComponents(dd.Product, dr, true);
//                    dd.Dealer = DealerDAL.GetDealerFromReader(dr);
//                    dd.qty = Utilities.FromDbValue<int>(dr["qty"]);
//                    dd.unique_id = (int)dr["unique_id"];
//                    dd.web_unique = dd.Product.web_unique;
//                    dd.client_id = dd.Dealer.user_id;
//                    result.Add(dd);
//                }
//                dr.Close();
//            }
//            return result;
//        }

		public static List<Dealer_brandstatus> GetDealerStatuses(int id,IList<Brand> brands )
		{
			var d = GetById(id);
			var result = new List<Dealer_brandstatus>();
			result.AddRange(d.DealerBrandstatuses);
			//find brands for which status is not manually set
			foreach (var brand in brands.Where(b=>d.DealerBrandstatuses.Count(dbs=>dbs.brand_id == b.brand_id)==0))
			{
				//If brands selected by images, find its status
				if (d.Dealer_Images.Any(im => im.Brands.Any(b => b.brand_id == brand.brand_id)))
				{
					d = GetDealerForBrand(id, brand.code);
					if(d != null)
						result.Add(new Dealer_brandstatus{brand_id = brand.brand_id,dealer_id = d.user_id,brand_status = (int) d.brand_status});
				}
			}
			return result;
		}

		public static List<Brand> GetDealerBrands(int id, List<Brand> brands = null)
		{
			var d = GetById(id);
			var result = new List<Brand>();
            if(brands == null)
			    brands = BrandsDAL.GetAll();
			//Try with manual
			if (d.DealerBrandstatuses != null)
			{
				result.AddRange(d.DealerBrandstatuses.Select(dbs => brands.FirstOrDefault(b=>b.brand_id == dbs.brand_id)));
			}
			foreach (var im in d.Dealer_Images)
			{
				foreach (var b in im.Brands.Where(b => result.Count(r=>r.brand_id == b.brand_id)==0))
				{
					result.Add(b);
				}
			}
			if (result.All(b => b.brand_id != d.default_brand))
			{
				var defBrand = brands.FirstOrDefault(b => b.brand_id == d.default_brand);
				if(defBrand !=null)
					result.Add(defBrand);
			}
			return result;
		}

		public static List<DealerBrandStatusSummary> GetDealerStatusByBrandInfo()
		{
			var result = new List<DealerBrandStatusSummary>();

			var dealers = GetAll();
			var dealerBrandInfo = GetDealerBrandInfo();
			var brands = BrandsDAL.GetAll();

			foreach (var brand in brands.Where(b=>b.eb_brand == 1))
			{
				var summary = new DealerBrandStatusSummary
					{
						brand_id = brand.brand_id,
						BrandStatus = DealerBrandStatus.Standard
					};

				//STANDARD - count dealers where default_brand = current brand and they don't have images or default_brand <> current brand and they have 1 image
				summary.DealerCount = dealers.Count(d => ((d.default_brand == brand.brand_id || dealerBrandInfo.Count(di => di.user_id == d.user_id && di.brand_id == brand.brand_id) > 0)
																&& dealerBrandInfo.Count(di => di.user_id == d.user_id && di.brand_id != brand.brand_id) == 0));
				result.Add(summary);

				//BRONZE two different brands
				summary = new DealerBrandStatusSummary
					{
						brand_id = brand.brand_id,
						BrandStatus = DealerBrandStatus.Bronze
					};
				summary.DealerCount = dealers.Count(d => (  (d.default_brand == brand.brand_id || dealerBrandInfo.Count(di => di.user_id == d.user_id && di.brand_id == brand.brand_id) > 0)
																&& dealerBrandInfo.Count(di => di.user_id == d.user_id && di.brand_id != brand.brand_id) == 1));
				result.Add(summary);

				//SILVER - three brands
				summary = new DealerBrandStatusSummary
				{
					brand_id = brand.brand_id,
					BrandStatus = DealerBrandStatus.Silver
				};
				summary.DealerCount = dealers.Count(d => ((d.default_brand == brand.brand_id || dealerBrandInfo.Count(di => di.user_id == d.user_id && di.brand_id == brand.brand_id) > 0)
																&& dealerBrandInfo.Count(di => di.user_id == d.user_id && di.brand_id != brand.brand_id) == 2));
				result.Add(summary);

				//GOld
				summary = new DealerBrandStatusSummary
				{
					brand_id = brand.brand_id,
					BrandStatus = DealerBrandStatus.Gold
				};
				summary.DealerCount = dealers.Count(d => ((d.default_brand == brand.brand_id || dealerBrandInfo.Count(di => di.user_id == d.user_id && di.brand_id == brand.brand_id) > 0)
																&& dealerBrandInfo.Count(di => di.user_id == d.user_id && di.brand_id != brand.brand_id) == 3));
				result.Add(summary);

				summary = new DealerBrandStatusSummary
				{
					brand_id = brand.brand_id,
					BrandStatus = DealerBrandStatus.Platinum
				};
				summary.DealerCount = dealers.Count(d => ((d.default_brand == brand.brand_id || dealerBrandInfo.Count(di => di.user_id == d.user_id && di.brand_id == brand.brand_id) > 0)
																&& dealerBrandInfo.Count(di => di.user_id == d.user_id && di.brand_id != brand.brand_id) == 4));
				result.Add(summary);

			}


			return result;
		}

		public static void Delete(int id)
		{
			using (var conn = new MySqlConnection(Settings.Default.ConnString))
			{
				conn.Open();
				var cmd = Utils.GetCommand("DELETE FROM dealers WHERE user_id = @id", conn);
				cmd.Parameters.AddWithValue("@id", id);
				cmd.ExecuteNonQuery();
			}
		}

		public static void DeleteDisplays(int dealer_id)
		{
			using (var conn = new MySqlConnection(Settings.Default.ConnString))
			{
				conn.Open();
				var cmd = Utils.GetCommand("DELETE FROM dealer_displays WHERE client_id = @id", conn);
				cmd.Parameters.AddWithValue("@id", dealer_id);
				cmd.ExecuteNonQuery();
			}
		}

		public static Dealer GetBySurveyCode(string dealerCode)
		{
			Dealer result = null;
			using (var conn = new MySqlConnection(Settings.Default.ConnString))
			{
				conn.Open();
				var cmd = Utils.GetCommand("SELECT * FROM dealers WHERE survey_id = @code", conn);
				cmd.Parameters.AddWithValue("@code", dealerCode);
				var dr = cmd.ExecuteReader();
				if (dr.Read())
					result = GetDealerFromReader(dr);
				dr.Close();
			}
			return result;
		}
		/***/
		private static int AddRegion(string postCode)
		{
			int num = 0;
			int a = 0, b = 0, c = 0, d = 0, e = 0, f = 0, g = 0, i = 0, j = 0, l = 0, k = 0, z = 0,v=0;

			var postCodeArea = new List<PostcodeAreas>();
			postCodeArea = GetPostcodeAreas();


			var patternWales = @"";
			var patternSouthEast = @"";
			var patternNorthEast = @"";
			var patternEastMidlands = @"";
			var patternEastOfEngland = @"";
			var patternLondon = @"";
			var patternNorthWest = @"";
			var patternNorthernIreland = @"";
			var patternScotland = @"";
			var patternWestMidlands = @"";
			var patternYorkshireHumber = @"";
			var patternSouthWest = @"";
			var patternOutIreland = @"";
			//string patternWales = @"(^SY\d+)|(^SA\d+)|(^NP\d+)|(^LL\d+)|(^LD\d+)|(^CF\d+)";
			//string patternSouthEast = @"(^SO\d+)|(^SL\d+)|(^RH\d+)|(^RG\d+)|(^PO\d+)|(^OX\d+)|(^GU\d+)|(^BN\d+)";
			//string patternNorthEast = @"(^SR\d+)|(^NE\d+)|(^DH\d+)";
			//string patternEastMidlands = @"(^S\d+)|(^PE\d+)|(^NG\d+)|(^LN\d+)|(^LE\d+)|(^DN\d+)|(^DE\d+)";
			//string patternEastOfEngland = @"(^TN\d+)|(^SS\d+)|(^SG\d+)|(^NR\d+)|(^MK\d+)|(^ME\d+)|(^LU\d+)|(^IP\d+)|(^HP\d+)|(^CT\d+)|(^CO\d+)|(^CM\d+)|(^CB\d+)|(^AL\d+)|";

			//GET pattern for REGEX from database [Postcode Area] @"(^SY\d+) | ()"...
			foreach (var post in postCodeArea)
			{
				switch (post.NumOfRegion)
				{
					case 12:
						patternSouthWest += z == 0 ? @"(^" + post.PostcodeArea + @"\d)" : @"|(^" + post.PostcodeArea + @"\d)";
						z++;
						break;
					case 11:
						patternYorkshireHumber += a == 0 ? @"(^" + post.PostcodeArea + @"\d)" : @"|(^" + post.PostcodeArea + @"\d)";
						a++;
						break;
					case 10:
						patternWestMidlands += b == 0 ? @"(^" + post.PostcodeArea + @"\d)" : @"|(^" + post.PostcodeArea + @"\d)";
						b++;
						break;
					case 9:
						patternWales += c == 0 ? @"(^" + post.PostcodeArea + @"\d)" : @"|(^" + post.PostcodeArea + @"\d)";
						c++;
						break;
					case 8:
						patternSouthEast += d == 0 ? @"(^" + post.PostcodeArea + @"\d)" : @"|(^" + post.PostcodeArea + @"\d)";
						d++;
						break;
					case 7:
						patternScotland += e == 0 ? @"(^" + post.PostcodeArea + @"\d)" : @"|(^" + post.PostcodeArea + @"\d)";
						e++;
						break;
					case 6:
						patternNorthernIreland += f == 0 ? @"(^" + post.PostcodeArea + @"\d)" : @"|(^" + post.PostcodeArea + @"\d)";
						f++;
						break;
					case 5:
						patternNorthWest += g == 0 ? @"(^" + post.PostcodeArea + @"\d)" : @"|(^" + post.PostcodeArea + @"\d)";
						g++;
						break;
					case 4:
						patternNorthEast += i == 0 ? @"(^" + post.PostcodeArea + @"\d)" : @"|(^" + post.PostcodeArea + @"\d)";
						i++;
						break;
					case 3:
						patternLondon += j == 0 ? @"(^" + post.PostcodeArea + @"\d)" : @"|(^" + post.PostcodeArea + @"\d)";
						j++;
						break;
					case 2:
						patternEastOfEngland += l == 0 ? @"(^" + post.PostcodeArea + @"\d)" : @"|(^" + post.PostcodeArea + @"\d)";
						l++;
						break;
					case 1:
						patternEastMidlands += k == 0 ? @"(^" + post.PostcodeArea + @"\d)" : @"|(^" + post.PostcodeArea + @"\d)";
						k++;
						break;
					case 0:
						patternOutIreland += v == 0 ? @"(^" + post.PostcodeArea + @"\d)" : @"|(^" + post.PostcodeArea + @"\d)";
						v++;
						break;


					default:

						break;
				}
			}

			// Insert NumberOfRegion, represent calculate ID for region
			if (Regex.IsMatch(postCode, patternSouthWest))
			{
				num = 12;
			}
			else if (Regex.IsMatch(postCode, patternYorkshireHumber))
			{
				num = 11;
			}
			else if (Regex.IsMatch(postCode, patternWestMidlands))
			{
				num = 10;
			}
			else if (Regex.IsMatch(postCode, patternWales))
			{
				num = 9;
			}

			else if (Regex.IsMatch(postCode, patternSouthEast))
			{
				num = 8;
			}
			else if (Regex.IsMatch(postCode, patternScotland))
			{
				num = 7;
			}
			else if (Regex.IsMatch(postCode, patternNorthernIreland))
			{
				num = 6;
			}
			else if (Regex.IsMatch(postCode, patternNorthWest))
			{
				num = 5;
			}
			else if (Regex.IsMatch(postCode, patternNorthEast))
			{
				num = 4;
			}
			else if (Regex.IsMatch(postCode, patternLondon))
			{
				num = 3;
			}
			else if (Regex.IsMatch(postCode, patternEastOfEngland))
			{
				num = 2;
			}
			else if (Regex.IsMatch(postCode, patternEastMidlands))
			{
				num = 1;
			}
			else
			{
				num = 0;
			}
			return num;
		}
		/***/

		public static List<Dealer> GetDealersOnSurvey(int defId)
		{
			var result = new List<Dealer>();
			using (var conn = new MySqlConnection(Settings.Default.ConnString))
			{
				conn.Open();
				var cmd =
					Utils.GetCommand(@"SELECT dealers.* FROM dealers INNER JOIN survey_result ON survey_result.dealer_id = dealers.user_id
											 WHERE survey_result.surveydef_id = @def_id ", conn);
				cmd.Parameters.AddWithValue("@def_id", defId);
				var dr = cmd.ExecuteReader();
				while (dr.Read())
				{
					result.Add(GetDealerFromReader(dr));
				}
			}
			return result;
		}
        public static void UpdateDealerDigital( int digital, int id)
        {
            using (var conn = new MySqlConnection(Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("UPDATE dealers SET digital = @digital WHERE user_id = @id", conn);
                cmd.Parameters.AddWithValue("@digital", digital);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }
        }
		public static void UpdateCWCodes(Dealer[] dealer)
		{
			using (var conn = new MySqlConnection(Settings.Default.ConnString))
			{
				conn.Open();
				var cmd = Utils.GetCommand("UPDATE dealers SET cw_code = @code WHERE user_id = @id", conn);
				cmd.Parameters.AddWithValue("@code", "");
				cmd.Parameters.AddWithValue("@id", 0);
				foreach (var d in dealer)
				{
					cmd.Parameters["@code"].Value = d.cw_code;
					cmd.Parameters["@id"].Value = d.user_id;
					cmd.ExecuteNonQuery();
				}
			}
		}

		public static void CreateCWDealers(Dealer[] data)
		{
			using (var conn = new MySqlConnection(Settings.Default.ConnString))
			{
				conn.Open();
				var cmd = Utils.GetCommand(@"INSERT INTO dealers(user_id,user_name,user_address1,user_address2,user_address3,user_address4,user_tel,postcode,cw_code)
						VALUES(@user_id,@user_name, @user_address1,@user_address2,@user_address3,@user_address4,@user_tel,@postcode, @cw_code)", conn);
				var cmdId = Utils.GetCommand("SELECT MAX(user_id)+1 FROM dealers", conn);

				cmd.Parameters.AddWithValue("@user_id", 0);
				cmd.Parameters.AddWithValue("@user_name", "");
				cmd.Parameters.AddWithValue("@user_address1", "");
				cmd.Parameters.AddWithValue("@user_address2", "");
				cmd.Parameters.AddWithValue("@user_address3", "");
				cmd.Parameters.AddWithValue("@user_address4", "");
				cmd.Parameters.AddWithValue("@user_tel", "");
				cmd.Parameters.AddWithValue("@postcode", "");
				cmd.Parameters.AddWithValue("@cw_code", "");
				foreach (var d in data)
				{
					cmd.Parameters["@user_id"].Value = Convert.ToInt32(cmdId.ExecuteScalar());
					cmd.Parameters["@user_name"].Value = d.user_name;
					cmd.Parameters["@user_address1"].Value = d.user_address1.Trim();
					cmd.Parameters["@user_address2"].Value = d.user_address2.Trim();
					cmd.Parameters["@user_address3"].Value = d.user_address3.Trim();
					cmd.Parameters["@user_address4"].Value = d.user_address4.Trim();
					cmd.Parameters["@user_tel"].Value = d.user_tel.Trim();
					cmd.Parameters["@postcode"].Value = d.postcode.Trim();
					cmd.Parameters["@cw_code"].Value = d.cw_code.Trim();
					cmd.ExecuteNonQuery();
				}
			}
		}

         public static List<Dealer> GetDealerByWebProductNewForDisplays(int web_unique )
        {
            var result = new List<Dealer>();
            using (var conn = new MySqlConnection(Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand(@"SELECT dealers.*

                                            FROM
                                            dealer_image_displays
                                            INNER JOIN dealer_images ON dealer_image_displays.image_id = dealer_images.image_unique
                                            INNER JOIN dealers ON dealer_images.dealer_id = dealers.user_id
                                            WHERE
                                            dealer_image_displays.web_unique = @web_unique
                                            GROUP BY
                                            dealer_images.dealer_id", conn);

                cmd.Parameters.AddWithValue("@web_unique", web_unique);

                var dr = cmd.ExecuteReader();
                while(dr.Read()){
                    Dealer d = GetDealerFromReader(dr);
                    result.Add(d);

                }
                dr.Close();

            }
            return result;

        }

         public static List<Dealer> GetByCountryCode(int country_code)
         {
             var results = new List<Dealer>();
             using (var conn = new MySqlConnection(Settings.Default.ConnString))
             {
                 conn.Open();
                 var cmd = Utils.GetCommand("SELECT * FROM dealers AS d INNER JOIN countries AS c ON d.user_country=c.ISO2 WHERE c.country_id=@cc", conn);
                 cmd.Parameters.AddWithValue("@cc", country_code);
                 var dr = cmd.ExecuteReader();
                 while (dr.Read())
                 {
                     results.Add(GetDealerFromReader(dr));
                 }
                 dr.Close();
             }
             return results;
         }

         public static long GetDisplaysCount(int user_id, int brand_id)
         {
             long result = 0;
             //var c = Request.Url.AbsoluteUri.Contains("localhost") ? "server=localhost;User Id=webapp;password=M6peb8ad;Persist Security Info=True;database=asaq" : Settings.Default.ConnString;
             using (var conn = new MySqlConnection(Settings.Default.ConnString))
             {
                 conn.Open();
                 var cmd = Utils.GetCommand(@"SELECT Count(dealer_image_displays.image_id)
                                                FROM dealer_image_displays INNER JOIN dealer_images
                                                ON dealer_image_displays.image_id = dealer_images.image_unique
                                                INNER JOIN web_product_new
                                                ON dealer_image_displays.web_unique = web_product_new.web_unique
                                                WHERE dealer_images.dealer_id = @user_id AND web_site_id = @brand_id ", conn);
                 cmd.Parameters.AddWithValue("@user_id", user_id);
                 cmd.Parameters.AddWithValue("@brand_id", brand_id);
                 result = (long)cmd.ExecuteScalar();
             }
             return result;
         }

	    public static List<DealerInfoModel> GetDealerData(int brandId)
	    {
	        var result = new List<DealerInfoModel>();
	        //var c = Request.Url.AbsoluteUri.Contains("localhost") ? "server=localhost;User Id=webapp;password=M6peb8ad;Persist Security Info=True;database=asaq" : Settings.Default.ConnString;
            using (var conn = new MySqlConnection(Settings.Default.ConnString))
	        {
	            conn.Open();
	            var cmd = Utils.GetCommand(@"SELECT DISTINCT user_id, user_name, postcode, COUNT(dealer_images.dealer_id) AS count
                                                FROM dealers
                                                INNER JOIN dealer_images ON dealer_images.dealer_id = dealers.user_id
                                                INNER JOIN dealer_brandstatus ON dealer_brandstatus.dealer_id = user_id
                                                WHERE dealer_brandstatus.brand_id = @brandId
                                                GROUP BY dealers.user_id", conn);
	            cmd.Parameters.AddWithValue("@brandId", brandId);
	            var dr = cmd.ExecuteReader();
	            while (dr.Read())
	            {
	                result.Add(GetDealerFromReader2(dr));
	            }
	            dr.Close();
	        }
	        return result;
	    }

	    public static DateTime GetArcadeDealerSince(int user_id, int brandId)
	    {
	        object result;
	        //var c = Request.Url.AbsoluteUri.Contains("localhost") ? "server=localhost;User Id=webapp;password=M6peb8ad;Persist Security Info=True;database=asaq" : Settings.Default.ConnString;
            using (var conn = new MySqlConnection(Settings.Default.ConnString))
	        {
	            conn.Open();
	            var cmd = Utils.GetCommand(@"SELECT dealer_displays_activity.datecreated
                                                FROM dealer_displays_activity
                                                INNER JOIN web_product_new ON dealer_displays_activity.web_unique = web_product_new.web_unique
                                                WHERE dealer_id = @user_id
                                                AND web_site_id = @brandid
                                                LIMIT 1", conn);
	            cmd.Parameters.AddWithValue("@user_id", user_id);
	            cmd.Parameters.AddWithValue("@brandid", brandId);
	            result = cmd.ExecuteScalar();
	            if (result == null)
	                return DateTime.MinValue;
	            else
	                return (DateTime)result;
	        }
	    }

	    public static int GetNumOfDealersForCountryAndBrand(string brand_code, string countryCode)
	    {
	        var result = 0;
	        using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString)) {
	            conn.Open();
	            MySqlCommand cmd = Utils.GetCommand(@"SELECT COUNT(*) FROM
                                                    dealers AS d INNER JOIN dealer_brandstatus AS db ON db.dealer_id = d.user_id
                                                    INNER JOIN brands AS b ON b.brand_id = db.brand_id
                                                    LEFT OUTER JOIN countries AS C ON d.user_country = c.ISO2
                                                    WHERE TRIM(COALESCE(d.user_country,'')) = @countryCode AND b.code=@brand_code ", conn);
	            cmd.Parameters.AddWithValue("@brand_code", brand_code);
	            cmd.Parameters.AddWithValue("@countryCode", countryCode);
	            result = Convert.ToInt32(Utilities.FromDbValue<long>(cmd.ExecuteScalar()));
	        }
	        return result;
	    }

	    private static DealerInfoModel GetDealerFromReader2(MySqlDataReader dr)
        {
            DealerInfoModel d = new DealerInfoModel();
            d.user_id = (int)dr["user_id"];
            d.user_name = String.Empty + dr["user_name"];
            d.postcode = String.Empty + dr["postcode"];
            d.countImages = (long)dr["count"];
            return d;
        }
	}

    public class DealerInfoModel
    {
        public int user_id { get; set; }
        public string user_name { get; set; }
        public string postcode { get; set; }
        public long countImages { get; set; }
        public long countDisplays { get; set; }
    }

}
