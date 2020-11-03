using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using erp.Model;

namespace erp.Model.DAL
{
    public class Product_technical
    {
        public static List<product_specification_type> GetAll()
        {
            var result = new List<product_specification_type>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM product_specification_types", conn);

                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();

            }
            //result.Add(new Product_specification_type { spec_unique = 1, spec_desc = "ovo je prvi" });
            return result;
        }

        public static List<Technical_data_type> TechnicalDataTypeGetAll()
        {
            var result = new List<Technical_data_type>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM technical_data_type", conn);

                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReaderTDT(dr));
                }
                dr.Close();

            }
            //result.Add(new Product_specification_type { spec_unique = 1, spec_desc = "ovo je prvi" });
            return result;
        }

       //Product_specificationsDAL.GetAll();

        public static List<Technical_product_data> TechnicalProductDataGetAll()
        {
            var result = new List<Technical_product_data>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM technical_product_data INNER JOIN technical_data_type ON technical_product_data.technical_data_type = technical_data_type.data_type_id", conn);

                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Technical_product_data tpd = GetFromDataReaderTechnical(dr);
                    tpd.TechnicalDataType = GetFromDataReaderTDT(dr);
                    result.Add(tpd);
                
                }
                dr.Close();

            }
            //result.Add(new Product_specification_type { spec_unique = 1, spec_desc = "ovo je prvi" });
            return result;
        }
        public static List<Technical_product_data> TechnicalProductDataGetByProduct(int mast_id)
        {
            var result = new List<Technical_product_data>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand(@"SELECT * 
                                             FROM technical_product_data INNER JOIN technical_data_type ON technical_product_data.technical_data_type = technical_data_type.data_type_id 
                                             WHERE technical_product_data.mast_id = @mast_id", conn);
                cmd.Parameters.AddWithValue("@mast_id",mast_id);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Technical_product_data tpd = GetFromDataReaderTechnical(dr);
                    tpd.TechnicalDataType = GetFromDataReaderTDT(dr);
                    result.Add(tpd);

                }
                dr.Close();

            }
            //result.Add(new Product_specification_type { spec_unique = 1, spec_desc = "ovo je prvi" });
            return result;
        }

        public static void Create(Technical_product_data o)
        {
            string insertsql = @"INSERT INTO technical_product_data (mast_id,technical_data_type,technical_data) VALUES(@mast_id,@technical_data_type,@technical_data)";
            using (var conn=new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand(insertsql, conn);
                BuildSqlParameters(cmd, o, true);
                cmd.ExecuteNonQuery();
            }
        }

        public static void Update(Technical_product_data o)
        {
            string updatesql = @"UPDATE technical_product_data SET mast_id=@mast_id,technical_data_type=@technical_data_type,technical_data=@technical_data  WHERE unique_id=@unique_id";
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand(updatesql, conn);
                BuildSqlParameters(cmd, o, false);
                cmd.ExecuteNonQuery();
            }
        }

        private static void BuildSqlParameters(MySqlCommand cmd, Technical_product_data o, bool p)
        {
            if (!p)
                cmd.Parameters.AddWithValue("@unique_id", o.unique_id);
            cmd.Parameters.AddWithValue("@mast_id", o.mast_id);
            cmd.Parameters.AddWithValue("@technical_data_type", o.technical_data_type);
            cmd.Parameters.AddWithValue("@technical_data", o.technical_data);
        }

        
        public static void DeleteTechnical(int id)
        {
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("DELETE FROM technical_product_data  WHERE unique_id = @id", conn);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }
        }
        


        private static product_specification_type GetFromDataReader(MySqlDataReader dr)
        {
            product_specification_type o = new product_specification_type();
            o.spec_unique = (int)dr["spec_unique"];
            o.spec_desc = string.Empty + Utilities.GetReaderField(dr, "spec_desc");
            return o;
        }
        private static Technical_data_type GetFromDataReaderTDT(MySqlDataReader dr)
        {
            Technical_data_type o = new Technical_data_type();
            o.data_type_id = (int)dr["data_type_id"];
            o.data_type_desc = string.Empty + Utilities.GetReaderField(dr, "data_type_desc");
            return o;
        }
        private static Technical_product_data GetFromDataReaderTechnical(MySqlDataReader dr)
        {
            Technical_product_data o = new Technical_product_data();
            o.unique_id = (int)dr["unique_id"];
            o.mast_id=(int)dr["mast_id"];
            o.technical_data_type=(int)dr["technical_data_type"];
            o.technical_data = string.Empty + Utilities.GetReaderField(dr, "technical_data");
            return o;
           
        }



        public static void CreateTechnicalDataType(Technical_data_type tdt)
        {
            string insertsql = @"INSERT INTO technical_data_type (data_type_desc) VALUES(@data_type_desc)";
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand(insertsql, conn);
                cmd.Parameters.AddWithValue("@data_type_desc", tdt.data_type_desc);
                cmd.ExecuteNonQuery();
            }
        }
    }
    

   

    //public class Technical_product_data
    //{
    //    public int unique_id { get; set; }
    //    public int mast_id { get; set; }
    //    public int technical_data_type { get; set; }
    //    public string technical_data { get; set; }
    //    public virtual Technical_data_type TechnicalDataType{get;set;}
    //}
    
    
}
