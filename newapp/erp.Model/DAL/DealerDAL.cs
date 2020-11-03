using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace asaq2.Model
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
        public static List<Dealer> GetNearestDealers(string website, double latitude, double longitude)
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
//                var cmd = new MySqlCommand(sql, conn);
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
            var dealers = GetDealersForBrand(website);
            foreach (var d in dealers)
            {
                if (d.latitude != null && d.longitude != null)
                {
                    d.Distance = GeoUtils.distance(latitude, longitude, d.latitude.Value, d.longitude.Value, 'M');
                    if (b.dealerstatus_manual != null && b.dealerstatus_manual.Value)
                    {
                        d.BrandStatuses = GetStatusesFromTable(d.user_id);
                        if (d.BrandStatuses.ContainsKey(b.brand_id))
                            d.brand_status = d.BrandStatuses[b.brand_id];
                    }
                    
                }
                else
                {
                    d.Distance = double.MaxValue;
                }
            }
            return dealers.OrderBy(d => d.Distance).ToList(); ;
        }

        public static List<Dealer> GetNearestDealers(string website, double latitude, double longitude, bool isGold = false, bool isSilver=false)
        {
//            List<Dealer> result = new List<Dealer>();
//            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
//            {
//                conn.Open();
//                Brand b = BrandsDAL.GetByCode(website);
//                if (b != null && !string.IsNullOrEmpty(b.dealerstatus_view))
//                {
//                    MySqlCommand cmd =
//                        new MySqlCommand(
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
            return GetNearestDealers(new[] {website}, latitude, longitude, isGold, isSilver);
        }

        public static List<Dealer> GetNearestDealers(string[] website_ids, double latitude, double longitude, bool isGold = false, bool isSilver = false)
        {
            List<Dealer> result = new List<Dealer>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                foreach (var website in website_ids)
                {
                    Brand b = BrandsDAL.GetByCode(website);
                    if (b != null && !string.IsNullOrEmpty(b.dealerstatus_view))
                    {
                        var cmd =
                            new MySqlCommand(
                                string.Format(
                                    @"SELECT {0}.*,(SELECT user_email3 FROM users WHERE user_id = {0}.user_type LIMIT 1) AS dist_email,
                                                             (SELECT userwelcome FROM userusers WHERE user_id = {0}.user_type LIMIT 1) AS dist_name FROM {0} WHERE latitude BETWEEN -90 AND 90 AND longitude BETWEEN -180 AND 180 AND hide_1 = 1",
                                    b.dealerstatus_view),
                                conn);
                        if (isGold)
                            cmd.CommandText += " AND (gold_override = 1 OR gold = 1)";
                        if (isSilver)
                            cmd.CommandText += " AND gold_override = 0 and gold = 0 and silver = 1";
                        cmd.CommandText +=
                            string.Format(@" AND ( EXISTS (SELECT dealer_displays.web_unique FROM dealer_displays 
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
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM dealers WHERE user_email = @email AND user_pwd = @password", conn);
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
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT * FROM dealers  WHERE user_email = @email", conn);
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
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT * FROM dealers", conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetDealerFromReader(dr));
                }
            }
            return result;
        }

        public static Dealer GetDealerForBrand(int dealer_id, string brand_code)
        {
            Dealer d = null;
            Brand b = BrandsDAL.GetByCode(brand_code);
            if (b != null && !string.IsNullOrEmpty(b.dealerstatus_view))
            { 
                using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
                {
                    conn.Open();
                    var cmd = new MySqlCommand(string.Format("SELECT * FROM {0}  WHERE client_id = @dealer_id", b.dealerstatus_view), conn);
                    cmd.Parameters.AddWithValue("@dealer_id", dealer_id);
                    var dr = cmd.ExecuteReader();
                    if (dr.Read())
                    {
                        d = GetDealerFromReader(dr);
                        d.Images = GetImages(d);
                    }
                    dr.Close();
                }
            }
            return d;
        }

        public static List<Dealer> GetDealersForBrand(string brand_code)
        {
            var result = new List<Dealer>();
            Brand b = BrandsDAL.GetByCode(brand_code);
            if (b != null )
            {
                using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
                {
                    conn.Open();
                    string sql = string.Empty;
                    var cmd = new MySqlCommand("", conn);
                    if (!string.IsNullOrEmpty(b.dealerstatus_view))
                    {
                        cmd.CommandText = string.Format(@"SELECT {0}.*,dist.user_name as dist_name, dist.customer_code As dist_code, 
                                                        (SELECT COUNT(*) FROM dealer_images WHERE dealer_id = {0}.user_id) AS numImages FROM {0} INNER JOIN users AS dist ON {0}.user_type = dist.user_id ",b.dealerstatus_view);
                    }
                    else
                    {
                        cmd.CommandText = @"SELECT dealers.*,dist.user_name as dist_name, dist.customer_code As dist_code,
                                (SELECT dealer_brandstatus.brand_status FROM dealer_brandstatus WHERE dealers.user_id = dealer_brandstatus.dealer_id AND dealer_brandstatus.brand_id = @brand_id) AS brand_status  , 
                                                        (SELECT COUNT(*) FROM dealer_images WHERE dealer_id = dealers.user_id) AS numImages FROM dealers INNER JOIN users AS dist ON dealers.user_type = dist.user_id 
                                
                        WHERE sales_registered=1 AND ( EXISTS(SELECT unique_id FROM dealer_displays INNER JOIN web_products ON dealer_displays.web_unique = web_products.web_unique 
                                WHERE web_products.web_site = @code AND dealer_displays.client_id = dealers.user_id) OR 
                          default_brand = @brand_id )";
                        cmd.Parameters.AddWithValue("@code", brand_code);
                    }
                    
                    cmd.Parameters.AddWithValue("@brand_id", b.brand_id);
                    var dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        result.Add(GetDealerFromReader(dr));
                    }
                    dr.Close();
                }
            }
            return result;
        }

        public static int GetDealerCountForBrand(string brand_code)
        {
            int result = 0;
            Brand b = BrandsDAL.GetByCode(brand_code);
            if (b != null && !string.IsNullOrEmpty(b.dealerstatus_view))
            {
                using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
                {
                    conn.Open();
                    var cmd = new MySqlCommand(string.Format(@"SELECT COUNT(*) FROM {0} ", b.dealerstatus_view), conn);
                    result = Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            return result;
        }

        public static List<DealerStat> GetDealerStatForBrand(int brand_id, DateTime? from= null, DateTime? to = null,bool salesregOnly = false)
        {
            var result = new List<DealerStat>();
            var b = BrandsDAL.GetById(brand_id);
            if (b != null)
            {
                if (!string.IsNullOrEmpty(b.dealerstatus_view))
                {
                    using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
                    {
                        conn.Open();
                        string sql = string.Empty;

                        sql = string.Format(
                            @"SELECT COUNT(*) AS Count, Users.user_id, users.user_name,users.customer_code,
                            (SELECT COUNT(DISTINCT {0}.user_id) FROM {0} INNER JOIN dealer_images ON dealer_images.dealer_id = {0}.user_id
                                WHERE {0}.user_type = users.user_id AND (@from IS NULL OR {0}.user_created >= @from) AND (@to IS NULL OR {0}.user_created <= @to) {1}) AS DealersWithImages
                            FROM {0} INNER JOIN users ON {0}.user_type = users.user_id 
                            WHERE (@from IS NULL OR {0}.user_created >= @from) AND (@to IS NULL OR {0}.user_created <= @to) {1}
                            GROUP BY users.user_id, users.user_name,users.customer_code  ", b.dealerstatus_view,
                            salesregOnly ? string.Format(" AND {0}.sales_registered = 1", b.dealerstatus_view) : "");

                        var cmd = new MySqlCommand(sql, conn);
                        cmd.Parameters.AddWithValue("@from", from != null ? (object)from.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("@to", to != null ? (object)to.Value : DBNull.Value);
                        var dr = cmd.ExecuteReader();
                        while (dr.Read())
                        {
                            result.Add(new DealerStat
                            {
                                brand_id = brand_id,
                                Count = Convert.ToInt32(dr["Count"]),
                                distributor_id = (int)dr["user_id"],
                                distributor_code = string.Empty + dr["customer_code"],
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
                                (from == null || d.user_created >= from) && (to == null || d.user_created <= to) &&
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
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand(string.Format(@"SELECT distributor.customer_code, numOfBrands, COUNT(*) AS numOfDealers FROM
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
                        ORDER BY customer_code", salesregOnly ? " AND dealers.sales_registered = 1" : "",specialBrandCodes != null ? string.Format(" OR brands.code IN ({0})",
                                               string.Join(",",specialBrandCodes.Select(s=>string.Format("'{0}'",s)))) : ""), conn);
                cmd.Parameters.AddWithValue("@from", from != null ? (object)from.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("@to", to != null ? (object)to.Value : DBNull.Value);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(new DealerMultiBrandStat
                    {
                        distributor_code = string.Empty + dr["customer_code"],
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
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand(string.Format(@"SELECT dealers.*, distributor.user_name AS dist_name, brands.`code`, brands.brandname,distributor.customer_code AS dist_code
                                FROM dealers
                                INNER JOIN users AS distributor ON distributor.user_id = dealers.user_type
                                INNER JOIN dealer_displays ON dealers.user_id = dealer_displays.client_id 
                                INNER JOIN web_products ON dealer_displays.web_unique = web_products.web_unique
                                INNER JOIN brands ON web_products.web_site = brands.`code`
                                WHERE (SELECT COUNT(DISTINCT web_products.web_site) FROM dealer_displays INNER JOIN web_products ON dealer_displays.web_unique = web_products.web_unique WHERE dealer_displays.client_id = dealers.user_id) > 1
                                AND (brands.eb_brand = 1 {0})
                                GROUP BY dealers.user_id, web_products.web_site",specialBrandCodes != null ? string.Format(" OR brands.code IN ({0})",
                                               string.Join(",",specialBrandCodes.Select(s=>string.Format("'{0}'",s)))) : ""), conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(new DealerMultiBrandStat2{brand_code = string.Empty + dr["code"], brandname = string.Empty + dr["brandname"],Dealer = GetDealerFromReader(dr)});
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
        //            var cmd = new MySqlCommand(string.Format("SELECT {0}.*, (SELECT COUNT(*) FROM dealer_images WHERE dealer_id = {0}.user_id) AS numImages FROM {0} ", b.dealerstatus_view), conn);

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
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd =
                    new MySqlCommand(
                        "SELECT COUNT(*) FROM dealers WHERE NOT EXISTS (SELECT * FROM dealer_displays WHERE client_id = dealers.user_id) AND (dealers.user_type = @user_type OR @user_type IS NULL)", conn);
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

        public static Dealer GetById(int id)
        {
            Dealer d = null;
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT * FROM dealers WHERE user_id = @id", conn);
                cmd.Parameters.AddWithValue("@id", id);
                MySqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    d = GetDealerFromReader(dr);
                    //get images
                    
                    d.DisplayedProducts = GetDisplaysForDealer(id);
                    d.Images = GetImages(d);
                    d.BrandStatuses = GetStatusesFromTable(id);
                    

                }
                dr.Close();
            }
            return d;
        }

        public static Dictionary<int, DealerBrandStatus> GetStatusesFromTable(int dealer_id)
        {
            var result = new Dictionary<int, DealerBrandStatus>();
            var statuses = new List<Dealer_brandstatus>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT * FROM dealer_brandstatus WHERE dealer_id = @dealer_id", conn);
                cmd.Parameters.AddWithValue("@dealer_id", dealer_id);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    statuses.Add(new Dealer_brandstatus{brand_id = (int) dr["brand_id"], brand_status = (int) dr["brand_status"]});
                }
                dr.Close();
                if (statuses.Count > 0)
                {
                    
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
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand(@"select `dealers`.`user_id` AS `user_id`,`dealer_image_brand`.`brand_id` AS `brand_id`
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
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(@"INSERT INTO auto_emails_dealers(autoe_dist, autoe_email, autoe_copy1, autoe_copy2, autoe_name, autoe_create_date, 
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
                                brand_b,brand_wc,confirmed,default_brand,sales_registered) 
                                VALUES(@user_id,@user_name,@user_account,@customer_code,@distributor,@user_welcomename,@user_address1,@user_address2,@user_address3,
                                @user_address4,@user_address5,@postcode,@ie_region,@user_country,@user_contact,@user_tel,@user_fax,@user_mobile,@user_website,@user_email,@user_email2,
                                @user_type,@user_access,@user_pwd,@user_created,@user_curr,@user_curr_pricing,@dynamic_pricing,@lastlogin,@hide_1,@gold_override,@opening,@directions1,
                                @directions2,@directions3,@directions4,@image,@longitude,@latitude,@opening1_from,@opening1_to,@opening2_from,@opening2_to,@opening3_from,@opening3_to,
                                @opening4_from,@opening4_to,@opening5_from,@opening5_to,@opening6_from,@opening6_to,@opening7_from,@opening7_to,@image_policy,@image_policy_acceptance,
                                @image_policy_ip,@training,@training_date,@brand_b,@brand_wc,@confirmed,@default_brand,@sales_registered)";

            var conn = new MySqlConnection(Properties.Settings.Default.ConnString);
			conn.Open();
			MySqlTransaction tr = conn.BeginTransaction();
            try
            {
                MySqlCommand cmd = new MySqlCommand("SELECT MAX(user_id)+1 FROM dealers", conn, tr);
                o.user_id = Convert.ToInt32(cmd.ExecuteScalar());

                cmd.CommandText = insertsql;
                BuildSqlParameters(cmd, o);
                cmd.ExecuteNonQuery();

                if (o.Dealer_Images != null)
                {
                    foreach (var image in o.Dealer_Images)
                    {
                        image.dealer_id = o.user_id;
                        Dealer_imagesDAL.Create(image, tr);
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
                                training = @training,training_date = @training_date,brand_b = @brand_b,brand_wc = @brand_wc, confirmed = @confirmed,default_brand = @default_brand,sales_registered = @sales_registered WHERE user_id = @user_id";

            var conn = new MySqlConnection(Properties.Settings.Default.ConnString);
            conn.Open();
            MySqlTransaction tr = conn.BeginTransaction();
            try
            {

                var cmd = new MySqlCommand(updatesql, conn, tr);
                BuildSqlParameters(cmd, d);
                cmd.ExecuteNonQuery();

                if (d.Dealer_Images != null)
                {
                    foreach (var image in d.Dealer_Images)
                    {
                        if (image.image_unique <= 0)
                        {
                            image.dealer_id = d.user_id;
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

                if (d.DisplayedProducts != null)
                {
                    
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@dealer_id", d.user_id);
                    cmd.Parameters.AddWithValue("@web_unique", 0);
                    cmd.Parameters.AddWithValue("@qty", 0);
                    foreach (var dp in d.DisplayedProducts)
                    {
                        cmd.Parameters[1].Value = dp.Product.web_unique;
                        cmd.Parameters[2].Value = dp.qty;
                        
                        if (dp.unique_id == 0)
                        {
                            cmd.CommandText = "INSERT INTO dealer_displays(client_id, web_unique, qty) VALUES(@dealer_id, @web_unique,@qty)";
                        }
                        else
                        {
                            cmd.CommandText = "UPDATE dealer_displays SET qty = @qty WHERE web_unique = @web_unique AND client_id = @dealer_id";
                        }
                        cmd.ExecuteNonQuery();
                    }
                }
                if (deletedProducts != null)
                {
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@dealer_id", d.user_id);
                    cmd.CommandText = string.Format("DELETE FROM dealer_displays WHERE client_id = @dealer_id AND web_unique IN ({0})",Utilities.CreateParametersFromIdList(cmd, deletedProducts));
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
            }
            finally
            {
                conn = null;
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
                conn = new MySqlConnection(Properties.Settings.Default.ConnString);
                conn.Open();
                tr = conn.BeginTransaction();
                MySqlCommand cmd = new MySqlCommand(deletesql);
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
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand("DELETE FROM dealer_images WHERE image_unique = @id", conn);
                cmd.Parameters.AddWithValue("@id", image_id);
                cmd.ExecuteNonQuery();
            }
        }

        public static void InsertImage(int dealer_id, string image_name)
        {
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand("INSERT INTO dealer_images (dealer_id, dealer_image, seq, hide) VALUES(@dealer_id, @dealer_image, 0,0)", conn);
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
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                string commandText;
                if (add)
                    commandText = "INSERT INTO dealer_displays(client_id, web_unique, qty) VALUES(@dealer_id, @product_id,1)";
                else
                    commandText = "DELETE FROM dealer_displays WHERE client_id = @dealer_id AND web_unique = @product_id";
                MySqlCommand cmd = new MySqlCommand(commandText, conn);
                cmd.Parameters.AddWithValue("@dealer_id", dealer_id);
                cmd.Parameters.AddWithValue("@product_id", product_id);
                cmd.ExecuteNonQuery();
            }
        }

        public  static Dealer GetDealerFromReader(MySqlDataReader dr)
        {
            Dealer d = new Dealer();
            d.user_id = (int) dr["user_id"];
            d.user_name = string.Empty + dr["user_name"];
            d.user_account = string.Empty + dr["user_account"];
            d.customer_code = string.Empty + dr["customer_code"];
            d.distributor = Utilities.FromDbValue<int>(dr["distributor"]);
            d.user_welcomename = string.Empty + dr["user_welcomename"];
            d.user_address1 = string.Empty + dr["user_address1"];
            d.user_address2 = string.Empty + dr["user_address2"];
            d.user_address3 = string.Empty + dr["user_address3"];
            d.user_address4 = string.Empty + dr["user_address4"];
            d.user_address5 = string.Empty + dr["user_address5"];
            d.postcode = string.Empty + dr["postcode"];
            d.ie_region = Utilities.FromDbValue<int>(dr["ie_region"]);
            d.user_country = string.Empty + dr["user_country"];
            d.user_contact = string.Empty + dr["user_contact"];
            d.user_tel = string.Empty + dr["user_tel"];
            d.user_fax = string.Empty + dr["user_fax"];
            d.user_mobile = string.Empty + dr["user_mobile"];
            d.user_website = string.Empty + dr["user_website"];
            d.user_email = string.Empty + dr["user_email"];
            d.user_email2 = string.Empty + dr["user_email2"];
            d.user_type = Utilities.FromDbValue<int>(dr["user_type"]);
            d.user_access = Utilities.FromDbValue<int>(dr["user_access"]);
            d.user_pwd = string.Empty + dr["user_pwd"];
            d.user_created = Utilities.FromDbValue<DateTime>(dr["user_created"]);
            d.user_curr = Utilities.FromDbValue<int>(dr["user_curr"]);
            d.user_curr_pricing = Utilities.FromDbValue<int>(dr["user_curr_pricing"]);
            d.dynamic_pricing = Utilities.FromDbValue<int>(dr["dynamic_pricing"]);
            d.lastlogin = Utilities.FromDbValue<DateTime>(dr["lastlogin"]);
            d.hide_1 = Utilities.FromDbValue<int>(dr["hide_1"]);
            d.gold_override = Utilities.FromDbValue<int>(dr["gold_override"]);
            d.opening = string.Empty + dr["opening"];
            d.directions1 = string.Empty + dr["directions1"];
            d.directions2 = string.Empty + dr["directions2"];
            d.directions3 = string.Empty + dr["directions3"];
            d.directions4 = string.Empty + dr["directions4"];
            d.image = string.Empty + dr["image"];
            d.longitude = Utilities.FromDbValue<double>(dr["longitude"]);
            d.latitude = Utilities.FromDbValue<double>(dr["latitude"]);
            d.opening1_from = string.Empty + dr["opening1_from"];
            d.opening1_to = string.Empty + dr["opening1_to"];
            d.opening2_from = string.Empty + dr["opening2_from"];
            d.opening2_to = string.Empty + dr["opening2_to"];
            d.opening3_from = string.Empty + dr["opening3_from"];
            d.opening3_to = string.Empty + dr["opening3_to"];
            d.opening4_from = string.Empty + dr["opening4_from"];
            d.opening4_to = string.Empty + dr["opening4_to"];
            d.opening5_from = string.Empty + dr["opening5_from"];
            d.opening5_to = string.Empty + dr["opening5_to"];
            d.opening6_from = string.Empty + dr["opening6_from"];
            d.opening6_to = string.Empty + dr["opening6_to"];
            d.opening7_from = string.Empty + dr["opening7_from"];
            d.opening7_to = string.Empty + dr["opening7_to"];
            d.image_policy = Utilities.FromDbValue<int>(dr["image_policy"]);
            d.image_policy_acceptance = Utilities.FromDbValue<DateTime>(dr["image_policy_acceptance"]);
            d.image_policy_ip = string.Empty + dr["image_policy_ip"];
            d.training = Utilities.FromDbValue<int>(dr["training"]);
            d.training_date = Utilities.FromDbValue<DateTime>(dr["training_date"]);
            d.brand_b = Utilities.FromDbValue<int>(dr["brand_b"]);
            
            if(Utilities.ColumnExists(dr,"default_brand"))
                d.default_brand = Utilities.FromDbValue<int>(dr["default_brand"]);
            if(Utilities.ColumnExists(dr,"confirmed"))
                d.confirmed = Utilities.FromDbValue<int>(dr["confirmed"]);
            if(Utilities.ColumnExists(dr, "brand_wc"))
                d.brand_wc = Utilities.FromDbValue<int>(dr["brand_wc"]);
            if (Utilities.ColumnExists(dr, "dist_name"))
                d.DistributorName = string.Empty + dr["dist_name"];
            if (Utilities.ColumnExists(dr, "dist_code"))
                d.DistributorCode = string.Empty + dr["dist_code"];
            if (Utilities.ColumnExists(dr, "dist_email"))
                d.DistributorEmail = string.Empty + dr["dist_email"];

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
            return d;

        }



        public static List<Dealer> GetDealersByDistributor(int distributor_id, string brand_code = "")
        {
            var result = new List<Dealer>();
            Brand brand = null;
            if (!string.IsNullOrEmpty(brand_code))
                brand = BrandsDAL.GetByCode(brand_code);
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                
                string sql =
                    @"SELECT dealers.*, (SELECT COUNT(*) FROM dealer_images WHERE dealer_id = dealers.user_id) AS numImages,
                                                      (SELECT COUNT(*) FROM dealer_displays WHERE client_id = dealers.user_id) AS numDisplays FROM dealers WHERE user_type = @user_type";
                var cmd = new MySqlCommand(sql, conn);
                if (!string.IsNullOrEmpty(brand_code))
                {
                    cmd.CommandText =
                        @"SELECT dealers.*, (SELECT COUNT(*) FROM dealer_images WHERE dealer_id = dealers.user_id) AS numImages,
                                                      (SELECT COUNT(*) FROM dealer_displays WHERE client_id = dealers.user_id) AS numDisplays 
                            FROM dealers WHERE user_type = @user_type 
                    AND (EXISTS(SELECT unique_id FROM dealer_displays INNER JOIN web_products ON dealer_displays.web_unique = web_products.web_unique 
                                WHERE web_products.web_site = @code AND dealer_displays.client_id = dealers.user_id) OR 
                          default_brand = @brand_id)";
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
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand(@"SELECT dealers.*, (SELECT COUNT(*) FROM dealer_images WHERE dealer_id = dealers.user_id) AS numImages,
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
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand(@"SELECT dealers.*, users.customer_code AS dist_code,
                                                (SELECT COUNT(*) FROM dealer_images WHERE dealer_id = dealers.user_id) AS numImages 
                                            FROM dealers INNER JOIN users ON dealers.user_type = users.user_id 
                                        WHERE ( COALESCE(dealers.hide_1,0) = 0 OR COALESCE(dealers.confirmed,0) = 0) AND (dealers.user_type = @user_type OR @user_type IS NULL)", conn);
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

        public static List<Dealer> GetDealersByCriteria(string searchText)
        {
            List<Dealer> result = new List<Dealer>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(@"SELECT dealers.* FROM  dealers WHERE user_name LIKE @criteria AND COALESCE(hide_1,0) <> 2", conn);
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
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(@"SELECT dealers.* FROM  dealers WHERE postcode = @postcode AND (user_type = @user_type OR @user_type IS NULL)", conn);
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

        public static List<Dealer_displays> GetDisplaysForDealer(int dealer_id)
        {
            List<Dealer_displays> result = new List<Dealer_displays>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                string sql = @"SELECT web_products2.*, dealer_displays.qty,dealer_displays.unique_id, dealers.*, brand_categories_sub.seq AS sub_seq FROM web_products2 INNER JOIN dealer_displays ON web_products2.web_unique = dealer_displays.web_unique 
                            INNER JOIN dealers ON dealer_displays.client_id  = dealers.user_id
                            LEFT OUTER JOIN brand_categories_sub ON web_products2.web_sub_category = brand_categories_sub.brand_sub_id
                            WHERE  dealer_displays.client_id = @dealer_id AND COALESCE(web_status,0) <> 2 ORDER BY web_seq2, sub_seq,brand_sub_desc";
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.Add(new MySqlParameter("@dealer_id", dealer_id));
                
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Dealer_displays dd = new Dealer_displays();
                    dd.Product = WebProductsDAL.GetProductFromReader(dr);
                    dd.Product.Components = WebProductsDAL.GetComponents(dd.Product, dr, true);
                    dd.Dealer = DealerDAL.GetDealerFromReader(dr);
                    dd.qty = Utilities.FromDbValue<int>(dr["qty"]);
                    dd.unique_id = (int)dr["unique_id"];
                    dd.web_unique = dd.Product.web_unique;
                    dd.client_id = dd.Dealer.user_id;
                    result.Add(dd);
                }
                dr.Close();
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
}
