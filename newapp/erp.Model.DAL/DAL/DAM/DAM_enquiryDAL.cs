using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace erp.Model.DAL
{
    public partial class DAM_enquiryDAL
    {
        public static List<DAM_enquiry> GetAll()
        {
            var result = new List<DAM_enquiry>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM dam_enquiries", conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
        }


        public static DAM_enquiry GetById(int id)
        {
            DAM_enquiry result = null;
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM dam_enquiries WHERE id = @id", conn);
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


        public static DAM_enquiry GetFromDataReader(MySqlDataReader dr)
        {
            DAM_enquiry o = new DAM_enquiry();

            o.id = (int)dr["id"];
            o.category = (string)dr["category"];
            o.message = (string)dr["message"];
            o.ip_address = string.Empty + (string)dr["ip_address"];
            o.user_id = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"user_id"));
            o.date = (DateTime)dr["date"];

            return o;

        }

        public static void Create(DAM_enquiry o)
        {
            string insertsql = @"INSERT INTO dam_enquiries (category,message,ip_address,user_id,date) VALUES(@category,@message,@ip_address,@user_id,@date)";

            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();

                var cmd = Utils.GetCommand(insertsql, conn);
                BuildSqlParameters(cmd, o);
                cmd.ExecuteNonQuery();
                cmd.CommandText = "SELECT id FROM dam_enquiries WHERE id = LAST_INSERT_ID()";
                o.id = (int)cmd.ExecuteScalar();

            }
        }

        private static void BuildSqlParameters(MySqlCommand cmd, DAM_enquiry o)
        {
            cmd.Parameters.AddWithValue("@id", o.id);
            cmd.Parameters.AddWithValue("@category", o.category);
            cmd.Parameters.AddWithValue("@message", o.message);
            cmd.Parameters.AddWithValue("@ip_address", o.ip_address);
            cmd.Parameters.AddWithValue("@user_id", o.user_id);
            cmd.Parameters.AddWithValue("@date", o.date);
        }

        public static void Update(DAM_enquiry o)
        {
            string updatesql = @"UPDATE dam_enquiries SET category=@category,message=@message,ip_address=@ip_address,user_id=@user_id,date=@date WHERE id = @id";

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
                var cmd = Utils.GetCommand("DELETE FROM dam_enquiries WHERE id = @id", conn);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }
        }

        public static List<DAM_enquiry> GetByUser(int userid)
        {
            var result = new List<DAM_enquiry>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM dam_enquiries WHERE user_id=@userid", conn);
                cmd.Parameters.AddWithValue("@userid",userid);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
        }

        public static List<DAM_enquiry> GetByCategory(int category)
        {
            var result = new List<DAM_enquiry>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                var name = string.Empty;
                conn.Open();
                switch(category)
                {
                    case 1:
                        name = "Sales enquiry";
                        break;
                    case 2:
                        name = "Technical issue";
                        break;
                    case 3:
                        name = "Contact request";
                        break;
                    default:
                        break;
                }
                var query = string.Format("SELECT * FROM dam_enquiries WHERE {0}",!string.IsNullOrEmpty(name) ? "category=@name" : "");
                var cmd = Utils.GetCommand(query, conn);
                cmd.Parameters.AddWithValue("@name", name);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
        }
    }
}


