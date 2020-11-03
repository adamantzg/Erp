using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using erp.Model;
using MySql.Data.MySqlClient;
using System.Text.RegularExpressions;

namespace erp.Model.DAL
{
    public class Webproduct_searchDAL
	{
	
		public static List<Webproduct_search> GetAll()
		{
			var result = new List<Webproduct_search>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM webproduct_search", conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}


        public static List<Webproduct_search> GetByBrandId(int site_id)
		{
            var result = new List<Webproduct_search>();
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM webproduct_search WHERE web_site_id = @id", conn);
                cmd.Parameters.AddWithValue("@id", site_id);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
			return result;
		}

        public static Webproduct_search GetByBarcode(string barcode)
        {
            Webproduct_search result = null;
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM webproduct_search WHERE barcode = @barcode", conn);
                cmd.Parameters.AddWithValue("@barcode", barcode);
                var dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    result = GetFromDataReader(dr);
                }
                dr.Close();
            }
            return result;
        }

        public static List<Webproduct_search> GetByCategory(int catid)
        {
            var result = new List<Webproduct_search>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM webproduct_search WHERE category_id = @id", conn);
                cmd.Parameters.AddWithValue("@id", catid);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
        }

        public static List<Webproduct_search> GetByProduct(int web_unique)
        {
            var result = new List<Webproduct_search>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM webproduct_search WHERE web_unique = @web_unique", conn);
                cmd.Parameters.AddWithValue("@web_unique", web_unique);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
        }

        public static List<Webproduct_search> GetByCustProduct(int cprod_id)
        {
            var result = new List<Webproduct_search>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM webproduct_search WHERE cprod_id = @cprod_id", conn);
                cmd.Parameters.AddWithValue("@cprod_id", cprod_id);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
        }

        public static List<Webproduct_search> GetAllForSite(int web_site_id)
        {
            var result = new List<Webproduct_search>();

            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand(@"SELECT
                                                *
                                                FROM
                                                webproduct_search
                                                WHERE
                                                webproduct_search.web_site_id = @web_site_id", conn);
                cmd.Parameters.AddWithValue("@web_site_id",web_site_id);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();

            }


                return result;
        }

        public static List<Webproduct_search> Search(string text, out int totalCount, int site_id, List<Search_word> words = null, int? page = null, int? pageSize = null, int? catid = null, bool forCategory = false)
        {
            var result = new List<Webproduct_search>();
            if (!string.IsNullOrEmpty(text) && Regex.IsMatch(text, "^[a-zA-Z0-9_-]+$"))
            {
                int? from = pageSize != null ? pageSize * page : 0;
                using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
                {
                    conn.Open();
                    /*var fromClause = @"{0} FROM webproduct_search 
								WHERE ((MATCH(web_name) AGAINST(@term2 IN BOOLEAN MODE) OR MATCH(web_code) AGAINST(@term2 IN BOOLEAN MODE) OR MATCH(cprod_code1) AGAINST(@term2 IN BOOLEAN MODE)
                                OR MATCH(cprod_name) AGAINST(@term2 IN BOOLEAN MODE) OR MATCH(whitebook_title) AGAINST(@term2 IN BOOLEAN MODE) OR MATCH(barcode) AGAINST(@term2 IN BOOLEAN MODE)) OR (web_name LIKE @term OR barcode LIKE @term OR web_code LIKE @term OR cprod_code1 LIKE @term OR cprod_name LIKE @term OR 
                                whitebook_title LIKE @term OR web_unique LIKE @termid OR cprod_id LIKE @termid)) AND (web_site_id = @site_id) {2} GROUP BY web_unique {1}";*/

	                var fromClause = @"{0} FROM webproduct_search 
								WHERE (web_name LIKE @term OR barcode LIKE @term OR web_code LIKE @term OR cprod_code1 LIKE @term OR cprod_name LIKE @term OR 
                                whitebook_title LIKE @term OR web_unique LIKE @termid OR cprod_id LIKE @termid) AND (web_site_id = @site_id) {2} GROUP BY web_unique {1}";

                    var cmd = Utils.GetCommand(string.Format(fromClause, "SELECT * ", pageSize != null && !forCategory ? $" ORDER BY web_name, web_code LIMIT {from},{pageSize} " : "", catid != null ? (" AND category_id=@catid ") : ""), conn);
                    cmd.Parameters.AddWithValue("@site_id", site_id);
                    cmd.Parameters.AddWithValue("@term", "%" + text + "%");
                    //cmd.Parameters.AddWithValue("@term2", string.Join(" ", GetSynonims(text, words)));
                    cmd.Parameters.AddWithValue("@termid", text);
                    if (catid != null)
                        cmd.Parameters.AddWithValue("@catid", catid.Value);
                    var dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        result.Add(GetFromDataReader(dr));
                    }
                    dr.Close();

                    if (!forCategory)
                    {
                        cmd.CommandText = string.Format(fromClause, "SELECT * ", "", catid != null ? (" AND category_id=@catid ") : "");
                        var dr2 = cmd.ExecuteReader();
                        var tempCount = 0;
                        while(dr2.Read())
                        {
                            tempCount += 1;
                        }
                        totalCount = tempCount;
                        dr2.Close();
                    }
                    else
                    {
                        totalCount = 0;
                    }
                }
            }
            else
                totalCount = 0;

            return result;
        }
		
	
		public static Webproduct_search GetFromDataReader(MySqlDataReader dr)
		{
			Webproduct_search o = new Webproduct_search();
		
			o.web_unique =  Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"web_unique")) ?? 0;
			o.web_name = string.Empty + Utilities.GetReaderField(dr,"web_name");
            o.cprod_id = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "cprod_id"));
            o.web_site_id = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "web_site_id"));
			o.web_code = string.Empty + Utilities.GetReaderField(dr,"web_code");
			o.whitebook_title = string.Empty + Utilities.GetReaderField(dr,"whitebook_title");
            o.cprod_code1 = string.Empty + Utilities.GetReaderField(dr, "cprod_code1");
            o.cprod_name = string.Empty + Utilities.GetReaderField(dr, "cprod_name");
			o.web_code_override = string.Empty + Utilities.GetReaderField(dr,"web_code_override");
            o.category_id = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "category_id"));
            o.barcode = string.Empty + Utilities.GetReaderField(dr,"barcode");
			
			return o;

		}
		
		private static void BuildSqlParameters(MySqlCommand cmd, Webproduct_search o, bool forInsert = true)
        {
			
			cmd.Parameters.AddWithValue("@web_unique", o.web_unique);
			cmd.Parameters.AddWithValue("@web_name", o.web_name);
			cmd.Parameters.AddWithValue("@web_site_id", o.web_site_id);
			cmd.Parameters.AddWithValue("@web_code", o.web_code);
			cmd.Parameters.AddWithValue("@whitebook_title", o.whitebook_title);
            cmd.Parameters.AddWithValue("@cprod_code1", o.cprod_code1);
            cmd.Parameters.AddWithValue("@cprod_name", o.cprod_name);
            cmd.Parameters.AddWithValue("@cprod_id", o.cprod_id);
			cmd.Parameters.AddWithValue("@web_code_override", o.web_code_override);
            cmd.Parameters.AddWithValue("@category_id", o.category_id);
            cmd.Parameters.AddWithValue("@barcode",o.barcode);
		}

        private static List<string> GetSynonims(string text, List<Search_word> words)
        {
            var synonims = new List<string>();
            text = text = text.Replace(" - ", " ").Replace("-", "").Replace(" -", "").Replace("- ", "").Trim();
            if (!string.IsNullOrEmpty(text) && Regex.IsMatch(text, "^[a-zA-Z0-9_-]+$"))
            {
                synonims.Add(string.Join(" ", text.Split(' ').Select(s => "+" + s)));
                if (words != null)
                {
                    var word = words.FirstOrDefault(w => w.word.ToLower() == text.ToLower());
                    if (word != null)
                        synonims = words.Where(w => w.group_id == word.group_id).Select(w => string.Format("({0})", string.Join(" ", w.word.Split(' ').Select(s => "+" + s)))).ToList();

                    //Handle individual words
                    var wordParts = text.Split(' ');
                    if (wordParts.Length > 1)
                    {
                        var synonim = synonims[0]; //modify original combination
                        foreach (string ws in wordParts)
                        {
                            word = words.FirstOrDefault(w => w.word.ToLower() == ws.ToLower());
                            if (word != null)
                            {
                                var partSynonims = words.Where(w => w.group_id == word.group_id);
                                synonim = synonim.Replace("+" + ws, string.Format("+({0})", string.Join(" ", partSynonims.Select(p => p.word))));
                                //synonims.AddRange(words.Where(w => w.group_id == word.group_id && w.word != word.word).Select(wordSynonim => string.Join(" ", text.Replace(ws, wordSynonim.word).Split(' ').Select(s => "+" + s))));
                            }
                        }
                        synonims[0] = synonim;
                    }

                }
            }
            return synonims;
        }
				
	}
}
			
			