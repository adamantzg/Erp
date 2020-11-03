using System;
using System.Collections.Generic;
using System.Linq;
using erp.Model;
using MySql.Data.MySqlClient;

namespace erp.Model.DAL
{
    public class Dist_productsDAL
    {
        public static List<Dist_products> GetAll()
        {
            var result = new List<Dist_products>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = Utils.GetCommand("SELECT * FROM dist_products", conn);
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
        }

        public static List<Dist_products> GetByUser(int userid)
        {
            var result = new List<Dist_products>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = Utils.GetCommand("SELECT * FROM dist_products WHERE client_id = @userid", conn);
                cmd.Parameters.AddWithValue("@userid",userid);
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
        }

        public static List<Dist_products> GetByIds(IList<int> ids )
        {
            var result = new List<Dist_products>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString)) {
                conn.Open();
                MySqlCommand cmd = Utils.GetCommand("", conn);
                cmd.CommandText = string.Format("SELECT * FROM dist_products WHERE dist_cprod_id IN ({0})",
                    Utils.CreateParametersFromIdList(cmd, ids));
                
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read()) {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
        }

        public static Dist_products GetById(int id)
        {
            Dist_products result = null;
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = Utils.GetCommand("SELECT * FROM dist_products WHERE distprod_id = @id", conn);
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

        private static Dist_products GetFromDataReader(MySqlDataReader dr)
        {
            Dist_products o = new Dist_products();

            o.distprod_id = (int)dr["distprod_id"];
            o.client_id = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "client_id"));
            o.dist_cprod_id = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "dist_cprod_id"));
            o.dist_opening_stock = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "dist_opening_stock"));
            o.dist_special_code = string.Empty + dr["dist_special_code"];
            o.dist_special_desc = string.Empty + dr["dist_special_desc"];
            o.dist_special_price = Utilities.FromDbValue<double>(Utilities.GetReaderField(dr, "dist_special_price"));
            o.dist_special_curr = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "dist_special_curr"));
            o.dist_special_moq = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "dist_special_moq"));
            o.dist_seq = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "dist_seq"));
            o.dist_spec_disc = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "dist_spec_dist"));
            o.dist_stock = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "dist_stock"));
            o.dist_stock_date = Utilities.FromDbValue<DateTime>(Utilities.GetReaderField(dr, "dist_stock_date"));
            o.client_system_code = string.Empty + dr["client_system_code"];
            o.dist_onorder = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "dist_onorder"));

            return o;

        }

        public static void Create(Dist_products o)
        {
            string insertsql = @"INSERT INTO dist_products (client_id,dist_cprod_id,dist_opening_stock,dist_special_code,dist_special_desc,dist_special_price,dist_special_curr,dist_special_moq,
                                dist_seq,dist_spec_disc,dist_stock,dist_stock_date,client_system_code,dist_onorder) VALUES(@client_id,@dist_cprod_id,@dist_opening_stock,@dist_special_code,
                                @dist_special_desc,@dist_special_price,@dist_special_curr,@dist_special_moq,@dist_seq,@dist_spec_disc,@dist_stock,@dist_stock_date,@client_system_code,@dist_onorder)";

            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();

                MySqlCommand cmd = Utils.GetCommand(insertsql, conn);
                BuildSqlParameters(cmd, o);
                cmd.ExecuteNonQuery();
                cmd.CommandText = "SELECT distprod_id FROM dist_products WHERE distprod_id = LAST_INSERT_ID()";
                o.distprod_id = (int)cmd.ExecuteScalar();

            }
        }

        private static void BuildSqlParameters(MySqlCommand cmd, Dist_products o, bool forInsert = true)
        {
            cmd.Parameters.AddWithValue("@client_id", o.client_id);
            cmd.Parameters.AddWithValue("@dist_cprod_id", o.dist_cprod_id);
            cmd.Parameters.AddWithValue("@dist_opening_stock", o.dist_opening_stock);
            cmd.Parameters.AddWithValue("@dist_special_code", o.dist_special_code);
            cmd.Parameters.AddWithValue("@dist_special_desc", o.dist_special_desc);
            cmd.Parameters.AddWithValue("@dist_special_price", o.dist_special_price);
            cmd.Parameters.AddWithValue("@dist_special_curr", o.dist_special_curr);
            cmd.Parameters.AddWithValue("@dist_special_moq", o.dist_special_moq);
            cmd.Parameters.AddWithValue("@dist_seq", o.dist_seq);
            cmd.Parameters.AddWithValue("@dist_spec_disc", o.dist_spec_disc);
            cmd.Parameters.AddWithValue("@dist_stock", o.dist_stock);
            cmd.Parameters.AddWithValue("@dist_stock_date", o.dist_stock_date);
            cmd.Parameters.AddWithValue("@client_system_code", o.client_system_code);
            cmd.Parameters.AddWithValue("@dist_onorder", o.dist_onorder);
            if (!forInsert)
                cmd.Parameters.AddWithValue("@distprod_id", o.distprod_id);
        }

        public static void Update(Dist_products o)
        {
            string updatesql = @"UPDATE dist_products SET client_id=@client_id,dist_cprod_id=@dist_cprod_id,dist_opening_stock=@dist_opening_stock,dist_special_code=@dist_special_code,
                                dist_special_desc=@dist_special_desc,dist_special_price=@dist_special_price,dist_special_curr=@dist_special_curr,dist_special_moq=@dist_special_moq,
                                dist_seq=@dist_seq,dist_spec_disc=@dist_spec_disc,dist_stock=@dist_stock,dist_stock_date=@dist_stock_date,client_system_code=@client_system_code,
                                dist_onorder=@dist_onorder  WHERE distprod_id = @distprod_id";

            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = Utils.GetCommand(updatesql, conn);
                BuildSqlParameters(cmd, o, false);
                cmd.ExecuteNonQuery();
            }
        }

        public static void Delete(int country_id)
        {
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = Utils.GetCommand("DELETE FROM dist_products WHERE distprod_id = @id", conn);
                cmd.Parameters.AddWithValue("@id", country_id);
                cmd.ExecuteNonQuery();
            }
        }

    }
}
