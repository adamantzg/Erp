using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace erp.Model.DAL
{
    public class ExternalDealerDAL
    {
        public static List<Dealer_external> GetNearestDealers(double lat, double lon, double? distance = null)
        {
            //using (var m = Model.CreateModel())
            using(var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                //m.Configuration.LazyLoadingEnabled = true;
                //m.Configuration.ProxyCreationEnabled = true;
                var sql = distance != null
                    ? string.Format(
                        @"SELECT dealer_external.*, fn_Distance({0},{1},latitude,longitude) AS Distance FROM dealer_external WHERE fn_Distance({0},{1},latitude,longitude) <= {2}",
                        lat, lon, distance)
                    : string.Format(
                        @"SELECT dealer_external.*, fn_Distance({0},{1},latitude,longitude) AS Distance FROM dealer_external",
                        lat, lon);
                var cmd = new MySqlCommand(sql, conn);
                //var dealers = distance != null
                //    ? m.Database.SqlQuery<Dealer_external_Ex>(
                //        string.Format(
                //            @"SELECT dealer_external.*, fn_Distance({0},{1},latitude,longitude) AS Distance FROM dealer_external WHERE fn_Distance({0},{1},latitude,longitude) <= {2}",
                //            lat, lon, distance)).ToList()
                //    : m.Database.SqlQuery<Dealer_external_Ex>(
                //        string.Format(
                //            @"SELECT dealer_external.*, fn_Distance({0},{1},latitude,longitude) AS Distance FROM dealer_external",
                //            lat, lon)).ToList();

                var dr = cmd.ExecuteReader();
                var dealers = new List<Dealer_external>();
                while (dr.Read())
                {
                    var d = GetFromDataReader(dr);
                    if (d.latitude != null && d.longitude != null)
                        d.Distance = Utilities.FromDbValue<double>(dr["Distance"]);
                    else
                    {
                        d.Distance = double.MaxValue;
                    }
                    dealers.Add(d);
                }

                return dealers;
            }

        }

        public static Dealer_external GetFromDataReader(MySqlDataReader dr)
        {
            Dealer_external o = new Dealer_external();

            o.id = (int)dr["id"];
            o.code = string.Empty + Utilities.GetReaderField(dr, "code");
            o.user_name = string.Empty + Utilities.GetReaderField(dr, "user_name");
            o.user_address1 = string.Empty + Utilities.GetReaderField(dr, "user_address1");
            o.user_address2 = string.Empty + Utilities.GetReaderField(dr, "user_address2");
            o.user_address3 = string.Empty + Utilities.GetReaderField(dr, "user_address3");
            o.user_address4 = string.Empty + Utilities.GetReaderField(dr, "user_address4");
            o.postcode = string.Empty + Utilities.GetReaderField(dr, "postcode");
            o.user_contact = string.Empty + Utilities.GetReaderField(dr, "user_contact");
            o.user_tel = string.Empty + Utilities.GetReaderField(dr, "user_tel");
            o.user_email = string.Empty + Utilities.GetReaderField(dr, "user_email");
            o.user_website = string.Empty + Utilities.GetReaderField(dr, "user_website");
            o.dealer_type = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "dealer_type"));
            o.sales_rep_id = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "sales_rep_id"));
            o.longitude = Utilities.FromDbValue<double>(Utilities.GetReaderField(dr, "longitude"));
            o.latitude = Utilities.FromDbValue<double>(Utilities.GetReaderField(dr, "latitude"));
            o.sqfeetrange = string.Empty + Utilities.GetReaderField(dr, "sqfeetrange");
            o.annual_turnover_range = string.Empty + Utilities.GetReaderField(dr, "annual_turnover_range");
            o.customer_type = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "customer_type"));
            //o.sales_rep = string.Empty + Utilities.GetReaderField(dr, "sales_rep");

            return o;

        }
    }
}
