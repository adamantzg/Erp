using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace asaq2.Model.DAL
{
    public partial class DAM_shortcutsDAL
    {
        public static List<DAM_shortcuts> GetAll()
        {
            var result = new List<DAM_shortcuts>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM dam_shortcuts", conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
        }


        public static DAM_shortcuts GetById(int id)
        {
            DAM_shortcuts result = null;
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM dam_shortcuts WHERE shortcut_id = @shortcut_id", conn);
                cmd.Parameters.AddWithValue("@shortcut_id", id);
                var dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    result = GetFromDataReader(dr);
                }
                dr.Close();
            }
            return result;
        }


        public static DAM_shortcuts GetFromDataReader(MySqlDataReader dr)
        {
            DAM_shortcuts o = new DAM_shortcuts();

            o.shortcut_id = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "shortcut_id"));
            o.shortcut_brand = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "shortcut_brand"));
            o.shortcut_category = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "shortcut_category"));
            o.password = string.Empty + Utilities.GetReaderField(dr, "password");
            o.shortcut_description = string.Empty + dr["shortcut_description"];

            return o;

        }

        public static void Create(DAM_shortcuts o)
        {
            string insertsql = @"INSERT INTO dam_shortcuts (shortcut_id,shortcut_brand,shortcut_category,shortcut_description,password) VALUES(@shortcut_id,@shortcut_brand,@shortcut_category,@shortcut_description,@password)";

            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();

                var cmd = Utils.GetCommand(insertsql, conn);
                BuildSqlParameters(cmd, o);
                cmd.ExecuteNonQuery();
                cmd.CommandText = "SELECT shortcut_id FROM dam_shortcuts WHERE shortcut_id = LAST_INSERT_ID()";
                o.shortcut_id = (int)cmd.ExecuteScalar();

            }
        }

        private static void BuildSqlParameters(MySqlCommand cmd, DAM_shortcuts o)
        {
            cmd.Parameters.AddWithValue("@shortcut_id", o.shortcut_id);
            cmd.Parameters.AddWithValue("@shortcut_brand", o.shortcut_brand);
            cmd.Parameters.AddWithValue("@shortcut_category", o.shortcut_category);
            cmd.Parameters.AddWithValue("@shortcut_description", o.shortcut_description);
            cmd.Parameters.AddWithValue("@password", o.password);
        }

        public static void Update(DAM_shortcuts o)
        {
            string updatesql = @"UPDATE dam_shortcuts SET shortcut_brand=@shortcut_brand, shortcut_category=@shortcut_category, shortcut_description=@shortcut_description, password=@password WHERE shortcut_id = @shortcut_id";

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
                var cmd = Utils.GetCommand("DELETE FROM dam_shortcuts WHERE shortcut_id = @shortcut_id", conn);
                cmd.Parameters.AddWithValue("@shortcut_id", id);
                cmd.ExecuteNonQuery();
            }
        }

        public static DAM_shortcuts GetByBrandAndCategoryId(int brandid, int? categoryid)
        {
            DAM_shortcuts result = null;
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var query = "SELECT * FROM dam_shortcuts WHERE shortcut_brand = @shortcut_brand {0}";
                query = string.Format(query, categoryid != null ? "AND shortcut_category=@shortcut_category" : "");

                var cmd = Utils.GetCommand(query, conn);
                if(categoryid != null)
                    cmd.Parameters.AddWithValue("@shortcut_category", categoryid);
                cmd.Parameters.AddWithValue("@shortcut_brand", brandid);
                
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


