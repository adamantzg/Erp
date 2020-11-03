using asaq2.Model;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace asaq2.Model.DAL 
{
    public partial class DAM_brand_visitsDAL
    {
        public static List<dam_brand_visits> GetAll()
        {
            var result = new List<dam_brand_visits>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM dam_brand_visits", conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
        }


        public static dam_brand_visits GetById(int id)
        {
            dam_brand_visits result = null;
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM dam_brand_visits WHERE id = @id", conn);
                cmd.Parameters.AddWithValue("@id", id);
                var dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    result = GetFromDataReader(dr);
                }
                dr.Close();
            }
            return result;
        }


        public static dam_brand_visits GetFromDataReader(MySqlDataReader dr)
        {
            dam_brand_visits o = new dam_brand_visits();

            o.id = (int)dr["id"];
            o.date = (DateTime)dr["date"];
            o.brand_id = (int)dr["brand_id"];
            o.session = string.Empty + dr["session"];
            o.ip_address = string.Empty + dr["ip_address"];
            o.register = (int)dr["register"];

            return o;

        }

        public static void Create(dam_brand_visits o)
        {
            string insertsql = @"INSERT INTO dam_brand_visits (brand_id,date,register,session,ip_address) VALUES(@brand_id,@date,@register,@session,@ip_address)";

            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();

                var cmd = Utils.GetCommand(insertsql, conn);
                BuildSqlParameters(cmd, o);
                cmd.ExecuteNonQuery();
                cmd.CommandText = "SELECT id FROM dam_brand_visits WHERE id = LAST_INSERT_ID()";
                o.id = (int)cmd.ExecuteScalar();

            }
        }

        private static void BuildSqlParameters(MySqlCommand cmd, dam_brand_visits o)
        {
            cmd.Parameters.AddWithValue("@id", o.id);
            cmd.Parameters.AddWithValue("@brand_id", o.brand_id);
            cmd.Parameters.AddWithValue("@date", o.date);
            cmd.Parameters.AddWithValue("@register", o.register);
            cmd.Parameters.AddWithValue("@session", o.session);
            cmd.Parameters.AddWithValue("@ip_address", o.ip_address);
        }

        public static void Update(dam_brand_visits o)
        {
            string updatesql = @"UPDATE dam_brand_visits SET brand_id = @brand_id, date=@date, register=@register, session=@session, ip_address=@ip_address WHERE id = @id";

            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand(updatesql, conn);
                BuildSqlParameters(cmd, o);
                cmd.ExecuteNonQuery();
            }
        }

        public static void Delete(int id)
        {
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("DELETE FROM dam_brand_visits WHERE id = @id", conn);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }
        }

        public static List<dam_brand_visits> GetByBrand(int brandid)
        {
            var result = new List<dam_brand_visits>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM dam_brand_visits WHERE brand_id=@brandid", conn);
                cmd.Parameters.AddWithValue("@brandid", brandid);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
        }

        public static dam_brand_visits GetBySession(string session)
        {
            dam_brand_visits result = null;
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM dam_brand_visits WHERE session = @session", conn);
                cmd.Parameters.AddWithValue("@session", session);
                var dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    result = GetFromDataReader(dr);
                }
                dr.Close();
            }
            return result;
        }
    }
}


