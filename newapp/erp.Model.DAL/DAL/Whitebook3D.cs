using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace erp.Model.DAL
{
    public class Whitebook3D_DAL
    {


        public static List<Cust_product_files> GetForComponent(int cprod_id)
        {
            var result = new List<Cust_product_files>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand(@"SELECT
                            cust_product_files.*,
                            cust_products_file_type.*
                            FROM
                            cust_product_files
                            INNER JOIN cust_products_file_type ON cust_product_files.file_type = cust_products_file_type.file_type_id
                            WHERE
                            cust_product_files.cprod_id = @cprod_id
                            ", conn);
                cmd.Parameters.AddWithValue("@cprod_id",cprod_id);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add( GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
        }

        private static Cust_product_files GetFromDataReader(MySqlDataReader dr)
        {
            var o = new Cust_product_files();
            o.id = (int)dr["id"];
            o.cprod_id = (int)dr["cprod_id"];
            o.file_name = string.Empty + Utilities.GetReaderField(dr, "file_name");
            o.name = string.Empty + Utilities.GetReaderField(dr, "name");
            o.path= string.Empty + Utilities.GetReaderField(dr, "path");

            return o;
        }
    }
}
