using System;
using System.Collections.Generic;
using System.Linq;
using erp.Model;
using System.Data.Entity;
using erp.DAL.EF.Repositories;
using LinqKit;

namespace erp.DAL.EF
{
	public class DealerRepository : GenericRepository<Dealer>
    {
		public DealerRepository(DbContext context) : base(context)
		{
		}

		public static List<Dealer> GetDealersProcessedInMonth(DateTime month, int status)
        {
            using (var m = Model.CreateModel())
            {

                var from = Utilities.GetMonthStart(month);
                var to = Utilities.GetMonthEnd(month);
                
                return m.Dealers.Include("Dealer_Images").Include("Distributors").Where(d => (d.hide_1 ?? 0) == status && d.confirmed == 1 && d.hide_1 != 2 
                                                                                    && ((d.user_created >= from && d.user_created <= to) || (d.user_modified >= from && d.user_modified <= to))).ToList();
            }
        }

        public static List<Dealer> GetPendingDealers(int? dist_id = null)
        {
            using (var m = Model.CreateModel())
            {
                //( COALESCE(dealers.hide_1,0) = 0 OR COALESCE(dealers.confirmed,0) = 0) AND dealers.hide_1 <> 2 
                return
                    m.Dealers.Include("Dealer_Images")
                     .Include("Distributors")
                     .Where(d => ((d.hide_1 ?? 0) == 0 || (d.confirmed ?? 0) == 0) && d.hide_1 != 2 && (dist_id == null || d.Distributors.Any(dist=>dist.user_id == dist_id)))
                     .ToList();
            }
            
        }

        public static List<Dealer> GetAll()
        {
            using (var m = Model.CreateModel())
            {
                return m.Dealers.Include("Dealer_Images").ToList();
            }
        }

        public static List<Dealer> GetRegistered()
        {
            using (var m = Model.CreateModel())
            {
                return m.Dealers.Include("Distributors").Where(d => d.sales_registered_2015 == true).ToList();
            }
        }

        public static Dealer GetByIdSimple(int id)
        {
            using (var m = Model.CreateModel())
            {
                return m.Dealers.FirstOrDefault(d => d.user_id == id);
            }
        }

        public static Dealer GetById(int id)
        {
            using (var m = Model.CreateModel())
            {
                var dealer = 
                    m.Dealers.Include("Dealer_Images")
                        .Include("Dealer_Images.Displays")
                        .Include("DisplayActivities")
                        .FirstOrDefault(d => d.user_id == id);
                if (dealer != null)
                {
                    foreach (var img in dealer.Dealer_Images)
                    {
                        foreach (var disp in img.Displays)
                        {
                            var act =
                                dealer.DisplayActivities.FirstOrDefault(
                                    a => a.web_unique == disp.web_unique && a.old_qty == null);
                            if (act != null)
                                disp.datecreated = act.datecreated;
                        }
                    }
                    return dealer;
                }
                return null;
            }
        }

        public List<Dealer> GetDealersByDistributor(int distributor_id, string brand_code = "")
        {
            var result = new List<Dealer>();
            Brand brand = null;
            if (!string.IsNullOrEmpty(brand_code))
            {
                var brandsRepo = new BrandsRepository();
                brand = brandsRepo.GetByCriteria(brand_code);
            }
                
            using (var m = Model.CreateModel())
            {

                result =
                    m.Dealers.Where(
                        d =>
                        d.Distributors.Any(dist => dist.user_id == distributor_id) /*&&
                        (brand_code == "" ||
                        
                        (d.Dealer_Images.Any(im=>im.Displays.Any(disp=>disp.ProductNew.WebSite.brand_id == brand.brand_id )) || d.default_brand == brand.brand_id || d.DealerBrandstatuses.Any(dbs=>dbs.brand_id == brand.brand_id))

                        )*/).ToList();


                
//                string sql =
//                    @"SELECT dealers.*, (SELECT COUNT(*) FROM dealer_images WHERE dealer_id = dealers.user_id) AS numImages,
//                                                      (SELECT COUNT(*) FROM dealer_image_displays INNER JOIN dealer_images ON dealer_image_displays.image_id = dealer_images.image_unique
//                                                      WHERE dealer_images.dealer_id = dealers.user_id) AS numDisplays FROM dealers WHERE user_type = @user_type";
//                var cmd = Utils.GetCommand(sql, conn);
//                if (!string.IsNullOrEmpty(brand_code))
//                {
//                    cmd.CommandText =
//                        @"SELECT dealers.*, (SELECT COUNT(*) FROM dealer_images WHERE dealer_id = dealers.user_id) AS numImages,
//                                                      (SELECT COUNT(*) FROM dealer_image_displays INNER JOIN dealer_images ON dealer_image_displays.image_id = dealer_images.image_unique
//                                                      WHERE dealer_images.dealer_id = dealers.user_id) AS numDisplays 
//                            FROM dealers WHERE user_type = @user_type 
//                    AND (EXISTS(SELECT dealer_image_displays.web_unique FROM dealer_image_displays INNER JOIN dealer_images ON dealer_image_displays.image_id = dealer_images.image_unique 
//                                INNER JOIN web_product_new ON dealer_image_displays.web_unique = web_product_new.web_unique INNER JOIN 
//                                web_site ON web_product_new.web_site_id = web_site.id
//                                WHERE web_site.brand_id = @brand_id AND dealer_images.dealer_id = dealers.user_id) OR 
//                          default_brand = @brand_id   OR 
//                            EXISTS (SELECT dealer_id FROM dealer_brandstatus WHERE dealer_id = dealers.user_id AND brand_id = @brand_id)
//                            )";
//                    cmd.Parameters.AddWithValue("@code", brand.code);
//                    cmd.Parameters.AddWithValue("@brand_id", brand.brand_id);
//                }

//                cmd.Parameters.AddWithValue("@user_type", distributor_id);
//                MySqlDataReader dr = cmd.ExecuteReader();
//                while (dr.Read())
//                {
//                    result.Add(GetDealerFromReader(dr));

//                }
//                dr.Close();
            }
            return result;
        }

        public static List<Dealers_all_brands_new_export> GetDealersForExport(IList<int> brands = null, IList<int> distributors = null )
        {
            const int Britton = 4;
            const int Burlington = 2;
            const int Clearwater = 1;
            const int Arcade = 11;

            var britton = brands != null && brands.Contains(Britton);
            var burlington = brands != null && brands.Contains(Burlington);
            var clearwater = brands != null &&  brands.Contains(Clearwater);
            var arcade = brands != null && brands.Contains(Arcade);


            using (var m = Model.CreateModel())
            {
                var predicate = PredicateBuilder.True<Dealers_all_brands_new_export>();
                predicate = predicate.And(d => d.hide_1 != 2);
                if (brands != null && brands.Count > 0)
                {
                    var statusPredicate = PredicateBuilder.False<Dealers_all_brands_new_export>();
                    if (britton)
                        statusPredicate =  statusPredicate.Or(d => d.BRITTON_STATUS != null && d.BRITTON_STATUS != "" && d.BRITTON_STATUS != "0");
                    if (burlington)
                        statusPredicate = statusPredicate.Or(d => d.burlington_STATUS != null && d.burlington_STATUS != "" && d.burlington_STATUS != "0");
                    if (clearwater)
                        statusPredicate = statusPredicate.Or(d => d.clearwater_STATUS != null && d.clearwater_STATUS != "" && d.clearwater_STATUS != "0");
                    if (arcade)
                        statusPredicate = statusPredicate.Or(d => d.ARCADE_STATUS != null && d.ARCADE_STATUS != "" && d.ARCADE_STATUS != "0");
                    predicate = predicate.And(statusPredicate.Expand());
                }
                var dealers = m.DealersAllBrandsNewExports.AsExpandable().Where(predicate).ToList();
                if (distributors != null)
                {
                    return
                        dealers.Where(
                            d => d.distributors.Split(',').Select(int.Parse).Intersect(distributors).Any())
                            .ToList();
                }
                return dealers;
            }
        }

        public static List<Dealers_all_brands_new_export> GetDealersForExportNotFiltered(IList<int> brands = null, IList<int> distributors = null)
        {
            using(var m = Model.CreateModel())
            {
                var dealers = m.DealersAllBrandsNewExports.AsExpandable().ToList();
                var dealerstest = dealers.Where(d=>d.user_id== 11216);
                if (distributors != null)
                {
                    return
                        dealers.Where(
                            d => d.distributors.Split(',').Select(int.Parse).Intersect(distributors).Any())
                            .ToList();
                }
                return dealers;
            }
        }

        public static List<Dealer> GetDealersWithStatuses(IList<int> statuses)
        {
            using (var m = Model.CreateModel())
            {
                var countries = new[] {"UK", "GB", "IE"};
                var dealers = m.Dealers.Include(d=>d.Distributors).Include("Dealer_Images.Displays").Where(d => statuses.Contains(d.hide_1.Value) && (d.user_country == null || d.user_country == "" || countries.Contains(d.user_country)) ).ToList();
                //foreach (var d in dealers)
                //{
                //    foreach (var im in d.Dealer_Images)
                //    {
                //        m.Entry(im).Collection(i=>i.Displays).Load();
                //    }
                //}
                return dealers;
            }
        }

		public override void Insert(Dealer entity)
		{
			entity.user_id = dbSet.Max(d => d.user_id) + 1;
			base.Insert(entity);
		}

	}
}
