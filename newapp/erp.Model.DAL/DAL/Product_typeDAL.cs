using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MySql.Data;
using erp.Model;
using MySql.Data.MySqlClient;

namespace erp.Model.DAL
{
    public class Product_typeDAL
    {
        public static Product_type GetById(int id)
        {
            Product_type result = null;

            using(var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = Utils.GetCommand("SELECT * FROM product_type WHERE product_type_id = @id", conn);
                cmd.Parameters.AddWithValue("@id",id);
                MySqlDataReader dr = cmd.ExecuteReader();
                if(dr.Read())
                {

                    result = GetFromDataReader(dr);
                }
                dr.Close();
            }
            return result;
        }

        public static Product_type GetFromDataReader(MySqlDataReader dr)
        {
            Product_type o = new Product_type();

            o.id = (int)dr["product_type_id"];
            o.product_type = string.Empty+Utilities.GetReaderField(dr,"product_type_desc");
            return o;
        }
    }
}
