using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using  erp.Model;

namespace erp.Model.DAL
{
    public class ImageCheckDAL
    {
        public static List<Model.ImageFileTest> GetImageFromLinkAll()
        {
            var result = new List<ImageFileTest>();

            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM images_rename_02", conn);
                var dr = cmd.ExecuteReader();


                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
        }
        private static ImageFileTest GetFromDataReader(MySqlDataReader dr)
        {
            ImageFileTest o = new ImageFileTest();

            //o.IdImage = (int)dr["web_unique"];
            //o.LinkImageGif = String.Empty + dr["gif"];
            //o.LinkImageJpg = String.Empty + dr["jpg"];
            o.ImageCode=String.Empty+dr["web_code"];
            o.ImageName = String.Empty + dr["image_name"];
            o.ImageName2 = String.Empty + dr["image_name_2"];
            return o;
        }
    }

    
}
