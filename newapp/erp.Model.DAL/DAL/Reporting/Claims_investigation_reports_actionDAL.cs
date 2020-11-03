using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace erp.Model.DAL
{
    public class Claims_investigation_reports_actionDAL
    {
        public static List<Claims_investigation_reports_action> GetClaimsInvestigationReports()
        {
            var result = new List<Claims_investigation_reports_action>();

            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand(@"SELECT * FROM claims_ivnestigation_reports_action ", conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();


            }

            return result;
        }

        public static List<Claims_investigation_reports_action> GetActionsForReports(int unique_id,bool images=false)
        {
            var result = new List<Claims_investigation_reports_action>();

            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand(@"SELECT * FROM claims_investigation_report_action 
                                             WHERE report_id= @unique_id  ", conn);
               cmd.Parameters.AddWithValue("@unique_id", unique_id);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
                if(images)
                {
                    foreach(var a in result)
                    {
                        a.ActionImages = Claims_investigation_report_action_imageDAL.GetImagesForAction(a.id);
                    }
                }
                dr.Close();
            }

            return result;
        }

        public static Claims_investigation_reports_action GetLastAddedAction()
        {
            Claims_investigation_reports_action result = null;
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand(@"SELECT *
                                            FROM
                                            claims_investigation_report_action
                                            
                                            ORDER BY id
                                            DESC LIMIT 1", conn);
               // cmd.Parameters.Add("@unique_id", unique_id);
                var dr = cmd.ExecuteReader();
                dr.Read();
                result = GetFromDataReader(dr);
                dr.Close();

            }

            return result;
        }


        public static void Update(Claims_investigation_reports_action o)
        {
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                string updatesql = @"UPDATE claims_investigation_report_action 
                        SET 
                        report_id=@report_id,comments=@comments
                        WHERE id=@id";
                conn.Open();
                MySqlCommand cmd = Utils.GetCommand(updatesql,conn);
                //BuildSqlParameters(cmd, o);
                cmd.Parameters.AddWithValue("@id",o.id);
                cmd.Parameters.AddWithValue("@report_id", o.report_id);
                cmd.Parameters.AddWithValue("@comments", o.comments);
                cmd.ExecuteNonQuery();
            }

        }



        private static Claims_investigation_reports_action GetFromDataReader(MySqlDataReader dr)
        {
            var o = new Claims_investigation_reports_action();
            o.id = (int)dr["id"];
            o.report_id= (int)dr["report_id"];
            o.comments = string.Empty + Utilities.GetReaderField(dr, "comments");
            
            return o;
        }
        private static void BuildSqlParameters(MySqlCommand cmd, Claims_investigation_reports_action o)
        {
            cmd.Parameters.AddWithValue("@id", null);
            cmd.Parameters.AddWithValue("@report_id", o.report_id);
            cmd.Parameters.AddWithValue("@comments", o.comments);
            
        } 

        public static void Create(Claims_investigation_reports_action o)
        {
            //            string insertsql = @"INSERT INTO claims_investigation_reports(unique_id,cprod_id,investigation,extras,created_by,date_created,date_modify,modify_by)
            //                                VALUES()";
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {


                conn.Open();
                MySqlCommand cmd = Utils.GetCommand(
                            @"INSERT INTO claims_investigation_report_action
                                        (id,report_id,comments)
                                  VALUES(@id,@report_id,@comments)", conn);
                //o.unique_id = Convert.ToInt32(cmd.ExecuteScalar());

                BuildSqlParameters(cmd, o);
                cmd.ExecuteNonQuery();


            }
        }
    }
}
