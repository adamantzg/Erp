using System.Collections.Generic;
using System.Linq;
using erp.Model;

namespace erp.DAL.EF
{
	public class ExternalDealerRepository
    {
        public static List<Dealer_external> GetAll()
        {
            using (var m = Model.CreateModel())
            {
                return m.ExternalDealers.Include("Brands").ToList();
            }
        }

        public static Dealer_external GetById(int id)
        {
            using (var m = Model.CreateModel())
            {
                return m.ExternalDealers.Include("Comments").Include("Comments.User").Include("Displays").
                    Include("Displays.WebProduct").Include("Displays.WebProduct.WebSite").Include("Brands").
                    Include("Displays.WebProduct.SelectedCategories").FirstOrDefault(d => d.id == id);
            }
        }

        public static List<Dealer_external> GetDealersByCriteria(string prefixText)
        {
            using (var m = Model.CreateModel())
            {
                return m.ExternalDealers.Where(d=>d.user_name.StartsWith(prefixText) || d.user_email.StartsWith(prefixText) || d.postcode.StartsWith(prefixText)).ToList();
            }
        }

        public static List<Dealer_external> GetDealersInRadius(Dealer_external d, double radius, bool? displaying = null)
        {
            var result = new List<Dealer_external>();
            var dealers = GetAll();
            foreach (var dealer in dealers)
            {
                if (dealer != null && dealer.latitude != null && dealer.longitude != null && dealer.id != d.id &&
                        GeoUtils.distance(dealer.latitude.Value, dealer.longitude.Value, d.latitude.Value,
                                            d.longitude.Value, 'M') <= radius)
                {
                    result.Add(dealer);
                }    
            }
            return result;

        }

        //public static List<Dealer_external> GetNearestDealers(double lat, double lon, double? distance = null)
        //{
        //    using (var m = Model.CreateModel())
        //    {
        //        m.Configuration.LazyLoadingEnabled = true;
        //        m.Configuration.ProxyCreationEnabled = true;

        //        var dealers = distance != null
        //            ? m.Database.SqlQuery<Dealer_external_Ex>(
        //                string.Format(
        //                    @"SELECT dealer_external.*, fn_Distance({0},{1},latitude,longitude) AS Distance FROM dealer_external WHERE fn_Distance({0},{1},latitude,longitude) <= {2}",
        //                    lat, lon, distance)).ToList()
        //            : m.Database.SqlQuery<Dealer_external_Ex>(
        //                string.Format(
        //                    @"SELECT dealer_external.*, fn_Distance({0},{1},latitude,longitude) AS Distance FROM dealer_external",
        //                    lat, lon)).ToList();



        //        var result = new List<Dealer_external>();
        //        foreach (var d in dealers)
        //        {
        //            if (d.latitude != null && d.longitude != null)
        //            {
        //                d.Distance = GeoUtils.distance(d.latitude.Value, d.longitude.Value, lat, lon, 'M');
        //            }
        //            else
        //            {
        //                d.Distance = double.MaxValue;
        //            }
        //            m.ExternalDealers.Attach(d);
        //            m.Entry(d).Collection("Brands").Load();
        //        }
        //        return dealers;
        //    }

        //}

        public static void Update(Dealer_external d)
        {
            using (var m = Model.CreateModel())
            {
                //var oldDealer = m.ExternalDealers.Include("Brands").FirstOrDefault(de => de.id == d.id);
                //m.Entry(oldDealer).State = EntityState.Detached;
                //foreach (var b in oldDealer.Brands)
                //{
                //    m.Entry(b).State = EntityState.Detached;
                //}
                //var dealer = m.ExternalDealers.Attach(d);
                //m.Entry(d).State = EntityState.Modified;
                //foreach (var brand in oldDealer.Brands.Where(brand => dealer.Brands.Count(b => b.id == brand.id) == 0))
                //{
                //    //Deleted
                //    dealer.Brands.Add(new Brand_external{id=brand.id});
                //    m.Entry(dealer.Brands.Last()).State = EntityState.Deleted;
                //}
                //foreach (var brand in dealer.Brands.Where(brand=> oldDealer.Brands.Count(b=>b.id == brand.id) == 0))
                //{
                //    //New
                //    m.Entry(brand).State = EntityState.Added;
                //}

                var dealer = m.ExternalDealers.Include("Brands").FirstOrDefault(de => de.id == d.id);
                if (dealer != null)
                {
                    dealer.annual_turnover_range = d.annual_turnover_range;
                    dealer.code = d.code;
                    dealer.customer_type = d.customer_type;
                    dealer.dealer_type = d.dealer_type;
                    dealer.latitude = d.latitude;
                    dealer.longitude = d.longitude;
                    dealer.postcode = d.postcode;
                    dealer.sales_rep_id = d.sales_rep_id;
                    dealer.sqfeetrange = d.sqfeetrange;
                    dealer.user_address1 = d.user_address1;
                    dealer.user_address2 = d.user_address2;
                    dealer.user_address3 = d.user_address3;
                    dealer.user_address4 = d.user_address4;
                    dealer.user_contact = d.user_contact;
                    dealer.user_email = d.user_email;
                    dealer.user_name = d.user_name;
                    dealer.user_tel = d.user_tel;
                    dealer.user_website = d.user_website;
                    var deleted = new List<Brand_external>();
                    foreach (var brand in dealer.Brands.Where(brand => d.Brands.Count(b => b.id == brand.id) == 0))
                    {
                        //Deleted
                        deleted.Add(brand);
                    }
                    foreach (var b in deleted)
                    {
                        dealer.Brands.Remove(b);
                    }
                    foreach (var brand in d.Brands.Where(brand => dealer.Brands.Count(b => b.id == brand.id) == 0))
                    {
                        //New
                        m.ExternalBrands.Attach(brand);
                        dealer.Brands.Add(brand);
                    }
                    m.SaveChanges();    
                }
                
                
            }
        }


        public static void CreateComment(Dealer_external_comment c)
        {
            using (var m = Model.CreateModel())
            {
                var dealer = m.ExternalDealers.Include("Comments").FirstOrDefault(d => d.id == c.dealer_id);
                if (dealer != null)
                {
                    dealer.Comments.Add(c);
                    m.SaveChanges();

                }
            }
        }
    }
}
