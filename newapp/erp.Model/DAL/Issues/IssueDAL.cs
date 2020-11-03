
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace asaq2.Model
{
    public class IssueDAL
	{
	
		public static List<Issue> GetAll(string sortField = "")
		{
			List<Issue> result = new List<Issue>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                string sql = GetIssueSQL();
                if (!string.IsNullOrEmpty(sortField))
                    sql += " ORDER BY " + sortField;
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}

        private static string GetIssueSQL()
        {
            return @"SELECT issue.*,issue_priority.priority_text, issue_status.statustext,
                        issue_area.areaname, Creator.userwelcome AS creator, Editor.userwelcome AS editor, issue_type.type_name, AssignedTo.userwelcome as assignedto
                        FROM issue
                        LEFT JOIN issue_priority ON issue_priority.priority_id = issue.priority_id
                        LEFT JOIN userusers AS Creator ON Creator.useruserid = issue.created_userid
                        LEFT JOIN userusers AS Editor ON Editor.useruserid = issue.modified_userid
                        LEFT JOIN userusers AS AssignedTo ON AssignedTo.useruserid = issue.assigned_id
                        INNER JOIN issue_status ON issue_status.status_id = issue.status_id
                        INNER JOIN issue_area ON issue_area.area_id = issue.area_id
                        INNER JOIN issue_type ON issue_type.type_id = issue.type_id";
        }
		
		
		public static Issue GetById(int id)
		{
			Issue result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(GetIssueSQL() + " WHERE issue_id = @id", conn);
				cmd.Parameters.AddWithValue("@id", id);
                MySqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    result = GetFromDataReader(dr);
                }
                dr.Close();
                if (result != null)
                {
                    cmd.CommandText = "SELECT * FROM issue_files WHERE issue_id = @id";
                    dr = cmd.ExecuteReader();
                    if (dr.HasRows)
                        result.Files = new List<Issue_files>();
                    while (dr.Read())
                    {
                        result.Files.Add(GetFileFromDataReader(dr));
                    }
                    dr.Close();
                }
                result.Comments = Issue_commentsDAL.GetForIssue(id);
                result.Categories = Issue_categoryDAL.GetAllCategoriesForIssue(id);
                result.Subscriptions = Issue_subscriptionDAL.GetForIssue(id);
                if (result.assigned_id != null)
                    result.UserAssignedTo = UserDAL.GetById(result.assigned_id.Value);
            }
			return result;
		}
	
		private static Issue GetFromDataReader(MySqlDataReader dr)
		{
			Issue o = new Issue();
		
			o.issue_id =  (int) dr["issue_id"];
			o.title = string.Empty + dr["title"];
			o.description = string.Empty + dr["description"];
			o.priority_id = (int) dr["priority_id"];
			o.datecreated = Utilities.FromDbValue<DateTime>(dr["datecreated"]);
			o.created_userid = Utilities.FromDbValue<int>(dr["created_userid"]);
			o.datemodified = Utilities.FromDbValue<DateTime>(dr["datemodified"]);
			o.modified_userid = Utilities.FromDbValue<int>(dr["modified_userid"]);
			o.duedate = Utilities.FromDbValue<DateTime>(dr["duedate"]);
			o.status_id = (int) dr["status_id"];
			o.comment = string.Empty + dr["comment"];
			o.area_id = (int) dr["area_id"];
            o.type_id = (int)dr["type_id"];
            o.assigned_id = Utilities.FromDbValue<int>(dr["assigned_id"]);

            if (Utilities.ColumnExists(dr, "areaname"))
                o.area_name = string.Empty + dr["areaname"];
            if (Utilities.ColumnExists(dr, "statustext"))
                o.status_text = string.Empty + dr["statustext"];
            if (Utilities.ColumnExists(dr, "priority_text"))
                o.priority_text = string.Empty + dr["priority_text"];
            if (Utilities.ColumnExists(dr, "creator"))
                o.creator = string.Empty + dr["creator"];
            if (Utilities.ColumnExists(dr, "editor"))
                o.editor = string.Empty + dr["editor"];
            if (Utilities.ColumnExists(dr, "type_name"))
                o.type_name = string.Empty + dr["type_name"];
            if (Utilities.ColumnExists(dr, "assignedto"))
                o.assignedto = string.Empty + dr["assignedto"];
			
			return o;

		}
		
		public static void Create(Issue o)
        {
            string insertsql = @"INSERT INTO issue (title,description,priority_id,datecreated,created_userid,datemodified,modified_userid,duedate,status_id,comment,area_id,type_id,assigned_id) VALUES(@title,@description,@priority_id,@datecreated,@created_userid,@datemodified,@modified_userid,@duedate,@status_id,@comment,@area_id,@type_id,@assigned_id)";

            MySqlConnection conn = null;
            MySqlTransaction tr = null;
            try
            {
                conn = new MySqlConnection(Properties.Settings.Default.ConnString);
                conn.Open();
                tr = conn.BeginTransaction();
                if (o.datecreated == null)
                    o.datecreated = DateTime.Now;
                MySqlCommand cmd = new MySqlCommand(insertsql, conn, tr);
                BuildSqlParameters(cmd, o);
                cmd.ExecuteNonQuery();
                cmd.CommandText = "SELECT issue_id FROM issue WHERE issue_id = LAST_INSERT_ID()";
                o.issue_id = (int)cmd.ExecuteScalar();

                UpdateCategories(cmd, o);
                                
                UpdateFiles(cmd, o);

                UpdateSubscriptions(cmd, o);
                
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

        private static void UpdateFiles(MySqlCommand cmd, Issue o)
        {
            if (o.Files != null)
            {
                foreach (var item in o.Files)
                {
                    if (item.datecreated == null)
                        o.datecreated = DateTime.Now;
                }

                cmd.Parameters.Clear();
                cmd.CommandText = "DELETE FROM issue_files WHERE issue_id = @issue_id";
                cmd.Parameters.AddWithValue("@issue_id", o.issue_id);
                cmd.ExecuteNonQuery();

                cmd.CommandText = "INSERT INTO issue_files(filename, issue_id) VALUES(@filename, @issue_id)";
                cmd.Parameters.AddWithValue("@filename", "");

                foreach (var item in o.Files)
                {
                    cmd.Parameters[1].Value = item.filename;
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private static void UpdateCategories(MySqlCommand cmd, Issue o)
        {
            cmd.Parameters.Clear();
            cmd.CommandText = "DELETE FROM issue_categories WHERE issue_id = @issue_id";
            cmd.Parameters.AddWithValue("@issue_id", o.issue_id);
            cmd.ExecuteNonQuery();

            if (o.Categories != null && o.Categories.Count > 0)
            {
                cmd.CommandText = "INSERT INTO issue_categories(category_id, issue_id) VALUES(@category_id, @issue_id)";
                cmd.Parameters.AddWithValue("@category_id", 0);

                foreach (var item in o.Categories)
                {
                    cmd.Parameters[1].Value = item.category_id;
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private static void UpdateSubscriptions(MySqlCommand cmd, Issue o)
        {
            if (o.Subscriptions != null && o.Subscriptions.Count > 0)
            {
                cmd.Parameters.Clear();
                cmd.CommandText = "DELETE FROM issue_subscription WHERE issue_id = @issue_id";
                cmd.Parameters.AddWithValue("@issue_id", o.issue_id);
                cmd.ExecuteNonQuery();

                cmd.CommandText = "INSERT INTO issue_subscription(group_id, user_id, issue_id) VALUES(@group_id,@user_id, @issue_id)";
                cmd.Parameters.AddWithValue("@group_id", DBNull.Value);
                cmd.Parameters.AddWithValue("@user_id", DBNull.Value);

                foreach (var item in o.Subscriptions)
                {
                    if(item.group_id != null)
                        cmd.Parameters[1].Value = item.group_id;
                    if (item.user_id != null)
                        cmd.Parameters[2].Value = item.user_id;
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static bool Validate(Issue i, out string message)
        {
            bool valid = true;
            message = string.Empty;
            if (string.IsNullOrEmpty(i.title))
            {
                message = Properties.Resources.IssueValidation_Title;
                valid = false;
                goto Exit;
            }
            if (i.area_id == null)
            {
                message = Properties.Resources.IssueValidaton_Area;
                valid = false;
                goto Exit;
            }
        Exit:
            return valid;
        }
		
		private static void BuildSqlParameters(MySqlCommand cmd, Issue o, bool forInsert = true)
        {
			
			if(!forInsert)
				cmd.Parameters.AddWithValue("@issue_id", o.issue_id);
			cmd.Parameters.AddWithValue("@title", o.title);
			cmd.Parameters.AddWithValue("@description", o.description);
			cmd.Parameters.AddWithValue("@priority_id", o.priority_id);
			cmd.Parameters.AddWithValue("@datecreated", o.datecreated);
			cmd.Parameters.AddWithValue("@created_userid", o.created_userid);
			cmd.Parameters.AddWithValue("@datemodified", o.datemodified);
			cmd.Parameters.AddWithValue("@modified_userid", o.modified_userid);
			cmd.Parameters.AddWithValue("@duedate", o.duedate);
			cmd.Parameters.AddWithValue("@status_id", o.status_id);
			cmd.Parameters.AddWithValue("@comment", o.comment);
			cmd.Parameters.AddWithValue("@area_id", o.area_id);
            cmd.Parameters.AddWithValue("@type_id", o.type_id);
            cmd.Parameters.AddWithValue("@assigned_id", o.assigned_id);
		}
		
		public static void Update(Issue o)
		{
            string updatesql = @"UPDATE issue SET title = @title,description = @description,priority_id = @priority_id,datecreated = @datecreated,created_userid = @created_userid,datemodified = @datemodified,modified_userid = @modified_userid,duedate = @duedate,status_id = @status_id,comment = @comment,area_id = @area_id, type_id = @type_id, assigned_id = @assigned_id WHERE issue_id = @issue_id";

            MySqlConnection conn = null;
            MySqlTransaction tr = null;
            try
            {
                conn = new MySqlConnection(Properties.Settings.Default.ConnString);
                o.datemodified = DateTime.Now;
                conn.Open();
                tr = conn.BeginTransaction();
                MySqlCommand cmd = new MySqlCommand(updatesql, conn, tr);
                BuildSqlParameters(cmd, o, false);
                cmd.ExecuteNonQuery();

                UpdateCategories(cmd, o);
                
                UpdateFiles(cmd, o);

                UpdateComments(cmd, o);

                UpdateSubscriptions(cmd, o);

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

        /// <summary>
        /// Adds new comments to database (comments cannot be edited or deleted)
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="o"></param>
        private static void UpdateComments(MySqlCommand cmd, Issue o)
        {
            if(o.Comments != null)
            {
                cmd.CommandText = @"INSERT INTO issue_comments (issue_id,comment_text,datecreated,created_userid,datemodified,modified_userid) 
                                    VALUES(@issue_id,@comment_text,@datecreated,@created_userid,@datemodified,@modified_userid)";
                
                foreach (var c in o.Comments.Where(c=>c.comment_id == 0))
                {
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@issue_id", c.issue_id);
                    cmd.Parameters.AddWithValue("@comment_text", c.comment_text);
                    if (c.datecreated == null)
                        c.datecreated = DateTime.Now;
                    cmd.Parameters.AddWithValue("@datecreated", c.datecreated);
                    cmd.Parameters.AddWithValue("@created_userid", c.created_userid);
                    cmd.Parameters.AddWithValue("@datemodified", c.datemodified);
                    cmd.Parameters.AddWithValue("@modified_userid", c.modified_userid);
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "SELECT comment_id FROM issue_comments WHERE comment_id = LAST_INSERT_ID()";
                    c.comment_id = (int)cmd.ExecuteScalar();

                    cmd.CommandText = "INSERT INTO issuecomment_files(comment_id, filename) VALUES(@comment_id, @filename)";
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@comment_id", c.comment_id);
                    cmd.Parameters.AddWithValue("@filename", "");
                    if(c.Files != null)
                    {
                        foreach (var file in c.Files)
	                    {
                            cmd.Parameters[1].Value = file.filename;
                            cmd.ExecuteNonQuery();
	                    }
                    }
                    
                }
            }
        }
		
		public static void Delete(int issue_id)
		{
            MySqlConnection conn = null;
            MySqlTransaction tr = null;
            try
            {
                conn = new MySqlConnection(Properties.Settings.Default.ConnString);
                conn.Open();
                tr = conn.BeginTransaction();
                MySqlCommand cmd = new MySqlCommand("DELETE FROM issue_categories WHERE issue_id = @id", conn, tr);
                cmd.Parameters.AddWithValue("@id", issue_id);
                cmd.ExecuteNonQuery();

                cmd.CommandText = "DELETE FROM issue_files WHERE issue_id = @id";
                cmd.ExecuteNonQuery();

                cmd.CommandText = "DELETE FROM issuecomment_files WHERE comment_id IN (SELECT comment_id FROM issue_comments WHERE issue_id = @id)";
                cmd.ExecuteNonQuery();
                
                cmd.CommandText = "DELETE FROM issue_comments WHERE issue_id = @id";
                cmd.ExecuteNonQuery();

                cmd.CommandText = "DELETE FROM issue_subscription WHERE issue_id = @id";
                cmd.ExecuteNonQuery();

                cmd.CommandText = "DELETE FROM issue WHERE issue_id = @id";
                cmd.ExecuteNonQuery();

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

        private static Issue_files GetFileFromDataReader(MySqlDataReader dr)
        {
            Issue_files o = new Issue_files();

            o.id = (int)dr["id"];
            o.issue_id = Utilities.FromDbValue<int>(dr["issue_id"]);
            o.filename = string.Empty + dr["filename"];

            return o;

        }
	}
}
			
			