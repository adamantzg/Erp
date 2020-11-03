
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace asaq2.Model
{
    public class NpdDAL
	{
	
		public static List<Npd> GetAll()
		{
			var result = new List<Npd>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand(@"SELECT npd.*, (SELECT comments_date FROM npd_comments WHERE npd_id = npd.prod_id ORDER BY comments_date DESC LIMIT 1) AS LastCommentDate, 
                                        npd_status.name AS status_text,(SELECT GROUP_CONCAT(brandname) FROM npd_brand INNER JOIN brands ON npd_brand.brand_id = brands.brand_id WHERE npd_brand.npd_id = npd.prod_id) AS brand_names,
                                            (SELECT GROUP_CONCAT(brand_cat_desc) FROM npd_categories INNER JOIN brand_categories ON npd_categories.category_id = brand_categories.brand_cat_id WHERE npd_categories.npd_id = npd.prod_id) AS category_names
                                            FROM npd LEFT OUTER JOIN npd_status ON npd.status_id = npd_status.npdstatus_id", conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}

        public static List<Npd> GetByCriteria(int? category1_id, string text)
        {
            var result = new List<Npd>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand(@"SELECT npd.*, (SELECT comments_date FROM npd_comments WHERE npd_id = npd.prod_id ORDER BY comments_date DESC LIMIT 1) AS LastCommentDate , npd_status.name AS status_text
                                            FROM npd LEFT OUTER JOIN npd_status ON npd.status_id = npd_status.npdstatus_id WHERE (prod_type = @category1 OR @category1 IS NULL) 
                                            AND (prod_name LIKE @text OR tracking_num LIKE @text OR @text IS NULL)", conn);
                cmd.Parameters.AddWithValue("@category1", category1_id);
                cmd.Parameters.AddWithValue("@text", "%" + text + "%");
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
        }

        public static List<Npd> GetForBrand(int brand_id, string text)
        {
            var result = new List<Npd>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand(@"SELECT npd.* FROM npd WHERE EXISTS(SELECT npd_id FROM npd_brand WHERE npd_id = npd.prod_id AND brand_id = @brand) AND prod_name LIKE @text", conn);
                cmd.Parameters.AddWithValue("@brand", brand_id);
                cmd.Parameters.AddWithValue("@text", "%" + text + "%");
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
        }

        public static int GetNextSequence(DateTime date)
        {
            int result = 1;
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand(@"SELECT COALESCE(MAX(month_sequence),0) FROM npd WHERE MONTH(datecreated) = @month", conn);
                cmd.Parameters.AddWithValue("@month", date.Month);
                result = Convert.ToInt32(cmd.ExecuteScalar())+1;
            }
            return result;
        }


        public static Npd GetById(int id)
		{
			Npd result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT npd.*, category1.cat1_name AS prod_type_text FROM npd LEFT OUTER JOIN category1 ON npd.prod_type = category1.category1_id WHERE prod_id = @id", conn);
				cmd.Parameters.AddWithValue("@id", id);
                var dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    result = GetFromDataReader(dr);
                }
                dr.Close();
                var brands = BrandsDAL.GetAll();
                if (result != null)
                {
                    if (result.createdby != null)
                        result.Creator = UserDAL.GetById(result.createdby.Value);
                    result.Comments = Npd_commentsDAL.GetForNpd(id);
                    result.Brands = GetBrandsForNpd(id);
                    result.Categories = GetCategoriesForNpd(id);
                    result.Files = Npd_fileDAL.GetByNpd(id);
                    //cmd.CommandText = "SELECT brand_id FROM npd_brand WHERE npd_id = @id";
                    //dr = cmd.ExecuteReader();
                    //result.Brands = new List<Brand>();
                    //while (dr.Read())
                    //{
                    //    var brand = brands.FirstOrDefault(b => b.brand_id == (int) dr["brand_id"]);
                    //    if(brand != null)
                    //        result.Brands.Add(brand);
                    //}
                    //dr.Close();
                }
            }
			return result;
		}

        public static List<Brand> GetBrandsForNpd(int id)
        {
            var result = new List<Brand>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd =
                    new MySqlCommand(
                        "SELECT Brands.* FROM npd_brand INNER JOIN brands ON npd_brand.brand_id = brands.brand_id WHERE npd_brand.npd_id = @npd_id",
                        conn);
                cmd.Parameters.AddWithValue("@npd_id", id);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(BrandsDAL.GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
        }

        //public static List<BrandCategory> GetCategoriesForNpd(int id)
        //{
        //    var result = new List<BrandCategory>();
        //    using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
        //    {
        //        conn.Open();
        //        var cmd =
        //            new MySqlCommand(
        //                "SELECT brand_categories.* FROM npd_categories INNER JOIN brand_categories ON npd_categories.category_id = brand_categories.brand_cat_id WHERE npd_categories.npd_id = @npd_id", conn);
        //        cmd.Parameters.AddWithValue("@npd_id", id);
        //        var dr = cmd.ExecuteReader();
        //        while (dr.Read())
        //        {
        //            result.Add(CategoriesDAL.GetCategoryFromReader(dr));
        //        }
        //        dr.Close();
        //    }
        //    return result;
        //}

        public static List<Category1> GetCategoriesForNpd(int id)
        {
            var result = new List<Category1>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd =
                    new MySqlCommand(
                        "SELECT category1.* FROM npd_categories INNER JOIN category1 ON npd_categories.category_id = category1.category1_id WHERE npd_categories.npd_id = @npd_id", conn);
                cmd.Parameters.AddWithValue("@npd_id", id);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(Category1DAL.GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
        }


        private static Npd GetFromDataReader(MySqlDataReader dr)
		{
			Npd o = new Npd();
		
			o.prod_id =  (int) dr["prod_id"];
			o.prod_name = string.Empty + Utilities.GetReaderField(dr,"prod_name");
			o.tracking_num = string.Empty + Utilities.GetReaderField(dr,"tracking_num");
			o.description = string.Empty + Utilities.GetReaderField(dr,"description");
			o.prod_type = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"prod_type"));
		    if (Utilities.ColumnExists(dr, "LastCommentDate"))
		        o.LastCommentDate = Utilities.FromDbValue<DateTime>(dr["LastCommentDate"]);
		    o.Month_Sequence = Utilities.FromDbValue<int>(dr["month_sequence"]);
		    o.DateCreated = Utilities.FromDbValue<DateTime>(dr["datecreated"]);
		    o.DateModified = Utilities.FromDbValue<DateTime>(dr["datemodified"]);
		    if (Utilities.ColumnExists(dr, "prod_type_text"))
		    {
		        o.prod_type_text = string.Empty + dr["prod_type_text"];
		    }
		    o.status_id = Utilities.FromDbValue<int>(dr["status_id"]);
		    o.cprod_id = Utilities.FromDbValue<int>(dr["cprod_id"]);
		    o.prod_code = string.Empty + dr["prod_code"];
            o.createdby = Utilities.FromDbValue<int>(dr["createdby"]);
            o.modifiedby = Utilities.FromDbValue<int>(dr["modifiedby"]);
		    if (Utilities.ColumnExists(dr, "status_text"))
		    {
		        o.status_name = string.Empty + dr["status_text"];
		    }
            if (Utilities.ColumnExists(dr, "brand_names"))
            {
                o.brand_names = string.Empty + dr["brand_names"];
            }
            if (Utilities.ColumnExists(dr, "category_names"))
            {
                o.category_names = string.Empty + dr["category_names"];
            }
		    return o;
		}
		
		
		public static void Create(Npd o)
        {
            string insertsql = @"INSERT INTO npd (prod_name,tracking_num,description,prod_type,month_sequence,datecreated,datemodified,status_id,cprod_id, prod_code,createdby,modifiedby)
                                VALUES(@prod_name,@tracking_num,@description,@prod_type,@month_sequence,@datecreated,@datemodified,@status_id,@cprod_id, @prod_code,@createdby,@modifiedby)";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var tr = conn.BeginTransaction();
                try
                {
                    o.Month_Sequence = GetNextSequence(DateTime.Today);
                    o.DateCreated = DateTime.Now;
                    o.tracking_num = string.Format("NPD-{0}-{1}", DateTime.Today.ToString("MMyyyy"),
                                                   o.Month_Sequence);
                    var cmd = new MySqlCommand(insertsql, conn, tr);
                    BuildSqlParameters(cmd, o);
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "SELECT prod_id FROM npd WHERE prod_id = LAST_INSERT_ID()";
                    o.prod_id = (int) cmd.ExecuteScalar();

                    if (o.Brands != null)
                    {
                        cmd.CommandText = "INSERT INTO npd_brand(brand_id, npd_id) VALUES (@brand_id, @npd_id)";
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("@brand_id", 0);
                        cmd.Parameters.AddWithValue("@npd_id", o.prod_id);
                        foreach (var brand in o.Brands)
                        {
                            cmd.Parameters["@brand_id"].Value = brand.brand_id;
                            cmd.ExecuteNonQuery();
                        }
                    }

                    if (o.Categories != null)
                    {
                        cmd.CommandText = "INSERT INTO npd_categories(category_id, npd_id) VALUES (@category_id, @npd_id)";
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("@category_id", 0);
                        cmd.Parameters.AddWithValue("@npd_id", o.prod_id);
                        foreach (var cat in o.Categories)
                        {
                            //cmd.Parameters["@category_id"].Value = cat.brand_cat_id;
                            cmd.Parameters["@category_id"].Value = cat.category1_id;
                            cmd.ExecuteNonQuery();
                        }
                    }
                    if (o.Files != null)
                    {
                        foreach (var file in o.Files)
                        {
                            file.npd_id = o.prod_id;
                            Npd_fileDAL.Create(file,tr);
                        }
                    }
                    if (o.Comments != null && o.Comments.Count > 0)
                    {
                        foreach (var c in o.Comments)
                        {
                            c.npd_id = o.prod_id;
                            Npd_commentsDAL.Create(c,tr);
                        }
                    }

                    tr.Commit();
                }
                catch (Exception)
                {
                    tr.Rollback();
                    throw;
                }
                
            }
		}
		
		private static void BuildSqlParameters(MySqlCommand cmd, Npd o, bool forInsert = true)
        {
			
			if(!forInsert)
				cmd.Parameters.AddWithValue("@prod_id", o.prod_id);
			cmd.Parameters.AddWithValue("@prod_name", o.prod_name);
			cmd.Parameters.AddWithValue("@tracking_num", o.tracking_num);
			cmd.Parameters.AddWithValue("@description", o.description);
			cmd.Parameters.AddWithValue("@prod_type", o.prod_type);
		    cmd.Parameters.AddWithValue("@month_sequence", o.Month_Sequence);
		    cmd.Parameters.AddWithValue("@datecreated", o.DateCreated);
		    cmd.Parameters.AddWithValue("@datemodified", o.DateModified);
		    cmd.Parameters.AddWithValue("@status_id", o.status_id);
		    cmd.Parameters.AddWithValue("@cprod_id", o.cprod_id);
		    cmd.Parameters.AddWithValue("@prod_code", o.prod_code);
		    cmd.Parameters.AddWithValue("@createdby", o.createdby);
		    cmd.Parameters.AddWithValue("@modifiedby", o.modifiedby);
        }
		
		public static void Update(Npd o)
		{
			string updatesql = @"UPDATE npd SET prod_name = @prod_name,tracking_num = @tracking_num,description = @description,prod_type = @prod_type, 
                            month_sequence = @month_sequence,datecreated = @datecreated, datemodified = @datemodified,status_id = @status_id,cprod_id = @cprod_id, prod_code = @prod_code,
                            createdby = @createdby, modifiedby = @modifiedby
                            WHERE prod_id = @prod_id";

            MySqlConnection conn = null;
            MySqlTransaction tr = null;
            try
            {
                conn = new MySqlConnection(Properties.Settings.Default.ConnString);
                conn.Open();
                tr = conn.BeginTransaction();
				var cmd = new MySqlCommand(updatesql, conn,tr);
                BuildSqlParameters(cmd,o, false);
                o.DateModified = DateTime.Now;
                cmd.ExecuteNonQuery();

                if (o.Brands != null)
                {
                    cmd.CommandText = "DELETE FROM npd_brand WHERE npd_id = @npd_id";
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@npd_id", o.prod_id);
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "INSERT INTO npd_brand(brand_id, npd_id) VALUES (@brand_id, @npd_id)";
                    cmd.Parameters.AddWithValue("@brand_id", 0);
                    
                    foreach (var brand in o.Brands)
                    {
                        cmd.Parameters["@brand_id"].Value = brand.brand_id;
                        cmd.ExecuteNonQuery();
                    }
                }

                if (o.Categories != null)
                {
                    cmd.CommandText = "DELETE FROM npd_categories WHERE npd_id = @npd_id";
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@npd_id", o.prod_id);
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "INSERT INTO npd_categories(category_id, npd_id) VALUES (@category_id, @npd_id)";
                    cmd.Parameters.AddWithValue("@category_id", 0);

                    foreach (var cat in o.Categories)
                    {
                        //cmd.Parameters["@category_id"].Value = cat.brand_cat_id;
                        cmd.Parameters["@category_id"].Value = cat.category1_id;
                        cmd.ExecuteNonQuery();
                    }
                }

                if (o.Comments != null)
                {
                    foreach (var c in o.Comments.Where(c => c.comments_id == 0))
                    {
                        Npd_commentsDAL.Create(c, tr);
                    }
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
                tr = null;
                conn = null;
            }
		}
		
		public static void Delete(int prod_id)
		{
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = new MySqlCommand("DELETE FROM npd WHERE prod_id = @id" , conn);
                cmd.Parameters.AddWithValue("@id", prod_id);
                cmd.ExecuteNonQuery();
            }
		}
		
		
	}
}
			
			