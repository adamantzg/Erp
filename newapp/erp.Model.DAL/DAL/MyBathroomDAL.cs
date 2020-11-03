using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace erp.Model.DAL
{
    public class MyBathroomDAL
    {
        public static List<MyBathroomEntry> GetEntries(long userid, string web_site)
        {
            var result = new List<MyBathroomEntry>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM web_mybathroom WHERE user_id = @userid AND web_site = @website", conn);
                cmd.Parameters.Add(new MySqlParameter("@userid", userid));
                cmd.Parameters.AddWithValue("@website", web_site);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    var entry = new MyBathroomEntry();
                    entry.productid = (int)dr["product_id"];
                    entry.quantity = Utilities.FromDbValue<double>(dr["quantity"]);
                    result.Add(entry);
                }
                dr.Close();
            }
            return result;
        }

        public static void SaveEntries(List<MyBathroomEntry> entries, long userid, string web_site)
        {
            MySqlTransaction trn = null;
            MySqlConnection conn = null;
            try
            {
                conn = new MySqlConnection(Properties.Settings.Default.ConnString);
                conn.Open();
                trn = conn.BeginTransaction();

                //Delete old entries
                MySqlCommand cmd = Utils.GetCommand("DELETE from web_mybathroom WHERE user_id = @userid AND web_site = @website", conn);
                cmd.Parameters.AddWithValue("@website", web_site);
                cmd.Parameters.Add(new MySqlParameter("@userid", userid));
                cmd.ExecuteNonQuery();

                cmd.CommandText = "INSERT INTO web_mybathroom (user_id, product_id, quantity, web_site) VALUES(@userid, @productid, @quantity,@website)";
                cmd.Parameters.Add(new MySqlParameter("@productid", 0));
                cmd.Parameters.Add(new MySqlParameter("@quantity", 0));

                foreach (var entry in entries)
                {
                    cmd.Parameters["@productid"].Value = entry.productid;
                    cmd.Parameters["@quantity"].Value = entry.quantity;
                    cmd.ExecuteNonQuery();
                }
                trn.Commit();

            }
            catch (Exception)
            {
                if (trn != null)
                    trn.Rollback();
            }
            finally
            {
                conn = null;
            }
        }
    }
}
