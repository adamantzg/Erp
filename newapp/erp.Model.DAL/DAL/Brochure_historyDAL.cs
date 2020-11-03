using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace erp.Model.DAL
{
    public class Brochure_historyDAL
    {
        private static Brochure_history GetFromDataReader(MySqlDataReader dr)
        {
            Brochure_history o = new Brochure_history();

            o.id = Convert.ToInt32(Utilities.FromDbValue<int>(dr["id"]));
            o.userid = Utilities.FromDbValue<int>(dr["userid"]);
            o.visit_date = Utilities.FromDbValue<DateTime>(dr["visit_date"]);
            o.page = string.Empty + dr["page"];
            o.pageURL = string.Empty + dr["pageURL"];

            return o;

        }

        public static int Create(Brochure_history o)
        {
            string insertsql = @"INSERT INTO brochure_history (userid,visit_date,page,pageURL) VALUES(@userid,@visit_date,@page,@pageURL)";
            var id = 0;
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();

                MySqlCommand cmd = Utils.GetCommand(insertsql, conn);
                BuildSqlParameters(cmd, o);
                cmd.ExecuteNonQuery();
                cmd.CommandText = "SELECT id FROM brochure_history WHERE id = LAST_INSERT_ID()";
                id = (int)cmd.ExecuteScalar();
            }
            return id;
        }

        private static void BuildSqlParameters(MySqlCommand cmd, Brochure_history o, bool forInsert = true)
        {

            if (!forInsert)
                cmd.Parameters.AddWithValue("@id", o.id);
            cmd.Parameters.AddWithValue("@userid", o.userid);
            cmd.Parameters.AddWithValue("@visit_date", o.visit_date);
            cmd.Parameters.AddWithValue("@page", o.page);
            cmd.Parameters.AddWithValue("@pageURL", o.pageURL);
        }

        public static void Update(Brochure_history o)
        {
            string updatesql = @"UPDATE brochure_history SET userid = @userid,visit_date = @visit_date,page = @page,pageURL = @pageURL WHERE id = @id";

            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = Utils.GetCommand(updatesql, conn);
                BuildSqlParameters(cmd, o, false);
                cmd.ExecuteNonQuery();
            }
        }

        public static void Delete(int id)
        {
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = Utils.GetCommand("DELETE FROM brochure_history WHERE id = @id", conn);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }
        }
    }
}
