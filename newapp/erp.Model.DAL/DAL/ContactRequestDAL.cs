using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace erp.Model.DAL
{
    public class ContactRequestDAL
    {
        public static ContactRequest GetById(int id, int? language_id = null)
        { 
            ContactRequest c = null;
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = Utils.GetCommand("SELECT * FROM contact_requests WHERE user_id = @id", conn);
                cmd.Parameters.AddWithValue("@id", id);
                MySqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    c = GetFromDataReader(dr);
                    //get products
                    
                    cmd.CommandText = "SELECT * FROM contactrequest_products WHERE contactrequest_id = @id";
                    dr.Close();
                    dr = cmd.ExecuteReader();
                    List<int> productids = new List<int>();
                    while (dr.Read())
                    {
                        productids.Add((int)dr["product_id"]);
                    }
                    c.Products = Web_product_newDAL.GetForIds(productids, language_id:language_id);
                }
                dr.Close();
            }
            return c;
        
        }

        private static ContactRequest GetFromDataReader(MySqlDataReader dr)
        {
            ContactRequest o = new ContactRequest();

            o.user_id = (int) dr["user_id"];
            o.distributor = Utilities.FromDbValue<int>(dr["distributor"]);
            o.user_surname = string.Empty + dr["user_surname"];
            o.user_firstname = string.Empty + dr["user_firstname"];
            o.user_title = string.Empty + dr["user_title"];
            o.user_address1 = string.Empty + dr["user_address1"];
            o.user_address2 = string.Empty + dr["user_address2"];
            o.user_address3 = string.Empty + dr["user_address3"];
            o.user_address4 = string.Empty + dr["user_address4"];
            o.user_address5 = string.Empty + dr["user_address5"];
            o.postcode = string.Empty + dr["postcode"];
            o.ie_region = Utilities.FromDbValue<int>(dr["ie_region"]);
            o.user_country = string.Empty + dr["user_country"];
            o.user_tel = string.Empty + dr["user_tel"];
            o.user_mobile = string.Empty + dr["user_mobile"];
            o.user_email = string.Empty + dr["user_email"];
            o.user_type = Utilities.FromDbValue<int>(dr["user_type"]);
            o.user_created = Utilities.FromDbValue<DateTime>(dr["user_created"]);
            o.date_sent = Utilities.FromDbValue<DateTime>(dr["date_sent"]);
            o.longitude = Utilities.FromDbValue<double>(dr["longitude"]);
            o.latitude = Utilities.FromDbValue<double>(dr["latitude"]);
            o.ip_address = string.Empty + dr["ip_address"];
            o.dealer_id = Utilities.FromDbValue<int>(dr["dealer_id"]);
            o.lead_source = Utilities.FromDbValue<int>(dr["lead_source"]);
            o.sales_status = Utilities.FromDbValue<int>(dr["sales_status"]);
            return o;

        }

        public static void Create(ContactRequest o)
        {
            string insertsql = @"INSERT INTO contact_requests(user_id,distributor,user_surname,user_firstname,user_title,user_address1,user_address2,user_address3,user_address4,
                                user_address5,postcode,ie_region,user_country,user_tel,user_mobile,user_email,user_type,user_created,date_sent,longitude,latitude,ip_address,dealer_id,
                                lead_source,sales_status) VALUES(@user_id,@distributor,@user_surname,@user_firstname,@user_title,@user_address1,@user_address2,@user_address3,
                                @user_address4,@user_address5,@postcode,@ie_region,@user_country,@user_tel,@user_mobile,@user_email,@user_type,@user_created,@date_sent,@longitude,
                                @latitude,@ip_address,@dealer_id,@lead_source,@sales_status)";
            var conn = new MySqlConnection(Properties.Settings.Default.ConnString);
            conn.Open();
            MySqlTransaction tr = conn.BeginTransaction();
            try
            {
                MySqlCommand cmd = Utils.GetCommand("SELECT COALESCE(MAX(user_id),0)+1 FROM contact_requests", conn);
                o.user_id = Convert.ToInt32(cmd.ExecuteScalar());

                cmd.CommandText = insertsql;
                BuildSqlParameters(cmd, o);
                cmd.ExecuteNonQuery();

                cmd.CommandText = "INSERT INTO contactrequest_products VALUES(@user_id, @product_id)";
                cmd.Parameters.Clear();
                cmd.Parameters.Add(new MySqlParameter("@user_id", 0));
                cmd.Parameters.Add(new MySqlParameter("@product_id", 0));
                foreach (var product in o.Products)
                {
                    cmd.Parameters[0].Value = o.user_id;
                    cmd.Parameters[1].Value = product.web_unique;
                    cmd.ExecuteNonQuery();
                }
                tr.Commit();
            }
            catch (Exception)
            {
                tr.Rollback();
                throw;
            }
            finally
            {
                conn = null;
                tr = null;
            }
                        
        }

        public static void Update(ContactRequest o)
		{
            string updatesql = @"UPDATE contact_requests SET distributor = @distributor,user_surname = @user_surname,user_firstname = @user_firstname,user_title = @user_title,
                                user_address1 = @user_address1,user_address2 = @user_address2,user_address3 = @user_address3,user_address4 = @user_address4,user_address5 = @user_address5,
                                postcode = @postcode,ie_region = @ie_region,user_country = @user_country,user_tel = @user_tel,user_mobile = @user_mobile,user_email = @user_email,
                                user_type = @user_type,user_created = @user_created,date_sent = @date_sent,longitude = @longitude,latitude = @latitude,ip_address = @ip_address,
                                dealer_id = @dealer_id,lead_source = @lead_source,sales_status = @sales_status WHERE user_id = @user_id";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = Utils.GetCommand(updatesql, conn);
				BuildSqlParameters(cmd,o);
                cmd.ExecuteNonQuery();
            }
		}

        private static void BuildSqlParameters(MySqlCommand cmd, ContactRequest o)
        {
            cmd.Parameters.AddWithValue("@user_id", o.user_id);
            cmd.Parameters.AddWithValue("@distributor", o.distributor);
            cmd.Parameters.AddWithValue("@user_surname", o.user_surname);
            cmd.Parameters.AddWithValue("@user_firstname", o.user_firstname);
            cmd.Parameters.AddWithValue("@user_title", o.user_title);
            cmd.Parameters.AddWithValue("@user_address1", o.user_address1);
            cmd.Parameters.AddWithValue("@user_address2", o.user_address2);
            cmd.Parameters.AddWithValue("@user_address3", o.user_address3);
            cmd.Parameters.AddWithValue("@user_address4", o.user_address4);
            cmd.Parameters.AddWithValue("@user_address5", o.user_address5);
            cmd.Parameters.AddWithValue("@postcode", o.postcode);
            cmd.Parameters.AddWithValue("@ie_region", o.ie_region);
            cmd.Parameters.AddWithValue("@user_country", o.user_country);
            cmd.Parameters.AddWithValue("@user_tel", o.user_tel);
            cmd.Parameters.AddWithValue("@user_mobile", o.user_mobile);
            cmd.Parameters.AddWithValue("@user_email", o.user_email);
            cmd.Parameters.AddWithValue("@user_type", o.user_type);
            cmd.Parameters.AddWithValue("@user_created", o.user_created);
            cmd.Parameters.AddWithValue("@date_sent", o.date_sent);
            cmd.Parameters.AddWithValue("@longitude", o.longitude);
            cmd.Parameters.AddWithValue("@latitude", o.latitude);
            cmd.Parameters.AddWithValue("@ip_address", o.ip_address);
            cmd.Parameters.AddWithValue("@dealer_id", o.dealer_id);
            cmd.Parameters.AddWithValue("@lead_source", o.lead_source);
            cmd.Parameters.AddWithValue("@sales_status", o.sales_status);

        }

    }
}
