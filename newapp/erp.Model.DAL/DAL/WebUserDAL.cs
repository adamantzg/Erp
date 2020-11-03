using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace erp.Model.DAL
{
    public class WebUserDAL
    {
        public static WebUser GetUser(string email, string password)
        {
            WebUser result = null;
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = Utils.GetCommand("SELECT * FROM brochure_requests WHERE user_email = @email AND user_password = @password", conn);
                cmd.Parameters.Add(new MySqlParameter("@email", email));
                cmd.Parameters.Add(new MySqlParameter("@password", password));
                MySqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    result = new WebUser();
                    result.Id = (int) dr["user_id"];
                    result.Email = string.Empty + dr["user_email"];
                }
                dr.Close();
            }
            return result;
        }

        public static bool EmailExists(string email)
        {
            bool result = false;
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = Utils.GetCommand("SELECT COUNT(*) FROM brochure_requests WHERE user_email = @email", conn);
                cmd.Parameters.Add(new MySqlParameter("@email", email));
                result = (Convert.ToInt64(cmd.ExecuteScalar())) > 0;
            }
            return result;
        }


        public static long Create(WebUser user)
        {
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = Utils.GetCommand("SELECT MAX(user_id) +1 FROM brochure_requests", conn);
                var id = Convert.ToInt64(cmd.ExecuteScalar());
                cmd = Utils.GetCommand("INSERT INTO brochure_requests(user_id, user_email, user_password) VALUES(@id,@email,@password)", conn);
                cmd.Parameters.Add(new MySqlParameter("@id", id));
                cmd.Parameters.Add(new MySqlParameter("@email", user.Email));
                cmd.Parameters.Add(new MySqlParameter("@password", user.Password));
                cmd.ExecuteNonQuery();
                conn.Close();
                return id;
            }
        }
    }
}
