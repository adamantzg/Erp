using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Dapper;
using MySql.Data.MySqlClient;

namespace erp.Model.DAL
{
    public partial class Web_product_fileDAL
    {
        public static List<Web_product_file> GetForProduct(int web_unique, IDbConnection conn = null)
        {
            var result = new List<Web_product_file>();
            bool dispose = false;
            if (conn == null)
            {
                conn = new MySqlConnection(Properties.Settings.Default.ConnString);
                conn.Open();
                dispose = true;
            }
            try
            {
                var cmd = Utils.GetCommand("SELECT * FROM web_product_file WHERE web_unique = @web_unique", (MySqlConnection)conn);
                cmd.Parameters.AddWithValue("@web_unique", web_unique);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            finally
            {
                if (dispose)
                    conn.Dispose();
            }

            return result;
        }

        public static void CreateEx(Web_product_file o, IDbTransaction tr = null)
        {
            string insertsql = @"INSERT INTO web_product_file (web_unique,name,file_type,width,height,image_site_id,hires_tif,hires_psd,hires_eps) VALUES(@web_unique,@name,@file_type,@width,@height,@image_site_id,@hires_tif,@hires_psd,@hires_eps)";

            var conn = tr != null ? tr.Connection : new MySqlConnection(Properties.Settings.Default.ConnString);
            try
            {
                if (tr == null)
                    conn.Open();

                var cmd = Utils.GetCommand(insertsql, (MySqlConnection)conn, (MySqlTransaction)tr);
                BuildSqlParameters(cmd, o);
                cmd.ExecuteNonQuery();
                cmd.CommandText = "SELECT id FROM web_product_file WHERE id = LAST_INSERT_ID()";
                o.id = (int)cmd.ExecuteScalar();

            }
            finally
            {
                if (tr == null)
                    conn.Dispose();
            }
        }

        public static void UpdateEx(Web_product_file o, IDbTransaction tr = null)
        {
            string updatesql = @"UPDATE web_product_file SET web_unique = @web_unique,name = @name,file_type = @file_type,width = @width,height = @height,image_site_id = @image_site_id , hires_tif = @hires_tif, hires_psd = @hires_psd, hires_eps = @hires_eps,approval=@approval WHERE id = @id";

            var conn = tr != null ? tr.Connection : new MySqlConnection(Properties.Settings.Default.ConnString);
            try
            {
                if (tr == null)
                    conn.Open();

                var cmd = Utils.GetCommand(updatesql, (MySqlConnection)conn, (MySqlTransaction)tr);
                BuildSqlParameters(cmd, o,false);
                cmd.ExecuteNonQuery();


            }
            finally
            {
                if (tr == null)
                    conn.Dispose();
            }


        }

        public static void Delete(int id, IDbTransaction tr = null)
        {
            var conn = tr != null ? tr.Connection : new MySqlConnection(Properties.Settings.Default.ConnString);
            try
            {
                if (tr == null)
                    conn.Open();
                var cmd = Utils.GetCommand("DELETE FROM web_product_file WHERE id = @id", (MySqlConnection)conn, (MySqlTransaction)tr);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }
            finally
            {
                if (tr == null)
                    conn.Dispose();
            }
        }

        /// <summary>
        /// Deletes images for web product that are not in the list
        /// </summary>
        /// <param name="web_unique"></param>
        /// <param name="ids"></param>
        /// <param name="tr"></param>
        public static void DeleteMissing(int web_unique, IList<int> ids, IDbTransaction tr = null)
        {
            var conn = tr != null ? tr.Connection : new MySqlConnection(Properties.Settings.Default.ConnString);
            try
            {
                if (tr == null)
                    conn.Open();
                var cmd = Utils.GetCommand("", (MySqlConnection)conn, (MySqlTransaction)tr);
                cmd.CommandText =
                    string.Format("DELETE FROM web_product_file WHERE web_unique = @web_unique AND id NOT IN ({0})",
                                  Utils.CreateParametersFromIdList(cmd, ids));
                cmd.Parameters.AddWithValue("@web_unique", web_unique);
                cmd.ExecuteNonQuery();
            }
            finally
            {
                if (tr == null)
                    conn.Dispose();
            }
        }

        public static List<Web_product_file> GetForTypes(int[] ids)
        {
            var result = new List<Web_product_file>();

            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("", conn);
                cmd.CommandText =
                    string.Format(
                        "SELECT * FROM web_product_file INNER JOIN web_product_new ON web_product_file.web_unique = web_product_new.web_unique WHERE file_type IN ({0})",
                        Utils.CreateParametersFromIdList(cmd, ids));
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    var file = GetFromDataReader(dr);
                    file.Product = Web_product_newDAL.GetFromDataReader(dr);
                    result.Add(file);
                }
                dr.Close();
            }
            return result;
        }

        public static List<Web_product_file> GetForSearch(int id,int fileType, int? date_period = null)
        {
            var result = new List<Web_product_file>();

            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("", conn);
                cmd.CommandText =
                    string.Format(
                        string.Format(@"SELECT  *
                        FROM web_product_file
                        INNER JOIN webproduct_search ON web_product_file.web_unique = webproduct_search.web_unique
                        WHERE webproduct_search.web_site_id = @id{0} AND web_product_file.file_type = @file_type",
                        date_period.HasValue ? " AND  web_product_file.created  BETWEEN  DATE_ADD(NOW(),INTERVAL -@period DAY) AND DATE_ADD(NOW(),INTERVAL 1 DAY)" : "")
                        );
                cmd.Parameters.Add(new MySqlParameter("@id", id));
                cmd.Parameters.Add(new MySqlParameter("@file_type", fileType));
                if (date_period.HasValue)
                    cmd.Parameters.Add(new MySqlParameter("@period", date_period));
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    var file = GetFromDataReader(dr);
                    file.Product = Web_product_newDAL.GetFromDataReader(dr);
                    result.Add(file);
                }
                dr.Close();
            }
            return result;
        }
        public static List<Web_product_file> GetForWebsites(int id, int? date_period=null)
        {
            var result = new List<Web_product_file>();

            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("", conn);
                cmd.CommandText =
                    string.Format(
                        string.Format(@"SELECT *
                        FROM web_product_file
                        INNER JOIN web_product_new ON web_product_file.web_unique = web_product_new.web_unique WHERE web_product_new.web_site_id = @id{0}",
                        date_period.HasValue ? " AND  web_product_file.created  BETWEEN  DATE_ADD(NOW(),INTERVAL -@period DAY) AND DATE_ADD(NOW(),INTERVAL 1 DAY)" : "")
                        );
                cmd.Parameters.Add(new MySqlParameter("@id", id));
                if (date_period.HasValue)
                    cmd.Parameters.Add(new MySqlParameter("@period",date_period));
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    var file = GetFromDataReader(dr);
                    file.Product = Web_product_newDAL.GetFromDataReader(dr);
                    result.Add(file);
                }
                dr.Close();
            }
            return result;
        }

        public static List<Web_product_file> GetForWebsitesList(List<int> ids, int? date_period = null)
        {
            var result = new List<Web_product_file>();
            var _ids = string.Join(", ", ids.ToArray());

            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {

                conn.Open();
                var cmd = Utils.GetCommand("", conn);
                cmd.CommandText =
                    string.Format(
                        string.Format(@"SELECT *
                        FROM web_product_file
                        INNER JOIN web_product_new ON web_product_file.web_unique = web_product_new.web_unique WHERE web_product_new.web_site_id IN ({1}) {0}",
                        date_period.HasValue ? " AND  web_product_file.created  BETWEEN  DATE_ADD(NOW(),INTERVAL -@period DAY) AND DATE_ADD(NOW(),INTERVAL 1 DAY)" : "",
                        Utils.CreateParametersFromIdList(cmd,ids))
                        );
               // cmd.Parameters.Add(new MySqlParameter("@ids", _ids));
                if (date_period.HasValue)
                    cmd.Parameters.Add(new MySqlParameter("@period", date_period));
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    var file = GetFromDataReader(dr);
                    file.Product = Web_product_newDAL.GetFromDataReader(dr);
                    result.Add(file);
                }
                dr.Close();
            }
            return result;
        }
        public static List<Web_product_file> GetForWebsitesWithWidthAndHeightEmpty(int id)
        {
            var result = new List<Web_product_file>();

            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("", conn);
                cmd.CommandText =
                    string.Format(
                        @"SELECT DISTINCT  web_product_file.* FROM web_product_file INNER JOIN web_product_new ON web_product_file.web_unique = web_product_new.web_unique
                            WHERE web_product_new.web_site_id = @id
                            AND ((web_product_file.width IS NULL OR web_product_file.width = 0) OR (web_product_file.height IS NULL OR web_product_file.height = 0))");
                cmd.Parameters.Add(new MySqlParameter("@id", id));
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    var file = GetFromDataReader(dr);
                    file.Product = Web_product_newDAL.GetFromDataReader(dr);
                    result.Add(file);
                }
                dr.Close();
            }
            return result;
        }

        public static List<Web_product_file> GetFiles(int catid, string searchQuery = "")
        {
            var results = new List<Web_product_file>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand();
                var cmdString = @"SELECT DISTINCT wpf.*  FROM web_product_file AS wpf
                                INNER JOIN web_product_category AS wpc ON wpf.web_unique = wpc.web_unique
                                INNER JOIN web_category AS wc ON wc.category_id = wpc.category_id
                                INNER JOIN web_product_new AS wpn ON wpn.web_unique = wpf.web_unique";

                if (!string.IsNullOrEmpty(searchQuery))
                {
                    var splitSQ = searchQuery.Split(' ');
                    if(splitSQ.Length == 2)
                    {
                        cmdString = string.Format("{0} and (wc.category_id like '%{1}%' or (wpf.name like '%{1}% %{2}%' or wpf.name like '%{2}% %{1}%') or wpf.web_unique like '%{1}%' or wpf.width like '%{1}%' or wpf.height like '%{1}%' or wpn.web_name like '%{1}%' or wpn.web_code like '%{1}%')", cmdString, splitSQ[0], splitSQ[1]);
                    }
                    else
                        cmdString = string.Format("{0} and (wc.category_id like '%{1}%' or wpf.name like '%{1}%' or wpf.web_unique like '%{1}%' or wpf.width like '%{1}%' or wpf.height like '%{1}%' or wpn.web_name like '%{1}%' or wpn.web_code like '%{1}%')", cmdString, searchQuery);
                }
                cmdString = string.Format("{0} {1}", cmdString, "WHERE wc.parent_id=@cat_id");
                cmd.Parameters.AddWithValue("@cat_id", catid);
                cmd.CommandText = cmdString;
                cmd.Connection = conn;
                var dr = cmd.ExecuteReader();
                if(dr.HasRows)
                {
                    while(dr.Read())
                    {
                        var file = GetFromDataReader(dr);
                        results.Add(file);
                    }
                    dr.Close();
                    return results;
                }
                else
                {
                    dr.Close();
                }

                cmdString = string.Format("{0} {1}", cmdString, "and wc.category_id=@cat_id2");
                cmd.Parameters.AddWithValue("@cat_id2", catid);
                cmd.CommandText = cmdString.Replace("and wc.parent_id=@cat_id","");
                cmd.Connection = conn;
                dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    var file = GetFromDataReader(dr);
                    results.Add(file);
                }
                dr.Close();
            }
            return results;
        }

        public static List<Web_product_file> GetDAMFiles(int catid, string searchQuery = "")
        {
            var results = new List<Web_product_file>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand();
                var cmdString = @"SELECT DISTINCT wpf.*  FROM web_product_file AS wpf
                                INNER JOIN web_product_category AS wpc ON wpf.web_unique = wpc.web_unique
                                INNER JOIN web_category AS wc ON wc.category_id = wpc.category_id
                                INNER JOIN web_product_new AS wpn ON wpn.web_unique = wpf.web_unique";

                if (!string.IsNullOrEmpty(searchQuery))
                {
                    var splitSQ = searchQuery.Split(' ');
                    if (splitSQ.Length == 2)
                    {
                        cmdString = string.Format("{0} and (wc.category_id like '%{1}%' or (wpf.name like '%{1}% %{2}%' or wpf.name like '%{2}% %{1}%') or wpf.web_unique like '%{1}%' or wpf.width like '%{1}%' or wpf.height like '%{1}%' or wpn.web_name like '%{1}%' or wpn.web_code like '%{1}%')", cmdString, splitSQ[0], splitSQ[1]);
                    }
                    else
                        cmdString = string.Format("{0} and (wc.category_id like '%{1}%' or wpf.name like '%{1}%' or wpf.web_unique like '%{1}%' or wpf.width like '%{1}%' or wpf.height like '%{1}%' or wpn.web_name like '%{1}%' or wpn.web_code like '%{1}%')", cmdString, searchQuery);
                }
                cmdString = string.Format("{0} {1}", cmdString, "WHERE wc.parent_id=@cat_id OR wc.parent_id IN(SELECT web_category.category_id FROM web_category WHERE web_category.parent_id=@cat_id OR web_category.parent_id IN(SELECT web_category.category_id FROM web_category WHERE web_category.parent_id=@cat_id))");
                cmd.Parameters.AddWithValue("@cat_id", catid);
                cmd.CommandText = cmdString;
                cmd.Connection = conn;
                var dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        var file = GetFromDataReader(dr);
                        results.Add(file);
                    }
                    dr.Close();
                    return results;
                }
                else
                {
                    dr.Close();
                    var files = GetFiles(catid, searchQuery);
                    if(files != null && files.Count > 0)
                        results.AddRange(files);
                    else
                    {
                        var cmd2 = Utils.GetCommand();
                        var cmdString2 = @"SELECT DISTINCT wpf.*  FROM web_product_file AS wpf
                                INNER JOIN web_product_category AS wpc ON wpf.web_unique = wpc.web_unique
                                INNER JOIN web_category AS wc ON wc.category_id = wpc.category_id
                                INNER JOIN web_product_new AS wpn ON wpn.web_unique = wpf.web_unique";
                        cmdString2 = string.Format("{0} {1}", cmdString2, "WHERE wc.category_id=@cat_id2");
                        cmd2.Parameters.AddWithValue("@cat_id2", catid);
                        cmd2.CommandText = cmdString2;
                        cmd2.Connection = conn;
                        var dr2 = cmd2.ExecuteReader();
                        if (dr2.HasRows)
                        {
                            while (dr2.Read())
                            {
                                var file = GetFromDataReader(dr2);
                                results.Add(file);
                            }
                            dr2.Close();
                            return results;
                        }
                        else
                            dr2.Close();
                    }
                }

                cmdString = string.Format("{0} {1}", cmdString, "and wc.category_id=@cat_id2");
                cmd.Parameters.AddWithValue("@cat_id2", catid);
                cmd.CommandText = cmdString.Replace("and wc.parent_id=@cat_id", "");
                cmd.Connection = conn;
                dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    var file = GetFromDataReader(dr);
                    results.Add(file);
                }
                dr.Close();
            }
            return results;
        }

        public static List<Web_product_file> GetFilesForDAMSearch(string term, int site_id)
        {
            var results = new List<Web_product_file>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand();
                var cmdString = @"select distinct wpf.*
                                    from web_product_file as wpf, web_product_category as wpc, web_category as wc, web_product_new as wpn
                                    where wpf.web_unique = wpc.web_unique
                                    and wc.category_id = wpc.category_id
                                    and wpn.web_unique = wpf.web_unique
                                    and wpn.web_site_id = @site_id
                                    and (wc.name like @text
			                                    or wpf.name like @text
			                                    or wpf.web_unique like @text
			                                    or wpn.web_name like @text
			                                    or wpn.web_code like @text)";
                cmd.Parameters.AddWithValue("@text", '%' + term + '%');
                cmd.Parameters.AddWithValue("@site_id", site_id);
                cmd.CommandText = cmdString;
                cmd.Connection = conn;
                var dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        var file = GetFromDataReader(dr);
                        results.Add(file);
                    }
                    dr.Close();

                }
            }
            return results;

        }

        public static List<Web_product_file> GetListByWebUniqueAndFileType(int web_unique, int file_type)
        {
            var results = new List<Web_product_file>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM web_product_file WHERE web_unique = @web_unique and file_type = @file_type", conn);
                cmd.Parameters.AddWithValue("@web_unique", web_unique);
                cmd.Parameters.AddWithValue("@file_type", file_type);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    var file = GetFromDataReader(dr);
                    results.Add(file);
                }
                dr.Close();
            }
            return results;
        }

        public static int GetCountByFilename(string name)
        {
            int result;
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand();
                cmd.CommandText = "SELECT Count(*) FROM web_product_file AS wpf WHERE wpf.name LIKE @name";
                cmd.Connection = conn;
                cmd.Parameters.AddWithValue("@name", name + "%");
                result = Convert.ToInt32(cmd.ExecuteScalar());
            }
            return result;
        }

		public static List<Web_product_file> GetForSlaveHost(int host_id)
		{
			List<Web_product_file> result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
			{
				conn.Open();

				conn.Query< Web_product_new,Web_product_file, Web_product_file >(
					@"SELECT web_product_new.*, web_product_file.* FROM web_product_file INNER JOIN web_product_new ON web_product_file.web_unique = web_product_new.web_unique
					  INNER JOIN web_product_file_transfer ON web_product_file.id = web_product_file_transfer.web_file_id 
						WHERE web_product_file_transfer.host_id = @host_id",
						(prod, file) =>
						{
							if (result == null)
								result = new List<Web_product_file>();
							file.Product = prod;
							result.Add(file);
							return file;
						}, new { host_id }, splitOn: "id");
			}
			return result;
		}

		public static void UpdateFileForSlaveHost(int host_id, int file_id)
		{
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
			{
				conn.Open();
				conn.Execute("DELETE FROM web_product_file_transfer WHERE host_id = @host_id AND web_file_id = @file_id", new { host_id, file_id });
			}
		}
	}
}
