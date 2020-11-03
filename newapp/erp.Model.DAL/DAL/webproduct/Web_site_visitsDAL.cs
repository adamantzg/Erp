using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace erp.Model.DAL
{
    public partial class Web_site_visitsDAL
    {
        public static List<Web_site_visits> GetAll()
        {
            var result = new List<Web_site_visits>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM web_site_visits ORDER BY visit_date DESC LIMIT 100", conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
        }


        public static Web_site_visits GetById(int id)
        {
            Web_site_visits result = null;
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM web_site_visits WHERE id = @id", conn);
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


        public static Web_site_visits GetFromDataReader(MySqlDataReader dr)
        {
            Web_site_visits o = new Web_site_visits();

            o.id = (int)dr["id"];
            o.visit_country = string.Empty + dr["visit_country"];
            o.visit_date = Utilities.FromDbValue<DateTime>(dr["visit_date"]);
            o.web_unique = Utilities.FromDbValue<int>(dr["web_unique"]);
            o.visit_IP = string.Empty + dr["visit_IP"];
            o.visit_site = Utilities.FromDbValue<int>(dr["visit_site"]);

            return o;

        }

        public static void Create(Web_site_visits o)
        {
            string insertsql = @"INSERT INTO web_site_visits (web_unique,visit_IP,visit_date,visit_country,visit_site) VALUES(@web_unique,@visit_IP,@visit_date,@visit_country,@visit_site)";

            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();

                var cmd = Utils.GetCommand(insertsql, conn);
                BuildSqlParameters(cmd, o);
                cmd.ExecuteNonQuery();
                cmd.CommandText = "SELECT id FROM web_site_visits WHERE id = LAST_INSERT_ID()";
                o.id = (int)cmd.ExecuteScalar();

            }
        }

        private static void BuildSqlParameters(MySqlCommand cmd, Web_site_visits o, bool forInsert = true)
        {

            if (!forInsert)
                cmd.Parameters.AddWithValue("@id", o.id);
            cmd.Parameters.AddWithValue("@web_unique", o.web_unique);
            cmd.Parameters.AddWithValue("@visit_IP", o.visit_IP);
            cmd.Parameters.AddWithValue("@visit_date", o.visit_date);
            cmd.Parameters.AddWithValue("@visit_country", o.visit_country);
            cmd.Parameters.AddWithValue("@visit_site", o.visit_site);
        }

        public static void Update(Web_site_visits o)
        {
            string updatesql = @"UPDATE web_site_visits SET web_unique = @web_unique, visit_IP=@visit_IP, visit_date=@visit_date, visit_country=@visit_country, visit_site=@visit_site WHERE id = @id";

            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand(updatesql, conn);
                BuildSqlParameters(cmd, o, false);
                cmd.ExecuteNonQuery();
            }
        }

        public static void Delete(int id)
        {
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("DELETE FROM web_site_visits WHERE id = @id", conn);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }
        }

        public static List<Web_site_visits> GetTopProduct(int? limit = null, string from = null, string to = null, int? site_id = null)
        {
            var results = new List<Web_site_visits>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var query = @"SELECT *, COUNT(*) as count from web_site_visits {1} {2}
                                              GROUP BY web_unique
                                              ORDER BY visit_date DESC, count(*) DESC {0}";

                query = string.Format(query, (limit != null && limit > 0 ? "LIMIT " + limit : ""), from != null ? "WHERE visit_date >=@datefrom" + (to != null ? " AND visit_date<=@dateto" : "") : to != null ? "WHERE visit_date<=@dateto" : "", site_id != null && (from != null || to != null) ? " AND visit_site=@site_id" : site_id != null ? " WHERE visit_site=@site_id" : "");

                var cmd = Utils.GetCommand(query, conn);
                if (from != null)
                    cmd.Parameters.AddWithValue("@datefrom", from);
                if (to != null)
                    cmd.Parameters.AddWithValue("@dateto", to);
                if (site_id != null)
                    cmd.Parameters.AddWithValue("@site_id", site_id);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    results.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return results;
        }

        public static List<Web_site_visits> GetTop10ProductsByBrand(int site_id, string from = null, string to = null, bool uniquevisits = false)
        {
            var result = new List<Web_site_visits>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var query = @"SELECT *, COUNT(*) as count from web_site_visits WHERE visit_site = @site_id {0}
                                            GROUP BY web_unique {1}
                                            ORDER BY count DESC LIMIT 10";
                query = string.Format(query, (from != null ? "AND visit_date >=@datefrom" + (to != null ? " AND visit_date<=@dateto" : "") : to != null ? "AND visit_date<=@dateto" : ""), uniquevisits ? ",visit_IP" : "");
                var cmd = Utils.GetCommand(query, conn);
                if (from != null)
                    cmd.Parameters.AddWithValue("@datefrom", from);
                if (to != null)
                    cmd.Parameters.AddWithValue("@dateto", to);
                cmd.Parameters.AddWithValue("@site_id", site_id);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    var visit = GetFromDataReader(dr);
                    visit.Count = Utilities.FromDbValue<int>(dr["count"]);
                    result.Add(visit);
                }
                dr.Close();
            }
            return result;
        }


        public static List<string> GetCountriesByProduct(int web_unique, int site_id)
        {
            var result = new List<string>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand(@"SELECT visit_country,COUNT(*) AS count FROM web_site_visits 
                                            WHERE web_unique = @web_unique
                                            GROUP by visit_country
                                            ORDER BY count DESC", conn);
                cmd.Parameters.AddWithValue("@web_unique", web_unique);
                cmd.Parameters.AddWithValue("@site_id", site_id);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add((string)dr["visit_country"]);
                }
                dr.Close();
            }
            return result;
        }

        public static List<ProductCountByCountry> GetCountriesVisitCountByProduct(int web_unique, string from = null, string to = null)
        {
            var result = new List<ProductCountByCountry>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var query = @"SELECT visit_country,COUNT(*) AS count FROM web_site_visits 
                                            WHERE web_unique = @web_unique {0}
                                            GROUP BY visit_country
                                            ORDER BY count DESC";
                query = string.Format(query, (from != null ? "AND visit_date >=@datefrom" + (to != null ? " AND visit_date<=@dateto" : "") : to != null ? "AND visit_date<=@dateto" : ""));
                var cmd = Utils.GetCommand(query, conn);
                if (from != null)
                    cmd.Parameters.AddWithValue("@datefrom", from);
                if (to != null)
                    cmd.Parameters.AddWithValue("@dateto", to);
                cmd.Parameters.AddWithValue("@web_unique", web_unique);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    var r = new ProductCountByCountry();
                    r.visit_country = (string)dr["visit_country"];
                    r.count = Convert.ToInt32(dr["count"]);
                    result.Add(r);
                }
                dr.Close();
            }
            return result;
        }
    }
}


