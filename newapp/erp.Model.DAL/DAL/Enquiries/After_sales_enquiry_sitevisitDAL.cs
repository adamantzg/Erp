
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace erp.Model.DAL
{
    public class After_sales_enquiry_sitevisitDAL
	{
	
		public static List<After_sales_enquiry_sitevisit> GetAll()
		{
			List<After_sales_enquiry_sitevisit> result = new List<After_sales_enquiry_sitevisit>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = Utils.GetCommand("SELECT * FROM after_sales_enquiry_sitevisit", conn);
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}

        public static List<After_sales_enquiry_sitevisit> GetByEnquiry(int enquiry_id)
        {
            List<After_sales_enquiry_sitevisit> result = new List<After_sales_enquiry_sitevisit>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = Utils.GetCommand(GetSelectSql() + " WHERE after_sales_enquiry_sitevisit.enquiry_id = @enquiry_id", conn);
                cmd.Parameters.AddWithValue("@enquiry_id", enquiry_id);
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    After_sales_enquiry_sitevisit sv = GetFromDataReader(dr);
                    result.Add(sv);
                    
                }
                dr.Close();
            }
            return result;
        }

        public static After_sales_enquiry_sitevisit GetByReference(string referenceNumber, int? contractor_id = null)
        {
            After_sales_enquiry_sitevisit result = null;
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = Utils.GetCommand(GetSelectSql() + " WHERE (after_sales_enquiry_sitevisit.contractor_id = @contractor_id OR @contractor_id IS NULL) AND after_sales_enquiry_sitevisit.reference_number = @refnumber", conn);
                cmd.Parameters.AddWithValue("@contractor_id", contractor_id == null ? (object) DBNull.Value : contractor_id.Value);
                cmd.Parameters.AddWithValue("@refnumber", referenceNumber);
                MySqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    result = GetFromDataReader(dr);
                }
                dr.Close();
            }
            return result;
        }

        public static List<After_sales_enquiry_sitevisit> GetByContractor(int contractor_id)
        {
            List<After_sales_enquiry_sitevisit> result = new List<After_sales_enquiry_sitevisit>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = Utils.GetCommand(GetSelectSql() + " WHERE after_sales_enquiry_sitevisit.contractor_id = @contractor_id", conn);
                cmd.Parameters.AddWithValue("@contractor_id", contractor_id);
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
                //Fill complete enquiry object because UI can require many data from id (dealer etc.)
                if(result.Count > 0)
                    FillEnquiries(result);
            }
            return result;
        }

        private static void FillEnquiries(List<After_sales_enquiry_sitevisit> result)
        {
            List<After_sales_enquiry> enquiries = After_sales_enquiryDAL.GetByIds(result.Select(r => r.enquiry_id).ToList());
            foreach (var r in result)
            {
                r.Enquiry = enquiries.Where(e => e.enquiry_id == r.enquiry_id).FirstOrDefault();
            }
        }

        private static string GetSelectSql()
        {
            return @"SELECT after_sales_enquiry_sitevisit.*, after_sales_enquiry_sitevisit_status.status_name, CONCAT(external_user.firstname,' ' , external_user.surname)  AS contractor
                     FROM after_sales_enquiry_sitevisit INNER JOIN after_sales_enquiry_sitevisit_status ON after_sales_enquiry_sitevisit.status_id = after_sales_enquiry_sitevisit_status.status_id
                     INNER JOIN external_user ON after_sales_enquiry_sitevisit.contractor_id = external_user.user_id
                     INNER JOIN after_sales_enquiry ON after_sales_enquiry.enquiry_id = after_sales_enquiry_sitevisit.enquiry_id
                     ";
        }
		
		
		public static After_sales_enquiry_sitevisit GetById(int id)
		{
			After_sales_enquiry_sitevisit result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = Utils.GetCommand(GetSelectSql() + " WHERE after_sales_enquiry_sitevisit.sitevisit_id = @id", conn);
				cmd.Parameters.AddWithValue("@id", id);
                MySqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    result = GetFromDataReader(dr);
                    result.Enquiry = After_sales_enquiryDAL.GetById(result.enquiry_id,false);
                    result.Files = After_sales_enquiry_sitevisit_filesDAL.GetForVisit(id);
                }
                dr.Close();
            }
			return result;
		}

        public static After_sales_enquiry_sitevisit GetLastVisit(int enquiry_id)
        {
            After_sales_enquiry_sitevisit result = null;
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = Utils.GetCommand(GetSelectSql() + " WHERE after_sales_enquiry_sitevisit.enquiry_id = @enquiry_id ORDER BY datecreated DESC LIMIT 1", conn);
                cmd.Parameters.AddWithValue("@enquiry_id", enquiry_id);
                MySqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    result = GetFromDataReader(dr);
                }
                dr.Close();
            }
            return result;
        }
	
		private static After_sales_enquiry_sitevisit GetFromDataReader(MySqlDataReader dr)
		{
			After_sales_enquiry_sitevisit o = new After_sales_enquiry_sitevisit();
		
			o.sitevisit_id =  (int) dr["sitevisit_id"];
			o.enquiry_id =  (int) dr["enquiry_id"];
			o.issue_cause = string.Empty + dr["issue_cause"];
			o.issue_resolution = string.Empty + dr["issue_resolution"];
			o.contractor_id =  (int) dr["contractor_id"];
			o.datecreated = Utilities.FromDbValue<DateTime>(dr["datecreated"]);
			o.created_userid = Utilities.FromDbValue<int>(dr["created_userid"]);
			o.datemodified = Utilities.FromDbValue<DateTime>(dr["datemodified"]);
			o.modified_userid = Utilities.FromDbValue<int>(dr["modified_userid"]);
            o.datecompleted = Utilities.FromDbValue<DateTime>(dr["datecompleted"]);
            o.reference_number = string.Empty + dr["reference_number"];
            o.requestmessage = string.Empty + dr["requestmessage"];
            o.status_id = (int)dr["status_id"];
            o.datevisited = Utilities.FromDbValue<DateTime>(dr["datevisited"]);

            if (Utilities.ColumnExists(dr, "status_name"))
                o.status_name = string.Empty + dr["status_name"];
            if (Utilities.ColumnExists(dr, "contractor"))
                o.contractor = string.Empty + dr["contractor"];
			
			return o;

		}
		
		public static void Create(After_sales_enquiry_sitevisit o, MySqlTransaction tr = null)
        {
            string insertsql = @"INSERT INTO after_sales_enquiry_sitevisit (sitevisit_id,enquiry_id,issue_cause,issue_resolution,contractor_id,datecreated,created_userid,datemodified,modified_userid,
                                 datecompleted,requestmessage,status_id,reference_number,datevisited) 
                                VALUES(@sitevisit_id,@enquiry_id,@issue_cause,@issue_resolution,@contractor_id,@datecreated,@created_userid,@datemodified,@modified_userid,@datecompleted,
                                        @requestmessage,@status_id,@reference_number,@datevisited)";

            MySqlConnection conn;
            if (tr != null)
                conn = tr.Connection;
            else
            {
                conn = new MySqlConnection(Properties.Settings.Default.ConnString);
                conn.Open();
            }
            MySqlCommand cmd = Utils.GetCommand(insertsql, conn);
            if (tr != null)
                cmd.Transaction = tr;
            BuildSqlParameters(cmd, o);
            cmd.ExecuteNonQuery();
            cmd.CommandText = "SELECT sitevisit_id FROM after_sales_enquiry_sitevisit WHERE sitevisit_id = LAST_INSERT_ID()";
            o.sitevisit_id = (int)cmd.ExecuteScalar();

            if (o.Files != null)
            {
                foreach (var file in o.Files)
                {
                    file.sitevisit_id = o.sitevisit_id;
                    After_sales_enquiry_sitevisit_filesDAL.Create(file, tr);
                }
            }

            if (tr == null)
                conn = null;
            
		}
		
		private static void BuildSqlParameters(MySqlCommand cmd, After_sales_enquiry_sitevisit o, bool forInsert = true)
        {
			
			cmd.Parameters.AddWithValue("@sitevisit_id", o.sitevisit_id);
			cmd.Parameters.AddWithValue("@enquiry_id", o.enquiry_id);
			cmd.Parameters.AddWithValue("@issue_cause", o.issue_cause);
			cmd.Parameters.AddWithValue("@issue_resolution", o.issue_resolution);
			cmd.Parameters.AddWithValue("@contractor_id", o.contractor_id);
			cmd.Parameters.AddWithValue("@datecreated", o.datecreated);
			cmd.Parameters.AddWithValue("@created_userid", o.created_userid);
			cmd.Parameters.AddWithValue("@datemodified", o.datemodified);
			cmd.Parameters.AddWithValue("@modified_userid", o.modified_userid);
            cmd.Parameters.AddWithValue("@datecompleted", o.datecompleted);
            cmd.Parameters.AddWithValue("@requestmessage", o.requestmessage);
            cmd.Parameters.AddWithValue("@status_id", o.status_id);
            cmd.Parameters.AddWithValue("@reference_number", o.reference_number);
            cmd.Parameters.AddWithValue("@datevisited", o.datevisited);
		}
		
		public static void Update(After_sales_enquiry_sitevisit o)
		{
            string updatesql = @"UPDATE after_sales_enquiry_sitevisit SET enquiry_id = @enquiry_id,issue_cause = @issue_cause,issue_resolution = @issue_resolution,
                                contractor_id = @contractor_id,datecreated = @datecreated,created_userid = @created_userid,datemodified = @datemodified,
                                modified_userid = @modified_userid, datecompleted = @datecompleted,requestmessage = @requestmessage, status_id = @status_id, 
                                reference_number = @reference_number, datevisited = @datevisited
                                WHERE sitevisit_id = @sitevisit_id";

            MySqlConnection conn = null;
            MySqlTransaction tr = null;
            try
            {
                conn = new MySqlConnection(Properties.Settings.Default.ConnString);
                o.datemodified = DateTime.Now;
                conn.Open();
                tr = conn.BeginTransaction();
                MySqlCommand cmd = Utils.GetCommand(updatesql, conn, tr);
                BuildSqlParameters(cmd, o, false);
                cmd.ExecuteNonQuery();

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
		
		public static void Delete(int sitevisit_id)
		{
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				MySqlCommand cmd = Utils.GetCommand("DELETE FROM after_sales_enquiry_sitevisit WHERE sitevisit_id = @id" , conn);
                cmd.Parameters.AddWithValue("@id", sitevisit_id);
                cmd.ExecuteNonQuery();
            }
		}

        private static void UpdateFiles(MySqlCommand cmd, After_sales_enquiry_sitevisit o)
        {
            if (o.Files != null)
            {
                foreach (var file in o.Files)
                {
                    if (file.datecreated == null)
                        o.datecreated = DateTime.Now;
                }

                cmd.Parameters.Clear();
                cmd.CommandText = "DELETE FROM after_sales_enquiry_sitevisit_files WHERE sitevisit_id = @sitevisit_id";
                cmd.Parameters.AddWithValue("@sitevisit_id", o.sitevisit_id);
                cmd.ExecuteNonQuery();

                
                foreach (var file in o.Files)
                {
                    file.sitevisit_id = o.sitevisit_id;
                    After_sales_enquiry_sitevisit_filesDAL.Create(file, cmd.Transaction);
                }
            }
        }

        public static List<After_sales_enquiry_sitevisit> GetByCreator(int creator_id)
        {
            List<After_sales_enquiry_sitevisit> result = new List<After_sales_enquiry_sitevisit>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = Utils.GetCommand(GetSelectSql() + " WHERE after_sales_enquiry_sitevisit.created_userid = @created_userid", conn);
                cmd.Parameters.AddWithValue("@created_userid", creator_id);
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
                //Fill complete enquiry object because UI can require many data from id (dealer etc.)
                if (result.Count > 0)
                    FillEnquiries(result);
            }
            return result;
        }

        public static List<After_sales_enquiry_sitevisit> GetByDistributor(int user_id)
        {
            List<After_sales_enquiry_sitevisit> result = new List<After_sales_enquiry_sitevisit>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = Utils.GetCommand(GetSelectSql() + " WHERE after_sales_enquiry.created_userid = @created_userid", conn);
                cmd.Parameters.AddWithValue("@created_userid", user_id);
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
                if (result.Count > 0)
                    FillEnquiries(result);
                
            }
            return result;
        }
    }
}
			
			