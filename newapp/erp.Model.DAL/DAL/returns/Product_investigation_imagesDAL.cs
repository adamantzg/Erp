using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using erp.Model;
using MySql.Data.MySqlClient;

namespace erp.Model.DAL
{
    public  class Product_investigation_imagesDAL
    {
        public static List<Product_investigation_images> GetAll()
        {
            var result = new List<Product_investigation_images>();

            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM product_investigation_images", conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;

        }

        public static List<Product_investigation_images> GetForProduct(int cprodId)
        {
            var result = new List<Product_investigation_images>();

            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM product_investigation_images WHERE cprod_id= @cprodId", conn);
                cmd.Parameters.AddWithValue("@cprodId",cprodId);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;

        }

        public static List<Product_investigation_images> GetForInvestigation(int id)
        {
            var result=new List<Product_investigation_images>();

            using(var conn=new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand(@"SELECT * FROM product_investigation_images WHERE investigation_id=@investigation_id", conn);
                cmd.Parameters.AddWithValue("@investigation_id", id);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
        }
       


        public static void Create(Product_investigation_images o)
        {
            string insertsql = @"INSERT INTO product_investigation_images(name,cprod_id,investigation_id)
                                VALUES(@name,@cprod_id,@investigation_id)";
            var conn = new MySqlConnection(Properties.Settings.Default.ConnString);
            conn.Open();
            MySqlTransaction tr = conn.BeginTransaction();
            try
            {
                MySqlCommand cmd = Utils.GetCommand("SELECT MAX(id) + 1 FROM product_investigation_images", conn, tr);
                //o.id = Convert.ToInt32(cmd.ExecuteScalar());

                cmd.CommandText = insertsql;
                BuildSqlParameters(cmd, o);
                cmd.ExecuteNonQuery();

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

        public static void Delete(int id)
        {
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("DELETE FROM product_investigation_images WHERE id = @id", conn);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }
        }
        private static void BuildSqlParameters(MySqlCommand cmd, Product_investigation_images o)
        {
            cmd.Parameters.AddWithValue("@id", o.id);
            cmd.Parameters.AddWithValue("@name", o.image_name);
            cmd.Parameters.AddWithValue("@cprod_id", o.cprod_id);
            cmd.Parameters.AddWithValue("@investigation_id", o.investigation_id);
        }

        private static Product_investigation_images GetFromDataReader(MySqlDataReader dr)
        {
            Product_investigation_images o = new Product_investigation_images();
            o.id  = (int)dr["id"];
            o.image_name =  (string)dr["name"];
            o.cprod_id=(int)dr["cprod_id"];
            o.investigation_id = (int)dr["investigation_id"];

            return o;
        }
        
    }
}
