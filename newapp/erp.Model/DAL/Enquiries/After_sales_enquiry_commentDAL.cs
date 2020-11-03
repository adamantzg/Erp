
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace asaq2.Model
{
    public class After_sales_enquiry_commentDAL
	{

        public static List<After_sales_enquiry_comment> GetForEnquiry(int enquiry_id, CompanyType role, bool includeFiles = true)
		{
			List<After_sales_enquiry_comment> result = new List<After_sales_enquiry_comment>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(@"SELECT after_sales_enquiry_comment.*,Creator.userwelcome AS creator, Recipient.user_name AS respondto_name, Recipient.distributor AS respondto_distributor FROM after_sales_enquiry_comment 
                                                    LEFT JOIN userusers AS Creator ON Creator.useruserid = after_sales_enquiry_comment.created_userid 
                                                    INNER JOIN users AS Recipient ON after_sales_enquiry_comment.respond_to = Recipient.user_id
                                                    WHERE enquiry_id = @enquiry_id AND (creator_role = @company_role OR respond_to_role = @company_role)", conn);
                cmd.Parameters.AddWithValue("@enquiry_id", enquiry_id);
                cmd.Parameters.AddWithValue("@company_role", role);

                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    After_sales_enquiry_comment c = GetFromDataReader(dr);
                    if (includeFiles)
                        c.Files = After_sales_enquiry_comment_filesDAL.GetForComment(c.comment_id);
                    result.Add(c);
                }
                dr.Close();
            }
            return result;
		}

        public static After_sales_enquiry_comment GetLastCommentForRole(int enquiry_id, CompanyType role)
        {
            After_sales_enquiry_comment result = null;
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM after_sales_enquiry_comment WHERE enquiry_id = @enquiry_id AND (creator_role = @company_role OR respond_to_role = @company_role) ORDER BY datecreated DESC LIMIT 1 ", conn);
                cmd.Parameters.AddWithValue("@enquiry_id", enquiry_id);
                cmd.Parameters.AddWithValue("@company_role", role);
                MySqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    result = GetFromDataReader(dr);
                }
                dr.Close();
            }
            return result;
        }
		
		
		public static After_sales_enquiry_comment GetById(int id)
		{
			After_sales_enquiry_comment result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM after_sales_enquiry_comment WHERE comment_id = @id", conn);
				cmd.Parameters.AddWithValue("@id", id);
                MySqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    result = GetFromDataReader(dr);
                }
                dr.Close();
            }
			return result;
		}
	
		private static After_sales_enquiry_comment GetFromDataReader(MySqlDataReader dr)
		{
			After_sales_enquiry_comment o = new After_sales_enquiry_comment();
		
			o.comment_id =  (int) dr["comment_id"];
			o.comment_text = string.Empty + dr["comment_text"];
            o.respond_to = (int)dr["respond_to"];
			o.enquiry_id = Utilities.FromDbValue<int>(dr["enquiry_id"]);
			o.datecreated = Utilities.FromDbValue<DateTime>(dr["datecreated"]);
			o.created_userid = Utilities.FromDbValue<int>(dr["created_userid"]);
			o.datemodified = Utilities.FromDbValue<DateTime>(dr["datemodified"]);
			o.modified_userid = Utilities.FromDbValue<int>(dr["modified_userid"]);
            if (Utilities.ColumnExists(dr, "creator"))
                o.creator = string.Empty + dr["creator"];
            if (Utilities.ColumnExists(dr, "respondto_name"))
                o.respondto_name = string.Empty + dr["respondto_name"];
            o.respondto_role = (CompanyType)((int)dr["respond_to_role"]);
            o.creator_role = (CompanyType)((int)dr["creator_role"]);
            ////respondto_distributor > 0 if company distributor, 0 otherwise (it is distributor field from users table)
            //if (Utilities.ColumnExists(dr, "respondto_distributor"))
            //{
            //    int dist = (int)dr["respondto_distributor"];
            //    if (dist > 0)
            //        o.respondto_role = CompanyType.Distributor;
            //    else
            //    {
            //        if (CompanyDAL.GetMasterDistributors().Contains(o.respond_to))
            //            o.respondto_role = CompanyType.MasterDistributor;
            //        else
            //            o.respondto_role = CompanyType.Manufacturer;
            //    }
                 
            //}
			
			return o;

		}

        public static void Create(After_sales_enquiry_comment o)
        {
            Create(o, null);
        }
		
		internal static void Create(After_sales_enquiry_comment o, MySqlTransaction tr = null)
        {
            string insertsql = @"INSERT INTO after_sales_enquiry_comment (comment_text,respond_to,enquiry_id,datecreated,created_userid,datemodified,modified_userid,respond_to_role,creator_role) 
                                VALUES(@comment_text,@respond_to,@enquiry_id,@datecreated,@created_userid,@datemodified,@modified_userid, @respond_to_role,@creator_role)";

            MySqlConnection conn = (tr != null ? tr.Connection : new MySqlConnection(Properties.Settings.Default.ConnString));
			if(conn.State != System.Data.ConnectionState.Open)
                conn.Open();
		        		
			MySqlCommand cmd = new MySqlCommand(insertsql, conn);
            if (tr != null)
                cmd.Transaction = tr;
            BuildSqlParameters(cmd,o);
            cmd.ExecuteNonQuery();
			cmd.CommandText = "SELECT comment_id FROM after_sales_enquiry_comment WHERE comment_id = LAST_INSERT_ID()";
            o.comment_id = (int) cmd.ExecuteScalar();

            if (o.Files != null)
            {
                foreach (var file in o.Files)
                {
                    file.comment_id = o.comment_id;
                    After_sales_enquiry_comment_filesDAL.Create(file, tr);
                }
            }

            if (tr == null)
                conn = null;
            
		}
		
		private static void BuildSqlParameters(MySqlCommand cmd, After_sales_enquiry_comment o, bool forInsert = true)
        {
			
			if(!forInsert)
				cmd.Parameters.AddWithValue("@comment_id", o.comment_id);
			cmd.Parameters.AddWithValue("@comment_text", o.comment_text);
			cmd.Parameters.AddWithValue("@respond_to", o.respond_to);
			cmd.Parameters.AddWithValue("@enquiry_id", o.enquiry_id);
			cmd.Parameters.AddWithValue("@datecreated", o.datecreated);
			cmd.Parameters.AddWithValue("@created_userid", o.created_userid);
			cmd.Parameters.AddWithValue("@datemodified", o.datemodified);
			cmd.Parameters.AddWithValue("@modified_userid", o.modified_userid);
            cmd.Parameters.AddWithValue("@respond_to_role", o.respondto_role);
            cmd.Parameters.AddWithValue("@creator_role", o.creator_role);
		}
		
		public static void Update(After_sales_enquiry_comment o)
		{
			string updatesql = @"UPDATE after_sales_enquiry_comment SET comment_text = @comment_text,respond_to = @respond_to,enquiry_id = @enquiry_id,datecreated = @datecreated,
                                created_userid = @created_userid,datemodified = @datemodified,modified_userid = @modified_userid, respond_to_role = @respond_to_role, creator_role = @creator_role
                                WHERE comment_id = @comment_id";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				MySqlCommand cmd = new MySqlCommand(updatesql, conn);
                BuildSqlParameters(cmd,o, false);
                cmd.ExecuteNonQuery();
            }
		}

        public static void Delete(int file_id, MySqlCommand cmd = null)
        {
            string sql = "DELETE FROM after_sales_enquiry_comment WHERE comment_id = @id";
            if (cmd == null)
            {
                using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
                {
                    conn.Open();
                    cmd = new MySqlCommand("", conn);
                    cmd.Parameters.AddWithValue("@id", file_id);
                    cmd.ExecuteNonQuery();
                }
            }
            else
            {
                if (cmd.Parameters.Count == 0)
                    cmd.Parameters.AddWithValue("@id", file_id);
                cmd.CommandText = sql;
                cmd.ExecuteNonQuery();
            }
        }
		
		public static void Delete(int comment_id)
		{
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				MySqlCommand cmd = new MySqlCommand("DELETE FROM after_sales_enquiry_comment WHERE comment_id = @id" , conn);
                cmd.Parameters.AddWithValue("@id", comment_id);
                cmd.ExecuteNonQuery();
            }
		}
	}
}
			
			