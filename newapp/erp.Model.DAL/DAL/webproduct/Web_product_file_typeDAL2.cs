using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace erp.Model.DAL
{
    public partial class Web_product_file_typeDAL
    {
        public static List<Web_product_file_type> GetForSite(int site_id, bool overrideUniversalTypes = true, IDbConnection conn = null)
        {
            bool dispose = false;
            if (conn == null)
            {
                conn = new MySqlConnection(Properties.Settings.Default.ConnString);
                conn.Open();
                dispose = true;
            }
            var result = new List<Web_product_file_type>();
            //using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                
                var cmd = Utils.GetCommand("SELECT * FROM web_product_file_type WHERE site_id = @site_id OR site_id IS NULL", (MySqlConnection) conn);
                cmd.Parameters.AddWithValue("@site_id", site_id);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }

            var types = File_typeDAL.GetAll(conn);

            if(dispose)
                conn.Dispose();
            
            return GetForSite(site_id,result,types,overrideUniversalTypes);
        }

        public static List<Web_product_file_type> GetForSite(int site_id, List<Web_product_file_type> all, List<File_type> fileTypes, bool overrideUniversalTypes = true )
        {
            var result = all.Where(ft => ft.site_id == site_id || ft.site_id == null).ToList();
            foreach (var webProductFileType in result)
            {
                if (string.IsNullOrEmpty(webProductFileType.path))
                {
                    var ft = fileTypes.FirstOrDefault(t => t.id == webProductFileType.file_type_id);
                    if (ft != null)
                        webProductFileType.path = ft.path;
                }
                if (string.IsNullOrEmpty(webProductFileType.previewpath))
                {
                    var ft = fileTypes.FirstOrDefault(t => t.id == webProductFileType.file_type_id);
                    if (ft != null)
                        webProductFileType.previewpath = ft.previewpath;
                }
            }

            var tobeRemoved = new List<Web_product_file_type>();
            //Eliminate default if there is an override
            if (overrideUniversalTypes)
            {
                foreach (var type in result.Where(r => r.site_id == null))
                {
                    if (result.Count(r => r.site_id == site_id && r.code == type.code) > 0)
                    {
                        tobeRemoved.Add(type);
                    }
                }
                foreach (var webProductFileType in tobeRemoved)
                {
                    result.Remove(webProductFileType);
                }
            }
            return result;
        }

		

    }
}
