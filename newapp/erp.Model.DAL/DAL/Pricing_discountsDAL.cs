using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace erp.Model.DAL
{
    public class Pricing_discountsDAL
    {
        public static List<Pricing_Discounts> GetAll()
        {
            List<Pricing_Discounts> result = new List<Pricing_Discounts>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = Utils.GetCommand("SELECT * FROM pricing_discounts", conn);
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
        }

        public static Pricing_Discounts GetById(int id)
        {
            Pricing_Discounts result = null;
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = Utils.GetCommand("SELECT * FROM pricing_discounts WHERE discount_id = @id", conn);
                cmd.Parameters.AddWithValue("@id", id);
                MySqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    result = GetFromDataReader(dr);
                }
                dr.Close();
            }
            return result;

        }

        private static Pricing_Discounts GetFromDataReader(MySqlDataReader dr)
        {
            Pricing_Discounts o = new Pricing_Discounts();

            o.discount_id = (int)dr["discount_id"];
            o.discount_name = string.Empty + dr["discount_name"];
            o.discount_status = Utilities.FromDbValue<int>(dr["discount_status"]);
            o.discount_ddp_cash_20 = Utilities.FromDbValue<double>(dr["discount_ddp_cash_20"]);
            o.discount_ddp_cash_40 = Utilities.FromDbValue<double>(dr["discount_ddp_cash_40"]);
            o.discount_ddp_credit_20 = Utilities.FromDbValue<double>(dr["discount_ddp_credit_20"]);
            o.discount_ddp_credit_40 = Utilities.FromDbValue<double>(dr["discount_ddp_credit_40"]);
            o.vat_rate = Utilities.FromDbValue<double>(dr["vat_rate"]);
            o.retailer_discount = Utilities.FromDbValue<double>(dr["retailer_discount"]);
            o.discount_brand = Utilities.FromDbValue<int>(dr["discount_brand"]);

            return o;
        }

    }
}
