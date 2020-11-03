using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using erp.Model;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace erp.Model.DAL
{
    public class Download_logDAL
    {
        public static void CreateLog(Download_logs o)
        {
             string insertsql = @"INSERT INTO download_logs (log_date,log_useruserid,log_file,log_cprodid,log_mastid) VALUES(@log_date,@log_useruserid,@log_file,@log_cprodid,@log_mastid)";
            using(var conn=new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                try 
	            {	        
		            MySqlCommand cmd=Utils.GetCommand(insertsql,conn);
                    BuildSqlParameters(cmd,o);
                    cmd.ExecuteNonQuery();
     
                }
	            catch (Exception)
	            {
		
		            throw;
	            }
            }
        }
        public static void Create(List<Download_logs> o)
        {
            string insertsql = @"INSERT INTO download_logs (log_date,log_useruserid,log_file,log_cprodid,log_mastid) VALUES(@log_date,@log_useruserid,@log_file,@log_cprodid,@log_mastid)";
            using(var conn=new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                foreach(var item in o)
                {
                    try
                    {
                        MySqlCommand cmd = Utils.GetCommand(insertsql, conn);
                        BuildSqlParameters(cmd, item);
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception)
                    {
                        
                        throw;
                    }
                }
                
            }
        }

        private static void BuildSqlParameters(MySqlCommand cmd, Download_logs o)
        {
           cmd.Parameters.AddWithValue("@log_date",o.log_date);
           cmd.Parameters.AddWithValue("@log_useruserid",o.log_useruserid);
           cmd.Parameters.AddWithValue("@log_file",o.log_file);
           cmd.Parameters.AddWithValue("@log_cprodid",o.log_crpodid);
           cmd.Parameters.AddWithValue("@log_mastid", o.log_mastid);
        }
    }
}
