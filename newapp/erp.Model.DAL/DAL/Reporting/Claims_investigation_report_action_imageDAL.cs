using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace erp.Model.DAL
{
    public class Claims_investigation_report_action_imageDAL
    {

        public static List<Claims_investigation_report_action_images> GetAll()
        {
            var result = new List<Claims_investigation_report_action_images>();

            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand(@"SELECT * FROM claims_investigation_report_action_images", conn);

                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }

            }
            return result;
        }


        public static List<Claims_investigation_report_action_images> GetImagesForAction(int id)
        {
            var result = new List<Claims_investigation_report_action_images>();

            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand(@"SELECT * FROM claims_investigation_report_action_images WHERE  action_id=@action_id",conn);
                cmd.Parameters.AddWithValue("@action_id",id);
                var dr = cmd.ExecuteReader();
                while(dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
            }
            return result;
        }

        private static Claims_investigation_report_action_images GetFromDataReader(MySqlDataReader dr)
        {
            var o = new Claims_investigation_report_action_images();
            o.id = (int)dr["id"];
            o.action_id = (int)dr["action_id"];
            o.name = string.Empty + Utilities.GetReaderField(dr, "name");
            o.image_title = string.Empty + Utilities.GetReaderField(dr, "image_title");
            return o;
        }

        public static void Create(Claims_investigation_report_action_images o)
        {                                        
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = Utils.GetCommand(
                            @"INSERT INTO claims_investigation_report_action_images
                                        (id,action_id,name,image_title)
                                  VALUES(@id,@action_id,@name,@image_title)", conn
                                                                            );
                
                BuildSqlParameters(cmd, o);
                cmd.ExecuteNonQuery();


            }
        }
        public static void UpdateTitle(Claims_investigation_report_action_images o)
        {
            using(var conn=new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = Utils.GetCommand(@"UPDATE claims_investigation_report_action_images
                                                       SET image_title = @image_title
                                                        WHERE id=@id",conn);
                cmd.Parameters.AddWithValue("@id", o.id);
                cmd.Parameters.AddWithValue("@image_title",o.image_title);

                cmd.ExecuteNonQuery();
            }
        }


        private static void BuildSqlParameters(MySqlCommand cmd, Claims_investigation_report_action_images o)
        {
            cmd.Parameters.AddWithValue("@id", null);
            cmd.Parameters.AddWithValue("@action_id", o.action_id);
            cmd.Parameters.AddWithValue("@name", o.name);
            cmd.Parameters.AddWithValue("@image_title", o.image_title);

        }


        public static void Delete(int id)
        {
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("DELETE FROM claims_investigation_report_action_images WHERE id=@id", conn);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }
        }


    }
}
