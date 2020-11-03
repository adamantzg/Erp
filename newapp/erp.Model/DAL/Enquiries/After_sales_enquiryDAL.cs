
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace asaq2.Model
{
    public class After_sales_enquiryDAL
	{

        public static List<After_sales_enquiry> GetAll(string sortField = "", int? status_id = null, DateTime? closedFrom = null, DateTime? closedTo = null)
		{
			List<After_sales_enquiry> result = new List<After_sales_enquiry>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM after_sales_enquiry", conn);
                
                //AppendStatusParameters(cmd, status_id, closedFrom, closedTo);
                if (!string.IsNullOrEmpty(sortField))
                    cmd.CommandText += " ORDER BY " + sortField;
                MySqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="distributor_id"></param>
        /// <param name="sortField"></param>
        /// <param name="closedFrom">First date of interval when issue closed</param>
        /// <param name="closedTo"></param>
        /// <returns></returns>
        public static List<After_sales_enquiry> GetByDistributor(int distributor_id, string sortField = "", bool completedOnly = false, DateTime? closedFrom = null, DateTime? closedTo = null)
        {
            List<After_sales_enquiry> result = new List<After_sales_enquiry>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                string sql = GetSelectSql() + " WHERE Creator.user_id = @distributor_id";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@distributor_id", distributor_id);
                //cmd.Parameters.AddWithValue("@company_role", CompanyType.Distributor);

                AppendStatusParameters(cmd, completedOnly, closedFrom, closedTo );
                if (!string.IsNullOrEmpty(sortField))
                    cmd.CommandText += " ORDER BY " + sortField;
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    After_sales_enquiry ase = GetFromDataReader(dr);
                    AddLastCommentAndVisit(ase, CompanyType.Distributor);
                    result.Add(ase);
                    
                }
                dr.Close();
            }
            return result;
        }

        private static void AddLastCommentAndVisit(After_sales_enquiry ase, CompanyType companyType)
        {
            After_sales_enquiry_comment lastComment = After_sales_enquiry_commentDAL.GetLastCommentForRole(ase.enquiry_id, companyType);
            if (lastComment != null)
            {
                ase.Comments = new List<After_sales_enquiry_comment>();
                ase.Comments.Add(lastComment);
            }
            After_sales_enquiry_sitevisit sv = After_sales_enquiry_sitevisitDAL.GetLastVisit(ase.enquiry_id);
            if (sv != null)
            {
                ase.SiteVisits = new List<After_sales_enquiry_sitevisit>();
                ase.SiteVisits.Add(sv);
            }
        }

        public static List<After_sales_enquiry> GetByIds(List<int> ids)
        {
            List<After_sales_enquiry> result = new List<After_sales_enquiry>();
            if (ids.Count > 0)
            {
                using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand("", conn);
                    string sql = GetSelectSql() + " WHERE enquiry_id IN ({0})";
                    string paramstring = Utilities.CreateParametersFromIdList(cmd, ids);
                    cmd.CommandText = string.Format(sql, paramstring);
                    MySqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        result.Add(GetFromDataReader(dr));
                    }
                    dr.Close();
                }
            }
            return result;
        }



        private static void AppendStatusParameters(MySqlCommand cmd,  bool completedOnly, DateTime? closedFrom, DateTime? closedTo)
        {
            List<string> where = new List<string>();
            if (completedOnly)
            {
                where.Add("after_sales_enquiry.dateclosed IS NOT NULL");
                
            }
            if (closedFrom != null)
            {
                //If status_id is set, clause has format AND status_id = 6 AND dateClosed >= @closedFrom
                //If status is not set, we must allow for other statuses so clause looks like this
                //   AND ((status_id = 6 AND dateclosed >= @closedFrom) OR status_id <> 6)
                where.Add(string.Format("((dateclosed >= @closedFrom) {1})", After_sales_enquiry_status.Processed,
                    (!completedOnly ? " OR after_sales_enquiry.dateclosed IS NULL" : "")));
                cmd.Parameters.AddWithValue("@closedFrom", closedFrom);
            }
            if (closedTo != null)
            {
                //where.Add(string.Format("((after_sales_enquiry.status_id = {0} AND dateclosed <= @closedTo) {1})", After_sales_enquiry_status.Processed,
                //    (status_id == null ? string.Format(" OR after_sales_enquiry.status_id <> {0}", After_sales_enquiry_status.Processed) : "")));
                where.Add(string.Format("((dateclosed <= @closedFrom) {1})", After_sales_enquiry_status.Processed,
                    (!completedOnly ? " OR after_sales_enquiry.dateclosed IS NULL" : "")));
                cmd.Parameters.AddWithValue("@closedTo", closedTo);
            }
            if (where.Count > 0)
            {
                string wherePart = string.Join(" AND ", where.ToArray());
                if (cmd.CommandText.Contains("WHERE"))
                    cmd.CommandText += " AND ";
                else
                    cmd.CommandText += " WHERE ";
                cmd.CommandText += wherePart;
            }
        }

        public static List<After_sales_enquiry> GetForMasterDistributor(string sortField = "", bool completedOnly = false, DateTime? closedFrom = null, DateTime? closedTo = null)
        {
            List<After_sales_enquiry> result = new List<After_sales_enquiry>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(GetSelectSql(), conn);

                AppendStatusParameters(cmd,completedOnly , closedFrom, closedTo);
                if (!string.IsNullOrEmpty(sortField))
                    cmd.CommandText += " ORDER BY " + sortField;
                MySqlDataReader dr = cmd.ExecuteReader();
                //cmd.Parameters.AddWithValue("@company_role", CompanyType.MasterDistributor);
                while (dr.Read())
                {
                    After_sales_enquiry ase = GetFromDataReader(dr);
                    AddLastCommentAndVisit(ase, CompanyType.MasterDistributor);
                    result.Add(ase);
                }
                dr.Close();
            }
            return result;
        }

        public static List<After_sales_enquiry> GetForManufacturer(User user, string sortField = "", bool completedOnly = false, DateTime? closedFrom = null, DateTime? closedTo = null)
        {
            List<After_sales_enquiry> result = new List<After_sales_enquiry>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(GetSelectSqlForManufacturer() + " WHERE admin_permissions.userid = @user_id", conn);

                AppendStatusParameters(cmd, completedOnly, closedFrom, closedTo);
                if (!string.IsNullOrEmpty(sortField))
                    cmd.CommandText += " ORDER BY " + sortField;
                cmd.Parameters.AddWithValue("@user_id", user.userid);
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    After_sales_enquiry ase = GetFromDataReader(dr);
                    AddLastCommentAndVisit(ase, CompanyType.Manufacturer);
                    result.Add(ase);
                }
                dr.Close();
            }
            return result;
        }


        private static string GetSelectSql()
        {
            //(SELECT respond_to_role FROM after_sales_enquiry_comment WHERE enquiry_id = after_sales_enquiry.enquiry_id AND (respond_to_role = @company_role OR creator_role = @company_role) ORDER BY comment_id DESC LIMIT 1) AS lastcommentrecipient_role,
            //(SELECT after_sales_enquiry_sitevisit.status_id FROM after_sales_enquiry_sitevisit WHERE after_sales_enquiry_sitevisit.enquiry_id = after_sales_enquiry.enquiry_id ORDER BY sitevisit_id DESC LIMIT 1) AS lastsitevisit_status

            return @"SELECT after_sales_enquiry.*, dealers.user_name AS dealer_name, clasification.clasification_name, Creator.userwelcome AS creator, 
                                            Editor.userwelcome AS editor, cust_products.cprod_name AS cprod_name, cust_products.cprod_code1 AS cprod_code, distributor.user_name as distributor_name, distributor.user_id as distributor_id
                                            FROM after_sales_enquiry INNER JOIN userusers AS Creator ON Creator.useruserid = after_sales_enquiry.created_userid
                                            INNER JOIN dealers ON dealers.user_id = after_sales_enquiry.dealer_id
                                            INNER JOIN users AS distributor ON dealers.user_type = distributor.user_id
                                            INNER JOIN cust_products ON cust_products.cprod_id = after_sales_enquiry.cprod_id
                                            INNER JOIN clasification ON clasification.clasification_id = after_sales_enquiry.clasification_id 
                                            LEFT JOIN userusers AS Editor ON Editor.useruserid = after_sales_enquiry.modified_userid";
        }

        private static string GetSelectSqlForManufacturer()
        {
            return GetSelectSql() + @" INNER JOIN mast_products ON mast_products.mast_id = cust_products.cprod_mast
                                       INNER JOIN admin_permissions ON mast_products.factory_id = admin_permissions.cusid AND admin_permissions.`returns` = 1
                                       ";
                                       
        }
		
		
		public static After_sales_enquiry GetById(int id, bool loadChildren = true, User currentUser = null)
		{
			After_sales_enquiry result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(GetSelectSql() + " WHERE enquiry_id = @id", conn);
				cmd.Parameters.AddWithValue("@id", id);
                MySqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    result = GetFromDataReader(dr);
                    if (loadChildren)
                    {
                        result.Files = After_sales_enquiry_filesDAL.GetForEnquiry(id);
                        if (currentUser != null)
                        {
                            result.Comments = After_sales_enquiry_commentDAL.GetForEnquiry(id, CompanyDAL.GetCompanyType(currentUser.company_id));
                        }
                        result.SiteVisits = After_sales_enquiry_sitevisitDAL.GetByEnquiry(id);
                    }
                }
                dr.Close();
            }
			return result;
		}

        public static List<User> GetCommentThreadUsers(int enquiry_id)
        {
            List<User> result = new List<User>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                //MySqlCommand cmd = new MySqlCommand(@"SELECT creator.*, company.* FROM After_sales_enquiry_comment INNER JOIN userusers AS creator ON After_sales_enquiry_comment.created_userid = creator.useruserid
                //                                    INNER JOIN users company ON userusers.user_id = users.user_id WHERE After_sales_enquiry_comment.enquiry_id = @enquiry_id", conn);
                MySqlCommand cmd = new MySqlCommand(@"SELECT creator.* FROM After_sales_enquiry_comment INNER JOIN userusers AS creator ON After_sales_enquiry_comment.created_userid = creator.useruserid
                                                    WHERE After_sales_enquiry_comment.enquiry_id = @enquiry_id", conn);
                cmd.Parameters.AddWithValue("@enquiry_id", enquiry_id);
                
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    User user = UserDAL.GetFromDataReader(dr);
                    //user.Company = CompanyDAL.GetFromDataReader(dr);
                    result.Add(user);
                }
                dr.Close();
            }
            return result;
        }
	
		private static After_sales_enquiry GetFromDataReader(MySqlDataReader dr)
		{
			After_sales_enquiry o = new After_sales_enquiry();
		
			o.enquiry_id =  (int) dr["enquiry_id"];
			o.dealer_id = Utilities.FromDbValue<int>(dr["dealer_id"]);
            o.reference_number = string.Empty + dr["reference_number"];
			o.cust_name = string.Empty + dr["cust_name"];
			o.cust_postcode = string.Empty + dr["cust_postcode"];
			o.cust_address1 = string.Empty + dr["cust_address1"];
			o.cust_address2 = string.Empty + dr["cust_address2"];
			o.cust_address3 = string.Empty + dr["cust_address3"];
			o.cust_address4 = string.Empty + dr["cust_address4"];
			o.cust_address5 = string.Empty + dr["cust_address5"];
			o.cust_tel = string.Empty + dr["cust_tel"];
			o.cust_mobile = string.Empty + dr["cust_mobile"];
			o.cust_email = string.Empty + dr["cust_email"];
			o.cust_web = string.Empty + dr["cust_web"];
			o.cust_country = string.Empty + dr["cust_country"];
			o.details = string.Empty + dr["details"];
			o.clasification_id = Utilities.FromDbValue<int>(dr["clasification_id"]);
			o.po_reference = string.Empty + dr["po_reference"];
			o.response_type = Utilities.FromDbValue<int>(dr["response_type"]);
			o.charge_type = Utilities.FromDbValue<int>(dr["charge_type"]);
			o.status_id = (int) dr["status_id"];
            //o.contractor_id = Utilities.FromDbValue<int>(dr["contractor_id"]);
            //o.contractor_message = string.Empty + dr["contractor_message"];
			o.datecreated = Utilities.FromDbValue<DateTime>(dr["datecreated"]);
			o.created_userid = Utilities.FromDbValue<int>(dr["created_userid"]);
			o.datemodified = Utilities.FromDbValue<DateTime>(dr["datemodified"]);
			o.modified_userid = Utilities.FromDbValue<int>(dr["modified_userid"]);
            o.cprod_id = (int) dr["cprod_id"];
            o.dateclosed = Utilities.FromDbValue<DateTime>(dr["dateclosed"]);

            if (Utilities.ColumnExists(dr, "dealer_name"))
                o.dealer_name = string.Empty + dr["dealer_name"];
            if (Utilities.ColumnExists(dr, "clasification_name"))
                o.classification_name = string.Empty + dr["clasification_name"];
            if (Utilities.ColumnExists(dr, "creator"))
                o.creator = string.Empty + dr["creator"];
            if (Utilities.ColumnExists(dr, "editor"))
                o.editor = string.Empty + dr["editor"];
            if (Utilities.ColumnExists(dr, "cprod_name"))
                o.cprod_name = string.Empty + dr["cprod_name"];
            if (Utilities.ColumnExists(dr, "cprod_code"))
                o.cprod_code = string.Empty + dr["cprod_code"];
            if (Utilities.ColumnExists(dr, "status_name"))
                o.status_name = string.Empty + dr["status_name"];
            if (Utilities.ColumnExists(dr, "distributor_id"))
                o.distributor_id = (int)dr["distributor_id"];
            if (Utilities.ColumnExists(dr, "distributor_name"))
                o.distributor_name = string.Empty + dr["distributor_name"];
            //if (Utilities.ColumnExists(dr, "lastsitevisit_status"))
            //    o.lastsitevisit_status = Utilities.FromDbValue<int>(dr["lastsitevisit_status"]);
            //if (Utilities.ColumnExists(dr, "lastcommentrecipient_role"))
            //{
            //    int? dbVal = Utilities.FromDbValue<int>(dr["lastcommentrecipient_role"]);
            //    if(dbVal != null)
            //        o.lastcommentrecipient_role = (CompanyType)dbVal.Value;
            //}
			return o;

		}
		
		public static void Create(After_sales_enquiry o)
        {
            string insertsql = @"INSERT INTO after_sales_enquiry (reference_number, dealer_id,cust_name,cust_postcode,cust_address1,cust_address2,cust_address3,cust_address4,cust_address5,cust_tel,cust_mobile,cust_email,cust_web,cust_country,details,clasification_id,po_reference,response_type,charge_type,status_id,datecreated,created_userid,datemodified,modified_userid,cprod_id, dateclosed) 
                                VALUES(@reference_number,@dealer_id,@cust_name,@cust_postcode,@cust_address1,@cust_address2,@cust_address3,@cust_address4,@cust_address5,@cust_tel,@cust_mobile,@cust_email,@cust_web,@cust_country,@details,@clasification_id,@po_reference,@response_type,@charge_type,@status_id,@datecreated,@created_userid,@datemodified,@modified_userid,@cprod_id,@dateclosed)";

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
                cmd.CommandText = "SELECT enquiry_id FROM after_sales_enquiry WHERE enquiry_id = LAST_INSERT_ID()";
                o.enquiry_id = (int)cmd.ExecuteScalar();
                                
                UpdateFiles(cmd, o);
                
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
		
		private static void BuildSqlParameters(MySqlCommand cmd, After_sales_enquiry o, bool forInsert = true)
        {
			
			if(!forInsert)
				cmd.Parameters.AddWithValue("@enquiry_id", o.enquiry_id);
			cmd.Parameters.AddWithValue("@dealer_id", o.dealer_id);
            cmd.Parameters.AddWithValue("@reference_number", o.reference_number);
			cmd.Parameters.AddWithValue("@cust_name", o.cust_name);
			cmd.Parameters.AddWithValue("@cust_postcode", o.cust_postcode);
			cmd.Parameters.AddWithValue("@cust_address1", o.cust_address1);
			cmd.Parameters.AddWithValue("@cust_address2", o.cust_address2);
			cmd.Parameters.AddWithValue("@cust_address3", o.cust_address3);
			cmd.Parameters.AddWithValue("@cust_address4", o.cust_address4);
			cmd.Parameters.AddWithValue("@cust_address5", o.cust_address5);
			cmd.Parameters.AddWithValue("@cust_tel", o.cust_tel);
			cmd.Parameters.AddWithValue("@cust_mobile", o.cust_mobile);
			cmd.Parameters.AddWithValue("@cust_email", o.cust_email);
			cmd.Parameters.AddWithValue("@cust_web", o.cust_web);
			cmd.Parameters.AddWithValue("@cust_country", o.cust_country);
			cmd.Parameters.AddWithValue("@details", o.details);
			cmd.Parameters.AddWithValue("@clasification_id", o.clasification_id);
			cmd.Parameters.AddWithValue("@po_reference", o.po_reference);
			cmd.Parameters.AddWithValue("@response_type", o.response_type);
			cmd.Parameters.AddWithValue("@charge_type", o.charge_type);
			cmd.Parameters.AddWithValue("@status_id", o.status_id);
            //cmd.Parameters.AddWithValue("@contractor_id", o.contractor_id);
            //cmd.Parameters.AddWithValue("@contractor_message", o.contractor_message);
			cmd.Parameters.AddWithValue("@datecreated", o.datecreated);
			cmd.Parameters.AddWithValue("@created_userid", o.created_userid);
			cmd.Parameters.AddWithValue("@datemodified", o.datemodified);
			cmd.Parameters.AddWithValue("@modified_userid", o.modified_userid);
            cmd.Parameters.AddWithValue("@cprod_id", o.cprod_id);
            cmd.Parameters.AddWithValue("@dateclosed", o.dateclosed);
		}
		
		public static void Update(After_sales_enquiry o)
		{
            string updatesql = @"UPDATE after_sales_enquiry SET reference_number = @reference_number,dealer_id = @dealer_id,cust_name = @cust_name,cust_postcode = @cust_postcode,cust_address1 = @cust_address1,cust_address2 = @cust_address2,cust_address3 = @cust_address3,cust_address4 = @cust_address4,cust_address5 = @cust_address5,cust_tel = @cust_tel,cust_mobile = @cust_mobile,cust_email = @cust_email,cust_web = @cust_web,cust_country = @cust_country,details = @details,clasification_id = @clasification_id,po_reference = @po_reference,response_type = @response_type,charge_type = @charge_type,status_id = @status_id,datecreated = @datecreated,created_userid = @created_userid,datemodified = @datemodified,modified_userid = @modified_userid,cprod_id = @cprod_id, dateclosed = @dateclosed 
                                WHERE enquiry_id = @enquiry_id";

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
                                
                UpdateFiles(cmd, o);

                UpdateComments(cmd, o, tr);

                UpdateVisits(cmd, o, tr);
                
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

        private static void UpdateFiles(MySqlCommand cmd, After_sales_enquiry o)
        {
            if (o.Files != null)
            {
                foreach (var item in o.Files)
                {
                    if (item.datecreated == null)
                        o.datecreated = DateTime.Now;
                }

                cmd.Parameters.Clear();
                cmd.CommandText = "DELETE FROM after_sales_enquiry_files WHERE enquiry_id = @enquiry_id";
                cmd.Parameters.AddWithValue("@enquiry_id", o.enquiry_id);
                cmd.ExecuteNonQuery();

                cmd.CommandText = "INSERT INTO after_sales_enquiry_files(file_name, enquiry_id) VALUES(@filename, @enquiry_id)";
                cmd.Parameters.AddWithValue("@filename", "");

                foreach (var item in o.Files)
                {
                    cmd.Parameters[1].Value = item.file_name;
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private static void UpdateComments(MySqlCommand cmd, After_sales_enquiry o, MySqlTransaction tr)
        {
            if (o.Comments != null)
            {
                foreach (var c in o.Comments.Where(c => c.comment_id == 0))
                {
                    After_sales_enquiry_commentDAL.Create(c, tr);
                }

//                cmd.CommandText = @"INSERT INTO after_sales_enquiry_comment (enquiry_id,comment_text,datecreated,created_userid,datemodified,modified_userid) 
//                                    VALUES(@enquiry_id,@comment_text,@datecreated,@created_userid,@datemodified,@modified_userid)";

//                foreach (var c in o.Comments.Where(c => c.comment_id == 0))
//                {
//                    cmd.Parameters.Clear();
//                    cmd.Parameters.AddWithValue("@enquiry_id", c.enquiry_id);
//                    cmd.Parameters.AddWithValue("@comment_text", c.comment_text);
//                    if (c.datecreated == null)
//                        c.datecreated = DateTime.Now;
//                    cmd.Parameters.AddWithValue("@datecreated", c.datecreated);
//                    cmd.Parameters.AddWithValue("@created_userid", c.created_userid);
//                    cmd.Parameters.AddWithValue("@datemodified", c.datemodified);
//                    cmd.Parameters.AddWithValue("@modified_userid", c.modified_userid);
//                    cmd.ExecuteNonQuery();

//                    cmd.CommandText = "SELECT comment_id FROM after_sales_enquiry_comment WHERE comment_id = LAST_INSERT_ID()";
//                    c.comment_id = (int)cmd.ExecuteScalar();

//                    cmd.CommandText = "INSERT INTO after_sales_enquiry_comment_files(comment_id, file_name) VALUES(@comment_id, @filename)";
//                    cmd.Parameters.Clear();
//                    cmd.Parameters.AddWithValue("@comment_id", c.comment_id);
//                    cmd.Parameters.AddWithValue("@filename", "");
//                    if (c.Files != null)
//                    {
//                        foreach (var file in c.Files)
//                        {
//                            cmd.Parameters[1].Value = file.file_name;
//                            cmd.ExecuteNonQuery();
//                        }
//                    }

//                }
            }
        }


        private static void UpdateVisits(MySqlCommand cmd, After_sales_enquiry o, MySqlTransaction tr)
        {
            if (o.SiteVisits != null)
            {
                foreach (var sv in o.SiteVisits.Where(v => v.sitevisit_id == 0))
                {
                    After_sales_enquiry_sitevisitDAL.Create(sv, tr);
                }
            }
        }


		
		public static void Delete(int enquiry_id)
		{
            MySqlConnection conn = null;
            MySqlTransaction tr = null;
            try
            {
                conn = new MySqlConnection(Properties.Settings.Default.ConnString);
                conn.Open();
                tr = conn.BeginTransaction();
                MySqlCommand cmd = new MySqlCommand("",conn, tr);
                cmd.Parameters.AddWithValue("@id", enquiry_id);

                cmd.CommandText = "DELETE FROM after_sales_enquiry_files WHERE enquiry_id = @id";
                cmd.ExecuteNonQuery();

                cmd.CommandText = "DELETE FROM after_sales_enquiry_comment_files WHERE comment_id IN (SELECT comment_id FROM after_sales_enquiry_comment WHERE enquiry_id = @id)";
                cmd.ExecuteNonQuery();

                cmd.CommandText = "DELETE FROM after_sales_enquiry_comment WHERE enquiry_id = @id";
                cmd.ExecuteNonQuery();

                cmd.CommandText = "DELETE FROM after_sales_enquiry_sitevisit_files WHERE sitevisit_id IN (SELECT sitevisit_id FROM after_sales_enquiry_sitevisit WHERE enquiry_id = @id)";
                cmd.ExecuteNonQuery();

                cmd.CommandText = "DELETE FROM after_sales_enquiry_sitevisit WHERE enquiry_id = @id";
                cmd.ExecuteNonQuery();

                cmd.CommandText = "DELETE FROM after_sales_enquiry WHERE enquiry_id = @id";
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

        /// <summary>
        /// Gets next sequence number for given user/distributor
        /// </summary>
        /// <param name="creator_id"></param>
        /// <returns></returns>
        public static int GetNextReferenceNumber(int distributor_id)
        {
            int result = 0;
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(@"SELECT COALESCE(COUNT(*),0)+1 FROM after_sales_enquiry INNER JOIN userusers ON after_sales_enquiry.created_userid = userusers.useruserid
                                                        WHERE userusers.user_id = @distributor_id AND DATE(datecreated) = @date", conn);
                cmd.Parameters.AddWithValue("@distributor_id", distributor_id);
                cmd.Parameters.AddWithValue("@date", DateTime.Today);
                result = Convert.ToInt32(cmd.ExecuteScalar());
            }
            return result;
        }

        
    }
}
			
			