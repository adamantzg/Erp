using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace Instructions.Model
{
    public class SearchDAL
    {
        public static List<SearchResult> Search(string text)
        {
            var result = new List<SearchResult>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd =
                    Utilities.GetCommand(
                        @"SELECT chapter_id, title, NULL AS section_id, NULL AS detail FROM chapter WHERE title LIKE @text
                                             UNION
                                             SELECT NULL AS chapter_id, NULL AS title, detail.section_id, detail.detail FROM detail WHERE detail LIKE @text ",
                        conn);
                cmd.Parameters.AddWithValue("@text", "%" + text + "%");
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    var res = new SearchResult();
                    res.chapter_id = Utilities.FromDbValue<int>(dr["chapter_id"]);
                    res.title = string.Empty + dr["title"];
                    res.section_id = Utilities.FromDbValue<int>(dr["section_id"]);
                    res.detail = string.Empty + dr["detail"];
                    result.Add(res);
                }
            }
            return result;
        }
    }
}
