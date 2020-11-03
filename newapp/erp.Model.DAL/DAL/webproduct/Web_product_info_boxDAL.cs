using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using MySql.Data;
using erp.Model;
using MySql.Data.MySqlClient;


namespace erp.Model.DAL
{
    public class Web_product_info_boxDAL
    {
        public static Web_product_info_box GetForProductByInfoType(int web_unique, int info_type)
        {
            Web_product_info_box result = null;
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM web_product_info_box WHERE web_unique=@id AND info_type=@info_type", conn);
                cmd.Parameters.AddWithValue("@id",web_unique);
                cmd.Parameters.AddWithValue("@info_type", info_type);
                var dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    result = GetFromDataReader(dr);
                }

            }
            return result;
        }

        public static List<Web_product_info_box> GetForProduct(int web_unique, IDbConnection conn = null)
        {
            var results = new List<Web_product_info_box>();
            bool dispose = false;
            if (conn == null)
            {
                conn = new MySqlConnection(Properties.Settings.Default.ConnString);
                conn.Open();
                dispose = true;
            }
            //using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                
                var cmd = Utils.GetCommand("SELECT * FROM web_product_info_box WHERE web_unique=@id", (MySqlConnection) conn);
                cmd.Parameters.AddWithValue("@id", web_unique);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    var infobox = GetFromDataReader(dr);
                    if (infobox != null)
                        results.Add(infobox);
                }
                dr.Close();
                if(dispose)
                    conn.Dispose();
            }
            return results;
        }

        private static Web_product_info_box GetFromDataReader(MySqlDataReader dr)
        {
            Web_product_info_box o = new Web_product_info_box();
            o.id = (int)dr["id"];
            o.web_unique = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "web_unique"));
            o.info_type = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "info_type"));
            return o;
        }
    }
}
