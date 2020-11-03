using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace erp.Model.DAL
{
    public class Claims_investigation_reportsDAL
    {
        public static List<Claims_investigation_reports> GetClaimsInvestigationReports()
        {
            var result = new List<Claims_investigation_reports>();

            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand(@"SELECT * FROM claims_ivnestigation_reports ",conn);
                var dr = cmd.ExecuteReader();
                while(dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();

                
            }

            return result;
        }
        

        public static List<Claims_investigation_reports> GetForProduct(int cprod_id, bool reports=false)
        {
            var result = new List<Claims_investigation_reports>();

            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand(@"SELECT * FROM claims_investigation_reports WHERE cprod_id=@cprod_id", conn);
                cmd.Parameters.AddWithValue("@cprod_id", cprod_id);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));

                }
                dr.Close();
                if (reports)
	            {
		            foreach(var r in result )
                    {
                        r.ReportActions = Claims_investigation_reports_actionDAL.GetActionsForReports(r.unique_id,images:true);
                    } 
	            }
                dr.Close();

            }

            return result;
        }

//        public static List<Claims_investigation_reports> GetForProductWithActions(int cprod_id)
//        {
//            var query= (@"SELECT * FROM claims_investigation_reports 
//                                    LEFT JOIN claims_investigation_report_action ON claims_investigation_reports.unique_id = claims_investigation_report_action.report_id
//                                    LEFT JOIN claims_investigation_report_action_images ON claims_investigation_report_action.id = claims_investigation_report_action_images.action_id");
//            var result = new List<Claims_investigation_reports>();
//            using(var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
//            {
//                conn.Open();
//                var cmd= 
//            }
//        }

        public static Claims_investigation_reports GetById(int unique_id)
        {
            Claims_investigation_reports result = null;
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand(@"SELECT *
                                            FROM
                                            claims_investigation_reports
                                            WHERE unique_id = @unique_id", conn);
                cmd.Parameters.AddWithValue("@unique_id", unique_id);
                var dr = cmd.ExecuteReader();
                dr.Read();
                result = GetFromDataReader(dr);
                dr.Close();

            }

            return result;
        }

        public static Claims_investigation_reports GetLastAddedReport()
        {
            Claims_investigation_reports result = null;
            using (var conn=new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand(@"SELECT *
                                            FROM
                                            claims_investigation_reports
                                            ORDER BY unique_id
                                            DESC LIMIT 1", conn);
                var dr = cmd.ExecuteReader();
                dr.Read();
                result=GetFromDataReader(dr);
                dr.Close();

            }
            
            return result;
        }

        

        private static Claims_investigation_reports GetFromDataReader(MySqlDataReader dr)
        {
            var o = new Claims_investigation_reports();
            o.unique_id = (int) dr["unique_id"];
            o.cprod_id = (int)dr["cprod_id"];
            o.investigation = string.Empty + Utilities.GetReaderField(dr, "investigation");
            o.extras = string.Empty + Utilities.GetReaderField(dr, "extras");
            o.created_by= string.Empty + Utilities.GetReaderField(dr, "created_by");
            o.date_created = Utilities.FromDbValue<DateTime>(Utilities.GetReaderField(dr, "date_created"));
            o.date_modify= Utilities.FromDbValue<DateTime>(Utilities.GetReaderField(dr, "date_modify"));
            o.modify_by = string.Empty + Utilities.GetReaderField(dr, "modify_by");

            return o;
        }

        public static void Create(Claims_investigation_reports o)
        {
//            string insertsql = @"INSERT INTO claims_investigation_reports(unique_id,cprod_id,investigation,extras,created_by,date_created,date_modify,modify_by)
//                                VALUES()";
            using(var conn=new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                

                    conn.Open();
                    MySqlCommand cmd = Utils.GetCommand(
                                @"INSERT INTO claims_investigation_reports
                                        (unique_id,cprod_id,investigation,extras,created_by,date_created,date_modify,modify_by)
                                  VALUES(@unique_id,@cprod_id,@investigation,@extras,@created_by,@date_created,@date_modify,@modify_by)", conn);
                    //o.unique_id = Convert.ToInt32(cmd.ExecuteScalar());

                    BuildSqlParameters(cmd, o);
                    cmd.ExecuteNonQuery();
               

            }
        }

        public static void Update(Claims_investigation_reports o)
        {
            using(var conn=new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = Utils.GetCommand(
                    @"UPDATE claims_investigation_reports 
                        SET investigation=@investigation,extras=@extras,date_modify=@date_modify,modify_by=@modify_by
                        WHERE unique_id=@unique_id",conn
                    );
                cmd.Parameters.AddWithValue("@unique_id",o.unique_id);
                cmd.Parameters.AddWithValue("@investigation",o.investigation);
                cmd.Parameters.AddWithValue("@extras",o.extras);
                cmd.Parameters.AddWithValue("@modify_by", o.modify_by);
                cmd.Parameters.AddWithValue("@date_modify",o.date_modify);
                
                cmd.ExecuteNonQuery();
            }

        }

        

        private static void BuildSqlParameters(MySqlCommand cmd, Claims_investigation_reports o)
        {
            cmd.Parameters.AddWithValue("@unique_id", null);
            cmd.Parameters.AddWithValue("@cprod_id", o.cprod_id);
            cmd.Parameters.AddWithValue("@investigation", o.investigation);
            cmd.Parameters.AddWithValue("@extras", o.extras);
            cmd.Parameters.AddWithValue("@created_by",o.created_by);
            cmd.Parameters.AddWithValue("@date_created", o.date_created);
            cmd.Parameters.AddWithValue("@date_modify",null /*o.date_modify*/);
            cmd.Parameters.AddWithValue("@modify_by", ""/*o.modify_by*/);
        } 

        

        public static void Delete(int unique_id)
        {
            using(var conn=new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("DELETE FROM claims_investigation_reports WHERE unique_id=@id", conn);
                cmd.Parameters.AddWithValue("@id", unique_id);
            }
        }

    }
}
