using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace asaq2.Model
{
    public class BrochureRequestDAL
    {
        public static void Create(BrochureRequest br)
        {
            string insertsql = @"INSERT INTO brochure_requests (user_id,distributor,user_surname,user_firstname,user_title,user_address1,user_address2,user_address3,user_address4,user_address5,
                                postcode,ie_region,user_country,user_tel,user_mobile,user_email,user_type,user_created,date_sent,longitude,latitude,ip_address,dealer_id,lead_source,sales_status,
                                user_password) VALUES(@user_id,@distributor,@user_surname,@user_firstname,@user_title,@user_address1,@user_address2,@user_address3,@user_address4,@user_address5,
                                @postcode,@ie_region,@user_country,@user_tel,@user_mobile,@user_email,@user_type,@user_created,@date_sent,@longitude,@latitude,@ip_address,@dealer_id,@lead_source,@sales_status,
                                @user_password)";
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("SELECT MAX(user_id)+1 FROM brochure_requests", conn);
                br.user_id = Convert.ToInt32(cmd.ExecuteScalar());

                cmd.CommandText = insertsql;
                BuildSqlParameters(cmd, br);
                cmd.ExecuteNonQuery();
            }
        }

        private static void BuildSqlParameters(MySqlCommand cmd, BrochureRequest br)
        {
            cmd.Parameters.AddWithValue("@user_id", br.user_id);
            cmd.Parameters.AddWithValue("@distributor", br.distributor);
            cmd.Parameters.AddWithValue("@user_surname", br.user_surname);
            cmd.Parameters.AddWithValue("@user_firstname", br.user_firstname);
            cmd.Parameters.AddWithValue("@user_title", br.user_title);
            cmd.Parameters.AddWithValue("@user_address1", br.user_address1);
            cmd.Parameters.AddWithValue("@user_address2", br.user_address2);
            cmd.Parameters.AddWithValue("@user_address3", br.user_address3);
            cmd.Parameters.AddWithValue("@user_address4", br.user_address4);
            cmd.Parameters.AddWithValue("@user_address5", br.user_address5);
            cmd.Parameters.AddWithValue("@postcode", br.postcode);
            cmd.Parameters.AddWithValue("@ie_region", br.ie_region);
            cmd.Parameters.AddWithValue("@user_country", br.user_country);
            cmd.Parameters.AddWithValue("@user_tel", br.user_tel);
            cmd.Parameters.AddWithValue("@user_mobile", br.user_mobile);
            cmd.Parameters.AddWithValue("@user_email", br.user_email);
            cmd.Parameters.AddWithValue("@user_type", br.user_type);
            cmd.Parameters.AddWithValue("@user_created", br.user_created);
            cmd.Parameters.AddWithValue("@date_sent", br.date_sent);
            cmd.Parameters.AddWithValue("@longitude", br.longitude);
            cmd.Parameters.AddWithValue("@latitude", br.latitude);
            cmd.Parameters.AddWithValue("@ip_address", br.ip_address);
            cmd.Parameters.AddWithValue("@dealer_id", br.dealer_id);
            cmd.Parameters.AddWithValue("@lead_source", br.lead_source);
            cmd.Parameters.AddWithValue("@sales_status", br.sales_status);
            cmd.Parameters.AddWithValue("@user_password", br.user_password);

        }

        public static void Update(BrochureRequest br)
        {
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(@"UPDATE brochure_requests SET distributor = @distributor,user_surname = @user_surname,user_firstname = @user_firstname,user_title = @user_title,user_address1 = @user_address1,
                                                        user_address2 = @user_address2,user_address3 = @user_address3,user_address4 = @user_address4,user_address5 = @user_address5,
                                                        postcode = @postcode,ie_region = @ie_region,user_country = @user_country,user_tel = @user_tel,user_mobile = @user_mobile,
                                                        user_email = @user_email,user_type = @user_type,user_created = @user_created,date_sent = @date_sent,longitude = @longitude,latitude = @latitude,
                                                        ip_address = @ip_address,dealer_id = @dealer_id,lead_source = @lead_source,sales_status = @sales_status,user_password = @user_password
                                                    WHERE user_id = @user_id" , conn);
                BuildSqlParameters(cmd, br);
                cmd.ExecuteNonQuery();
            }
        }

        public static BrochureRequest GetByCriteria(string email)
        {
            BrochureRequest br = null;
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM brochure_requests WHERE user_email = @email", conn);
                cmd.Parameters.Add("@email", email);
                MySqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                    br = GetBrochureRequestFromReader(dr);
                dr.Close();
            }
            return br;
        }

        public static List<BrochureRequest> GetByDealer(int dealer_id)
        {
            List<BrochureRequest> result = new List<BrochureRequest>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM brochure_requests WHERE dealer_id = @dealer_id", conn);
                cmd.Parameters.Add("@dealer_id", dealer_id);
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetBrochureRequestFromReader(dr));
                }
                dr.Close();
            }
            return result;
        }

        public static BrochureRequest GetById(int id)
        {
            BrochureRequest br = null;
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM brochure_requests WHERE user_id = @id", conn);
                cmd.Parameters.Add("@id", id);
                MySqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                    br = GetBrochureRequestFromReader(dr);
                dr.Close();
            }
            return br;
        }

        private static BrochureRequest GetBrochureRequestFromReader(MySqlDataReader dr)
        {
            BrochureRequest br = new BrochureRequest();
            br.user_id = (int)dr["user_id"];
            br.distributor = Utilities.FromDbValue<int>(dr["distributor"]);
            br.user_surname = string.Empty + dr["user_surname"];
            br.user_firstname = string.Empty + dr["user_firstname"];
            br.user_title = string.Empty + dr["user_title"];
            br.user_address1 = string.Empty + dr["user_address1"];
            br.user_address2 = string.Empty + dr["user_address2"];
            br.user_address3 = string.Empty + dr["user_address3"];
            br.user_address4 = string.Empty + dr["user_address4"];
            br.user_address5 = string.Empty + dr["user_address5"];
            br.postcode = string.Empty + dr["postcode"];
            br.ie_region = Utilities.FromDbValue<int>(dr["ie_region"]);
            br.user_country = string.Empty + dr["user_country"];
            br.user_tel = string.Empty + dr["user_tel"];
            br.user_mobile = string.Empty + dr["user_mobile"];
            br.user_email = string.Empty + dr["user_email"];
            br.user_type = Utilities.FromDbValue<int>(dr["user_type"]);
            br.user_created = Utilities.FromDbValue<DateTime>(dr["user_created"]);
            br.date_sent = Utilities.FromDbValue<DateTime>(dr["date_sent"]);
            br.longitude = Utilities.FromDbValue<double>(dr["longitude"]);
            br.latitude = Utilities.FromDbValue<double>(dr["latitude"]);
            br.ip_address = string.Empty + dr["ip_address"];
            br.dealer_id = Utilities.FromDbValue<int>(dr["dealer_id"]);
            br.lead_source = Utilities.FromDbValue<int>(dr["lead_source"]);
            br.sales_status = Utilities.FromDbValue<int>(dr["sales_status"]);
            if (br.sales_status == 0)
                br.sales_status = null;
            br.user_password = string.Empty + dr["user_password"];
            return br;
        }
    }
}
