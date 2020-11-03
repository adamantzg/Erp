using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using erp.Model;
using MySql.Data.MySqlClient;
using System.Data;
using System.Collections.ObjectModel;

namespace erp.Model.DAL
{
    public class web_product_new_relatedDAL
    {
        public static List<web_product_new_related> GetAll()
        {
            var result = new List<web_product_new_related>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM web_product_new_related", conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;

            

        }


        public static ICollection<web_product_new_related> GetForProduct(int web_unique,int relation_type=2,bool web_product_new=false,bool web_files=false, IDbConnection conn=null)
        {
            var result = new Collection<web_product_new_related>();
            bool dispose = false;
            if (conn == null)
            {
                conn = new MySqlConnection(Properties.Settings.Default.ConnString);
                conn.Open();
                dispose = true;
            }
            try
            {
                var cmd = Utils.GetCommand(@"SELECT web_product_new_related.* FROM web_product_new_related 
                                            WHERE web_product_new_related.web_unique = @web_unique  
                                            AND web_product_new_related.relation_type=@relation_type",(MySqlConnection)conn);
                cmd.Parameters.AddWithValue("@web_unique", web_unique);
                cmd.Parameters.AddWithValue("@relation_type", relation_type);
                
                var dr=cmd.ExecuteReader();
                while (dr.Read())
                {
                    var r = GetFromDataReader(dr);
                    result.Add(r);
                }
                dr.Close();
                foreach(var r in result)
                {
                    if (web_product_new)
                        r.Web_product_new = Web_product_newDAL.GetByIdEx(r.web_unique_related);
                    //if (web_files)
                    //    r.WebFiles = Web_product_fileDAL.GetForProduct(r.web_unique_related,conn:conn);
                }
            }
            finally
            {

                if (dispose)
                    conn.Dispose();
            }
            return result;


        }




        public static web_product_new_related GetFromDataReader(MySqlDataReader dr)
        {
            web_product_new_related o = new web_product_new_related();

            o.web_unique = (int)dr["web_unique"];
            o.web_unique_related = (int)dr["web_unique_related"];
            o.relation_type = Utilities.FromDbValue<int>(dr["relation_type"]);
            //o.order = Utilities.FromDbValue<int>(dr["order"]);
           // o.Component = Cust_productsDAL.GetFromDataReader(dr);

            return o;

        }
        private static void BuildSqlParameters(MySqlCommand cmd, web_product_new_related o, bool forInsert = true)
        {
            cmd.Parameters.AddWithValue("@web_unique", o.web_unique);
            cmd.Parameters.AddWithValue("@web_unique_related", o.web_unique_related);
            cmd.Parameters.AddWithValue("@relation_type", o.relation_type);
        }

        /*??????*/
        public static void Update(web_product_new_related o)
        {
            string updatesql = @"UPDATE web_product_new_related SET relation_type = @relation_type,order = @order WHERE cprod_id = @cprod_id AND web_unique = @web_unique";

            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand(updatesql, conn);
                BuildSqlParameters(cmd, o, false);
                cmd.ExecuteNonQuery();
            }
        }
        public static void Insert(web_product_new_related o)
        {
            string insertsql =string.Format( @"INSERT INTO web_product_new_related(web_unique, web_unique_related,relation_type) VALUES(@web_unique,@web_unique_related,{0})",(int)RelationType.Complementary);
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand(insertsql, conn);
                BuildSqlParameters(cmd, o, false);
                cmd.ExecuteNonQuery();
            }
        }
        public static void Delete(int web_unique_related, int web_unique)
        {
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("DELETE FROM web_product_new_related WHERE web_unique_related = @web_unique_related AND web_unique = @web_unique", conn);
                cmd.Parameters.AddWithValue("@web_unique_related", web_unique_related);
                cmd.Parameters.AddWithValue("@web_unique", web_unique);
                cmd.ExecuteNonQuery();
            }
        }
    }
}
